using System;
using System.Drawing;
using System.Drawing.Printing;
using PC;

namespace ExamenStunt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowHeight = 10;
            Console.Title = "PrintingDaemon";
            Console.WriteLine("Started");
            for (int I = 0; I < 10; I++)
            {
                PrintDocument();
            }                
            Console.ReadKey(true);
            
        }
        
        public static void PrintDocument()
        {
            //Create an instance of our printer class
            PCPrint printer = new PCPrint();
            //Set the font we want to use
            printer.PrinterFont = new Font("Verdana", 10);
            //Set the TextToPrint property
            printer.TextToPrint = "";
            //Issue print command
            printer.Print();
        }
        
    }
    
}
