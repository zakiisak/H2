using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileTransferClient
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Explorer : Window
    {
        private FileClient Client;
        private string Directory = "/";
        public Explorer(FileClient Client)
        {
            this.Client = Client;
            InitializeComponent();

            Client.GetFileNamesInDirectory(Directory, UpdateFiles);
            FileItems.MouseDoubleClick += FileItems_MouseDoubleClick;
        }

        private void UpdateDirectory(string fullPathDirectory)
        {
            Directory = fullPathDirectory;
            DirectoryLabel.Content = Directory;
            Client.GetFileNamesInDirectory(Directory, UpdateFiles);
        }

        private void FileItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileItem selected = (FileItem)((ListView)sender).SelectedValue;
            if(selected.IsDirectory)
            {
                if(selected.Name.StartsWith("..."))
                {
                    //Go one up
                    string newDirectory = Directory.Substring(0, Directory.LastIndexOf('/'));
                    newDirectory = newDirectory.Substring(0, newDirectory.LastIndexOf('/') + 1);
                    UpdateDirectory(newDirectory);
                }
                else
                    UpdateDirectory(Directory + selected.Name + "/");
            }
            //Download and open file
            else
            {
                Client.ReceiveFile(Directory, selected.Name, OnFileReceived);
            }
        }

        private void OnFileReceived(string fullPath)
        {
            System.Diagnostics.Process.Start(fullPath);
        }

        private void UpdateFiles(string directory, string[] files)
        {
            Debug.WriteLine(directory + ", file count: " + files.Length);
            Dispatcher.Invoke(() =>
            {
                PopulateFiles(directory, files);
            });
        }


        private void PopulateFiles(string directory, string[] files)
        {
            Directory = directory;
            List<FileItem> filesInDirectory = new List<FileItem>();
            foreach(string file in files)
            {
                if (file.EndsWith("/"))
                    filesInDirectory.Add(new FileItem(true, file.Substring(0, file.Length - 1)));
                else filesInDirectory.Add(new FileItem(false, file));
            }

            FileItems.ItemsSource = filesInDirectory;
        }

        private void FileItems_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach(string file in files)
                {
                    Client.WriteFile(Directory, file, delegate (string filePath, bool Success)
                    {
                        Client.GetFileNamesInDirectory(Directory, UpdateFiles);
                    });
                }

            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
