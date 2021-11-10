using FileTransferServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static FileTransferClient.ClientEvents;
using static FileTransferServer.NetFileService;

namespace FileTransferClient
{
    public class FileClient
    {
        private Socket? Client;
        private string Ip;
        private int Port;

        private Queue<Action> SendQueue = new Queue<Action>();

        public bool IsConnected()
        {
            return Client != null && Client.Connected;
        }

        public bool Connect(string Ip, int Port)
        {
            this.Ip = Ip;
            this.Port = Port;
            try
            {
                // Establish the local endpoint for the socket.
                IPHostEntry ipHost = Dns.GetHostEntry(Ip);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, Port);

                // Create a TCP socket.
                Client = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint.
                Client.Connect(ipEndPoint);
                ThreadPool.QueueUserWorkItem(SendPackets);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to connect to " + Ip + ":" + Port + " Exception: " + ex);
            }
            return false;
        }

        private void SendPackets(object? state)
        {
            while (IsConnected())
            {
                lock(SendQueue)
                {
                    while (SendQueue.Count == 0)
                        Monitor.Wait(SendQueue);
                    //Run the next action in the queue
                    SendQueue.Dequeue()();
                }
            }
        }

        public void GetFileNamesInDirectory(string remoteDirectory, ClientEvents.OnReceiveFileList onReceiveFileList)
        {
            lock (SendQueue)
            {
                SendQueue.Enqueue(() =>
                {
                    byte[] stringData = Encoding.Unicode.GetBytes(remoteDirectory);


                    Client.Send(new byte[] { FileTransfer.Config.ServerListCode });

                    byte[] confirmationBuffer = new byte[256];
                    Client.Receive(confirmationBuffer);
                    if (confirmationBuffer[0] == FileTransfer.Config.ServerListCode)
                    {
                        Client.Send(stringData);

                        byte[] receivedData = new byte[8192];

                        int bytesReceived = Client.Receive(receivedData);
                        string fileSegments = Encoding.Unicode.GetString(receivedData, 0, bytesReceived);
                        string[] split = fileSegments.Split('\n');

                        onReceiveFileList(remoteDirectory, split);
                    }

                });
                Monitor.PulseAll(SendQueue);
            }
        }

        public void WriteFile(string serverDirectory, string filePath, OnFileSent onFileSent = null)
        {
            lock (SendQueue)
            {
                SendQueue.Enqueue(() =>
                {
                    Client.Send(new byte[] { FileTransfer.Config.ServerFileReceiveCode });

                    byte[] confirmationBuffer = new byte[256];
                    int length = Client.Receive(confirmationBuffer);

                    if(length == 1)
                    {
                        NetFileService.SendFile(Client, filePath, serverDirectory, onFileSent);
                    }

                });
                Monitor.PulseAll(SendQueue);
            }
        }

        public void ReceiveFile(string serverDirectory, string fileName, OnReceiveFile OnReceived = null)
        {
            try
            {
                lock (SendQueue)
                {
                    SendQueue.Enqueue(() =>
                    {
                        Client.Send(new byte[] { FileTransfer.Config.ServerFileSendCode });
                        byte[] confirmationBuffer = new byte[32];
                        int length = Client.Receive(confirmationBuffer);

                        //Send the local file path
                        string localPath = Path.Join(serverDirectory, fileName);
                        byte[] localPathBytes = Encoding.Unicode.GetBytes(localPath);
                        Client.Send(localPathBytes);

                        if(length == 1)
                        {
                            string fullFilePath = NetFileService.ReceiveFile(Client, Path.GetTempPath());

                            if(fullFilePath != null && OnReceived != null)
                            {
                                OnReceived(fullFilePath);
                            }
                        }

                    });
                    Monitor.PulseAll(SendQueue);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to receive file! Exception: " + ex);
            }
        }

        public void StopConnection()
        {
            try
            {
                // Release the socket.
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Stop Connection error! " + ex);
            }
        }

    }
}
