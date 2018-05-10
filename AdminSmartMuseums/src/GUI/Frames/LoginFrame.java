package GUI.Frames;

import ClientJava.Client;
import GUI.Panels.LoginPanel;

import java.awt.*;

public class LoginFrame extends Frame {

   LoginPanel loginPanel;
    private Client client;
    public LoginFrame(Client client) throws HeadlessException {
        super("Login");
        this.client = client;
        init();
    }

    private void init(){
        appearence();

        loginPanel = new LoginPanel(this, client);
        add(loginPanel, BorderLayout.CENTER);


        }}


