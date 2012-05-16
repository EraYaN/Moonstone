using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using TagLib;
using System.Windows.Controls;
using iTunesLib;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace EMP
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		BackgroundWorker scanBackgroundWorkerF = new BackgroundWorker(); //Folder Source Scan BackgroundWorker Thread
		BackgroundWorker scanBackgroundWorkerI = new BackgroundWorker(); //iTunes Source Scan BackgroundWorker Thread
		ConfigurationWindow configurationWindow;
		AboutWindow aboutWindow;
		public Configuration config;
		static FileInfo ConfigurationFilePath = new FileInfo(@"Configuration\Configuration.bin");
		public MainWindow()
		{
			InitializeComponent();
			Application.Current.Exit += new ExitEventHandler(Current_Exit);
			Assembly assem = Assembly.GetEntryAssembly();
			AssemblyName assemName = assem.GetName();
			Version ver = assemName.Version;
			writeLine("Welcome to the " + assemName.Name + " version " + ver.ToString());
			#region Workers Init
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
			#endregion
			#region Config Init
			config = new Configuration();
			try
			{
				if (ConfigurationFilePath.Exists)
				{
					//deserialize
					//Open the file written above and read values from it.
					Stream stream = System.IO.File.Open(ConfigurationFilePath.FullName, FileMode.Open);
					BinaryFormatter formatter = new BinaryFormatter();

					ConfigurationSaveHelper configSaveHelper = (ConfigurationSaveHelper)formatter.Deserialize(stream);
					stream.Close();
					config.LoadConfigurationHelper(configSaveHelper);
					writeLine("Config succesfully loaded from file.");
				}
				else if (!Directory.Exists(ConfigurationFilePath.Directory.FullName))
				{
					Directory.CreateDirectory(ConfigurationFilePath.Directory.FullName);
				}
			}
			catch (SerializationException e)
			{
				writeLine("Config could not be loaded default has been loaded. Message: " + e.Message);
				config = new Configuration();
			}
			catch (Exception e)
			{
				writeLine("Error loading config: " + e.Message);
				ExceptionHandler.TriggerException("Error loading config: " + e.Message, ExceptionHandler.ExceptionLevel.Error, e);
				throw e;
			}
			finally
			{
				SaveConfigurationToFile();
			}

			#endregion
			#region Windows Init
			configurationWindow = new ConfigurationWindow();
			aboutWindow = new AboutWindow();
			#endregion
			#region Config UI creation
			foreach (Entities.Tab tab in config.Tabs)
			{
				Grid grid = new Grid();
				grid.Name = "ConfigurationGrid" + tab.Identifier;
				StackPanel stackpanel = new StackPanel();
				stackpanel.Name = "ConfigurationStackpanel" + tab.Identifier;
				stackpanel.HorizontalAlignment = HorizontalAlignment.Stretch;
				stackpanel.VerticalAlignment = VerticalAlignment.Stretch;
				grid.Children.Add(stackpanel);
				foreach (Entities.Group group in tab.Groups)
				{
					GroupBox groupbox = new GroupBox();
					groupbox.Name = "ConfigurationGroupBox" + group.Identifier;
					groupbox.Header = group.Name;
					groupbox.HorizontalAlignment = HorizontalAlignment.Stretch;
					groupbox.VerticalAlignment = VerticalAlignment.Top;
					Grid groupgrid = new Grid();
					groupbox.Content = groupgrid;
					StackPanel groupstackpanel = new StackPanel();
					groupstackpanel.Name = "ConfigurationGroupStackpanel" + tab.Identifier;
					groupstackpanel.HorizontalAlignment = HorizontalAlignment.Stretch;
					groupstackpanel.VerticalAlignment = VerticalAlignment.Stretch;
					groupgrid.Children.Add(groupstackpanel);
					foreach (Entities.Setting setting in group.Settings)
					{
						//add extra types of controls here
						if (setting.Type == typeof(String))
						{
							Label label = new Label();
							label.HorizontalAlignment = HorizontalAlignment.Stretch;
							label.Content = setting.Name;
							label.Height = 28;
							label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
							groupstackpanel.Children.Add(label);
							TextBox textbox = new TextBox();
							textbox.HorizontalAlignment = HorizontalAlignment.Stretch;
							textbox.Text = setting.Value.ToString();
							textbox.Name = "ConfigurationSetting" + setting.Identifier;
							RegisterName(textbox.Name, textbox);
							textbox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
							groupstackpanel.Children.Add(textbox);
							label.Target = textbox;
						}
						else if (setting.Type == typeof(Boolean))
						{
							CheckBox checkbox = new CheckBox();
							checkbox.HorizontalAlignment = HorizontalAlignment.Stretch;
							checkbox.Content = setting.Name;
							checkbox.IsChecked = (Boolean)setting.Value;
							checkbox.Name = "ConfigurationSetting" + setting.Identifier;
							checkbox.Margin = new Thickness(5, 5, 5, 5);
							RegisterName(checkbox.Name, checkbox);
							checkbox.VerticalAlignment = System.Windows.VerticalAlignment.Top;
							groupstackpanel.Children.Add(checkbox);
						}

					}
					stackpanel.Children.Add(groupbox);
				}

				TabItem tabitem = new TabItem();

				tabitem.Name = "ConfigurationTabItem" + tab.Identifier;
				tabitem.Header = tab.Name;
				tabitem.Content = grid;
				configurationWindow.ConfigurationTabs.Items.Add(tabitem);
			}
			#endregion
			#region Config Event Subscription
			configurationWindow.ButtonOK.Click += new RoutedEventHandler(ConfigurationWindow_ButtonOK_Click);
			configurationWindow.ButtonCancel.Click += new RoutedEventHandler(ConfigurationWindow_ButtonCancel_Click);
			#endregion
			#region About Event Subscription
			aboutWindow.Loaded += new RoutedEventHandler(aboutWindow_Loaded);
			#endregion
		}




		#region ScanBackgroundWorkerFolderSource
		void scanBackgroundWorkerF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			mainWindow.textBlockStatus.Text = "Completed";
			progressBarScan.Value = 100;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			UpdateMemoryUsage();
		}

		void scanBackgroundWorkerF_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBarScan.Value = e.ProgressPercentage;
			textBlockStatus.Text = "Scanning...";
			writeLine((String)e.UserState);
			UpdateMemoryUsage();
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
			UpdateMemoryUsage();
		}

		void scanBackgroundWorkerI_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBarScan.Value = e.ProgressPercentage;
			writeLine((String)e.UserState);
			UpdateMemoryUsage();
		}

		void scanBackgroundWorkerI_DoWork(object sender, DoWorkEventArgs e)
		{
			//do work
			scanBackgroundWorkerI.ReportProgress(0, "Loading iTunes COM object...");
			using (iTunesCOM iTCOM = new iTunesCOM())
			{
				iTCOM.iTunesQuit += new _IiTunesEvents_OnQuittingEventEventHandler(iTCOM_iTunesQuit);
				iTCOM.iTunesAboutToPromptUser += new _IiTunesEvents_OnAboutToPromptUserToQuitEventEventHandler(iTCOM_iTunesAboutToPromptUser);
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
		#region EventHandlers ConfigurationWindow
		private void ConfigurationWindow_ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			//save shit in objects
			foreach (Entities.Tab tab in config.Tabs)
			{
				foreach (Entities.Group group in tab.Groups)
				{
					foreach (Entities.Setting setting in group.Settings)
					{
						String elementName = "ConfigurationSetting" + setting.Identifier;
						Object element = FindName(elementName);
						if (element != null)
						{
							//add extra types of controls here
							if (setting.Type == typeof(String))
							{
								setting.Value = ( (TextBox)element ).Text;
							}
							else if (setting.Type == typeof(Boolean))
							{
								setting.Value = ( (CheckBox)element ).IsChecked;
							}
						}
						else
						{
							writeLine("Element " + elementName + " is null");
						}
					}
				}
			}

			//saveshit
			SaveConfigurationToFile();
		}
		private void ConfigurationWindow_ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			//restore shit
			foreach (Entities.Tab tab in config.Tabs)
			{
				foreach (Entities.Group group in tab.Groups)
				{
					foreach (Entities.Setting setting in group.Settings)
					{
						String elementName = "ConfigurationSetting" + setting.Identifier;
						Object element = FindName(elementName);
						if (element != null)
						{
							//add extra types of controls here
							if (setting.Type == typeof(String))
							{
								( (TextBox)element ).Text = (String)setting.Value;
							}
							else if (setting.Type == typeof(Boolean))
							{
								( (CheckBox)element ).IsChecked = (Boolean)setting.Value;
							}
						}
						else
						{
							writeLine("Element " + elementName + " is null");
						}
					}
				}
			}
		}
		#endregion
		#region EventHandlers AboutWindow
		void aboutWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//TODO shit.
		}
		#endregion
		#region ConfigurationWindow HelperFunctions
		private void SaveConfigurationToFile()
		{
			try
			{
				Stream stream = System.IO.File.Open(ConfigurationFilePath.FullName, FileMode.Create);
				BinaryFormatter formatter = new BinaryFormatter();
				ConfigurationSaveHelper configSaveHelper = new ConfigurationSaveHelper(config);
				formatter.Serialize(stream, configSaveHelper);
				stream.Close();
			}
			catch (Exception e)
			{
				writeLine("Error saving config: " + e.Message);
				ExceptionHandler.TriggerException("Error saving config: " + e.Message, ExceptionHandler.ExceptionLevel.Error, e);
				throw e;
			}
		}
		#endregion

		#region EventHandlers
		void Current_Exit(object sender, ExitEventArgs e)
		{
			AbortScan();
			SaveConfigurationToFile();
		}
		private void iTCOM_iTunesQuit()
		{
			scanBackgroundWorkerI.ReportProgress(0, "\niTunes has quit unexpectedly. iTunes scan has been stopped unceremoniously.");
		}
		private void iTCOM_iTunesAboutToPromptUser()
		{
			scanBackgroundWorkerI.ReportProgress(0, "\nTrying to close all connections with iTunes. iTunes scan will be stopped.");
			AbortScan(false, true);
		}
		private void buttonScan_Click(object sender, RoutedEventArgs e)
		{
			if (( (ComboBoxItem)comboBoxSource.SelectedItem ).Content.ToString() == "Folder")
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
			else if (( (ComboBoxItem)comboBoxSource.SelectedItem ).Content.ToString() == "iTunes")
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

		private void mainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateMemoryUsage();
		}

		private void buttonCancel_Click(object sender, RoutedEventArgs e)
		{
			AbortScan();
		}

		private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void MenuItemOptions_Click(object sender, RoutedEventArgs e)
		{
			configurationWindow.Show();
		}
		private void MenuItemSaveLog_Click(object sender, RoutedEventArgs e)
		{
			// create a writer and open the file
			TextWriter tw = new StreamWriter("output.log", false);

			// write a line of text to the file
			tw.Write(textBoxTagLibTest.Text);

			// close the stream
			tw.Close();
		}
		private void MenuItemClearLogView_Click(object sender, RoutedEventArgs e)
		{
			textBoxTagLibTest.Text = "";
			GC.Collect();
			GC.WaitForPendingFinalizers();
			UpdateMemoryUsage();
		}
		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			aboutWindow.Show();
		}
		#endregion
		#region HelperFuntions
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
			if (scanBackgroundWorkerF.IsBusy && F)
			{
				scanBackgroundWorkerF.CancelAsync();
			}
			if (scanBackgroundWorkerI.IsBusy && I)
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
		public static Int32 CalcPercentage(decimal part, decimal whole)
		{
			return (Int32)Math.Round(part / whole * 100);
		}
		private void UpdateMemoryUsage()
		{
			textBlockData.Text = Math.Round((double)GC.GetTotalMemory(true) / 1024 / 1024, 2) + " MB MEM";
		}
		#endregion





	}
}
