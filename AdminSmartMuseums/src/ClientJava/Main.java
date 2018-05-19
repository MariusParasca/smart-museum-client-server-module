package ClientJava;

import GUI.Frames.AdminAppFrame;

public class Main {
    public static void main(String [] args) {
        if(Client.getInstance().open("127.0.0.1", 8001)) {
            System.out.println("Just connected to " + Client.getInstance().getSocket().getRemoteSocketAddress());
            //Client.getInstance().sendText("Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().Java actually does have something just like memcpy().");
            Client.getInstance().sendZip("C:\\IP.rar");
        }
        new AdminAppFrame().setVisible(true);
        //client.close();
    }
}
