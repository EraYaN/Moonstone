using System;
using System.IO;
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
using System.Windows.Shapes;

namespace TestAppLocalPLayer
{
    public delegate void PathSetEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaction logic for PathWindow.xaml
    /// </summary>
    public partial class PathWindow : Window
    {
        public event PathSetEventHandler PathSet;

        public PathWindow()
        {
            InitializeComponent();
        }

        protected virtual void OnPathSet(EventArgs e)
        {
            if (PathSet != null)
                PathSet(this, e);
        }

        private void setButton_Click (object sender, RoutedEventArgs e)
        {
            string tmp;
            tmp = pathTextBox.Text.Trim();

            if (String.IsNullOrEmpty(tmp) || !Directory.Exists(tmp))
            {
                MessageBox.Show("Please enter a valid path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                MainWindow.musicPath = tmp;
                OnPathSet(EventArgs.Empty);
                this.Hide();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
