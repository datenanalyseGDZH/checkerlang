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
using System.Text;

namespace CheckerLang
{
    public class Interpreter
    {
        private Environment baseEnvironment;
        private Environment environment;
        
        public Interpreter(bool secure = true)
        {
            baseEnvironment = Environment.GetBaseEnvironment(secure);
            environment = baseEnvironment.NewEnv();
            if (!secure)
            {
                baseEnvironment.Put("stdout", new ValueOutput(new StringWriter()));
                baseEnvironment.Put("stdin", new ValueInput(new StringReader("")));
                baseEnvironment.Put("run", new FuncRun(this));
            }
        }

        public Environment GetBaseEnvironment()
        {
            return baseEnvironment;
        }

        public Environment GetEnvironment()
        {
            return environment;
        }

        public void MakeSecure()
        {
            baseEnvironment.Remove("stdout");
            baseEnvironment.Remove("stdin");
            baseEnvironment.Remove("run");
            baseEnvironment.GetParent().Remove("file_input");
            baseEnvironment.GetParent().Remove("file_output");
            baseEnvironment.GetParent().Remove("close");
        }

        public void SetStandardOutput(TextWriter stdout)
        {
            baseEnvironment.Put("stdout", new ValueOutput(stdout));
        }

        public void SetStandardInput(TextReader stdin)
        {
            baseEnvironment.Put("stdin", new ValueInput(stdin));
        }
        
        public void LoadFile(string filename, Encoding encoding = null)
        {
            Interpret(File.ReadAllText(filename, encoding ?? Encoding.UTF8), filename);
        }

        public Value Interpret(string script, string filename, Environment environment = null)
        {
            return Interpret(new StringReader(script), filename, environment);
        }

        public Value Interpret(TextReader input, string filename, Environment environment = null)
        {
            return Interpret(Parser.Parse(input, filename), environment);
        }

        public Value Interpret(Node expression, Environment environment = null)
        {
            Environment savedParent = null;
            Environment env;
            if (environment == null)
            {
                env = this.environment;
            }
            else
            {
                var environment_ = environment;
                while (environment_ != null && environment_.GetParent() != null)
                {
                    environment_ = environment_.GetParent();
                }
                savedParent = environment_?.GetParent();
                environment_?.WithParent(this.environment);
                env = environment;
            }

            try
            {
                var result = expression.Evaluate(env);
                
                if (result.IsReturn())
                {
                    return result.AsReturn().value;
                }
                else if (result.IsBreak())
                {
                    throw new ControlErrorException("Cannot use break without surrounding for loop", result.AsBreak().pos);
                }
                else if (result.IsContinue())
                {
                    throw new ControlErrorException("Cannot use continue without surrounding for loop", result.AsContinue().pos);
                }

                return result;
            }
            finally
            {
                if (savedParent != null)
                {
                    var environment_ = environment;
                    while (environment_ != null && environment_.GetParent() != null)
                    {
                        environment_ = environment_.GetParent();
                    }

                    environment_?.WithParent(savedParent);
                }
            }
        }
    }
}