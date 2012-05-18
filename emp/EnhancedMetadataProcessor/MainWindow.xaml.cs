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
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Windows.Threading;
using System.IO.Compression;

namespace EMP
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		BackgroundWorker scanBackgroundWorkerF = new BackgroundWorker(); //Folder Source Scan BackgroundWorker Thread
		BackgroundWorker scanBackgroundWorkerI = new BackgroundWorker(); //iTunes Source Scan BackgroundWorker Thread
		BackgroundWorker updaterBackgroudWorker = new BackgroundWorker(); //Updater BackgroundWorker
		Library library;
		public static String currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		ConfigurationWindow configurationWindow;
		AboutWindow aboutWindow;
		AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
		public Configuration config;
		static FileInfo ConfigurationFilePath = new FileInfo(@"Configuration\Configuration.bin");
		public MainWindow()
		{
			InitializeComponent();
			Directory.SetCurrentDirectory(currentAssemblyDirectoryName);
			Application.Current.Exit += new ExitEventHandler(Current_Exit);
			writeLine("Welcome to the " + assemblyName.Name + " v" + assemblyName.Version.Major + "." + assemblyName.Version.Minor + "." + assemblyName.Version.Build);
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
			updaterBackgroudWorker.WorkerReportsProgress = true;
			updaterBackgroudWorker.WorkerSupportsCancellation = false;
			updaterBackgroudWorker.DoWork += new DoWorkEventHandler(updaterBackgroudWorker_DoWork);
			updaterBackgroudWorker.ProgressChanged += new ProgressChangedEventHandler(updaterBackgroudWorker_ProgressChanged);
			updaterBackgroudWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updaterBackgroudWorker_RunWorkerCompleted);
			#endregion
			#region Config Init
			OpenConfigurationToFile();
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
			#region Library loading
			if ((String)config.GetSetting(Setting.iTunesMediaXMLPath) == "")
			{
				using (iTunesCOM iTCOM = new iTunesCOM())
				{
					config.SetSetting(Setting.iTunesMediaXMLPath, iTCOM.GetiTunesMediaXMLPath());
				}
			}
			OpenLibrary();
			writeLine(String.Format("{0} Movies; {1} TVShows; {2} Mediafiles;", library.Movies.Rows.Count, library.TVShows.Rows.Count, library.MediaFileTargets.Rows.Count));
			#endregion
		}

		#region UpdaterBackGroundWorker

		void updaterBackgroudWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result == null)
			{
				aboutWindow.TextBlockUpdateStatus.Text = "Update check messed up.";
				return;
			}
			if ((Int32)e.Result == 2)
			{
				aboutWindow.TextBlockUpdateStatus.Text = "You have the latest version.";
			}
			else if ((Int32)e.Result == 1)
			{
				aboutWindow.TextBlockUpdateStatus.Text = "There is a newer version available.";
			}
			else if ((Int32)e.Result == 0)
			{
				aboutWindow.TextBlockUpdateStatus.Text = "Something went wrong while checking for updates.";
			}
			else
			{
				aboutWindow.TextBlockUpdateStatus.Text = "Update check messed up.";
			}
		}

		void updaterBackgroudWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.ProgressPercentage == 0 || aboutWindow.TextBlockUpdateStatus.Text.Length > 10)
			{
				aboutWindow.TextBlockUpdateStatus.Text = "";
			}
			else
			{
				aboutWindow.TextBlockUpdateStatus.Text += "-";
			}
		}

		void updaterBackgroudWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Boolean arg = (Boolean)e.Argument;
			WebRequest updateCheckRequest = WebRequest.Create("http://erayan.eu/intelligence/emp/");
			updateCheckRequest.Method = "POST";
			updaterBackgroudWorker.ReportProgress(10);
			String data = "";
			StringBuilder sb = new StringBuilder();
			//add AssemblyVersions
			//EnhancedMetadataProcessor			
			List<AssemblyName> assemblies = new List<AssemblyName>();

			//assemblies.Add(assemblyName);
			DirectoryInfo di = new DirectoryInfo(currentAssemblyDirectoryName);
			FileInfo[] files = di.GetFiles("*.dll");
			updaterBackgroudWorker.ReportProgress(20);
			foreach (FileInfo file in files)
			{
				AssemblyName an = AssemblyName.GetAssemblyName(file.FullName);
				assemblies.Add(an);
			}
			files = di.GetFiles("*.exe");
			updaterBackgroudWorker.ReportProgress(30);
			foreach (FileInfo file in files)
			{
				if (file.Name.Contains("vshost"))
				{
					continue;
				}
				AssemblyName an = AssemblyName.GetAssemblyName(file.FullName);
				assemblies.Add(an);
			}
			updaterBackgroudWorker.ReportProgress(40);
			foreach (AssemblyName an in assemblies)
			{
				sb.AppendFormat("versions[{0}][Major]={1}&", an.Name, an.Version.Major);
				sb.AppendFormat("versions[{0}][Minor]={1}&", an.Name, an.Version.Minor);
				sb.AppendFormat("versions[{0}][Build]={1}&", an.Name, an.Version.Build);
				sb.AppendFormat("versions[{0}][Revision]={1}&", an.Name, an.Version.Revision);
			}
			//String data = "versions[" + assemblyName.Name + "][Major]=" + assemblyName.Version.Major;
			data = sb.ToString();
			Byte[] data_bytes = StrToByteArray(data);
			( (HttpWebRequest)updateCheckRequest ).UserAgent = "EraYaNUpdater (" + assemblyName.Name + " v" + assemblyName.Version.ToString() + ")";
			updateCheckRequest.ContentLength = data_bytes.Length;
			updateCheckRequest.ContentType = "application/x-www-form-urlencoded";
			Stream dataStream = updateCheckRequest.GetRequestStream();
			dataStream.Write(data_bytes, 0, data_bytes.Length);
			dataStream.Close();
			updaterBackgroudWorker.ReportProgress(50);
			WebResponse updateCheckResponse = updateCheckRequest.GetResponse();
			String status = ( (HttpWebResponse)updateCheckResponse ).StatusDescription;
			Stream data_response = updateCheckResponse.GetResponseStream();
			StreamReader reader = new StreamReader(data_response);
			String responseFromServerStatus = reader.ReadLine();
			updaterBackgroudWorker.ReportProgress(70);
			String responseFromServerInfo = reader.ReadToEnd();
			updaterBackgroudWorker.ReportProgress(80);
			reader.Close();
			data_response.Close();
			updateCheckResponse.Close();
			//MessageBox.Show("Response from server: " + responseFromServerStatus + "\nInfo:\n" + responseFromServerInfo + "\n Status: " + status);
			if (responseFromServerStatus != "")
			{
				if (responseFromServerStatus == "latest")
				{
					//MessageBox.Show("Latest version. Response from server: " + responseFromServer + "\n Status: " + status);
					e.Result = 2;
					if (arg)
					{
						//display dialog
						MessageBox.Show("The program is up to date!", "Up to date!", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					updaterBackgroudWorker.ReportProgress(90);
				}
				else
				{
					//MessageBox.Show("Response from server: " + responseFromServer + "\n Status: " + status);
					e.Result = 1;
					if (arg)
					{
						//display dialog
						if (MessageBox.Show("There is a newer version available!\n\nDo you want to go to the website to download it?", "Newer version!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
						{
							//open website			
							Process website_launch = new Process();
							website_launch.StartInfo.FileName = "http://erayan.eu/products/emp/";
							website_launch.Start();
						}
					}
					updaterBackgroudWorker.ReportProgress(90);
				}
			}
			else
			{
				e.Result = 0;
				if (arg)
				{
					//display dialog
					MessageBox.Show("The check messed up.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			updaterBackgroudWorker.ReportProgress(100);
		}
		#endregion
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
			textBoxOutput.ScrollToEnd();
			textBoxOutput.Refresh();
			//UpdateMemoryUsage();
		}

		void scanBackgroundWorkerF_DoWork(object sender, DoWorkEventArgs e)
		{
			DirectoryInfo dirinfo = (DirectoryInfo)e.Argument;
			FileInfo[] files = dirinfo.GetFiles("*.m??", SearchOption.AllDirectories);
			files = dirinfo.GetFiles("*.m??", SearchOption.AllDirectories);
			double count = files.Count();
			double filenum = 0;
			scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), "Count: " + count);
			//Timer
			Stopwatch swProcessTime = new Stopwatch();
			swProcessTime.Start();
			StringBuilder progress = new StringBuilder();
			HelperDictionary helperDictionary = new HelperDictionary();
			foreach (FileInfo file in files)
			{
				if (scanBackgroundWorkerF.CancellationPending)
				{
					break;
				}
				filenum++;
				try
				{
					progress.AppendFormat("\r\n{0}\r\n", file.Name);
					TagLib.File fileTag = TagLib.File.Create(file.FullName);
					//Timer
					Stopwatch swFileTime = new Stopwatch();
					swFileTime.Start();
					//Parse results
					FileInfoParser fileInfoParser = new FileInfoParser(file, helperDictionary);
					//FileInfoParser fileInfoParser = new FileInfoParser(file);
					progress.AppendFormat("Parse result: {0}\n", fileInfoParser);
					progress.AppendFormat("TagType: {0}\r", fileTag.TagTypes.ToString());
					progress.AppendFormat("Title: {0}; Year: {1}\n", fileTag.Tag.Title, fileTag.Tag.Year);
					//Timer output
					swFileTime.Stop();
					TimeSpan fileTime = swFileTime.Elapsed;
					progress.AppendFormat("Parsed in {0}ms.\n", fileTime.TotalMilliseconds);
					fileInfoParser = null;
					fileTag.Dispose();
					fileTag = null;
				}
				catch (UnsupportedFormatException Exception)
				{
					progress.Append("File format not supported.\r\n");
					ExceptionHandler.TriggerException(Exception.Message);
				}
				catch (Exception Exception)
				{
					progress.Append("ERROR processing file.\r\n");
					ExceptionHandler.TriggerException(Exception.Message);
					throw Exception;
				}
				if ((Int32)filenum % (Int32)Math.Round(count / 20) == 0)
				{
					scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), progress.ToString());
					progress.Clear();
				}
			}
			scanBackgroundWorkerF.ReportProgress((int)Math.Round(filenum / count * 100), progress.ToString());
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
			//UpdateMemoryUsage();
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
			RestoreValuesInUI();
		}
		#endregion
		#region EventHandlers AboutWindow
		void aboutWindow_Loaded(object sender, RoutedEventArgs e)
		{
			aboutWindow.TextBlockProductName.Text = assemblyName.Name;
			aboutWindow.TextBlockVersion.Text = "v" + assemblyName.Version.ToString();
			//check for updates
			checkForUpdates(false);
			//update aboutWindow.TextBlockUpdateStatus.Text for progress and result.
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
				ExceptionHandler.TriggerException("Error saving config: " + e.Message, ExceptionLevel.Error, e);
				throw e;
			}
		}
		private void OpenConfigurationToFile()
		{
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
				ExceptionHandler.TriggerException("Error loading config: " + e.Message, ExceptionLevel.Error, e);
				throw e;
			}
			finally
			{
				SaveConfigurationToFile();
			}
		}
		private void RestoreValuesInUI()
		{
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

		#region EventHandlers
		private void MenuItemOptionsRestoreToDefault_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to restore the settings to default?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
			{
				config = new Configuration();
				RestoreValuesInUI();
			}
		}
		void Current_Exit(object sender, ExitEventArgs e)
		{
			AbortScan();
			SaveConfigurationToFile();
			SaveLibrary();
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
			RestoreValuesInUI();
			configurationWindow.Show();
		}
		private void MenuItemSaveLog_Click(object sender, RoutedEventArgs e)
		{
			// create a writer and open the file
			TextWriter tw = new StreamWriter("output.log", false);

			// write a line of text to the file
			tw.Write(textBoxOutput.Text);

			// close the stream
			tw.Close();
		}
		private void MenuItemClearLogView_Click(object sender, RoutedEventArgs e)
		{
			textBoxOutput.Text = "";
			GC.Collect();
			GC.WaitForPendingFinalizers();
			UpdateMemoryUsage();
		}
		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			aboutWindow.Show();
		}
		private void MenuItemCheckForUpdates_Click(object sender, RoutedEventArgs e)
		{
			checkForUpdates(true);
		}
		#endregion
		#region HelperFuntions
		public void SaveLibrary()
		{
			String librarylocation = (String)config.GetSetting(Setting.libraryPath);
			if (!Directory.Exists(Path.GetDirectoryName(librarylocation)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(librarylocation));
			}
			try
			{
				library.RemotingFormat = System.Data.SerializationFormat.Binary;
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream filestream = System.IO.File.Open(librarylocation, FileMode.Create);
				formatter.Serialize(filestream, library);
				filestream.Close();
				writeLine("Library succesfully saved.");
			}
			catch
			{
				writeLine("Error writing library to binary format.");
			}
			try
			{
				library.WriteXml(librarylocation + ".xml", System.Data.XmlWriteMode.IgnoreSchema);
			}
			catch
			{
				writeLine("Error writing library to XML backup format.");
			}
		}
		public void OpenLibrary()
		{
			library = new Library();
			String librarylocation = (String)config.GetSetting(Setting.libraryPath);
			//load from bin
			if (System.IO.File.Exists(librarylocation))
			{
				try
				{
					library.RemotingFormat = System.Data.SerializationFormat.Binary;
					FileStream stream = System.IO.File.Open(librarylocation, FileMode.Open);
					//GZipStream gzipstream = new GZipStream(stream, CompressionMode.Decompress);
					BinaryFormatter formatter = new BinaryFormatter();
					//library = (Library)formatter.Deserialize(gzipstream);
					library = (Library)formatter.Deserialize(stream);
					stream.Close();
					writeLine("Library succesfully loaded.");
					return;
				}
				catch
				{
					writeLine("Error reading library from binary format.");
				}

			}
			// load from xml
			if (System.IO.File.Exists(librarylocation + ".xml"))
			{
				try
				{
					library.ReadXml(librarylocation + ".xml", System.Data.XmlReadMode.Auto);
					writeLine("Library succesfully loaded from XML backup format.");
				}
				catch
				{
					writeLine("Error reading library from XML backup format.");
				}
			}
		}
		public void checkForUpdates(Boolean displayDialog)
		{
			if (!updaterBackgroudWorker.IsBusy)
				updaterBackgroudWorker.RunWorkerAsync(displayDialog);
		}
		public static Byte[] StrToByteArray(string str)
		{
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
			return encoding.GetBytes(str);
		}
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
			textBoxOutput.Text += line + "\r\n";
		}
		public void writeLine()
		{
			textBoxOutput.Text += "\r\n";
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
		#region Dummy adders
		private void MenuItemDIOma_Click(object sender, RoutedEventArgs e)
		{
			LibraryHelpers.AddMovieToLibrary(ref library, "oma.mp4");
		}
		private void MenuItemDIRandom_Click(object sender, RoutedEventArgs e)
		{
			for (int I = 0; I < 1000; I++)
			{
				LibraryHelpers.AddMovieToLibrary(ref library, "Random" + I + "_" + DateTime.Now.Ticks + ".mp4");
			}
		}
		#endregion

		private void MenuItemClearLibrary_Click(object sender, RoutedEventArgs e)
		{
			library.Clear();
		}
	}
}
