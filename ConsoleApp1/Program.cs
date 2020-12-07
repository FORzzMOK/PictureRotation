using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace PictureRotation
{
    class Program
    {
        private static string path;//Путь к папке с файлами
        private static RotateFlipType turnAngel;//Угол поворота

        private static Queue<String> ImageFiles = new Queue<string>();//Очередь содержащая картинки
        private static object locker = new object();//Объект который мы будем блокировать

        public static void ImageTurn()//Метод поворачивающий картинку
        {
            string fileName = "";
            Image newImage = null;
            Thread thread = Thread.CurrentThread;
            while (ImageFiles.Count != 0)
            {
                lock (locker)
                {
                    fileName = ImageFiles.Dequeue();
                    Console.WriteLine("Осталось обработать {0} изображений \nThread ID: {1}", ImageFiles.Count, thread.ManagedThreadId);
                }
                try
                {
                    newImage = Image.FromFile(path + @"\" + fileName, true);
                }
                catch(System.IO.FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
                newImage.RotateFlip(turnAngel);
                newImage.Save(path + @"\" + fileName);
            }
        }
        public static Queue<String> GetImageFiles(string path) 
        {
            FileInfo[] _files;
            try
            {
                var _info = new DirectoryInfo(path);
                _files = _info.GetFiles();
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (System.ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (System.ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (System.IO.PathTooLongException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            Queue<string> _ImageFiles = new Queue<string>();
            foreach (FileInfo file in _files)
            {
                if (file.Extension.ToUpper() == ".JPEG" || file.Extension.ToUpper() == ".JPG" || file.Extension.ToUpper() == ".BMP" || file.Extension.ToUpper() == ".PNG")
                {
                    _ImageFiles.Enqueue(file.Name);
                }
            }
            if(_ImageFiles.Count == 0)
            {
                Console.WriteLine("Увы.. Но директория не содержит изображений");
                return null;
            }
            return _ImageFiles;
        }//Метод при помощи которого мы получаем файлы изображения из папки.
        static void Main(string[] args)
        {
            path = @"C:\Users\FORzzMOK\Desktop\Pavel";
            turnAngel = RotateFlipType.Rotate90FlipNone;
            ImageFiles = GetImageFiles(path);
            int ThreadCount = Environment.ProcessorCount;


            if (ImageFiles != null)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Thread[] myThread = new Thread[ThreadCount];
                for (int i = 0; i < ThreadCount; i++)
                {
                    myThread[i] = new Thread(ImageTurn);
                    myThread[i].Start();
                }
                while (true)
                {
                    if (myThread.All(T => !T.IsAlive))
                    {
                        stopWatch.Stop();
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                        Console.WriteLine("RunTime " + elapsedTime);
                        break;
                    }
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine("End... ");
            Console.ReadLine();
        }
    }
}
