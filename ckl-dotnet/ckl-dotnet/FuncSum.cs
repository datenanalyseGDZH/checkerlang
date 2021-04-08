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

namespace CheckerLang
{
    public class FuncSum : FuncBase
    {
        public FuncSum() : base("sum")
        {
            info = "sum(list, ignore = [])\r\n" +
                "\r\n" +
                "Returns the sum of a list of numbers. Values contained in the optional list ignore\r\n" +
                "are counted as 0.\r\n" +
                "\r\n" +
                ": sum([1, 2, 3]) ==> 6\r\n" +
                ": sum([1, 2.5, 3]) ==> 6.5\r\n" +
                ": sum([1, 2.5, 1.5, 3]) ==> 8.0\r\n" +
                ": sum([1.0, 2.0, 3.0]) ==> 6.0\r\n" +
                ": sum([1.0, 2, -3.0]) ==> 0.0\r\n" +
                ": sum([1, 2, -3]) ==> 0\r\n" +
                ": sum([1, '1', 1], ignore = ['1']) ==> 2\r\n" +
                ": sum(range(101)) ==> 5050\r\n" +
                ": sum([]) ==> 0\r\n" +
                ": sum([NULL], ignore = [NULL]) ==> 0\r\n" +
                ": sum([1, NULL, 3], ignore = [NULL]) ==> 4\r\n" +
                ": sum([1, NULL, '', 3], ignore = [NULL, '']) ==> 4\r\n";
        }

        public override List<string> GetArgNames()
        {
            return new List<string> {"list", "ignore"};
        }
        
        public override Value Execute(Args args, Environment environment, SourcePos pos)
        {
            if (args.IsNull("list")) return ValueNull.NULL;
            
            var list = args.GetList("list").GetValue();
            var ignore = new List<Value>();

            if (args.HasArg("ignore"))
            {
                ignore.AddRange(args.GetList("ignore").GetValue());
            }

            var resultdecimal = (decimal) 0;
            var resultint = 0L;
            var decimalrequired = false;
            
            foreach (var value in list)
            {
                var skipvalue = false;
                foreach (var ignoreval in ignore)
                {
                    if (ignoreval.IsEquals(value))
                    {
                        skipvalue = true;
                        break;
                    }
                }
                if (skipvalue) continue;

                if (decimalrequired)
                {
                    resultdecimal += value.AsDecimal().GetValue();
                } 
                else if (value.IsInt())
                {
                    resultint += value.AsInt().GetValue();
                }
                else if (value.IsDecimal())
                {
                    if (!decimalrequired)
                    {
                        decimalrequired = true;
                        resultdecimal = resultint;
                    }

                    resultdecimal += value.AsDecimal().GetValue();
                }
                else
                {
                    throw new ControlErrorException(new ValueString("ERROR"),"Cannot sum " + value, pos);
                }
            }

            if (decimalrequired)
            {
                return new ValueDecimal(resultdecimal);
            }
            return new ValueInt(resultint);
        }
    }
}
