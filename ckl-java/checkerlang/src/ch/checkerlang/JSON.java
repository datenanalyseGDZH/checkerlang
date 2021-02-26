package ch.checkerlang;

import ch.checkerlang.values.*;

import java.io.*;
import java.util.ArrayList;
import java.util.List;

/**
 *  This is a partial parser for JSON objects. It parses lists, dictionaries/maps, strings,
 *  ints, doubles (without scientific notation), booleans and null into corresponding CKL values.
 */
public class JSON {

    public static Value parse(String data) {
        return parse(new Lexer(data));
    }

    private static Value parse(Lexer lexer) {
        try {
            String token = lexer.nextToken();
            if (token.equals("[")) {
                ValueList result = new ValueList();
                String next = lexer.nextToken();
                while (!next.equals("]")) {
                    lexer.putBackToken(next);
                    result.addItem(parse(lexer));
                    next = lexer.nextToken();
                    if (!next.equals("]")) {
                        lexer.putBackToken(next);
                        lexer.matchToken(",");
                        next = lexer.nextToken();
                    }
                }
                return result;
            } else if (token.equals("{")) {
                ValueMap result = new ValueMap();
                String next = lexer.nextToken();
                while (!next.equals("}")) {
                    lexer.putBackToken(next);
                    String key = lexer.nextToken();
                    if (!key.startsWith("\"")) throw new RuntimeException("Expected object key, but got " + key);
                    key = key.substring(1, key.length() - 1);
                    lexer.matchToken(":");
                    result.addItem(new ValueString(key), parse(lexer));
                    next = lexer.nextToken();
                    if (!next.equals("}")) {
                        lexer.putBackToken(next);
                        lexer.matchToken(",");
                        next = lexer.nextToken();
                    }
                }
                return result;
            } else if (token.startsWith("\"")) {
                return new ValueString(token.substring(1, token.length() - 1));
            } else if (token.equals("true") || token.equals("false")) {
                return ValueBoolean.from(Boolean.parseBoolean(token));
            } else if (token.equals("null")) {
                return ValueNull.NULL;
            } else if (token.indexOf('.') == -1) {
                return new ValueInt(Long.parseLong(token));
            } else {
                return new ValueDecimal(Double.parseDouble(token));
            }
        } catch (Exception e) {
            return null;
        }
    }

    public static class Lexer {
        private PushbackReader in;
        private List<String> tokens = new ArrayList<>();

        public Lexer(String data) {
            in = new PushbackReader(new StringReader(data), 1);
        }

        public void putBackToken(String token) {
            tokens.add(token);
        }

        public String nextToken() throws Exception {
            if (!tokens.isEmpty()) {
                return tokens.remove(tokens.size() - 1);
            }
            StringBuilder result = new StringBuilder();
            int state = 0;
            int c = in.read();
            while (c != -1) {
                char ch = (char) c;
                switch (state) {
                    case 0: // skip whitespace
                        if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') {
                            break;
                        } else if (ch == '"') {
                            state = 1;
                            result.append(ch);
                        } else if (Character.isDigit(ch) || ch == '+' || ch == '-') {
                            state = 2;
                            result.append(ch);
                        } else if (ch == ':' || ch == ',' || ch == '{' || ch == '}' || ch == '[' || ch == ']') {
                            result.append(ch);
                            state = -1;
                        } else {
                            state = 3;
                            result.append(ch);
                        }
                        break;

                    case 1: // string
                        if (ch == '\\') {
                            state = 4;
                        } else if (ch == '"') {
                            result.append(ch);
                            state = -1;
                        } else {
                            result.append(ch);
                        }
                        break;

                    case 2: // integer
                        if (Character.isDigit(ch)) {
                            result.append(ch);
                        } else if (ch == '.') {
                            result.append(ch);
                            state = 21;
                        } else {
                            in.unread(c);
                            state = -1;
                        }
                        break;

                    case 21: // double
                        if (Character.isDigit(ch)) {
                            result.append(ch);
                        } else {
                            in.unread(c);
                            state = -1;
                        }
                        break;

                    case 3: // keyword
                        if ('a' <= ch && ch <= 'z') {
                            result.append(ch);
                        } else {
                            in.unread(c);
                            state = -1;
                        }
                        break;

                    case 4: // escape
                        if (ch == '\\') {
                            result.append(ch);
                        } else if (ch == '\"') {
                            result.append('\"');
                        } else if (ch == 'b') {
                            result.append('\b');
                        } else if (ch == 'f') {
                            result.append('\f');
                        } else if (ch == 'n') {
                            result.append('\n');
                        } else if (ch == 'r') {
                            result.append('\r');
                        } else if (ch == 't') {
                            result.append('\t');
                        } else if (ch == 'u') {
                            c = in.read();
                            if (c != -1) {
                                char ch1 = (char) c;
                                c = in.read();
                                if (c != -1) {
                                    char ch2 = (char) c;
                                    c = in.read();
                                    if (c != -1) {
                                        char ch3 = (char) c;
                                        c = in.read();
                                        if (c != -1) {
                                            char ch4 = (char) c;
                                            String s = "" + ch1 + ch2 + ch3 + ch4;
                                            result.append(Character.valueOf((char) Integer.parseInt(s, 16)));
                                        }
                                    }
                                }
                            }
                        } else {
                            result.append("\\").append(ch);
                        }
                        state = 1;
                        break;
                }
                if (state == -1) break;
                c = in.read();
            }
            if (result.length() > 0) {
                return result.toString();
            } else {
                return null;
            }
        }

        public void matchToken(String token) throws Exception {
            String nexttoken = nextToken();
            if (!token.equals(nexttoken)) throw new RuntimeException("Expected " + token + " but got " + nexttoken);
        }
    }

}
