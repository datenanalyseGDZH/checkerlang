/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
package ch.checkerlang;

import ch.checkerlang.nodes.*;
import ch.checkerlang.values.*;

import java.io.IOException;
import java.io.Reader;
import java.io.StringReader;
import java.util.*;

public class Parser {
    public static Node parse(String script, String filename) throws IOException {
        return parse(new StringReader(script), filename);
    }

    public static Node parse(Reader reader, String filename) throws IOException {
        return parse(new Lexer(reader, filename));
    }

    public static Node parse(Lexer lexer) {
        if (!lexer.hasNext()) return new NodeNull(new SourcePos(lexer.getFilename(), 1, 1));
        Node result = new Parser().parseBareBlock(lexer);
        if (lexer.hasNext())
            throw new SyntaxError("Expected end of input but got '" + lexer.next() + "'", lexer.getPos());
        if (result instanceof NodeReturn) {
            result = ((NodeReturn) result).getExpression();
        } else if (result instanceof NodeBlock) {
            NodeBlock block = (NodeBlock) result;
            List<Node> expressions = block.getExpressions();
            if (expressions.size() > 0) {
                Node lastexpr = expressions.get(expressions.size() - 1);
                if (lastexpr instanceof NodeReturn) {
                    expressions.set(expressions.size() - 1, ((NodeReturn) lastexpr).getExpression());
                }
            }
        }
        return result;
    }

    public Node parseBareBlock(Lexer lexer) {
        NodeBlock block = new NodeBlock(lexer.getPosNext());
        Node expression;
        if (lexer.peek("do", TokenType.Keyword)) {
            expression = parseBlock(lexer);
        } else {
            expression = parseStatement(lexer);
        }
        if (!lexer.hasNext()) {
            return expression;
        }
        block.add(expression);
        while (lexer.matchIf(";", TokenType.Interpunction)) {
            if (!lexer.hasNext()) break;
            if (lexer.peek("do", TokenType.Keyword)) {
                expression = parseBlock(lexer);
            } else {
                expression = parseStatement(lexer);
            }
            block.add(expression);
        }
        if (block.getExpressions().size() == 1 && !block.hasFinally() && !block.hasCatch()) {
            return block.getExpressions().get(0);
        }
        return block;
    }

    public Node parseBlock(Lexer lexer) {
        NodeBlock block = new NodeBlock(lexer.getPosNext());
        lexer.match("do", TokenType.Keyword);
        while (!lexer.peekOne("catch", "finally", "end", TokenType.Keyword)) {
            if (lexer.peek("do", TokenType.Keyword)) {
                block.add(parseBlock(lexer));
            } else {
                block.add(parseStatement(lexer));
            }
            if (lexer.peekOne("catch", "finally", "end", TokenType.Keyword)) break;
            lexer.match(";", TokenType.Interpunction);
            if (lexer.peekOne("catch", "finally", "end", TokenType.Keyword)) break;
        }

        while (lexer.matchIf("catch", TokenType.Keyword)) {
            Node err;
            if (lexer.matchIf("all", TokenType.Identifier)) {
                err = null;
            } else {
                err = parseExpression(lexer);
            }

            Node expr;
            if (lexer.peek("do", TokenType.Keyword)) {
                expr = parseBlock(lexer);
            } else {
                expr = parseStatement(lexer);
            }

            block.addCatch(err, expr);
            if (lexer.peek(";", TokenType.Interpunction)) lexer.eat(1);
        }

        if (lexer.matchIf("finally", TokenType.Keyword)) {
            while (!lexer.peek("end", TokenType.Keyword)) {
                if (lexer.peek("do", TokenType.Keyword)) {
                    block.addFinally(parseBlock(lexer));
                } else {
                    block.addFinally(parseStatement(lexer));
                }
                if (lexer.peek("end", TokenType.Keyword)) break;
                lexer.match(";", TokenType.Interpunction);
            }
        }
        lexer.match("end", TokenType.Keyword);
        if (block.getExpressions().size() == 1 && !block.hasFinally() && !block.hasCatch()) {
            return block.getExpressions().get(0);
        }
        return block;
    }

    public Node parseStatement(Lexer lexer) {
        String comment = "";
        if (lexer.peek().type == TokenType.String && lexer.peekn(2, "def")) {
            comment = lexer.next().value;
        }
        if (lexer.matchIf("require", TokenType.Keyword)) {
            SourcePos pos = lexer.getPos();
            Node modulespec = parseExpression(lexer);
            boolean unqualified = false;
            Map<String, String> symbols = null;
            String name = null;
            if (lexer.matchIf("unqualified", TokenType.Identifier)) {
                unqualified = true;
            } else if (lexer.matchIf("as", TokenType.Keyword)) {
                name = lexer.matchIdentifier();
            } else if (lexer.matchIf("import", TokenType.Identifier, "[", TokenType.Interpunction)) {
                symbols = new HashMap<>();
                while (!lexer.peekn(1, "]", TokenType.Interpunction)) {
                    String symbol = lexer.matchIdentifier();
                    String symbolname = symbol;
                    if (lexer.matchIf("as", TokenType.Keyword)) {
                        symbolname = lexer.matchIdentifier();
                    }
                    symbols.put(symbol, symbolname);
                    if (!lexer.peekn(1, "]", TokenType.Interpunction)) {
                        lexer.match(",", TokenType.Interpunction);
                    }
                }
                lexer.match("]", TokenType.Interpunction);
            }
            return new NodeRequire(modulespec, name, unqualified, symbols, pos);
        }

        if (lexer.matchIf("def", TokenType.Keyword)) {
            SourcePos pos = lexer.getPos();
            Token token;
            if (lexer.matchIf("[", TokenType.Interpunction)) {
                // handle destructuring def
                List<String> identifiers = new ArrayList<>();
                while (!lexer.peek("]", TokenType.Interpunction)) {
                    token = lexer.next();
                    if (token.type == TokenType.Keyword) throw new SyntaxError("Cannot redefine keyword '" + token + "'", token.pos);
                    if (token.type != TokenType.Identifier) throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
                    identifiers.add(token.value);
                    if (!lexer.peek("]", TokenType.Interpunction)) lexer.match(",", TokenType.Interpunction);
                }
                lexer.match("]", TokenType.Interpunction);
                lexer.match("=", TokenType.Operator);
                return new NodeDefDestructuring(identifiers, parseExpression(lexer), comment, pos);
            }
            // handle single var def
            token = lexer.next();            
            if (token.type == TokenType.Identifier && token.value.equals("class") && lexer.peek().type == TokenType.Identifier) {
                token = lexer.next();
                if (token.type == TokenType.Keyword)
                    throw new SyntaxError("Cannot redefine keyword '" + token + "'", token.pos);
                if (token.type != TokenType.Identifier)
                    throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
                lexer.match("do", TokenType.Keyword);
                NodeClass result = new NodeClass(token.value, "", pos);
                while (!lexer.peekn(1, "end", TokenType.Keyword)) {
                    if (lexer.matchIf("def", TokenType.Keyword)) {
                        pos = lexer.getPos();
                        token = lexer.next();
                        if (token.type == TokenType.Keyword)
                            throw new SyntaxError("Cannot redefine keyword '" + token + "'", token.pos);
                        if (token.type != TokenType.Identifier)
                            throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
                        if (lexer.peekn(1, "(", TokenType.Interpunction)) {
                            result.addMember(new NodeDef(token.value, this.parseFn(lexer, pos), comment, pos));
                        } else {
                            lexer.match("=", TokenType.Operator);
                            result.addMember(new NodeDef(token.value, this.parseExpression(lexer), comment, pos));
                        }
                        if (lexer.peekn(1, ";", TokenType.Interpunction)) 
                            lexer.match(";", TokenType.Interpunction);
                    }
                }
                lexer.match("end", TokenType.Keyword);
                return result;
            } else {
                if (token.type == TokenType.Keyword)
                    throw new SyntaxError("Cannot redefine keyword '" + token + "'", token.pos);
                if (token.type != TokenType.Identifier)
                    throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
                if (lexer.peek("(", TokenType.Interpunction)) {
                    return new NodeDef(token.value, parseFn(lexer, pos), comment, pos);
                } else {
                    lexer.match("=", TokenType.Operator);
                    return new NodeDef(token.value, parseExpression(lexer), comment, pos);
                }
            }
        }

        if (lexer.matchIf("for", TokenType.Keyword)) {
            SourcePos pos = lexer.getPos();
            List<String> identifiers = new ArrayList<>();
            if (lexer.matchIf("[", TokenType.Interpunction)) {
                while (!lexer.peekn(1, "]", TokenType.Interpunction)) {
                    Token token = lexer.next();
                    if (token.type != TokenType.Identifier) throw new SyntaxError("Expected identifier in for loop but got '" + token + "'", token.pos);
                    identifiers.add(token.value);
                    if (!lexer.peekn(1, "]", TokenType.Interpunction)) lexer.match(",", TokenType.Interpunction);
                }
                lexer.match("]", TokenType.Interpunction);
            } else {
                Token token = lexer.next();
                if (token.type != TokenType.Identifier)
                    throw new SyntaxError("Expected identifier in for loop but got '" + token + "'", token.pos);
                identifiers.add(token.value);
            }
            lexer.match("in", TokenType.Keyword);
            String what = "values";
            if (lexer.matchIf("keys", TokenType.Identifier)) what = "keys";
            else if (lexer.matchIf("values", TokenType.Identifier)) what = "values";
            else if (lexer.matchIf("entries", TokenType.Identifier)) what = "entries";
            Node expression = parseExpression(lexer);
            if (lexer.peek("do", TokenType.Keyword)) {
                return new NodeFor(identifiers, expression, parseBlock(lexer), what, pos);
            }
            return new NodeFor(identifiers, expression, parseExpression(lexer), what, pos);
        }

        if (lexer.matchIf("while", TokenType.Keyword)) {
            SourcePos pos = lexer.getPos();
            Node expr = parseOrExpr(lexer);
            Node block = parseBlock(lexer);
            return new NodeWhile(expr, block, pos);
        }

        return parseExpression(lexer);
    }

    private Node parseExpression(Lexer lexer) {
        if (lexer.peek("if", TokenType.Keyword)) {
            NodeIf result = new NodeIf(lexer.getPos());
            while (lexer.matchIf("if", TokenType.Keyword) || lexer.matchIf("elif", TokenType.Keyword)) {
                Node condition = parseOrExpr(lexer);
                lexer.match("then", TokenType.Keyword);
                if (lexer.peek("do", TokenType.Keyword)) {
                    result.addIf(condition, parseBlock(lexer));
                } else {
                    result.addIf(condition, parseOrExpr(lexer));
                }
            }

            if (lexer.matchIf("else", TokenType.Keyword)) {
                if (lexer.peek("do", TokenType.Keyword)) {
                    result.setElse(parseBlock(lexer));
                } else {
                    result.setElse(parseOrExpr(lexer));
                }
            }

            return result;
        }

        return parseOrExpr(lexer);
    }

    private Node parseOrExpr(Lexer lexer) {
        Node expr = parseXorExpr(lexer);
        if (lexer.peek("or", TokenType.Keyword)) {
            NodeOr result = new NodeOr(expr, lexer.getPosNext());
            while (lexer.matchIf("or", TokenType.Keyword)) {
                result.addOrClause(parseXorExpr(lexer));
            }

            return result;
        }

        return expr;
    }

    private Node parseXorExpr(Lexer lexer) {
        Node expr = parseAndExpr(lexer);
        if (lexer.matchIf("xor", TokenType.Keyword)) {
            SourcePos pos = lexer.getPos();
            Node expr2 = parseAndExpr(lexer);
            NodeXor result = new NodeXor(pos, expr, expr2);
            while (lexer.matchIf("xor", TokenType.Keyword)) {
                result = new NodeXor(pos, result, parseAndExpr(lexer));
            }

            return result;
        }

        return expr;
    }

    private Node parseAndExpr(Lexer lexer) {
        Node expr = parseNotExpr(lexer);
        if (lexer.peek("and", TokenType.Keyword)) {
            NodeAnd result = new NodeAnd(expr, lexer.getPosNext());
            while (lexer.matchIf("and", TokenType.Keyword)) {
                result.addAndClause(parseNotExpr(lexer));
            }

            return result;
        }

        return expr;
    }

    private Node parseNotExpr(Lexer lexer) {
        if (lexer.matchIf("not", TokenType.Keyword)) {
            SourcePos pos = lexer.getPos();
            return new NodeNot(parseRelExpr(lexer), pos);
        }

        return parseRelExpr(lexer);
    }

    private Node parseRelExpr(Lexer lexer) {
        Node expr = parseAddExpr(lexer);
        Set<String> relops = new HashSet<>();
        relops.add("==");
        relops.add("!=");
        relops.add("<>");
        relops.add("<");
        relops.add("<=");
        relops.add(">");
        relops.add(">=");
        relops.add("is");
        if (!lexer.hasNext() || !relops.contains(lexer.peek().value)) {
            return expr;
        }

        NodeAnd result = new NodeAnd(lexer.getPosNext());
        Node lhs = expr;
        while (lexer.hasNext() && relops.contains(lexer.peek().value)) {
            String relop = lexer.next().value;
            if (relop.equals("is") && lexer.peek().value.equals("not")) {
                relop = "is not";
                lexer.eat(1);
            }
            SourcePos pos = lexer.getPos();
            Node rhs = parseAddExpr(lexer);
            Node cmp = null;
            switch (relop) {
                case "<":
                    cmp = funcCall("less", lhs, rhs, pos);
                    break;

                case "<=":
                    cmp = funcCall("less_equals", lhs, rhs, pos);
                    break;

                case ">":
                    cmp = funcCall("greater", lhs, rhs, pos);
                    break;

                case ">=":
                    cmp = funcCall("greater_equals", lhs, rhs, pos);
                    break;

                case "==":
                case "is":
                    cmp = funcCall("equals", lhs, rhs, pos);
                    break;

                case "<>":
                case "!=":
                case "is not":
                    cmp = funcCall("not_equals", lhs, rhs, pos);
                    break;
            }
            result.addAndClause(cmp);
            lhs = rhs;
        }

        return result.getSimplified();
    }

    private Node parseAddExpr(Lexer lexer) {
        Node expr = parseMulExpr(lexer);
        while (lexer.peekOne("+", "-", TokenType.Operator)) {
            if (lexer.matchIf("+", TokenType.Operator)) {
                SourcePos pos = lexer.getPos();
                expr = funcCall("add", expr, parseMulExpr(lexer), pos);
            } else if (lexer.matchIf("-", TokenType.Operator)) {
                SourcePos pos = lexer.getPos();
                expr = funcCall("sub", expr, parseMulExpr(lexer), pos);
            }
        }

        return expr;
    }

    private Node parseMulExpr(Lexer lexer) {
        Node expr = parseUnaryExpr(lexer);
        while (lexer.peekOne("*", "/", "%", TokenType.Operator)) {
            if (lexer.matchIf("*", TokenType.Operator)) {
                SourcePos pos = lexer.getPos();
                expr = funcCall("mul", expr, parseUnaryExpr(lexer), pos);
            } else if (lexer.matchIf("/", TokenType.Operator)) {
                SourcePos pos = lexer.getPos();
                expr = funcCall("div", expr, parseUnaryExpr(lexer), pos);
            } else if (lexer.matchIf("%", TokenType.Operator)) {
                SourcePos pos = lexer.getPos();
                expr = funcCall("mod", expr, parseUnaryExpr(lexer), pos);
            }
        }

        return expr;
    }

    private Node parseUnaryExpr(Lexer lexer) {
        if (lexer.matchIf("+", TokenType.Operator)) {
            return parsePredExpr(lexer, false);
        }

        if (lexer.matchIf("-", TokenType.Operator)) {
            SourcePos pos = lexer.getPos();
            if (lexer.peek().type == TokenType.Int) {
                return parsePredExpr(lexer, true);
            }
            if (lexer.peek().type == TokenType.Decimal) {
                return parsePredExpr(lexer, true);
            }
            NodeFuncall call = new NodeFuncall(new NodeIdentifier("sub", pos), pos);
            call.addArg("a", new NodeLiteral(new ValueInt(0), pos));
            call.addArg("b", parsePredExpr(lexer, false));
            return call;
        }

        return parsePredExpr(lexer, false);
    }

    private Node parsePredExpr(Lexer lexer, boolean unaryMinus) {
        Node expr = parsePrimaryExpr(lexer, unaryMinus);
        SourcePos pos = lexer.getPosNext();
        if (lexer.matchIf("is", TokenType.Keyword)) {
            if (lexer.matchIf("not", TokenType.Keyword)) {
                if (lexer.matchIf("in")) {
                    return new NodeNot(new NodeIn(expr, this.parsePrimaryExpr(lexer, false), pos), pos);
                } else if (lexer.matchIf("empty", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("is_empty", expr, pos), pos);
                } else if (lexer.matchIf("zero", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("is_zero", expr, pos), pos);
                } else if (lexer.matchIf("negative", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("is_negative", expr, pos), pos);
                } else if (lexer.matchIf("numerical", TokenType.Identifier)) {
                    return new NodeNot(this.collectPredicateMinMaxExact("is_numerical", this.funcCall("string", expr, pos), lexer, pos), pos);
                } else if (lexer.matchIf("alphanumerical", TokenType.Identifier)) {
                    return new NodeNot(this.collectPredicateMinMaxExact("is_alphanumerical", this.funcCall("string", expr, pos), lexer, pos), pos);
                } else if (lexer.matchIf("date", "with", "hour", TokenType.Identifier, TokenType.Identifier, TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("is_valid_date", "str", this.funcCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMddHH"), pos), pos), pos);
                } else if (lexer.matchIf("date", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("is_valid_date", "str", this.funcCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMdd"), pos), pos), pos);
                } else if (lexer.matchIf("time", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("is_valid_time", "str", this.funcCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("HHmm"), pos), pos), pos);
                } else if (lexer.matchIf("string", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("string"), pos), pos), pos);
                } else if (lexer.matchIf("int", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("int"), pos), pos), pos);
                } else if (lexer.matchIf("decimal", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("decimal"), pos), pos), pos);
                } else if (lexer.matchIf("boolean", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("boolean"), pos), pos), pos);
                } else if (lexer.matchIf("pattern", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("pattern"), pos), pos), pos);
                } else if (lexer.matchIf("date", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("date"), pos), pos), pos);
                } else if (lexer.matchIf("null", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("null"), pos), pos), pos);
                } else if (lexer.matchIf("func", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("func"), pos), pos), pos);
                } else if (lexer.matchIf("input", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("input"), pos), pos), pos);
                } else if (lexer.matchIf("output", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("output"), pos), pos), pos);
                } else if (lexer.matchIf("list", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("lsit"), pos), pos), pos);
                } else if (lexer.matchIf("set", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("set"), pos), pos), pos);
                } else if (lexer.matchIf("map", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("map"), pos), pos), pos);
                } else if (lexer.matchIf("object", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("object"), pos), pos), pos);
                } else if (lexer.matchIf("node", TokenType.Identifier)) {
                    return new NodeNot(this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("node"), pos), pos), pos);
                } else {
                    lexer.putback(); // not
                    lexer.putback(); // is
                    return expr;
                }
            } else if (lexer.matchIf("in", TokenType.Keyword)) {
                return new NodeIn(expr, this.parsePrimaryExpr(lexer, false), pos);
            } else if (lexer.matchIf("empty", TokenType.Identifier)) {
                return this.funcCall("is_empty", expr, pos);
            } else if (lexer.matchIf("zero", TokenType.Identifier)) {
                return this.funcCall("is_zero", expr, pos);
            } else if (lexer.matchIf("negative", TokenType.Identifier)) {
                return this.funcCall("is_negative", expr, pos);
            } else if (lexer.matchIf("numerical", TokenType.Identifier)) {
                return this.collectPredicateMinMaxExact("is_numerical", this.funcCall("string", expr, pos), lexer, pos);
            } else if (lexer.matchIf("alphanumerical", TokenType.Identifier)) {
                return this.collectPredicateMinMaxExact("is_alphanumerical", this.funcCall("string", expr, pos), lexer, pos);
            } else if (lexer.matchIf("date", "with", "hour", TokenType.Identifier, TokenType.Identifier, TokenType.Identifier)) {
                return this.funcCall("is_valid_date", "str", this.funcCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMddHH"), pos), pos);
            } else if (lexer.matchIf("date", TokenType.Identifier)) {
                return this.funcCall("is_valid_date", "str", this.funcCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMdd"), pos), pos);
            } else if (lexer.matchIf("time", TokenType.Identifier)) {
                return this.funcCall("is_valid_time", "str", this.funcCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("HHmm"), pos), pos);
            } else if (lexer.matchIf("string", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("string"), pos), pos);
            } else if (lexer.matchIf("int", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("int"), pos), pos);
            } else if (lexer.matchIf("decimal", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("decimal"), pos), pos);
            } else if (lexer.matchIf("boolean", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("boolean"), pos), pos);
            } else if (lexer.matchIf("pattern", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("pattern"), pos), pos);
            } else if (lexer.matchIf("date", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("date"), pos), pos);
            } else if (lexer.matchIf("null", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("null"), pos), pos);
            } else if (lexer.matchIf("func", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("func"), pos), pos);
            } else if (lexer.matchIf("input", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("input"), pos), pos);
            } else if (lexer.matchIf("output", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("output"), pos), pos);
            } else if (lexer.matchIf("list", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("list"), pos), pos);
            } else if (lexer.matchIf("set", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("set"), pos), pos);
            } else if (lexer.matchIf("map", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("map"), pos), pos);
            } else if (lexer.matchIf("object", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("object"), pos), pos);
            } else if (lexer.matchIf("node", TokenType.Identifier)) {
                return this.funcCall("equals", this.funcCall("type", expr, pos), new NodeLiteral(new ValueString("node"), pos), pos);
            }
            lexer.putback(); // is
            return expr;
        } else if (lexer.matchIf("not", "in", TokenType.Keyword, TokenType.Keyword)) {
            return new NodeNot(new NodeIn(expr, this.parsePrimaryExpr(lexer, false), pos), pos);
        } else if (lexer.matchIf("in", TokenType.Keyword)) {
            return new NodeIn(expr, this.parsePrimaryExpr(lexer, false), pos);
        } else if (lexer.matchIf("starts", "not", "with", TokenType.Identifier, TokenType.Keyword, TokenType.Identifier)) {
            return new NodeNot(this.funcCall("starts_with", "str", expr, "part", this.parsePrimaryExpr(lexer, false), pos), pos);
        } else if (lexer.matchIf("starts", "with", TokenType.Identifier, TokenType.Identifier)) {
            return this.funcCall("starts_with", "str", expr, "part", this.parsePrimaryExpr(lexer, false), pos);
        } else if (lexer.matchIf("ends", "not", "with", TokenType.Identifier, TokenType.Keyword, TokenType.Identifier)) {
            return new NodeNot(this.funcCall("ends_with", "str", expr, "part", this.parsePrimaryExpr(lexer, false), pos), pos);
        } else if (lexer.matchIf("ends", "with", TokenType.Identifier, TokenType.Identifier)) {
            return this.funcCall("ends_with", "str", expr, "part", this.parsePrimaryExpr(lexer, false), pos);
        } else if (lexer.matchIf("contains", "not", TokenType.Identifier, TokenType.Keyword)) {
            return new NodeNot(this.funcCall("contains", "obj", expr, "part", this.parsePrimaryExpr(lexer, false), pos), pos);
        } else if (lexer.matchIf("contains", TokenType.Identifier)) {
            return this.funcCall("contains", "obj", expr, "part", this.parsePrimaryExpr(lexer, false), pos);
        } else if (lexer.matchIf("matches", "not", TokenType.Identifier, TokenType.Keyword)) {
            return new NodeNot(this.funcCall("matches", "str", expr, "pattern", this.parsePrimaryExpr(lexer, false), pos), pos);
        } else if (lexer.matchIf("matches", TokenType.Identifier)) {
            return this.funcCall("matches", "str", expr, "pattern", this.parsePrimaryExpr(lexer, false), pos);
        }

        return expr;
    }

    private Node collectPredicateMinMaxExact(String fn, Node expr, Lexer lexer, SourcePos pos) {
        Node min_len = new NodeLiteral(new ValueInt(1), pos);
        Node max_len = new NodeLiteral(new ValueInt(9999), pos);
        if (lexer.matchIf("min_len", TokenType.Identifier)) {
            min_len = parsePrimaryExpr(lexer, false);
        }

        if (lexer.matchIf("max_len", TokenType.Identifier)) {
            max_len = parsePrimaryExpr(lexer, false);
        }

        if (lexer.matchIf("exact_len", TokenType.Identifier)) {
            min_len = max_len = parsePrimaryExpr(lexer, false);
        }

        return funcCall(fn, "str", expr, "min", min_len, "max", max_len, pos);
    }

    private Node parsePrimaryExpr(Lexer lexer, boolean unaryMinus) {
        if (!lexer.hasNext()) throw new SyntaxError("Unexpected end of input", lexer.getPos());

        Node result;

        Token token = lexer.next();
        if (token.value.equals("(")) {
            result = parseBareBlock(lexer);
            lexer.match(")", TokenType.Interpunction);
            return derefOrCallOrInvoke(lexer, result);
        }

        switch (token.type) {
            case Identifier:
                result = new NodeIdentifier(token.value, token.pos);
                if (lexer.matchIf("=", TokenType.Operator)) {
                    result = new NodeAssign(token.value, parseExpression(lexer), token.pos);
                } else if (lexer.matchIf("+=", TokenType.Operator)) {
                    Node value = parseExpression(lexer);
                    result = new NodeAssign(token.value, funcCall("add", result, value, token.pos), token.pos);
                    break;
                } else if (lexer.matchIf("-=", TokenType.Operator)) {
                    Node value = parseExpression(lexer);
                    result = new NodeAssign(token.value, funcCall("sub", result, value, token.pos), token.pos);
                    break;
                } else if (lexer.matchIf( "*=", TokenType.Operator)) {
                    Node value = parseExpression(lexer);
                    result = new NodeAssign(token.value, funcCall("mul", result, value, token.pos), token.pos);
                    break;
                } else if (lexer.matchIf( "/=", TokenType.Operator)) {
                    Node value = parseExpression(lexer);
                    result = new NodeAssign(token.value, funcCall("div", result, value, token.pos), token.pos);
                    break;
                } else if (lexer.matchIf( "%=", TokenType.Operator)) {
                    Node value = parseExpression(lexer);
                    result = new NodeAssign(token.value, funcCall("mod", result, value, token.pos), token.pos);
                    break;
                } else {
                    result = derefOrCallOrInvoke(lexer, result);
                }
                break;
            case String:
                result = new NodeLiteral(new ValueString(token.value), token.pos);
                result = derefOrInvoke(lexer, result);
                break;
            case Int:
                result = new NodeLiteral(new ValueInt(Long.parseLong(token.value) * (unaryMinus ? -1 : 1)), token.pos);
                result = invoke(lexer, result);
                break;
            case Decimal:
                result = new NodeLiteral(new ValueDecimal(Double.parseDouble(token.value) * (unaryMinus ? -1 : 1)), token.pos);
                result = invoke(lexer, result);
                break;
            case Boolean:
                result = new NodeLiteral(ValueBoolean.from(token.value.equals("TRUE")), token.pos);
                result = invoke(lexer, result);
                break;
            case Pattern:
                result = new NodeLiteral(new ValuePattern(token.value.substring(2, token.value.length() - 2)), token.pos);
                result = invoke(lexer, result);
                break;
            default:
                if (token.value.equals("fn") && token.type == TokenType.Keyword) {
                    result = parseFn(lexer, token.pos);
                } else if (token.value.equals("break") && token.type == TokenType.Keyword) {
                    result = new NodeBreak(token.pos);
                } else if (token.value.equals("continue") && token.type == TokenType.Keyword) {
                    result = new NodeContinue(token.pos);
                } else if (token.value.equals("return") && token.type == TokenType.Keyword) {
                    if (lexer.peekn(1, ";", TokenType.Interpunction)) result = new NodeReturn(null, token.pos);
                    else result = new NodeReturn(parseExpression(lexer), token.pos);
                } else if (token.value.equals("error") && token.type == TokenType.Keyword) {
                    result = new NodeError(parseExpression(lexer), token.pos);
                } else if (token.value.equals("do") && token.type == TokenType.Keyword) {
                    lexer.putback();
                    result = parseBlock(lexer);
                } else if (token.value.equals("[") && token.type == TokenType.Interpunction) {
                    result = parseListLiteral(lexer, token);
                    if (lexer.peek("=", TokenType.Operator)) {
                        List<String> identifiers = new ArrayList<>();
                        for (Node item : ((NodeList) result).getItems()) {
                            if (!(item instanceof NodeIdentifier)) throw new SyntaxError("Destructuring assign expected identifier but got " + item, token.pos);
                            identifiers.add(((NodeIdentifier) item).getValue());
                        }
                        lexer.match("=", TokenType.Operator);
                        result = new NodeAssignDestructuring(identifiers, parseExpression(lexer), token.pos);
                    }
                } else if (token.value.equals("<<") && token.type == TokenType.Interpunction) {
                    result = parseSetLiteral(lexer, token);
                } else if (token.value.equals("<<<") && token.type == TokenType.Interpunction) {
                    result = parseMapLiteral(lexer, token);
                } else if (token.value.equals("<*") && token.type == TokenType.Interpunction) {
                    result = parseObjectLiteral(lexer, token);
                } else if (token.value.equals("...") && token.type == TokenType.Interpunction) {
                    token = lexer.next();
                    if (token.value.equals("[") && token.type == TokenType.Interpunction) {
                        result = parseListLiteral(lexer, token);
                    } else if (token.value.equals("<<<") && token.type == TokenType.Interpunction) {
                        result = parseMapLiteral(lexer, token);
                    } else if (token.type == TokenType.Identifier) {
                        result = new NodeIdentifier(token.value, token.pos);
                    } else {
                        throw new SyntaxError("Spread operator only allowed with identifiers, list and map literals", token.pos);
                    }
                    result = new NodeSpread(result, token.pos);
                } else {
                    throw new SyntaxError("Invalid syntax at '" + token + "'", token.pos);
                }
                break;
        }

        return result;
    }

    private Node parseListLiteral(Lexer lexer, Token token) {
        if (lexer.matchIf("]", TokenType.Interpunction)) {
            return derefOrInvoke(lexer, new NodeList(token.pos));
        } else {
            Node expr = parseExpression(lexer);
            if (lexer.matchIf("for", TokenType.Keyword)) {
                String identifier = lexer.matchIdentifier();
                lexer.match("in", TokenType.Keyword);
                String what = null;
                if (lexer.matchIf("keys", TokenType.Identifier)) what = "keys";
                else if (lexer.matchIf("values", TokenType.Identifier)) what = "values";
                else if (lexer.matchIf("entries", TokenType.Identifier)) what = "entries";
                Node listExpr = parseOrExpr(lexer);
                if (lexer.matchIf("for", TokenType.Keyword)) {
                    String identifier2 = lexer.matchIdentifier();
                    lexer.match("in", TokenType.Keyword);
                    String what2 = null;
                    if (lexer.matchIf("keys", TokenType.Identifier)) what2 = "keys";
                    else if (lexer.matchIf("values", TokenType.Identifier)) what2 = "values";
                    else if (lexer.matchIf("entries", TokenType.Identifier)) what2 = "entries";
                    Node listExpr2 = parseOrExpr(lexer);
                    NodeListComprehensionProduct comprehension = new NodeListComprehensionProduct(expr, identifier, listExpr, what, identifier2, listExpr2, what2, token.pos);
                    if (lexer.matchIf("if", TokenType.Keyword)) {
                        comprehension.setCondition(parseOrExpr(lexer));
                    }
                    lexer.match("]", TokenType.Interpunction);
                    return derefOrInvoke(lexer, comprehension);
                } else if (lexer.matchIf("also", "for", TokenType.Keyword)) {
                    String identifier2 = lexer.matchIdentifier();
                    lexer.match("in", TokenType.Keyword);
                    String what2 = null;
                    if (lexer.matchIf("keys", TokenType.Identifier)) what2 = "keys";
                    else if (lexer.matchIf("values", TokenType.Identifier)) what2 = "values";
                    else if (lexer.matchIf("entries", TokenType.Identifier)) what2 = "entries";
                    Node listExpr2 = parseOrExpr(lexer);
                    NodeListComprehensionParallel comprehension = new NodeListComprehensionParallel(expr, identifier, listExpr, what, identifier2, listExpr2, what2, token.pos);
                    if (lexer.matchIf("if", TokenType.Keyword)) {
                        comprehension.setCondition(parseOrExpr(lexer));
                    }
                    lexer.match("]", TokenType.Interpunction);
                    return derefOrInvoke(lexer, comprehension);
                } else {
                    NodeListComprehension comprehension = new NodeListComprehension(expr, identifier, listExpr, what, token.pos);
                    if (lexer.matchIf("if", TokenType.Keyword)) {
                        comprehension.setCondition(parseOrExpr(lexer));
                    }
                    lexer.match("]", TokenType.Interpunction);
                    return derefOrInvoke(lexer, comprehension);
                }
            } else {
                NodeList list = new NodeList(token.pos);
                while (!lexer.peek("]", TokenType.Interpunction)) {
                    list.addItem(expr);
                    expr = null;
                    if (!lexer.peek("]", TokenType.Interpunction)) {
                        lexer.match(",", TokenType.Interpunction);
                        if (!lexer.peek("]", TokenType.Interpunction)) {
                            expr = parseExpression(lexer);
                        }
                    }
                }
                if (expr != null) list.addItem(expr);
                lexer.match("]", TokenType.Interpunction);
                return derefOrInvoke(lexer, list);
            }
        }
    }

    private Node parseSetLiteral(Lexer lexer, Token token) {
        if (lexer.matchIf(">>", TokenType.Interpunction)) {
            return derefOrInvoke(lexer, new NodeSet(token.pos));
        } else {
            Node expr = parseExpression(lexer);
            if (lexer.matchIf("for", TokenType.Keyword)) {
                String identifier = lexer.matchIdentifier();
                lexer.match("in", TokenType.Keyword);
                String what = null;
                if (lexer.matchIf("keys", TokenType.Identifier)) what = "keys";
                else if (lexer.matchIf("values", TokenType.Identifier)) what = "values";
                else if (lexer.matchIf("entries", TokenType.Identifier)) what = "entries";
                Node listExpr = parseOrExpr(lexer);
                if (lexer.matchIf("for", TokenType.Keyword)) {
                    String identifier2 = lexer.matchIdentifier();
                    lexer.match("in", TokenType.Keyword);
                    String what2 = null;
                    if (lexer.matchIf("keys", TokenType.Identifier)) what2 = "keys";
                    else if (lexer.matchIf("values", TokenType.Identifier)) what2 = "values";
                    else if (lexer.matchIf("entries", TokenType.Identifier)) what2 = "entries";
                    Node listExpr2 = parseOrExpr(lexer);
                    NodeSetComprehensionProduct comprehension = new NodeSetComprehensionProduct(expr, identifier, listExpr, what, identifier2, listExpr2, what2, token.pos);
                    if (lexer.matchIf("if", TokenType.Keyword)) {
                        comprehension.setCondition(parseOrExpr(lexer));
                    }
                    lexer.match(">>", TokenType.Interpunction);
                    return derefOrInvoke(lexer, comprehension);
                } else if (lexer.matchIf("also", "for", TokenType.Keyword)) {
                    String identifier2 = lexer.matchIdentifier();
                    lexer.match("in", TokenType.Keyword);
                    String what2 = null;
                    if (lexer.matchIf("keys", TokenType.Identifier)) what2 = "keys";
                    else if (lexer.matchIf("values", TokenType.Identifier)) what2 = "values";
                    else if (lexer.matchIf("entries", TokenType.Identifier)) what2 = "entries";
                    Node listExpr2 = parseOrExpr(lexer);
                    NodeSetComprehensionParallel comprehension = new NodeSetComprehensionParallel(expr, identifier, listExpr, what, identifier2, listExpr2, what2, token.pos);
                    if (lexer.matchIf("if", TokenType.Keyword)) {
                        comprehension.setCondition(parseOrExpr(lexer));
                    }
                    lexer.match(">>", TokenType.Interpunction);
                    return derefOrInvoke(lexer, comprehension);
                } else {
                    NodeSetComprehension comprehension = new NodeSetComprehension(expr, identifier, listExpr, what, token.pos);
                    if (lexer.matchIf("if", TokenType.Keyword)) {
                        comprehension.setCondition(this.parseOrExpr(lexer));
                    }
                    lexer.match(">>", TokenType.Interpunction);
                    return this.derefOrInvoke(lexer, comprehension);
                }
            } else {
                NodeSet set = new NodeSet(token.pos);
                set.addItem(expr);
                if (!lexer.peek(">>", TokenType.Interpunction)) {
                    lexer.match(",", TokenType.Interpunction);
                }
                while (!lexer.peek(">>", TokenType.Interpunction)) {
                    set.addItem(parseExpression(lexer));
                    if (!lexer.peek(">>", TokenType.Interpunction)) {
                        lexer.match(",", TokenType.Interpunction);
                    }
                }
                lexer.match(">>", TokenType.Interpunction);
                return derefOrInvoke(lexer, set);
            }
        }
    }

    private Node parseMapLiteral(Lexer lexer, Token token) {
        if (lexer.matchIf(">>>", TokenType.Interpunction)) {
            return derefOrInvoke(lexer, new NodeMap(token.pos));
        } else {
            Node key = parseExpression(lexer);
            lexer.match("=>", TokenType.Interpunction);
            Node value = parseExpression(lexer);
            if (lexer.matchIf("for", TokenType.Keyword)) {
                String identifier = lexer.matchIdentifier();
                lexer.match("in", TokenType.Keyword);
                String what = null;
                if (lexer.matchIf("keys", TokenType.Identifier)) what = "keys";
                else if (lexer.matchIf("values", TokenType.Identifier)) what = "values";
                else if (lexer.matchIf("entries", TokenType.Identifier)) what = "entries";
                Node listExpr = parseOrExpr(lexer);
                NodeMapComprehension comprehension = new NodeMapComprehension(key, value, identifier, listExpr, what, token.pos);
                if (lexer.matchIf("if", TokenType.Keyword)) {
                    comprehension.setCondition(this.parseOrExpr(lexer));
                }
                lexer.match(">>>", TokenType.Interpunction);
                return this.derefOrInvoke(lexer, comprehension);
            } else {
                NodeMap map = new NodeMap(token.pos);
                if (key instanceof NodeIdentifier) {
                    key = new NodeLiteral(new ValueString(((NodeIdentifier) key).getValue()), key.getSourcePos());
                }
                map.addKeyValue(key, value);
                if (!lexer.peek(">>>", TokenType.Interpunction)) {
                    lexer.match(",", TokenType.Interpunction);
                }
                while (!lexer.peek(">>>", TokenType.Interpunction)) {
                    key = parseExpression(lexer);
                    if (key instanceof NodeIdentifier) {
                        key = new NodeLiteral(new ValueString(((NodeIdentifier) key).getValue()), key.getSourcePos());
                    }
                    lexer.match("=>", TokenType.Interpunction);
                    value = parseExpression(lexer);
                    map.addKeyValue(key, value);
                    if (!lexer.peek(">>>", TokenType.Interpunction)) {
                        lexer.match(",", TokenType.Interpunction);
                    }
                }
                lexer.match(">>>", TokenType.Interpunction);
                return derefOrInvoke(lexer, map);
            }
        }
    }

    private Node parseObjectLiteral(Lexer lexer, Token token) {
        NodeObject obj = new NodeObject(token.pos);
        while (!lexer.peekn(1, "*>", TokenType.Interpunction)) {
            String key = lexer.matchIdentifier();
            if (lexer.peekn(1, "(", TokenType.Interpunction)) {
                NodeLambda fn = this.parseFn(lexer, lexer.getPos());
                obj.addKeyValue(key, fn);
            } else {
                lexer.match("=", TokenType.Operator);
                Node value = this.parseExpression(lexer);
                obj.addKeyValue(key, value);
            }
            if (!lexer.peekn(1, "*>", TokenType.Interpunction)) {
                lexer.match(",", TokenType.Interpunction);
            }
        }
        lexer.match("*>", TokenType.Interpunction);
        return this.derefOrInvoke(lexer, obj);
    }

    private NodeLambda parseFn(Lexer lexer, SourcePos pos) {
        NodeLambda lambda = new NodeLambda(pos);
        lexer.match("(", TokenType.Interpunction);
        while (!lexer.matchIf(")", TokenType.Interpunction)) {
            Token token = lexer.next();
            if (token.type == TokenType.Keyword)
                throw new SyntaxError("Cannot use keyword '" + token + "' as parameter name", token.pos);
            if (token.type != TokenType.Identifier)
                throw new SyntaxError("Expected parameter name but got '" + token + "'", token.pos);
            String argname = token.value;
            Node defvalue = null;
            if (lexer.matchIf("=", TokenType.Operator)) {
                defvalue = parseExpression(lexer);
            }
            lambda.addArg(argname, defvalue);
            if (argname.endsWith("...") && !lexer.peek(")", TokenType.Interpunction)) {
                throw new SyntaxError("Rest argument " + argname + " must be last argument", token.pos);
            }
            if (!lexer.peek(")", TokenType.Interpunction)) lexer.match(",", TokenType.Interpunction);
        }

        if (lexer.peek("do", TokenType.Keyword)) {
            lambda.setBody(parseBlock(lexer));
        } else {
            lambda.setBody(parseExpression(lexer));
        }
        return lambda;
    }

    private Node _invoke(Lexer lexer, Node node) {
        if (lexer.matchIf("!>", TokenType.Operator)) {
            Node fn;
            if (lexer.matchIf("(", TokenType.Interpunction, "fn", TokenType.Keyword)) {
                fn = parseFn(lexer, lexer.getPos());
                lexer.match(")", TokenType.Interpunction);
            } else {
                fn = new NodeIdentifier(lexer.matchIdentifier(), lexer.getPos());
                while (lexer.matchIf("->", TokenType.Operator)) {
                    fn = new NodeDeref(fn, new NodeLiteral(new ValueString(lexer.matchIdentifier()), lexer.getPos()), null, lexer.getPos());
                }
            }
            NodeFuncall call = new NodeFuncall(fn, lexer.getPos());
            call.addArg(null, node);
            lexer.match("(", TokenType.Interpunction);
            while (!lexer.peekn(1, ")", TokenType.Interpunction)) {
                if (lexer.peek().type == TokenType.Identifier && lexer.peekn(2, "=", TokenType.Operator)) {
                    String name = lexer.matchIdentifier();
                    lexer.match("=", TokenType.Operator);
                    call.addArg(name, parseExpression(lexer));
                } else {
                    call.addArg(null, parseExpression(lexer));
                }
                if (!lexer.peekn(1, ")", TokenType.Interpunction)) lexer.match(",", TokenType.Interpunction);
            }
            lexer.eat(1);
            node = call;
        }
        return node;
    }

    private Node _call(Lexer lexer, Node node) {
        if (lexer.matchIf("(", TokenType.Interpunction)) {
            NodeFuncall call = new NodeFuncall(node, lexer.getPos());
            while (!lexer.peekn(1, ")", TokenType.Interpunction)) {
                if (lexer.peek().type == TokenType.Identifier && lexer.peekn(2, "=", TokenType.Operator)) {
                    String name = lexer.matchIdentifier();
                    lexer.match("=", TokenType.Operator);
                    call.addArg(name, this.parseExpression(lexer));
                } else {
                    call.addArg(null, this.parseExpression(lexer));
                }
                if (!lexer.peekn(1, ")", TokenType.Interpunction)) lexer.match(",", TokenType.Interpunction);
            }
            lexer.eat(1);
            node = call;
        }
        return node;
    }

    private static class DerefResult {
        public Node node;
        public boolean interrupt;
    }

    private DerefResult _deref(Lexer lexer, Node node) {
        DerefResult result = new DerefResult();
        if (lexer.matchIf("->", TokenType.Operator)) {
            SourcePos pos = lexer.getPos();
            String identifier = lexer.matchIdentifier();
            Node index = new NodeLiteral(new ValueString(identifier), pos);
            if (lexer.matchIf("=", TokenType.Operator)) {
                Node value = this.parseExpression(lexer);
                result.node = new NodeDerefAssign(node, index, value, pos);
                result.interrupt = true;
            } else if (lexer.matchIf("(", TokenType.Interpunction)) {
                NodeDerefInvoke n = new NodeDerefInvoke(node, identifier, pos);
                while (!lexer.peekn(1, ")", TokenType.Interpunction)) {
                    if (lexer.peek().type == TokenType.Identifier && lexer.peekn(2, "=", TokenType.Operator)) {
                        String name = lexer.matchIdentifier();
                        lexer.match("=", TokenType.Operator);
                        n.addArg(name, this.parseExpression(lexer));
                    } else {
                        n.addArg(null, this.parseExpression(lexer));
                    }
                    if (!lexer.peekn(1, ")", TokenType.Interpunction)) lexer.match(",", TokenType.Interpunction);
                }
                lexer.eat(1);
                result.node = n;
            } else if (lexer.matchIf("+=", TokenType.Operator)) {
                Node value = this.parseExpression(lexer);
                result.node = new NodeDerefAssign(node, index, this.funcCall("add", new NodeDeref(node, index, null, pos), value, pos), pos);
                result.interrupt = true;
            } else if (lexer.matchIf( "-=", TokenType.Operator)) {
                Node value = this.parseExpression(lexer);
                result.node = new NodeDerefAssign(node, index, this.funcCall("sub", new NodeDeref(node, index, null, pos), value, pos), pos);
                result.interrupt = true;
            } else if (lexer.matchIf("*=", TokenType.Operator)) {
                Node value = this.parseExpression(lexer);
                result.node = new NodeDerefAssign(node, index, this.funcCall("mul", new NodeDeref(node, index, null, pos), value, pos), pos);
                result.interrupt = true;
            } else if (lexer.matchIf("/=", TokenType.Operator)) {
                Node value = this.parseExpression(lexer);
                result.node = new NodeDerefAssign(node, index, this.funcCall("div", new NodeDeref(node, index, null, pos), value, pos), pos);
                result.interrupt = true;
            } else if (lexer.matchIf("%=", TokenType.Operator)) {
                Node value = this.parseExpression(lexer);
                result.node = new NodeDerefAssign(node, index, this.funcCall("mod", new NodeDeref(node, index, null, pos), value, pos), pos);
                result.interrupt = true;
            } else {
                result.node = new NodeDeref(node, index, null, pos);
            }
        } else if (lexer.matchIf("[", TokenType.Interpunction)) {
            SourcePos pos = lexer.getPos();
            Node index = this.parseExpression(lexer);
            if (lexer.matchIf("to", TokenType.Identifier)) {
                Node end;
                if (lexer.matchIf("*", TokenType.Operator)) {
                    end = null;
                } else {
                    end = this.parseExpression(lexer);
                }
                result.node = new NodeDerefSlice(node, index, end, pos);
                lexer.match("]", TokenType.Interpunction);
            } else {
                Node defaultValue = null;
                if (lexer.matchIf(",", TokenType.Interpunction)) {
                    defaultValue = this.parseExpression(lexer);
                }
                if (lexer.matchIf("]", TokenType.Interpunction, "=", TokenType.Operator)) {
                    Node value = this.parseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, value, pos);
                    result.interrupt = true;
                } else if (lexer.matchIf("]", TokenType.Interpunction, "+=", TokenType.Operator)) {
                    Node value = this.parseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.funcCall("add", new NodeDeref(node, index, defaultValue, pos), value, pos), pos);
                    result.interrupt = true;
                } else if (lexer.matchIf("]", TokenType.Interpunction, "-=", TokenType.Operator)) {
                    Node value = this.parseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.funcCall("sub", new NodeDeref(node, index, defaultValue, pos), value, pos), pos);
                    result.interrupt = true;
                } else if (lexer.matchIf("]", TokenType.Interpunction, "*=", TokenType.Operator)) {
                    Node value = this.parseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.funcCall("mul", new NodeDeref(node, index, defaultValue, pos), value, pos), pos);
                    result.interrupt = true;
                } else if (lexer.matchIf("]", TokenType.Interpunction, "/=", TokenType.Operator)) {
                    Node value = this.parseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.funcCall("div", new NodeDeref(node, index, defaultValue, pos), value, pos), pos);
                    result.interrupt = true;
                } else if (lexer.matchIf("]", TokenType.Interpunction, "%=", TokenType.Operator)) {
                    Node value = this.parseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.funcCall("mod", new NodeDeref(node, index, defaultValue, pos), value, pos), pos);
                    result.interrupt = true;
                } else {
                    result.node = new NodeDeref(node, index, defaultValue, pos);
                    lexer.match("]", TokenType.Interpunction);
                }
            }
        }
        return result;
    }

    public Node derefOrCallOrInvoke(Lexer lexer, Node node) {
        while (lexer.peekn(1, "!>", TokenType.Operator) || lexer.peekn(1, "[", TokenType.Interpunction) || lexer.peekn(1, "(", TokenType.Interpunction) || lexer.peekn(1, "->", TokenType.Operator)) {
            if (lexer.peekn(1, "!>", TokenType.Operator)) {
                node = this._invoke(lexer, node);
            } else if (lexer.peekn(1, "(", TokenType.Interpunction)) {
                node = this._call(lexer, node);
            } else if (lexer.peekn(1, "[", TokenType.Interpunction) || lexer.peekn(1, "->", TokenType.Operator)) {
                DerefResult result = this._deref(lexer, node);
                node = result.node;
                if (result.interrupt) break;
            }
        }
        return node;
    }

    public Node derefOrInvoke(Lexer lexer, Node node) {
        while (lexer.peekn(1, "!>", TokenType.Operator) || lexer.peekn(1, "[", TokenType.Interpunction) || lexer.peekn(1, "->", TokenType.Operator)) {
            if (lexer.peekn(1, "!>", TokenType.Operator)) {
                node = this._invoke(lexer, node);
            } else if (lexer.peekn(1, "[", TokenType.Interpunction) || lexer.peekn(1, "->", TokenType.Operator)) {
                DerefResult result = this._deref(lexer, node);
                node = result.node;
                if (result.interrupt) break;
            }
        }
        return node;
    }

    public Node invoke(Lexer lexer, Node node) {
        while (lexer.peekn(1, "!>", TokenType.Operator)) {
            node = this._invoke(lexer, node);
        }
        return node;
    }

    private Node funcCall(String fn, Node expr, SourcePos pos) {
        NodeFuncall result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
        result.addArg("obj", expr);
        return result;
    }

    private Node funcCall(String fn, Node exprA, Node exprB, SourcePos pos) {
        NodeFuncall result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
        result.addArg("a", exprA);
        result.addArg("b", exprB);
        return result;
    }

    private Node funcCall(String fn, String a, Node exprA, String b, Node exprB, SourcePos pos) {
        NodeFuncall result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
        result.addArg(a, exprA);
        result.addArg(b, exprB);
        return result;
    }

    private Node funcCall(String fn, String a, Node exprA, String b, Node exprB, String c, Node exprC, SourcePos pos) {
        NodeFuncall result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
        result.addArg(a, exprA);
        result.addArg(b, exprB);
        result.addArg(c, exprC);
        return result;
    }

}
