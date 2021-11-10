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
                        NetFileService.ReceiveFile(Socket, FileTransfer.Config.ServerBaseDirectory);
                    else if (receivedByte == FileTransfer.Config.ServerListCode)
                        SendLists();
                    else if (receivedByte == FileTransfer.Config.ServerFileSendCode)
                        SendFile();
                }   
            }
        }

        

        private void SendFile()
        {
            //Send a one byte packet back to the client to confirm the request
            Socket.Send(new byte[] { FileTransfer.Config.ServerFileSendCode });

            byte[] localFilePathBuffer = new byte[2048];
            int fileNameLength = Socket.Receive(localFilePathBuffer);

            string filePath = Encoding.Unicode.GetString(localFilePathBuffer, 0, fileNameLength);

            string fullPath = Path.Join(FileTransfer.Config.ServerBaseDirectory, filePath);

            NetFileService.SendFile(Socket, fullPath, "/", null);
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
                bool hasParent = dirInfo.Parent != null;

                string finalString = "";

                //If the length is only 1, it means we're in the base directory: "/"
                if (hasParent && receivedDirectory.Length > 1)
                    finalString += ".../\n";

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
