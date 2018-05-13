package GUI.Frames;

import ClientJava.Client;

import javax.swing.*;
import java.awt.*;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;

public class Frame extends JFrame {

    public Frame(String title) throws HeadlessException {
        super(title);
    }


    protected void appearence() {

        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setSize(new Dimension(800, 500));
        setLayout(new BorderLayout());

        addWindowListener(new WindowAdapter()
        {
            @Override
            public void windowClosing(WindowEvent e)
            {
                Client.getInstance().close();
                System.out.println("Socket closed");
                e.getWindow().dispose();
            }
        });
    }
}



