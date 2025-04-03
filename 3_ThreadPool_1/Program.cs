using System;
using System.Threading;

namespace _3_ThreadPool_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //lehetővé teszi a több szálon történő végrehajtást,
            //amely hatékonyan kezeli a szálak létrehozását és újrahasználatát
            //előnyök
            //- teljesítmény: A ThreadPool csökkenti a szálak létrehozásának
            //és megsemmisítésének költségeit, mivel újrahasználja a meglévő szálakat.
            //- egyszerűség: A felhasználóknak nem kell közvetlenül kezelniük
            //a szálak életciklusát, amely csökkenti a hibák lehetőségét.
            //- skálázhatóság: A ThreadPool automatikusan kezeli a szálak számát 
            //a rendszer terhelésének megfelelően.

            //hátrányok
            //- kontroll hiánya: A ThreadPool használatakor a programozók nem rendelkeznek
            //közvetlen kontrollal a szálak felett, ami bizonyos helyzetekben hátrányos lehet.
            //- feladatok mérete: Nagyobb feladatok esetén a ThreadPool nem biztos, 
            //hogy optimális megoldást nyújt, mivel a feladatok mérete 
            //befolyásolhatja a teljesítményt.


            //Feladat hozzáadása a ThreadPool-hoz
            //ThreadPool.QueueUserWorkItem(new WaitCallback(Metodus), "C# programozás");
            ThreadPool.QueueUserWorkItem(Metodus, "C# programozás");

            //Fő szál várakozása, hogy a feladat befejeződjön
            Console.WriteLine("Fő szál várakozik...");
            Thread.Sleep(1000); // Várakozás a feladat befejezésére
            Console.WriteLine("Fő szál vége.");

            //A főszál várakozik, hogy a feladat befejeződjön
            Console.ReadKey();
        }

        static void Metodus(object parameter)
        {
            Console.WriteLine("Feladat végrehajtása: " + parameter);
        }

    }
}
