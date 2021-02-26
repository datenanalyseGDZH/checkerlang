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
    public class TestLexer
    {
        [Test]
        public void TestSimple()
        {
            Assert.AreEqual("[1_6_V04, nicht, leer] @ 0", new Lexer(new StringReader("1_6_V04 nicht leer"), "{test}").ToString());
        }

        [Test]
        public void TestQuotes()
        {
            Assert.AreEqual("[a, double, b, single, c] @ 0",
                new Lexer(new StringReader("a \"double\" b 'single' c"), "{test}").ToString());
        }

        [Test]
        public void TestPattern()
        {
            Assert.AreEqual("[//abc//] @ 0",
                new Lexer(new StringReader("//abc//"), "{test}").ToString());
        }

        [Test]
        public void TestIn()
        {
            Assert.AreEqual("[feld1, in, [, a, ,, bb, ,, ccc, ]] @ 0",
                new Lexer(new StringReader("feld1 in ['a', 'bb', 'ccc']"), "{test}").ToString());
        }

        [Test]
        public void TestComplex()
        {
            Assert.AreEqual(
                "[(, felda, beginnt, mit, D12, ), oder, (, feldb, <=, 1.3, *, (, feldc, -, feldd, /, 2, ), )] @ 0",
                new Lexer(new StringReader("(felda beginnt mit 'D12') oder (feldb <= 1.3*(feldc-feldd/2))"), "{test}")
                    .ToString());
        }

        [Test]
        public void TestCompare()
        {
            Assert.AreEqual("[1, >, 2, d] @ 0",
                new Lexer(new StringReader("1>2 d"), "{test}").ToString());
        }

        [Test]
        public void TestNonZero()
        {
            Assert.AreEqual("[non_zero, (, 12, ,, 3, )] @ 0",
                new Lexer(new StringReader("non_zero('12', '3')"), "{test}").ToString());
        }
 
        [Test]
        public void TestList()
        {
            Assert.AreEqual("[a, in, [, 1, ,, 2, ,, 3, ]] @ 0",
                new Lexer(new StringReader("a in [1, 2, 3]"), "{test}").ToString());
        }
        
        [Test]
        public void TestSetLiteral()
        {
            Assert.AreEqual("[a, in, <<, 1, ,, 2, ,, 3, >>] @ 0",
                new Lexer(new StringReader("a in <<1, 2, '3'>>"), "{test}").ToString());
        }
        
        [Test]
        public void TestMapLiteral()
        {
            Assert.AreEqual("[def, m, =, <<<, 1, =>, 100, ,, 2, =>, 200, >>>] @ 0",
                new Lexer(new StringReader("def m = <<<1 => 100, 2 => 200>>>"), "{test}").ToString());
        }
        
        [Test]
        public void TestListAddInt()
        {
            Assert.AreEqual("[[, 1, ,, 2, ,, 3, ], +, 4] @ 0",
                new Lexer(new StringReader("[1, 2, 3] + 4"), "{test}").ToString());
        }
 
        [Test]
        public void TestSpreadOperatorIdentifier() {
                Assert.AreEqual("[..., a] @ 0",
                new Lexer(new StringReader("...a"), "test").ToString());
        }

        [Test]
        public void TestSpreadOperatorList() {
            Assert.AreEqual("[..., [, 1, ,, 2, ]] @ 0",
            new Lexer(new StringReader("...[1, 2]"), "test").ToString());
        }

        [Test]
        public void TestSpreadOperatorFuncall() {
            Assert.AreEqual("[f, (, a, ,, ..., b, ,, c, )] @ 0",
            new Lexer(new StringReader("f(a, ...b, c)"), "test").ToString());
        }
        
        [Test]
        public void TestInvokeOperator() {
            Assert.AreEqual("[a, !>, b] @ 0",
            new Lexer(new StringReader("a!>b"), "test").ToString());
        }

        [Test]
        public void TestCrLf() {
            Assert.AreEqual("[if, UploadID_KTR, is, empty, then, return, TRUE, ;] @ 0",
                new Lexer(new StringReader("#comment\r\nif UploadID_KTR is empty\r\nthen return TRUE;"), "test").ToString());
        }
        
        [Test]
        public void TestLf() {
            Assert.AreEqual("[if, UploadID_KTR, is, empty, then, return, TRUE, ;] @ 0",
                new Lexer(new StringReader("#comment\nif UploadID_KTR is empty\nthen return TRUE;"), "test").ToString());
        }

        [Test]
        public void TestSingleLine() {
            Assert.AreEqual("[if, UploadID_KTR, is, empty, then, return, TRUE, ;] @ 0",
                new Lexer(new StringReader("#comment\nif UploadID_KTR is empty then return TRUE;"), "test").ToString());
        }

        [Test]
        public void TestStringLiteralWithNewline()
        {
            Assert.AreEqual("[one\\ntwo] @ 0", new Lexer(new StringReader("'one\\ntwo'"), "test").ToString());
        }

        [Test]
        public void TestDerefProperty()
        {
            Assert.AreEqual("[a, ->, b, ->, c, ->, d] @ 0", new Lexer(new StringReader("a->b ->c -> d"), "test").ToString());
        }

        [Test]
        public void TestSourcePos()
        {
            var lexer = new Lexer(new StringReader("1 / 2\r\nx == 3"), "{test}");
            var token = lexer.Next();
            Assert.AreEqual("1", token.value);
            Assert.AreEqual("{test}:1:1", token.pos.ToString());
            token = lexer.Next();
            Assert.AreEqual("/", token.value);
            Assert.AreEqual("{test}:1:3", token.pos.ToString());
            token = lexer.Next();
            Assert.AreEqual("2", token.value);
            Assert.AreEqual("{test}:1:5", token.pos.ToString());
            token = lexer.Next();
            Assert.AreEqual("x", token.value);
            Assert.AreEqual("{test}:2:1", token.pos.ToString());
            token = lexer.Next();
            Assert.AreEqual("==", token.value);
            Assert.AreEqual("{test}:2:3", token.pos.ToString());
            token = lexer.Next();
            Assert.AreEqual("3", token.value);
            Assert.AreEqual("{test}:2:5", token.pos.ToString());
        }
    }
}