using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferClient
{
    public class FileItem
    {
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string Icon { get
            {
                return IsDirectory ? "Assets/folder.png" : "Assets/file.jpg";
            } }

        public FileItem(bool IsDirectory, string Name)
        {
            this.IsDirectory = IsDirectory;
            this.Name = Name;
        }
    }
}
