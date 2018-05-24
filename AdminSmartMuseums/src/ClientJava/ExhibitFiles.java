package ClientJava;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.*;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

public class ExhibitFiles {
    private String path;
    private String pathImg;
    public void createDirectory(String path, String directoryName) {
        this.path = path+"\\"+directoryName+"\\";
        this.pathImg = this.path+"\\img";
        new File(this.path).mkdirs();
        new File(this.pathImg).mkdirs();
    }

    public static void addToZipFile(String filePath) throws FileNotFoundException, IOException {
        try {
            File file = new File(filePath);
            FileOutputStream fos = new FileOutputStream(filePath + ".zip");
            ZipOutputStream zos = new ZipOutputStream(fos);

            zos.putNextEntry(new ZipEntry(file.getName()));

            //byte[] bytes = Files.readAllBytes(Paths.get("./exhibits/" + filePath));
            byte[] bytes = new byte[111111];
            int len;
            FileInputStream in = new FileInputStream(filePath);
            while ((len = in.read(bytes)) > 0) {
                zos.write(bytes, 0, len);
            }
            zos.close();
        } catch (IOException ex) {
            ex.printStackTrace();
        }
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
            byte[] buffer = new byte[10240000];
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
