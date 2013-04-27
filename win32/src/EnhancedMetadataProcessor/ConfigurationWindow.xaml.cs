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
using System.Windows.Shapes;

namespace EMP
{
	/// <summary>
	/// Interaction logic for ConfigurationWindow.xaml
	/// </summary>
	public partial class ConfigurationWindow : Window
	{
		public ConfigurationWindow()
		{
			InitializeComponent();
		}
		private void configurationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}
		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}
	}
}
