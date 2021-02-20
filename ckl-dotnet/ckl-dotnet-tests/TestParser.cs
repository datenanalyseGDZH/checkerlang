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
using System.IO;
using CheckerLang;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestParser
    {
        [Test]
        public void TestSimple()
        {
            Assert.AreEqual("eins",
                Parse("eins"));
        }

        [Test]
        public void TestAddition()
        {
            Assert.AreEqual("(add 2, 3)",
                Parse("2 + 3"));
        }

        [Test]
        public void TestWenn()
        {
            Assert.AreEqual("(if (greater 1, 2): TRUE else: FALSE)",
                Parse("if 1>2 then TRUE else FALSE"));
        }

        [Test]
        public void TestIn()
        {
            Assert.AreEqual("(feld1 in [a, bb, ccc])",
                Parse("feld1 in [a, bb, ccc]"));
        }

        [Test]
        public void TestInWithLiteralList()
        {
            Assert.AreEqual("(feld1 in ['a', 'bb', 'ccc'])",
                Parse("feld1 in ['a', 'bb', 'ccc']"));
        }

        [Test]
        public void TestInWithLiteralSet()
        {
            Assert.AreEqual("(feld1 in <<'a', 'bb', 'ccc'>>)",
                Parse("feld1 in <<'a', 'bb', 'ccc'>>"));
        }

        [Test]
        public void TestInWithLiteralMap()
        {
            Assert.AreEqual("<<<'a' => 1, 'bb' => -1, 'ccc' => 100>>>",
                Parse("<<<'a' => 1, 'bb' => -1, 'ccc' => 100>>>"));
        }

        [Test]
        public void TestListAddInt()
        {
            Assert.AreEqual("(add [1, 2, 3], 4)",
                Parse("[1, 2, 3] + 4"));
        }

        [Test]
        public void TestRelop1()
        {
            Assert.AreEqual("(less a, b)",
                Parse("a < b"));
        }

        [Test]
        public void TestRelop2()
        {
            Assert.AreEqual("((less a, b) and (less b, c))",
                Parse("a < b < c"));
        }

        [Test]
        public void TestRelop3()
        {
            Assert.AreEqual("((less_equals a, b) and (less b, c) and (equals c, d))",
                Parse("a <= b < c == d"));
        }

        [Test]
        public void TestNonZeroFuncall()
        {
            Assert.AreEqual("(non_zero '12', '3')",
                Parse("non_zero('12', '3')"));
        }
 
        [Test]
        public void TestTooManyTokens()
        {
            Assert.Throws<SyntaxError>(() => Parse("1 + 1 1"));
        }

        [Test]
        public void TestNotEnoughTokens()
        {
            Assert.Throws<SyntaxError>(() => Parse("1 + "));
        }

        [Test]
        public void TestMissingThen()
        {
            Assert.Throws<SyntaxError>(() => Parse("if 1 < 2 else FALSE"));
        }

        [Test]
         public void TestIfThenOrExpr()
         {
             Assert.AreEqual("(if (equals a, 1): ((b in c) or (equals d, 9999)) else: TRUE)", 
                 Parse("if a == 1 then b in c or d == 9999"));
         }
 
         [Test]
        public void TestMissingClosingParens()
        {
            Assert.Throws<SyntaxError>(() => Parse("2 * (3 + 4( - 3"));
        }

        [Test]
        public void TestLambda()
        {
            Assert.AreEqual("(lambda a, b=3, (mul (string a), (b 2, 3)))", 
                Parse("fn(a, b=3) string(a) * b(2, 3)"));
        }

        [Test]
        public void TestWhile()
        {
            Assert.AreEqual("(while (greater x, 0) do (x = (sub x, 1)))", 
                Parse("while x > 0 do x = x - 1; end"));
        }

        [Test]
        public void TestSpreadIdentifier() {
            Assert.AreEqual("(f a, ...b, c)",
                Parse("f(a, ...b, c)"));
        }

        [Test]
        public void TestSpreadList() {
            Assert.AreEqual("(f a, ...[1, 2], c)",
                Parse("f(a, ...[1, 2], c)"));
        }

        [Test]
        public void TestDefDestructure()
        {
            Assert.AreEqual("(def [a, b] = [1, 2])", Parse("def [a, b] = [1, 2]"));
        }

        [Test]
        public void TestAssignDestructure()
        {
            Assert.AreEqual("([a, b] = [1, 2])", Parse("[a, b] = [1, 2]"));
        }
        
        private static string Parse(string s)
        {
            return Parser.Parse(new Lexer(new StringReader(s), "{test}")).ToString();
        }
    }
}