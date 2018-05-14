package ClientJava;
import java.io.File;

public class CreateMuseum {
    private String path;
    public void CreateMuseum(String path, String directoryName) {
        this.path = path+directoryName+"\\";
        new File(this.path).mkdirs();
    }
}

