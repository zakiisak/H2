using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferClient
{
    public class ClientEvents
    {
        public delegate void OnFileSent(string filePath, bool Success);
        public delegate void OnReceiveFileList(string directory, string[] files);
    }
}
