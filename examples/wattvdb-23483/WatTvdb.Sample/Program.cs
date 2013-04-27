using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatTvdb.V1;

namespace WatTvdb.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Tvdb api = new Tvdb("apikey");
            var mirrors = api.GetMirrors();

            Console.Write("Enter TV Series: ");
            var search = Console.ReadLine();
            if (string.IsNullOrEmpty(search))
                return;

            Console.WriteLine("Synchronous call...");
            {
                var result = api.SearchSeries(search);
                Console.WriteLine(string.Format("{0} matches found", result.Count));

                foreach (var item in result)
                {
                    Console.WriteLine(string.Format("{0} #{1}", item.SeriesName, item.id));
                }
            }

            Console.WriteLine("Asynchronous call...");
            {
                api.SearchSeries(search, "Sample async call", result =>
                    {
                        Console.WriteLine(result.UserState as string);

                        Console.WriteLine(string.Format("{0} matches found", result.Data.Count));

                        foreach (var item in result.Data)
                        {
                            Console.WriteLine(string.Format("{0} #{1}", item.SeriesName, item.id));
                        }
                    });
            }


            Console.WriteLine("Press any key...");
            Console.Read();
        }
    }
}
