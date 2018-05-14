package Tests;

import ClientJava.ExhibitFiles;
import org.junit.Test;

import static org.junit.Assert.*;

public class ExhibitFilesTest {

    @Test
    public void createDirectoryPathTest() {
        ExhibitFiles exhibitFiles = new ExhibitFiles();
        exhibitFiles.createDirectory("C:\\Test", "Test2");
        String path = exhibitFiles.getPath();
        assertEquals("C:\\Test\\Test2\\", path);
    }

}