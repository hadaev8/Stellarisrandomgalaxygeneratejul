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

        public static List<Point> GenJulSet(int w, int h, int maxIter, double zoom, int randomSeed)
        {
            List<Point> vect = new List<Point>();

            Random rnd;
            if (randomSeed == 0)
                rnd = new Random();
            else
                rnd = new Random(randomSeed);

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
            Complex c = new Complex(c1, c2);
            // рандомный сдвиг, если нужен
            int randw = rnd.Next(-w / 2, w / 2);
            int randh = rnd.Next(-h / 2, h / 2);
            // вспомогательные переменные для приведения координат
            double r = 0.5 * (1 + Math.Sqrt(1 + 4 * Complex.Abs(c)));
            double xStep = 2 * r / w;
            double yStep = 2 * r / h;
            double yhelp = 2 * r * (1 - zoom);

            double yfirst = h - h * zoom + randh;
            double xfirst = w - w * zoom + randw;
            double ylast = h * zoom + randh;
            double xlast = w * zoom + randw;

            // чтобы убивать повторяющиеся
            double predx = 0;
            int povtor = 0;

            bool eptline = true;
            // первая строка по иксу (y = 0)
            Parallel.For(0, 99, (index, loopState) =>
            //for (double x = xfirst; x < xlast; x++)
            {
                int i = maxIter;
                Complex z = new Complex(-r + (xfirst + index) * xStep, -r + yfirst * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    loopState.Stop();
                }
            });
            if (eptline)// == false)
            {
                goto startofcycle;
            }
            // последняя строка по иксу (y = 99)
            eptline = true;
            //for (double x = xfirst; x < xlast; x++)
            Parallel.For(0, 99, (index, loopState) =>
            {
                int i = maxIter;
                Complex z = new Complex(-r + (xfirst + index) * xStep, -r + ylast * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    loopState.Stop();
                }
            });
            if (eptline)// == false)
            {
                goto startofcycle;
            }
            // первая строка по игреку (x = 0)
            eptline = true;
            //for (double y = yfirst; y < ylast; y++)
            Parallel.For(0, 99, (index, loopState) =>
            {
                int i = maxIter;
                Complex z = new Complex(-r + xfirst * xStep, -r + (yfirst + index) * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    loopState.Stop();
                }
            });
            if (eptline)// == false)
            {
                goto startofcycle;
            }
            // последняя строка по игреку (x = 99)
            eptline = true;
            //for (double y = yfirst; y < ylast; y++)
            Parallel.For(0, 99, (index, loopState) =>
            {
                int i = maxIter;
                Complex z = new Complex(-r + xlast * xStep, -r + (yfirst + index) * yStep);
                while (Complex.Abs(z) < r && i != 0)
                {
                    z = (z * z + c);
                    --i;
                }
                if (i == 0)
                {
                    eptline = false;
                    loopState.Stop();
                }
            });
            if (eptline)// == false)
            {
                goto startofcycle;
            }
            //int yy = 0;

            Parallel.For(0, 99, (index, loopState) =>
            //for (double y = yfirst; y < ylast; y++)
            {
                int xx = 0;
                for (double x = xfirst; x < xlast; x++)
                {
                    int i = maxIter;
                    Complex z = new Complex(-r + x * xStep, -r + (yfirst + index) * yStep);
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
                            vect.Add(new Point(xx, yy));
                        }
                    }
                    xx++;
                }
                //yy++;
                if (vect.Count > 3000)
                    //loopState.Stop();
                    goto startofcycle;
            }//);
        startofcycle:
            return vect;
        }
        public static void exporttoconsole(List<Point> vect)
        {
            Console.Clear();
            foreach (Point currenttuple in vect)
            {
                Console.SetCursorPosition(currenttuple.x, currenttuple.y);
                Console.Write('#');
            }
            Console.WriteLine();
        }
        public static void exporttopic(List<Point> vect, string Way_out_pic)
        {
            Bitmap image = new Bitmap(100, 100);
            foreach (Point currenttuple in vect)
                image.SetPixel(currenttuple.x, currenttuple.y, Color.Black);

            image.Save(Way_out_pic);
        }
        public static void exporttofile(List<Point> vect, int w, int h, int maxIter, double zoom, Random rnd, Log currentlog, string filename, string Way_out_file)
        {
            using (StreamWriter sw = new StreamWriter(Way_out_file))
            {
                Shuffle(vect, rnd);
                sw.Write("static_galaxy_scenario = {\n\tname = \"" + filename.Replace(".txt", "") + " stars: " + (vect.Count + 1) + "\"\n\tpriority = 0\n\tdefault = no\n\tcolonizable_planet_odds = 1.0\n\tnum_empires = { min = 0 max = 60 }\n\tnum_empire_default = 21\n\tfallen_empire_default = 4\n\tfallen_empire_max = 4\n\tadvanced_empire_default = 7\n\tcore_radius = 0\n\trandom_hyperlanes = yes\n\n");
                foreach (Point currenttuple in vect)
                {
                    int rand = rnd.Next(-2, 3);
                    int x = (10 * (currenttuple.x - 50) + rand);
                    rand = rnd.Next(-2, 3);
                    int y = (10 * (currenttuple.y - 50) + rand);
                    if (x > 500)
                        x -= 8;
                    else if (x < -500)
                        x += 8;
                    if (y > 500)
                        y -= 8;
                    else if (y < -500)
                        y += 8;
                    sw.Write("\tsystem = {\n\t\tid = " + vect.IndexOf(currenttuple) + "\n\t\t\tposition = {\n\t\t\tx = " + x + "\n\t\t\ty = " + y + "\n\t\t}\n\t}\r");
                    if (rnd.Next(250) == 1)
                    {
                        rand = rnd.Next(40, 100);
                        sw.Write("\tnebula = {\n\t\tposition = {\n\t\t\tx = " + x + "\n\t\t\ty = " + y + "\n\t\t}\n\t\tradius = " + rand + "\n\t}\r");
                    }
                }
                sw.Write("\tsystem = {\n\t\tid = " + (vect.Count + 1) + "\n\t\tposition = {\n\t\tx = 0\n\t\ty = 0\n\t\t}\n\t}\r");
                sw.Write("}\n#h = " + h + "\n#w = " + w + "\n#c1 = " + currentlog.c1 + "\n#c2 = " + currentlog.c2 + "\n#zoom = " + zoom + "\n#iter = " + maxIter + "\n#randh = " + currentlog.randh + "\n#randw = " + currentlog.randw);
            }
        }

        public static void Main(string[] args)
        {
            Console.WindowHeight = Console.LargestWindowHeight;
            Console.WindowWidth = Console.LargestWindowWidth;
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "text generation");
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
            string Way_out_pic = null;
            int randomSeed = 0;
            Random rnd = new Random();
            // set parameters
            int w = 10000;
            int h = 10000;
            int maxIter = 350;
            double zoom = 0.505;

            // other values
            int randw = 0;
            int randh = 0;
            Complex c = 0;

            Stopwatch st = new Stopwatch();
            st.Start();

            List<List<Point>> maps = new List<List<Point>>();
            List<Log> log = new List<Log>();
            int startvalue = 1600;
            Parallel.For(startvalue, startvalue + 21, index =>
            //for (int index = startvalue; index < startvalue + 21; index++)
            {
                List<Point> vect = new List<Point>();
                do
                {
                    vect = GenJulSet(w, h, maxIter, zoom, randomSeed);
                } while (vect.Count > 3000 || vect.Count < 50);

                maps.Add(vect);
                log.Add(new Log((maps.Count - 1), 0, randh, randw, c.Real, c.Imaginary));
                Console.WriteLine(index);
            });
            Console.WriteLine(st.Elapsed);
            Console.WriteLine(maps.Count);
            foreach (List<Point> vect in maps)
            {
                // file name
                string filename = "Insane Julia Set Rand NS " + (maps.IndexOf(vect) + startvalue);

                // export to console
                //exporttoconsole(vect);

                Way_out_pic = Path.Combine(dirs["bad_pics"], filename + ".jpg");

                //if (Console.ReadKey().Key == ConsoleKey.Enter)
                //{
                //    Way_out_pic = Path.Combine(picdirectorygood, filename + ".jpg");

                //    // лучше я не придумал для замены 1 значения тупла
                //    Tuple<int, int, int, int, double, double> currentlog = log[maps.IndexOf(vect)];
                //    currentlog = new Tuple<int, int, int, int, double, double>(1, currentlog.Item2, currentlog.Item3, currentlog.Item4, currentlog.Item5, currentlog.Item6);
                //    log.RemoveAt(maps.IndexOf(vect));
                //    log.Insert(maps.IndexOf(vect), currentlog);

                //}

                //Console.Clear();

                // вывод в файл
                exporttofile(vect, w, h, maxIter, zoom, rnd, log[maps.IndexOf(vect)], filename, Path.Combine(dirs["map_new"], filename + ".txt"));

                // export to pic
                exporttopic(vect, Way_out_pic);

            }
            //using (StreamWriter sw = new StreamWriter(Path.Combine(directory, "log.txt")))
            //{
            //    foreach (Tuple<int, int, int, int, double, double> currenttuple in log)
            //        sw.Write(currenttuple + "\n");
            //}
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

    public struct Log
    {
        public double c1, c2;
        public int imageId, isGood, randh, randw;

        public Log(int imageId, int isGood, int randh, int randw, double c1, double c2)
        {
            this.imageId = imageId;
            this.isGood = isGood;
            this.randh = randh;
            this.randw = randw;
            this.c1 = c1;
            this.c2 = c2;
        }
    }
}