package ClientJava;

import GUI.Frames.AdminAppFrame;
import GUI.Frames.AdminAppFrame;

import java.io.*;
import java.net.Socket;
import java.util.Scanner;

public class Main {
    public static void main(String [] args) {
        Client client = new Client("127.0.0.1", 8001);
        //client.open();
        /*System.out.println("Numarul primt de la server este: " + client.receiveInt());
        System.out.println("Textul primit de la server este: " + client.recieveString());
        client.sendInt(10);
        client.sendString("Test ana are mere");
        client.sendZip("D:\\Client_Server\\meme.zip");
        */

//        Scanner keyboard = new Scanner(System.in);
//        boolean running = true;
//        while(running == true){
//            System.out.println("Comanda:");
//            //1 - Logare
//            //2 - Inregistrare
//            int command = keyboard.nextInt();
//            if(command == 1)
//            {
//                client.sendInt(command);
//                client.sendString("Nume");
//                client.sendString("Prenume");
//                System.out.println(client.recieveString());
//            }
//
//            if(command == 2){
//                client.sendInt(command);
//                client.sendString("Nume");
//                client.sendString("Prenume");
//                client.sendInt(22);
//                System.out.println(client.recieveString());
//            }
//
//            if(command == 3){
//                client.sendInt(command);
//                running = false;
//            }
//
//            if(running == false)
//                client.close();
//        }
        new AdminAppFrame().setVisible(true);
    }
}
