using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace EMP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BackgroundWorker scanBackgroundWorkerF = new BackgroundWorker(); //Folder Source Scan BackgroundWorker Thread
        public BackgroundWorker scanBackgroundWorkerI = new BackgroundWorker(); //iTunes Source Scan BackgroundWorker Thread
        public MainWindow()
        {
            InitializeComponent();
            scanBackgroundWorkerF.WorkerReportsProgress = true;
            scanBackgroundWorkerF.WorkerSupportsCancellation = true;
            scanBackgroundWorkerF.DoWork += new DoWorkEventHandler(scanBackgroundWorkerF_DoWork);
            scanBackgroundWorkerF.ProgressChanged += new ProgressChangedEventHandler(scanBackgroundWorkerF_ProgressChanged);
            scanBackgroundWorkerF.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanBackgroundWorkerF_RunWorkerCompleted);
            scanBackgroundWorkerI.WorkerReportsProgress = true;
            scanBackgroundWorkerI.WorkerSupportsCancellation = true;
            scanBackgroundWorkerI.DoWork += new DoWorkEventHandler(scanBackgroundWorkerI_DoWork);
            scanBackgroundWorkerI.ProgressChanged += new ProgressChangedEventHandler(scanBackgroundWorkerI_ProgressChanged);
            scanBackgroundWorkerI.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanBackgroundWorkerI_RunWorkerCompleted);
        }
        #region ScanBackgroundWorkerFolderSource        
        void scanBackgroundWorkerF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mainWindow.textBlockStatus.Text = "Completed";
            progressBarScan.Value = 100;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(true) / 1024 / 1024, 2);
        }

        void scanBackgroundWorkerF_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarScan.Value = e.ProgressPercentage;
            textBlockStatus.Text = "Scanning...";
            writeLine((String)e.UserState);
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 2);
        }

        void scanBackgroundWorkerF_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectoryInfo dirinfo = (DirectoryInfo)e.Argument;
            FileInfo[] files = dirinfo.GetFiles("*.m??", SearchOption.AllDirectories);
            double count = files.Count();
            double filenum = 0;
            scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "Count: " + count);
            //Timer
            Stopwatch swProcessTime = new Stopwatch();
            swProcessTime.Start();
            foreach (FileInfo file in files)
            {
                if (scanBackgroundWorkerF.CancellationPending)
                {
                    break;
                }
                filenum++;
                try
                {

                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "\r\n" + file.Name);
                    TagLib.File fileTag = TagLib.File.Create(file.FullName);
                    //Timer
                    Stopwatch swFileTime = new Stopwatch();
                    swFileTime.Start();
                    //Parse results
                    fileInfoParser fileInfoParser = new fileInfoParser(file);
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "Parse result: " + fileInfoParser);
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "TagType: " + fileTag.TagTypes.ToString());
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "Title: " + fileTag.Tag.Title + "; Year: " + fileTag.Tag.Year);
                    //Timer output
                    swFileTime.Stop();
                    TimeSpan fileTime = swFileTime.Elapsed;
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "Parsed in " + fileTime.TotalMilliseconds + "ms");
                    fileInfoParser = null;
                    fileTag.Dispose();
                    fileTag = null;
                }
                catch (Exception exception)
                {
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "ERROR processing file.");
                    exceptionHandler.triggerException(exception.Message);
                }

            }

            swProcessTime.Stop();
            TimeSpan processTime = swProcessTime.Elapsed;
            scanBackgroundWorkerF.ReportProgress(100, "All files (" + files.Count() + ") parsed in " + processTime.TotalMilliseconds + " ms");
            files = null;
            dirinfo = null;
        }       
        #endregion
        #region ScanBackgroundWorkeriTunesSource
        void scanBackgroundWorkerI_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mainWindow.textBlockStatus.Text = "Completed";
            progressBarScan.Value = 100;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(true) / 1024 / 1024, 2);
        }

        void scanBackgroundWorkerI_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarScan.Value = e.ProgressPercentage;
            textBlockStatus.Text = "Scanning...";
            writeLine((String)e.UserState);
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 2);
        }

        void scanBackgroundWorkerI_DoWork(object sender, DoWorkEventArgs e)
        {
            //do work
            iTunesCOM iTCOM = new iTunesCOM();
        }
        #endregion
        #region EventHandlers
        
        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            if (!scanBackgroundWorkerF.IsBusy) {
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
                scanBackgroundWorkerF.RunWorkerAsync(dirinfo);
            }
                        
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
            if (scanBackgroundWorkerF.IsBusy)
            {
                scanBackgroundWorkerF.CancelAsync();
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
        #endregion
        public void writeLine(String line)
        {
            textBoxTagLibTest.Text += line + "\r\n";
        }
        public void writeLine()
        {
            textBoxTagLibTest.Text += "\r\n";
        }
    }
}
