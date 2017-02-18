using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Drawing;
using System.Diagnostics;


namespace galgen
{
    public class Program
    {
        public static void Shuffle<T>(IList<T> list, Random rnd)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void GenJulSet(List<Tuple<int, int>> vect, int w, int h, int maxIter, double zoom, Random rnd, ref int randh, ref int randw, ref int starcount, ref Complex c)
        {
            // вспомогательные переменные для приведения координат
            double zoomkoorh = Math.Abs(h - 2 * h * zoom);
            double zoomkoorw = Math.Abs(w - 2 * w * zoom);
            startofcycle:
            vect.Clear();
            // рандомим произвольную постоянную
            double c1 = rnd.NextDouble();
            double c2 = rnd.NextDouble();
            if (rnd.Next(2) == 1)
            {
                c1 = c1 * (-1);
            }
            if (rnd.Next(2) == 1)
            {
                c2 = c2 * (-1);
            }
            c = new Complex(c1, c2);
            // вспомогательные переменные для приведения координат
            double r = 0.5 * (1 + Math.Sqrt(1 + 4 * Complex.Abs(c)));
            double xStep = 2 * r / w;
            double yStep = 2 * r / h;
            // число звезд
            starcount = 0;
            // рандомный сдвиг, если нужен
            randw = rnd.Next(-w / 2, w / 2);
            randh = rnd.Next(-h / 2, h / 2);
            //randw = 0;
            //randh = 0;
            // чтобы убивать повторяющиеся
            double predx = 0;
            int povtor = 0;
            bool eptline = true;
            for (double y = h - h * zoom + randh; y < h * zoom + randh; y++)
            {
                for (double x = w - w * zoom + randw; x < w * zoom + randw; x++)
                {
                    int i = maxIter;
                    Complex z = new Complex(-r + x * xStep, -r + y * yStep);
                    while (Complex.Abs(z) < r && i != 0)
                    {
                        z = (z * z + c);
                        --i;
                    }
                    if (i == 0)
                        eptline = false;
                }
                if (eptline)
                    goto startofcycle;

            }
            for (double x = h - h * zoom + randw; x < h * zoom + randw; x++)
            {
                for (double y = w - w * zoom + randh; y < w * zoom + randh; y++)
                {
                    int i = maxIter;
                    Complex z = new Complex(-r + x * xStep, -r + y * yStep);
                    while (Complex.Abs(z) < r && i != 0)
                    {
                        z = (z * z + c);
                        --i;
                    }
                    if (i == 0)
                        eptline = false;
                }
                if (eptline)

                    goto startofcycle;

            }
            for (double y = h * zoom + randh; y > h - h * zoom + randh; y--)
            {
                for (double x = w - w * zoom + randw; x < w * zoom + randw; x++)
                {
                    int i = maxIter;
                    Complex z = new Complex(-r + x * xStep, -r + y * yStep);
                    while (Complex.Abs(z) < r && i != 0)
                    {
                        z = (z * z + c);
                        --i;
                    }
                    if (i == 0)
                        eptline = false;
                }
                if (eptline)
                    goto startofcycle;

            }
            for (double x = h * zoom + randw; x > h - h * zoom + randw; x--)
            {
                for (double y = w - w * zoom + randh; y < w * zoom + randh; y++)
                {
                    int i = maxIter;
                    Complex z = new Complex(-r + x * xStep, -r + y * yStep);
                    while (Complex.Abs(z) < r && i != 0)
                    {
                        z = (z * z + c);
                        --i;
                    }
                    if (i == 0)
                        eptline = false;
                }
                if (eptline)
                    goto startofcycle;

            }
            int yy = 0;
            for (double y = h - h * zoom + randh; y < h * zoom + randh; y++)
            {
                int xx = 0;
                for (double x = w - w * zoom + randw; x < w * zoom + randw; x++)
                {
                    int i = maxIter;
                    Complex z = new Complex(-r + x * xStep, -r + y * yStep);
                    while (Complex.Abs(z) < r && i != 0)
                    {
                        z = (z * z + c);
                        --i;
                    }
                    if (i == 0)
                    {
                        if (povtor > 0 && predx == x - 1)
                            povtor--;
                        else
                        {
                            if (predx == x - 1)
                                povtor = 2;
                            else
                                povtor = 0;
                        }
                        predx = x;
                        if (povtor == 0)
                        {
                            vect.Add(new Tuple<int, int>(xx, yy));
                            starcount++;
                        }
                    }
                    xx++;
                }

                if (starcount > 3000)
                    goto startofcycle;

                yy++;
            }
            // отсекаем слишком полные и слишком пустые
            if (starcount > 3000 || starcount < 50)
            {
                goto startofcycle;
            }
            Console.WriteLine(vect.Count);
            //exporttoconsole(vect);
            //if (Console.ReadKey().Key == ConsoleKey.Enter)
            //    goto startofcycle;
        }
        public static void exporttoconsole(List<Tuple<int, int>> vect)
        {
            Console.Clear();
            foreach (Tuple<int, int> currenttuple in vect)
            {
                Console.SetCursorPosition(currenttuple.Item1, currenttuple.Item2);
                Console.Write('#');
            }
            Console.WriteLine();
        }
        public static void exporttopic(List<Tuple<int, int>> vect, string Way_out_pic)
        {
            Bitmap image = new Bitmap(100, 100);
            foreach (Tuple<int, int> currenttuple in vect)
                image.SetPixel(currenttuple.Item1, currenttuple.Item2, Color.Black);

            image.Save(Way_out_pic);
        }
        public static void exporttofile(List<Tuple<int, int>> vect, int w, int h, int maxIter, double zoom, Random rnd, ref int randh, ref int randw, ref int starcount, ref Complex c, string filename, string Way_out_file)
        {
            using (StreamWriter sw = new StreamWriter(Way_out_file))
            {
                Shuffle(vect, rnd);
                sw.Write(string.Format("static_galaxy_scenario = {\n\tname = \"{0} stars: {1}\"\n\tpriority = 0\n\tdefault = no\n\tcolonizable_planet_odds = 1.0\n\tnum_empires = { min = 0 max = 60 }\n\tnum_empire_default = 21\n\tfallen_empire_default = 4\n\tfallen_empire_max = 4\n\tadvanced_empire_default = 7\n\tcore_radius = 0\n\trandom_hyperlanes = yes\n\n", filename.Replace(".txt", ""), (starcount + 1)););
                starcount = -1;
                foreach (Tuple<int, int> currenttuple in vect)
                {
                    int rand = rnd.Next(-5, 6);
                    int x = (10 * (currenttuple.Item1 - 50) + rand);
                    rand = rnd.Next(-5, 6);
                    int y = (10 * (currenttuple.Item2 - 50) + rand);
                    if (x > 500)
                        x -= 8;
                    else if (x < -500)
                        x += 8;
                    if (y > 500)
                        y -= 8;
                    else if (y < -500)
                        y += 8;
                    starcount++;
                    sw.Write("\tsystem = {\n\t\tid = " + starcount + "\n\t\t\tposition = {\n\t\t\tx = " + x + "\n\t\t\ty = " + y + "\n\t\t}\n\t}\r");
                    if (rnd.Next(250) == 1)
                    {
                        rand = rnd.Next(40, 100);
                        sw.Write("\tnebula = {\n\t\tposition = {\n\t\t\tx = " + x + "\n\t\t\ty = " + y + "\n\t\t}\n\t\tradius = " + rand + "\n\t}\r");
                    }
                }
                sw.Write("\tsystem = {\n\t\tid = " + (starcount + 1) + "\n\t\tposition = {\n\t\tx = 0\n\t\ty = 0\n\t\t}\n\t}\r");
                sw.Write("}\n#h = " + h + "\n#w = " + w + "\n#c = " + c + "\n#zoom = " + zoom + "\n#iter = " + maxIter + "\n#randh = " + randh + "\n#randw = " + randw);
            }
            Console.WriteLine(filename);
        }

        public static void Main(string[] args)
        {
            Console.WindowHeight = 50;
            Console.WindowWidth = 150;
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "генерация текста");
            //string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string newdirectory = Path.Combine(directory, "map_new");
            string picdirectory = Path.Combine(directory, "map_pics");
            string Way_out_pic = null;
            string Way_out_file = null;
            if (Directory.Exists(newdirectory))
                Directory.Delete(newdirectory, true);
            if (Directory.Exists(picdirectory))
                Directory.Delete(picdirectory, true);
            Directory.CreateDirectory(newdirectory);
            Directory.CreateDirectory(picdirectory);
            var rnd = new Random(726568);
            // set parameters
            int w = 10000;
            int h = 10000;
            int maxIter = 350;
            double zoom = 0.505;
            // other values
            int randw = 0;
            int randh = 0;
            int starcount = 0;
            Complex c = 0;
            //       Parallel.For(1, 100, index =>
            Stopwatch st = new Stopwatch();
            st.Start();
            for (int index = 1; index < 10; index++)
            {
                // file name
                string filename = "Insane Julia Set Rand NS " + index;
                Way_out_pic = Path.Combine(picdirectory, filename + ".jpg");
                Way_out_file = Path.Combine(newdirectory, filename + ".txt");


                // create list for dots
                List<Tuple<int, int>> vect = new List<Tuple<int, int>>();

                // generate julia set
                GenJulSet(vect, w, h, maxIter, zoom, rnd, ref randh, ref randw, ref starcount, ref c);

                // export to console
                exporttoconsole(vect);

                // export to pic
                exporttopic(vect, Way_out_pic);

                // export to stellaris file
                exporttofile(vect, w, h, maxIter, zoom, rnd, ref randh, ref randw, ref starcount, ref c, filename, Way_out_file);
            }
            st.Stop();
            Console.WriteLine(st.Elapsed);
        }
    }
}