package ch.checkerlang;

import org.junit.Assert;
import org.junit.jupiter.api.Test;

import java.io.IOException;

public class TestVaria {
    @Test
    public void testExternalEnv() throws IOException {
        Interpreter interpreter = new Interpreter();
        Environment env = Environment.getBaseEnvironment();
        interpreter.interpret("def say_hello = fn(obj) \"Hello \" + obj;", "test", env);
        Assert.assertEquals("Hello Du", interpreter.interpret("say_hello(\"Du\")", "test", env).asString().getValue());
    }

    @Test
    public void testExternalEnv2() throws IOException {
        Interpreter interpreter = new Interpreter();
        Environment env = interpreter.getBaseEnvironment();
        interpreter.interpret("def say_hello = fn(obj) \"Hello \" + obj;", "test", env);
        Assert.assertEquals("Hello Du", interpreter.interpret("say_hello(\"Du\")", "test").asString().getValue());
    }

    @Test
    public void testExternalEnvChained() throws IOException {
        Interpreter interpreter = new Interpreter();
        Environment env = Environment.getNullEnvironment();
        interpreter.interpret("def say_hello = fn(obj) \"Hello \" + obj;", "test", env);
        Environment env2 = env.newEnv();
        Assert.assertEquals("Hello Du", interpreter.interpret("say_hello(\"Du\")", "test", env2).asString().getValue());
    }
}
