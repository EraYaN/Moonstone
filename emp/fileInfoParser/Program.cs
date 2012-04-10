using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMP
{
    class Program
    {
        static void Main()
        {
            fileInfoParser c = new fileInfoParser();

            Console.WriteLine(c);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
