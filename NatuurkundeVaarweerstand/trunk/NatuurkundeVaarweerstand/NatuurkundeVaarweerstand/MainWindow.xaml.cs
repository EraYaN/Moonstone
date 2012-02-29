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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Drawing;

namespace NatuurkundeVaarweerstand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double _height = 100.0;
        double _width = 100.0;
        static string path = @"\\SERVER\erwin\Documents\School\Natuurkunde\Vaarweerstand\Puddle Jumper IV";
        DirectoryInfo dir = new DirectoryInfo(path);
        static double fps = 30.0;
        static int count = 0;
        static int maxframes = 0;
        FileInfo[] files;
        public MainWindow()
        {
            InitializeComponent();
            files = dir.GetFiles("*.txt",SearchOption.AllDirectories);
            richTextBox.AppendText("Started!\r");
            foreach (FileInfo file in files)
            {
                richTextBox.AppendText(file.Name + "\r");
            }            
            
        }

        private void buttonGo_Click(object sender, RoutedEventArgs e)
        {
            _height = canvas.ActualHeight;
            _width = canvas.ActualWidth;
            canvas.Children.Clear();
            foreach (FileInfo file in files)
            {
                DataFile data = parseFile(file);
                data.process();
                Polyline pl = MakeGraph(data.position, 1);
                Polyline zero = MakeGraph(new double[100], 1);
                Polyline pls = MakeGraph(data.scale,1);
                //Polyline plr = MakeGraph(data.real,1);
                Polyline plv = MakeGraph(data.velocity, 1);
                Polyline pla = MakeGraph(data.acceleration,1);
                zero.Stroke = System.Windows.Media.Brushes.Black;
                zero.StrokeThickness = 2.0;                
                plv.StrokeDashArray = new DoubleCollection(new double[] { 5, 2 });
                pla.StrokeDashArray = new DoubleCollection(new double[] { 20, 5 });
                canvas.Children.Add(pl);
                canvas.Children.Add(pls);
                //canvas.Children.Add(plr);
                canvas.Children.Add(plv);
                canvas.Children.Add(pla);
                canvas.Children.Add(zero);
                //richTextBox.AppendText(Double.Join(", \r",data.velocity+"\n"));
                for (int i = 0; i < data.size; i++)
                {
                    richTextBox.AppendText(i + " p:" + data.position[i] + " s:" + data.scale[i] + " v:" + data.velocity[i] + " a:" + data.acceleration[i] + "\r");
                }
                richTextBox.AppendText(String.Join(", \r", data.acceleration + "\n"));
                richTextBox.AppendText(data.size + " frames plotted\r");
                if (maxframes < data.size)
                    maxframes = data.size;
                
                count++;
            }
            richTextBox.AppendText("\n Max Frames: "+maxframes+"\r");
            richTextBox.ScrollToEnd();
        }
        public Polyline MakeGraph(double[] iData, double dVerticalScale, string Name = "Graph")
        {
            // Get maximum value in data.
            double iMaxValue = iData.Max();
            iMaxValue = 2;
            double iMinValue = iData.Min();
            iMinValue = -1;
            // Make points for the Polyline.
            int iPoints = iData.Length;            // Number of points on x-axis.
            double dScale = (_height / iMaxValue) * dVerticalScale;
            double dStepX = _width / /*iPoints*/ 100;      // Distance between divisions on x-axis.
            System.Windows.Point[] iP = new System.Windows.Point[iPoints];       // Points for the Polyline.
            for (int i = 0; i < iPoints; i++)
            {
                iP[i].X = i * dStepX;
                iP[i].Y = _height - (iData[i] * dScale)-_height/2;
            }

            // Make a new Polyline.
            Polyline oPLine = new Polyline();
            oPLine.Name = Name;
            HSLColor hslcolor = new HSLColor((count*20)%240,240.0,120.0);
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = hslcolor;
            oPLine.Stroke = brush;
            oPLine.StrokeThickness = 1.5;
            oPLine.Fill = System.Windows.Media.Brushes.Transparent;
            oPLine.StrokeStartLineCap = PenLineCap.Round;
            oPLine.StrokeEndLineCap = PenLineCap.Round;
            oPLine.StrokeLineJoin = PenLineJoin.Round;
            for (int i = 0; i < iP.Length; i++)
            {
                oPLine.Points.Add(iP[i]);
            }

            return oPLine;
        }
        public DataFile parseFile(FileInfo file)
        {
            ArrayList tmp_position = new ArrayList();
            ArrayList tmp_scale = new ArrayList();
            using(TextReader stream = file.OpenText()){
                string line;
                bool pos = false;
                bool scl = false;
                while((line = stream.ReadLine()) != null)
                {
                                     
                   if (pos && line.Trim() == "")
                   {
                       pos = false;
                   }
                   if (pos)
                   {
                       string[] arr = line.Trim().Split(new char[] {'\t'},StringSplitOptions.RemoveEmptyEntries);
                       string message = "";
                       foreach (String str in arr)
                       {
                           message += str + "; ";

                       }                       

                       //richTextBox.AppendText(message + " - " + double.Parse(arr[1], CultureInfo.InvariantCulture) + "\r");
                       tmp_position.Add(double.Parse(arr[1], CultureInfo.InvariantCulture));
                       
                   }
                   if(line.Trim() == "Position"){                    
                    pos = true;
                    stream.ReadLine();
                   }
                   if (scl && line.Trim() == "")
                   {
                       scl = false;
                   }
                   if (scl)
                   {
                       string[] arr = line.Trim().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                       string message = "";
                       foreach (String str in arr)
                       {
                           message += str + "; ";

                       }

                       //richTextBox.AppendText(message + " - " + double.Parse(arr[1], CultureInfo.InvariantCulture) + "\r");
                       tmp_scale.Add(double.Parse(arr[1], CultureInfo.InvariantCulture));

                   }
                   if (line.Trim() == "Scale")
                   {
                       scl = true;
                       stream.ReadLine();
                   }  
                }
            }
            DataFile datafile = new DataFile(tmp_position.Count);
            for (int i = 0; i < tmp_position.Count; i++)
            {
                datafile.position[i] = (double)tmp_position[i];
                datafile.scale[i] = (double)tmp_scale[i];
            }            
            return datafile;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            richTextBox.ScrollToEnd();
            
        }
    }
    public class DataFile
    {
        public double[] position;
        public double[] scale;
        //public double[] real;
        public double[] velocity;
        public double[] acceleration;
        public int size;
        static double fps = 30.0;
        public DataFile(int _size)
        {
            size = _size;
            position = new double[size];
            scale = new double[size];
            //real = new double[size];
            velocity = new double[size];
            acceleration = new double[size];
        }
        public void process()
        {
            for (int I = 0; I < size; I++)
            {
                position[I] = position[I] * (scale[I] / 100) / 1200 * 0.8;
                if (I > 0)
                {
                   /*double[] tmp = { (real[I] - real[I - 1]), (real[I + 1]- real[I]) };
                    velocity[I] = tmp.Average();
                    double[] tmp2 = { (velocity[I] - velocity[I - 1]),  (velocity[I + 1] - velocity[I])};
                    acceleration[I] = tmp2.Average();
                    /*velocity[I] = (real[I] - real[I - 1]) - (real[I] - real[I + 1]) * -1;
                    
                    acceleration[I] = ( velocity[I] - velocity[I - 1]) -( velocity[I] - velocity[I + 1]) ;*/
                    velocity[I] = (position[I] - position[I - 1]) * fps;
                    acceleration[I] = (velocity[I] - velocity[I - 1]);
                }                
                else
                {
                    velocity[I] = 0;
                    acceleration[I] = 0;
                }                
            }
        }
    }
}
