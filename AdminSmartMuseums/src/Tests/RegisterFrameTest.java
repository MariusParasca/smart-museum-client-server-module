package Tests;

import GUI.Frames.RegisterFrame;
import GUI.Panels.RegisterPanel;
import org.junit.Test;

import javax.swing.*;

import static junit.framework.TestCase.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class RegisterFrameTest {


    @Test
    public void testInputMuseumTextField() {

        RegisterFrame registerFrame = new RegisterFrame();
        registerFrame.setVisible(true);
        RegisterPanel registerPanel = new RegisterPanel(registerFrame);
        String expectedResults;


        JTextField inputTests = registerPanel.museumText;

        assertNotNull("cant access", inputTests);

        inputTests.setText("");



        inputTests.postActionEvent();


        expectedResults = "";
        assertEquals(expectedResults, inputTests.getText());



    }

    @Test
    public void WhenNullFieldsSubmitShouldBeFalse() {

        RegisterFrame registerFrame = new RegisterFrame();
        registerFrame.setVisible(true);
        RegisterPanel registerPanel = new RegisterPanel(registerFrame);
        String expectedResults;


        JTextField inputTests = registerPanel.museumText;
        JTextField inputTests1 = registerPanel.emailText;
        JButton submit = registerPanel.submit;

        assertNotNull("cant access", submit);

        inputTests.setText("");
        inputTests1.setText("");



        submit.doClick();



        assertEquals(false, submit.isValid());



    }

    @Test
    public void WhenInvalidEmailSumbitShouldBeFalse() {

        RegisterFrame registerFrame = new RegisterFrame();
        registerFrame.setVisible(true);
        RegisterPanel registerPanel = new RegisterPanel(registerFrame);
        String expectedResults;


        JTextField museum = registerPanel.museumText;
        JTextField email = registerPanel.emailText;
        JButton submit = registerPanel.submit;

        assertNotNull("cant access", submit);

        museum.setText("Luvru");
        email.setText("madalina.olariu@gmail.com");



        submit.doClick();


        assertEquals(false, submit.isValid());




    }

    @Test
    public void WhenValidEmailAndMuseumSubmitShouldBeTrue() {

        RegisterFrame registerFrame = new RegisterFrame();
        registerFrame.setVisible(true);
        RegisterPanel registerPanel = new RegisterPanel(registerFrame);
        String expectedResults;


        JTextField museum = registerPanel.museumText;
        JTextField email = registerPanel.emailText;
        JButton submit = registerPanel.submit;

        assertNotNull("cant access", submit);

        museum.setText("Luvru");
        email.setText("madalina.olariu@info.uaic.ro");



        submit.doClick();



        assertNotNull(submit);



    }


}