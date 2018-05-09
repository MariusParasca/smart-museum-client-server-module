package GUI.Frames;

import GUI.Panels.InsertPanel;

import javax.swing.*;
import java.awt.*;

public class InsertFrame extends JFrame {

    private InsertPanel insertPanel;

    public InsertFrame() throws HeadlessException {
        super("Insert exhibit");
        init();

    }

    private void init(){

        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setSize(new Dimension(500, 500));
        setLayout(new BorderLayout());
        insertPanel = new InsertPanel(this);
        add(insertPanel, BorderLayout.CENTER);


    }
}
