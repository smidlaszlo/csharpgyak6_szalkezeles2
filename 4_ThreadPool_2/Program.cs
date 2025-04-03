using System;
using System.IO;
using System.Threading;

namespace _ThreadPool2
{
    class FajlfeldolgozasOsztaly
    {
        //Szimulált fájlfeldolgozási idő
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            string[] fajlok = Directory.GetFiles(@".");

            foreach (string fajl in fajlok)
            {
                ThreadPool.QueueUserWorkItem(FajlFeldolgozasa, fajl);
            }

            Console.WriteLine("Minden fájl feldolgozásra került sorba.");
            Console.ReadKey();
        }

        static void FajlFeldolgozasa(object fajlObjektum)
        {
            string fajl = (string)fajlObjektum;
            Console.WriteLine($"Feldolgozás kezdete: {fajl} - {Thread.CurrentThread.ManagedThreadId}");

            //Szimulált feldolgozási idő
            Thread.Sleep(random.Next(1000, 5000));

            try
            {
                //Valós fájlfeldolgozási logika
                string fajlTartalom = File.ReadAllText(fajl);

                // ... egyéb feldolgozási lépések ...
                Console.WriteLine($"Feldolgozás befejezve: {fajl} - {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a fájl feldolgozásakor: {fajl} - {ex.Message}");
            }
        }
    }
}
