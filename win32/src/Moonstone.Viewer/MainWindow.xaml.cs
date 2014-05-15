using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFMediaKit.DirectShow.Controls;

namespace Moonstone.Viewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		ViewModel vm = new ViewModel();
		Matrix transform = Matrix.Identity;
		Matrix transformScale = Matrix.Identity;
		double currentScale = 1;
		double currentX = 0;
		double currentY = 0;
		Point startDragPosition;
		bool dragging;
		Timer t = new Timer(1000);
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = vm;
			t.AutoReset = false;
			t.Elapsed += t_Elapsed;
			mediaElement.RenderTransform = new MatrixTransform();
			mediaElement.LayoutTransform = new MatrixTransform();
		}

		void t_Elapsed(object sender, ElapsedEventArgs e)
		{
			vm.MessageVisible = false;			
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			Quit();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();
			int numargs = args.Count();
			if (numargs > 1)
			{
				try
				{
					FileInfo path = new FileInfo(args[1]);
					if (path.Exists)
					{
						Uri uri = new Uri(path.FullName);
						mediaElement.Source = uri;
					}
					else
					{
						Quit();
					}
				}
				catch
				{
					Quit();
				}

			}
			else
			{
				Quit();
			}
		}
		private void Quit()
		{
			Application.Current.Shutdown();
		}

		private void Element_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scaleamount = e.Delta / 1000.0;			
			currentScale += scaleamount;
			if (currentScale < 0.1)
			{
				currentScale = 0.1;
			}
			if (currentScale > 10)
			{
				currentScale = 10;
			}			
			vm.MessageText = String.Format("Zoom: {0}%",currentScale*100);
			vm.MessageVisible = true;
			t.Stop();
			t.Start();
			//MediaUri el = sender as Control;
			UpdateTransform();
		}

		private void mediaElement_MediaFailed(object sender, WPFMediaKit.DirectShow.MediaPlayers.MediaFailedEventArgs e)
		{
			vm.MessageVisible = true;
			vm.MessageText = "Media Failed.\n" + e.Message + "\n Exiting now.";			
			Application.Current.Shutdown();
		}

		private void mainWindow_MouseUp(object sender, MouseButtonEventArgs e)
		{
			//Quit();
		}

		private void mediaElement_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (dragging && startDragPosition!=null)
				{
					System.Windows.Point position = e.GetPosition(this);
					currentX -= startDragPosition.X - position.X;
					currentY -= startDragPosition.Y - position.Y;
					startDragPosition = position;
					UpdateTransform();
				}
				e.Handled = true;
			}
		}

		private void mediaElement_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
			{
				startDragPosition = e.GetPosition(this);
				dragging = true;
				e.Handled = true;
			}
		}

		private void mediaElement_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Released)
			{
				dragging = false;
				e.Handled = true;
			}
		}

		private void UpdateTransform()
		{
			transform = Matrix.Identity;
			transformScale = Matrix.Identity;
			transformScale.Scale(currentScale, currentScale);
			transform.Translate(currentX, currentY);
			((MatrixTransform)mediaElement.RenderTransform).Matrix = transform;
			((MatrixTransform)mediaElement.LayoutTransform).Matrix = transformScale;
			//System.Diagnostics.Debug.WriteLine("Updated Transform: {0}, {1}, {2}", currentScale, currentX, currentY);
		}

	}
}
