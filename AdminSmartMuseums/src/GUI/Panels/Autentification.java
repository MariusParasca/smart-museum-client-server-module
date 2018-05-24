package GUI.Panels;

import GUI.Frames.AdminAppFrame;
import GUI.Frames.LoginFrame;
import GUI.Frames.RegisterFrame;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class Autentification extends JPanel{

    private final AdminAppFrame adminAppFrame;
    JButton registerButton = new JButton("Register");
    JButton loginButton = new JButton("Login");
    JLabel welcomeMessage= new JLabel(" Welcome to the app where you can configure your museum exhibits");
    JPanel buttonPanel = new JPanel();


    public Autentification(AdminAppFrame adminAppFrame) {
        this.adminAppFrame = adminAppFrame;
        init();
    }

    private void initButtons(JButton button){
        button.setBorder(BorderFactory.createEmptyBorder(10, 15, 10, 15));
        button.setBackground(new Color(0, 152, 205));
        button.setForeground(Color.WHITE);
        button.setFocusPainted(false);
        button.setFont(new Font("Arial", Font.BOLD, 15));
    }

    private void initLabel(JLabel label){
        label.setHorizontalAlignment(SwingConstants.CENTER);
        label.setFont(new Font("Arial", Font.BOLD, 15));

    }
    private void init(){


        this.setLayout(new BorderLayout());
        setBorder(BorderFactory.createEmptyBorder(70, 50, 10, 60));
        buttonPanel.setBorder(BorderFactory.createEmptyBorder(70, 50, 10, 60));

        initButtons( registerButton);
        initButtons(loginButton);
        initLabel(welcomeMessage);

        add(welcomeMessage, BorderLayout.NORTH);
        buttonPanel.add(registerButton, BorderLayout.CENTER);
        buttonPanel.add(loginButton, BorderLayout.CENTER);
        add(buttonPanel, BorderLayout.CENTER);

        registerButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {

                RegisterFrame registerFrame=new RegisterFrame();
                registerFrame.setVisible(true);
                registerFrame.setSize(800, 500);
                adminAppFrame.setVisible(false);
            }

        });

        loginButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                LoginFrame loginFrame=new LoginFrame();
                loginFrame.setVisible(true);
                adminAppFrame.setVisible(false);
            }

        });

    }
}