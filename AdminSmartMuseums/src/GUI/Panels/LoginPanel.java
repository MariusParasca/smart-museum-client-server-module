package GUI.Panels;

import ClientJava.Client;
import GUI.Frames.AfterLoginFrame;
import GUI.Frames.LoginFrame;
import Models.Login;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionListener;
import java.io.IOException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class LoginPanel extends JPanel {

    private LoginFrame loginframe;
    private Client client;
    JLabel username = new JLabel(" Username");
    JLabel password = new JLabel(" Password");
    JLabel info = new JLabel("");
    JTextField usernameText = new JTextField();
    JTextField passText = new JTextField();
    JButton login = new JButton("Login");



    public static final Pattern VALID_EMAIL_ADDRESS_REGEX =
            Pattern.compile("^[A-Z0-9._%+-]+@[A-Z0-9.-]", Pattern.CASE_INSENSITIVE);

    public static boolean validate(String emailStr) {
        Matcher matcher = VALID_EMAIL_ADDRESS_REGEX .matcher(emailStr);
        return matcher.find();
    }


    public LoginPanel(LoginFrame loginFrame) {
        this.loginframe = loginFrame;
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


                if( usernameText.getText().length() == 0 || passText.getText().length() == 0 ){
                    JOptionPane.showMessageDialog(null,"Please complete all fields");
                }
                else
                if(!validate(usernameText.getText())) {
                    JOptionPane.showMessageDialog(null,"Insert a valid email");

                }
                  else
            {
                try {


                       Login log = new Login(usernameText.getText(), passText.getText());
                   } catch (IOException e1) {
                       e1.printStackTrace();
                    }

                    AfterLoginFrame afterLoginFrame = new AfterLoginFrame();
                    afterLoginFrame.setVisible(true);
                    loginframe.setVisible(false);
                }
            }

        });

        }
    }


