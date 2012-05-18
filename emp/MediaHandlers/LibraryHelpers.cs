using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
	public static class LibraryHelpers
	{
		public static Boolean AddMovieToLibrary(Library library, String filePath)
		{
			Library.MoviesRow movieRow = library.Movies.NewMoviesRow();
			movieRow.BeginEdit();

			movieRow.EndEdit();
			movieRow.AcceptChanges();
			return false;
		}
		public static Boolean AddTVShowToLibrary(Library library, String filePath)
		{
			return false;
		}
	}
}
