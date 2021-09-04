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

import ch.checkerlang.OutputCallback;

import java.io.IOException;
import java.io.Writer;

public class ValueOutput extends Value {
    private Writer output;
    private OutputCallback callback;
    private boolean closed;

    public ValueOutput(OutputCallback callback) {
        this.callback = callback;
        this.closed = false;
    }

    public ValueOutput(Writer output) {
        this.output = output;
        this.closed = false;
    }

    public void write(String value) throws IOException {
        if (output != null) output.write(value);
        if (callback != null) callback.append_(value);
    }

    public void writeLine(String value) throws IOException {
        if (output != null) {
            output.write(value);
            output.write("\n");
            output.flush();
        }
        if (callback != null) {
            callback.append(value);
        }
    }

    public void close() throws Exception {
        if (closed) return;
        if (output != null) output.close();
        if (callback != null) callback.close();
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
        return "output";
    }

    public ValueOutput asOutput() {
        return this;
    }

    public boolean isOutput() {
        return true;
    }

    public String toString() {
        return "<!output-stream>";
    }

    public String getStringOutput() {
        // This does only really work for string output objects...
        return output.toString();
    }
}
