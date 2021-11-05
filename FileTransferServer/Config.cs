using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer
{
    public class Config
    {
        public static readonly int Port = 8008;

        public static readonly byte ListCode = 3;
        public static readonly byte FileCode = 2;

        public static readonly string ServerBaseDirectory = "C:";
    }
}
