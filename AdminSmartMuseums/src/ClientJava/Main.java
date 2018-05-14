package ClientJava;

import GUI.Frames.AdminAppFrame;
import GUI.Frames.AdminAppFrame;

import java.io.*;
import java.net.Socket;
import java.util.Scanner;

public class Main {
    public static void main(String [] args) {
        Client.getInstance().open("127.0.0.1", 8001);
        new AdminAppFrame().setVisible(true);
        //client.close();
    }
}
