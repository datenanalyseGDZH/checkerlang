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
using Environment = CheckerLang.Environment;

namespace Tests
{
    [TestFixture]
    public class TestInterpreter
    {
        [Test]
        public void TestString()
        {
            Verify("'abc'", "'abc'");
        }

        [Test]
        public void TestInt()
        {
            Verify("123", "123");
        }

        [Test]
        public void TestIntList()
        {
            Verify("[1, 2, 3]", "[1, 2, 3]");
        }

        [Test]
        public void TestStringList()
        {
            Verify("['a', 'b', 'c']", "['a', 'b', 'c']");
        }

        [Test]
        public void TestBlock()
        {
            Verify("12", "do 2 * 3; 3 * 4; end");
        }

        [Test]
        public void TestIfThen()
        {
            Verify("'b'", "if 13 < 12 then 'a' if 11 < 12 then 'b'");
            Verify("TRUE", "if 13 < 12 then 'a' if 14 < 12 then 'b'");
        }

        [Test]
        public void TestIfThenElse()
        {
            Verify("'c'", "if 13 < 12 then 'a' if 14 < 12 then 'b' else 'c'");
        }

        [Test]
        public void TestIfThenWithBlock()
        {
            Verify("'b'", "if 13 < 12 then do 'a' end if 11 < 12 then do 2 * 2; 'b'; end");
        }

        [Test]
        public void TestForLoop()
        {
            Verify("5", "for i in range(10) do if i == 5 then return i end");
        }

        [Test]
        public void TestDef()
        {
            Verify("14", "def a=12;a+2");
        }

        [Test]
        public void TestAssign()
        {
            Verify("27", "def a=12; def b = 3; a = b + 2 * a; a");
        }

        [Test]
        public void TestOrExpr()
        {
            Verify("TRUE", "2 == 3 or 3 == 4 or 4 == 4");
            Verify("FALSE", "2 == 3 or 3 == 4 or 4 == 5");
        }

        [Test]
        public void TestAndExpr()
        {
            Verify("TRUE", "2 == 2 and 3 == 3 and 4 == 4");
            Verify("FALSE", "2 == 2 and 3 == 3 and 4 == 5");
        }

        [Test]
        public void TestNotExpr()
        {
            Verify("TRUE", "not 2 == 3");
            Verify("FALSE", "not 2 == 2");
        }

        [Test]
        public void TestBooleanAlgebra()
        {
            Verify("TRUE", "2 == 3 or 3 == 3 and not 4 == 5");
        }

        [Test]
        public void TestComparison()
        {
            Verify("TRUE", "2 == 2");
            Verify("TRUE", "2 == 2 != 3");
            Verify("TRUE", "2 == 2 <> 3");
            Verify("TRUE", "2 < 3");
            Verify("TRUE", "2 < 3 <= 3 < 4");
            Verify("FALSE", "2 < 3 < 3 <= 4");
            Verify("TRUE", "5 >= 5 > 4 >= 2 == 2");
            Verify("FALSE", "5 >= 5 > 5 >= 2 == 2");
        }

        [Test]
        public void TestArithmeticAdd()
        {
            Verify("5", "2 + 3");
            Verify("9", "2 + 3 + 4");
        }

        [Test]
        public void TestArithmeticSub()
        {
            Verify("-1", "2 - 3");
            Verify("-5", "2 - 3 - 4");
        }

        [Test]
        public void TestArithmeticMul()
        {
            Verify("6", "2 * 3");
            Verify("24", "2 * 3 * 4");
        }

        [Test]
        public void TestArithmeticDiv()
        {
            Verify("3", "6 / 2");
            Verify("0.5", "10 / 20.0");
        }

        [Test]
        public void TestArithmeticMod()
        {
            Verify("0", "6 % 2");
            Verify("1", "7 % 2");
        }

        [Test]
        public void TestArithmetic()
        {
            Verify("7", "(2 + 3) * (3 + 4) / 5");
            Verify("11", "2 + 3 * 3 + 4 / 5");
            Verify("11.8", "2 + 3 * 3 + 4 / 5.0");
        }

        [Test]
        public void TestUnary()
        {
            Verify("-2", "-2");
            Verify("0", "2 + -2");
        }

        [Test]
        public void TestIsEmpty()
        {
            Verify("TRUE", "feld1 is empty", "feld1", "");
            Verify("FALSE", "feld1 is empty", "feld1", "'a'");
        }

        [Test]
        public void TestIsNotEmpty()
        {
            Verify("FALSE", "feld1 is not empty", "feld1", "");
            Verify("TRUE", "feld1 is not empty", "feld1", "a");
        }

        [Test]
        public void TestIsDate()
        {
            Verify("TRUE", "feld1 is date", "feld1", "20180101");
            Verify("FALSE", "feld1 is date", "feld1", "20180230");
        }

        [Test]
        public void TestIsTime()
        {
            Verify("TRUE", "feld1 is time", "feld1", "1245");
            Verify("FALSE", "feld1 is time", "feld1", "2512");
            Verify("FALSE", "feld1 is time", "feld1", "123456");
        }

        [Test]
        public void TestIsDateWithHour()
        {
            Verify("TRUE", "feld1 is date with hour", "feld1", "2018010123");
            Verify("FALSE", "feld1 is date with hour", "feld1", "2018010126");
        }

        [Test]
        public void TestIsNumerical()
        {
            Verify("TRUE", "feld1 is numerical exact_len 5", "feld1", "12345");
            Verify("FALSE", "feld1 is numerical exact_len 5", "feld1", "1234");
            Verify("FALSE", "feld1 is numerical exact_len 5", "feld1", "123456");
            Verify("FALSE", "feld1 is numerical exact_len 5", "feld1", "1234a");
        }

        [Test]
        public void TestIsAlphanumerical()
        {
            Verify("TRUE", "feld1 is alphanumerical", "feld1", "abcde");
            Verify("FALSE", "feld1 is alphanumerical", "feld1", "ab /x");
        }

        [Test]
        public void TestStartsWith()
        {
            Verify("TRUE", "feld1 starts with 'ab'", "feld1", "ab123");
            Verify("FALSE", "feld1 starts with 'cd'", "feld1", "ab12");
        }

        [Test]
        public void TestEndsWith()
        {
            Verify("TRUE", "feld1 ends with '23'", "feld1", "ab123");
            Verify("FALSE", "feld1 ends with '23'", "feld1", "ab12");
        }

        [Test]
        public void TestContains()
        {
            Verify("TRUE", "feld1 contains '12'", "feld1", "ab123");
            Verify("FALSE", "feld1 contains 'x1'", "feld1", "ab12");
        }

        [Test]
        public void TestMatches()
        {
            Verify("TRUE", "feld1 matches //ab[1234567890]{3}//", "feld1", "ab123");
            Verify("FALSE", "feld1 matches //ab[1234567890]{3}//", "feld1", "ab12");
        }

        [Test]
        public void TestInList()
        {
            Verify("TRUE", "feld1 is in ['M230', 'M231', 'M232']", "feld1", "M231");
            Verify("FALSE", "feld1 in ['M230', 'M231', 'M232']", "feld1", "M233");
            Verify("FALSE", "feld1 not in ['M230', 'M231', 'M232']", "feld1", "M231");
            Verify("TRUE", "feld1 in [1, 2, 3]", "feld1", 2);
            Verify("FALSE", "feld1 is in [1, 2, 3]", "feld1", "2");
            Verify("FALSE", "feld1 in [1, 2, 3]", "feld1", 4);
        }

        [Test]
        public void TestDeref()
        {
            Verify("'b'", "feld1[1]", "feld1", "ab123");
            Verify("3", "[1, 2, 3][2]");
            Verify("'b'", "[['a', 1], ['b', 2]][1][0]");
            Verify("'c'", "'abcd'[2]");
        }

        [Test]
        public void TestFuncDef()
        {
            Verify("6", "def dup = fn(n) 2 * n; dup(3)");
        }

        [Test]
        public void TestFuncDefWithBlock()
        {
            Verify("7", "def myfn = fn(n) do def m = 2 * n; if m % 2 == 0 then m + 1 else m end; myfn(3)");
        }

        [Test]
        public void TestLambda()
        {
            Verify("'555555'", "(fn(a, b = 3) string(a) * b)(55)");
        }

        [Test]
        public void TestFuncRecursive()
        {
            Verify("3628800", "def a = fn(x) do def y = x - 1; if x == 0 then 1 else x * a(y) end; a(10)");
        }

        [Test]
        public void TestFuncDefaultArg()
        {
            Verify("10", "def a = fn(x = 12) x; a(10)");
            Verify("12", "def a = fn(x = 12) x; a()");
        }

        [Test]
        public void TestFuncLength()
        {
            Verify("3", "length('abc')");
            Verify("3", "length([1, 2, 3])");
        }

        [Test]
        public void TestFuncLower()
        {
            Verify("'abc'", "lower('Abc')");
        }

        [Test]
        public void TestFuncUpper()
        {
            Verify("'ABC'", "upper('Abc')");
        }

        [Test]
        public void TestFuncNonZero()
        {
            Verify("'12'", "non_zero('12', '3')");
            Verify("'3'", "non_zero('0', '3')");
        }

        [Test]
        public void TestFuncNonEmpty()
        {
            Verify("'12'", "non_empty('12', '3')");
            Verify("'3'", "non_empty('', '3')");
        }

        [Test]
        public void TestFuncInt()
        {
            Verify("12", "int('12')");
        }

        [Test]
        public void TestFuncDecimal()
        {
            Verify("12.3", "decimal('12.3')");
        }

        [Test]
        public void TestFuncBoolean()
        {
            Verify("TRUE", "boolean('1')");
            Verify("FALSE", "boolean('0')");
        }

        [Test]
        public void TestFuncString()
        {
            Verify("'123'", "string(123)");
        }

        [Test]
        public void TestFuncPattern()
        {
            Verify("//^abc[0-9]+$//", "pattern('^abc[0-9]+$')");
        }

        [Test]
        public void TestFuncSplit()
        {
            Verify("['a', 'b', 'ccc', 'd', 'e']", "split('a,b,ccc,d,e', ',')");
            Verify("['a', 'b', 'ccc', 'd', 'e']", "split('a, b;ccc,d ,e', ' ?[,;] ?')");
        }

        [Test]
        public void TestList()
        {
            Verify("[1, 2, 3]", "[1, 2, 3]");
            Verify("[1, 2, 3]", "[1, 2, 3,]");
            Verify("[1]", "[1]");
            Verify("[]", "[]");
        }

        [Test]
        public void testListWithIf()
        {
            Verify("['a', 'x', 'd']", "[if 1 < 2 then 'a' else 'b', 'x', if 1 > 2 then 'c' else 'd']");
        }

        [Test]
        public void testListWithFn()
        {
            Verify("[1, <#lambda>, 2]", "[1, fn(x) 2*x, 2]");
        }

        [Test]
        public void TestListComprehensionSimple()
        {
            Verify("[0, 2, 4, 6, 8]", "[x * 2 for x in range(5)]");
        }

        [Test]
        public void TestListComprehensionWithCondition()
        {
            Verify("[2, 6]", "[x * 2 for x in range(5) if x % 2 == 1]");
        }

        [Test]
        public void TestSetComprehensionSimple() {
            Verify("<<0, 2, 4, 6, 8>>", "<<x * 2 for x in range(5)>>");
        }

        [Test]
        public void TestSetComprehensionWithCondition() {
            Verify("<<2, 6>>", "<<x * 2 for x in range(5) if x % 2 == 1>>");
        }

        [Test]
        public void TestListComprehensionString() {
            Verify("[1, 2, 3]", "[int(ch) for ch in '123']");
        }

        [Test]
        public void TestSetComprehensionString() {
            Verify("<<1, 2, 3>>", "<<int(ch) for ch in '12312'>>");
        }

        [Test]
        public void TestMapComprehensionSimple() {
            Verify("<<<0 => 0, 1 => 2, 2 => 4, 3 => 6, 4 => 8>>>", "<<<a => 2 * a for a in range(5)>>>");
        }

        [Test]
        public void TestMapComprehensionSimple2() {
            Verify("<<<'x0' => 0, 'x1' => 2, 'x2' => 4, 'x3' => 6, 'x4' => 8>>>", "<<<'x' + a => 2 * a for a in range(5)>>>");
        }

        [Test]
        public void TestMapComprehensionWithCondition() {
            Verify("<<<0 => 0, 1 => 2, 2 => 4>>>", "<<<a => 2 * a for a in range(5) if 2 * a < 6>>>");
        }

        [Test]
        public void TestFuncRange()
        {
            Verify("[]", "range()");
            Verify("[0, 1, 2, 3, 4, 5, 6, 7, 8, 9]", "range(10)");
            Verify("[5, 6, 7, 8, 9]", "range(5, 10)");
            Verify("[10, 9, 8, 7, 6]", "range(10, 5, -1)");
            Verify("[]", "range(5, 10, -1)");
        }

        [Test]
        public void TestFuncSubstr()
        {
            Verify("'def'", "substr('abcdef', 3)");
            Verify("'d'", "substr('abcdef', 3, 4)");
            Verify("'f'", "substr('abcdef', 5)");
            Verify("''", "substr('abcdef', 6)");
        }

        [Test]
        public void TestFuncSublist()
        {
            Verify("[3, 4]", "sublist([1, 2, 3, 4], 2)");
            Verify("[3]", "sublist([1, 2, 3, 4], 2, 3)");
            Verify("[4]", "sublist([1, 2, 3, 4], 3)");
            Verify("[]", "sublist([1, 2, 3, 4], 4)");
        }

        [Test]
        public void TestFuncFindStr()
        {
            Verify("1", "find('abcd', 'b')");
            Verify("-1", "find('abcd', 'e')");
        }

        [Test]
        public void TestFuncStrFind()
        {
            Verify("1", "str_find('abcd', 'bc')");
            Verify("-1", "str_find('abcd', 'de')");
        }

        [Test]
        public void TestFuncFindList()
        {
            Verify("1", "find([1, 2, 3], 2)");
            Verify("-1", "find([1, 2, 3], 4)");
        }

        [Test]
        public void TestFuncFindListWithKey()
        {
            Verify("1", "find([[1, 'a'], [2, 'b'], [3, 'c']], 2, fn(x) x[0])");
            Verify("-1", "find([[1, 'a'], [2, 'b'], [3, 'c']], 4, fn(x) x[0])");
        }

        [Test]
        public void TestFuncSet()
        {
            Verify("<<1, 2, 3, 4, 5>>", "set([1, 2, 3, 3, 4, 5])");
        }

        [Test]
        public void TestFuncMap()
        {
            Verify("<<<1 => 'a', 2 => 'b', 3 => 'd', 4 => 'e', 5 => 'f'>>>",
                "map([[1, 'a'], [2, 'b'], [3, 'c'], [3, 'd'], [4, 'e'], [5, 'f']])");
        }

        [Test]
        public void TestFuncSubstitute()
        {
            Verify("'abcxef'", "substitute('abcdef', 3, 'x')");
            Verify("[1, 2, 'x', 4]", "substitute([1, 2, 3, 4], 2, 'x')");
        }

        [Test]
        public void TestFuncRandom()
        {
            Verify("2", "set_seed(1); random(10)");
        }

        [Test]
        public void TestEnvByteGet()
        {
            const byte b = 12;
            var env = Environment.GetBaseEnvironment();
            env.Put("test", b);
            Assert.AreEqual(12, env.Get("test", null).AsInt().GetValue());
        }

        [Test]
        public void TestEnvSbyteGet()
        {
            const sbyte b = 12;
            var env = Environment.GetBaseEnvironment();
            env.Put("test", b);
            Assert.AreEqual(12, env.Get("test", null).AsInt().GetValue());
        }

        [Test]
        public void TestFuncSqrt()
        {
            Verify("2", "sqrt(4)");
        }

        [Test]
        public void TestBlockFuncOrdering()
        {
            Verify("30", "def a = fn(y) do def b = fn(x) 2 * c(x); def c = fn(x) 3 + x; b(y); end; a(12)");
        }

        [Test]
        public void TestBlockFuncOrderingGlobal()
        {
            Verify("30", "def b = fn(x) 2 * c(x); def c = fn(x) 3 + x; b(12)");
        }

        [Test]
        public void TestFuncType()
        {
            Verify("'int'", "type(4)");
            Verify("'decimal'", "type(4.0)");
            Verify("'string'", "type('a')");
            Verify("'pattern'", "type(//a//)");
            Verify("'list'", "type([1])");
            Verify("'set'", "type(set([1]))");
            Verify("'map'", "type(map([[1, 2]]))");
            Verify("'func'", "type(fn(x) x)");
            Verify("'boolean'", "type(TRUE)");
            Verify("'date'", "type(date('20120101'))");
        }

        [Test]
        public void TestEmptyListLiteral()
        {
            Verify("[2, 3]", "def f(x, y) do def r = []; append(r, x); append(r, y); return r; end; f(1, 2); f(2, 3);");
        }

        [Test]
        public void TestNonEmptyListLiteral()
        {
            Verify("[1, 2, 3]",
                "def f(x, y) do def r = [1]; append(r, x); append(r, y); return r; end; f(1, 2); f(2, 3);");
        }

        [Test]
        public void TestParseSimpleReturn()
        {
            Verify("(add x, 1)", "parse('return x + 1')");
        }

        [Test]
        public void TestParseSingleBlockReturn()
        {
            Verify("(add x, 1)", "parse('do return x + 1; end')");
        }

        [Test]
        public void TestParseBlockReturn()
        {
            Verify("(block (def x = 1), (add x, 1))", "parse('do def x = 1; return x + 1; end')");
        }

        [Test]
        public void TestParseBlockEarlyReturn()
        {
            Verify("(block (return (add x, 1)), (def x = 1))", "parse('do return x + 1; def x = 1; end')");
        }

        [Test]
        public void TestParseBareBlockReturn()
        {
            Verify("(block (def x = 1), (add x, 1))", "parse('def x = 1; return x + 1;')");
        }

        [Test]
        public void TestParseBareBlockEarlyReturn()
        {
            Verify("(block (return (add x, 1)), (def x = 1))", "parse('return x + 1; def x = 1')");
        }

        [Test]
        public void TestParseLambdaReturn()
        {
            Verify("(def fun = (lambda x, (add x, 1)))", "parse('def fun(x) return x + 1')");
        }

        [Test]
        public void TestParseLambdaBlockReturn()
        {
            Verify("(def fun = (lambda x, (block (x = (mul x, 2)), (add x, 1))))",
                "parse('def fun(x) do x = x * 2; return x + 1; end')");
        }

        [Test]
        public void TestParseLambdaBlockEarlyReturn()
        {
            Verify("(def fun = (lambda x, (block (return (add x, 1)), (x = (mul x, 2)))))",
                "parse('def fun(x) do return x + 1; x = x * 2; end')");
        }

        [Test]
        public void testSpreadInListLiteral()
        {
            Verify("[1, 2, 3, 4, 5]", "[1, ...[2, 3, 4], 5]");
        }

        [Test]
        public void testSpreadInListLiteralWithIdentifier()
        {
            Verify("[1, 2, 3, 4, 5]", "def a = [2, 3, 4]; [1, ...a, 5]");
        }

        [Test]
        public void TestSpreadInFuncallBasic()
        {
            Verify("[1, 2, 3]", "def f(a, b, c) [a, b, c]; f(1, ...[2, 3])");
        }

        [Test]
        public void testSpreadInFuncallMap()
        {
            Verify("[1, 2, 3]", "def f(a, b, c) [a, b, c]; f(...<<<'c' => 3, 'a' => 1, 'b' => 2>>>)");
        }

        [Test]
        public void testSpreadInFuncall()
        {
            Verify("[1, 2, 3, 4, 5]", "def f(args...) args...; f(1, ...[2, 3, 4], 5)");
        }

        [Test]
        public void testSpreadInFuncallWithIdentifier()
        {
            Verify("[1, 2, 3, 4, 5]", "def f(args...) args...; def a = [2, 3, 4]; f(1, ...a, 5)");
        }

        [Test]
        public void TestDefDestructure1()
        {
            Verify("[1, 2]", "def [a, b] = [1, 2]; [a, b]");
        }

        [Test]
        public void TestDefDestructure2() 
        {
            Verify("1", "def [a] = [1, 2]; a");
            
        }
        
        [Test]
        public void TestDefDestructure3() 
        {
            Verify("3", "def [a, b, c] = <<1, 2, 3>>; c");
        }
        
        [Test]
        public void TestDefDestructure4() 
        {
            Verify("NULL", "def [a, b, c] = [1, 2]");
        }
        
        [Test]
        public void TestAssignDestructure1() 
        {
            Verify("[1, 2]", "def a = 1; def b = 1; [a, b] = [1, 2]; [a, b]");
        }
        
        [Test]
        public void TestAssignDestructure2() 
        {
            Verify("2", "def a = 1; [a] = [2, 3]; a");
        }
        
        [Test]
        public void TestSwapUsingDestructure() 
        {
            Verify("[2, 1]", "def a = 1; def b = 2; [a, b] = [b, a]; [a, b]");
        }
 
        [Test]
        public void TestAll() {
            Verify("TRUE", "all([2, 4, 6], fn(x) x % 2 == 0)");
        }

        [Test]
        public void TestMethods1() {
            Verify("TRUE", "'abcdef'!>starts_with('abc')");
        }

        [Test]
        public void TestMethods2() {
            Verify("'xy'", "' xy '!>trim()");
        }

        [Test]
        public void TestMethods3() {
            Verify("[3, 2, 1]", "[1, 2, 3]!>reverse()");
        }

        [Test]
        public void TestMethods4() {
            Verify("TRUE", "[2, 4, 6] !> all(fn(x) x % 2 == 0)");
        }

        [Test]
        public void TestMethods5() {
            Verify("12", "12 !> max(2)");
        }

        [Test]
        public void TestMethods6() {
            Verify("'3-2-1'", "[1, 2, 3] !> reverse() !> join(sep = '-')");
        }
        
        [Test]
        public void TestDerefProperty() 
        {
            Verify("2", "def a = <<<'x' => 1, 'y' => 2>>>; a->y");
        }

        [Test]
        public void TestMapLiteralImplicitString() 
        {
            Verify("<<<'x' => 1, 'y' => 2>>>", "<<<x => 1, y => 2>>>");
        }

        [Test]
        public void testPipelineLambda()
        {
            Verify("3", "[1, 2, 3] !> (fn(lst) lst[2])()");
        }

        [Test]
        public void TestIfThenElifThenElse()
        {
            Verify("5", "if 1 == 2 then 3 elif 1 == 3 then 4 elif 1 == 1 then 5 else 6");
        }

        [Test]
        public void TestForDestructuringListList() {
            Verify("21", "def a = 0; for [x, y, z] in [[1, 2, 3], [4, 5, 6]] do a += x + y + z; end; a;");
        }

        [Test]
        public void TestForDestructuringListSet() {
            Verify("21", "def a = 0; for [x, y, z] in [<<1, 2, 3>>, <<4, 5, 6>>] do a += x + y + z; end; a;");
        }

        [Test]
        public void TestForDestructuringSetList() {
            Verify("21", "def a = 0; for [x, y, z] in <<[1, 2, 3], [4, 5, 6]>> do a += x + y + z; end; a;");
        }

        [Test]
        public void TestForDestructuringSetSet() {
            Verify("21", "def a = 0; for [x, y, z] in << <<1, 2, 3>>, <<4, 5, 6>> >> do a += x + y + z; end; a;");
        }
        
        private void Verify(string expected, string script)
        {
            var env = new Environment();
            var result = new Interpreter().Interpret(script, "{test}", env);
            Assert.AreEqual(expected, result.ToString());
        }
        
        private void Verify(string expected, string script, string variable, object value)
        {
            var env = new Environment();
            env.Put(variable, value);
            var result = new Interpreter().Interpret(script, "{test}", env);
            Assert.AreEqual(expected, result.ToString());
        }
    }
}