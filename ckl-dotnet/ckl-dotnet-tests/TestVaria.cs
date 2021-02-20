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
    public class TestVaria
    {
       [Test]
        public void TestExternalEnv()
        {
            var interpreter = new Interpreter();
            var env = Environment.GetBaseEnvironment();
            interpreter.Interpret("def say_hello = fn(obj) \"Hello \" + obj;", "{test}", env);
            Assert.AreEqual("Hello Du", interpreter.Interpret("say_hello(\"Du\")", "{test}", env).AsString().GetValue());
        }
        
        [Test]
        public void TestExternalEnv2()
        {
            var interpreter = new Interpreter();
            var env = interpreter.GetBaseEnvironment();
            interpreter.Interpret("def say_hello = fn(obj) \"Hello \" + obj;", "{test}", env);
            Assert.AreEqual("Hello Du", interpreter.Interpret("say_hello(\"Du\")", "{test}").AsString().GetValue());
        }
        
        [Test]
        public void TestExternalEnvChained()
        {
            var interpreter = new Interpreter();
            var env = Environment.GetNullEnvironment();
            interpreter.Interpret("def say_hello = fn(obj) \"Hello \" + obj;", "{test}", env);
            var env2 = env.NewEnv();
            Assert.AreEqual("Hello Du", interpreter.Interpret("say_hello(\"Du\")", "{test}", env2).AsString().GetValue());
        }
    }
}