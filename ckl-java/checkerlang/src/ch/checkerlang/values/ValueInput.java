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
package ch.checkerlang.values;

import ch.checkerlang.ControlErrorException;
import ch.checkerlang.SourcePos;

import java.io.BufferedReader;
import java.io.IOException;
import java.util.function.Function;

public class ValueInput extends Value {
    private BufferedReader input;
    private boolean closed;

    public ValueInput(BufferedReader input) {
        this.input = input;
        this.closed = false;
    }

    public int process(Function<String, Value> callback) {
        try {
            String line = input.readLine();
            int count = 0;
            while (line != null) {
                count++;
                callback.apply(line);
                line = input.readLine();
            }
            return count;
        } catch (IOException e) {
            throw new ControlErrorException("Cannot process file", SourcePos.Unknown);
        }
    }

    public String readLine() throws IOException {
        return input.readLine();
    }

    public String read() throws IOException {
        int ch = input.read();
        return ch == -1 ? null : Character.toString((char) ch);
    }

    public String readAll() throws IOException {
        StringBuilder result = new StringBuilder();
        char[] buffer = new char[10240];
        int read = input.read(buffer);
        if (read == -1) return null;
        while (read != -1) {
            result.append(buffer, 0, read);
            read = input.read(buffer);
        }
        return result.toString();
    }

    public void close() throws IOException {
        if (closed) return;
        input.close();
        closed = true;
    }

    public boolean isEquals(Value value) {
        return value == this;
    }

    public int compareTo(Value value) {
        return toString().compareTo(value.toString());
    }

    public int hashCode() {
        return 0;
    }

    public String type() {
        return "input";
    }

    public ValueString asString() {
        return new ValueString(toString());
    }

    public ValueInput asInput() {
        return this;
    }

    public boolean isInput() {
        return true;
    }

    public String toString() {
        return "<!input-stream>";
    }

}
