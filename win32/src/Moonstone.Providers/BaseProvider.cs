using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moonstone.Providers
{
	class BaseProvider : IProvider
	{
		private bool _disposed = false;

		public BaseProvider()
		{

		}

		~BaseProvider()
		{
			Dispose(false);
		}
		
		// Implement IDisposable. 
		// Do not make this method virtual. 
		// A derived class should not be able to override this method. 
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method. 
			// Therefore, you should call GC.SupressFinalize to 
			// take this object off the finalization queue 
			// and prevent finalization code for this object 
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		// Dispose(bool disposing) executes in two distinct scenarios. 
		// If disposing equals true, the method has been called directly 
		// or indirectly by a user's code. Managed and unmanaged resources 
		// can be disposed. 
		// If disposing equals false, the method has been called by the 
		// runtime from inside the finalizer and you should not reference 
		// other objects. Only unmanaged resources can be disposed. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources. 
				if (disposing)
				{
					// Dispose managed resources.
					
				}

				// Call the appropriate methods to clean up 
				// unmanaged resources here. 
				// If disposing is false, 
				// only the following code is executed.
				// Dispose unmanaged resources.
				// set the object references to null

				// Note disposing has been done.
				_disposed = true;

			}
		}
	}
}
