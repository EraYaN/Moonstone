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
    /// Copyright (c) 2012 Erwin de Haan. All rights reserved.
    /// </summary>
    public partial class MainWindow : Window
    {
        double _height = 100.0;
        double _width = 100.0;
        static string path = @"Vaarweerstand";
        DirectoryInfo dir = new DirectoryInfo(path);        
        public double[] avgp;
        public double[] avgv;
        public double[] avga;
        public double[] avgw;
        public double[] stdevp;
        public double[] stdevv;
        public double[] stdeva;
        double yMax_norm = 0.9;
        double yMin_norm = -0.2;
        double yMax_nul = 2;
        double yMin_nul = -0.75;
        double yMax_w = 8;
        double yMin_w = 6;
        public bool wrijvinggrafiek = false;
        ArrayList datafiles = new ArrayList();
        static int count = 0;
        static int maxframes = 0;
        public static bool busy = false;
        //FileInfo[] files;
        public MainWindow()
        {
            InitializeComponent();
            richTextBox.AppendText("Started!\r");
            textBlockUnits.Text = "Units (yMax; yMin); Wrijving: (" + yMax_w.ToString() + "; " + yMin_w.ToString() + "); Nul-meting: (" + yMax_nul.ToString() + "; " + yMin_nul.ToString() + "); Others: (" + yMax_norm.ToString() + "; " + yMin_norm.ToString() + ")";
        }
        private void eventUpdate(object sender, RoutedEventArgs e)
        {
            if (!busy)
            {
                busy = true;
                buttonGo.IsEnabled = false;
                int c_number = 0;               
               
                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    FileInfo[] files = di.GetFiles("*.txt", SearchOption.AllDirectories);
                    richTextBox.AppendText("\r" + di.Name + ":\r");
                    foreach (FileInfo file in files)
                    {
                        richTextBox.AppendText(file.Name + "\r");
                    }

                    if (files.Count() > 0)
                    {
                        Canvas canvas = new Canvas();
                        int boot = 0;
                        if (di.Name == "Nul-meting")
                        {
                            canvas = canvas0;
                            boot = 0;
                        }
                        else if (di.Name == "Puddle Jumper I")
                        {
                            canvas = canvas1;
                            boot = 1;
                        }
                        else if (di.Name == "Puddle Jumper II")
                        {
                            canvas = canvas2;
                            boot = 2;
                        }
                        else if (di.Name == "Puddle Jumper III")
                        {
                            canvas = canvas3;
                            boot = 3;
                        }
                        else if (di.Name == "Puddle Jumper IV")
                        {
                            canvas = canvas4;
                            boot = 4;
                        }
                        proccessfiles(files, canvas, boot);
                        c_number++;
                    }
                }
                busy = false;
                buttonGo.IsEnabled = true;
            }
        }
        public void proccessfiles(FileInfo[] files, Canvas canvas, int boot = 0)
        {
            maxframes = 0;
            double massa = 1.289;
            switch (boot)
            {
                case 1:
                massa += 0.177;
                break;
                case 2:
                massa += 0.245;
                break;
                case 3:
                massa += 0.206;
                break;
                case 4:
                massa += 0.128;
                break;
                default:
                massa += 0.128;
                break;
            }
            double yMax = yMax_norm;
            double yMin = yMin_norm;
            if (boot == 0)
            {
                yMax = yMax_nul;
                yMin = yMin_nul;
            }
            double Ft = massa * 9.81 / 2;
            _height = canvas.ActualHeight;
            _width = canvas.ActualWidth;
            canvas.Children.Clear();
            foreach (FileInfo file in files)
            {
                DataFile data = parseFile(file);
                datafiles.Add(data);
                data.process();
                prepareCanvas(canvas, yMax, yMin);                
                if (!wrijvinggrafiek)
                {
                    prepareCanvas(canvasW, yMax_w, yMin_w);
                    wrijvinggrafiek = true;
                }
                richTextBox.AppendText(data.size + " frames plotted\r");
                if (maxframes < data.size)
                    maxframes = data.size;
                count++;
            }
            avgp = new double[maxframes];
            avgv = new double[maxframes];
            avga = new double[maxframes];
            avgw = new double[maxframes];
            stdevp = new double[maxframes];
            stdevv = new double[maxframes];
            stdeva = new double[maxframes];
            int filecount = datafiles.Count;
            double[][] avgptmp = new double[maxframes][];
            double[][] avgvtmp = new double[maxframes][];
            double[][] avgatmp = new double[maxframes][];
            for (int frame = 0; frame < maxframes; frame++)
            {
                avgptmp[frame] = new double[filecount];
                avgvtmp[frame] = new double[filecount];
                avgatmp[frame] = new double[filecount];
            }
            for (int id = 0; id < datafiles.Count; id++)
            {
                DataFile data = ((DataFile)datafiles[id]);
                for (int frame = 0; frame < data.size; frame++)
                {
                    avgptmp[frame][id] = data.position[frame];
                    avgvtmp[frame][id] = data.velocity[frame];
                    avgatmp[frame][id] = data.acceleration[frame];
                }
            }
            List<double> blah_v_1 = new List<double>();
            List<double> blah_v_2 = new List<double>();
            List<double> blah_a_1 = new List<double>();
            List<double> blah_a_2 = new List<double>();
            double[] blah_a = new double[maxframes / 2];
            double[] stdeva_lijn_boven = new double[maxframes];
            double[] stdeva_lijn_onder = new double[maxframes];
            double[] stdevv_lijn_boven = new double[maxframes];
            double[] stdevv_lijn_onder = new double[maxframes];
            double[] stdevp_lijn_boven = new double[maxframes];
            double[] stdevp_lijn_onder = new double[maxframes];
            for (int frame = 0; frame < maxframes; frame++)
            {
                avgp[frame] = avgptmp[frame].Average();
                avgv[frame] = avgvtmp[frame].Average();
                avga[frame] = avgatmp[frame].Average();
                double[] afstandena = new double[avgatmp[frame].Count()];
                for(int value = 0;value<avgatmp[frame].Count();value++){
                    afstandena[value] = Math.Abs(avgatmp[frame][value]-avga[frame]);
                }
                stdeva[frame] = afstandena.Average();

                double[] afstandenv = new double[avgvtmp[frame].Count()];
                for (int value = 0; value < avgvtmp[frame].Count(); value++)
                {
                    afstandenv[value] = Math.Abs(avgvtmp[frame][value] - avgv[frame]);
                }
                stdevv[frame] = afstandenv.Average();

                double[] afstandenp = new double[avgptmp[frame].Count()];
                for (int value = 0; value < avgptmp[frame].Count(); value++)
                {
                    afstandenp[value] = Math.Abs(avgptmp[frame][value] - avgp[frame]);
                }
                stdevp[frame] = afstandenp.Average();

                stdeva_lijn_boven[frame] = avga[frame] + stdeva[frame];
                stdeva_lijn_onder[frame] = avga[frame] - stdeva[frame];

                stdevv_lijn_boven[frame] = avgv[frame] + stdevv[frame];
                stdevv_lijn_onder[frame] = avgv[frame] - stdevv[frame];

                stdevp_lijn_boven[frame] = avgp[frame] + stdevp[frame];
                stdevp_lijn_onder[frame] = avgp[frame] - stdevp[frame];

                avgw[frame] = Ft - (massa * avga[frame]);
                if (frame < maxframes / 4)
                {
                    blah_v_1.Add(avgv[frame]);
                    blah_a_1.Add(avga[frame]);
                }
                else
                {
                    blah_v_2.Add(avgv[frame]);
                    blah_a_2.Add(avga[frame]);
                }
            }
            richTextBox.AppendText("Gemiddeldes (p,v1,v2,a1,a2,w,stdev): (" + avgp.Average() + ", " + blah_v_1.Average() + ", " + blah_v_2.Average() + ", " + blah_a_1.Average() + ", " + blah_a_2.Average() + ", " + avgw.Average() + ", " + stdeva.Average() + ")");

            
            Polyline plavgp = MakeGraph(avgp, yMax, yMin);
            Polyline plstdevp_b = MakeGraph(stdevp_lijn_boven, yMax, yMin);
            Polyline plstdevp_o = MakeGraph(stdevp_lijn_onder, yMax, yMin);
            Polyline plavgv = MakeGraph(avgv, yMax, yMin);
            Polyline plstdevv_b = MakeGraph(stdevv_lijn_boven, yMax, yMin);
            Polyline plstdevv_o = MakeGraph(stdevv_lijn_onder, yMax, yMin);
            Polyline plavga = MakeGraph(avga, yMax, yMin);
            Polyline plstdeva_b = MakeGraph(stdeva_lijn_boven, yMax, yMin);
            Polyline plstdeva_o = MakeGraph(stdeva_lijn_onder, yMax, yMin);
            Polyline plavgw = MakeGraph(avgw, yMax_w, yMin_w);           
            plavgp.Stroke = System.Windows.Media.Brushes.DarkBlue;
            plavgp.StrokeThickness = 2.5;
            plstdevp_b.Stroke = System.Windows.Media.Brushes.DarkBlue;
            plstdevp_b.StrokeThickness = 1;
            plstdevp_o.Stroke = System.Windows.Media.Brushes.DarkBlue;
            plstdevp_o.StrokeThickness = 1;

            plavgv.Stroke = System.Windows.Media.Brushes.DarkRed;
            plavgv.StrokeThickness = 2.5;
            plstdevv_b.Stroke = System.Windows.Media.Brushes.DarkRed;
            plstdevv_b.StrokeThickness = 1;
            plstdevv_o.Stroke = System.Windows.Media.Brushes.DarkRed;
            plstdevv_o.StrokeThickness = 1;

            plavga.Stroke = System.Windows.Media.Brushes.DarkGreen;
            plavga.StrokeThickness = 2.5;
            plstdeva_b.Stroke = System.Windows.Media.Brushes.DarkGreen;
            plstdeva_b.StrokeThickness = 1;
            plstdeva_o.Stroke = System.Windows.Media.Brushes.DarkGreen;
            plstdeva_o.StrokeThickness = 1;

            switch (boot)
            {
                case 0:
                    plavgw.Stroke = System.Windows.Media.Brushes.DarkGray;
                break;
                case 1:
                    plavgw.Stroke = System.Windows.Media.Brushes.Blue;
                break;
                case 2:
                    plavgw.Stroke = System.Windows.Media.Brushes.Green;
                break;
                case 3:
                    plavgw.Stroke = System.Windows.Media.Brushes.Red;
                break;
                case 4:
                    plavgw.Stroke = System.Windows.Media.Brushes.Orange;
                break;
                default:
                    plavgw.Stroke = System.Windows.Media.Brushes.DarkGoldenrod;
                break;
            }           
            plavgw.StrokeThickness = 1.5;
            canvas.Children.Add(plavgp);
            canvas.Children.Add(plstdevp_b);
            canvas.Children.Add(plstdevp_o);
            canvas.Children.Add(plavgv);
            canvas.Children.Add(plstdevv_b);
            canvas.Children.Add(plstdevv_o);
            canvas.Children.Add(plavga);
            canvas.Children.Add(plstdeva_b);
            canvas.Children.Add(plstdeva_o);
            canvasW.Children.Add(plavgw);
            richTextBox.AppendText("\n Max Frames: " + maxframes + "\r");
            richTextBox.ScrollToEnd();
            datafiles.Clear();
        }
        public void prepareCanvas(Canvas c, double iMaxValue = 1.5, double iMinValue = -1.5)
        {
            double heigthValue = iMaxValue - iMinValue;
            double h = c.ActualHeight;
            double w = c.ActualWidth;
            //x-axis
            Line x = new Line();
            double Y0 = h / heigthValue * (heigthValue - (0 - iMinValue)) ;
            x.SnapsToDevicePixels = true;
            x.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            x.X1 = 0;
            x.X2 = w-10;
            x.Y1 = Y0;
            x.Y2 = Y0;
            x.Stroke = System.Windows.Media.Brushes.Black;
            x.StrokeThickness = 1.0;
            if (Y0 <= h+10)
            {
                c.Children.Add(x);
            }
            //y-axis
            Line y = new Line();  
            y.SnapsToDevicePixels = true;
            y.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            y.X1 = 1;
            y.X2 = 1;
            y.Y1 = h/20;
            y.Y2 = h-h/20;
            y.Stroke = System.Windows.Media.Brushes.Black;
            y.StrokeThickness = 1.0;
            c.Children.Add(y);            
        }
        public Polyline MakeGraph(double[] iData, double iMaxValue = 1.5, double iMinValue = -1.5, string Name = "Graph")
        {
            int iPoints = iData.Length;
            double heigthValue = iMaxValue - iMinValue;            
            double dStepX = _width / 100;
            System.Windows.Point[] iP = new System.Windows.Point[iPoints];
            for (int i = 0; i < iPoints; i++)
            {
                iP[i].X = i * dStepX;                
                iP[i].Y = _height / heigthValue * (heigthValue-(iData[i]-iMinValue));
            }            
            Polyline oPLine = new Polyline();
            oPLine.Name = Name;
            HSLColor hslcolor = new HSLColor((count*10)%240,240.0,120.0);
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = hslcolor;
            oPLine.Stroke = brush;
            oPLine.StrokeThickness = 1;
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
            string nulmeting = "Nul-meting";
            if (file.DirectoryName.Substring(file.DirectoryName.Length - nulmeting.Length, nulmeting.Length) == nulmeting)
            {
                datafile.fps = 25.0;                
            }
            richTextBox.AppendText("File is "+datafile.fps+" fps\r");
            return datafile;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            richTextBox.ScrollToEnd();
            
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            eventUpdate(sender, new RoutedEventArgs());
        }
    }
    public class DataFile
    {
        public double[] position;
        public double[] scale;        
        public double[] velocity;
        public double[] acceleration;
        public int size;
        public double fps = 30.0;
        public DataFile(int _size)
        {
            size = _size;
            position = new double[size];
            scale = new double[size];            
            velocity = new double[size];
            acceleration = new double[size];
        }
        public void process()
        {
            for (int I = 0; I < size; I++)
            {
                scale[I] = scale[I]/100;
                position[I] = position[I] * (scale[I]) * 0.000670771937541236;
                if (I > 0)
                {                   
                    velocity[I] = -(position[I] - position[I - 1]) * fps;
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
