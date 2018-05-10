package GUI.Panels;

import ClientJava.Client;
import GUI.Frames.AfterLoginFrame;
import GUI.Frames.LoginFrame;
import Models.Login;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionListener;
import java.io.IOException;

public class LoginPanel extends JPanel {

    private LoginFrame loginframe;
    private Client client;
    JLabel username = new JLabel(" Username");
    JLabel password = new JLabel(" Password");
    JLabel info = new JLabel("");
    JTextField usernameText = new JTextField();
    JTextField passText = new JTextField();
    JButton login = new JButton("Login");


    public LoginPanel(LoginFrame loginFrame, Client client) {
        this.client = client;
        this.loginframe = loginframe;
        init();
    }

    protected void initButtons(JButton button){
        button.setBorder(BorderFactory.createEmptyBorder(10, 15, 10, 15));
        button.setBackground(new Color(0, 152, 205));
        button.setForeground(Color.WHITE);
        button.setFocusPainted(false);
        button.setFont(new Font("Arial", Font.BOLD, 12));
    }


    protected void initLabel(JLabel label){

        label.setFont(new Font("Arial", Font.LAYOUT_LEFT_TO_RIGHT ,15));

    }




    private void init(){


        //framePanel.setLayout(new BorderLayout());
        setLayout(new BoxLayout(this, BoxLayout.PAGE_AXIS));
        setBorder(BorderFactory.createEmptyBorder(100, 280, 10, 60));
        //setAlignmentX(Component.RIGHT_ALIGNMENT);

        usernameText.setMaximumSize(new Dimension(450,25));
        passText.setMaximumSize(new Dimension(450,25));


        initLabel(username);
        initLabel(password);
        initButtons(login);

        add(username);
        add(usernameText);
        add(password);
        add(passText);

        add(login);


        login.addActionListener(new ActionListener() {

            public void actionPerformed(java.awt.event.ActionEvent e) {
                try {
                    Login log = new Login(usernameText.getText(), passText.getText(), client);
                } catch (IOException e1) {
                    e1.printStackTrace();
                }

                AfterLoginFrame afterLoginFrame=new AfterLoginFrame();
                afterLoginFrame.setVisible(true);
                loginframe.setVisible(false);

            }

        });




    }
    }


