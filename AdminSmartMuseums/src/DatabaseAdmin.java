import java.sql.*;

public class DatabaseAdmin {
    private Connection conn = null;

    public void open() {
        try {
            Class.forName("com.mysql.jdbc.Driver");
        }
        catch(ClassNotFoundException ex) {
            System.out.println("Error: unable to load driver class!");
            System.exit(1);
        }

        try {
            System.out.println("Connecting to database...");
            conn = DriverManager.getConnection("jdbc:mysql://smartmuseumdb.cye3478n2lhi.eu-west-2.rds.amazonaws.com:3306/", "masterUser", "SmartMuseum4Secret");
            System.out.println("Connected database successfully...");
        }catch(SQLException se){
            System.out.println("Error: connection error!");
            se.printStackTrace();
        }
    }

    public void close() {
        try{
            if(conn!=null)
                conn.close();
        }catch(SQLException se){
            se.printStackTrace();
        }
        System.out.println("Disconnected!");
    }

    public void insertIntoMuseums(String museumName, float latitude, float longitude, float radius, String path) {
        Statement stmt = null;

        try {
            System.out.println("Inserting records into the table...");
            stmt = conn.createStatement();
            String sql = "INSERT INTO SmartMuseumDB.Museums (museumName, latitude, longitude, radius, path)"
                    + "VALUES ('" + museumName + "', " + latitude + ", " + longitude + ", " + radius + ", '" + path + "')";
            stmt.executeUpdate(sql);
            System.out.println("Inserted records into the table...");
        }catch(SQLException se){
            System.out.println("Error: sql error!");
            se.printStackTrace();
        }

        try{
            if(stmt!=null)
                stmt.close();
        }catch(SQLException se){
            se.printStackTrace();
        }
    }

    public void insertIntoExhibits(int id, int idMuseum, String name, String author, String description, String path) {
        Statement stmt = null;

        try {
            System.out.println("Inserting records into the table...");
            stmt = conn.createStatement();
            String sql = "INSERT INTO SmartMuseumDB.Exhibits (id, idMuseum, name, author, description, path)"
                    + "VALUES (" + id + ", " + idMuseum + ", '" + name + "', '" + author + "', '" + description + "', '" + path + "') ";
            stmt.executeUpdate(sql);
            System.out.println("Inserted records into the table...");
        }catch(SQLException se){
            System.out.println("Error: sql error!");
            se.printStackTrace();
        }

        try{
            if(stmt!=null)
                stmt.close();
        }catch(SQLException se){
            se.printStackTrace();
        }
    }

    public void getMuseumsContent() {
        Statement stmt = null;
        try {
            System.out.println("Selecting from Museums...");
            stmt = conn.createStatement();
            ResultSet rs = stmt.executeQuery("SELECT id, museumName, latitude, longitude, radius, path FROM SmartMuseumDB.Museums");
            while(rs.next()){
                int id = rs.getInt("id");
                String museumName = rs.getString("museumName");
                float latitude = rs.getFloat("latitude");
                float longitude = rs.getFloat("longitude");
                float radius = rs.getFloat("radius");
                String path = rs.getString("path");

                System.out.println(id + ", "+ museumName + ", " + latitude + ", " + longitude + ", " + radius + ", " + path);
            }
            rs.close();
        }catch(SQLException se){
            System.out.println("Error: sql error!");
            se.printStackTrace();
        }
    }

    public void getExhibitsContent() {
        Statement stmt = null;
        try {
            System.out.println("Selecting from Exhibits...");
            stmt = conn.createStatement();
            ResultSet rs = stmt.executeQuery("SELECT id, idMuseum, name, author, description, path FROM SmartMuseumDB.Exhibits");
            while(rs.next()){
                int id = rs.getInt("id");
                int idMuseum = rs.getInt("idMuseum");
                String name = rs.getString("name");
                String author = rs.getString("author");
                String description = rs.getString("description");
                String path = rs.getString("path");

                System.out.println(id + ", " + idMuseum + ", " + name + ", " + author + ", " + description + ", " + path);
            }
            rs.close();
        }catch(SQLException se){
            System.out.println("Error: sql error!");
            se.printStackTrace();
        }
    }
}
