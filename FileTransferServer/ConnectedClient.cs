using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferServer
{
    public class ConnectedClient
    {
        private Socket Socket;

        public ConnectedClient(Socket socket)
        {
            Socket = socket;
        }

        private void Run(object? state)
        {
            while(Socket.Connected)
            {
                byte[] buffer = new byte[1024];
                int receivedCount = Socket.Receive(buffer);
                if(receivedCount == 1)
                {
                    byte receivedByte = buffer[0];
                    Socket.Send(new byte[] { receivedByte });
                    if (receivedByte == FileTransfer.Config.ServerFileReceiveCode)
                        ReceiveFile();
                    else if (receivedByte == FileTransfer.Config.ServerListCode)
                        SendLists();
                }   
            }
        }

        private void ReceiveFile()
        {
            try
            {
                byte[] buffer = new byte[4096];
                //First fetch the file name
                int nameBufferLength = Socket.Receive(buffer);

                string fileNameAndDirectory = Encoding.Unicode.GetString(buffer, 0, nameBufferLength);
                string[] split = fileNameAndDirectory.Split('\n');
                string directory = split[0];
                string fileName = split[1];

                //Clear the buffer
                for (int i = 0; i < nameBufferLength; i++)
                    buffer[i] = 0;

                //Send a random confirmation byte back that the name was received
                Socket.Send(new byte[] { 0 });

                using (var fs = new FileStream(Path.Join(FileTransfer.Config.ServerBaseDirectory + directory, fileName), FileMode.Create, FileAccess.Write))
                {
                    int received = 0;
                    while ((received = Socket.Receive(buffer)) != 0)
                    {
                        if(received == FileTransfer.Config.EndOfFile.Length)
                        {
                            bool equal = true;
                            for(int i = 0; i < received; i++)
                            {
                                if(buffer[i] != FileTransfer.Config.EndOfFile[i])
                                {
                                    equal = false;
                                    break;
                                }
                            }
                            if(equal)
                            {
                                break;
                            }
                        }
                        fs.Write(buffer, 0, received);
                    }
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to receive file! ");
            }
        }

        private void SendFile()
        {

        }

        private void SendLists()
        {
            string receivedDirectory = null;
            try
            {
                byte[] directoryBytes = new byte[2048];
                int receivedBytes = Socket.Receive(directoryBytes);
                receivedDirectory = Encoding.Unicode.GetString(directoryBytes, 0, receivedBytes);

                string dir = FileTransfer.Config.ServerBaseDirectory + receivedDirectory;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                
                FileInfo[] files = dirInfo.GetFiles();
                DirectoryInfo[] directories = dirInfo.GetDirectories();

                string finalString = "";
                for(int i = 0; i < files.Length; i++)
                {
                    finalString += files[i].Name;
                    if (directories.Length > 0 || i < files.Length - 1)
                        finalString += "\n";
                }
                for(int i = 0; i < directories.Length; i++)
                {
                    finalString += directories[i].Name + "/";
                    if (i < directories.Length - 1)
                        finalString += "\n";
                }
                byte[] listData = Encoding.Unicode.GetBytes(finalString);
                Socket.Send(listData);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to retrieve directory / lists in directory. Received Directory: " + receivedDirectory + ". Exception: " + ex);
            }
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(Run);
        }

    }
}
