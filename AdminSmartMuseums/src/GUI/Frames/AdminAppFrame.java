package GUI.Frames;

import Database.DatabaseAdmin;
import GUI.Panels.Autentification;


import java.awt.*;

public class AdminAppFrame extends Frame{

    public static DatabaseAdmin adminApp;
    Autentification autentification;

    public AdminAppFrame() throws HeadlessException {
        super("Administrator Museum App");
        init();

    }

    private void init(){

        appearence();
        adminApp= new DatabaseAdmin();
        autentification = new Autentification(this);
        add(autentification, BorderLayout.CENTER);
    }


}
