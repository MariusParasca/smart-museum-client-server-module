package ClientJava;

// File Name GreetingClient.java

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.IOException;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.file.Files;

public class Client {
    private Socket socket;
    private String serverName;
    private int port;
    private DataOutputStream out;
    private DataInputStream in;

    private static Client instance = new Client();

    private Client (){ }

    public static Client getInstance(){
        return instance;
    }

    public boolean open(String serverName, int port) {
        try {
            this.serverName = serverName;
            this.port = port;
            System.out.println("Connecting to " + serverName + " on port " + port);
            socket = new Socket(serverName, port);

            out = new DataOutputStream(socket.getOutputStream());
            in = new DataInputStream(socket.getInputStream());
            if (socket.isConnected())
                return true;
        } catch (IOException e) {
            e.printStackTrace();
        }
        return false;
    }

    public boolean close() {
        try {
            socket.close();
            if(socket.isConnected())
                return true;
        } catch (IOException e) {
            e.printStackTrace();
        }
        return false;
    }

    public Socket getSocket() {
        return socket;
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

    public void sendText(String string) {

            int sum = 0;
            byte[] bytes = string.getBytes();
            send(bytes);

    }

    public void send(byte[] bytes) {
        try {
            int nrBytes = bytes.length;
            int sum = 0;
            sendInt(nrBytes);
            System.out.println(nrBytes);
            int crt = 0;
            int len = nrBytes;
            while(crt < nrBytes) {

                if(len >= 1024) {
                    byte[] pachet = new byte[1024];
                    System.arraycopy(bytes, crt, pachet, 0, 1024);
                    sendInt(1024);
                    sum = 0;
                    for(byte b : pachet) {
                        sum += b;
                    }
                    System.out.println( " ");
                    sendInt(sum);
                    out.write(pachet);
                    crt += 1024;
                    len -= 1024;
                    System.out.println("Reeee" + crt + "  " + len);
                } else {
                    byte[] pachet = new byte[len];
                    System.arraycopy(bytes, crt, pachet, 0, len);
                    sendInt(len);
                    System.out.println(len);
                    sum = 0;
                    for(byte b : pachet){
                        sum += b;
                    }
                    sendInt(sum);
                    out.write(pachet);
                    crt += len;
                    System.out.println("Reeee" + crt );
                }

            }
        } catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    public void sendZip(String path) {
        try {
            byte[] array = Files.readAllBytes(new File(path).toPath());
            System.out.println(array.length);
            send(array);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public String getServerName() {
        return serverName;
    }

    public int getPort() {
        return port;
    }

    public DataOutputStream getOut() {
        return out;
    }
}
