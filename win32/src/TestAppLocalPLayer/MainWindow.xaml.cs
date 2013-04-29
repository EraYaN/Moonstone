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

namespace TestAppLocalPLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PathWindow pathWindow = new PathWindow();
        public static string musicPath;

        public MainWindow()
        {
            InitializeComponent();
            pathWindow.PathSet += pathWindow_PathSet;
            this.Closed += MainWindow_Closed;
        }

        #region Button handlers
        private void setPathButton_Click(object sender, RoutedEventArgs e)
        {
            pathWindow.Show();
        }

        private void playpauseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        #region Event handlers
        private void pathWindow_PathSet(object sender, EventArgs e)
        {
            MusicList musicList = new MusicList();
            foreach (string filepath in musicList.FilePaths)
            {
                musiclistListView.Items.Add(filepath);
            }
            entriesfoundLabel.Content = musicList.FilePaths.Count() + " entries found";
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            pathWindow.Close();
        }
        #endregion
    }


}
