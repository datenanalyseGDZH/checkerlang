/*  Copyright (c) 2022 Damian Brunold, Gesundheitsdirektion Kanton Zürich

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

import java.util.HashSet;
import java.util.Set;

public class Keywords {
    private static Set<String> keywords = new HashSet<>();

    static {
        keywords.add("if");
        keywords.add("then");
        keywords.add("elif");
        keywords.add("else");
        keywords.add("and");
        keywords.add("xor");
        keywords.add("or");
        keywords.add("not");
        keywords.add("is");
        keywords.add("in");
        keywords.add("def");
        keywords.add("fn");
        keywords.add("for");
        keywords.add("while");
        keywords.add("do");
        keywords.add("end");
        keywords.add("catch");
        keywords.add("finally");
        keywords.add("break");
        keywords.add("continue");
        keywords.add("return");
        keywords.add("error");
        keywords.add("require");
        keywords.add("as");
        keywords.add("also");
        keywords.add("class");
    }

    public static boolean isKeyword(String s) {
        return keywords.contains(s);
    }
}
