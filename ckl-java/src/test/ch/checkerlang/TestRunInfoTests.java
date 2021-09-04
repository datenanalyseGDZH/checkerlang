package ch.checkerlang;

import ch.checkerlang.nodes.Node;
import org.junit.Assert;
import org.junit.jupiter.api.Test;

import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.nio.charset.StandardCharsets;

public class TestRunInfoTests {
    @Test
    public void testInfoTests() throws IOException {
        Node node = Parser.parse(new InputStreamReader(TestRunInfoTests.class.getResourceAsStream("/tests.txt"), StandardCharsets.UTF_8), "{res}tests.txt");
        Interpreter interpreter = new Interpreter(false, true);
        interpreter.setStandardOutput(new OutputStreamWriter(System.out));
        try {
            node.evaluate(interpreter.getBaseEnvironment()); // throws error if test fails
        } catch (ControlErrorException e) {
            System.out.println(e.getPos());
            System.out.println(e.getStacktrace());
            Assert.fail(e.getErrorValue().toString());
        }
    }

}
