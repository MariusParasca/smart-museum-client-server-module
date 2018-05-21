package ClientJava;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.*;
import java.util.ArrayList;

public class ExhibitFiles {
    private String path;
    private String pathImg;
    public void createDirectory(String path, String directoryName) {
        this.path = path+"\\"+directoryName+"\\";
        this.pathImg = this.path+"\\img";
        new File(this.path).mkdirs();
        new File(this.pathImg).mkdirs();
    }

    public void addImages(ArrayList<File> photos, String path) {
        try{
            for(File photo : photos) {
                BufferedImage buffer = ImageIO.read(photo);
                String fileName = photo.getName();
                File outputfile = new File(pathImg+"\\"+fileName);
                System.out.println(path+fileName);
                ImageIO.write(buffer, "jpeg", outputfile);
            }
        } catch (IOException ex) {
            ex.printStackTrace();
        }
    }

    public void addAudio(File audioFile, String dest) throws IOException {
        InputStream is = null;
        OutputStream os = null;
        try {
            String audioPath = audioFile.getAbsolutePath();
            String audioName = audioFile.getName();
            is = new FileInputStream(audioPath);
            System.out.println(audioPath);

            os = new FileOutputStream(dest+audioName+".mp3");
            byte[] buffer = new byte[102400];
            int length;
            while ((length = is.read(buffer)) > 0) {
                os.write(buffer, 0, length);
            }
        }catch (IOException ex) {
            ex.printStackTrace();
        } finally {
            is.close();
            os.close();
        }
    }

    public String getPath() {
        return path;
    }

    public String getPathImg() {
        return pathImg;
    }
}
