package Models;

import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;
import java.net.Socket;

public class Login  {

    private String username;
    private String password;
    private String text;

    private final static String SERVER_ADDRESS = "127.0.0.1";
    private final static int PORT = 8100;
    private static OutputStream out;
    private static PrintStream ps;
    private static Socket socket;
    public Login(String username, String password) throws IOException {
        this.username = username;
        this.password = password;

        text = "[login-admin]"+ "<"+username+">"+"<"+password+">";
        // send(out, text);
       //Client.getInstance().sendText(text);

    }


}
