using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileTransferServer
{
    public static class NetFileService
    {
        public delegate void OnFileSent(string filePath, bool Success);


        //The format is as follows:
        //The socket receives the file name/directory to save it in
        //Sends back a confirmation byte that the filename was received
        //Now Receives the entire file, and the endof file byte array to indicate that the end was reached
        public static string ReceiveFile(Socket Socket, string baseDirectory)
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

                string filePath = Path.Join(Path.Join(baseDirectory, directory), fileName);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    int received = 0;
                    while ((received = Socket.Receive(buffer)) != 0)
                    {
                        if (received == FileTransfer.Config.EndOfFile.Length)
                        {
                            bool equal = true;
                            for (int i = 0; i < received; i++)
                            {
                                if (buffer[i] != FileTransfer.Config.EndOfFile[i])
                                {
                                    equal = false;
                                    break;
                                }
                            }
                            if (equal)
                            {
                                break;
                            }
                        }
                        fs.Write(buffer, 0, received);
                    }
                    fs.Flush();
                    fs.Close();
                }
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to receive file! " + ex);
                MessageBox.Show("Kunne ikke hente filen! " + ex, "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static void SendFile(Socket Client, string filePath, string serverDirectory, OnFileSent onFileSent)
        {
            bool success = false;
            try
            {

                FileStream stream = File.OpenRead(filePath);
                string fileName = Path.GetFileName(filePath);

                byte[] fileNameBytes = Encoding.Unicode.GetBytes(serverDirectory + "\n" + fileName);

                Client.Send(fileNameBytes);

                byte[] fileNameConfirmation = new byte[32];

                int confirmationBytesReceived = Client.Receive(fileNameConfirmation);
                if (confirmationBytesReceived == 1)
                {
                    byte[] buffer = new byte[4096];

                    int num = 0;
                    while ((num = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        Client.Send(buffer, 0, num, SocketFlags.None);
                    }

                    Client.Send(FileTransfer.Config.EndOfFile);
                }

                success = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send file: " + filePath + ", Exception: " + ex);
            }
            if (onFileSent != null)
                onFileSent(filePath, success);
        }

    }
}
