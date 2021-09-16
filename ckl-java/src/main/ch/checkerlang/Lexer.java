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

import java.io.IOException;
import java.io.Reader;
import java.util.ArrayList;
import java.util.List;

public class Lexer {
    private String filename;
    private List<Token> tokens = new ArrayList<>();
    private int nextToken;

    public Lexer(Reader reader, String filename) throws IOException {
        this.filename = filename;
        lex(reader);
    }

    public String getFilename() {
        return filename;
    }

    public boolean hasNext() {
        return nextToken < tokens.size();
    }

    public Token next() {
        return tokens.get(nextToken++);
    }

    public Token peek() {
        return tokens.get(nextToken);
    }

    public boolean peekn(int n, String token, TokenType type) {
        if (nextToken + n - 1 < tokens.size()) {
            return tokens.get(nextToken + n - 1).value.equals(token) && tokens.get(nextToken + n - 1).type == type;
        }

        return false;
    }

    public boolean peekn(int n, String token) {
        if (nextToken + n - 1 < tokens.size()) {
            Token t = tokens.get(nextToken + n - 1);
            return t.value.equals(token) && (t.type == TokenType.Identifier || t.type == TokenType.Keyword);
        }

        return false;
    }


    public boolean peek(String a, TokenType type) {
        return peekn(1, a, type);
    }

    public boolean peekOne(String a, String b, TokenType type) {
        return peekn(1, a, type) || peekn(1, b, type);
    }

    public boolean peekOne(String a, String b, String c, TokenType type) {
        return peekn(1, a, type) || peekn(1, b, type) || peekn(1, c, type);
    }

    public SourcePos getPos() {
        if (nextToken == 0) return getPosNext();
        return tokens.get(nextToken - 1).pos;
    }

    public SourcePos getPosNext() {
        if (!hasNext()) return getPos();
        return tokens.get(nextToken).pos;
    }

    public String matchIdentifier() {
        if (!hasNext()) throw new SyntaxError("Expected identifier but reached end of input", getPos());
        Token token = next();
        if (token.type != TokenType.Identifier) {
            throw new SyntaxError("Expected identifier but got '" + token + "'", token.pos);
        }
        return token.value;
    }

    public boolean matchIf(String a, TokenType type) {
        if (peekn(1, a, type)) {
            eat(1);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, TokenType type) {
        if (peekn(1, a, type) && peekn(2, b, type)) {
            eat(2);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, TokenType typea, String b, TokenType typeb) {
        if (peekn(1, a, typea) && peekn(2, b, typeb)) {
            eat(2);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, String c, TokenType type) {
        if (peekn(1, a, type) && peekn(2, b, type) && peekn(3, c, type)) {
            eat(3);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, String c, String d, TokenType type) {
        if (peekn(1, a, type) && peekn(2, b, type) && peekn(3, c, type) && peekn(4, d, type)) {
            eat(4);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, String c, String d, String e, TokenType type) {
        if (peekn(1, a, type) && peekn(2, b, type) && peekn(3, c, type) && peekn(4, d, type) && peekn(5, e, type)) {
            eat(5);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a) {
        if (peekn(1, a)) {
            eat(1);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b) {
        if (peekn(1, a) && peekn(2, b)) {
            eat(2);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, String c) {
        if (peekn(1, a) && peekn(2, b) && peekn(3, c)) {
            eat(3);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, String c, String d) {
        if (peekn(1, a) && peekn(2, b) && peekn(3, c) && peekn(4, d)) {
            eat(4);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, String c, String d, String e) {
        if (peekn(1, a) && peekn(2, b) && peekn(3, c) && peekn(4, d) && peekn(5, e)) {
            eat(5);
            return true;
        }

        return false;
    }

    public boolean matchIf(String a, String b, TokenType ta, TokenType tb) {
        if (peekn(1, a, ta) && peekn(2, b, tb)) {
            eat(2);
            return true;
        }
        return false;
    }

    public boolean matchIf(String a, String b, String c, TokenType ta, TokenType tb, TokenType tc) {
        if (peekn(1, a, ta) && peekn(2, b, tb) && peekn(3, c, tc)) {
            eat(3);
            return true;
        }
        return false;
    }

    public void eat(int n) {
        nextToken += n;
    }

    public void match(String expected, TokenType type) {
        if (!hasNext()) {
            throw new SyntaxError("Unexpected end of input", getPos());
        }
        Token token = next();
        if (!token.value.equals(expected) || token.type != type) {
            throw new SyntaxError("Expected " + expected + " but got " + token, token.pos);
        }
    }

    public void putback() {
        if (nextToken == 0) throw new RuntimeException("Cannot putback at first token");
        nextToken--;
    }

    // Perform lexical analysis using a classical deterministic finite state machine.
    private void lex(Reader reader) throws IOException {
        String tempbuf = "";
        StringBuilder token = new StringBuilder();
        int state = 0;
        int c = reader.read();
        int lastc = -1;
        int line = 1;
        int column = 0;
        boolean updatepos = true;
        while (c != -1) {
            char ch = (char) c;
            if (updatepos) {
                if (ch == '\n') {
                    line++;
                    column = 0;
                } else {
                    column++;
                }
            }
            switch (state) {
                case 0: // Eat whitespace
                    if (ch == '#') {
                        state = 9;
                    } else if ("+-*%".indexOf(ch) != -1) {
                        token.append(ch);
                        state = 10;
                    } else if ("()[],;".indexOf(ch) != -1) {
                        tokens.add(new Token(Character.toString(ch), TokenType.Interpunction, new SourcePos(filename, line, column)));
                    } else if (ch == '/') {
                        state = 5;
                    } else if ("<>=!".indexOf(ch) != -1) {
                        token.append(ch);
                        state = 2;
                    } else if (ch == '"') {
                        state = 3;
                    } else if (ch == '\'') {
                        state = 4;
                    } else if ("0123456789".indexOf(ch) != -1) {
                        token.append(ch);
                        state = 7;
                    } else if (" \t\r\n".indexOf(ch) == -1) {
                        token.append(ch);
                        state = 1;
                    }

                    break;
                case 1: // normal token
                    if ("()+-*/%[]<>=,;!\"' \t\r\n#".indexOf(ch) != -1) {
                        if (token.length() > 0) {
                            String t = token.toString();
                            if (t.equals("TRUE"))
                                tokens.add(new Token("TRUE", TokenType.Boolean, new SourcePos(filename, line, column - "TRUE".length())));
                            else if (t.equals("FALSE"))
                                tokens.add(new Token("FALSE", TokenType.Boolean, new SourcePos(filename, line, column - "FALSE".length())));
                            else if (Keywords.isKeyword(t))
                                tokens.add(new Token(t, TokenType.Keyword, new SourcePos(filename, line, column - t.length())));
                            else
                                tokens.add(new Token(t, TokenType.Identifier, new SourcePos(filename, line, column - t.length())));
                            token = new StringBuilder();
                        }

                        lastc = c;
                        state = 0;
                    } else {
                        token.append(ch);
                        if (token.toString().equals("...")) {
                            tokens.add(new Token(token.toString(), TokenType.Interpunction, new SourcePos(filename, line, column - token.length())));
                            token = new StringBuilder();
                            state = 0;
                        }
                    }

                    break;
                case 2: // <>, <=, >=, ==, <<, >>, <<<, >>>
                    if (ch == '=') {
                        token.append(ch);
                        tokens.add(new Token(token.toString(), TokenType.Operator, new SourcePos(filename, line, column - token.length() + 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '>' && token.toString().equals("=")) {
                        token.append(ch);
                        tokens.add(new Token(token.toString(), TokenType.Interpunction, new SourcePos(filename, line, column - token.length() + 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '*' && token.toString().equals("<")) {
                        token.append(ch);
                        tokens.add(new Token("<*", TokenType.Interpunction, new SourcePos(filename, line, column - token.length() + 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '>' && token.toString().equals("<")) {
                        tokens.add(new Token("<>", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '>' && token.toString().equals("!")) {
                        tokens.add(new Token("!>", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '<' && token.toString().equals("<")) {
                        token.append(ch);
                        state = 21;
                    } else if (ch == '>' && token.toString().equals(">")) {
                        token.append(ch);
                        state = 21;
                    } else {
                        tokens.add(new Token(token.toString(), TokenType.Operator, new SourcePos(filename, line, column - token.length())));
                        token = new StringBuilder();
                        lastc = c;
                        state = 0;
                    }

                    break;
                case 21: // <<, >>, <<<, >>>
                    if (ch == '<' && token.toString().equals("<<")) {
                        tokens.add(new Token("<<<", TokenType.Interpunction, new SourcePos(filename, line, column - 3)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '>' && token.toString().equals(">>")) {
                        tokens.add(new Token(">>>", TokenType.Interpunction, new SourcePos(filename, line, column - 3)));
                        token = new StringBuilder();
                        state = 0;
                    } else {
                        tokens.add(new Token(token.toString(), TokenType.Interpunction, new SourcePos(filename, line, column - token.length())));
                        token = new StringBuilder();
                        lastc = c;
                        state = 0;
                    }

                    break;
                case 3: // double quotes
                    if (ch == '"') {
                        tokens.add(new Token(token.toString(), TokenType.String, new SourcePos(filename, line, column - token.length() - 2 + 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '\\') {
                        state = 31;
                    } else {
                        token.append(ch);
                    }

                    break;
                case 31: // double quotes escapes
                    if (ch == 'n') {
                        token.append("\n");
                        state = 3;
                    } else if (ch == 'r') {
                        token.append('\r');
                        state = 3;
                    } else if (ch == 't') {
                        token.append('\t');
                        state = 3;
                    } else if (ch == 'x') {
                        state = 311;
                    } else {
                        token.append(ch);
                        state = 3;
                    }

                    break;
                case 311:
                    tempbuf = Character.toString(ch);
                    state = 312;

                    break;
                case 312:
                    tempbuf += Character.toString(ch);
                    token.append((char) Integer.parseInt(tempbuf, 16));
                    state = 3;

                    break;
                case 4: // single quote
                    if (ch == '\'') {
                        tokens.add(new Token(token.toString(), TokenType.String, new SourcePos(filename, line, column - token.length() - 2 + 1)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (ch == '\\') {
                        state = 41;
                    } else {
                        token.append(ch);
                    }

                    break;
                case 41: // single quotes escapes
                    if (ch == 'n') {
                        token.append("\n");
                        state = 4;
                    } else if (ch == 'r') {
                        token.append('\r');
                        state = 4;
                    } else if (ch == 't') {
                        token.append('\t');
                        state = 4;
                    } else {
                        token.append(ch);
                        state = 4;
                    }

                    break;
                case 5: // check for pattern
                    if (ch == '/') {
                        token.append("//");
                        state = 6;
                    } else if (ch == '=') {
                        tokens.add(new Token("/=", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                        state = 0;
                    } else {
                        tokens.add(new Token("/", TokenType.Operator, new SourcePos(filename, line, column - 1)));
                        lastc = c;
                        state = 0;
                    }

                    break;
                case 6: // pattern
                    token.append(ch);
                    if (token.toString().endsWith("//")) {
                        tokens.add(new Token(token.toString(), TokenType.Pattern, new SourcePos(filename, line, column - token.length() - 4 + 1)));
                        token = new StringBuilder();
                        state = 0;
                    }

                    break;
                case 7: // int or decimal
                    if (ch == '.') {
                        token.append(ch);
                        state = 8;
                    } else if ("0123456789".indexOf(ch) != -1) {
                        token.append(ch);
                    } else if ("()[]<>=! \t\n\r+-*/%,;#".indexOf(ch) != -1) {
                        tokens.add(new Token(token.toString(), TokenType.Int, new SourcePos(filename, line, column - token.length())));
                        token = new StringBuilder();
                        lastc = c;
                        state = 0;
                    } else {
                        token.append(ch);
                        state = 1;
                    }

                    break;
                case 8: // decimal
                    if ("0123456789".indexOf(ch) != -1) {
                        token.append(ch);
                    } else if ("()[]<>=! \t\n\r+-*/%,;#".indexOf(ch) != -1) {
                        tokens.add(new Token(token.toString(), TokenType.Decimal, new SourcePos(filename, line, column - token.length())));
                        token = new StringBuilder();
                        lastc = c;
                        state = 0;
                    } else {
                        token.append(ch);
                        state = 1;
                    }

                    break;
                case 9: // comment
                    if (ch == '\n') {
                        state = 0;
                    }

                    break;

                case 10: // potentially composite assign
                    if (ch == '=') {
                        token.append(ch);
                        tokens.add(new Token(token.toString(), TokenType.Operator, new SourcePos(filename, line, column)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (token.toString().equals("-") && ch == '>') {
                        tokens.add(new Token("->", TokenType.Operator, new SourcePos(filename, line, column)));
                        token = new StringBuilder();
                        state = 0;
                    } else if (token.toString().equals("*") && ch == '>') {
                        tokens.add(new Token("*>", TokenType.Interpunction, new SourcePos(filename, line, column)));
                        token = new StringBuilder();
                        state = 0;
                    } else {
                        tokens.add(new Token(token.toString(), TokenType.Operator, new SourcePos(filename, line, column)));
                        token = new StringBuilder();
                        lastc = c;
                        state = 0;
                    }
                    break;
            }

            if (lastc != -1) {
                c = lastc;
                lastc = -1;
                updatepos = false;
            } else {
                c = reader.read();
                updatepos = true;
            }
        }

        if (token.length() > 0) {
            String t = token.toString();
            switch (state) {
                case 1:
                    if (t.equals("TRUE")) {
                        tokens.add(new Token("TRUE", TokenType.Boolean, new SourcePos(filename, line, column - "TRUE".length())));
                    } else if (t.equals("FALSE")) {
                        tokens.add(new Token("FALSE", TokenType.Boolean, new SourcePos(filename, line, column - "FALSE".length())));
                    } else if (Keywords.isKeyword(t)) {
                        tokens.add(new Token(t, TokenType.Keyword, new SourcePos(filename, line, column - t.length())));
                    } else if (Operators.isOperator(t)) {
                        tokens.add(new Token(t, TokenType.Operator, new SourcePos(filename, line, column - t.length())));
                    } else tokens.add(new Token(t, TokenType.Identifier, new SourcePos(filename, line, column - t.length())));
                    break;
                case 21:
                    tokens.add(new Token(t, TokenType.Interpunction, new SourcePos(filename, line, column - t.length())));
                    break;
                case 3:
                case 4:
                case 31:
                case 41:
                    tokens.add(new Token(t, TokenType.String, new SourcePos(filename, line, column - t.length() - 2)));
                    break;
                case 7:
                    tokens.add(new Token(t, TokenType.Int, new SourcePos(filename, line, column - t.length())));
                    break;
                case 8:
                    tokens.add(new Token(t, TokenType.Decimal, new SourcePos(filename, line, column - t.length())));
                    break;
                case 9: // ignore comment
                    break;
                default:
                    if (Keywords.isKeyword(t)) {
                        tokens.add(new Token(t, TokenType.Keyword, new SourcePos(filename, line, column - t.length())));
                    } else if (Operators.isOperator(t)) {
                        tokens.add(new Token(t, TokenType.Operator, new SourcePos(filename, line, column - t.length())));
                    } else tokens.add(new Token(t, TokenType.Identifier, new SourcePos(filename, line, column - t.length())));
                    break;
            }
        }
    }

    @Override
    public String toString() {
        StringBuilder builder = new StringBuilder();
        builder.append("[");
        for (Token token : tokens) {
            builder.append(token).append(", ");
        }
        if (tokens.size() > 0) builder.setLength(builder.length() - 2);
        builder.append("]");
        return builder + " @ " + nextToken;
    }
}
