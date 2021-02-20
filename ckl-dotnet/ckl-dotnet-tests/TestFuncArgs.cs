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
using CheckerLang;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestFuncArgs
    {
        [Test]
        public void TestOneArg()
        {
            Verify("12", "(fn(a) a)(12)");
        }

        [Test]
        public void TestTwoArgs()
        {
            Verify("[1, 2]", "(fn(a, b) [a, b])(1, 2)");
        }

        [Test]
        public void TestTwoArgsKeywords()
        {
            Verify("[1, 2]", "(fn(a, b) [a, b])(a = 1,  b=2)");
            Verify("[1, 2]", "(fn(a, b) [a, b])(b = 2,  a=1)");
            Verify("[1, 2]", "(fn(a, b) [a, b])(1,  b=2)");
            Verify("[1, 2]", "(fn(a, b) [a, b])(2,  a=1)");
        }
        
        [Test]
        public void TestRestArg()
        {
            Verify("[1, 2]", "(fn(a...) a...)(1, 2)");
        }

        [Test]
        public void TestMixed()
        {
            Verify("[1, 2, []]", "(fn(a, b, c...) [a, b, c...])(1, 2)");
            Verify("[1, 2, [3]]", "(fn(a, b, c...) [a, b, c...])(1, 2, 3)");
            Verify("[1, 2, [3, 4]]", "(fn(a, b, c...) [a, b, c...])(1, 2, 3, 4)");
        }

        [Test]
        public void TestMixedWithDefaults()
        {
            Verify("[1, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])()");
            Verify("[1, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])(1)");
            Verify("[1, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])(1, 2)");
            Verify("[11, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])(a=11)");
            Verify("[1, 12, []]", "(fn(a=1, b=2, c...) [a, b, c...])(b=12)");
            Verify("[1, 12, [3, 4]]", "(fn(a=1, b=2, c...) [a, b, c...])(1, 3, 4, b=12)");
        }

              
        private void Verify(string expected, string script)
        {
            var env = new Environment();
            var result = new Interpreter().Interpret(script, "{test}", env);
            Assert.AreEqual(expected, result.ToString());
        }
     }
}