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
using TestApp.Spotify;

namespace TestAppWPF
{
	public delegate void LoggedInEventHandler(object sender, EventArgs e);
	/// <summary>
	/// Interaction logic for loginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{

		public event LoggedInEventHandler LoggedIn;
		public LoginWindow()
		{
			InitializeComponent();
		}
		protected virtual void OnLoggedIn(EventArgs e)
		{
			if (LoggedIn != null)
				LoggedIn(this, e);
		}
		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string username, password;
				username = usernameTextBox.Text.Trim();
				password = passwordTextBox.Password;
				if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
				{
					MessageBox.Show("Please enter a valid username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				if (Spotify.Login(Configuration.appkey, username, password))
				{
					this.Hide();
					OnLoggedIn(EventArgs.Empty);
					// Session.Login();
					//Spotify.GetUserDisplayName();                    
				}
				else
				{
					MessageBox.Show("Can't log in!");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Login Exception\n\n" + ex.ToString());
			}
		}
	}
}
