package ch.checkerlang;

import ch.checkerlang.values.Value;
import org.junit.Assert;
import org.junit.jupiter.api.Test;

import java.io.IOException;

public class TestInterpreter {
    @Test
    public void testString() {
        verify("'abc'", "'abc'");
    }

    @Test
    public void testInt() {
        verify("123", "123");
    }

    @Test
    public void testIntList() {
        verify("[1, 2, 3]", "[1, 2, 3]");
    }

    @Test
    public void testStringList() {
        verify("['a', 'b', 'c']", "['a', 'b', 'c']");
    }

    @Test
    public void testBlock() {
        verify("12", "do 2 * 3; 3 * 4; end");
    }

    @Test
    public void testIfThen() {
        verify("'b'", "if 13 < 12 then 'a' if 11 < 12 then 'b'");
        verify("TRUE", "if 13 < 12 then 'a' if 14 < 12 then 'b'");
    }

    @Test
    public void testIfThenElse() {
        verify("'c'", "if 13 < 12 then 'a' if 14 < 12 then 'b' else 'c'");
    }

    @Test
    public void testIfThenWithBlock() {
        verify("'b'", "if 13 < 12 then do 'a' end if 11 < 12 then do 2 * 2; 'b'; end");
    }

    @Test
    public void testListWithIf() {
        verify("['a', 'x', 'd']", "[if 1 < 2 then 'a' else 'b', 'x', if 1 > 2 then 'c' else 'd']");
    }

    @Test
    public void testListWithFn() {
        verify("[1, <#lambda>, 2]", "[1, fn(x) 2*x, 2]");
    }
    @Test
    public void testForLoop() {
        verify("5", "for i in range(10) do if i == 5 then return i end");
    }

    @Test
    public void testDef() {
        verify("14", "def a=12;a+2");
    }

    @Test
    public void testAssign() {
        verify("27", "def a=12; def b = 3; a = b + 2 * a; a");
    }

    @Test
    public void testOrExpr() {
        verify("TRUE", "2 == 3 or 3 == 4 or 4 == 4");
        verify("FALSE", "2 == 3 or 3 == 4 or 4 == 5");
    }

    @Test
    public void testXorExpr() {
        verify("TRUE", "TRUE xor FALSE");
        verify("FALSE", "TRUE xor TRUE");
        verify("FALSE", "FALSE xor FALSE");
        verify("TRUE", "FALSE xor TRUE xor FALSE and TRUE");
        verify("TRUE", "FALSE xor TRUE or FALSE");
    }

    @Test
    public void testAndExpr() {
        verify("TRUE", "2 == 2 and 3 == 3 and 4 == 4");
        verify("FALSE", "2 == 2 and 3 == 3 and 4 == 5");
    }

    @Test
    public void testNotExpr() {
        verify("TRUE", "not 2 == 3");
        verify("FALSE", "not 2 == 2");
    }

    @Test
    public void testBooleanAlgebra() {
        verify("TRUE", "2 == 3 or 3 == 3 and not 4 == 5");
    }

    @Test
    public void testComparison() {
        verify("TRUE", "2 == 2");
        verify("TRUE", "2 == 2 != 3");
        verify("TRUE", "2 == 2 <> 3");
        verify("TRUE", "2 < 3");
        verify("TRUE", "2 < 3 <= 3 < 4");
        verify("FALSE", "2 < 3 < 3 <= 4");
        verify("TRUE", "5 >= 5 > 4 >= 2 == 2");
        verify("FALSE", "5 >= 5 > 5 >= 2 == 2");
    }

    @Test
    public void testArithmeticAdd() {
        verify("5", "2 + 3");
        verify("9", "2 + 3 + 4");
    }

    @Test
    public void testArithmeticSub() {
        verify("-1", "2 - 3");
        verify("-5", "2 - 3 - 4");
    }

    @Test
    public void testArithmeticMul() {
        verify("6", "2 * 3");
        verify("24", "2 * 3 * 4");
    }

    @Test
    public void testArithmeticDiv() {
        verify("3", "6 / 2");
        verify("0.5", "10 / 20.0");
    }

    @Test
    public void testArithmeticMod() {
        verify("0", "6 % 2");
        verify("1", "7 % 2");
    }

    @Test
    public void testArithmetic() {
        verify("7", "(2 + 3) * (3 + 4) / 5");
        verify("11", "2 + 3 * 3 + 4 / 5");
        verify("11.8", "2 + 3 * 3 + 4 / 5.0");
    }

    @Test
    public void testUnary() {
        verify("-2", "-2");
        verify("0", "2 + -2");
    }

    @Test
    public void testIsEmpty() {
        verify("TRUE", "[] is empty");
        verify("FALSE", "[1, 2] is empty");
    }

    @Test
    public void testIsNotEmpty() {
        verify("FALSE", "[] is not empty");
        verify("TRUE", "[1, 2] is not empty");
    }

    @Test
    public void testInList() {
        verify("TRUE", "feld1 is in ['M230', 'M231', 'M232']", "feld1", "M231");
        verify("FALSE", "feld1 in ['M230', 'M231', 'M232']", "feld1", "M233");
        verify("FALSE", "feld1 not in ['M230', 'M231', 'M232']", "feld1", "M231");
        verify("TRUE", "feld1 in [1, 2, 3]", "feld1", 2);
        verify("FALSE", "feld1 is in [1, 2, 3]", "feld1", "2");
        verify("FALSE", "feld1 in [1, 2, 3]", "feld1", 4);
    }

    @Test
    public void testIsZero() {
        verify("FALSE", "1 is zero");
        verify("TRUE", "0 is zero");
    }

    @Test
    public void testIsNotZero() {
        verify("TRUE", "1 is not zero");
    }

    @Test
    public void testIsNegative() {
        verify("FALSE", "0 is negative");
        verify("TRUE", "-1 is negative");
        verify("TRUE", "1 is not negative");
    }

    @Test
    public void testIsNumerical() {
        verify("TRUE", "'1234' is numerical");
        verify("TRUE", "'12a' is not numerical");
    }

    @Test
    public void testIsAlphanumerical() {
        verify("TRUE", "'abc123' is alphanumerical");
        verify("TRUE", "'abc--' is not alphanumerical");
    }

    @Test
    public void testIsDateWithHour() {
        verify("TRUE", "'2001010199' is not date with hour");
        verify("TRUE", "'2001010112' is date with hour");
        verify("FALSE", "'20010101' is date with hour");
    }

    @Test
    public void testIsDate() {
        verify("TRUE", "'20010133' is not date");
        verify("TRUE", "'20010101' is date");
    }

    @Test
    public void testIsTime() {
        verify("TRUE", "'1245' is time");
        verify("TRUE", "'2512' is not time");
    }

    @Test
    public void testStartsWith() {
        verify("TRUE", "'abc' starts with 'a'");
        verify("TRUE", "'abc' starts not with 'b'");
    }

    @Test
    public void testEndsWith() {
        verify("TRUE", "'abc' ends with 'c'");
        verify("TRUE", "'abc' ends not with 'b'");
    }

    @Test
    public void testContains() {
        verify("TRUE", "'abc' contains 'b'");
        verify("TRUE", "'abc' contains not 'x'");
    }

    @Test
    public void testMatches() {
        verify("TRUE", "'abc' matches //[a-z]+//");
        verify("TRUE", "'abc' matches not //[1-9]+//");
    }
    
    @Test
    public void testDeref() {
        verify("'b'", "feld1[1]", "feld1", "ab123");
        verify("3", "[1, 2, 3][2]");
        verify("'b'", "[['a', 1], ['b', 2]][1][0]");
        verify("'c'", "'abcd'[2]");
    }

    @Test
    public void testFuncDef() {
        verify("6", "def dup = fn(n) 2 * n; dup(3)");
    }

    @Test
    public void testFuncDefWithBlock() {
        verify("7", "def myfn = fn(n) do def m = 2 * n; if m % 2 == 0 then m + 1 else m end; myfn(3)");
    }

    @Test
    public void testLambda() {
        verify("'555555'", "(fn(a, b = 3) string(a) * b)(55)");
    }

    @Test
    public void testFuncRecursive() {
        verify("3628800", "def a = fn(x) do def y = x - 1; if x == 0 then 1 else x * a(y) end; a(10)");
    }

    @Test
    public void testFuncDefaultArg() {
        verify("10", "def a = fn(x = 12) x; a(10)");
        verify("12", "def a = fn(x = 12) x; a()");
    }

    @Test
    public void testFuncLength() {
        verify("3", "length('abc')");
        verify("3", "length([1, 2, 3])");
    }

    @Test
    public void testFuncLower() {
        verify("'abc'", "lower('Abc')");
    }

    @Test
    public void testFuncUpper() {
        verify("'ABC'", "upper('Abc')");
    }

    @Test
    public void testFuncNonZero() {
        verify("'12'", "non_zero('12', '3')");
        verify("'3'", "non_zero('0', '3')");
    }

    @Test
    public void testFuncNonEmpty() {
        verify("'12'", "non_empty('12', '3')");
        verify("'3'", "non_empty('', '3')");
    }

    @Test
    public void testFuncInt() {
        verify("12", "int('12')");
    }

    @Test
    public void testFuncDecimal() {
        verify("12.3", "decimal('12.3')");
    }

    @Test
    public void testFuncBoolean() {
        verify("TRUE", "boolean('1')");
        verify("FALSE", "boolean('0')");
    }

    @Test
    public void testFuncString() {
        verify("'123'", "string(123)");
    }

    @Test
    public void testFuncPattern() {
        verify("//^abc[0-9]+$//", "pattern('^abc[0-9]+$')");
    }

    @Test
    public void testFuncSplit() {
        verify("['a', 'b', 'ccc', 'd', 'e']", "split('a,b,ccc,d,e', ',')");
        verify("['a', 'b', 'ccc', 'd', 'e']", "split('a, b;ccc,d ,e', ' ?[,;] ?')");
    }

    @Test
    public void testList() {
        verify("[1, 2, 3]", "[1, 2, 3]");
        verify("[1, 2, 3]", "[1, 2, 3,]");
        verify("[1]", "[1]");
        verify("[]", "[]");
    }

    @Test
    public void testListComprehensionSimple() {
        verify("[0, 2, 4, 6, 8]", "[x * 2 for x in range(5)]");
    }

    @Test
    public void testListComprehensionWithCondition() {
        verify("[2, 6]", "[x * 2 for x in range(5) if x % 2 == 1]");
    }

    @Test
    public void testSetComprehensionSimple() {
        verify("<<0, 2, 4, 6, 8>>", "<<x * 2 for x in range(5)>>");
    }

    @Test
    public void testSetComprehensionWithCondition() {
        verify("<<2, 6>>", "<<x * 2 for x in range(5) if x % 2 == 1>>");
    }

    @Test
    public void testListComprehensionString() {
        verify("[1, 2, 3]", "[int(ch) for ch in '123']");
    }

    @Test
    public void testSetComprehensionString() {
        verify("<<1, 2, 3>>", "<<int(ch) for ch in '12312'>>");
    }

    @Test
    public void testMapComprehensionSimple() {
        verify("<<<0 => 0, 1 => 2, 2 => 4, 3 => 6, 4 => 8>>>", "<<<a => 2 * a for a in range(5)>>>");
    }

    @Test
    public void testMapComprehensionSimple2() {
        verify("<<<'x0' => 0, 'x1' => 2, 'x2' => 4, 'x3' => 6, 'x4' => 8>>>", "<<<'x' + a => 2 * a for a in range(5)>>>");
    }

    @Test
    public void testMapComprehensionWithCondition() {
        verify("<<<0 => 0, 1 => 2, 2 => 4>>>", "<<<a => 2 * a for a in range(5) if 2 * a < 6>>>");
    }

    @Test
    public void testFuncRange() {
        verify("[]", "range()");
        verify("[0, 1, 2, 3, 4, 5, 6, 7, 8, 9]", "range(10)");
        verify("[5, 6, 7, 8, 9]", "range(5, 10)");
        verify("[10, 9, 8, 7, 6]", "range(10, 5, -1)");
        verify("[]", "range(5, 10, -1)");
    }

    @Test
    public void testFuncSubstr() {
        verify("'def'", "substr('abcdef', 3)");
        verify("'d'", "substr('abcdef', 3, 4)");
        verify("'f'", "substr('abcdef', 5)");
        verify("''", "substr('abcdef', 6)");
    }

    @Test
    public void testFuncSublist() {
        verify("[3, 4]", "sublist([1, 2, 3, 4], 2)");
        verify("[3]", "sublist([1, 2, 3, 4], 2, 3)");
        verify("[4]", "sublist([1, 2, 3, 4], 3)");
        verify("[]", "sublist([1, 2, 3, 4], 4)");
    }

    @Test
    public void testFuncFindStr() {
        verify("1", "find('abcd', 'b')");
        verify("-1", "find('abcd', 'e')");
    }

    @Test
    public void testFuncStrFind() {
        verify("1", "str_find('abcd', 'bc')");
        verify("-1", "str_find('abcd', 'de')");
    }

    @Test
    public void testFuncFindList() {
        verify("1", "find([1, 2, 3], 2)");
        verify("-1", "find([1, 2, 3], 4)");
    }

    @Test
    public void testFuncFindListWithKey() {
        verify("1", "find([[1, 'a'], [2, 'b'], [3, 'c']], 2, fn(x) x[0])");
        verify("-1", "find([[1, 'a'], [2, 'b'], [3, 'c']], 4, fn(x) x[0])");
    }

    @Test
    public void testFuncSet() {
        verify("<<1, 2, 3, 4, 5>>", "set([1, 2, 3, 3, 4, 5])");
    }

    @Test
    public void testFuncMap() {
        verify("<<<1 => 'a', 2 => 'b', 3 => 'd', 4 => 'e', 5 => 'f'>>>", "map([[1, 'a'], [2, 'b'], [3, 'c'], [3, 'd'], [4, 'e'], [5, 'f']])");
    }

    @Test
    public void testFuncSubstitute() {
        verify("'abcxef'", "substitute('abcdef', 3, 'x')");
        verify("[1, 2, 'x', 4]", "substitute([1, 2, 3, 4], 2, 'x')");
    }

    @Test
    public void testFuncRandom() {
        verify("5", "set_seed(1); random(10)");
    }

    @Test
    public void testEnvByteGet() {
        Byte b = 12;
        Environment env = Environment.getBaseEnvironment();
        env.put("test", b);
        Assert.assertEquals(12, env.get("test", null).asInt().getValue());
    }

    @Test
    public void testFuncSqrt() {
        verify("2.0", "sqrt(4)");
    }

    @Test
    public void testBlockFuncOrdering() {
        verify("30", "def a = fn(y) do def b = fn(x) 2 * c(x); def c = fn(x) 3 + x; b(y); end; a(12)");
    }

    @Test
    public void testBlockFuncOrderingGlobal() {
        verify("30", "def b = fn(x) 2 * c(x); def c = fn(x) 3 + x; b(12)");
    }

    @Test
    public void testEmptyListLiteral() { verify("[2, 3]", "def f(x, y) do def r = []; append(r, x); append(r, y); return r; end; f(1, 2); f(2, 3);"); }

    @Test
    public void testNonEmptyListLiteral() { verify("[1, 2, 3]", "def f(x, y) do def r = [1]; append(r, x); append(r, y); return r; end; f(1, 2); f(2, 3);"); }

    @Test
    public void testFuncType() {
        verify("'int'", "type(4)");
        verify("'decimal'", "type(4.0)");
        verify("'string'", "type('a')");
        verify("'pattern'", "type(//a//)");
        verify("'list'", "type([1])");
        verify("'set'", "type(set([1]))");
        verify("'map'", "type(map([[1, 2]]))");
        verify("'func'", "type(fn(x) x)");
        verify("'boolean'", "type(TRUE)");
        verify("'date'", "type(date('20120101'))");
    }

    @Test
    public void TestParseSimpleReturn() {
        verify("(add x, 1)", "parse('return x + 1')");
    }

    @Test
    public void TestParseSingleBlockReturn() {
        verify("(add x, 1)", "parse('do return x + 1; end')");
    }

    @Test
    public void TestParseBlockReturn() {
        verify("(block (def x = 1), (add x, 1))", "parse('do def x = 1; return x + 1; end')");
    }

    @Test
    public void TestParseBlockEarlyReturn() {
        verify("(block (return (add x, 1)), (def x = 1))", "parse('do return x + 1; def x = 1; end')");
    }

    @Test
    public void TestParseBareBlockReturn() {
        verify("(block (def x = 1), (add x, 1))", "parse('def x = 1; return x + 1;')");
    }

    @Test
    public void TestParseBareBlockEarlyReturn() {
        verify("(block (return (add x, 1)), (def x = 1))", "parse('return x + 1; def x = 1')");
    }

    @Test
    public void TestParseLambdaReturn() {
        verify("(def fun = (lambda x, (add x, 1)))", "parse('def fun(x) return x + 1')");
    }

    @Test
    public void TestParseLambdaBlockReturn() {
        verify("(def fun = (lambda x, (block (x = (mul x, 2)), (add x, 1))))", "parse('def fun(x) do x = x * 2; return x + 1; end')");
    }

    @Test
    public void TestParseLambdaBlockEarlyReturn() {
        verify("(def fun = (lambda x, (block (return (add x, 1)), (x = (mul x, 2)))))", "parse('def fun(x) do return x + 1; x = x * 2; end')");
    }

    @Test
    public void testSpreadInListLiteral() {
        verify("[1, 2, 3, 4, 5]", "[1, ...[2, 3, 4], 5]");
    }

    @Test
    public void testSpreadInListLiteralWithIdentifier() {
        verify("[1, 2, 3, 4, 5]", "def a = [2, 3, 4]; [1, ...a, 5]");
    }

    @Test
    public void testSpreadInFuncallBasic() {
        verify("[1, 2, 3]", "def f(a, b, c) [a, b, c]; f(1, ...[2, 3])");
    }

    @Test
    public void testSpreadInFuncallMap() {
        verify("[1, 2, 3]", "def f(a, b, c) [a, b, c]; f(...<<<'c' => 3, 'a' => 1, 'b' => 2>>>)");
    }

    @Test
    public void testSpreadInFuncall() {
        verify("[1, 2, 3, 4, 5]", "def f(args...) args...; f(1, ...[2, 3, 4], 5)");
    }

    @Test
    public void testSpreadInFuncallWithIdentifier() {
        verify("[1, 2, 3, 4, 5]", "def f(args...) args...; def a = [2, 3, 4]; f(1, ...a, 5)");
    }
    
    @Test
    public void testDefDestructure1() {
        verify("[1, 2]", "def [a, b] = [1, 2]; [a, b]");
    }

    @Test
    public void testDefDestructure2() {
           verify("1", "def [a] = [1, 2]; a");
    }
        
    @Test
    public void testDefDestructure3() {
        verify("3", "def [a, b, c] = <<1, 2, 3>>; c");
    }
        
    @Test
    public void testDefDestructure4() {
        verify("NULL", "def [a, b, c] = [1, 2]");
    }
        
    @Test
    public void testAssignDestructure1() {
        verify("[1, 2]", "def a = 1; def b = 1; [a, b] = [1, 2]; [a, b]");
    }
        
    @Test
    public void testAssignDestructure2() {
        verify("2", "def a = 1; [a] = [2, 3]; a");
    }
        
    @Test
    public void testSwapUsingDestructure() {
        verify("[2, 1]", "def a = 1; def b = 2; [a, b] = [b, a]; [a, b]");
    }

    @Test
    public void testAll() {
        verify("TRUE", "all([2, 4, 6], fn(x) x % 2 == 0)");
    }

    @Test
    public void testMethods1() {
        verify("TRUE", "'abcdef'!>starts_with('abc')");
    }

    @Test
    public void testMethods2() {
        verify("'xy'", "' xy '!>trim()");
    }

    @Test
    public void testMethods3() {
        verify("[3, 2, 1]", "require List; [1, 2, 3]!>List->reverse()");
    }

    @Test
    public void testMethods4() {
        verify("TRUE", "[2, 4, 6] !> all(fn(x) x % 2 == 0)");
    }

    @Test
    public void testMethods5() {
        verify("12", "12 !> max(2)");
    }

    @Test
    public void testMethods6() {
        verify("'3-2-1'", "require List; [1, 2, 3] !> List->reverse() !> join(sep = '-')");
    }

    @Test
    public void testDerefProperty() {
        verify("2", "def a = <<<'x' => 1, 'y' => 2>>>; a->y");
    }

    @Test
    public void testMapLiteralImplicitString() {
        verify("<<<'x' => 1, 'y' => 2>>>", "<<<x => 1, y => 2>>>");
    }

    @Test
    public void testPipelineLambda() { verify("3", "[1, 2, 3] !> (fn(lst) lst[2])()"); }

    @Test
    public void testIfThenElifThenElse() {
        verify("5", "if 1 == 2 then 3 elif 1 == 3 then 4 elif 1 == 1 then 5 else 6");
    }

    @Test
    public void testForDestructuringListList() {
        verify("21", "def a = 0; for [x, y, z] in [[1, 2, 3], [4, 5, 6]] do a += x + y + z; end; a;");
    }

    @Test
    public void testForDestructuringListSet() {
        verify("21", "def a = 0; for [x, y, z] in [<<1, 2, 3>>, <<4, 5, 6>>] do a += x + y + z; end; a;");
    }

    @Test
    public void testForDestructuringSetList() {
        verify("21", "def a = 0; for [x, y, z] in <<[1, 2, 3], [4, 5, 6]>> do a += x + y + z; end; a;");
    }

    @Test
    public void testForDestructuringSetSet() {
        verify("21", "def a = 0; for [x, y, z] in << <<1, 2, 3>>, <<4, 5, 6>> >> do a += x + y + z; end; a;");
    }

    @Test
    public void TestForMapValues() {
        verify("[1, 2, 3]",
                "def result = []; def obj = <<<a=>1, b=>2, c=>3>>>; for o in values obj do append(result, o) end; result;");
    }

    @Test
    public void TestForMapDefault() {
        verify("[1, 2, 3]",
                "def result = []; def obj = <<<a=>1, b=>2, c=>3>>>; for o in obj do append(result, o); end; result;");
    }

    @Test
    public void TestForMapKeys() {
        verify("['a', 'b', 'c']",
                "def result = []; def obj = <<<a=>1, b=>2, c=>3>>>; for o in keys obj do append(result, o); end; result;");
    }

    @Test
    public void TestForMapEntries() {
        verify("[['a', 1], ['b', 2], ['c', 3]]",
                "def result = []; def obj = <<<a=>1, b=>2, c=>3>>>; for o in entries obj do append(result, o); end; result;");
    }

    @Test
    public void TestForObjectValues() {
        verify("[1, 2, 3]",
                "def result = []; def obj = <*a=1, b=2, c=3*>; for o in values obj append(result, o); result;");
    }

    @Test
    public void TestForObjectDefault() {
        verify("[1, 2, 3]",
                "def result = []; def obj = <*a=1, b=2, c=3*>; for o in obj append(result, o); result;");
    }

    @Test
    public void TestForObjectKeys() {
        verify("['a', 'b', 'c']",
                "def result = []; def obj = <*a=1, b=2, c=3*>; for o in keys obj append(result, o); result;");
    }

    @Test
    public void TestForObjectEntries() {
        verify("[['a', 1], ['b', 2], ['c', 3]]",
                "def result = []; def obj = <*a=1, b=2, c=3*>; for o in entries obj append(result, o); result;");
    }

    @Test
    public void TestListComprehensionParallel() {
        verify("[1, 4, 9]",
                "[a * b for a in [1, 2, 3] also for b in [1, 2, 3]]");
    }

    @Test
    public void TestListComprehensionProduct() {
        verify("[1, 2, 3, 2, 4, 6, 3, 6, 9]",
                "[a * b for a in [1, 2, 3] for b in [1, 2, 3]]");
    }

    @Test
    public void TestSetComprehensionParallel() {
        verify("<<1, 4, 9>>",
                "<<a * b for a in [1, 2, 3] also for b in [1, 2, 3]>>");
    }

    @Test
    public void TestSetComprehensionProduct() {
        verify("<<1, 2, 3, 4, 6, 9>>",
                "<<a * b for a in [1, 2, 3] for b in [1, 2, 3]>>");
    }

    @Test public void TestListComprehensionKeysMap() {
        verify("['a', 'b']", "[x for x in keys <<<'a' => 12, 'b' => 13>>>]");
    }

    @Test public void TestListComprehensionValuesMap() {
        verify("[12, 13]", "[x for x in values <<<'a' => 12, 'b' => 13>>>]");
    }

    @Test public void TestListComprehensionEntriesMap() {
        verify("[['a', 12], ['b', 13]]", "[x for x in entries <<<'a' => 12, 'b' => 13>>>]");
    }

    @Test public void TestListComprehensionKeysObject() {
        verify("['a', 'b']", "[x for x in keys <*a = 12, b = 13*>]");
    }

    @Test public void TestListComprehensionValuesObject() {
        verify("[12, 13]", "[x for x in values <*a = 12, b = 13*>]");
    }

    @Test public void TestListComprehensionEntriesObject() {
        verify("[['a', 12], ['b', 13]]", "[x for x in entries <*a = 12, b = 13*>]");
    }

    @Test public void TestSetComprehensionKeysMap() {
        verify("<<'a', 'b'>>", "<<x for x in keys <<<'a' => 12, 'b' => 13>>> >>");
    }

    @Test public void TestSetComprehensionValuesMap() {
        verify("<<12, 13>>", "<<x for x in values <<<'a' => 12, 'b' => 13>>> >>");
    }

    @Test public void TestSetComprehensionEntriesMap() {
        verify("<<['a', 12], ['b', 13]>>", "<<x for x in entries <<<'a' => 12, 'b' => 13>>> >>");
    }

    @Test public void TestMapComprehensionValuesMap() {
        verify("<<<'u' => 12, 'v' => 13>>>", "<<<x[0] => x[1] for x in values <<<'a' => ['u', 12], 'b' => ['v', 13]>>> >>>");
    }

    @Test public void TestMapComprehensionEntriesMap() {
        verify("<<<'a' => 12, 'b' => 13>>>", "<<<x[0] => x[1][1] for x in entries <<<'a' => ['u', 12], 'b' => ['v', 13]>>> >>>");
    }

    @Test
    public void testMapDefaultValue() {
        verify("-1", "def m = <<<'a' => 12>>>; m['b', -1]");
    }

    @Test
    public void testMapDefaultIncrement() {
        verify("1", "def m = <<<>>>; m['a', 0] += 1; m['a']");
    }

    private void verify(String expected, String script) {
        Environment env = new Environment();
        try {
            Value result = new Interpreter().interpret(script, "test", env);
            Assert.assertEquals(expected, result.toString());
        } catch (IOException e) {
            e.printStackTrace();
            Assert.fail();
        }
    }

    private void verify(String expected, String script, String variable, Object value) {
        Environment env = new Environment();
        env.put(variable, value);
        try {
            Value result = new Interpreter().interpret(script, "test", env);
            Assert.assertEquals(expected, result.toString());
        } catch (IOException e) {
            Assert.fail();
        }
    }
}
