package GUI.Panels;

import GUI.Frames.AfterLoginFrame;
import GUI.Frames.InsertFrame;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class AfterLoginPanel extends JPanel {

    private AfterLoginFrame afterLoginFrame;

    JButton insertButton = new JButton("Insert ");
    JButton updateButton = new JButton("Update");
    JButton deleteButton = new JButton("Delete");
    JPanel buttonPanel = new JPanel();
    public AfterLoginPanel(AfterLoginFrame afterloginFrame) {

        this.afterLoginFrame = afterloginFrame;
        init();
    }

    private void initButtons(JButton button){
        button.setBorder(BorderFactory.createEmptyBorder(10, 15, 10, 15));
        button.setBackground(new Color(0, 152, 205));
        button.setForeground(Color.WHITE);
        button.setFocusPainted(false);
        button.setFont(new Font("Arial", Font.BOLD, 15));
    }


   private  void init(){

       this.setLayout(new BorderLayout());

       setBorder(BorderFactory.createEmptyBorder(100, 50, 10, 60));
       buttonPanel.setBorder(BorderFactory.createEmptyBorder(70, 50, 10, 60));

       initButtons( insertButton);
       initButtons(updateButton);
       initButtons(deleteButton);

       buttonPanel.add(insertButton, BorderLayout.WEST);
       buttonPanel.add(updateButton, BorderLayout.CENTER);
       buttonPanel.add(deleteButton, BorderLayout.EAST);
       add(buttonPanel, BorderLayout.CENTER);
       insertButton.addActionListener(new ActionListener() {

           public void actionPerformed(ActionEvent e) {

               InsertFrame insertFrame=new InsertFrame();
               insertFrame.setVisible(true);

           }

       });
//       updateButton.addActionListener(new ActionListener() {
//
//           public void actionPerformed(ActionEvent e) {
//
//               UpdateFrame insertFrame=new UpdateFrame();
//               updateFrame.setVisible(true);
//
//           }
//
//       });
//
//       deleteButton.addActionListener(new ActionListener() {
//
//           public void actionPerformed(ActionEvent e) {
//
//               DeleteFrame deleteFrame=new DeleteFrame();
//               deleteFrame.setVisible(true);
//
//           }
//
//       });
   }

}
