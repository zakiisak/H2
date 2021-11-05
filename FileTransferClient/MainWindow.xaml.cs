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

namespace FileTransferClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileClient Client;
        public MainWindow()
        {
            InitializeComponent();
            Client = new FileClient();
        }

        private void OnConnectPressed(object sender, RoutedEventArgs e)
        {
            int port;
            if (int.TryParse(PortField.Text, out port))
            {
                if (Client.Connect(IpField.Text, port))
                {
                    new Explorer(Client).Show();
                    this.Close();
                }
                else MessageBox.Show("Kunne ikke forbinde til " + IpField + ":" + port, "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else MessageBox.Show("Den angivne port er ikke valid", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
}
