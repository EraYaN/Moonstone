using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
	public static class LibraryHelpers
	{
		public static Boolean AddMovieToLibrary(ref Library library, String filePath)
		{
			Library.MoviesRow movieRow = library.Movies.NewMoviesRow();
			movieRow.BeginEdit();
			//movieRow.MovieKey = null;			
			Int32 MovieKey = movieRow.MovieKey;
			movieRow.Title = filePath;			
			movieRow.EndEdit();
			library.Movies.AddMoviesRow(movieRow);
			movieRow.AcceptChanges();
			return true;
		}
		public static Boolean AddTVShowToLibrary(ref Library library, String filePath)
		{
			Library.TVShowsRow tvShowRow = library.TVShows.NewTVShowsRow();
			tvShowRow.BeginEdit();
			//movieRow.MovieKey = null;			
			Int32 TVShowKey = tvShowRow.TVShowKey;
			tvShowRow.Title = filePath;
			tvShowRow.Episode = 1;
			tvShowRow.Season = 1;
			tvShowRow.EndEdit();
			library.TVShows.AddTVShowsRow(tvShowRow);
			tvShowRow.AcceptChanges();
			return true;
		}
	}
}
