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
            if (block.GetExpressions().Count == 1 && !block.HasFinally())
            {
                return block.GetExpressions()[0];
            }
            return block;
        }

        public Node ParseBlock(Lexer lexer)
        {
            var block = new NodeBlock(lexer.GetPosNext());
            lexer.Match("do", TokenType.Keyword);
            var infinally = false;
            while (!lexer.Peek("end", TokenType.Keyword))
            {
                if (lexer.Peek("finally", TokenType.Keyword))
                {
                    infinally = true;
                    lexer.Eat(1);
                }
                if (lexer.Peek("end", TokenType.Keyword)) break;
                if (infinally)
                {
                    block.AddFinally(ParseExpression(lexer));
                }
                else
                {
                    block.Add(ParseExpression(lexer));
                }
                if (lexer.Peek("end", TokenType.Keyword)) break;
                lexer.Match(";", TokenType.Interpunction);
                if (lexer.Peek("end", TokenType.Keyword)) break;
            }
            lexer.Match("end", TokenType.Keyword);
            if (block.GetExpressions().Count == 1 && !block.HasFinally())
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
                var expression = ParseExpression(lexer);
                if (lexer.Peek("do", TokenType.Keyword))
                {
                    return new NodeFor(identifiers, expression, ParseBlock(lexer), pos);
                }
                return new NodeFor(identifiers, expression, ParseExpression(lexer), pos);
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
                call.AddArg("a", new NodeLiteralInt(0, pos));
                call.AddArg("b", ParsePredExpr(lexer));
                return call;
            }

            return ParsePredExpr(lexer);
        }

        private Node ParsePredExpr(Lexer lexer, bool unary_minus = false)
        {
            var expr = ParsePrimaryExpr(lexer, unary_minus);
            var pos = lexer.GetPosNext();
            if (lexer.MatchIf("is", "not", "empty"))
            {
                return FuncCall("is_not_empty", "obj", expr, pos);
            }

            if (lexer.MatchIf("is", "empty"))
            {
                return FuncCall("is_empty", "obj", expr, pos);
            }

            if (lexer.MatchIf("is", "not", "zero"))
            {
                return new NodeNot(FuncCall("is_zero", "obj", expr, pos), pos);
            }

            if (lexer.MatchIf("is", "zero"))
            {
                return FuncCall("is_zero", "obj", expr, pos);
            }

            if (lexer.MatchIf("is", "not", "negative"))
            {
                return new NodeNot(FuncCall("is_negative", "obj", expr, pos), pos);
            }

            if (lexer.MatchIf("is", "negative"))
            {
                return FuncCall("is_negative", "obj", expr, pos);
            }

            if (lexer.MatchIf("is", "not", "numerical"))
            {
                return new NodeNot(CollectPredicateMinMaxExact("is_numerical", 
                    FuncCall("string", "obj", expr, pos), lexer, pos), pos);
            }

            if (lexer.MatchIf("is", "numerical"))
            {
                return CollectPredicateMinMaxExact("is_numerical", 
                    FuncCall("string", "obj", expr, pos), lexer, pos);
            }

            if (lexer.MatchIf("is", "not", "alphanumerical"))
            {
                return new NodeNot(CollectPredicateMinMaxExact("is_alphanumerical", 
                    FuncCall("string", "obj", expr, pos), lexer, pos), pos);
            }

            if (lexer.MatchIf("is", "alphanumerical"))
            {
                return CollectPredicateMinMaxExact("is_alphanumerical", 
                    FuncCall("string", "obj", expr, pos), lexer, pos);
            }

            if (lexer.MatchIf("is", "not", "date", "with", "hour"))
            {
                return new NodeNot(FuncCall("is_valid_date", "str", 
                    FuncCall("string", "obj", expr, pos), "fmt", 
                    new NodeLiteralString("yyyyMMddHH", pos), pos), pos);
            }

            if (lexer.MatchIf("is", "date", "with", "hour"))
            {
                return FuncCall("is_valid_date", "str", 
                    FuncCall("string", "obj", expr, pos), "fmt", 
                    new NodeLiteralString("yyyyMMddHH", pos), pos);
            }

            if (lexer.MatchIf("is", "not", "date"))
            {
                return new NodeNot(FuncCall("is_valid_date", "str", 
                    FuncCall("string", "obj", expr, pos), "fmt", 
                    new NodeLiteralString("yyyyMMdd", pos), pos), pos);
            }

            if (lexer.MatchIf("is", "date"))
            {
                return FuncCall("is_valid_date", "str", 
                    FuncCall("string", "obj", expr, pos), "fmt", 
                    new NodeLiteralString("yyyyMMdd", pos), pos);
            }

            if (lexer.MatchIf("is", "not", "time"))
            {
                return new NodeNot(FuncCall("is_valid_time", "str", 
                    FuncCall("string", "obj", expr, pos), "fmt", 
                    new NodeLiteralString("HHmm", pos), pos), pos);
            }

            if (lexer.MatchIf("is", "time"))
            {
                return FuncCall("is_valid_time", "str", 
                    FuncCall("string", "obj", expr, pos), "fmt", 
                    new NodeLiteralString("HHmm", pos), pos);
            }

            if (lexer.MatchIf("is", "not", "in"))
            {
                return new NodeNot(new NodeIn(expr, ParsePrimaryExpr(lexer), pos), pos);
            }

            if (lexer.MatchIf("not", "in"))
            {
                return new NodeNot(new NodeIn(expr, ParsePrimaryExpr(lexer), pos), pos);
            }

            if (lexer.MatchIf("is", "in"))
            {
                return new NodeIn(expr, ParsePrimaryExpr(lexer), pos);
            }

            if (lexer.MatchIf("in"))
            {
                return new NodeIn(expr, ParsePrimaryExpr(lexer), pos);
            }

            if (lexer.MatchIf("starts", "not", "with"))
            {
                return new NodeNot(FuncCall("starts_with", "str", expr, "part", ParsePrimaryExpr(lexer), pos), pos);
            }

            if (lexer.MatchIf("starts", "with"))
            {
                return FuncCall("starts_with", "str", expr, "part", ParsePrimaryExpr(lexer), pos);
            }

            if (lexer.MatchIf("ends", "not", "with"))
            {
                return new NodeNot(FuncCall("ends_with", "str", expr, "part", ParsePrimaryExpr(lexer), pos), pos);
            }

            if (lexer.MatchIf("ends", "with"))
            {
                return FuncCall("ends_with", "str", expr, "part", ParsePrimaryExpr(lexer), pos);
            }

            if (lexer.MatchIf("contains", "not"))
            {
                return new NodeNot(FuncCall("contains", "str", expr, "part", ParsePrimaryExpr(lexer), pos), pos);
            }

            if (lexer.MatchIf("contains"))
            {
                return FuncCall("contains", "str", expr, "part", ParsePrimaryExpr(lexer), pos);
            }

            if (lexer.MatchIf("matches", "not"))
            {
                return new NodeNot(FuncCall("matches", "str", expr, "pattern", ParsePrimaryExpr(lexer), pos), pos);
            }

            if (lexer.MatchIf("matches"))
            {
                return FuncCall("matches", "str", expr, "pattern", ParsePrimaryExpr(lexer), pos);
            }

            return expr;
        }

        private Node CollectPredicateMinMaxExact(string fn, Node expr, Lexer lexer, SourcePos pos)
        {
            Node min_len = new NodeLiteralInt(0, pos);
            Node max_len = new NodeLiteralInt(9999, pos);
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
                    result = new NodeLiteralString(token.value, token.pos);
                    result = DerefOrInvoke(lexer, result);
                    break;
                case TokenType.Int:
                    result = new NodeLiteralInt(Convert.ToInt64(token.value) * (unary_minus ? -1 : 1), token.pos);
                    result = Invoke(lexer, result);
                    break;
                case TokenType.Decimal:
                    result = new NodeLiteralDecimal(Convert.ToDecimal(token.value) * (unary_minus ? -1 : 1), token.pos);
                    result = Invoke(lexer, result);
                    break;
                case TokenType.Boolean:
                    result = new NodeLiteralBoolean(token.value == "TRUE", token.pos);
                    result = Invoke(lexer, result);
                    break;
                case TokenType.Pattern:
                    result = new NodeLiteralPattern(token.value.Substring(2, token.value.Length - 4), token.pos);
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
                        result = new NodeReturn(ParseExpression(lexer), token.pos);
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
                key = new NodeLiteralString(((NodeIdentifier) key).GetValue(), key.GetSourcePos());
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
                    key = new NodeLiteralString(((NodeIdentifier) key).GetValue(), key.GetSourcePos());
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
                        fn = new NodeDeref(fn, new NodeLiteralString(lexer.MatchIdentifier(), lexer.GetPos()), lexer.GetPos());
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
                var index = new NodeLiteralString(identifier, pos);
                if (lexer.MatchIf("=", TokenType.Operator)) 
                {
                    var value = this.ParseExpression(lexer);
                    result.node = new NodeDerefAssign(node, index, value, pos);
                    result.interrupt = true;
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
        
        private Node FuncCall(string fn, string a, Node exprA, string b, Node exprB, SourcePos pos)
        {
            var result = new NodeFuncall(new NodeIdentifier(fn, pos), pos);
            result.AddArg(a, exprA);
            result.AddArg(b, exprB);
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