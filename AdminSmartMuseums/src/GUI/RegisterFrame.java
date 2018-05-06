package GUI;

import Models.Register;

import java.awt.*;

public class RegisterFrame extends Frame
{

    RegisterPanel registerPanel;

    public RegisterFrame() throws HeadlessException {
        super("bla");
        init();

    }

    private void init(){

        appearence();
        registerPanel = new RegisterPanel(this);
        add(registerPanel, BorderLayout.CENTER);

    }


}
