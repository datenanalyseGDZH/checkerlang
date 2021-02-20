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
using System.Collections.Generic;
using System.IO;
using System.Text;
using CheckerLang;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TestCollectVars
    {
        [Test]
        public void TestVarsSimple()
        {
            Verify("{abc}", "abc is zero");
        }

        [Test]
        public void TestVarsMulti()
        {
            Verify("{abc, bcd}", "abc is zero and bcd is not zero");
        }

        [Test]
        public void TestVarsDef()
        {
            Verify("{bcd}", "def abc = 12; abc > 0 and bcd is not zero");
        }

        [Test]
        public void TestVarsDefWithVarref()
        {
            Verify("{bcd}", "def abc = bcd * 2; abc > 0 and bcd < 12");
        }

        [Test]
        public void TestFuncDefAndUse()
        {
            Verify("{abc, bcd}", "def dup = fn(x) 2 * x; abc > 0 and dup(bcd) < 12");
        }

        [Test]
        public void TestLambdaCall()
        {
            Verify("{abc}", "(fn(x) 2 * x)(abc)");
        }

        [Test]
        public void TestLambda()
        {
            Verify("{}","fn(x) 2 * x");
        }

        [Test]
        public void TestListComprehension()
        {
            Verify("{y}","[2*x for x in y]");
        }

        [Test]
        public void TestListComprehensionWithCondition()
        {
            Verify("{y}","[2*x for x in y if x < 12]");
        }

        [Test]
        public void TestCascadedLambdas()
        {
            Verify("{abc}", "def a = fn(y) fn(x) y * x; a(abc)(2)");
        }

        [Test]
        public void TestLambdaOrdering()
        {
            Verify("{d}", "def a = fn(y) do def b = fn(x) 2 * c(x); def c = fn(x) d * x; end");
        }

        [Test]
        public void TestLambdaOrderingWithFreeCall()
        {
            Verify("{b}", "def a = fn(y) do b(y); def b = fn(x) 2 * x; end;");
        }

        [Test]
        public void TestLambdaOrderingGlobal()
        {
            Verify("{d}", "def b = fn(x) 2 * c(x); def c = fn(x) d * x");
        }

        [Test]
        public void TestPredefinedFunctions()
        {
            var node = Parser.Parse(new Lexer(new StringReader("lower(a) < 'a'"), "{test}"));
            var result = SetToString(FreeVars.Get(node, new Interpreter().GetBaseEnvironment()));
            Assert.AreEqual("{a}", result);
        }
        
        private void Verify(string free, string script)
        {
            var node = Parser.Parse(new Lexer(new StringReader(script), "{test}"));
            var freeVars = new SortedSet<string>();
            var boundVars = new SortedSet<string>();
            var additionalBoundVars = new SortedSet<string>();
            node.CollectVars(freeVars, boundVars, additionalBoundVars);
            Assert.AreEqual(free, SetToString(FreeVars.Get(node, Environment.GetBaseEnvironment())));
        }

        private string SetToString(IEnumerable<string> set)
        {
            var result = new StringBuilder();
            result.Append("{");
            foreach (var s in set)
            {
                result.Append(s).Append(", ");
            }
            if (result.Length > 1) result.Remove(result.Length - 2, 2);
            result.Append("}");
            return result.ToString();
        }

        [Test]
        public void TestFreeVars()
        {
            var interpreter = new Interpreter();
            var env = Environment.GetBaseEnvironment();
            var parseTree1 = Parser.Parse(new Lexer(new StringReader("def say_hello = fn(obj) \"Hello \" + obj;"), "{test}"));
            interpreter.Interpret(parseTree1, env);
            
            var parseTree2 = Parser.Parse(new Lexer(new StringReader("say_hello(\"du\") + unknown_func()"), "{test}"));
            var freeVars = FreeVars.Get(parseTree2, env);
            Assert.IsTrue(freeVars.Contains("unknown_func"));
            Assert.IsFalse(freeVars.Contains("say_hello"));
        }
    }
}