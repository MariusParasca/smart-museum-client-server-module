package Tests;

import ClientJava.ExhibitJSON;
import junit.framework.TestCase;

public class ExhibitJSONTest extends TestCase {

    public void testAdd() {
        ExhibitJSON exhibitJSON = new ExhibitJSON("Exponat", "D:\\");
        String path = exhibitJSON.getPath();
        assertEquals("D:\\", path);
    }
    public void testAddName() {
        ExhibitJSON exhibitJSON = new ExhibitJSON("Exponat", "D:\\");
        String name = exhibitJSON.getFileName();
        assertEquals("Exponat", name);
    }
}