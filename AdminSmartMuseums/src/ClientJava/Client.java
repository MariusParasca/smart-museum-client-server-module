package ClientJava;

// File Name GreetingClient.java
import java.net.*;
import java.io.*;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class Client {
    private Socket socket;
    private String serverName;
    private int port;
    private DataOutputStream out;
    private DataInputStream in;

    public Client(String serverName, int port) {
        this.serverName = serverName;
        this.port = port;
    }

    public Client (){

    }
    public void open() {
        try {
            System.out.println("Connecting to " + serverName + " on port " + port);
            socket = new Socket(serverName, port);

            System.out.println("Just connected to " + socket.getRemoteSocketAddress());
            out = new DataOutputStream(socket.getOutputStream());
            in = new DataInputStream(socket.getInputStream());
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void close() {
        try {
            socket.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    //Primeste de la server o valoare de tip int si o retureneaza.
    public int receiveInt() {
        try {
            byte[] b = new byte[4];
            in.read(b);
            ByteBuffer bb = ByteBuffer.wrap(b);
            bb.order(ByteOrder.LITTLE_ENDIAN);
            return bb.getInt();
        } catch (IOException e) {
            e.printStackTrace();
        }
        return -1;
    }

    //Primim de la server un mesaj text.
    public String recieveString() {
        try{
            int nrBytes = receiveInt();
            byte[] b = new byte[nrBytes];
            in.read(b);
            String text = new String(b);
            return text;
        } catch (IOException e) {
            e.printStackTrace();
        }
        return "Mesajul nu a fost primit.";
    }

    public void sendInt(int number) {
        try {
            byte[] b = new byte[4];
            b[0] = (byte) (number & 0xFF);
            b[1] = (byte) ((number >> 8) & 0xFF);
            b[2] = (byte) ((number >> 16) & 0xFF);
            b[3] = (byte) ((number >> 24) & 0xFF);
            out.write(b);
            System.out.println("Numarul "  + number + " a fost trimis");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void sendString(String string) {
        try {
            byte[] bytes = string.getBytes();
            int nrBytes = bytes.length;
            sendInt(nrBytes);
            out.write(bytes);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void sendZip(String path) {
       /* byte bytes[] = null;
        try (FileInputStream fis = new FileInputStream(new File(path))) {
            try (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                byte[] buffer = new byte[4096000];
                int read = -1;
                while ((read = fis.read(buffer)) != -1) {
                    baos.write(buffer, 0, read);
                }
                bytes = baos.toByteArray();
                sendInt(bytes.length);
                out.write(bytes);
            }
        } catch (IOException exp) {
            exp.printStackTrace();
        }*/
       ;
    }
}
