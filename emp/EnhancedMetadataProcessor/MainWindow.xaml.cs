using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using TagLib;
using System.Windows.Controls;
using iTunesLib;

namespace EMP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker scanBackgroundWorkerF = new BackgroundWorker(); //Folder Source Scan BackgroundWorker Thread
        BackgroundWorker scanBackgroundWorkerI = new BackgroundWorker(); //iTunes Source Scan BackgroundWorker Thread
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
                    FileInfoParser fileInfoParser = new FileInfoParser(file);
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
                catch (UnsupportedFormatException Exception)
                {
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "File format not supported.");
                    ExceptionHandler.TriggerException(Exception.Message);
                }
                catch (Exception Exception)
                {
                    scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "ERROR processing file.");
                    ExceptionHandler.TriggerException(Exception.Message);
                    throw Exception;
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
            writeLine((String)e.UserState);
            textBlockData.Text = "Data (MB):\n" + Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 2);
        }

        void scanBackgroundWorkerI_DoWork(object sender, DoWorkEventArgs e)
        {
            //do work
            scanBackgroundWorkerI.ReportProgress(0, "Loading iTunes COM object...");
            using (iTunesCOM iTCOM = new iTunesCOM())
            {
                iTCOM.iTunesQuit +=new _IiTunesEvents_OnQuittingEventEventHandler(iTCOM_iTunesQuit);
                iTCOM.iTunesAboutToPromptUser +=new _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler(iTCOM_iTunesAboutToPromptUser);
                scanBackgroundWorkerI.ReportProgress(0, "Connected to iTunes.");
                int totalitems = iTCOM.TVShowPlaylist.Tracks.Count + iTCOM.MoviePlaylist.Tracks.Count + iTCOM.Tracks.Count;
                int currentitem = 1;
                if (scanBackgroundWorkerI.CancellationPending)
                {
                    return;
                }
                scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), iTCOM.Tracks.Count + " \"tracks\" found.");
                scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), iTCOM.Playlists.Count + " \"playlists\" found.");
                scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), iTCOM.MoviePlaylist.Name + " contains movies.");
                scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), iTCOM.TVShowPlaylist.Name + " contains TV shows.");
                if (scanBackgroundWorkerI.CancellationPending)
                {
                    return;
                }
                if (iTCOM.MoviePlaylist.Tracks.Count > 0)
                {
                    scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), "\nMovies:");
                    foreach (IITTrack Track in iTCOM.MoviePlaylist.Tracks)
                    {
                        
                        scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem++, totalitems), "\t- " + Track.Name + " (" + Track.Year + ")");
                        if (scanBackgroundWorkerI.CancellationPending)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), "\nNo Movies found.");
                }
                if (scanBackgroundWorkerI.CancellationPending)
                {
                    return;
                }
                if (iTCOM.TVShowPlaylist.Tracks.Count > 0)
                {
                    scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), "\nTV Shows:");
                    foreach (IITTrack Track in iTCOM.TVShowPlaylist.Tracks)
                    {
                        if (scanBackgroundWorkerI.CancellationPending)
                        {
                            return;
                        }
                        scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem++, totalitems), "\t- " + Track.Name + " (" + Track.Year + ")");
                    }
                }
                else
                {
                    scanBackgroundWorkerI.ReportProgress(CalcPercentage(currentitem, totalitems), "\nNo TV Shows found.");
                }
                iTCOM.iTunesQuit -= new _IiTunesEvents_OnQuittingEventEventHandler(iTCOM_iTunesQuit);
                iTCOM.iTunesAboutToPromptUser -= new _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler(iTCOM_iTunesAboutToPromptUser);
            }
            
        }
        #endregion
        #region EventHandlers
        private void iTCOM_iTunesQuit()
        {
            scanBackgroundWorkerI.ReportProgress(0, "\niTunes has quit unexpectedly. iTunes scan has been stopped unceremoniously.");            
        }
        private void iTCOM_iTunesAboutToPromptUser()
        {
            scanBackgroundWorkerI.ReportProgress(0, "\nTrying to close all connections with iTunes. iTunes scan will be stopped.");
            AbortScan(false,true);
        }        
        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            if (((ComboBoxItem)comboBoxSource.SelectedItem).Content.ToString() == "Folder")
            {
                if (!scanBackgroundWorkerF.IsBusy)
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
                    textBlockStatus.Text = "Scanning...";
                    progressBarScan.Value = 0;
                    //bw worker
                    scanBackgroundWorkerF.RunWorkerAsync(dirinfo);
                }
                else
                {
                    writeLine("The worker thread is busy.");
                    textBlockStatus.Text = "Error.";
                }
            }
            else if (((ComboBoxItem)comboBoxSource.SelectedItem).Content.ToString() == "iTunes")
            {
                if (!scanBackgroundWorkerI.IsBusy)
                {
                    writeLine();
                    textBlockStatus.Text = "Scanning...";
                    progressBarScan.Value = 0;
                    scanBackgroundWorkerI.RunWorkerAsync();
                }
                else
                {
                    writeLine("The worker thread is busy.");
                    textBlockStatus.Text = "Error.";
                }
            }
            else
            {
                writeLine("Please select a source.");
                textBlockStatus.Text = "Idle.";
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
            AbortScan();
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
        private void AbortScan()
        {
            if (scanBackgroundWorkerF.IsBusy)
            {
                scanBackgroundWorkerF.CancelAsync();
            }
            if (scanBackgroundWorkerI.IsBusy)
            {
                scanBackgroundWorkerI.CancelAsync();
            }
        }
        private void AbortScan(bool F, bool I)
        {
            if (scanBackgroundWorkerF.IsBusy&&F)
            {
                scanBackgroundWorkerF.CancelAsync();
            }
            if (scanBackgroundWorkerI.IsBusy&&I)
            {
                scanBackgroundWorkerI.CancelAsync();
            }
        }
        public void writeLine(String line)
        {
            textBoxTagLibTest.Text += line + "\r\n";
        }
        public void writeLine()
        {
            textBoxTagLibTest.Text += "\r\n";
        }
        public static Int32 CalcPercentage(decimal part, decimal whole){
            return (Int32)Math.Round(part / whole * 100);
        }

        private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
        {
            if (scanBackgroundWorkerF.IsBusy)
            {
                scanBackgroundWorkerF.CancelAsync();
            }
            if (scanBackgroundWorkerI.IsBusy)
            {
                scanBackgroundWorkerI.CancelAsync();
            }
            
        }
    }
}
