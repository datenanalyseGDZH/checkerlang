package ch.checkerlang;

import org.junit.Assert;
import org.junit.jupiter.api.Test;

import java.io.IOException;
import java.io.StringReader;

public class TestLexer {
    @Test
    public void TestSimple() throws IOException {
        Assert.assertEquals("[1_6_V04, nicht, leer] @ 0", new Lexer(new StringReader("1_6_V04 nicht leer"), "test").toString());
    }

    @Test
    public void TestQuotes() throws IOException {
        Assert.assertEquals("[a, double, b, single, c] @ 0",
                new Lexer(new StringReader("a \"double\" b 'single' c"), "test").toString());
    }

    @Test
    public void TestPattern() throws IOException {
        Assert.assertEquals("[//abc//] @ 0",
                new Lexer(new StringReader("//abc//"), "test").toString());
    }

    @Test
    public void TestIn() throws IOException {
        Assert.assertEquals("[feld1, in, [, a, ,, bb, ,, ccc, ]] @ 0",
                new Lexer(new StringReader("feld1 in ['a', 'bb', 'ccc']"), "test").toString());
    }

    @Test
    public void TestComplex() throws IOException {
        Assert.assertEquals(
                "[(, felda, beginnt, mit, D12, ), oder, (, feldb, <=, 1.3, *, (, feldc, -, feldd, /, 2, ), )] @ 0",
                new Lexer(new StringReader("(felda beginnt mit 'D12') oder (feldb <= 1.3*(feldc-feldd/2))"), "test").toString());
    }

    @Test
    public void TestCompare() throws IOException {
        Assert.assertEquals("[1, >, 2, d] @ 0",
                new Lexer(new StringReader("1>2 d"), "test").toString());
    }

    @Test
    public void TestNonZero() throws IOException {
        Assert.assertEquals("[non_zero, (, 12, ,, 3, )] @ 0",
                new Lexer(new StringReader("non_zero('12', '3')"), "test").toString());
    }

    @Test
    public void TestSetLiteral() throws IOException {
        Assert.assertEquals("[a, in, <<, 1, ,, 2, ,, 3, >>] @ 0",
                new Lexer(new StringReader("a in <<1, 2, 3>>"), "{test}").toString());
    }

    @Test
    public void TestMapLiteral() throws IOException  {
        Assert.assertEquals("[def, m, =, <<<, 1, =>, 100, ,, 2, =>, 200, >>>] @ 0",
                new Lexer(new StringReader("def m = <<<1 => 100, 2 => 200>>>"), "{test}").toString());
    }

    @Test
    public void TestList() throws IOException {
        Assert.assertEquals("[a, in, [, 1, ,, 2, ,, 3, ]] @ 0",
                new Lexer(new StringReader("a in [1, 2, 3]"), "test").toString());
    }

    @Test
    public void TestListAddInt() throws IOException {
        Assert.assertEquals("[[, 1, ,, 2, ,, 3, ], +, 4] @ 0",
                new Lexer(new StringReader("[1, 2, 3] + 4"), "test").toString());
    }

    @Test
    public void testSpreadOperatorIdentifier() throws IOException {
        Assert.assertEquals("[..., a] @ 0",
                new Lexer(new StringReader("...a"), "test").toString());
    }

    @Test
    public void testSpreadOperatorList() throws IOException {
        Assert.assertEquals("[..., [, 1, ,, 2, ]] @ 0",
                new Lexer(new StringReader("...[1, 2]"), "test").toString());
    }

    @Test
    public void testSpreadOperatorFuncall() throws IOException {
        Assert.assertEquals("[f, (, a, ,, ..., b, ,, c, )] @ 0",
                new Lexer(new StringReader("f(a, ...b, c)"), "test").toString());
    }

    @Test
    public void testInvokeOperator() throws IOException {
        Assert.assertEquals("[a, !>, b] @ 0",
                new Lexer(new StringReader("a!>b"), "test").toString());
    }

    @Test
    public void testStringLiteralWithNewline() throws IOException {
        Assert.assertEquals("[one\\ntwo] @ 0", new Lexer(new StringReader("'one\\ntwo'"), "test").toString());
    }

    @Test
    public void testDerefProperty() throws IOException {
        Assert.assertEquals("[a, ->, b, ->, c, ->, d] @ 0", new Lexer(new StringReader("a->b ->c -> d"), "test").toString());
    }

    @Test
    public void testIfThenElifElseKeywords() throws IOException {
        Assert.assertEquals("[if, a, then, b, elif, c, then, d, else, e] @ 0",
                new Lexer(new StringReader("if a then b elif c then d else e"), "test").toString());
    }

    @Test
    public void TestSourcePos() throws IOException {
        Lexer lexer = new Lexer(new StringReader("1 / 2\r\nx == 3"), "test");
        Token token = lexer.next();
        Assert.assertEquals("1", token.value);
        Assert.assertEquals("test:1:1", token.pos.toString());
        token = lexer.next();
        Assert.assertEquals("/", token.value);
        Assert.assertEquals("test:1:3", token.pos.toString());
        token = lexer.next();
        Assert.assertEquals("2", token.value);
        Assert.assertEquals("test:1:5", token.pos.toString());
        token = lexer.next();
        Assert.assertEquals("x", token.value);
        Assert.assertEquals("test:2:1", token.pos.toString());
        token = lexer.next();
        Assert.assertEquals("==", token.value);
        Assert.assertEquals("test:2:3", token.pos.toString());
        token = lexer.next();
        Assert.assertEquals("3", token.value);
        Assert.assertEquals("test:2:5", token.pos.toString());
    }
}
