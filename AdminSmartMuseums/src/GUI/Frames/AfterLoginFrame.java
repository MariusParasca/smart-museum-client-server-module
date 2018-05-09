package GUI.Frames;

import GUI.Panels.AfterLoginPanel;

import java.awt.*;

public class AfterLoginFrame extends Frame {


    AfterLoginPanel afterLoginPanel;

    public AfterLoginFrame() throws HeadlessException {
        super("Upload informations");
        init();
    }

    private void init(){
        appearence();

        afterLoginPanel = new AfterLoginPanel(this);
        add(afterLoginPanel, BorderLayout.CENTER);


    }}

