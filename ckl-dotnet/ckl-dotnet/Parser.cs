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
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace CheckerLang
{
    public class Parser
    {
        public static Node Parse(string script, string filename)
        {
            return Parse(new StringReader(script), filename);
        }

        public static Node Parse(TextReader reader, string filename)
        {
            return Parse(new Lexer(reader, filename));
        }

        public static Node Parse(Lexer lexer)
        {
            if (!lexer.HasNext()) return new NodeNull(new SourcePos(lexer.GetFilename(), 1, 1));
            var result = new Parser().ParseBareBlock(lexer);
            if (lexer.HasNext()) throw new SyntaxError("Expected end of input but got '" + lexer.Next() + "'", lexer.GetPos());
            if (result is NodeReturn)
            {
                result = ((NodeReturn) result).GetExpression();
            }
            else if (result is NodeBlock)
            {
                var block = (NodeBlock) result;
                var expressions = block.GetExpressions();
                if (expressions.Count > 0)
                {
                    var lastexpr = expressions[expressions.Count - 1];
                    if (lastexpr is NodeReturn)
                    {
                        expressions[expressions.Count - 1] = ((NodeReturn) lastexpr).GetExpression();
                    }
                }
                
            }
            return result;
        }

        public Node ParseBareBlock(Lexer lexer)
        {
            var block = new NodeBlock(lexer.GetPosNext());
            var expression = ParseExpression(lexer);
            if (!lexer.HasNext())
            {
                return expression;
            }
            block.Add(expression);
            while (lexer.MatchIf(";", TokenType.Interpunction))
            {
                if (!lexer.HasNext()) break;
                block.Add(ParseExpression(lexer));
            }
            if (block.GetExpressions().Count == 1 && !block.HasFinally() && !block.HasCatch())
            {
                return block.GetExpressions()[0];
            }
            return block;
        }

        public Node ParseBlock(Lexer lexer)
        {
            var block = new NodeBlock(lexer.GetPosNext());
            lexer.Match("do", TokenType.Keyword);
            while (!lexer.PeekOne("catch", "finally", "end", TokenType.Keyword))
            {
                if (lexer.Peek("do", TokenType.Keyword))
                {
                    block.Add(ParseBlock(lexer));
                }
                else
                {
                    block.Add(ParseExpression(lexer));
                }
                if (lexer.PeekOne("catch", "finally", "end", TokenType.Keyword)) break;
                lexer.Match(";", TokenType.Interpunction);
                if (lexer.PeekOne("catch", "finally", "end", TokenType.Keyword)) break;
            }

            while (lexer.MatchIf("catch", TokenType.Keyword))
            {
                Node err;
                if (lexer.MatchIf("all", TokenType.Identifier))
                {
                    err = null;
                }
                else
                {
                    err = ParseExpression(lexer);
                }
                
                Node expr;
                if (lexer.Peek("do", TokenType.Keyword))
                {
                    expr = ParseBlock(lexer);
                }
                else
                {
                    expr = ParseExpression(lexer);
                }

                block.AddCatch(err, expr);
                if (lexer.Peek(";", TokenType.Interpunction)) lexer.Eat(1);
            }

            if (lexer.MatchIf("finally", TokenType.Keyword))
            {
                while (!lexer.Peek("end", TokenType.Keyword))
                {
                    if (lexer.Peek("do", TokenType.Keyword))
                    {
                        block.AddFinally(ParseBlock(lexer));
                    }
                    else
                    {
                        block.AddFinally(ParseExpression(lexer));
                    }
                    if (lexer.Peek("end", TokenType.Keyword)) break;
                    lexer.Match(";", TokenType.Interpunction);
                }
            }

            lexer.Match("end", TokenType.Keyword);
            
            if (block.GetExpressions().Count == 1 && !block.HasFinally() && !block.HasCatch())
            {
                return block.GetExpressions()[0];
            }
            return block;
        }

        public Node ParseExpression(Lexer lexer)
        {
            var comment = "";
            if (lexer.Peek().type == TokenType.String && lexer.Peekn(2, "def"))
            {
                comment = lexer.Next().value;
            }
            if (lexer.MatchIf("require", TokenType.Keyword)) 
            {
                var pos = lexer.GetPos();
                var modulespec = ParseIfExpr(lexer);
                var unqualified = false;
                Dictionary<string, string> symbols = null;
                string name = null;
                if (lexer.MatchIf("unqualified", TokenType.Identifier)) 
                {
                    unqualified = true;
                } 
                else if (lexer.MatchIf("as", TokenType.Keyword)) 
                {
                    name = lexer.MatchIdentifier();
                }
                else if (lexer.MatchIf("import", TokenType.Identifier, "[", TokenType.Interpunction))
                {
                    symbols = new Dictionary<string, string>();
                    while (!lexer.Peekn(1, "]", TokenType.Interpunction)) {
                        var symbol = lexer.MatchIdentifier();
                        var symbolname = symbol;
                        if (lexer.MatchIf("as", TokenType.Keyword)) {
                            symbolname = lexer.MatchIdentifier();
                        }
                        symbols[symbol] = symbolname;
                        if (!lexer.Peekn(1, "]", TokenType.Interpunction)) {
                            lexer.Match(",", TokenType.Interpunction);
                        }
                    }
                    lexer.Match("]", TokenType.Interpunction);
                }
                return new NodeRequire(modulespec, name, unqualified, symbols, pos);
            }
            if (lexer.MatchIf("def", TokenType.Keyword))
            {
                var pos = lexer.GetPos();
                Token token;
                if (lexer.MatchIf("[", TokenType.Interpunction)) 
                {
                    // handle destructuring def
                    var identifiers = new List<string>();
                    while (!lexer.Peek("]", TokenType.Interpunction)) 
                    {
                        token = lexer.Next();
                        if (token.type == TokenType.Keyword) throw new SyntaxError("Cannot redefine keyword '" + token + "'", token.pos);
                        if (token.type != TokenType.Identifier) throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
                        identifiers.Add(token.value);
                        if (!lexer.Peek("]", TokenType.Interpunction)) lexer.Match(",", TokenType.Interpunction);
                    }
                    lexer.Match("]", TokenType.Interpunction);
                    lexer.Match("=", TokenType.Operator);
                    return new NodeDefDestructuring(identifiers, ParseIfExpr(lexer), comment, pos);
                }
                // handle single var def
                token = lexer.Next();
                if (token.type == TokenType.Keyword)
                    throw new SyntaxError("Cannot redefine keyword '" + token + "'", token.pos);
                if (token.type != TokenType.Identifier)
                    throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
                if (lexer.Peek("(", TokenType.Interpunction))
                {
                    return new NodeDef(token.value, ParseFn(lexer, pos), comment, pos);
                }

                lexer.Match("=", TokenType.Operator);
                return new NodeDef(token.value, ParseIfExpr(lexer), comment, pos);
            }
            
            if (lexer.MatchIf("for", TokenType.Keyword))
            {
                var pos = lexer.GetPos();
                var identifiers = new List<string>();
                if (lexer.MatchIf("[", TokenType.Interpunction)) {
                    while (!lexer.Peekn(1, "]", TokenType.Interpunction)) {
                        Token token = lexer.Next();
                        if (token.type != TokenType.Identifier) throw new SyntaxError("Expected identifier in for loop but got '" + token + "'", token.pos);
                        identifiers.Add(token.value);
                        if (!lexer.Peekn(1, "]", TokenType.Interpunction)) lexer.Match(",", TokenType.Interpunction);
                    }
                    lexer.Match("]", TokenType.Interpunction);
                }
                else
                {
                    var token = lexer.Next();
                    if (token.type != TokenType.Identifier)
                        throw new SyntaxError("Expected identifier in for loop but got '" + token + "'", token.pos);
                    identifiers.Add(token.value);
                }

                lexer.Match("in", TokenType.Keyword);
                var what = "values";
                if (lexer.MatchIf("keys", TokenType.Identifier)) what = "keys";
                else if (lexer.MatchIf("values", TokenType.Identifier)) what = "values";
                else if (lexer.MatchIf("entries", TokenType.Identifier)) what = "entries";
                var expression = ParseExpression(lexer);
                if (lexer.Peek("do", TokenType.Keyword))
                {
                    return new NodeFor(identifiers, expression, ParseBlock(lexer), what, pos);
                }
                return new NodeFor(identifiers, expression, ParseExpression(lexer), what, pos);
            }

            if (lexer.MatchIf("while", TokenType.Keyword))
            {
                var pos = lexer.GetPos();
                var expr = ParseOrExpr(lexer);
                var block = ParseBlock(lexer);
                return new NodeWhile(expr, block, pos);
            }

            return ParseIfExpr(lexer);
        }

        private Node ParseIfExpr(Lexer lexer)
        {
            if (lexer.Peek("if", TokenType.Keyword))
            {
                var result = new NodeIf(lexer.GetPos());
                while (lexer.MatchIf("if", TokenType.Keyword) || lexer.MatchIf("elif", TokenType.Keyword))
                {
                    var condition = ParseOrExpr(lexer);
                    lexer.Match("then", TokenType.Keyword);
                    if (lexer.Peek("do", TokenType.Keyword))
                    {
                        result.AddIf(condition, ParseBlock(lexer));
                    }
                    else
                    {
                        result.AddIf(condition, ParseOrExpr(lexer));
                    }
                }

                if (lexer.MatchIf("else", TokenType.Keyword))
                {
                    if (lexer.Peek("do", TokenType.Keyword))
                    {
                        result.SetElse(ParseBlock(lexer));
                    }
                    else
                    {
                        result.SetElse(ParseOrExpr(lexer));
                    }
                }

                return result;
            }
            
            return ParseOrExpr(lexer);
        }

        private Node ParseOrExpr(Lexer lexer)
        {
            var expr = ParseAndExpr(lexer);
            if (lexer.Peek("or", TokenType.Keyword))
            {
                var result = new NodeOr(expr, lexer.GetPosNext());
                while (lexer.MatchIf("or", TokenType.Keyword))
                {
                    result.AddOrClause(ParseAndExpr(lexer));
                }

                return result;
            }

            return expr;
        }

        private Node ParseAndExpr(Lexer lexer)
        {
            var expr = ParseNotExpr(lexer);
            if (lexer.Peek("and", TokenType.Keyword))
            {
                var result = new NodeAnd(expr, lexer.GetPosNext());
                while (lexer.MatchIf("and", TokenType.Keyword))
                {
                    result.AddAndClause(ParseNotExpr(lexer));
                }

                return result;
            }

            return expr;
        }

        private Node ParseNotExpr(Lexer lexer)
        {
            if (lexer.MatchIf("not", TokenType.Keyword))
            {
                var pos = lexer.GetPos();
                return new NodeNot(ParseRelExpr(lexer), pos);
            }

            return ParseRelExpr(lexer);
        }

        private Node ParseRelExpr(Lexer lexer)
        {
            var expr = ParseAddExpr(lexer);
            var relops = new HashSet<string>();
            relops.Add("==");
            relops.Add("!=");
            relops.Add("<>");
            relops.Add("<");
            relops.Add("<=");
            relops.Add(">");
            relops.Add(">=");
            relops.Add("is");
            if (!lexer.HasNext() || !relops.Contains(lexer.Peek().value))
            {
                return expr;
            }

            var result = new NodeAnd(lexer.GetPosNext());
            var lhs = expr;
            while (lexer.HasNext() && relops.Contains(lexer.Peek().value))
            {
                var relop = lexer.Next().value;
                if (relop == "is" && lexer.Peek().value == "not")
                {
                    relop = "is not";
                    lexer.Eat(1);
                }
                var pos = lexer.GetPos();
                var rhs = ParseAddExpr(lexer);
                Node cmp = null;
                switch (relop)
                {
                    case "<":
                        cmp = FuncCall("less", "a", lhs, "b", rhs, pos);
                        break;
                    
                    case "<=":
                        cmp = FuncCall("less_equals", "a", lhs, "b", rhs, pos);
                        break;
                    
                    case ">":
                        cmp = FuncCall("greater", "a", lhs, "b", rhs, pos);
                        break;
                    
                    case ">=":
                        cmp = FuncCall("greater_equals", "a", lhs, "b", rhs, pos);
                        break;
                    
                    case "==":
                    case "is":
                        cmp = FuncCall("equals", "a", lhs, "b", rhs, pos);
                        break;
                    
                    case "<>":
                    case "!=":
                    case "is not":
                        cmp = FuncCall("not_equals", "a", lhs, "b", rhs, pos);
                        break;
                }
                result.AddAndClause(cmp);
                lhs = rhs;
            }

            return result.GetSimplified();
        }

        private Node ParseAddExpr(Lexer lexer)
        {
            var expr = ParseMulExpr(lexer);
            while (lexer.PeekOne("+", "-", TokenType.Operator))
            {
                if (lexer.MatchIf("+", TokenType.Operator))
                {
                    var pos = lexer.GetPos();
                    expr = FuncCall("add", "a", expr, "b", ParseMulExpr(lexer), pos);
                }
                else if (lexer.MatchIf("-", TokenType.Operator))
                {
                    var pos = lexer.GetPos();
                    expr = FuncCall("sub", "a", expr, "b", ParseMulExpr(lexer), pos);
                }
            }

            return expr;
        }

        private Node ParseMulExpr(Lexer lexer)
        {
            var expr = ParseUnaryExpr(lexer);
            while (lexer.PeekOne("*", "/", "%", TokenType.Operator))
            {
                if (lexer.MatchIf("*", TokenType.Operator))
                {
                    var pos = lexer.GetPos();
                    expr = FuncCall("mul", "a", expr, "b", ParseUnaryExpr(lexer), pos);
                }
                else if (lexer.MatchIf("/", TokenType.Operator))
                {
                    var pos = lexer.GetPos();
                    expr = FuncCall("div", "a", expr, "b", ParseUnaryExpr(lexer), pos);
                }
                else if (lexer.MatchIf("%", TokenType.Operator))
                {
                    var pos = lexer.GetPos();
                    expr = FuncCall("mod", "a", expr, "b", ParseUnaryExpr(lexer), pos);
                }
            }

            return expr;
        }

        private Node ParseUnaryExpr(Lexer lexer)
        {
            if (lexer.MatchIf("+", TokenType.Operator))
            {
                return ParsePredExpr(lexer);
            }

            if (lexer.MatchIf("-", TokenType.Operator))
            {
                var pos = lexer.GetPos();
                if (lexer.Peek().type == TokenType.Int) 
                {
                    return ParsePredExpr(lexer, true);
                }
                if (lexer.Peek().type == TokenType.Decimal) 
                {
                    return ParsePredExpr(lexer, true);
                }
                var call = new NodeFuncall(new NodeIdentifier("sub", pos), pos);
                call.AddArg("a", new NodeLiteral(new ValueInt(0), pos));
                call.AddArg("b", ParsePredExpr(lexer));
                return call;
            }

            return ParsePredExpr(lexer);
        }

        private Node ParsePredExpr(Lexer lexer, bool unary_minus = false)
        {
            var expr = ParsePrimaryExpr(lexer, unary_minus);
            var pos = lexer.GetPosNext();
            if (lexer.MatchIf("is", TokenType.Keyword)) {
                if (lexer.MatchIf("not", TokenType.Keyword)) {
                    if (lexer.MatchIf("in")) {
                        return new NodeNot(new NodeIn(expr, this.ParsePrimaryExpr(lexer, false), pos), pos);
                    } else if (lexer.MatchIf("empty", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("is_empty", "obj", expr, pos), pos);
                    } else if (lexer.MatchIf("zero", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("is_zero", "obj", expr, pos), pos);
                    } else if (lexer.MatchIf("negative", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("is_negative", "obj", expr, pos), pos);
                    } else if (lexer.MatchIf("numerical", TokenType.Identifier)) {
                        return new NodeNot(this.CollectPredicateMinMaxExact("is_numerical", this.FuncCall("string", "obj", expr, pos), lexer, pos), pos);
                    } else if (lexer.MatchIf("alphanumerical", TokenType.Identifier)) {
                        return new NodeNot(this.CollectPredicateMinMaxExact("is_alphanumerical", this.FuncCall("string", "obj", expr, pos), lexer, pos), pos);
                    } else if (lexer.MatchIf("date", "with", "hour", TokenType.Identifier, TokenType.Identifier, TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("is_valid_date", "str", this.FuncCall("string", "obj", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMddHH"), pos), pos), pos);
                    } else if (lexer.MatchIf("date", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("is_valid_date", "str", this.FuncCall("string", "obj", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMdd"), pos), pos), pos);
                    } else if (lexer.MatchIf("time", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("is_valid_time", "str", this.FuncCall("string", "obj", expr, pos), "fmt", new NodeLiteral(new ValueString("HHmm"), pos), pos), pos);
                    } else if (lexer.MatchIf("string", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("string"), pos), pos), pos);
                    } else if (lexer.MatchIf("int", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("int"), pos), pos), pos);
                    } else if (lexer.MatchIf("decimal", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("decimal"), pos), pos), pos);
                    } else if (lexer.MatchIf("boolean", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("boolean"), pos), pos), pos);
                    } else if (lexer.MatchIf("pattern", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("pattern"), pos), pos), pos);
                    } else if (lexer.MatchIf("date", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("date"), pos), pos), pos);
                    } else if (lexer.MatchIf("null", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("null"), pos), pos), pos);
                    } else if (lexer.MatchIf("func", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("func"), pos), pos), pos);
                    } else if (lexer.MatchIf("input", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("input"), pos), pos), pos);
                    } else if (lexer.MatchIf("output", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("output"), pos), pos), pos);
                    } else if (lexer.MatchIf("list", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("lsit"), pos), pos), pos);
                    } else if (lexer.MatchIf("set", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("set"), pos), pos), pos);
                    } else if (lexer.MatchIf("map", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("map"), pos), pos), pos);
                    } else if (lexer.MatchIf("object", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("object"), pos), pos), pos);
                    } else if (lexer.MatchIf("node", TokenType.Identifier)) {
                        return new NodeNot(this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("node"), pos), pos), pos);
                    } else {
                        lexer.Putback(); // not
                        lexer.Putback(); // is
                        return expr;
                    }
                } else if (lexer.MatchIf("in", TokenType.Keyword)) {
                    return new NodeIn(expr, this.ParsePrimaryExpr(lexer, false), pos);
                } else if (lexer.MatchIf("empty", TokenType.Identifier)) {
                    return this.FuncCall("is_empty", expr, pos);
                } else if (lexer.MatchIf("zero", TokenType.Identifier)) {
                    return this.FuncCall("is_zero", expr, pos);
                } else if (lexer.MatchIf("negative", TokenType.Identifier)) {
                    return this.FuncCall("is_negative", expr, pos);
                } else if (lexer.MatchIf("numerical", TokenType.Identifier)) {
                    return this.CollectPredicateMinMaxExact("is_numerical", this.FuncCall("string", expr, pos), lexer, pos);
                } else if (lexer.MatchIf("alphanumerical", TokenType.Identifier)) {
                    return this.CollectPredicateMinMaxExact("is_alphanumerical", this.FuncCall("string", expr, pos), lexer, pos);
                } else if (lexer.MatchIf("date", "with", "hour", TokenType.Identifier, TokenType.Identifier, TokenType.Identifier)) {
                    return this.FuncCall("is_valid_date", "str", this.FuncCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMddHH"), pos), pos);
                } else if (lexer.MatchIf("date", TokenType.Identifier)) {
                    return this.FuncCall("is_valid_date", "str", this.FuncCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("yyyyMMdd"), pos), pos);
                } else if (lexer.MatchIf("time", TokenType.Identifier)) {
                    return this.FuncCall("is_valid_time", "str", this.FuncCall("string", expr, pos), "fmt", new NodeLiteral(new ValueString("HHmm"), pos), pos);
                } else if (lexer.MatchIf("string", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("string"), pos), pos);
                } else if (lexer.MatchIf("int", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("int"), pos), pos);
                } else if (lexer.MatchIf("decimal", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("decimal"), pos), pos);
                } else if (lexer.MatchIf("boolean", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("boolean"), pos), pos);
                } else if (lexer.MatchIf("pattern", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("pattern"), pos), pos);
                } else if (lexer.MatchIf("date", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("date"), pos), pos);
                } else if (lexer.MatchIf("null", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("null"), pos), pos);
                } else if (lexer.MatchIf("func", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("func"), pos), pos);
                } else if (lexer.MatchIf("input", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("input"), pos), pos);
                } else if (lexer.MatchIf("output", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("output"), pos), pos);
                } else if (lexer.MatchIf("list", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("list"), pos), pos);
                } else if (lexer.MatchIf("set", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("set"), pos), pos);
                } else if (lexer.MatchIf("map", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("map"), pos), pos);
                } else if (lexer.MatchIf("object", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("object"), pos), pos);
                } else if (lexer.MatchIf("node", TokenType.Identifier)) {
                    return this.FuncCall("equals", this.FuncCall("type", expr, pos), new NodeLiteral(new ValueString("node"), pos), pos);
                }
                lexer.Putback(); // is
                return expr;
            } else if (lexer.MatchIf("not", "in", TokenType.Keyword, TokenType.Keyword)) {
                return new NodeNot(new NodeIn(expr, this.ParsePrimaryExpr(lexer, false), pos), pos);
            } else if (lexer.MatchIf("in", TokenType.Keyword)) {
                return new NodeIn(expr, this.ParsePrimaryExpr(lexer, false), pos);
            } else if (lexer.MatchIf("starts", "not", "with", TokenType.Identifier, TokenType.Keyword, TokenType.Identifier)) {
                return new NodeNot(this.FuncCall("starts_with", "str", expr, "part", this.ParsePrimaryExpr(lexer, false), pos), pos);
            } else if (lexer.MatchIf("starts", "with", TokenType.Identifier, TokenType.Identifier)) {
                return this.FuncCall("starts_with", "str", expr, "part", this.ParsePrimaryExpr(lexer, false), pos);
            } else if (lexer.MatchIf("ends", "not", "with", TokenType.Identifier, TokenType.Keyword, TokenType.Identifier)) {
                return new NodeNot(this.FuncCall("ends_with", "str", expr, "part", this.ParsePrimaryExpr(lexer, false), pos), pos);
            } else if (lexer.MatchIf("ends", "with", TokenType.Identifier, TokenType.Identifier)) {
                return this.FuncCall("ends_with", "str", expr, "part", this.ParsePrimaryExpr(lexer, false), pos);
            } else if (lexer.MatchIf("contains", "not", TokenType.Identifier, TokenType.Keyword)) {
                return new NodeNot(this.FuncCall("contains", "str", expr, "part", this.ParsePrimaryExpr(lexer, false), pos), pos);
            } else if (lexer.MatchIf("contains", TokenType.Identifier)) {
                return this.FuncCall("contains", "str", expr, "part", this.ParsePrimaryExpr(lexer, false), pos);
            } else if (lexer.MatchIf("matches", "not", TokenType.Identifier, TokenType.Keyword)) {
                return new NodeNot(this.FuncCall("matches", "str", expr, "pattern", this.ParsePrimaryExpr(lexer, false), pos), pos);
            } else if (lexer.MatchIf("matches", TokenType.Identifier)) {
                return this.FuncCall("matches", "str", expr, "pattern", this.ParsePrimaryExpr(lexer, false), pos);
            }

            return expr;
        }

        private Node CollectPredicateMinMaxExact(string fn, Node expr, Lexer lexer, SourcePos pos)
        {
            Node min_len = new NodeLiteral(new ValueInt(1), pos);
            Node max_len = new NodeLiteral(new ValueInt(9999), pos);
            if (lexer.MatchIf("min_len", TokenType.Identifier))
            {
                min_len = ParsePrimaryExpr(lexer);
            }

            if (lexer.MatchIf("max_len", TokenType.Identifier))
            {
                max_len = ParsePrimaryExpr(lexer);
            }

            if (lexer.MatchIf("exact_len", TokenType.Identifier))
            {
                min_len = max_len = ParsePrimaryExpr(lexer);
            }

            return FuncCall(fn, "str", expr, "min", min_len, "max", max_len, pos);
        }

        private Node ParsePrimaryExpr(Lexer lexer, bool unary_minus = false)
        {
            if (!lexer.HasNext()) throw new SyntaxError("Unexpected end of input", lexer.GetPos());
            
            Node result;
            
            var token = lexer.Next();
            if (token.value == "(")
            {
                result = ParseBareBlock(lexer);
                lexer.Match(")", TokenType.Interpunction);
                return DerefOrCallOrInvoke(lexer, result);
            }

            switch (token.type)
            {
                case TokenType.Identifier:
                    result = new NodeIdentifier(token.value, token.pos);
                    if (lexer.MatchIf("=", TokenType.Operator)) {
                        result = new NodeAssign(token.value, ParseExpression(lexer), token.pos);
                    } else if (lexer.MatchIf("+=", TokenType.Operator)) {
                        var value = ParseExpression(lexer);
                        result = new NodeAssign(token.value, FuncCall("add", "a", result, "b", value, token.pos), token.pos);
                    } else if (lexer.MatchIf("-=", TokenType.Operator)) {
                        var value = ParseExpression(lexer);
                        result = new NodeAssign(token.value, FuncCall("sub", "a", result, "b", value, token.pos), token.pos);
                    } else if (lexer.MatchIf( "*=", TokenType.Operator)) {
                        var value = ParseExpression(lexer);
                        result = new NodeAssign(token.value, FuncCall("mul", "a", result, "b", value, token.pos), token.pos);
                    } else if (lexer.MatchIf( "/=", TokenType.Operator)) {
                        var value = ParseExpression(lexer);
                        result = new NodeAssign(token.value, FuncCall("div", "a", result, "b", value, token.pos), token.pos);
                    } else if (lexer.MatchIf( "%=", TokenType.Operator)) {
                        var value = ParseExpression(lexer);
                        result = new NodeAssign(token.value, FuncCall("mod", "a", result, "b", value, token.pos), token.pos);
                    } else {
                        result = DerefOrCallOrInvoke(lexer, result);
                    }
                    break;
                case TokenType.String:
                    result = new NodeLiteral(new ValueString(token.value), token.pos);
                    result = DerefOrInvoke(lexer, result);
                    break;
                case TokenType.Int:
                    result = new NodeLiteral(new ValueInt(Convert.ToInt64(token.value) * (unary_minus ? -1 : 1)), token.pos);
                    result = Invoke(lexer, result);
                    break;
                case TokenType.Decimal:
                    result = new NodeLiteral(new ValueDecimal(Convert.ToDecimal(token.value) * (unary_minus ? -1 : 1)), token.pos);
                    result = Invoke(lexer, result);
                    break;
                case TokenType.Boolean:
                    result = new NodeLiteral(ValueBoolean.From(token.value == "TRUE"), token.pos);
                    result = Invoke(lexer, result);
                    break;
                case TokenType.Pattern:
                    result = new NodeLiteral(new ValuePattern(token.value.Substring(2, token.value.Length - 4)), token.pos);
                    result = Invoke(lexer, result);
                    break;
                default:
                    if (token.value == "fn")
                    {
                        result = ParseFn(lexer, token.pos);
                    }
                    else if (token.value == "break")
                    {
                        result = new NodeBreak(token.pos);
                    }
                    else if (token.value == "continue")
                    {
                        result = new NodeContinue(token.pos);
                    }
                    else if (token.value == "return")
                    {
                        if (lexer.Peekn(1, ";", TokenType.Interpunction)) result = new NodeReturn(null, token.pos);
                        else result = new NodeReturn(this.ParseExpression(lexer), token.pos);
                    }
                    else if (token.value == "error")
                    {
                        result = new NodeError(ParseExpression(lexer), token.pos);
                    }
                    else if (token.value == "do")
                    {
                        lexer.Putback();
                        result = ParseBlock(lexer);
                    }
                    else if (token.value == "[")
                    {
                        result = ParseListLiteral(lexer, token);
                        if (lexer.Peek("=", TokenType.Operator)) 
                        {
                            var identifiers = new List<string>();
                            foreach (var item in ((NodeList) result).GetItems()) {
                                if (!(item is NodeIdentifier)) throw new SyntaxError("Destructuring assign expected identifier but got " + item, token.pos);
                                identifiers.Add(((NodeIdentifier) item).GetValue());
                            }
                            lexer.Match("=", TokenType.Operator);
                            result = new NodeAssignDestructuring(identifiers, ParseIfExpr(lexer), token.pos);
                        }
                    }
                    else if (token.value == "<<") // set literal
                    {
                        result = ParseSetLiteral(lexer, token);
                    }
                    else if (token.value == "<<<") // map literal
                    {
                        result = ParseMapLiteral(lexer, token);
                    } 
                    else if (token.value == "<*") // object literal
                    {
                        result = ParseObjectLiteral(lexer, token);
                    } 
                    else if (token.value == "..." && token.type == TokenType.Interpunction) 
                    {
                        token = lexer.Next();
                        if (token.value == "[" && token.type == TokenType.Interpunction) 
                        {
                            result = ParseListLiteral(lexer, token);
                        } 
                        else if (token.value == "<<<" && token.type == TokenType.Interpunction) 
                        {
                            result = ParseMapLiteral(lexer, token);
                        } 
                        else if (token.type == TokenType.Identifier) 
                        {
                            result = new NodeIdentifier(token.value, token.pos);
                        } 
                        else 
                        {
                            throw new SyntaxError("Spread operator only allowed with identifiers, list and map literals", token.pos);
                        }
                        result = new NodeSpread(result, token.pos);
                    }
                    else
                    {
                        throw new SyntaxError("Invalid syntax at '" + token + "'", token.pos);
                    }
                    break;
            }

            return result;
        }

        private Node ParseListLiteral(Lexer lexer, Token token)
        {
            if (lexer.MatchIf("]", TokenType.Interpunction))
            {
                return DerefOrInvoke(lexer, new NodeList(token.pos));
            }
            
            Node expr;
            if (lexer.Peek("if", TokenType.Keyword))
            {
                expr = ParseIfExpr(lexer);
            }
            else
            {
                expr = ParseOrExpr(lexer);
            }
            if (lexer.MatchIf("for", TokenType.Keyword))
            {
                var identifier = lexer.MatchIdentifier();
                lexer.Match("in", TokenType.Keyword);
                var listExpr = ParseOrExpr(lexer);
                var comprehension = new NodeListComprehension(expr, identifier, listExpr, token.pos);
                if (lexer.MatchIf("if", TokenType.Keyword))
                {
                    comprehension.SetCondition(ParseOrExpr(lexer));
                }
                lexer.Match("]", TokenType.Interpunction);
                return DerefOrInvoke(lexer, comprehension);
            }
            
            var list = new NodeList(token.pos);
            while (!lexer.Peek("]", TokenType.Interpunction))
            {
                list.AddItem(expr);
                expr = null;
                if (!lexer.Peek("]", TokenType.Interpunction))
                {
                    lexer.Match(",", TokenType.Interpunction);
                    if (!lexer.Peek("]", TokenType.Interpunction))
                    {
                        expr = ParseIfExpr(lexer);
                    }
                }
            }
            if (expr != null) list.AddItem(expr);
            lexer.Match("]", TokenType.Interpunction);
            return DerefOrInvoke(lexer, list);
        }

        public Node ParseSetLiteral(Lexer lexer, Token token)
        {
            if (lexer.MatchIf(">>", TokenType.Interpunction))
            {
                return DerefOrInvoke(lexer, new NodeSet(token.pos));
            }
            
            var expr = ParseIfExpr(lexer);
            if (lexer.MatchIf("for", TokenType.Keyword)) {
                var identifier = lexer.MatchIdentifier();
                lexer.Match("in", TokenType.Keyword);
                var listExpr = ParseOrExpr(lexer);
                var comprehension = new NodeSetComprehension(expr, identifier, listExpr, token.pos);
                if (lexer.MatchIf("if", TokenType.Keyword)) {
                    comprehension.SetCondition(ParseOrExpr(lexer));
                }

                lexer.Match(">>", TokenType.Interpunction);
                return DerefOrInvoke(lexer, comprehension);
            }
            var set = new NodeSet(token.pos);
            set.AddItem(expr);
            if (!lexer.Peek(">>", TokenType.Interpunction)) {
                lexer.Match(",", TokenType.Interpunction);
            }
            while (!lexer.Peek(">>", TokenType.Interpunction)) {
                set.AddItem(ParseIfExpr(lexer));
                if (!lexer.Peek(">>", TokenType.Interpunction)) {
                    lexer.Match(",", TokenType.Interpunction);
                }
            }
            lexer.Match(">>", TokenType.Interpunction);
            return DerefOrInvoke(lexer, set);
        }

        public Node ParseMapLiteral(Lexer lexer, Token token)
        {
            if (lexer.MatchIf(">>>", TokenType.Interpunction))
            {
                return DerefOrInvoke(lexer, new NodeMap(token.pos));
            }
            var key = ParseIfExpr(lexer);
            lexer.Match("=>", TokenType.Interpunction);
            var value = ParseIfExpr(lexer);
            if (lexer.MatchIf("for", TokenType.Keyword))
            {
                var identifier = lexer.MatchIdentifier();
                lexer.Match("in", TokenType.Keyword);
                var listExpr = ParseOrExpr(lexer);
                var comprehension = new NodeMapComprehension(key, value, identifier, listExpr, token.pos);
                if (lexer.MatchIf("if", TokenType.Keyword)) {
                    comprehension.SetCondition(ParseOrExpr(lexer));
                }

                lexer.Match(">>>", TokenType.Interpunction);
                return DerefOrInvoke(lexer, comprehension);
            }
            var map = new NodeMap(token.pos);
            if (key is NodeIdentifier) {
                key = new NodeLiteral(new ValueString(((NodeIdentifier) key).GetValue()), key.GetSourcePos());
            }
            map.AddKeyValue(key, value);
            if (!lexer.Peek(">>>", TokenType.Interpunction))
            {
                lexer.Match(",", TokenType.Interpunction);
            }
            while (!lexer.Peek(">>>", TokenType.Interpunction))
            {
                key = ParseIfExpr(lexer);
                if (key is NodeIdentifier) {
                    key = new NodeLiteral(new ValueString(((NodeIdentifier) key).GetValue()), key.GetSourcePos());
                }
                lexer.Match("=>", TokenType.Interpunction);
                value = ParseIfExpr(lexer);
                map.AddKeyValue(key, value);
                if (!lexer.Peek(">>>", TokenType.Interpunction))
                {
                    lexer.Match(",", TokenType.Interpunction);
                }
            }

            lexer.Match(">>>", TokenType.Interpunction);
            return DerefOrInvoke(lexer, map);
        }
        
        public Node ParseObjectLiteral(Lexer lexer, Token token) {
            var obj = new NodeObject(token.pos);
            while (!lexer.Peekn(1, "*>", TokenType.Interpunction)) {
                var key = lexer.MatchIdentifier();
                if (lexer.Peekn(1, "(", TokenType.Interpunction)) {
                    var fn = this.ParseFn(lexer, lexer.GetPos());
                    obj.AddKeyValue(key, fn);
                }
                else
                {
                    lexer.Match("=", TokenType.Operator);
                    var value = ParseIfExpr(lexer);
                    obj.AddKeyValue(key, value);
                }

                if (!lexer.Peekn(1, "*>", TokenType.Interpunction)) {
                    lexer.Match(",", TokenType.Interpunction);
                }
            }
            lexer.Match("*>", TokenType.Interpunction);
            return DerefOrInvoke(lexer, obj);
        }

        public Node ParseFn(Lexer lexer, SourcePos pos)
        {
            var lambda = new NodeLambda(pos);
            lexer.Match("(", TokenType.Interpunction);
            while (!lexer.MatchIf(")", TokenType.Interpunction))
            {
                var token = lexer.Next();
                if (token.type == TokenType.Keyword) throw new SyntaxError("Cannot use keyword '" + token + "' as parameter name", token.pos);
                if (token.type != TokenType.Identifier) throw new SyntaxError("Expected parameter name but got '" + token + "'", token.pos);
                var argname = token.value;
                Node defvalue = null;
                if (lexer.MatchIf("=", TokenType.Operator))
                {
                    defvalue = ParseExpression(lexer);
                }
                lambda.AddArg(argname, defvalue);
                if (argname.EndsWith("...") && !lexer.Peek(")", TokenType.Interpunction))
                {
                    throw new SyntaxError("Rest argument " + argname + " must be last argument", token.pos);
                }
                if (!lexer.Peek(")", TokenType.Interpunction)) lexer.Match(",", TokenType.Interpunction);
            }

            if (lexer.Peek("do", TokenType.Keyword))
            {
                lambda.SetBody(ParseBlock(lexer));
            }
            else
            {
                lambda.SetBody(ParseIfExpr(lexer));
            }

            return lambda;
        }
            
        private Node _invoke(Lexer lexer, Node node) 
        {
            if (lexer.MatchIf("!>", TokenType.Operator)) 
            {
                Node fn;
                if (lexer.MatchIf("(", TokenType.Interpunction, "fn", TokenType.Keyword)) {
                    fn = ParseFn(lexer, lexer.GetPos());
                    lexer.Match(")", TokenType.Interpunction);
                } else {
                    fn = new NodeIdentifier(lexer.MatchIdentifier(), lexer.GetPos());
                    while (lexer.MatchIf("->", TokenType.Operator)) {
                        fn = new NodeDeref(fn, new NodeLiteral(new ValueString(lexer.MatchIdentifier()), lexer.GetPos()), lexer.GetPos());
                    }
                }
                var call = new NodeFuncall(fn, lexer.GetPos());
                call.AddArg(null, node);
                lexer.Match("(", TokenType.Interpunction);
                while (!lexer.Peekn(1, ")", TokenType.Interpunction)) 
                {
                    if (lexer.Peek().type == TokenType.Identifier && lexer.Peekn(2, "=", TokenType.Operator)) 
                    {
                        String name = lexer.MatchIdentifier();
                        lexer.Match("=", TokenType.Operator);
                        call.AddArg(name, ParseExpression(lexer));
                    } 
                    else 
                    {
                        call.AddArg(null, ParseExpression(lexer));
                    }
                    if (!lexer.Peekn(1, ")", TokenType.Interpunction)) lexer.Match(",", TokenType.Interpunction);
                }
                lexer.Eat(1);
                node = call;
            }
            return node;
        }

        private Node _call(Lexer lexer, Node node) 
        {
            if (lexer.MatchIf("(", TokenType.Interpunction)) 
            {
                NodeFuncall call = new NodeFuncall(node, lexer.GetPos());
                while (!lexer.Peekn(1, ")", TokenType.Interpunction)) 
                {
                    if (lexer.Peek().type == TokenType.Identifier && lexer.Peekn(2, "=", TokenType.Operator)) 
                    {
                        String name = lexer.MatchIdentifier();
                        lexer.Match("=", TokenType.Operator);
                        call.AddArg(name, this.ParseExpression(lexer));
                    } 
                    else 
                    {
                        call.AddArg(null, this.ParseExpression(lexer));
                    }
                    if (!lexer.Peekn(1, ")", TokenType.Interpunction)) lexer.Match(",", TokenType.Interpunction);
                }
                lexer.Eat(1);
                node = call;
            }
            return node;
        }

        private class DerefResult {
            public Node node;
            public bool interrupt;
        }

        private DerefResult _deref(Lexer lexer, Node node) 
        {
            var result = new DerefResult();
            if (lexer.MatchIf("->", TokenType.Operator))
            {
                var pos = lexer.GetPos();
                var identifier = lexer.MatchIdentifier();
                var index = new NodeLiteral(new ValueString(identifier), pos);
                if (lexer.MatchIf("=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, value, pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("(", TokenType.Interpunction)) 
                {
                    var n = new NodeDerefInvoke(node, identifier, pos);
                    while (!lexer.Peekn(1, ")", TokenType.Interpunction)) {
                        if (lexer.Peek().type == TokenType.Identifier && lexer.Peekn(2, "=", TokenType.Operator)) {
                            var name = lexer.MatchIdentifier();
                            lexer.Match("=", TokenType.Operator);
                            n.AddArg(name, this.ParseExpression(lexer));
                        } else {
                            n.AddArg(null, this.ParseExpression(lexer));
                        }
                        if (!lexer.Peekn(1, ")", TokenType.Interpunction)) lexer.Match(",", TokenType.Interpunction);
                    }
                    lexer.Eat(1);
                    result.node = n;
                } 
                else if (lexer.MatchIf("+=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("add", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("-=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("sub", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("*=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("mul", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("/=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("div", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("%=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("mod", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else 
                {
                    result.node = new NodeDeref(node, index, pos);
                }
            }
            else if (lexer.MatchIf("[", TokenType.Interpunction)) 
            {
                var pos = lexer.GetPos();
                var index = this.ParseExpression(lexer);
                if (lexer.MatchIf("]", TokenType.Interpunction, "=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, value, pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("]", TokenType.Interpunction, "+=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("add", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("]", TokenType.Interpunction, "-=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("sub", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("]", TokenType.Interpunction, "*=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("mul", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("]", TokenType.Interpunction, "/=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("div", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else if (lexer.MatchIf("]", TokenType.Interpunction, "%=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, this.FuncCall("mod", "a", new NodeDeref(node, index, pos), "b", value, pos), pos);
                    result.interrupt = true;
                } 
                else 
                {
                    result.node = new NodeDeref(node, index, pos);
                    lexer.Match("]", TokenType.Interpunction);
                }
            }
            return result;
        }

        public Node DerefOrCallOrInvoke(Lexer lexer, Node node) {
            while (lexer.Peekn(1, "!>", TokenType.Operator) || lexer.Peekn(1, "[", TokenType.Interpunction) || lexer.Peekn(1, "(", TokenType.Interpunction) || lexer.Peekn(1, "->", TokenType.Operator)) 
            {
                if (lexer.Peekn(1, "!>", TokenType.Operator)) 
                {
                    node = this._invoke(lexer, node);
                } 
                else if (lexer.Peekn(1, "(", TokenType.Interpunction)) 
                {
                    node = this._call(lexer, node);
                } 
                else if (lexer.Peekn(1, "[", TokenType.Interpunction) || lexer.Peekn(1, "->", TokenType.Operator)) 
                {
                    DerefResult result = this._deref(lexer, node);
                    node = result.node;
                    if (result.interrupt) break;
                }
            }
            return node;
        }

        public Node DerefOrInvoke(Lexer lexer, Node node) 
        {
            while (lexer.Peekn(1, "!>", TokenType.Operator) || lexer.Peekn(1, "[", TokenType.Interpunction) || lexer.Peekn(1, "->", TokenType.Operator)) 
            {
                if (lexer.Peekn(1, "!>", TokenType.Operator)) 
                {
                    node = this._invoke(lexer, node);
                } 
                else if (lexer.Peekn(1, "[", TokenType.Interpunction) || lexer.Peekn(1, "->", TokenType.Operator)) 
                {
                    DerefResult result = this._deref(lexer, node);
                    node = result.node;
                    if (result.interrupt) break;
                }
            }
            return node;
        }

        public Node Invoke(Lexer lexer, Node node) {
            while (lexer.Peekn(1, "!>", TokenType.Operator)) 
            {
                node = this._invoke(lexer, node);
            }
            return node;
        }

        private Node FuncCall(string fn, string a, Node exprA, SourcePos pos)
        {
            var result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
            result.AddArg(a, exprA);
            return result;
        }
        
        private Node FuncCall(string fn, Node exprA, SourcePos pos)
        {
            var result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
            result.AddArg("obj", exprA);
            return result;
        }
        
        private Node FuncCall(string fn, string a, Node exprA, string b, Node exprB, SourcePos pos)
        {
            var result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
            result.AddArg(a, exprA);
            result.AddArg(b, exprB);
            return result;
        }
        
        private Node FuncCall(string fn, Node exprA, Node exprB, SourcePos pos)
        {
            var result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
            result.AddArg("a", exprA);
            result.AddArg("b", exprB);
            return result;
        }
        
        private Node FuncCall(string fn, string a, Node exprA, string b, Node exprB, string c, Node exprC, SourcePos pos)
        {
            var result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
            result.AddArg(a, exprA);
            result.AddArg(b, exprB);
            result.AddArg(c, exprC);
            return result;
        }
        
    }
}