using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransferServer
{
    public class FileServer
    {
        private Socket? Server;

        private List<ConnectedClient> ConnectedSockets = new List<ConnectedClient>();

        public FileServer()
        {

        }

        private void Listen(object? state)
        {
            try
            {
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, FileTransfer.Config.Port);

                // Create a Socket that will use the Tcp protocol      
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);

                // Specify how many requests a Socket can listen before it gives Server busy response.  
                // We will listen 10 requests at a time  
                listener.Listen(10);

                Socket newClient = listener.Accept();
                ConnectedClient client = new ConnectedClient(newClient);
                lock(ConnectedSockets)
                {
                    ConnectedSockets.Add(client);
                }
                client.Start();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start server! Exception: " + ex);
            }
            
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(Listen);
        }

        public bool IsStarted()
        {
            return false;
        }
    }
}
