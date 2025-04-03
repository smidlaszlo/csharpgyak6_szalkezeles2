using System;
using System.Threading;

namespace _ThreadPool3
{
    public class HivandoObjektum
    {
        public static int szalakSzama = 0;
        public static object zarolandoObjektum = new object();
        ManualResetEvent manualisEsemeny;

        public HivandoObjektum(ManualResetEvent parameter)
        {
            manualisEsemeny = parameter;
        }

        public void HivandoMetodus(object i)
        {
            //a kiírás helyességéhez lockolni kell mert több szál írhatja egyszerre
            lock (zarolandoObjektum)
            {
                szalakSzama++;
                Console.WriteLine("START munka száma:{0,2} szálak száma:{1,2}", (int)i, szalakSzama);
            }

            Thread.Sleep(2000);

            lock (zarolandoObjektum)
            {
                szalakSzama--;
                Console.WriteLine("STOP  munka száma:{0,2} szálak száma:{1,2}", (int)i, szalakSzama);
            }

            manualisEsemeny.Set();//Munka készre állítása
        }
    }

    public class Program
    {
        private static void Main(string[] args)
        {
            const int munkakSzama = 9;
            ManualResetEvent[] esemenyek = new ManualResetEvent[munkakSzama];

            for (int i = 0; i < munkakSzama; i++)
            {
                esemenyek[i] = new ManualResetEvent(false);
                HivandoObjektum munkaObjektum = new HivandoObjektum(esemenyek[i]);
                ThreadPool.QueueUserWorkItem(munkaObjektum.HivandoMetodus, i);
            }

            //A threadpool-ban lévő szálak háttérszálak
            //tehát meg kell várnunk hogy az összes lefusson
            WaitHandle.WaitAll(esemenyek);

            Console.WriteLine("Feladatok elvégezve.");
        }
    }
}
