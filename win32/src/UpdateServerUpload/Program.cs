using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace UpdateServerUpload
{
	class Program
	{


		static void Main(string[] args)
		{

			String SolutionName = @"EMP";
			String ProjectName = @"EnhancedMetadataProcessor";
			//String SolutionPath = @"\\SERVER\erwin\Documents\Visual Studio 2010\Projects\" + SolutionName + @"\";
			String SolutionPath = @"F:\Users\" + SolutionName + @"\";
			String ReleasePath = SolutionPath + @"Release\";
			String TargetPath = SolutionPath + ProjectName + @"\bin\Release\";
			String SolutionFile = SolutionPath + SolutionName + @".sln";
			String ReleaseLogFile = SolutionPath + @"ReleaseBuildLog.log";
			
			AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
			Console.Title = assemblyName.Name + " v" + assemblyName.Version.Major + "." + assemblyName.Version.Minor + "." + assemblyName.Version.Build + " by EraYaN";
			//TODO ftp smartness			
			//
			/*IDictionary envVars = Environment.GetEnvironmentVariables();
			foreach (DictionaryEntry de in envVars)
			{
				Console.WriteLine("{0} = {1};",de.Key,de.Value);
			}
			Console.ReadKey(true);
			return;*/
			//end
			Console.WriteLine("Welcome to the release program for " + ProjectName + ".");
			Console.WriteLine("Do you really want to release the current version? [Y/N]");
			ConsoleKeyInfo cki;
			do
			{
				cki = Console.ReadKey(true);
			} while (cki.Key != ConsoleKey.Y && cki.Key != ConsoleKey.N);
			if (cki.Key == ConsoleKey.Y)
			{
				Console.WriteLine("You choose to release the current version.");
				Console.WriteLine("Building release table from files.");
				Console.WriteLine("Path: {0}", ReleasePath);
				if (!Directory.Exists(ReleasePath))
				{
					Directory.CreateDirectory(ReleasePath);
					Console.WriteLine("Created Release path.");
				}
				if (Directory.Exists(ReleasePath))
				{
					//building
					if (File.Exists(ReleaseLogFile))
					{
						File.Delete(ReleaseLogFile);
					}

					String command = "/build Release /project " + ProjectName + " /out \"" + ReleaseLogFile + "\" \"" + SolutionFile + "\"";
					String executable = "devenv";
					Console.WriteLine("Building solution with {0} {1}", executable, command);
					Console.WriteLine();
					Process process = new Process();
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.Arguments = command;
					process.StartInfo.FileName = executable;
					process.Start();
					process.WaitForExit();
					StreamReader logfilereader = new StreamReader(ReleaseLogFile);
					Console.WriteLine("Any output devenv gave:\r\n{0}", logfilereader.ReadToEnd());
					logfilereader.Close();
					TimeSpan duration = process.ExitTime - process.StartTime;
					Console.WriteLine("{0} has exited with code {1} after {2} seconds", executable, process.ExitCode, Math.Round(duration.TotalSeconds, 1));
					if (process.ExitCode == 0)
					{
						Console.WriteLine("Building succesfull.");
						Console.WriteLine("Release Folder Purge");
						//Purge Release folder
						DirectoryInfo di = new DirectoryInfo(ReleasePath);
						FileInfo[] files_purge = di.GetFiles();
						foreach (FileInfo file in files_purge)
						{
							File.Delete(file.FullName);
							Console.WriteLine("Deleted {0}", file.Name);
						}
						Console.WriteLine();
						Console.WriteLine("Builded Files Copy");
						//Copy files
						DirectoryInfo di_copy = new DirectoryInfo(TargetPath);
						FileInfo[] files_copy = di_copy.GetFiles();
						foreach (FileInfo file in files_copy)
						{
							if (Path.GetExtension(file.FullName) != ".exe" && Path.GetExtension(file.FullName) != ".dll")
							{
								continue;
							}
							if (file.FullName.Contains("vshost"))
							{
								continue;
							}
							if (File.Exists(ReleasePath + file.Name))
							{
								File.Delete(ReleasePath + file.Name);
							}
							File.Copy(file.FullName, ReleasePath + file.Name);
							Console.WriteLine("Copied {0}", file.Name);
						}
						Console.WriteLine();
						Console.WriteLine("File Versions");
						//File Versions						
						FileInfo[] files = di.GetFiles();
						foreach (FileInfo file in files)
						{
							AssemblyName an = AssemblyName.GetAssemblyName(file.FullName);
							Console.WriteLine("{0}|{1}|{2}|{3}|{4}", an.Name, an.Version.Major, an.Version.Minor, an.Version.Build, an.Version.Revision);

						}
					}
					else
					{
						Console.WriteLine("Building unsuccesfull.");
					}

				}
				else
				{
					Console.WriteLine("Release path does not exist, aborting.");
				}
			}			
			Console.WriteLine();
			Console.WriteLine("Hit any key to exit.");
			Console.ReadKey(true);
		}
	}
}
