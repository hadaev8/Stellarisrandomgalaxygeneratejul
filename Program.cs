using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;

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
        public static void GenJulSet(JulSet currentjulset, Random rnd)
        {
            currentjulset.points.Clear();
            // рандомим произвольную постоянную
            currentjulset.c1 = rnd.NextDouble();
            currentjulset.c2 = rnd.NextDouble();
            if (rnd.Next(2) == 1)
            {
                currentjulset.c1 = currentjulset.c1 * (-1);
            }
            if (rnd.Next(2) == 1)
            {
                currentjulset.c2 = currentjulset.c2 * (-1);
            }
            Complex c = new Complex(currentjulset.c1, currentjulset.c2);
            // рандомный сдвиг, если нужен
            currentjulset.randw = rnd.Next(-currentjulset.w / 2, currentjulset.w / 2);
            currentjulset.randh = rnd.Next(-currentjulset.h / 2, currentjulset.h / 2);
            // вспомогательные переменные для приведения координат
            double r = 0.5 * (1 + Math.Sqrt(1 + 4 * Complex.Abs(c)));
            double xStep = 2 * r / currentjulset.w;
            double yStep = 2 * r / currentjulset.h;
            double yhelp = 2 * r * (1 - currentjulset.zoom);

            double yfirst = currentjulset.h - currentjulset.h * currentjulset.zoom + currentjulset.randh;
            double xfirst = currentjulset.w - currentjulset.w * currentjulset.zoom + currentjulset.randw;
            double ylast = currentjulset.h * currentjulset.zoom + currentjulset.randh;
            double xlast = currentjulset.w * currentjulset.zoom + currentjulset.randw;

            // чтобы убивать повторяющиеся
            double predx = 0;
            int povtor = 0;

            bool eptline = true;
            // первая строка по иксу (y = 0)
            for (double x = xfirst; x < xlast; x++)
            {
                int i = currentjulset.maxiter;
                Complex z = new Complex(-r + x * xStep, -r + yfirst * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    break;
                }
            }
            if (eptline)// == false)
            {
                return;
            }
            // последняя строка по иксу (y = 99)
            eptline = true;
            for (double x = xfirst; x < xlast; x++)
            {
                int i = currentjulset.maxiter;
                Complex z = new Complex(-r + x * xStep, -r + ylast * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    break;
                }
            }
            if (eptline)// == false)
            {
                return;
            }
            // первая строка по игреку (x = 0)
            eptline = true;
            for (double y = yfirst; y < ylast; y++)
            {
                int i = currentjulset.maxiter;
                Complex z = new Complex(-r + xfirst * xStep, -r + y * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    break;
                }
            }
            if (eptline)// == false)
            {
                return;
            }
            // последняя строка по игреку (x = 99)
            eptline = true;
            for (double y = yfirst; y < ylast; y++)
            {
                int i = currentjulset.maxiter;
                Complex z = new Complex(-r + xlast * xStep, -r + y * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    break;
                }
            }
            if (eptline)// == false)
            {
                return;
            }
            int yy = 0;
            
            for (double y = yfirst; y < ylast; y++)
            {
                int xx = 0;
                for (double x = xfirst; x < xlast; x++)
                {
                    int i = currentjulset.maxiter;
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
                            currentjulset.points.Add(new Point(xx, yy));
                        }
                    }
                    xx++;
                }
                yy++;
                if (currentjulset.points.Count > 3000)
                    break;
            }
        }
        public static void exporttoconsole(JulSet currentjulset)
        {
            //Console.Clear();
            foreach (Point currentpoint in currentjulset.points)
            {
                Console.SetCursorPosition(currentpoint.x, currentpoint.y);
                Console.Write('#');
            }
            Console.WriteLine();
        }
        public static void exporttopic(JulSet currentjulset, string Way_out_pic)
        {
            Bitmap image = new Bitmap(100, 100);
            foreach (Point currentpoint in currentjulset.points)
                image.SetPixel(currentpoint.x, currentpoint.y, Color.Black);

            image.Save(Way_out_pic);
        }
        public static void exporttofile(JulSet currentjulset, Random rnd, string filename, string Way_out_file)
        {
            using (StreamWriter sw = new StreamWriter(Way_out_file))
            {
                Shuffle(currentjulset.points, rnd);
                sw.Write("static_galaxy_scenario = {\n\tname = \"" + filename.Replace(".txt", "") + " stars: " + (currentjulset.points.Count + 1) + "\"\n\tpriority = 0\n\tdefault = no\n\tcolonizable_planet_odds = 1.0\n\tnum_empires = { min = 0 max = 60 }\n\tnum_empire_default = 21\n\tfallen_empire_default = 4\n\tfallen_empire_max = 4\n\tadvanced_empire_default = 7\n\tcore_radius = 0\n\trandom_hyperlanes = yes\n\n");
                foreach (Point currentpoint in currentjulset.points)
                {
                    int rand = rnd.Next(-2, 3);
                    int x = (10 * (currentpoint.x - 50) + rand);
                    rand = rnd.Next(-2, 3);
                    int y = (10 * (currentpoint.y - 50) + rand);
                    if (x > 500)
                        x -= 8;
                    else if (x < -500)
                        x += 8;
                    if (y > 500)
                        y -= 8;
                    else if (y < -500)
                        y += 8;
                    sw.Write("\tsystem = {\n\t\tid = " + currentjulset.points.IndexOf(currentpoint) + "\n\t\tposition = {\n\t\t\tx = " + x + "\n\t\t\ty = " + y + "\n\t\t}\n\t}\r");
                    if (rnd.Next(250) == 1)
                    {
                        rand = rnd.Next(40, 100);
                        sw.Write("\tnebula = {\n\t\tposition = {\n\t\t\tx = " + x + "\n\t\t\ty = " + y + "\n\t\t}\n\t\tradius = " + rand + "\n\t}\r");
                    }
                }
                sw.Write("\tsystem = {\n\t\tid = " + (currentjulset.points.Count + 1) + "\n\t\tposition = {\n\t\tx = 0\n\t\ty = 0\n\t\t}\n\t}\r");
                sw.Write("}\n#h = " + currentjulset.h + "\n#w = " + currentjulset.w + "\n#c1 = " + currentjulset.c1 + "\n#c2 = " + currentjulset.c2 + "\n#zoom = " + currentjulset.zoom + "\n#iter = " + currentjulset.maxiter + "\n#randh = " + currentjulset.randh + "\n#randw = " + currentjulset.randw);
            }
        }
        public static List<object> getvaluesfromjulset(JulSet currentjulset)
        {
            List<object> values = new List<object>();
            values.Add(currentjulset.imageId);
            values.Add(currentjulset.isGood);
            values.Add(currentjulset.h);
            values.Add(currentjulset.w);
            values.Add(currentjulset.zoom);
            values.Add(currentjulset.maxiter);
            values.Add(currentjulset.randh);
            values.Add(currentjulset.randw);
            values.Add(currentjulset.c1);
            values.Add(currentjulset.c2);

            return values;
        }

        public static void Main(string[] args)
        {
            Console.WindowHeight = Console.LargestWindowHeight;
            Console.WindowWidth = Console.LargestWindowWidth;
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "text generation");
            string Way_out_pic = null;
            Dictionary<string, string> dirs = new Dictionary<string, string>()
            {
                { "map_new",  Path.Combine(directory, "map_new") },
                { "map_pics",  Path.Combine(directory, "map_pics") },
                { "good_pics",  Path.Combine(directory, "good_pics") },
                { "bad_pics",  Path.Combine(directory, "bad_pics") },
            };
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir.Value))
                    Directory.Delete(dir.Value, true);
                Directory.CreateDirectory(dir.Value);
            }
            
            Random rnd = new Random();

            // parameters for set
            int w = 10000;
            int h = 10000;
            int maxiter = 350;
            double zoom = 0.505;
            
            Stopwatch st = new Stopwatch();
            st.Start();

            List<JulSet> maps = new List<JulSet>();
            int startvalue = 1800;
            Parallel.For(startvalue, startvalue + 201, index =>
            {
                JulSet currentjulset = new JulSet(index, 0, w, h, zoom, maxiter, 0, 0, 0, 0, new List<Point>());
                do
                {
                    GenJulSet(currentjulset, rnd);
                } while (currentjulset.points.Count > 3000 || currentjulset.points.Count < 50);

                maps.Add(currentjulset);
                Console.Clear();
                Console.WriteLine(maps.Count/2 + "%");
            });
            Console.WriteLine(st.Elapsed);
            foreach (JulSet currentjulset in maps)
            {
                // file name
                string filename = "Insane Julia Set Rand NS " + (maps.IndexOf(currentjulset) + startvalue);

                // export to console
                exporttoconsole(currentjulset);

                Way_out_pic = Path.Combine(dirs["bad_pics"], filename + ".jpg");

                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    Way_out_pic = Path.Combine(dirs["good_pics"], filename + ".jpg");

                    currentjulset.isGood = 1;

                    // export to stellaris file
                    exporttofile(currentjulset, rnd, filename, Path.Combine(dirs["map_new"], filename + ".txt"));
                }
                Console.Clear();
                
                // export to pic
                exporttopic(currentjulset, Way_out_pic);
            }
            // print log
            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, "log.txt")))
            {
                foreach (JulSet currentjulset in maps)
                {
                    foreach (object currentvalue in getvaluesfromjulset(currentjulset))
                        sw.Write(currentvalue + ";");
                    sw.Write("\n");
                }
            }
            st.Stop();
            Console.WriteLine(st.Elapsed);
        }
    }

    public struct Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class JulSet
    {
        public List<Point> points;
        public double c1, c2, zoom;
        public int w, h, maxiter, imageId, isGood, randh, randw;

        public JulSet(int imageId, int isGood, int w, int h, double zoom, int maxiter, int randh, int randw, double c1, double c2, List<Point> points)
        {
            this.imageId = imageId;
            this.isGood = isGood;
            this.w = w;
            this.h = h;
            this.zoom = zoom;
            this.maxiter = maxiter;
            this.randh = randh;
            this.randw = randw;
            this.c1 = c1;
            this.c2 = c2;
            this.points = points;
        }
    }
}