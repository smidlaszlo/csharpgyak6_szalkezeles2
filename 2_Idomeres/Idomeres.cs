using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace _2_Idomeres
{
    class Idomeres
    {
        static void Main(string[] args)
        {
            int processzorSzam = Environment.ProcessorCount;

            Console.WriteLine("Processzorok száma: " + processzorSzam);

            Stopwatch stopper = Stopwatch.StartNew();

            for (int i = 0; i < processzorSzam / 2; i++)
            {
                ParosSzamokOsszege();
                ParatlanSzamokOsszege();                
            }

            stopper.Stop();

            Console.WriteLine("Eltelt idő: " + stopper.ElapsedMilliseconds);

            stopper = Stopwatch.StartNew();


            for (int i = 0; i < processzorSzam / 2; i++)
            {
                Thread szal1;
                Thread szal2;

                szal1 = new Thread(ParosSzamokOsszege);
                szal2 = new Thread(ParatlanSzamokOsszege);

                szal1.Start();
                szal2.Start();

                //szal1.Join();
                //szal2.Join();

                if (processzorSzam / 2 - 1 == i)
                {
                    szal2.Join();
                }

            }


            stopper.Stop();

            Console.WriteLine("Eltelt idő: " + stopper.ElapsedMilliseconds);
        }

        public static void ParosSzamokOsszege()
        {
            double szumma = 0;

            for (int i = 0; i <= 50000000; i++)
            {
                if (i % 2 == 0)
                {
                    szumma += i;
                }
            }

            Console.WriteLine("Páros számok összege: " + szumma);
        }

        public static void ParatlanSzamokOsszege()
        {
            double szumma = 0;

            for (int i = 0; i <= 50000000; i++)
            {
                if (i % 2 != 0)
                {
                    szumma += i;
                }
            }

            Console.WriteLine("Páratlan számok összege: " + szumma);
        }

    }
}
