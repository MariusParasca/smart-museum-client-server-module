package GUI.Panels;

import ClientJava.Client;
import GUI.Frames.LoginFrame;
import GUI.Frames.RegisterFrame;
import Models.Register;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class RegisterPanel  extends JPanel {

    private final RegisterFrame registerFrame;
    JLabel museumName = new JLabel(" Add your museum name");
    JLabel emailAddress = new JLabel(" Add your email Address");
    JLabel info = new JLabel("");
    public JTextField museumText = new JTextField();
    public JTextField emailText = new JTextField();
    public JButton submit = new JButton("Submit");
    private Client client;
    public JOptionPane message = new JOptionPane();


    public static final Pattern VALID_EMAIL_ADDRESS_REGEX =
            Pattern.compile("^[A-Z0-9._%+-]+@[A-Z0-9.-]", Pattern.CASE_INSENSITIVE);

    public static boolean validate(String emailStr) {
        Matcher matcher = VALID_EMAIL_ADDRESS_REGEX .matcher(emailStr);
        return matcher.find();
    }
    public RegisterPanel(RegisterFrame registerFrame) {
        this.registerFrame = registerFrame;
        //client = new Client();
        init();
    }

    private void initButtons(JButton button){
        button.setBorder(BorderFactory.createEmptyBorder(10, 15, 10, 15));
        button.setBackground(new Color(0, 152, 205));
        button.setForeground(Color.WHITE);
        button.setFocusPainted(false);
        button.setFont(new Font("Arial", Font.BOLD, 12));
    }


    private void initLabel(JLabel label){

        label.setFont(new Font("Arial", Font.LAYOUT_LEFT_TO_RIGHT ,15));

    }





    private void init(){

        // this.setLayout(new BorderLayout());
        setLayout(new BoxLayout(this, BoxLayout.PAGE_AXIS));
        setBorder(BorderFactory.createEmptyBorder(100, 280, 10, 60));
        //setAlignmentX(Component.RIGHT_ALIGNMENT);

        museumText.setMaximumSize(new Dimension(450,25));
        emailText.setMaximumSize(new Dimension(450,25));

        info.setFont(new Font("Arial", Font.LAYOUT_LEFT_TO_RIGHT ,10));
        initLabel(museumName);
        initLabel(emailAddress);
        initButtons(submit);

        add(museumName);
        add(museumText);
        add(emailAddress);
        add(emailText);
        add(info);
        add(submit);

        submit.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {

                if(museumText.getText().trim().length() != 0 &&  emailText.getText().trim().length() != 0 )
                    if(validate(emailText.getText()))
                    {
                        Register reg=new Register(emailText.getText(), museumText.getText(), "Smart Museums ");
//                System.out.print("Email sent");
                        JFrame loginFrame=new LoginFrame();
                        loginFrame.setVisible(true);
                        registerFrame.setVisible(false);
                        message.showMessageDialog(null,"Account setup was successful");
                    }
                    else {
                        message.showMessageDialog(null,"Enter a valid email");
                    }
                else
                {
                    message.showMessageDialog(null,"Please complete all fields");
                }

            }
        });




    }

}