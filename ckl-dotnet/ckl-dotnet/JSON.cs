using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CheckerLang
{
    /**
     *  This is a partial parser for JSON objects. It parses lists, dictionaries/maps, strings,
     *  ints, doubles (without scientific notation), booleans and null into corresponding CKL values.
     */
    public class JSON
    {
        public static Value Parse(string data) 
        {
            return Parse(new Lexer(data));
        }

        private static Value Parse(Lexer lexer) 
        {
            try {
                var token = lexer.NextToken();
                if (token == "[") 
                {
                    var result = new ValueList();
                    var next = lexer.NextToken();
                    while (next != "]") 
                    {
                        lexer.PutBackToken(next);
                        result.AddItem(Parse(lexer));
                        next = lexer.NextToken();
                        if (next != "]") 
                        {
                            lexer.PutBackToken(next);
                            lexer.MatchToken(",");
                            next = lexer.NextToken();
                        }
                    }
                    return result;
                }
                if (token == "{") 
                {
                    var result = new ValueMap();
                    var next = lexer.NextToken();
                    while (next != "}") 
                    {
                        lexer.PutBackToken(next);
                        var key = lexer.NextToken();
                        if (!key.StartsWith("\"")) throw new Exception("Expected object key, but got " + key);
                        key = key.Substring(1, key.Length - 2);
                        lexer.MatchToken(":");
                        result.AddItem(new ValueString(key), Parse(lexer));
                        next = lexer.NextToken();
                        if (next != "}") 
                        {
                            lexer.PutBackToken(next);
                            lexer.MatchToken(",");
                            next = lexer.NextToken();
                        }
                    }
                    return result;
                }
                if (token.StartsWith("\"")) 
                {
                    return new ValueString(token.Substring(1, token.Length - 2));
                }
                if (token == "true" || token == "false") 
                {
                    return ValueBoolean.From(bool.Parse(token));
                }
                if (token == "null") 
                {
                    return ValueNull.NULL;
                }
                if (token.IndexOf('.') == -1) 
                {
                    return new ValueInt(long.Parse(token));
                }
                return new ValueDecimal(decimal.Parse(token));
            } catch (Exception e) {
                return null;
            }
        }

        public class Lexer {
            private TextReader inp;
            private List<string> tokens = new List<string>();
            private int lastc = -1;

            public Lexer(string data) {
                inp = new StringReader(data);
            }

            public void PutBackToken(string token) {
                tokens.Add(token);
            }

            public string NextToken() {
                if (tokens.Count > 0)
                {
                    var value = tokens[tokens.Count - 1];
                    tokens.RemoveAt(tokens.Count - 1);
                    return value;
                }
                var result = new StringBuilder();
                var state = 0;
                int c;
                if (lastc != -1)
                {
                    c = lastc;
                    lastc = -1;
                }
                else
                {
                    c = inp.Read();
                }
                while (c != -1) {
                    var ch = (char) c;
                    switch (state) {
                        case 0: // skip whitespace
                            if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') 
                            {
                                break;
                            } 
                            else if (ch == '"') 
                            {
                                state = 1;
                                result.Append(ch);
                            } 
                            else if (Char.IsDigit(ch) || ch == '+' || ch == '-') 
                            {
                                state = 2;
                                result.Append(ch);
                            } 
                            else if (ch == ':' || ch == ',' || ch == '{' || ch == '}' || ch == '[' || ch == ']') 
                            {
                                result.Append(ch);
                                state = -1;
                            } 
                            else 
                            {
                                state = 3;
                                result.Append(ch);
                            }
                            break;

                        case 1: // string
                            if (ch == '\\') 
                            {
                                state = 4;
                            } 
                            else if (ch == '"') 
                            {
                                result.Append(ch);
                                state = -1;
                            } 
                            else 
                            {
                                result.Append(ch);
                            }
                            break;

                        case 2: // integer
                            if (Char.IsDigit(ch)) 
                            {
                                result.Append(ch);
                            } 
                            else if (ch == '.') 
                            {
                                result.Append(ch);
                                state = 21;
                            } 
                            else
                            {
                                lastc = c;
                                state = -1;
                            }
                            break;

                        case 21: // double
                            if (Char.IsDigit(ch)) 
                            {
                                result.Append(ch);
                            } 
                            else
                            {
                                lastc = c;
                                state = -1;
                            }
                            break;

                        case 3: // keyword
                            if ('a' <= ch && ch <= 'z') 
                            {
                                result.Append(ch);
                            } 
                            else
                            {
                                lastc = c;
                                state = -1;
                            }
                            break;

                        case 4: // escape
                            if (ch == '\\') {
                                result.Append(ch);
                            } else if (ch == '\"') {
                                result.Append('\"');
                            } else if (ch == 'b') {
                                result.Append('\b');
                            } else if (ch == 'f') {
                                result.Append('\f');
                            } else if (ch == 'n') {
                                result.Append('\n');
                            } else if (ch == 'r') {
                                result.Append('\r');
                            } else if (ch == 't') {
                                result.Append('\t');
                            } else if (ch == 'u') {
                                c = inp.Read();
                                if (c != -1) {
                                    var ch1 = (char) c;
                                    c = inp.Read();
                                    if (c != -1) {
                                        var ch2 = (char) c;
                                        c = inp.Read();
                                        if (c != -1) {
                                            var ch3 = (char) c;
                                            c = inp.Read();
                                            if (c != -1) {
                                                var ch4 = (char) c;
                                                var s = "" + ch1 + ch2 + ch3 + ch4;
                                                result.Append((char) int.Parse(s, System.Globalization.NumberStyles.HexNumber));
                                            }
                                        }
                                    }
                                }
                            } else {
                                result.Append("\\").Append(ch);
                            }
                            state = 1;
                            break;
                    }
                    if (state == -1) break;
                    if (lastc != -1)
                    {
                        c = lastc;
                        lastc = -1;
                    }
                    else
                    {
                        c = inp.Read();
                    }
                }
                if (result.Length > 0) {
                    return result.ToString();
                } else {
                    return null;
                }
            }

            public void MatchToken(string token) 
            {
                var nexttoken = NextToken();
                if (token != nexttoken) throw new Exception("Expected " + token + " but got " + nexttoken);
            }
        }
    }
}