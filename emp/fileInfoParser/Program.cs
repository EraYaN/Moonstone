using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
    class Program
    {
        static void Main()
        {
            FileInfo fileInfo = new FileInfo(@"\\SERVER\Users\Admin\Videos\Movies\District 9\District 9 (2009)\District.9.2009.720p.BrRip.YIFY.mkv");
            fileInfoParser c = new fileInfoParser(fileInfo);

            Console.WriteLine(c);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
