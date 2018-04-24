import com.jcraft.jsch.JSchException;
import com.jcraft.jsch.SftpException;

public class Main {

    public static void main(String argv[]) throws SftpException, JSchException {
        Upload upload = new Upload();
        upload.start();
        upload.file("jsch-0.1.54.jar","/home/smartmuseum/museums");
        upload.stop();
    }

}
