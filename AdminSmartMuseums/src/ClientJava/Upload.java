package ClientJava;

import com.jcraft.jsch.*;

public class Upload {

    private  Session session;
    private Channel channel;
    private JSch ssh;
    private ChannelSftp sftp;

    public Upload(){
        this.session = null;
        this.channel = null;
        this.ssh = null;
        this.sftp = null;
    }

    public void start() throws JSchException {
            ssh = new JSch();
            ssh.setConfig("StrictHostKeyChecking", "no");
            ssh.setKnownHosts("/home/smartmuseum");
            session = ssh.getSession("smartmuseum", "ec2-52-37-205-247.us-west-2.compute.amazonaws.com", 22);
            session.setPassword("1234");
            session.connect();
            channel = session.openChannel("sftp");
            channel.connect();
            sftp = (ChannelSftp) channel;
            System.out.println("Service [UPLOAD] started.");
    }

    public void stop(){
        sftp.disconnect();
        channel.disconnect();
        session.disconnect();
        ssh = null;
        sftp = null;
        channel = null;
        session = null;
        System.out.println("Service [UPLOAD] closed.");
    }

    public void file(String filePath, String destination) throws SftpException {

        if (sftp != null)
        {
            sftp.put(filePath, destination);
            System.out.println("File has been send!");

        }else{
            System.out.println("You need to call start() first!");
        }

    }


}
