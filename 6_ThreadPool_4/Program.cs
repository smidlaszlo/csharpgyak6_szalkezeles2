using System;
using System.Threading;

namespace _ThreadPool4
{
    class Program
    {
        static void Main(string[] args)
        {
            int munkakSzama = 10;
            int munkaIdeje = 100;
            int foSzalAltatasa = 110;

            ElerhetoSzalakLekerdezese();

            for (int i = 0; i < munkakSzama; i++)
            {
                ThreadPool.QueueUserWorkItem(Metodus, munkaIdeje);
            }

            ElerhetoSzalakLekerdezese();

            Thread.Sleep(foSzalAltatasa);

            ElerhetoSzalakLekerdezese();

            Console.WriteLine("Main metodus vege");
        }

        static void Metodus(object state)
        {
            Console.WriteLine("Munka");

            Thread.Sleep((int)state);
        }

        static void ElerhetoSzalakLekerdezese()
        {
            int szabadMunkasSzalakSzama, elerhetoIOMunkasSzalakSzama;

            ThreadPool.GetAvailableThreads(out szabadMunkasSzalakSzama, out elerhetoIOMunkasSzalakSzama);

            Console.WriteLine("Elerheto altalanos feladatokat elvegzo szalak szama: {0}", szabadMunkasSzalakSzama);
            Console.WriteLine("Elerheto I/O completion port (hatterben vegzett I/O muveletekhez kotheto) szalak szama: {0}", elerhetoIOMunkasSzalakSzama);
        }
    }
}
