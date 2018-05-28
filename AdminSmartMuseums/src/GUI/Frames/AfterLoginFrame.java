package GUI.Frames;

import GUI.Panels.AfterLoginPanel;

import java.awt.*;

public class AfterLoginFrame extends Frame {

    AfterLoginPanel afterLoginPanel;

    public AfterLoginFrame(String museumName) throws HeadlessException {
        super("Configure your museum");
        init();
    }

    private void init(){
        appearence();

        afterLoginPanel = new AfterLoginPanel(this);
        add(afterLoginPanel, BorderLayout.CENTER);


    }}

