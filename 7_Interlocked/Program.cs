using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace _7_Interlocked
{
    internal class Program
    {
        private static int osszeg = 0;
        private static int szamlalo = 0;
        private static int eroforrasHasznalatban = 0; //0: szabad, >0: foglalt

        public static void Main(string[] args)
        {
            int szalakSzama = 5;
            Thread[] szalak = new Thread[szalakSzama];

            for (int i = 0; i < szalak.Length; i++)
            {
                szalak[i] = new Thread(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        Interlocked.Add(ref osszeg, 5); //Minden szál 5*100-at ad hozzá
                    }
                });

                szalak[i].Start();
            }

            foreach (Thread szal in szalak)
            {
                szal.Join();
            }

            Console.WriteLine("Osszeg: " + osszeg);

            szalak[0] = new Thread(SzamlaloFrissitoMetodus);
            szalak[1] = new Thread(SzamlaloFrissitoMetodus);
            szalak[0].Start();
            szalak[1].Start();
            szalak[0].Join();
            szalak[1].Join();

            //SzamlaloFrissites(0, 1);
            Console.WriteLine("Szamlalo: " + szamlalo);

            //Exchange pelda
            for (int i = 0; i < szalak.Length; i++)
            {
                int szalAzonosito = i + 1;
                szalak[i] = new Thread(() =>
                {
                    //if (Interlocked.Exchange(ref eroforrasHasznalatban, szalAzonosito) == 0)
                    if (EroforrasLefoglalas(szalAzonosito))
                    {
                        Console.WriteLine($"{szalAzonosito}. szal hasznalja az eroforrast.");
                        //Thread.Sleep(1); //Erőforrás használata
                        //eroforrasHasznalatban = 0; //Erőforrás felszabadítása
                        EroforrasFelszabaditas();
                    }
                    else
                    {
                        Console.WriteLine($"{szalAzonosito}. szalnak nem sikerult megszerezni az eroforrast.");
                    }
                });

                szalak[i].Start();
            }

            foreach (Thread szal in szalak)
            {
                szal.Join();
            }
        }
        public static bool EroforrasLefoglalas(int szal)
        {
            if (Interlocked.Exchange(ref eroforrasHasznalatban, szal) == 0)
            {
                //Az erőforrás sikeresen lefoglalva
                return true;
            }
            else
            {
                //Az erőforrás már foglalt
                return false;
            }
        }

        public static void EroforrasFelszabaditas()
        {
            Interlocked.Exchange(ref eroforrasHasznalatban, 0);
        }

        public static bool SzamlaloFrissites(int elvart, int uj)
        {
            return Interlocked.CompareExchange(ref szamlalo, uj, elvart) == elvart;
        }

        static void SzamlaloFrissitoMetodus()
        {
            int ujErtek = 1;
            int elvartErtek = 0;

            if (SzamlaloFrissites(elvartErtek, ujErtek))
            {
                Console.WriteLine($"A változó értéke sikeresen frissítve lett. {elvartErtek} -> {ujErtek}");
            }
            else
            {
                Console.WriteLine($"Nem sikerült frissíteni. A várt érték ({elvartErtek}) nem egyezett meg a jelenlegi értékkel ({szamlalo}).");
            }
        }
    }
}
