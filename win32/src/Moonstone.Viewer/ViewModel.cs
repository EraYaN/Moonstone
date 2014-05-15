using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonstone.Viewer
{
	class ViewModel : INotifyPropertyChanged
	{
		private string messageText;
		private bool messageVisible;
		// Declare the event 
		public event PropertyChangedEventHandler PropertyChanged;
		
		public string MessageText
		{
			get { return messageText; }
			set
			{
				messageText = value;
				// Call OnPropertyChanged whenever the property is updated
				OnPropertyChanged("MessageText");
			}
		}
		public bool MessageVisible
		{
			get { return messageVisible; }
			set
			{
				messageVisible = value;
				// Call OnPropertyChanged whenever the property is updated
				OnPropertyChanged("MessageVisible");
			}
		}
		// Create the OnPropertyChanged method to raise the event 
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
