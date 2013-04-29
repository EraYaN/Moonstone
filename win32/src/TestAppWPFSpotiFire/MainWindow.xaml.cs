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
using System.IO;
using SpotiFire;

namespace SpotiFire.TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        Session session;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void SetupSession()
        {
            session.MusicDelivered += session_MusicDeliverd;
        }
        void session_MusicDeliverd(Session sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = e.Frames;
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            session = await Spotify.Task;
            SetupSession();
            
        }        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            session.Logout();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (session.ConnectionState != ConnectionState.LoggedIn)
                {
                    await session.Login("erayan", "de341h1aa9n", false);
                }
                label.Content = session.ConnectionState.ToString();
                
            } catch(Exception ex){
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
