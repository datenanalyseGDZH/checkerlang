package ch.checkerlang;

import org.junit.Assert;
import org.junit.Test;

import java.io.IOException;
import java.io.StringReader;

public class TestParser {
    @Test
    public void testSimple() {
        Assert.assertEquals("eins",
                parse("eins"));
    }

    @Test
    public void testAddition() {
        Assert.assertEquals("(add 2, 3)",
                parse("2 + 3"));
    }

    @Test
    public void testWenn() {
        Assert.assertEquals("(if (greater 1, 2): TRUE else: FALSE)",
                parse("if 1>2 then TRUE else FALSE"));
    }

    @Test
    public void testIn() {
        Assert.assertEquals("(feld1 in ['a', 'bb', 'ccc'])",
                parse("feld1 in ['a', 'bb', 'ccc']"));
    }

    @Test
    public void TestInWithLiteralSet()
    {
        Assert.assertEquals("(feld1 in <<'a', 'bb', 'ccc'>>)",
                parse("feld1 in <<'a', 'bb', 'ccc'>>"));
    }

    @Test
    public void TestInWithLiteralMap()
    {
        Assert.assertEquals("<<<'a' => 1, 'bb' => -1, 'ccc' => 100>>>",
                parse("<<<'a' => 1, 'bb' => -1, 'ccc' => 100>>>"));
    }

    @Test
    public void testListAddInt() {
        Assert.assertEquals("(add [1, 2, 3], 4)",
                parse("[1, 2, 3] + 4"));
    }

    @Test
    public void testRelop1() {
        Assert.assertEquals("(less a, b)",
                parse("a < b"));
    }

    @Test
    public void testRelop2() {
        Assert.assertEquals("((less a, b) and (less b, c))",
                parse("a < b < c"));
    }

    @Test
    public void testRelop3() {
        Assert.assertEquals("((less_equals a, b) and (less b, c) and (equals c, d))",
                parse("a <= b < c == d"));
    }

    @Test
    public void testNonZeroFuncall() {
        Assert.assertEquals("(non_zero '12', '3')",
                parse("non_zero('12', '3')"));
    }

    @Test(expected = SyntaxError.class)
    public void testTooManyTokens() {
        parse("1 + 1 1");
    }

    @Test(expected = SyntaxError.class)
    public void testNotEnoughTokens() {
        parse("1 + ");
    }

    @Test(expected = SyntaxError.class)
    public void testMissingThen() {
        parse("if 1 < 2 else FALSE");
    }

    @Test
    public void testIfThenOrExpr() {
        Assert.assertEquals("(if (equals a, 1): ((b in c) or (equals d, 9999)) else: TRUE)",
                parse("if a == 1 then b in c or d == 9999"));
    }

    @Test(expected = SyntaxError.class)
    public void testMissingClosingParens() {
        parse("2 * (3 + 4( - 3");
    }

    @Test
    public void testLambda() {
        Assert.assertEquals("(lambda a, b=3, (mul (string a), (b 2, 3)))",
                parse("fn(a, b=3) string(a) * b(2, 3)"));
    }

    @Test
    public void testWhile() {
        Assert.assertEquals("(while (greater x, 0) do (x = (sub x, 1)))",
                parse("while x > 0 do x = x - 1; end"));
    }

    @Test
    public void testSpreadIdentifier() {
        Assert.assertEquals("(f a, ...b, c)",
                parse("f(a, ...b, c)"));
    }

    @Test
    public void testSpreadList() {
        Assert.assertEquals("(f a, ...[1, 2], c)",
                parse("f(a, ...[1, 2], c)"));
    }

    @Test
    public void testDefDestructure()
    {
        Assert.assertEquals("(def [a, b] = [1, 2])",
                parse("def [a, b] = [1, 2]"));
    }

    @Test
    public void TestAssignDestructure()
    {
        Assert.assertEquals("([a, b] = [1, 2])",
                parse("[a, b] = [1, 2]"));
    }


    private String parse(String s) {
        try {
            return Parser.parse(new Lexer(new StringReader(s), "test")).toString();
        } catch (IOException e) {
            // ignore
            return null;
        }
    }
}
