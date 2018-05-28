package ClientJava;

import GUI.Frames.AdminAppFrame;

public class Main {
    public static void main(String [] args) {
        if(Client.getInstance().open("127.0.0.1", 8001)) {
            System.out.println("Just connected to " + Client.getInstance().getSocket().getRemoteSocketAddress());
            //Client.getInstance().sendText("ABCDEFGHIJKLMNOPQRSThis issue is obviously relate to message size, threre are two options you could try here:");
            // "\n" +
            //"First of all you can try to increase the corespond binding attribute elements both on client and server sides such as:");
            //Client.getInstance().receiveInt();
            //System.out.println("Textul este" + Client.getInstance().recieveText());
            //System.out.println(Client.getInstance().recieveText());
            //Client.getInstance().receiveInt();
            //Client.getInstance().sendZip("C:\\Users\\lucai\\Desktop\\AdminSmartMuseums.zip", "[set-exhibit]");
            //Client.getInstance().getExhibitList("Muzeu de test");
            //Client.getInstance().sendText("Ana are mere", "[login]");
            new AdminAppFrame().setVisible(true);
        }
    }
}
