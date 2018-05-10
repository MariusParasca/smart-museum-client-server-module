package GUI.Frames;

import ClientJava.Client;

import GUI.Panels.Autentification;


import java.awt.*;

public class AdminAppFrame extends Frame{

    Autentification autentification;
    public Client client;

    public AdminAppFrame(Client client) throws HeadlessException {
        super("Administrator Museum App");
        this.client = client;
        init();

    }

    private void init(){

        appearence();

        autentification = new Autentification(this, client);
        add(autentification, BorderLayout.CENTER);
    }


}
