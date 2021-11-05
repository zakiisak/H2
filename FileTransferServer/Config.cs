using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer
{
    public class Config
    {
        public static readonly byte[] EndOfFile = new byte[] { 0, 1, 2, 3, 4, 5, 5, 4, 3, 2, 1, 0xff, 0xff, 0xff }; 


        public static readonly int Port = 8008;

        public static readonly byte ServerListCode = 3;
        public static readonly byte ServerFileReceiveCode = 2;
        public static readonly byte ServerFileSendCode = 1;

        public static readonly string ServerBaseDirectory = @"C:/Users/Isak/source/repos/H2 Opgaver";
    }
}
