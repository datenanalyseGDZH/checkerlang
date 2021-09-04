package ch.checkerlang;

import ch.checkerlang.values.Value;
import org.junit.Assert;
import org.junit.jupiter.api.Test;

import java.io.IOException;

public class TestFuncArgs {
    @Test
    public void TestOneArg() {
        verify("12", "(fn(a) a)(12)");
    }

    @Test
    public void TestTwoArgs() {
        verify("[1, 2]", "(fn(a, b) [a, b])(1, 2)");
    }

    @Test
    public void TestTwoArgsKeywords() {
        verify("[1, 2]", "(fn(a, b) [a, b])(a = 1,  b=2)");
        verify("[1, 2]", "(fn(a, b) [a, b])(b = 2,  a=1)");
        verify("[1, 2]", "(fn(a, b) [a, b])(1,  b=2)");
        verify("[1, 2]", "(fn(a, b) [a, b])(2,  a=1)");
    }

    @Test
    public void TestRestArg() {
        verify("[1, 2]", "(fn(a...) a...)(1, 2)");
    }

    @Test
    public void TestMixed() {
        verify("[1, 2, []]", "(fn(a, b, c...) [a, b, c...])(1, 2)");
        verify("[1, 2, [3]]", "(fn(a, b, c...) [a, b, c...])(1, 2, 3)");
        verify("[1, 2, [3, 4]]", "(fn(a, b, c...) [a, b, c...])(1, 2, 3, 4)");
    }

    @Test
    public void TestMixedWithDefaults() {
        verify("[1, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])()");
        verify("[1, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])(1)");
        verify("[1, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])(1, 2)");
        verify("[11, 2, []]", "(fn(a=1, b=2, c...) [a, b, c...])(a=11)");
        verify("[1, 12, []]", "(fn(a=1, b=2, c...) [a, b, c...])(b=12)");
        verify("[1, 12, [3, 4]]", "(fn(a=1, b=2, c...) [a, b, c...])(1, 3, 4, b=12)");
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
}
