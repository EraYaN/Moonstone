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
using SpotiFire;
using System.Security;
using System.Threading.Tasks;

namespace TestAppWPF
{
	/// <summary>
	/// Interaction logic for loginWindow.xaml
	/// </summary>
    public partial class LoginDialog : Window
	{

		Session _session;
        public  LoginDialog(Session session)
        {
            _session = session;
            InitializeComponent();            
        }
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            stateLogin.Text = "Logging in...";
            String username = usernameTextBox.Text;
            String password = passwordTextBox.Password;
            Boolean rememberMe = (bool)rememberCheckBox.IsChecked;
            Error result = await _session.Login(username, password, rememberMe);
           
            if (result == Error.OK)
            {
                DialogResult = true;
                stateLogin.Text = "Logged in.";
                Close();
            }
            else
            {
                //DialogResult = null;
                stateLogin.Foreground = Brushes.Red;
                stateLogin.Text = "Login Error: " + result.ToString(); //TODO use my newly added extension method out of SpotiFire (wait for release)
                passwordTextBox.Clear();
            }
        }
       
	}
}
