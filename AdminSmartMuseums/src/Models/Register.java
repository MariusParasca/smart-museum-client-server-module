package Models;

import javax.mail.*;
import javax.mail.internet.*;
import java.math.BigInteger;
import java.nio.charset.Charset;
import java.security.SecureRandom;
import java.util.*;

public class Register {

    final String senderEmailID = "museums.smart@gmail.com";
    final String senderPassword = "SmartMuseums";
    final String emailSMTPserver = "smtp.gmail.com";
    final String emailServerPort = "465";
    String receiverEmailID =null;
    static String emailSubject = "Test Mail";
    static String emailBody = ":)";

    public String randomString (){
        SecureRandom secureRandom = new SecureRandom();
        byte[] token = new byte[10];
        secureRandom.nextBytes(token);
        return new BigInteger(1, token).toString(16);
    }
    public Register(String rcv, String Subject) {

        // Receiver Email Address
        // Subject
        this.receiverEmailID=rcv;
        this.emailSubject = Subject;
        // Body
        this.emailBody = "Your username:" + randomString() + " and password: "+ randomString();


        Properties props = new Properties();
        props.put("mail.smtp.user", senderEmailID);
        props.put("mail.smtp.host", emailSMTPserver);
        props.put("mail.smtp.port", emailServerPort);
        props.put("mail.smtp.starttls.enable", "true");
        props.put("mail.smtp.auth", "true");
        props.put("mail.smtp.socketFactory.port", emailServerPort);
        props.put("mail.smtp.socketFactory.class", "javax.net.ssl.SSLSocketFactory");
        props.put("mail.smtp.socketFactory.fallback", "false");
        SecurityManager security = System.getSecurityManager();
        try {
            Authenticator auth = new SMTPAuthenticator();
            Session session = Session.getInstance(props, auth);
            MimeMessage msg = new MimeMessage(session);
            msg.setText(emailBody);
            msg.setSubject(emailSubject);
            msg.setFrom(new InternetAddress(senderEmailID));
            msg.addRecipient(Message.RecipientType.TO, new InternetAddress(receiverEmailID));
            Transport.send(msg);
            System.out.println("Message send Successfully:)");
        }

        catch (Exception mex) {
            mex.printStackTrace();
        }

    }

    public class SMTPAuthenticator extends javax.mail.Authenticator {
        public PasswordAuthentication getPasswordAuthentication() {
            return new PasswordAuthentication(senderEmailID, senderPassword);
        }
    }

}