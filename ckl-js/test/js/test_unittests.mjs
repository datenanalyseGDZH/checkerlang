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

import { Lexer } from "../../js/lexer.mjs";
import { Parser } from "../../js/parser.mjs";
import { Environment } from "../../js/functions.mjs";
import { Interpreter } from "../../js/interpreter.mjs";
import { isLeapYear, convertDateToOADate, convertOADateToDate, ValueString, ValueInt, ValueList } from "../../js/values.mjs";
import { HashSet, HashMap } from "../../js/collections.mjs";

let tests_run = 0;
let tests_failed = 0;

function addLine(line) {
    const div = document.createElement("div");
    div.appendChild(document.createTextNode(line));
    document.getElementById("output").appendChild(div);
}

function addReport(lines) {
    lines.forEach(line => addLine(line));
    document.getElementById("output").appendChild(document.createElement("hr"));
}

function run_test(description, runfunction, expected) {
    try {
        tests_run++;
        const result = runfunction();
        if (result !== expected) {
            addReport([
                "Test '" + description + "' failed:", 
                "Expected: " + expected,
                "Result__: " + result]);
            tests_failed++;
        }
    } catch (e) {
        console.log(e);
        const report = document.createElement("div");
        addReport([
            "Test '" + description + "' failed:", 
            "Expected: " + expected,
            "Result__:  exception thrown: " + e.toString()]);
        tests_failed++;
    }
}

function run_test_exception(description, runfunction) {
    try {
        tests_run++;
        const result = runfunction();
        addReport([
            "Test '" + description + "' failed:", 
            "Expected: exception but got " + result]);
        tests_failed++;
    } catch (e) {
        // got exception, as expected
    }
}

function lexer_test(description, code, expected) {
    run_test("Lexer:" + description, function() { return Lexer.init(code, "{test}").toString() }, expected);
}
function lexer_test_exception(description, code) {
    run_test_exception("Lexer:" + description, function() { return Lexer.init(code, "{test}").toString() });
}

lexer_test("Simple", "1_6_V04 nicht leer", "[1_6_V04 (identifier), nicht (identifier), leer (identifier)] @ 0");
lexer_test("Quotes", "a \"double\" b 'single' c", "[a (identifier), double (string), b (identifier), single (string), c (identifier)] @ 0");
lexer_test("Pattern", "//abc//", "[//abc// (pattern)] @ 0");
lexer_test("In", "feld1 in ['a', 'bb', 'ccc']", "[feld1 (identifier), in (keyword), [ (interpunction), a (string), , (interpunction), bb (string), , (interpunction), ccc (string), ] (interpunction)] @ 0");
lexer_test("Complex", "(felda beginnt mit 'D12') oder (feldb <= 1.3*(feldc-feldd/2))", 
            "[( (interpunction), felda (identifier), beginnt (identifier), mit (identifier), D12 (string), ) (interpunction), oder (identifier), ( (interpunction), feldb (identifier), " + 
            "<= (operator), 1.3 (decimal), * (operator), ( (interpunction), feldc (identifier), - (operator), feldd (identifier), / (operator), 2 (int), ) (interpunction), ) (interpunction)] @ 0");
lexer_test("Compare", "1>2 d", "[1 (int), > (operator), 2 (int), d (identifier)] @ 0");
lexer_test("NonZero", "non_zero('12', '3')", "[non_zero (identifier), ( (interpunction), 12 (string), , (interpunction), 3 (string), ) (interpunction)] @ 0");
lexer_test("SetLiteral", "a in <<1, 2, 3>>", "[a (identifier), in (keyword), << (interpunction), 1 (int), , (interpunction), 2 (int), , (interpunction), 3 (int), >> (interpunction)] @ 0");
lexer_test("MapLiteral", "def m = <<<1 => 100, 2=>200 >>>", 
            "[def (keyword), m (identifier), = (operator), <<< (interpunction), 1 (int), => (interpunction), 100 (int), , (interpunction), 2 (int), => (interpunction), 200 (int), >>> (interpunction)] @ 0");
lexer_test("List", "a in [1, 2, 3]", "[a (identifier), in (keyword), [ (interpunction), 1 (int), , (interpunction), 2 (int), , (interpunction), 3 (int), ] (interpunction)] @ 0");
lexer_test("ListAddInt", "[1, 2, 3] + 4", "[[ (interpunction), 1 (int), , (interpunction), 2 (int), , (interpunction), 3 (int), ] (interpunction), + (operator), 4 (int)] @ 0");
lexer_test("SpreadOperatorIdentifier", "...a", "[... (interpunction), a (identifier)] @ 0");
lexer_test("SpreadOperatorList", "...[1, 2]", "[... (interpunction), [ (interpunction), 1 (int), , (interpunction), 2 (int), ] (interpunction)] @ 0");
lexer_test("SpreadOperatorFuncall", "f(a, ...b, c)", "[f (identifier), ( (interpunction), a (identifier), , (interpunction), ... (interpunction), b (identifier), , (interpunction), c (identifier), ) (interpunction)] @ 0");
lexer_test("InvokeOperator", "a!>b", "[a (identifier), !> (operator), b (identifier)] @ 0");
lexer_test("StringLiteralWithNewline", "'one\\ntwo'", "[one\\ntwo (string)] @ 0");
lexer_test("DerefProperty", "a->b ->c -> d", "[a (identifier), -> (operator), b (identifier), -> (operator), c (identifier), -> (operator), d (identifier)] @ 0");
lexer_test("IfThenElifElseKeywords", "if a then b elif c then d else e", "[if (keyword), a (identifier), then (keyword), b (identifier), elif (keyword), c (identifier), then (keyword), d (identifier), else (keyword), e (identifier)] @ 0");

function parser_test(description, code, expected) {
    run_test("Parser:" + description, function() { return Parser.parseScript(code, "{test}").toString(); }, expected);
}
function parser_test_exception(description, code) {
    run_test_exception("Parser:" + description, function() { return Parser.parseScript(code, "{test}").toString(); });
}

parser_test("Simple", "eins", "eins");
parser_test("Addition", "2 + 3", "(add 2, 3)");
parser_test("Wenn", "if 1>2 then TRUE else FALSE", "(if (greater 1, 2): TRUE else: FALSE)");
parser_test("In", "feld1 in ['a', 'bb', 'ccc']", "(feld1 in ['a', 'bb', 'ccc'])");
parser_test("InWithSet", "feld1 in <<'a', 'bb', 'ccc'>>", "(feld1 in <<'a', 'bb', 'ccc'>>)");
parser_test("IsZero", "1 is zero", "(is_zero 1)");
parser_test("LiteralMap", "<<<'a' => 1, 'bb' => -1, 'ccc' => 100>>>", "<<<'a' => 1, 'bb' => -1, 'ccc' => 100>>>");
parser_test("LiteralSet", "<<1, 2, 2, 3>>", "<<1, 2, 2, 3>>");
parser_test("ListAddInt", "[1, 2, 3] + 4", "(add [1, 2, 3], 4)");
parser_test("Relop1", "a < b", "(less a, b)");
parser_test("Relop2", "a < b < c", "((less a, b) and (less b, c))");
parser_test("Relop3", "a <= b < c == d", "((less_equals a, b) and (less b, c) and (equals c, d))");
parser_test("NonZeroFuncall", "non_zero('12', '3')", "(non_zero '12', '3')");
parser_test_exception("TooManyTokens", "1 + 1 1");
parser_test_exception("NotEnoughTokens", "1 + ");
parser_test_exception("MissingThen", "if 1 < 2 else FALSE");
parser_test("IfThenOrExpr", "if a == 1 then b in c or d == 9999", "(if (equals a, 1): ((b in c) or (equals d, 9999)) else: TRUE)");
parser_test("IfThenElif", "if a == 1 then b elif c == 1 or d == 2 then b in c or d == 9999", "(if (equals a, 1): b if ((equals c, 1) or (equals d, 2)): ((b in c) or (equals d, 9999)) else: TRUE)");
parser_test_exception("MissingClosingParens", "2 * (3 + 4( - 3");
parser_test("Lambda", "fn(a, b=3) string(a) * b(2, 3)", "(lambda a, b=3, (mul (string a), (b 2, 3)))");
parser_test("While", "while x > 0 do x = x - 1; end", "(while (greater x, 0) do (x = (sub x, 1)))");
parser_test("SpreadIdentifier", "f(a, ...b, c)", "(f a, ...b, c)");
parser_test("SpreadList", "f(a, ...[1, 2], c)", "(f a, ...[1, 2], c)");
parser_test("DefDestructure", "def [a, b] = [1, 2]", "(def [a,b] = [1, 2])");
parser_test("AssignDestructure", "[a, b] = [1, 2]", "([a,b] = [1, 2])");
parser_test("Pipeline", "0 !> sprintf(fmt=\"part2: {0}\") !> println()", "(println (sprintf 0, 'part2: {0}'))");


function interpreter_test(description, code, expected) {
    run_test("Interpreter:" + description, function() { return new Interpreter().interpret(code, "{test}").toString(); }, expected);
}
function interpreter_test_exception(description, code) {
    run_test_exception("Interpreter:" + description, function() { return new interpreter().interpret(code, "{test}").toString(); });
}

interpreter_test("String", "'abc'" , "'abc'");
interpreter_test("Int", "123", "123");
interpreter_test("IntList", "[1, 2, 3]", "[1, 2, 3]");
interpreter_test("Block", "do 2 * 3; 3 * 4; end", "12");
interpreter_test("IfThen", "if 13 < 12 then 'a' if 11 < 12 then 'b'", "'b'");
interpreter_test("IfThen", "if 13 < 12 then 'a' if 14 < 12 then 'b'", "TRUE");
interpreter_test("IfThenElse", "if 13 < 12 then 'a' if 14 < 12 then 'b' else 'c'", "'c'");
interpreter_test("IfThenWithBlock", "if 13 < 12 then do 'a' end if 11 < 12 then do 2 * 2; 'b'; end", "'b'");
interpreter_test("ListWithIf", "[if 1 < 2 then 'a' else 'b', 'x', if 1 > 2 then 'c' else 'd']", "['a', 'x', 'd']");
interpreter_test("ListWithFn", "[1, fn(x) 2*x, 2]", "[1, <#lambda>, 2]");
interpreter_test("ForLoop", "for i in range(10) do if i == 5 then return i end", "5");
interpreter_test("Def", "def a=12;a+2", "14");
interpreter_test("Assign", "def a=12; def b = 3; a = b + 2 * a; a", "27");
interpreter_test("OrExpr", "2 == 3 or 3 == 4 or 4 == 4", "TRUE");
interpreter_test("OrExpr", "2 == 3 or 3 == 4 or 4 == 5", "FALSE");
interpreter_test("AndExpr", "2 == 2 and 3 == 3 and 4 == 4", "TRUE");
interpreter_test("AndExpr", "2 == 2 and 3 == 3 and 4 == 5", "FALSE");
interpreter_test("NotExpr", "not 2 == 3", "TRUE");
interpreter_test("NotExpr", "not 2 == 2", "FALSE");
interpreter_test("BooleanAlgebra", "2 == 3 or 3 == 3 and not 4 == 5", "TRUE");
interpreter_test("Comparison", "2 == 2", "TRUE");
interpreter_test("Comparison", "2 == 2 != 3", "TRUE");
interpreter_test("Comparison", "2 == 2 <> 3", "TRUE");
interpreter_test("Comparison", "2 < 3", "TRUE");
interpreter_test("Comparison", "2 < 3 <= 3 < 4", "TRUE");
interpreter_test("Comparison", "2 < 3 < 3 <= 4", "FALSE");
interpreter_test("Comparison", "5 >= 5 > 4 >= 2 == 2", "TRUE");
interpreter_test("Comparison", "5 >= 5 > 5 >= 2 == 2", "FALSE");
interpreter_test("ArithmeticAdd", "2 + 3", "5");
interpreter_test("ArithmeticAdd", "2 + 3 + 4", "9");
interpreter_test("ArithmeticSub", "2 - 3", "-1");
interpreter_test("ArithmeticSub", "2 - 3 - 4", "-5");
interpreter_test("ArithmeticMul", "2 * 3", "6");
interpreter_test("ArithmeticMul", "2 * 3 * 4", "24");
interpreter_test("ArithmeticDiv", "6 / 2", "3");
interpreter_test("ArithmeticDiv", "10 / 20.0", "0.5");
interpreter_test("ArithmeticMod", "6 % 2", "0");
interpreter_test("ArithmeticMod", "7 % 2", "1");
interpreter_test("Arithmetic", "(2 + 3) * (3 + 4) / 5", "7");
interpreter_test("Arithmetic", "2 + 3 * 3 + 4 / 5", "11");
interpreter_test("Arithmetic", "2 + 3 * 3 + 4 / 5.0", "11.8");
interpreter_test("Unary", "-2", "-2");
interpreter_test("Unary", "2 + -2", "0");
interpreter_test("IsEmpty", "[] is empty", "TRUE");
interpreter_test("IsEmpty", "[1, 2] is empty", "FALSE");
interpreter_test("IsNotEmpty", "[] is not empty", "FALSE");
interpreter_test("IsNotEmpty", "[1, 2] is not empty", "TRUE");
interpreter_test("InList", "def feld1 = 'M231'; feld1 is in ['M230', 'M231', 'M232']", "TRUE");
interpreter_test("InList", "def feld1 = 'M233'; feld1 is in ['M230', 'M231', 'M232']", "FALSE");
interpreter_test("InList", "def feld1 = 'M231'; feld1 not in ['M230', 'M231', 'M232']", "FALSE");
interpreter_test("InList", "def feld1 = 2; feld1 in [1, 2, 3]", "TRUE");
interpreter_test("InList", "def feld1 = '2'; feld1 is in [1, 2, 3]", "FALSE");
interpreter_test("InList", "def feld1 = 4; feld1 in [1, 2, 3]", "FALSE");
interpreter_test("IsZero1", "1 is zero", "FALSE");
interpreter_test("IsZero2", "0 is zero", "TRUE");
interpreter_test("IsNotZero", "1 is not zero", "TRUE");
interpreter_test("IsNegative1", "0 is negative", "FALSE");
interpreter_test("IsNegative2", "-1 is negative", "TRUE");
interpreter_test("IsNotNegative", "1 is not negative", "TRUE");
interpreter_test("IsNumerical", "'1234' is numerical", "TRUE");
interpreter_test("IsNotNumerical", "'12a' is not numerical", "TRUE");
interpreter_test("IsAlphanumerical", "'abc123' is alphanumerical", "TRUE");
interpreter_test("IsNotAlphanumerical", "'abc--' is not alphanumerical", "TRUE");
interpreter_test("IsNotDateWithHour", "'2001010199' is not date with hour", "TRUE");
interpreter_test("IsDateWithHour1", "'2001010112' is date with hour", "TRUE");
interpreter_test("IsDateWithHour2", "'20010101' is date with hour", "FALSE");
interpreter_test("IsNotDate", "'20010133' is not date", "TRUE");
interpreter_test("IsDate", "'20010101' is date", "TRUE");
interpreter_test("IsTime", "'1245' is time", "TRUE");
interpreter_test("IsNotTime", "'2512' is not time", "TRUE");
interpreter_test("StartsWith", "'abc' starts with 'a'", "TRUE");
interpreter_test("StartsNotWith", "'abc' starts not with 'b'", "TRUE");
interpreter_test("EndsWith", "'abc' ends with 'c'", "TRUE");
interpreter_test("EndsNotWith", "'abc' ends not with 'b'", "TRUE");
interpreter_test("Contains", "'abc' contains 'b'", "TRUE");
interpreter_test("ContainsNot", "'abc' contains not 'x'", "TRUE");
interpreter_test("Matches", "'abc' matches //[a-z]+//", "TRUE");
interpreter_test("MatchesNot", "'abc' matches not //[1-9]+//", "TRUE");
interpreter_test("Deref", "def feld1 = 'abc123'; feld1[1]", "'b'");
interpreter_test("Deref", "[1, 2, 3][2]", "3");
interpreter_test("Deref", "[['a', 1], ['b', 2]][1][0]", "'b'");
interpreter_test("Deref", "'abcd'[2]'", "'c'");
interpreter_test("FuncDef", "def dup = fn(n) 2 * n; dup(3)", "6");
interpreter_test("FuncDef", "def dup(n) 2 * n; dup(3)", "6");
interpreter_test("FuncDefWithBlock", "def myfn = fn(n) do def m = 2 * n; if m % 2 == 0 then m + 1 else m end; myfn(3)", "7");
interpreter_test("Lambda", "(fn(a, b = 3) string(a) * b)(55)", "'555555'");
interpreter_test("FuncRecursive", "def a = fn(x) do def y = x - 1; if x == 0 then 1 else x * a(y) end; a(10)", "3628800");
interpreter_test("FuncDefaultArg", "def a = fn(x = 12) x; a(10)", "10");
interpreter_test("FuncDefaultArg", "def a = fn(x = 12) x; a()", "12");
interpreter_test("FuncLength", "length('abc')", "3");
interpreter_test("FuncLength", "length([1, 2, 3])", "3");
interpreter_test("FuncLower", "lower('Abc')", "'abc'");
interpreter_test("FuncUpper", "upper('Abc')", "'ABC'");
interpreter_test("FuncNonZero", "non_zero('12', '3')", "'12'");
interpreter_test("FuncNonZero", "non_zero('0', '3')", "'3'");
interpreter_test("FuncNonEmpty", "non_empty('12', '3')", "'12'");
interpreter_test("FuncNonEmpty", "non_empty('', '3')", "'3'");
interpreter_test("FuncInt", "int('12')", "12");
interpreter_test("FuncDecimal", "decimal('12.3')", "12.3");
interpreter_test("FuncBoolean", "boolean('1')", "TRUE");
interpreter_test("FuncBoolean", "boolean('0')", "FALSE");
interpreter_test("FuncString", "string(123)", "'123'");
interpreter_test("FuncPattern", "pattern('^abc[0-9]+$')", "//^abc[0-9]+$//");
interpreter_test("FuncSplit", "split('a,b,ccc,d,e', ',')", "['a', 'b', 'ccc', 'd', 'e']");
interpreter_test("FuncSplit", "split('a, b;ccc,d ,e', ' ?[,;] ?')", "['a', 'b', 'ccc', 'd', 'e']");
interpreter_test("List", "[1, 2, 3]", "[1, 2, 3]");
interpreter_test("List", "[1, 2, 3,]", "[1, 2, 3]");
interpreter_test("List", "[1]", "[1]");
interpreter_test("List", "[]", "[]");
interpreter_test("ListComprehensionSimple", "[x * 2 for x in range(5)]", "[0, 2, 4, 6, 8]");
interpreter_test("ListComprehensionWithCondition", "[x * 2 for x in range(5) if x % 2 == 1]", "[2, 6]");
interpreter_test("FuncRange", "range()", "[]");
interpreter_test("FuncRange", "range(10)", "[0, 1, 2, 3, 4, 5, 6, 7, 8, 9]");
interpreter_test("FuncRange", "range(5, 10)", "[5, 6, 7, 8, 9]");
interpreter_test("FuncRange", "range(10, 5, -1)", "[10, 9, 8, 7, 6]");
interpreter_test("FuncRange", "range(5, 10, -1)", "[]");
interpreter_test("FuncSubstr", "substr('abcdef', 3)", "'def'");
interpreter_test("FuncSubstr", "substr('abcdef', 3, 4)", "'d'");
interpreter_test("FuncSubstr", "substr('abcdef', 5)", "'f'");
interpreter_test("FuncSubstr", "substr('abcdef', 6)", "''");
interpreter_test("FuncSublist", "sublist([1, 2, 3, 4], 2)", "[3, 4]");
interpreter_test("FuncSublist", "sublist([1, 2, 3, 4], 2, 3)", "[3]");
interpreter_test("FuncSublist", "sublist([1, 2, 3, 4], 3)", "[4]");
interpreter_test("FuncSublist", "sublist([1, 2, 3, 4], 4)", "[]");
interpreter_test("FuncFindStr", "find('abcd', 'b')", "1");
interpreter_test("FuncFindStr", "find('abcd', 'e')", "-1");
interpreter_test("FuncStrFind", "str_find('abcd', 'bc')", "1");
interpreter_test("FuncStrFind", "str_find('abcd', 'de')", "-1");
interpreter_test("FuncFindList", "find([1, 2, 3], 2)", "1");
interpreter_test("FuncFindList", "find([1, 2, 3], 4)", "-1");
interpreter_test("FuncFindListWithKey", "find([[1, 'a'], [2, 'b'], [3, 'c']], 2, fn(x) x[0])", "1");
interpreter_test("FuncFindListWithKey", "find([[1, 'a'], [2, 'b'], [3, 'c']], 4, fn(x) x[0])", "-1");
interpreter_test("FuncSet", "set([1, 2, 3, 3, 4, 5])", "<<1, 2, 3, 4, 5>>");
interpreter_test("FuncMap", "map([[1, 'a'], [2, 'b'], [3, 'c'], [3, 'd'], [4, 'e'], [5, 'f']])", "<<<1 => 'a', 2 => 'b', 3 => 'd', 4 => 'e', 5 => 'f'>>>");
interpreter_test("FuncSubstitute", "substitute('abcdef', 3, 'x')", "'abcxef'");
interpreter_test("FuncRandom", "set_seed(1); random(10)", "2");
interpreter_test("FuncSqrt", "sqrt(4)", "2.0");
interpreter_test("BlockFuncOrdering", "def a = fn(y) do def b = fn(x) 2 * c(x); def c = fn(x) 3 + x; b(y); end; a(12)", "30");
interpreter_test("BlockFuncOrderingGlobal", "def b = fn(x) 2 * c(x); def c = fn(x) 3 + x; b(12)", "30");
interpreter_test("EmptyListLiteral", "def f(x, y) do def r = []; append(r, x); append(r, y); return r; end; f(1, 2); f(2, 3);", "[2, 3]");
interpreter_test("NoneEmptyListLiteral", "def f(x, y) do def r = [1]; append(r, x); append(r, y); return r; end; f(1, 2); f(2, 3);", "[1, 2, 3]");
interpreter_test("FuncType", "type(4)", "'int'");
interpreter_test("FuncType", "type(4.0)", "'decimal'");
interpreter_test("FuncType", "type('a')", "'string'");
interpreter_test("FuncType", "type(//a//)", "'pattern'");
interpreter_test("FuncType", "type([1])", "'list'");
interpreter_test("FuncType", "type(<<1>>)", "'set'");
interpreter_test("FuncType", "type(<<<1 => 2>>>)", "'map'");
interpreter_test("FuncType", "type(fn(x) x)", "'func'");
interpreter_test("FuncType", "type(TRUE)", "'boolean'");
interpreter_test("FuncType", "type(date())", "'date'");
interpreter_test("ParseSimpleReturn", "parse('return x + 1')", "(add x, 1)");
interpreter_test("ParseSimpleBlockReturn", "parse('do return x + 1; end')", "(add x, 1)");
interpreter_test("ParseBlockReturn", "parse('do def x = 1; return x + 1; end')", "(block (def x = 1), (add x, 1))");
interpreter_test("ParseBlockEarlyReturn", "parse('do return x + 1; def x = 1; end')", "(block (return (add x, 1)), (def x = 1))");
interpreter_test("ParseBareBlockReturn", "parse('def x = 1; return x + 1;')", "(block (def x = 1), (add x, 1))");
interpreter_test("ParseBareBlockEarlyReturn", "parse('return x + 1; def x = 1')", "(block (return (add x, 1)), (def x = 1))");
interpreter_test("ParseLambdaReturn", "parse('def fun(x) return x + 1')", "(def fun = (lambda x, (add x, 1)))");
interpreter_test("ParseLambdaBlockReturn", "parse('def fun(x) do x = x * 2; return x + 1; end')", "(def fun = (lambda x, (block (x = (mul x, 2)), (add x, 1))))");
interpreter_test("ParseLambdaBlockEarlyReturn", "parse('def fun(x) do return x + 1; x = x * 2; end')", "(def fun = (lambda x, (block (return (add x, 1)), (x = (mul x, 2)))))");
interpreter_test("SpreadInListLiteral", "[1, ...[2, 3, 4], 5]", "[1, 2, 3, 4, 5]");
interpreter_test("SpreadInListLiteralWithIdentifier", "def a = [2, 3, 4]; [1, ...a, 5]", "[1, 2, 3, 4, 5]");
interpreter_test("SpreadInFuncallBasic", "def f(a, b, c) [a, b, c]; f(1, ...[2, 3])", "[1, 2, 3]");
interpreter_test("SpreadInFuncallMap", "def f(a, b, c) [a, b, c]; f(...<<<'c' => 3, 'a' => 1, 'b' => 2>>>)", "[1, 2, 3]");
interpreter_test("SpreadInFuncall", "def f(args...) args...; f(1, ...[2, 3, 4], 5)", "[1, 2, 3, 4, 5]");
interpreter_test("SpreadInFuncallWithIdentifier", "def f(args...) args...; def a = [2, 3, 4]; f(1, ...a, 5)", "[1, 2, 3, 4, 5]");
interpreter_test("TestOneArg", "(fn(a) a)(12)", "12");
interpreter_test("TestTwoArgs", "(fn(a, b) [a, b])(1, 2)", "[1, 2]");
interpreter_test("TestTwoArgsKeywords", "(fn(a, b) [a, b])(a = 1,  b=2)", "[1, 2]");
interpreter_test("TestTwoArgsKeywords", "(fn(a, b) [a, b])(b = 2,  a=1)", "[1, 2]");
interpreter_test("TestTwoArgsKeywords", "(fn(a, b) [a, b])(1,  b=2)", "[1, 2]");
interpreter_test("TestTwoArgsKeywords", "(fn(a, b) [a, b])(2,  a=1)", "[1, 2]");
interpreter_test("TestRestArg", "(fn(a...) a...)(1, 2)", "[1, 2]");
interpreter_test("TestMixed1", "(fn(a, b, c...) [a, b, c...])(1, 2)", "[1, 2, []]");
interpreter_test("TestMixed2", "(fn(a, b, c...) [a, b, c...])(1, 2, 3)", "[1, 2, [3]]");
interpreter_test("TestMixed3", "(fn(a, b, c...) [a, b, c...])(1, 2, 3, 4)", "[1, 2, [3, 4]]");
interpreter_test("TestMixedWithDefaults1", "(fn(a=1, b=2, c...) [a, b, c...])()", "[1, 2, []]");
interpreter_test("TestMixedWithDefaults2", "(fn(a=1, b=2, c...) [a, b, c...])(1)", "[1, 2, []]");
interpreter_test("TestMixedWithDefaults3", "(fn(a=1, b=2, c...) [a, b, c...])(1, 2)", "[1, 2, []]");
interpreter_test("TestMixedWithDefaults4", "(fn(a=1, b=2, c...) [a, b, c...])(a=11)", "[11, 2, []]");
interpreter_test("TestMixedWithDefaults5", "(fn(a=1, b=2, c...) [a, b, c...])(b=12)", "[1, 12, []]");
interpreter_test("TestMixedWithDefaults6", "(fn(a=1, b=2, c...) [a, b, c...])(1, 3, 4, b=12)", "[1, 12, [3, 4]]");
interpreter_test("DefDestructure1", "def [a, b] = [1, 2]; [a, b]", "[1, 2]");
interpreter_test("DefDestructure2", "def [a] = [1, 2]; a", "1");
interpreter_test("DefDestructure3", "def [a, b, c] = <<1, 2, 3>>; c", "3");
interpreter_test("DefDestructure4", "def [a, b, c] = [1, 2]", "NULL");
interpreter_test("AssignDestructure1", "def a = 1; def b = 1; [a, b] = [1, 2]; [a, b]", "[1, 2]");
interpreter_test("AssignDestructure2", "def a = 1; [a] = [2, 3]; a", "2");
interpreter_test("SwapUsingDestructure", "def a = 1; def b = 2; [a, b] = [b, a]; [a, b]", "[2, 1]");
interpreter_test("All", "all([2, 4, 6], fn(x) x % 2 == 0)", "TRUE");
interpreter_test("Methods1", "'abcdef'!>starts_with('abc')", "TRUE");
interpreter_test("Methods2", "' xy '!>trim()", "'xy'");
interpreter_test("Methods3", "[1, 2, 3]!>reverse()", "[3, 2, 1]");
interpreter_test("Methods4", "[2, 4, 6] !> all(fn(x) x % 2 == 0)", "TRUE");
interpreter_test("Methods5", "12 !> max(2)", "12");
interpreter_test("Methods6", "[1, 2, 3] !> reverse() !> join(sep = '-')", "'3-2-1'");
interpreter_test("WhileStringTest", "def s = '012'; while s !> starts_with('0') do s = s !> substr(1); end;", "'12'");
interpreter_test("NumConv1", "int('-5')", "-5");
interpreter_test_exception("NumConv2", "int('-5.5')");
interpreter_test("NumConv3", "int(-5.0)", "-5");
interpreter_test("NumConv4", "int(-5)", "-5");
interpreter_test("NumConv5", "decimal('-5')", "-5.0");
interpreter_test("NumConv6", "decimal('-5.5')", "-5.5");
interpreter_test("NumConv7", "decimal(-5.0)", "-5.0");
interpreter_test("NumConv8", "decimal(-5)", "-5.0");
interpreter_test("TestDoFinally1", "def a = 1; def b = 1; do a += 1; finally b += 2; end; [a, b]", "[2, 3]");
interpreter_test("TestDoFinally2", "def a = 1; def f(x) a = x + 1; def b = 1; do f(3); finally b += 2; end; [a, b]", "[4, 3]");
interpreter_test("DerefProperty", "def a = <<<'x' => 1, 'y' => 2>>>; a->y", "2");
interpreter_test("MapLiteralImplicitString", "<<<x => 1, y => 2>>>", "<<<'x' => 1, 'y' => 2>>>");
interpreter_test("PipelineLambda", "[1, 2, 3] !> (fn(lst) lst[2])()", "3");
interpreter_test("IfThenElifThenElse", "if 1 == 2 then 3 elif 1 == 3 then 4 elif 1 == 1 then 5 else 6", "5");


function collectvars_test(description, code, expected) {
    let get_freevars = function(node, environment) {
        let freeVars = [];
        let boundVars = [];
        let additionalboundVars = []; 
        node.collectVars(freeVars, boundVars, additionalboundVars);
        let result = [];
        freeVars.sort();
        for (let freeVar of freeVars) {
            if (!environment.isDefined(freeVar)) result.push(freeVar);
        }
        return JSON.stringify(result);
    };
    run_test("Collectvars:" + description, function() { return get_freevars(Parser.parseScript(code, "{test}"), Environment.getBaseEnvironment()); }, expected);
}

collectvars_test("TestVarsSimple", "abc is 0", '["abc"]');
collectvars_test("TestVarsMulti", "abc is 0 and bcd is not 0", '["abc","bcd"]');
collectvars_test("TestVarsDef", "def abc = 12; abc > 0 and bcd is not 0", '["bcd"]');
collectvars_test("TestVarsDefWithVarref", "def abc = bcd * 2; abc > 0 and bcd < 12", '["bcd"]');
collectvars_test("TestFuncDefAndUse", "def dup = fn(x) 2 * x; abc > 0 and dup(bcd) < 12", '["abc","bcd"]');
collectvars_test("TestLambdaCall", "(fn(x) 2 * x)(abc)", '["abc"]');
collectvars_test("TestLambda", "fn(x) 2 * x", '[]');
collectvars_test("TestListComprehension", "[2*x for x in y]", '["y"]');
collectvars_test("TestListComprehensionWithCondition", "[2*x for x in y if x < 12]", '["y"]');
collectvars_test("TestCascadedLambdas", "def a = fn(y) fn(x) y * x; a(abc)(2)", '["abc"]');
collectvars_test("TestLambdaOrdering", "def a = fn(y) do def b = fn(x) 2 * c(x); def c = fn(x) d * x; end", '["d"]');
collectvars_test("TestLambdaOrderingWithFreeCall", "def a = fn(y) do b(y); def b = fn(x) 2 * x; end;", '["b"]');
collectvars_test("TestLambdaOrderingGlobal", "def b = fn(x) 2 * c(x); def c = fn(x) d * x", '["d"]');
collectvars_test("TestPredefinedFunctions", "lower(a) < 'a'", '["a"]');


// extracts tests from the info associated to functions
// and runs and verifies them.

const env = Environment.getBaseEnvironment(false);
for (const symbol of env.getSymbols()) {
    const tests = [];
    const info = env.get(symbol).info;
    info.split(/[\r\n]+/).forEach(line => { 
        if (line.startsWith(":")) {
            const [test, expected] = line.substr(1).split(" ==> ");
            tests.push([test, expected]);
        }
    });
    for (let [test, expected] of tests) {
        const interpreter = new Interpreter(false);
        tests_run++;
        let result = interpreter.interpret(test, "{info-test}");
        if (!expected.startsWith("<")) expected = interpreter.interpret(expected, "{info-test-expected}");
        else expected = new ValueString(expected);
        if (expected.isString() && expected.value.startsWith("'")) expected = expected.substr(1, expected.length - 1);
        if (expected.isString() && !result.isString()) result = result.asString();
        if (!result.isEquals(expected)) {
            tests_failed++;
            addReport([
                "Test " + test + " failed:", 
                "Expected: " + expected,
                "Result__: " + result]);
        }
    }
}

/*
for symbol in ls() do
	def lines = [substr(s, 1) for s in split(info(eval(symbol)), '[\r\n]+') if str_starts_with(s, ':')];
	def tests = [label_data(['test', 'expected'], split(line, escape_pattern(' => '))) for line in lines];
	for test in tests do
	    def executor(test) do
            def result = eval(test['test']);
            def expected = test['expected'];
            if not str_starts_with(expected, '<') then do expected = eval(expected); end;
            if is_string(expected) and str_starts_with(expected, "'") then do expected = substr(expected, 1, length(expected) - 1); end;
            if is_string(expected) and not is_string(result) then do result = string(result); end;
            if result != expected then do
                println('Test ' + test['test'] + ' expected ' + expected + ' but got ' + result);
                error 'Test ' + test['test'] + ' expected ' + expected + ' but got ' + result;
            end;
	    end;
	    executor(test);
	end;
end;
*/


/* collections */

run_test("HashSet1", () => new HashSet()
    .add(new ValueInt(12))
    .add(new ValueString("abc"))
    .add(new ValueInt(12))
    .toString(), "HashSet(['abc', 12])");
run_test("HashSet2", () => {
    const s = new HashSet();
    for (let i = 0; i < 33; i++) {
        s.add(new ValueInt(i));
    }
    s.add(new ValueInt(12));
    return s.toString();
}, "HashSet([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32])");
run_test("HashSet3", () => new HashSet().add(new ValueInt(12)).has(new ValueInt(12)), true);
run_test("HashSet4", () => new HashSet().add(new ValueInt(12)).has(new ValueInt(13)), false);
run_test("HashSet5", () => new HashSet()
    .add(new ValueInt(12))
    .add(new ValueInt(13))
    .remove(new ValueInt(12))
    .has(new ValueInt(12)), false);
run_test("HashMap1", () => new HashMap()
    .set(new ValueString("abc"), new ValueInt(12))
    .set(new ValueString("abc"), new ValueInt(13))
    .set(new ValueString("def"), new ValueInt(12))
    .toString(), "HashMap(['abc' => 13, 'def' => 12])");
run_test("HashMap2", () => new HashMap().set(new ValueString("abc"), new ValueInt(12)).has(new ValueString("abc")), true);
run_test("HashMap3", () => new HashMap().set(new ValueString("abc"), new ValueInt(12)).has(new ValueInt("def")), false);
run_test("HashMap4", () => {
    const s = new HashMap();
    for (let i = 0; i < 33; i++) {
        const key = new ValueList().addItems([new ValueString("a"), new ValueInt(i)]);
        const value = new ValueInt(1000 + i);
        s.set(key, value);
    }
    s.set(new ValueList().addItem(new ValueString("a")).addItem(new ValueInt(12)), new ValueInt(-1));
    return s.toString();
}, "HashMap([['a', 0] => 1000, ['a', 1] => 1001, ['a', 2] => 1002, ['a', 3] => 1003, ['a', 4] => 1004, " + 
    "['a', 5] => 1005, ['a', 6] => 1006, ['a', 7] => 1007, ['a', 8] => 1008, ['a', 9] => 1009, ['a', 10] => 1010, " + 
    "['a', 11] => 1011, ['a', 12] => -1, ['a', 13] => 1013, ['a', 14] => 1014, ['a', 15] => 1015, ['a', 16] => 1016, " + 
    "['a', 17] => 1017, ['a', 18] => 1018, ['a', 19] => 1019, ['a', 20] => 1020, ['a', 21] => 1021, " + 
    "['a', 22] => 1022, ['a', 23] => 1023, ['a', 24] => 1024, ['a', 25] => 1025, ['a', 26] => 1026, " + 
    "['a', 27] => 1027, ['a', 28] => 1028, ['a', 29] => 1029, ['a', 30] => 1030, ['a', 31] => 1031, ['a', 32] => 1032])");
run_test("HashMap4", () => new HashMap()
    .set(new ValueString("abc"), new ValueInt(12))
    .get(new ValueString("abc")).value, 12);
run_test("HashMap5", () => new HashMap()
    .set(new ValueString("abc"), new ValueInt(12))
    .get(new ValueString("def")), null);
run_test("HashMap6", () => new HashMap()
    .set(new ValueString("abc"), new ValueInt(12))
    .set(new ValueString("def"), new ValueInt(13))
    .remove(new ValueString("abc"))
    .has(new ValueString("abc")), false);


/* varia */

run_test("isLeapYear1", () => isLeapYear(1999), false);
run_test("isLeapYear2", () => isLeapYear(1980), true);
run_test("isLeapYear3", () => isLeapYear(1900), false);
run_test("isLeapYear4", () => isLeapYear(2000), true);

run_test("convertDateToOADate1", () => convertDateToOADate(new Date(1970, 0, 1)), 25569);
run_test("convertDateToOADate2", () => convertDateToOADate(new Date(2000, 5, 1)), 36678);
run_test("convertDateToOADate3", () => convertDateToOADate(new Date(2000, 5, 10)), 36687);
run_test("convertDateToOADate4", () => convertDateToOADate(new Date(1970, 0, 1, 12)), 25569.5);
run_test("convertDateToOADate5", () => convertDateToOADate(new Date(2000, 5, 1, 12)), 36678.5);
run_test("convertDateToOADate6", () => convertDateToOADate(new Date(2000, 5, 1, 12, 48, 36)), 36678.53375);
run_test("convertDateToOADate7", () => convertDateToOADate(new Date(2000, 5, 1, 12, 48, 36, 444)), 36678.533755138895);

run_test("convertOADateToDate1", () => convertOADateToDate(25569).toString(), new Date(1970, 0, 1).toString());
run_test("convertOADateToDate2", () => convertOADateToDate(36678).toString(), new Date(2000, 5, 1).toString());
run_test("convertOADateToDate3", () => convertOADateToDate(36687).toString(), new Date(2000, 5, 10).toString());
run_test("convertOADateToDate4", () => convertOADateToDate(25569.5).toString(), new Date(1970, 0, 1, 12).toString());
run_test("convertOADateToDate5", () => convertOADateToDate(36678.5).toString(), new Date(2000, 5, 1, 12).toString());
run_test("convertOADateToDate6", () => convertOADateToDate(36678.53375).toString(), new Date(2000, 5, 1, 12, 48, 36).toString());
run_test("convertOADateToDate7", () => convertOADateToDate(36678.533755138895).toString(), new Date(2000, 5, 1, 12, 48, 36, 444).toString());

run_test("roundTripOADate", () => convertOADateToDate(convertDateToOADate(new Date(2017, 3, 5))).toString(), new Date(2017, 3, 5).toString());
run_test("roundTripOADateMinus3", () => convertOADateToDate(convertDateToOADate(new Date(2017, 3, 5)) - 3).toString(), new Date(2017, 3, 2).toString());


addLine("Total tests run: " + tests_run);
addLine("Total tests failed: " + tests_failed);
