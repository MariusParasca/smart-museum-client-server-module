package GUI.Frames;

import GUI.Panels.RegisterPanel;

import java.awt.*;

public class RegisterFrame extends Frame
{

    RegisterPanel registerPanel;

    public RegisterFrame() throws HeadlessException {
        super("Register");
        init();

    }

    private void init(){

        appearence();
        registerPanel = new RegisterPanel(this);
        add(registerPanel, BorderLayout.CENTER);

    }


}
