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
                    if (receivedByte == FileTransfer.Config.FileCode)
                        ReceiveFile();
                    else if (receivedByte == FileTransfer.Config.ListCode)
                        SendLists();
                }   
            }
        }

        private void ReceiveFile()
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

                string[] files = Directory.GetFileSystemEntries(FileTransfer.Config.ServerBaseDirectory + receivedDirectory);
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
