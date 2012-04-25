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
using System.ComponentModel;

namespace EMP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BackgroundWorker bw = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBlockStatus.Text = "Completed";
            progressBarScan.Value = 100;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(true) / 1024 / 1024, 2);
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarScan.Value = e.ProgressPercentage;            
            textBlockStatus.Text = "Scanning...";
            writeLine((String)e.UserState);
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 2);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo dirinfo = (DirectoryInfo)e.Argument;
            FileInfo[] files = dirinfo.GetFiles("*.m??", SearchOption.AllDirectories);
            double count = files.Count();
            double filenum = 0;
            bw.ReportProgress((int)Math.Round(filenum/count*100),"Count: "+count);
            //Timer
            Stopwatch swProcessTime = new Stopwatch();
            swProcessTime.Start();
            foreach (FileInfo file in files)
            {
                if (bw.CancellationPending)
                {
                    break;
                }
                filenum++;
                try
                {
                                        
                    bw.ReportProgress((int)Math.Round(filenum / count * 100), "\r\n"+file.Name);
                    TagLib.File fileTag = TagLib.File.Create(file.FullName);
                    //Timer
                    Stopwatch swFileTime = new Stopwatch();
                    swFileTime.Start();
                    //Parse results
                    fileInfoParser fileInfoParser = new fileInfoParser(file);
                    bw.ReportProgress((int)Math.Round(filenum / count * 100), "Parse result: " + fileInfoParser);
                    bw.ReportProgress((int)Math.Round(filenum / count * 100), "TagType: " + fileTag.TagTypes.ToString());
                    bw.ReportProgress((int)Math.Round(filenum / count * 100), "Title: " + fileTag.Tag.Title + "; Year: " + fileTag.Tag.Year);
                    //Timer output
                    swFileTime.Stop();
                    TimeSpan fileTime = swFileTime.Elapsed;
                    bw.ReportProgress((int)Math.Round(filenum / count * 100), "Parsed in " + fileTime.TotalMilliseconds + "ms");
                    fileInfoParser = null;
                    fileTag.Dispose();
                    fileTag = null;
                }
                catch (Exception exception)
                {
                    bw.ReportProgress((int)Math.Round(filenum / count * 100), "ERROR processing file.");
                    exceptionHandler.triggerException(exception.Message);
                }
                
            }

            swProcessTime.Stop();
            TimeSpan processTime = swProcessTime.Elapsed;
            bw.ReportProgress(100, "All files (" + files.Count() + ") parsed in " + processTime.TotalMilliseconds + " ms");
            files = null;
            dirinfo = null;
        }       

        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy) {
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
                //bw worker
                bw.RunWorkerAsync(dirinfo);
            }
                        
        }
        public void writeLine(String line){
            textBoxTagLibTest.Text += line+"\r\n";
        }
        public void writeLine(){
            textBoxTagLibTest.Text += "\r\n";
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxTagLibTest.Text = "";
            GC.Collect();
            GC.WaitForPendingFinalizers();
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(true) / 1024 / 1024, 2);
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(true) / 1024 / 1024, 2);
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter("output.log",false);

            // write a line of text to the file
            tw.Write(textBoxTagLibTest.Text);

            // close the stream
            tw.Close();
        }
       
    }
}
