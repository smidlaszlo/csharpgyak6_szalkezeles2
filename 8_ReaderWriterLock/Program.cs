/*
A ReaderWriterLockSlim hasonló a ReaderWriterLock-hoz, de egyszerűbbek a szabályai.
A ReaderWriterLockSlim elkerül számos potenciális holtpontot. 
Ezenkívül a teljesítménye jelentősen jobb.
*/

//#define READERWRITERLOCKSLIM

using System;
using System.Threading;

namespace _8_ReaderWriterLock
{
    /*
    ReaderWriterLock-nál
    AcquireReaderLock(TimeSpan timeout): Olvasási zárolás megszerzése
    AcquireWriterLock(TimeSpan timeout): Írási zárolás megszerzése
    ReleaseReaderLock(): Olvasási zárolás feloldása
    ReleaseWriterLock(): Írási zárolás feloldása
    UpgradeToWriterLock(ref LockCookie lockCookie, TimeSpan timeout): Olvasási zárolás átalakítása írási zárolássá
    DowngradeFromWriterLock(ref LockCookie lockCookie): Írási zárolás átalakítása olvasási zárolássá
    LockCookie struktúra tárolja az állapotot, ami szükséges a visszaalakításhoz

    ReaderWriterLockSlim-nél
    EnterReadLock(): Olvasási zárolás megszerzése
    EnterWriteLock(): Írási zárolás megszerzése
    ExitReadLock(): Olvasási zárolás feloldása
    ExitWriteLock(): Írási zárolás feloldása
    EnterUpgradeableReadLock(): Lehetővé teszi az írási zárolásra való átalakítást
    ExitUpgradeableReadLock(): Visszaalakítás olvasási zárolássá
     */

    public class ReaderWriterLockPelda
    {
#if READERWRITERLOCKSLIM
        static ReaderWriterLockSlim olvasoIroZarolas = new ReaderWriterLockSlim();
#else
        static ReaderWriterLock olvasoIroZarolas = new ReaderWriterLock();
#endif
        static int osztottAdat = 0;

        static void Olvasas(object szalAzonosito)
        {
            while (true)
            {
#if READERWRITERLOCKSLIM
                olvasoIroZarolas.EnterReadLock();
#else
                //Timeout.Infinite - a szál végtelen ideig várakozik
                olvasoIroZarolas.AcquireReaderLock(Timeout.Infinite);
#endif

                Console.WriteLine($"Reader {szalAzonosito}: {osztottAdat}");

#if READERWRITERLOCKSLIM
                olvasoIroZarolas.ExitReadLock();
#else
                olvasoIroZarolas.ReleaseReaderLock();
#endif                

                Thread.Sleep(500);
            }
        }

        static void Iras(object szalAzonosito)
        {
            Random rnd = new Random();

            while (true)
            {
#if READERWRITERLOCKSLIM
                olvasoIroZarolas.EnterWriteLock();
#else
                olvasoIroZarolas.AcquireWriterLock(Timeout.Infinite);
#endif                

                osztottAdat = rnd.Next(100);
                Console.WriteLine($"Writer {szalAzonosito}: {osztottAdat}");

#if READERWRITERLOCKSLIM
                olvasoIroZarolas.ExitWriteLock();
#else
                olvasoIroZarolas.ReleaseWriterLock();
#endif

                Thread.Sleep(1000);
            }
        }

        public static void Main(string[] args)
        {
            int szalakSzama = 5;
            Thread[] szalak = new Thread[szalakSzama];

            for (int i = 0; i < szalak.Length; i++)
            {
                szalak[i] = new Thread(Olvasas);
                szalak[i].Start(i);
            }

            Thread writer = new Thread(Iras);
            writer.Start(5);

            Console.ReadKey();
        }
    }
}
