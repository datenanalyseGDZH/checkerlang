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
using System.Text;

namespace CheckerLang
{
    public class Lexer
    {
        private string filename;
        private List<Token> tokens = new List<Token>();
        private int nextToken;

        public Lexer(TextReader reader, string filename)
        {
            this.filename = filename;
            Lex(reader);
        }

        public bool HasNext()
        {
            return nextToken < tokens.Count;
        }

        public Token Next()
        {
            return tokens[nextToken++];
        }

        public Token Peek()
        {
            return tokens[nextToken];
        }

        public bool Peekn(int n, string token, TokenType type)
        {
            if (nextToken + n - 1 < tokens.Count)
            {
                return tokens[nextToken + n - 1].value == token && tokens[nextToken + n - 1].type == type;
            }

            return false;
        }

        public bool Peekn(int n, string token)
        {
            if (nextToken + n - 1 < tokens.Count)
            {
                var t = tokens[nextToken + n - 1]; 
                return t.value == token && (t.type == TokenType.Identifier || t.type == TokenType.Keyword);
            }

            return false;
        }


        public bool Peek(string a, TokenType type)
        {
            return Peekn(1, a, type);
        }

        public bool PeekOne(string a, string b, TokenType type)
        {
            return Peekn(1, a, type) || Peekn(1, b, type);
        }

        public bool PeekOne(string a, string b, string c, TokenType type)
        {
            return Peekn(1, a, type) || Peekn(1, b, type) || Peekn(1, c, type);
        }

        public SourcePos GetPos()
        {
            if (nextToken == 0) return GetPosNext();
            return tokens[nextToken - 1].pos;
        }
        
        public SourcePos GetPosNext()
        {
            if (!HasNext()) return GetPos();
            return tokens[nextToken].pos;
        }
        
        public string MatchIdentifier()
        {
            if (!HasNext()) throw new SyntaxError("Expected identifier but reached end of input", GetPos());
            var token = Next();
            if (token.type != TokenType.Identifier)
            {
                throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
            }
            return token.value;
        }

        public bool MatchIf(string a, TokenType type)
        {
            if (Peekn(1, a, type))
            {
                Eat(1);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, TokenType type)
        {
            if (Peekn(1, a, type) && Peekn(2, b, type))
            {
                Eat(2);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, TokenType typea, string b, TokenType typeb)
        {
            if (Peekn(1, a, typea) && Peekn(2, b, typeb))
            {
                Eat(2);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, TokenType typea, TokenType typeb)
        {
            if (Peekn(1, a, typea) && Peekn(2, b, typeb))
            {
                Eat(2);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c, TokenType typea, TokenType typeb, TokenType typec)
        {
            if (Peekn(1, a, typea) && Peekn(2, b, typeb) && Peekn(3, c, typec))
            {
                Eat(3);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c, TokenType type)
        {
            if (Peekn(1, a, type) && Peekn(2, b, type) && Peekn(3, c, type))
            {
                Eat(3);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c, string d, TokenType type)
        {
            if (Peekn(1, a, type) && Peekn(2, b, type) && Peekn(3, c, type) && Peekn(4, d, type))
            {
                Eat(4);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c, string d, string e, TokenType type)
        {
            if (Peekn(1, a, type) && Peekn(2, b, type) && Peekn(3, c, type) && Peekn(4, d, type) && Peekn(5, e, type))
            {
                Eat(5);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a)
        {
            if (Peekn(1, a))
            {
                Eat(1);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b)
        {
            if (Peekn(1, a) && Peekn(2, b))
            {
                Eat(2);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c)
        {
            if (Peekn(1, a) && Peekn(2, b) && Peekn(3, c))
            {
                Eat(3);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c, string d)
        {
            if (Peekn(1, a) && Peekn(2, b) && Peekn(3, c) && Peekn(4, d))
            {
                Eat(4);
                return true;
            }

            return false;
        }

        public bool MatchIf(string a, string b, string c, string d, string e)
        {
            if (Peekn(1, a) && Peekn(2, b) && Peekn(3, c) && Peekn(4, d) && Peekn(5, e))
            {
                Eat(5);
                return true;
            }

            return false;
        }

        
        public void Eat(int n)
        {
            nextToken += n;
        }

        public void Match(string expected, TokenType type)
        {
            if (!HasNext())
            {
                throw new SyntaxError("Unexpected end of input", GetPos());
            }
            var token = Next();
            if (token.value != expected || token.type != type)
            {
                throw new SyntaxError("Expected " + expected + " but got " + token, token.pos);
            }
        }

        public void Putback()
        {
            if (nextToken == 0) throw new Exception("Cannot putback at first token");
            nextToken--;
        }

        // Perform lexical analysis using a classical deterministic finite state machine.
        private void Lex(TextReader reader)
        {
            var tempbuf = "";
            var token = new StringBuilder();
            var state = 0;
            var c = reader.Read();
            var lastc = -1;
            var line = 1;
            var column = 0;
            var updatepos = true;
            while (c != -1)
            {
                var ch = (char) c;
                if (updatepos)
                {
                    if (ch == '\n')
                    {
                        line++;
                        column = 0;
                    }
                    else 
                    {
                        column++;
                    }
                }
                switch (state)
                {
                    case 0: // Eat whitespace
                        if (ch == '#')
                        {
                            state = 9;
                        }
                        else if ("+-*%".IndexOf(ch) != -1)
                        {
                            token.Append(ch);
                            state = 10;
                        }
                        else if ("()[],;".IndexOf(ch) != -1)
                        {
                            tokens.Add(new Token(ch.ToString(), TokenType.Interpunction, new SourcePos(filename, line, column)));
                        }
                        else if (ch == '/')
                        {
                            state = 5;
                        }
                        else if ("<>=!".IndexOf(ch) != -1)
                        {
                            token.Append(ch);
                            state = 2;
                        }
                        else if (ch == '"')
                        {
                            state = 3;
                        }
                        else if (ch == '\'')
                        {
                            state = 4;
                        }
                        else if (ch == '0') 
                        {
                            state = 70;
                        }
                        else if ("123456789".IndexOf(ch) != -1)
                        {
                            token.Append(ch);
                            state = 7;
                        }
                        else if (" \t\r\n".IndexOf(ch) == -1)
                        {
                            token.Append(ch);
                            state = 1;
                        }

                        break;
                    case 1: // normal token
                        if ("()+-*/%[]<>=,;!\"' \t\r\n#".IndexOf(ch) != -1)
                        {
                            if (token.Length > 0)
                            {
                                var t = token.ToString();
                                if (t == "TRUE") tokens.Add(new Token("TRUE", TokenType.Boolean, new SourcePos(filename, line, column - "TRUE".Length)));
                                else if (t == "FALSE") tokens.Add(new Token("FALSE", TokenType.Boolean, new SourcePos(filename, line, column - "FALSE".Length)));
                                else if (Keywords.IsKeyword(t)) tokens.Add(new Token(t, TokenType.Keyword, new SourcePos(filename, line, column - t.Length)));
                                else tokens.Add(new Token(t, TokenType.Identifier, new SourcePos(filename, line, column - t.Length)));
                                token = new StringBuilder();
                            }

                            lastc = c;
                            state = 0;
                        }
                        else
                        {
                            token.Append(ch);
                            if (token.ToString() == "...")
                            {
                                tokens.Add(new Token(token.ToString(), TokenType.Interpunction, new SourcePos(filename, line, column - token.Length)));
                                token = new StringBuilder();
                                state = 0;
                            }
                        }

                        break;
                    case 2: // <>, <=, >=, ==, <<, >>, <<<, >>>, =>
                        if (ch == '=')
                        {
                            token.Append(ch);
                            tokens.Add(new Token(token.ToString(), TokenType.Operator, new SourcePos(filename, line, column - token.Length + 1)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '>' && token.ToString() == "=")
                        {
                            token.Append(ch);
                            tokens.Add(new Token(token.ToString(), TokenType.Interpunction, new SourcePos(filename, line, column - token.Length + 1)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '>' && token.ToString() == "<")
                        {
                            tokens.Add(new Token("<>", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '*' && token.ToString() == "<")
                        {
                            tokens.Add(new Token("<*", TokenType.Interpunction, new SourcePos(filename, line, column - 1)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '>' && token.ToString() == "!")
                        {
                            tokens.Add(new Token("!>", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '<' && token.ToString() == "<")
                        {
                            token.Append(ch);
                            state = 21;
                        }
                        else if (ch == '>' && token.ToString() == ">")
                        {
                            token.Append(ch);
                            state = 21;
                        }
                        else
                        {
                            tokens.Add(new Token(token.ToString(), TokenType.Operator, new SourcePos(filename, line, column - token.Length)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        }

                        break;
                    
                    case 21: // <<, >>, <<<, >>>
                        if (ch == '<' && token.ToString() == "<<")
                        {
                            tokens.Add(new Token("<<<", TokenType.Interpunction, new SourcePos(filename, line, column - 3)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '>' && token.ToString() == ">>")
                        {
                            tokens.Add(new Token(">>>", TokenType.Interpunction, new SourcePos(filename, line, column - 3)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else
                        {
                            tokens.Add(new Token(token.ToString(), TokenType.Interpunction, new SourcePos(filename, line, column - token.Length)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        }                        
                        
                        break;
                    case 3: // double quotes
                        if (ch == '"')
                        {
                            tokens.Add(new Token(token.ToString(), TokenType.String, new SourcePos(filename, line, column - token.Length - 2 + 1)));
                            token = new StringBuilder();
                            state = 0;
                        } 
                        else if (ch == '\\') 
                        {
                            state = 31;
                        }
                        else
                        {
                            token.Append(ch);
                        }

                        break;
                    case 31: // double quotes escapes
                        if (ch == 'n') {
                            token.Append("\n");
                            state = 3;
                        } else if (ch == 'r') {
                            token.Append('\r');
                            state = 3;
                        } else if (ch == 't') {
                            token.Append('\t');
                            state = 3;
                        } else if (ch == 'x') {
                            state = 311;
                        } else {
                            token.Append(ch);
                            state = 3;
                        }

                        break;
                    case 311:
                        tempbuf = ch.ToString();
                        state = 312;

                        break;
                    case 312:
                        tempbuf += ch.ToString();
                        token.Append(Char.ConvertFromUtf32(Convert.ToInt32(tempbuf, 16)));
                        state = 3;

                        break;
                    case 4: // single quote
                        if (ch == '\'')
                        {
                            tokens.Add(new Token(token.ToString(), TokenType.String, new SourcePos(filename, line, column - token.Length - 2 + 1)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (ch == '\\') 
                        {
                            state = 41;
                        }
                        else
                        {
                            token.Append(ch);
                        }

                        break;
                    case 41: // single quotes escapes
                        if (ch == 'n') {
                            token.Append("\n");
                            state = 4;
                        } else if (ch == 'r') {
                            token.Append('\r');
                            state = 4;
                        } else if (ch == 't') {
                            token.Append('\t');
                            state = 4;
                        } else if (ch == 'x') {
                            state = 411;
                        } else {
                            token.Append(ch);
                            state = 4;
                        }

                        break;
                    case 411:
                        tempbuf = ch.ToString();
                        state = 412;

                        break;
                    case 412:
                        tempbuf += ch.ToString();
                        token.Append(Char.ConvertFromUtf32(Convert.ToInt32(tempbuf, 16)));
                        state = 4;

                        break;
                    case 5: // check for pattern
                        if (ch == '/')
                        {
                            token.Append("//");
                            state = 6;
                        } 
                        else if (ch == '=') 
                        {
                            tokens.Add(new Token("/=", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                            state = 0;
                        }
                        else
                        {
                            tokens.Add(new Token("/", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                            lastc = c;
                            state = 0;
                        }

                        break;
                    case 6: // pattern
                        token.Append(ch);
                        if (token.ToString().EndsWith("//"))
                        {
                            tokens.Add(new Token(token.ToString(), TokenType.Pattern, new SourcePos(filename, line, column - token.Length - 4 + 1)));
                            token = new StringBuilder();
                            state = 0;
                        }

                        break;
                    case 7: // int or decimal
                        if (ch == '.')
                        {
                            token.Append(ch);
                            state = 8;
                        }
                        else if ("0123456789_".IndexOf(ch) != -1)
                        {
                            token.Append(ch);
                        }
                        else if ("()[]<>=! \t\n\r+-*/%,;#".IndexOf(ch) != -1)
                        {
                            tokens.Add(new Token(token.ToString().Replace("_", ""), TokenType.Int, new SourcePos(filename, line, column - token.Length)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        }
                        else
                        {
                            token.Append(ch);
                            state = 1;
                        }

                        break;
                    case 70: // int, decimal or hex/binary int literal
                        if (ch == 'x') {
                            state = 71; // hex int literal
                        } else if (ch == 'b') {
                            state = 72; // binary int literal
                        } else {
                            token.Append('0');
                            lastc = c;
                            state = 7;
                        }

                        break;
                    case 71: // hex int literal
                        if ("0123456789abcdefABCDEF_".IndexOf(ch) != -1) {
                            token.Append(ch);
                        } else if ("()[]<>=! \t\n\r+-*/%,;#".IndexOf(ch) != -1) {
                            tokens.Add(new Token(Convert.ToUInt64(token.ToString().Replace("_", ""), 16).ToString(), TokenType.Int, new SourcePos(filename, line, column - token.Length)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        } else {
                            token.Append(ch);
                            state = 1;
                        } 

                        break;
                    case 72: // binary int literal
                        if ("01_".IndexOf(ch) != -1) {
                            token.Append(ch);
                        } else if ("()[]<>=! \t\n\r+-*/%,;#".IndexOf(ch) != -1) {
                            tokens.Add(new Token(Convert.ToUInt64(token.ToString().Replace("_", ""), 2).ToString(), TokenType.Int, new SourcePos(filename, line, column - token.Length)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        } else {
                            token.Append(ch);
                            state = 1;
                        } 

                        break;
                    case 8: // decimal
                        if ("0123456789_".IndexOf(ch) != -1)
                        {
                            token.Append(ch);
                        }
                        else if ("()[]<>=! \t\n\r+-*/%,;#".IndexOf(ch) != -1)
                        {
                            tokens.Add(new Token(token.ToString().Replace("_", ""), TokenType.Decimal, new SourcePos(filename, line, column - token.Length)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        }
                        else
                        {
                            token.Append(ch);
                            state = 1;
                        }

                        break;
                    case 9: // comment
                        if (ch == '\n')
                        {
                            state = 0;
                        }

                        break;

                    case 10: // potentially composite assign, ->, *>
                        if (ch == '=')
                        {
                            token.Append(ch);
                            tokens.Add(new Token(token.ToString(), TokenType.Operator,
                                new SourcePos(filename, line, column)));
                            token = new StringBuilder();
                            state = 0;
                        }
                        else if (token.ToString() == "-" && ch == '>')
                        {
                            tokens.Add(new Token("->", TokenType.Operator, new SourcePos(filename, line, column)));
                            token = new StringBuilder();
                            state = 0;
                        } 
                        else if (token.ToString() == "*" && ch == '>')
                        {
                            tokens.Add(new Token("*>", TokenType.Interpunction, new SourcePos(filename, line, column)));
                            token = new StringBuilder();
                            state = 0;
                        } 
                        else 
                        {
                            tokens.Add(new Token(token.ToString(), TokenType.Operator, new SourcePos(filename, line, column)));
                            token = new StringBuilder();
                            lastc = c;
                            state = 0;
                        }
                        
                        break;
                }

                if (lastc != -1)
                {
                    c = lastc;
                    lastc = -1;
                    updatepos = false;
                }
                else
                {
                    c = reader.Read();
                    updatepos = true;
                }
            }

            if (token.Length > 0)
            {
                var t = token.ToString();
                switch (state)
                {
                    case 1:
                        if (t == "TRUE") tokens.Add(new Token("TRUE", TokenType.Boolean, new SourcePos(filename, line, column - "TRUE".Length)));
                        else if (t == "FALSE") tokens.Add(new Token("FALSE", TokenType.Boolean, new SourcePos(filename, line, column - "FALSE".Length)));
                        else if (Keywords.IsKeyword(t)) tokens.Add(new Token(t, TokenType.Keyword, new SourcePos(filename, line, column - t.Length)));
                        else if (Operators.IsOperator(t)) tokens.Add(new Token(t, TokenType.Operator, new SourcePos(filename, line, column - t.Length)));
                        else tokens.Add(new Token(t, TokenType.Identifier, new SourcePos(filename, line, column - t.Length)));
                        break;
                    case 21:
                        tokens.Add(new Token(t, TokenType.Interpunction, new SourcePos(filename, line, column - t.Length)));
                        break;
                    case 3:
                    case 4:
                        tokens.Add(new Token(t, TokenType.String, new SourcePos(filename, line, column - t.Length - 2)));
                        break;
                    case 7:
                        tokens.Add(new Token(t.Replace("_", ""), TokenType.Int, new SourcePos(filename, line, column - t.Length)));
                        break;
                    case 71:
                        tokens.Add(new Token(Convert.ToUInt64(t.Replace("_", ""), 16).ToString(), TokenType.Int, new SourcePos(filename, line, column - t.Length)));
                        break;
                    case 72:
                        tokens.Add(new Token(Convert.ToUInt64(t.Replace("_", ""), 2).ToString(), TokenType.Int, new SourcePos(filename, line, column - t.Length)));
                        break;
                    case 8:
                        tokens.Add(new Token(t.Replace("_", ""), TokenType.Decimal, new SourcePos(filename, line, column - t.Length)));
                        break;
                    case 9: // ignore comment
                        break;
                    default:
                        if (Keywords.IsKeyword(t)) tokens.Add(new Token(t, TokenType.Keyword, new SourcePos(filename, line, column - t.Length)));
                        else if (Operators.IsOperator(t)) tokens.Add(new Token(t, TokenType.Operator, new SourcePos(filename, line, column - t.Length)));
                        else tokens.Add(new Token(t, TokenType.Identifier, new SourcePos(filename, line, column - t.Length)));
                        break;
                }
            }
            else if (state == 70) 
            {
                tokens.Add(new Token("0", TokenType.Int, new SourcePos(filename, line, column - 1)));
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[");
            foreach (var token in tokens)
            {
                builder.Append(token).Append(", ");
            }
            if (tokens.Count > 0) builder.Remove(builder.ToString().Length - 2, 2);
            builder.Append("]");
            return builder + " @ " + nextToken;
        }

        public string GetFilename()
        {
            return filename;
        }
    }
    
}