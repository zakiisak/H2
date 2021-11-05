using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferClient
{
    public class FileClient
    {
        private Socket? Client;
        private string Ip;
        private int Port;

        private Queue<Action> SendQueue = new Queue<Action>();

        public FileClient(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }

        public bool IsConnected()
        {
            return Client != null && Client.Connected;
        }

        public bool Connect()
        {
            try
            {
                // Establish the local endpoint for the socket.
                IPHostEntry ipHost = Dns.GetHostEntry(Ip);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, Port);

                // Create a TCP socket.
                Client = new Socket(AddressFamily.InterNetwork,
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

        private static byte[] dataPrefix = new byte[] { 84, 84, 84, 32, 32, 32, 13, 37 };

        public void GetFileNamesInDirectory(string remoteDirectory, ClientEvents.OnReceiveFileList onReceiveFileList)
        {
            lock (SendQueue)
            {
                SendQueue.Enqueue(() =>
                {
                    byte[] stringData = Encoding.Unicode.GetBytes(remoteDirectory);

                    byte[] data;
                    int offset;
                    lock (dataPrefix)
                    {
                        data = new byte[dataPrefix.Length + stringData.Length];

                        for (int i = 0; i < dataPrefix.Length; i++)
                            data[i] = dataPrefix[i];
                        offset = dataPrefix.Length;
                    }

                    for (int i = 0; i < stringData.Length; i++)
                        data[offset + i] = stringData[i];

                    Client.Send(data);

                    byte[] receivedData = new byte[8192];

                    int bytesReceived = Client.Receive(receivedData);
                    string fileSegments = Encoding.Unicode.GetString(receivedData, 0, bytesReceived);
                    string[] split = fileSegments.Split('\n');

                    onReceiveFileList(remoteDirectory, split);

                });
            }
        }

        public void WriteFile(string filePath, ClientEvents.OnFileSent onFileSent = null)
        {
            lock (SendQueue)
            {
                SendQueue.Enqueue(() =>
                {
                    bool success = false;
                    try
                    {
                        Client.SendFile(filePath);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to send file: " + filePath + ", Exception: " + ex);
                    }
                    if (onFileSent != null)
                        onFileSent(filePath, success);

                });
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
