package ClientJava;

import org.json.simple.JSONObject;

import java.io.FileWriter;
import java.io.IOException;

public class ExhibitJSON {
    String fileName;
    String path;
    JSONObject obj;

    public ExhibitJSON(String fileName, String path) {
        this.fileName = fileName;
        this.path = path;
        System.out.println(path);
        obj = new JSONObject();
    }

    public  void add(String title, String description, String descriptionEn, String videoPath) {
        {
            obj.put("videoPath", videoPath);
            obj.put("descriptionEn", descriptionEn);
            obj.put("description", description);
            obj.put("title", title);
        }
    }

    public void save() {
        try{
            FileWriter file = new FileWriter(path+fileName+".json");
            file.write(obj.toJSONString());
            file.flush();

        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public String getFileName() {
        return fileName;
    }

    public String getPath() {
        return path;
    }
}