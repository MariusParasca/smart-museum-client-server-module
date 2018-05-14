package GUI.Frames;

import ClientJava.Client;
import GUI.Panels.LoginPanel;

import java.awt.*;

public class LoginFrame extends Frame {

   LoginPanel loginPanel;
    public LoginFrame() throws HeadlessException {
        super("Login");
        init();
    }

    private void init(){
        appearence();

        loginPanel = new LoginPanel(this);
        add(loginPanel, BorderLayout.CENTER);


        }}


