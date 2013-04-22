using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using libspotifydotnet;

namespace TestAppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr session;
        libspotify.sp_session_config config = new libspotify.sp_session_config();
        libspotify.sp_session_callbacks callbacks = new libspotify.sp_session_callbacks();
        public MainWindow()
        {
            InitializeComponent();
            config.api_version = libspotify.SPOTIFY_API_VERSION;
            config.application_key = IntPtr.Zero;

           
        }
    }
}
