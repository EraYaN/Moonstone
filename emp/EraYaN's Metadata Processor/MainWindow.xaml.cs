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
using TagLib;
using System.IO;

namespace EMP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Started");
            //File file = File.Create(@"\\SERVER\media\iTunes\iTunes Media\Movies\My Week With Marilyn\My Week With Marilyn.m4v","taglib/m4v",ReadStyle.Average);
                       
            //MessageBox.Show("Done");
            //textBlockTagLibTest.Text += "\r\n Title: "+file.Tag.Title;
            //textBlockTagLibTest.Text += "Je oma";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            writeLine();
            /*foreach (String str in SupportedMimeType.AllMimeTypes)
            {
                writeLine("Mime: " + str);
            }*/
            DirectoryInfo dirinfo = new DirectoryInfo(@"\\SERVER\media\Videos");
            FileInfo[] files = dirinfo.GetFiles("*.m??",SearchOption.AllDirectories);
            writeLine("Count: "+files.Count().ToString());
            foreach(FileInfo file in files){
                try{
                    writeLine();
                    writeLine(file.Name);
                    TagLib.File fileTag = TagLib.File.Create(file.FullName);                    
                    writeLine("TagType: " + fileTag.TagTypes.ToString());
                    writeLine("Title: " + fileTag.Tag.Title + "; Year: " + fileTag.Tag.Year);
                } catch(Exception exception){
                    exceptionHandling.triggerExeption(exception.Message);
                }
            }
            
        }
        public void writeLine(String line){
            textBoxTagLibTest.Text += line+"\r\n";
        }
        public void writeLine(){
            textBoxTagLibTest.Text += "\r\n";
        }
    }
}
