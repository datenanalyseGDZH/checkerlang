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
using CheckerLang;
using NUnit.Framework;

namespace Tests
{
    public class TestCaseInterpreter
    {
        private string script;
        private TestCaseInstance current;
        private List<TestCaseInstance> tests = new List<TestCaseInstance>();
        
        public static TestCaseInterpreter Test(string script)
        {
            return new TestCaseInterpreter {script = script, current = TestCaseInstance.Create(script)};
        }
        
        public TestCaseInterpreter With(string varname, object value)
        {
            current.With(varname, value);
            return this;
        }

        public TestCaseInterpreter WithList(string varname, params string[] values)
        {
            var list = new ValueList();
            foreach (var value in values)
            {
                list.AddItem(new ValueString(value));
            }

            With(varname, list);
            return this;
        }

        public TestCaseInterpreter WithList(string varname, params int[] values)
        {
            var list = new ValueList();
            foreach (var value in values)
            {
                list.AddItem(new ValueInt(value));
            }

            With(varname, list);
            return this;
        }

        public TestCaseInterpreter ExpectTrue()
        {
            current.ExpectTrue();
            tests.Add(current);
            current = TestCaseInstance.Create(script);
            return this;
        }
        
        public TestCaseInterpreter ExpectFalse()
        {
            current.ExpectFalse();
            tests.Add(current);
            current = TestCaseInstance.Create(script);
            return this;
        }

        public void Execute()
        {
            foreach (var test in tests)
            {
                var interpreter = new Interpreter();
                foreach (var varname in test.vars.Keys)
                {
                    interpreter.GetEnvironment().Put(varname, test.vars[varname]);
                }

                var result = interpreter.Interpret(test.script, "test");
                Assert.AreEqual(test.expectedResult, result, test.ToString());
            }
        }
    }
}
