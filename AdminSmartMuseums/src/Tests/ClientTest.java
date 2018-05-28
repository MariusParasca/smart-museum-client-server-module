package Tests;

import ClientJava.Client;
import org.junit.Test;

import static org.junit.Assert.*;

public class ClientTest {

    @Test
    public void getInstanceTest() {
        assertNotNull("Clientul este null", Client.getInstance());
    }

    @Test
    public void getServerName() {
        Client.getInstance().open("127.0.0.1", 8001);
        assertEquals("127.0.0.1", Client.getInstance().getServerName());
    }

    @Test
    public void getPort() {
        Client.getInstance().open("127.0.0.1", 8001);
        assertEquals(8001, Client.getInstance().getPort());
    }

    @Test
    public void connectionRefused() {
        assertEquals(false, Client.getInstance().open("127.0.0.1", 100));
    }

    @Test
    public void connectionAccepted() {
        assertEquals(true, Client.getInstance().open("127.0.0.1", 8001));
    }
}