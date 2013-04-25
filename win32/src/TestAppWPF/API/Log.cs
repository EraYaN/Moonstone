using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace TestApp.Spotify
{
	public static class Log
	{
		static public void Debug(string msg, params object[] vals)
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine("Debug: " + msg, vals);
#endif
		}
		static public void Error(string msg, params object[] vals)
		{
			System.Diagnostics.Debug.WriteLine("Error: " + msg, vals);
		}
		static public void Warning(string msg, params object[] vals)
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine("Warning: " + msg, vals);
#endif
		}
	}
}
