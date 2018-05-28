package GUI.Panels;

import ClientJava.Client;
import GUI.Frames.AfterLoginFrame;
import GUI.Frames.InsertFrame;
import Models.Login;

import javax.swing.*;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;
import javax.swing.table.DefaultTableModel;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class AfterLoginPanel extends JPanel {

    private Login login;

    private AfterLoginFrame afterLoginFrame;

    private String museumName = "Muzeu de test";
    JButton insertButton = new JButton("Insert ");
    JButton deleteButton = new JButton("Delete");
    JPanel buttonPanel = new JPanel();


    //
    Object rowData[][] = new Object[27][1];
    Object columnNames[] = { "Exhibit"};
    DefaultTableModel tableModel = new DefaultTableModel(rowData, columnNames);
    JTable table = new JTable(tableModel);
    //


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


    public void tableShow(){

        JFrame frame = new JFrame();

        Client.getInstance().open("127.0.0.1", 8001);
        //String museumName = Client.getInstance().getMuseumName();
        String exhibitList [] = Client.getInstance().getExhibitList(museumName);
        int row =0;

        // aici ar trebui sa se populeze tabelul cu exponatele din muzeu
        for ( int exhibit =0; exhibit < exhibitList.length;exhibit ++) {

            tableModel.insertRow( exhibit, new String[]{exhibitList[exhibit]});

        }
        //tableModel.insertRow(1, new String[]{"bla"});
        JScrollPane scrollPane = new JScrollPane(table);
        frame.add(scrollPane, BorderLayout.CENTER);
        frame.setSize(400,500);
        frame.setVisible(true);

    }
    private  void init(){

        this.setLayout(new BorderLayout());

        setBorder(BorderFactory.createEmptyBorder(100, 50, 10, 60));
        buttonPanel.setBorder(BorderFactory.createEmptyBorder(70, 50, 10, 60));

        initButtons( insertButton);

        initButtons(deleteButton);

        buttonPanel.add(insertButton, BorderLayout.WEST);

        buttonPanel.add(deleteButton, BorderLayout.EAST);
        add(buttonPanel, BorderLayout.CENTER);
        insertButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {

                InsertFrame insertFrame=new InsertFrame();
                insertFrame.setVisible(true);

            }

        });






        //
        deleteButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                tableShow();
            }

        });


        table.getSelectionModel().addListSelectionListener(new ListSelectionListener() {
            @Override
            public void valueChanged(ListSelectionEvent event) {
                int  index = table.getSelectedRow();
                if (table.getSelectedRow() > -1) {
                    int input = JOptionPane.showConfirmDialog(null, "Do you want to delete this exhibit?");
                    if(input == 0) {

                        // request la server sa se stearga exponatul cu numele getSelectedRow().get
                        tableModel.removeRow(table.getSelectedRow());
                        Client.getInstance().sendText(table.getModel().getValueAt(index, 0).toString(), "[get-exhibit-list]");
                        //System.out.println(table.getModel().getValueAt(index, 0).toString());
                    }
                    else { }

                }
            }
        });
    }

}
