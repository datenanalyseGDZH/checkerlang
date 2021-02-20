package ch.checkerlang;

import ch.checkerlang.nodes.Node;
import org.junit.Assert;
import org.junit.Test;

import java.io.IOException;
import java.io.StringReader;
import java.util.Collection;
import java.util.Set;
import java.util.TreeSet;

public class TestCollectVars {
    @Test
    public void TestVarsSimple() {
        verify("{abc}", "abc is 0");
    }

    @Test
    public void TestVarsMulti() {
        verify("{abc, bcd}", "abc is 0 and bcd is not 0");
    }

    @Test
    public void TestVarsDef() {
        verify("{bcd}", "def abc = 12; abc > 0 and bcd is not 0");
    }

    @Test
    public void TestVarsDefWithVarref() {
        verify("{bcd}", "def abc = bcd * 2; abc > 0 and bcd < 12");
    }

    @Test
    public void TestFuncDefAndUse() {
        verify("{abc, bcd}", "def dup = fn(x) 2 * x; abc > 0 and dup(bcd) < 12");
    }

    @Test
    public void TestLambdaCall() {
        verify("{abc}", "(fn(x) 2 * x)(abc)");
    }

    @Test
    public void TestLambda() {
        verify("{}", "fn(x) 2 * x");
    }

    @Test
    public void TestListComprehension() {
        verify("{y}", "[2*x for x in y]");
    }

    @Test
    public void TestListComprehensionWithCondition() {
        verify("{y}", "[2*x for x in y if x < 12]");
    }

    @Test
    public void TestCascadedLambdas() {
        verify("{abc}", "def a = fn(y) fn(x) y * x; a(abc)(2)");
    }

    @Test
    public void TestLambdaOrdering() {
        verify("{d}", "def a = fn(y) do def b = fn(x) 2 * c(x); def c = fn(x) d * x; end");
    }

    @Test
    public void TestLambdaOrderingWithFreeCall() {
        verify("{b}", "def a = fn(y) do b(y); def b = fn(x) 2 * x; end;");
    }

    @Test
    public void TestLambdaOrderingGlobal() {
        verify("{d}", "def b = fn(x) 2 * c(x); def c = fn(x) d * x");
    }

    @Test
    public void TestPredefinedFunctions() throws IOException {
        Node node = Parser.parse(new Lexer(new StringReader("lower(a) < 'a'"), "test"));
        String result = SetToString(FreeVars.get(node, new Interpreter().getBaseEnvironment()));
        Assert.assertEquals("{a}", result);
    }

    private void verify(String free, String script) {
        try {
            Node node = Parser.parse(new Lexer(new StringReader(script), "test"));
            Set<String> freeVars = new TreeSet<>();
            Set<String> boundVars = new TreeSet<>();
            Set<String> additionalBoundVars = new TreeSet<>();
            node.collectVars(freeVars, boundVars, additionalBoundVars);
            Assert.assertEquals(free, SetToString(FreeVars.get(node, Environment.getBaseEnvironment())));
        } catch (IOException e) {
            e.printStackTrace();
            Assert.fail();
        }
    }

    private String SetToString(Collection<String> set) {
        StringBuilder result = new StringBuilder();
        result.append("{");
        for (String s : set) {
            result.append(s).append(", ");
        }
        if (result.length() > 1) result.setLength(result.length() - 2);
        result.append("}");
        return result.toString();
    }

    @Test
    public void TestFreeVars() throws IOException {
        Interpreter interpreter = new Interpreter();
        Environment env = Environment.getBaseEnvironment();
        Node parseTree1 = Parser.parse(new Lexer(new StringReader("def say_hello = fn(obj) \"Hello \" + obj;"), "test"));
        interpreter.interpret(parseTree1, env);

        Node parseTree2 = Parser.parse(new Lexer(new StringReader("say_hello(\"du\") + unknown_func()"), "test"));
        Set<String> freeVars = FreeVars.get(parseTree2, env);
        Assert.assertTrue(freeVars.contains("unknown_func"));
        Assert.assertFalse(freeVars.contains("say_hello"));
    }
}
