package GUI.Panels;

import ClientJava.Client;
import ClientJava.ExhibitFiles;
import ClientJava.ExhibitJSON;
import GUI.Frames.InsertFrame;

import javax.swing.*;
import javax.swing.filechooser.FileNameExtensionFilter;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;

public class InsertPanel extends JPanel {

    private InsertFrame insertFrame;
    JLabel name = new JLabel(" Exhibit name");
    JLabel description = new JLabel(" Short description in Romanian");
    JLabel descriptionEng = new JLabel(" Short description English");
    JLabel links = new JLabel(" Video links");
    JTextField nameText = new JTextField();
    JTextField descText = new JTextField();
    JTextField descEnglishText = new JTextField();
    JTextField linkText = new JTextField();
    JButton uploadPhoto = new JButton("Upload photos");
    JButton uploadAudio = new JButton("Upload audio");

    JButton submit = new JButton("Create exhibit");
    JPanel insert = new JPanel();
    JPanel buttons = new JPanel();



    ArrayList<File> photos = new ArrayList<File>();
    private File audioFile = null;
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

        nameText.setMaximumSize(new Dimension(500, 30));
        descText.setMaximumSize(new Dimension(500, 30));
        descEnglishText.setMaximumSize(new Dimension(500, 30));
        linkText.setMaximumSize(new Dimension(500, 30));


        initLabel(name);
        initLabel(description);
        initLabel(descriptionEng);
        initLabel(links);
        initButtons(uploadPhoto);
        initButtons(submit);
        initButtons(uploadAudio);

        insert.add(name);
        insert.add(nameText);
        insert.add(description);
        insert.add(descText);
        insert.add(descriptionEng);
        insert.add(descEnglishText);
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

                JFileChooser chooser = new JFileChooser();
                chooser.setCurrentDirectory(new File(System.getProperty("user.home")));

                // multi selection configure
                int mode = chooser.getFileSelectionMode();
                boolean multi = chooser.isMultiSelectionEnabled();
                chooser.setMultiSelectionEnabled( true );
                chooser.setFileSelectionMode( JFileChooser.FILES_ONLY );

                //filter the files
                FileNameExtensionFilter filter = new FileNameExtensionFilter("*.Images", "jpg","gif","png");
                chooser.addChoosableFileFilter(filter);


                int result = chooser.showSaveDialog(null);
                // selectedFiles is an array of files
                File[] selectedFiles;

                if(result == JFileChooser.APPROVE_OPTION){
                    selectedFiles = chooser.getSelectedFiles();
                    chooser.setMultiSelectionEnabled(multi);
                    chooser.setFileSelectionMode( mode );

                    for (int i=0;i<selectedFiles.length;i++) {
                        // add the file on i position to the ArrayList  photos that will be sent to the server
                        photos.add(selectedFiles[i]);
                        System.out.println(selectedFiles[i].getAbsolutePath());
                    }}

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
                    audioFile = selectedFile;
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

                    ExhibitFiles exhibitFiles = new ExhibitFiles();
                    String path = Client.getInstance().getPath();
                    exhibitFiles.createDirectory(path, nameText.getText());

                    //create json file
                    ExhibitJSON exhibitJSON = new ExhibitJSON(nameText.getText(), exhibitFiles.getPath());
                    exhibitJSON.add(nameText.getText(), descText.getText(), descText.getText(), linkText.getText());
                    exhibitJSON.save();

                    exhibitFiles.addImages(photos, exhibitFiles.getPath());

                    if(audioFile != null) {
                        try {
                            exhibitFiles.addAudio(audioFile, exhibitFiles.getPath());
                        } catch (IOException ex) {
                            ex.printStackTrace();
                        }
                    }


                    System.out.println(path+"\\" + nameText.getText());
                    String OUTPUT_ZIP_FILE = path+"\\"+ nameText.getText() +".zip";
                    String SOURCE_FOLDER = path+"\\" + nameText.getText();  // SourceFolder path

                    com.company.ZipUtils appZip = new com.company.ZipUtils(SOURCE_FOLDER, OUTPUT_ZIP_FILE);
                    File file = new File(SOURCE_FOLDER);
                    appZip.generateFileList(file);
                    appZip.zipIt(OUTPUT_ZIP_FILE);
                    Client.getInstance().open("127.0.0.1", 8001);
                    Client.getInstance().sendZip(OUTPUT_ZIP_FILE, "[set-exhibit]");
                    Client.getInstance().close();
                    insertFrame.setVisible(false);

                }
                else {
                    JOptionPane.showMessageDialog(null,"Complete all fields ");
                }


            }
        });



    }




}
