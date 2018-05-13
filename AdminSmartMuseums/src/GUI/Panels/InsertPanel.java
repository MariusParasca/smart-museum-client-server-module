package GUI.Panels;

import GUI.Frames.InsertFrame;

import javax.swing.*;
import javax.swing.filechooser.FileNameExtensionFilter;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;

public class InsertPanel extends JPanel {

    private InsertFrame insertFrame;
    JLabel name = new JLabel(" Name of exhibit");
    JLabel description = new JLabel(" Short description");
    JLabel links = new JLabel(" Video links");
    JTextField nameText = new JTextField();
    JTextField descText = new JTextField();
    JTextField linkText = new JTextField();
    JButton uploadPhoto = new JButton("Upload photo");
    JButton uploadAudio = new JButton("Upload Audio");

    JButton submit = new JButton("Create exhibit");
    JPanel insert = new JPanel();
    JPanel buttons = new JPanel();

    public InsertPanel(InsertFrame insertFrame) {

        this.insertFrame = insertFrame;
        init();
    }

    protected void initButtons(JButton button) {
        button.setBorder(BorderFactory.createEmptyBorder(10, 15, 10, 15));
        button.setBackground(new Color(0, 152, 205));
        button.setForeground(Color.WHITE);
        button.setFocusPainted(false);
        button.setFont(new Font("Arial", Font.BOLD, 12));
    }


    protected void initLabel(JLabel label) {

        label.setFont(new Font("Arial", Font.LAYOUT_LEFT_TO_RIGHT, 15));

    }



    private void init() {
        this.setLayout(new BorderLayout());
        insert.setLayout(new BoxLayout(insert, BoxLayout.PAGE_AXIS));
        buttons.setLayout(new BorderLayout());
        insert.setBorder(BorderFactory.createEmptyBorder(30, 75, 50, 60));
        buttons.setBorder(BorderFactory.createEmptyBorder(30, 70, 80, 70));
        setBorder(BorderFactory.createEmptyBorder(30, 50, 50, 60));

        //setAlignmentX(Component.RIGHT_ALIGNMENT);

        nameText.setMaximumSize(new Dimension(450, 25));
        descText.setMaximumSize(new Dimension(450, 25));
        linkText.setMaximumSize(new Dimension(450, 25));


        initLabel(name);
        initLabel(description);
        initLabel(links);
        initButtons(uploadPhoto);
        initButtons(submit);
        initButtons(uploadAudio);

        insert.add(name);
        insert.add(nameText);
        insert.add(description);
        insert.add(descText);
        insert.add(links);
        insert.add(linkText);
        add(insert, BorderLayout.NORTH);
        buttons.add(uploadAudio, BorderLayout.WEST);
        buttons.add(uploadPhoto, BorderLayout.EAST);
        add(buttons, BorderLayout.CENTER);
        add(submit, BorderLayout.SOUTH);


        uploadPhoto.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {

                JFileChooser file = new JFileChooser();
                file.setCurrentDirectory(new File(System.getProperty("user.home")));
                //filter the files
                FileNameExtensionFilter filter = new FileNameExtensionFilter("*.Images", "jpg","gif","png");
                file.addChoosableFileFilter(filter);
                int result = file.showSaveDialog(null);

                if(result == JFileChooser.APPROVE_OPTION){
                    File selectedFile = file.getSelectedFile();
                    String path = selectedFile.getAbsolutePath();

                }


                else if(result == JFileChooser.CANCEL_OPTION){
                    System.out.println("No File Select");
                }
            }
        });



        uploadAudio.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {

                JFileChooser file = new JFileChooser();
                file.setCurrentDirectory(new File(System.getProperty("user.home")));
                //filter the files
                FileNameExtensionFilter filter = new FileNameExtensionFilter("*.audio", "mp3");
                file.addChoosableFileFilter(filter);
                int result = file.showSaveDialog(null);

                if(result == JFileChooser.APPROVE_OPTION){
                    File selectedFile = file.getSelectedFile();
                    String path = selectedFile.getAbsolutePath();

                }


                else if(result == JFileChooser.CANCEL_OPTION){
                    System.out.println("No File Select");
                }
            }
        });

        submit.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {

                if(nameText.getText().trim().length() != 0 &&  descText.getText().trim().length() != 0 && linkText.getText().trim().length() !=0)

                    { JOptionPane.showMessageDialog(null,"Exhibit created successfully");

                        insertFrame.setVisible(false);
                    }
                    else {
                        JOptionPane.showMessageDialog(null,"Complete all fields ");
                    }


            }
        });



    }




}
