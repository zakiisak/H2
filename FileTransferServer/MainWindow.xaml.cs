using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileTransferServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileServer Server;
        public MainWindow()
        {
            InitializeComponent();
            this.Server = new FileServer();

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Server.Stop();
        }

        private void OnStartClicked(object sender, RoutedEventArgs e)
        {
            if (Server.IsStarted())
            {
                Server.Stop();
                StartButton.Content = "Start";
            }
            else
            {
                Server.Start();
                StartButton.Content = "Stop";
            }
        }
    }
}
