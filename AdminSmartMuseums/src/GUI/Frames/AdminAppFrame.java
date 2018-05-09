package GUI.Frames;

import ClientJava.Client;

import GUI.Panels.Autentification;


import java.awt.*;

public class AdminAppFrame extends Frame{

    public static Client adminApp;
    Autentification autentification;

    public AdminAppFrame() throws HeadlessException {
        super("Administrator Museum App");
        init();

    }

    private void init(){

        appearence();
        adminApp= new Client();
        autentification = new Autentification(this);
        add(autentification, BorderLayout.CENTER);
    }


}
