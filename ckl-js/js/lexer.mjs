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

import { SyntaxError } from "./errors.mjs";

export const Keywords = [
    "if", "then", "elif", "else", "and", "or", "not", 
    "is", "in", "def", "fn", "for", "while", 
    "do", "end", "finally", "break", "continue", 
    "return", "error"
];

export const Operators = [
    "+", "-", "*", "/", "%", 
    "==", "<>", "!=", "<", "<=", ">", ">=", 
    "=", "+=", "-=", "*=", "/=", "%=", 
    "!>", "->"
];

export class SourcePos {
    constructor(filename, line, column) {
        this.filename = filename;
        this.line = line;
        this.column = column; 
    }

    toString() {
        if (this.filename === null) return "-";
        return (this.filename !== "-" ? (this.filename + ":") : ":") + this.line + ":" + this.column;
    }
}

export class Token {
    constructor(value, type, pos) {
        this.value = value;
        this.type = type;
        this.pos = pos;
    }

    toString() {
        return this.value.replace(/\\/g, "\\\\").replace(/'/g, "\\'").replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\t/g, "\\t") + " (" + this.type + ")";
    }
}

export class Lexer {
    constructor(script, name) {
        this.script = script + ' ';
        this.name = name;
        this.tokens = [];
    }

    static init(script, name) {
        return new Lexer(script, name).scan();
    }

    hasNext() { return this.nextToken < this.tokens.length; }
    next() { return this.tokens[this.nextToken++]; }
    peek() { return this.tokens[this.nextToken]; }
    eat(n) { this.nextToken += n; }
    previous() { 
        if (this.nextToken === 0) throw new SyntaxError("Cannot go before beginning", this.getPos());
        this.nextToken--;
    }

    getPos() {
        if (this.nextToken == 0) return this.getPosNext();
        return this.tokens[this.nextToken - 1].pos;
    }

    getPosNext() {
        if (!this.hasNext()) return this.getPos();
        return this.tokens[this.nextToken].pos;
    }

    peekn(n, token, type = null) {
        if (this.nextToken + n - 1 < this.tokens.length) {
            const t = this.tokens[this.nextToken + n - 1];
            if (type === null) {
                return t.value === token && (t.type == "identifier" || t.type == "keyword");
            } else {
                return t.value === token && t.type === type;
            }
        }
        return false;
    }

    peekOne(n, tokens, type = null) {
        for (const token of tokens) {
            if (this.peekn(n, token, type)) return true;
        }
        return false;
    }

    matchIf(token, type = null) {
        if (token instanceof Array) {
            if (type instanceof Array) {
                for (let i = 0; i < token.length; i++) {
                    if (!this.peekn(i + 1, token[i], type[i])) {
                        return false;
                    }
                }
            } else {
                for (let i = 0; i < token.length; i++) {
                    if (!this.peekn(i + 1, token[i], type)) {
                        return false;
                    }
                }
            }
            this.eat(token.length);
            return true;
        } else {
            if (this.peekn(1, token, type)) {
                this.eat(1);
                return true;
            }
            return false;
        }
    }

    match(token, type) {
        if (!this.hasNext()) {
            throw new SyntaxError("Unexpected end of input", this.getPos());
        }
        const t = this.next();
        if (t.value !== token || t.type !== type) {
            throw new SyntaxError("Expected " + token + " but got " + t, t.pos);
        }
    }

    matchIdentifier() {
        if (!this.hasNext()) {
            throw new SyntaxError("Unexpected end of input", this.getPos());
        }
        const t = this.next();
        if (t.type !== "identifier") {
            throw new SyntaxError("Expected identifier but got " + t, t.pos);
        }
        return t.value;
    }

    scan() {
        this.tokens = [];
        this.nextToken = 0;
        
        const filename = this.name;

        let token = "";
        let state = 0;
        let pos = 0;
        let line = 1;
        let column = 0;
        let updatepos = true;
        while (pos <= this.script.length) {
            let ch = this.script.charAt(pos++);
            if (updatepos) {
                if (ch === '\n') {
                    line++;
                    column = 0;
                } else {
                    column++;
                }
            }
            updatepos = true;
            switch (state) {
                case 0: // Eat whitespace
                    if (ch === '#') {
                        state = 9;
                    } else if ("+-*%".indexOf(ch) !== -1) {
                        token = token.concat(ch);
                        state = 10;
                    } else if ("()[],;".indexOf(ch) !== -1) {
                        this.tokens.push(new Token(ch, "interpunction", new SourcePos(filename, line, column)));
                    } else if (ch === '/') {
                        state = 5;
                    } else if ("<>=!".indexOf(ch) !== -1) {
                        token = token.concat(ch);
                        state = 2;
                    } else if (ch === '"') {
                        state = 3;
                    } else if (ch === '\'') {
                        state = 4;
                    } else if ("0123456789".indexOf(ch) !== -1) {
                        token = token.concat(ch);
                        state = 7;
                    } else if (" \t\r\n".indexOf(ch) === -1) {
                        token = token.concat(ch);
                        state = 1;
                    }
                    break;

                case 1: // normal token
                    if ("()+-*/%[]<>=,;!\"' \t\r\n#".indexOf(ch) !== -1) {
                        if (token.length > 0) {
                            if (token === "TRUE") {
                                this.tokens.push(new Token("TRUE", "boolean", new SourcePos(filename, line, column - "TRUE".length)));
                            } else if (token === "FALSE") {
                                this.tokens.push(new Token("FALSE", "boolean", new SourcePos(filename, line, column - "FALSE".length)));
                            } else if (Keywords.indexOf(token) !== -1) {
                                this.tokens.push(new Token(token, "keyword", new SourcePos(filename, line, column - token.length)));
                            } else {
                                this.tokens.push(new Token(token, "identifier", new SourcePos(filename, line, column - token.length)));
                            }
                            token = "";
                        }
                        pos--;
                        updatepos = false;
                        state = 0;
                    } else {
                        token = token.concat(ch);
                        if (token === "...") {
                            this.tokens.push(new Token(token, "interpunction", new SourcePos(filename, line, column - token.length)));
                            token = "";
                            state = 0;
                        }
                    }
                    break;

                case 2: // <>, <=, >=, ==, <<, >>, <<<, >>>, !>
                    if (ch === '=') {
                        token = token.concat(ch);
                        this.tokens.push(new Token(token, "operator", new SourcePos(filename, line, column - token.length + 1)));
                        token = "";
                        state = 0;
                    } else if (ch === '>' && token === "=") {
                        token = token.concat(ch);
                        this.tokens.push(new Token(token, "interpunction", new SourcePos(filename, line, column - token.length + 1)));
                        token = "";
                        state = 0;
                    } else if (ch === '>' && token === "<") {
                        this.tokens.push(new Token("<>", "operator", new SourcePos(filename, line, column - 1)));
                        token = "";
                        state = 0;
                    } else if (ch === '<' && token === "<") {
                        token = token.concat(ch);
                        state = 21;
                    } else if (ch === '>' && token === ">") {
                        token = token.concat(ch);
                        state = 21;
                    } else if (ch === '>' && token === "!") {
                        token = token.concat(ch);
                        this.tokens.push(new Token("!>", "operator", new SourcePos(filename, line, column - token.length + 1)));
                        token = "";
                        state = 0;
                    } else {
                        this.tokens.push(new Token(token, "operator", new SourcePos(filename, line, column - token.length)));
                        token = "";
                        pos--;
                        updatepos = false;
                        state = 0;
                    }
                    break;

                case 21: // <<, >>, <<<, >>>
                    if (ch === '<' && token === "<<") {
                        this.tokens.push(new Token("<<<", "interpunction", new SourcePos(filename, line, column - 3)));
                        token = "";
                        state = 0;
                    } else if (ch === '>' && token === ">>") {
                        this.tokens.push(new Token(">>>", "interpunction", new SourcePos(filename, line, column - 3)));
                        token = "";
                        state = 0;
                    } else {
                        this.tokens.push(new Token(token, "interpunction", new SourcePos(filename, line, column - token.length)));
                        token = "";
                        pos--;
                        updatepos = false;
                        state = 0;
                    }
                    break;

                case 3: // double quotes
                    if (ch === '"') {
                        this.tokens.push(new Token(token, "string", new SourcePos(filename, line, column - token.length - 2 + 1)));
                        token = "";
                        state = 0;
                    } else if (ch === '\\') {
                        state = 31;
                    } else {
                        token = token.concat(ch);
                    }
                    break;

                case 31: // double quotes escapes
                    if (ch == 'n') {
                        token = token.concat('\n');
                        state = 3;
                    } else if (ch == 'r') {
                        token = token.concat('\r');
                        state = 3;
                    } else if (ch == 't') {
                        token = token.concat('\t');
                        state = 3;
                    } else {
                        token = token.concat(ch);
                        state = 3;
                    }
                    break;

                case 4: // single quote
                    if (ch === '\'') {
                        this.tokens.push(new Token(token, "string", new SourcePos(filename, line, column - token.length - 2 + 1)));
                        token = "";
                        state = 0;
                    } else if (ch === '\\') {
                        state = 41;
                    } else {
                        token = token.concat(ch);
                    }
                    break;


                case 41: // single quotes escapes
                    if (ch == 'n') {
                        token = token.concat('\n');
                        state = 4;
                    } else if (ch == 'r') {
                        token = token.concat('\r');
                        state = 4;
                    } else if (ch == 't') {
                        token = token.concat('\t');
                        state = 4;
                    } else {
                        token = token.concat(ch);
                        state = 4;
                    }
                    break;

                case 5: // check for pattern
                    if (ch === '/') {
                        token = token.concat("//");
                        state = 6;
                    } else if (ch === '=') {
                        this.tokens.push(new Token("/=", "operator", new SourcePos(filename, line, column - 1)));
                        state = 0;
                    } else {
                        this.tokens.push(new Token("/", "operator", new SourcePos(filename, line, column - 1)));
                        pos--;
                        updatepos = false;
                        state = 0;
                    }
                    break;

                case 6: // pattern
                    token = token.concat(ch);
                    if (token.endsWith("//")) {
                        this.tokens.push(new Token(token, "pattern", new SourcePos(filename, line, column - token.length - 4 + 1)));
                        token = "";
                        state = 0;
                    }
                    break;

                case 7: // int or decimal
                    if (ch === '.') {
                        token = token.concat(ch);
                        state = 8;
                    } else if ("0123456789".indexOf(ch) !== -1) {
                        token = token.concat(ch);
                    } else if ("()[]<>=! \t\n\r+-*/%,;#".indexOf(ch) !== -1) {
                        this.tokens.push(new Token(token, "int", new SourcePos(filename, line, column - token.length)));
                        token = "";
                        pos--;
                        updatepos = false;
                        state = 0;
                    } else {
                        token = token.concat(ch);
                        state = 1;
                    }
                    break;

                case 8: // decimal
                    if ("0123456789".indexOf(ch) !== -1) {
                        token = token.concat(ch);
                    } else if ("()[]<>=! \t\n\r+-*/%,;#".indexOf(ch) !== -1) {
                        this.tokens.push(new Token(token, "decimal", new SourcePos(filename, line, column - token.length)));
                        token = "";
                        pos--;
                        updatepos = false;
                        state = 0;
                    } else {
                        token = token.concat(ch);
                        state = 1;
                    }
                    break;

                case 9: // comment
                    if (ch === '\n') {
                        state = 0;
                    }
                    break;

                case 10: // potentially composite assign or ->
                    if (ch === '=') {
                        token = token.concat(ch);
                        this.tokens.push(new Token(token, "operator", new SourcePos(filename, line, column)));
                        token = "";
                        state = 0;
                    } else if (token === '-' && ch === '>') {
                        this.tokens.push(new Token("->", "operator", new SourcePos(filename, line, column)));
                        token = "";
                        state = 0;
                    } else {
                        this.tokens.push(new Token(token, "operator", new SourcePos(filename, line, column)));
                        token = "";
                        pos--;
                        updatepos = false;
                        state = 0;
                    }
                    break;
            }
        }
        return this;
    }

    toString() {
        let result = "[";
        for (const token of this.tokens) {
            result = result.concat(token, ", ");
        }
        if (result.length > 1) result = result.substr(0, result.length - 2);
        return result.concat("] @ ", this.nextToken.toString());
    }
}
