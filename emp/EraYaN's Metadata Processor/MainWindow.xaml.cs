using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            writeLine();
            /*foreach (String str in SupportedMimeType.AllMimeTypes)
            {
                writeLine("Mime: " + str);
            }*/
            DirectoryInfo dirinfo = new DirectoryInfo(@"\\SERVER\media\Videos");
            if (!dirinfo.Exists)
            {
                dirinfo = new DirectoryInfo(@"\\SERVER\Users\Admin\Videos\Movies");
            }
            FileInfo[] files = dirinfo.GetFiles("*.m??",SearchOption.AllDirectories);
            writeLine("Count: "+files.Count());
            //Timer
            Stopwatch swProcessTime = new Stopwatch();
            swProcessTime.Start();
            foreach(FileInfo file in files){
                try{
                    writeLine();
                    writeLine(file.Name);
                    TagLib.File fileTag = TagLib.File.Create(file.FullName);
                    //Timer
                    Stopwatch swFileTime = new Stopwatch();
                    swFileTime.Start();
                    //Parse results
                    fileInfoParser fileInfoParser = new fileInfoParser(file);
                    writeLine("Parse result: " + fileInfoParser);
                    writeLine("TagType: " + fileTag.TagTypes.ToString());
                    writeLine("Title: " + fileTag.Tag.Title + "; Year: " + fileTag.Tag.Year);
                    //Timer output
                    swFileTime.Stop();
                    TimeSpan fileTime = swFileTime.Elapsed;
                    writeLine("Parsed in " + fileTime.TotalMilliseconds + "ms");
                } catch(Exception exception){
                    writeLine("ERROR processing file.");
                    exceptionHandler.triggerException(exception.Message);                    
                }
            }
            swProcessTime.Stop();
            TimeSpan processTime = swProcessTime.Elapsed;
            writeLine("All files (" + files.Count() + ") parsed in " + processTime.TotalMilliseconds + " ms");
            
        }
        public void writeLine(String line){
            textBoxTagLibTest.Text += line+"\r\n";
        }
        public void writeLine(){
            textBoxTagLibTest.Text += "\r\n";
        }
    }
}
