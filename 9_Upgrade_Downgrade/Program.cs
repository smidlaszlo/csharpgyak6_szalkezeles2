//#define CSAK_KET_SZAL_FUT //csak 2 szal fut es nincs visszaallitas
using System;
using System.Threading;

public class ReaderWriterLockUpgradePelda
{
    static ReaderWriterLock olvasoIroZarolas = new ReaderWriterLock();
    static int osztottAdat = 0;
    static int olvasasokSzama = 0;
    static int irasokSzama = 0;
    static bool szalakFutnak = true;

    //UpgradeToWriterLock és DowngradeFromWriterLock példa
    static void SzalMetodus(object szalAzonosito)
    {
        try
        {
            olvasoIroZarolas.AcquireReaderLock(Timeout.Infinite);

            try
            {
                Console.WriteLine($"{szalAzonosito,2}. szál: olvasás, adat = {osztottAdat}");
                Interlocked.Increment(ref olvasasokSzama);

                //Szükség van az adatok módosítására, ezért íróvá alakítjuk a zárolást.
                //A szálak az írási sorba kerülnek.
                try
                {
                    //A LockCookie struktúra tárolja az állapotot, ami szükséges a visszaalakításhoz.
                    LockCookie zarolasiSuti = olvasoIroZarolas.UpgradeToWriterLock(Timeout.Infinite);

                    Console.WriteLine($"{szalAzonosito,2}. szál: íróvá alakítjuk a zárolást");

                    try
                    {
                        //Módosítjuk az adatokat.
                        osztottAdat++;
                        Interlocked.Increment(ref irasokSzama);

                        Console.WriteLine($"{szalAzonosito,2}. szál: írás, adat = {osztottAdat}");
                    }
                    finally
                    {
                        //Visszaalakítjuk olvasó zárolássá.
                        olvasoIroZarolas.DowngradeFromWriterLock(ref zarolasiSuti);

                        Console.WriteLine($"{szalAzonosito,2}. szál: visszaállítjuk olvasóvá a zárolást");
                    }
                }
                catch (ApplicationException)
                {
                    Console.WriteLine($"{szalAzonosito,2}. szál: UpgradeToWriterLock() kérés időtúllépése");
                }

                Console.WriteLine($"{szalAzonosito,2}. szál: ismét olvasás, adat = {osztottAdat}");
                Interlocked.Increment(ref olvasasokSzama);
            }
            finally
            {
                olvasoIroZarolas.ReleaseReaderLock();
            }
        }
        catch (ApplicationException)
        {
            Console.WriteLine($"{szalAzonosito,2}. szál: AcquireReaderLock() kérés időtúllépése");
        }
    }

    //ReleaseLock és RestoreLock példa
    static void VisszaallitasMetodus(Random veletlenObjektum)
    {
        int utolsoIroSzal;
        int olvasasiZarolasIdotullepese = 10;
        int maximalisVarakozasiIdo = 1000;//veletlenszam [0, maxVarakozas)

        try
        {
            olvasoIroZarolas.AcquireReaderLock(olvasasiZarolasIdotullepese);

            try
            {
                //Az osztott adatot cache-eljuk.
                int eltaroltAdat = osztottAdat;
                Interlocked.Increment(ref olvasasokSzama);

                KiirVisszaallitasnal("olvasas, eltárolt adat: " + eltaroltAdat);

                //WriterSeqNum = writer sequence number
                utolsoIroSzal = olvasoIroZarolas.WriterSeqNum;

                //Feloldjuk a zárolást és elmentjük az állapotot a későbbi visszaállításhoz.
                LockCookie zarolasiSuti = olvasoIroZarolas.ReleaseLock();

                Thread.Sleep(veletlenObjektum.Next(maximalisVarakozasiIdo));

                //Visszaállítjuk az előbb elmentett állapotot.
                olvasoIroZarolas.RestoreLock(ref zarolasiSuti);

                //Ellenőrizzük, hogy más szálak szereztek-e írási zárolást a várakozás alatt.
                //Ha nem, akkor a gyorsítótárazott érték továbbra is érvényes.
                if (olvasoIroZarolas.AnyWritersSince(utolsoIroSzal))
                {
                    eltaroltAdat = osztottAdat;
                    Interlocked.Increment(ref olvasasokSzama);

                    KiirVisszaallitasnal("osztott adat valtozott: " + eltaroltAdat);
                }
                else
                {
                    KiirVisszaallitasnal("osztott adat NEM valtozott: " + eltaroltAdat);
                }
            }
            finally
            {
                olvasoIroZarolas.ReleaseReaderLock();
            }
        }
        catch (ApplicationException)
        {
            KiirVisszaallitasnal("AcquireReaderLock() kérés időtúllépése");
        }
    }

    static void KiirVisszaallitasnal(string szoveg)
    {
        Console.WriteLine("{0} nevű szál: {1}", Thread.CurrentThread.Name, szoveg);
    }


    public static void Main(string[] args)
    {
#if CSAK_KET_SZAL_FUT
        Thread elsoSzal = new Thread(SzalMetodus);
        Thread masodikSzal = new Thread(SzalMetodus);

        elsoSzal.Start(1);
        masodikSzal.Start(2);

        elsoSzal.Join();
        masodikSzal.Join();
#else
        const int szalakSzama = 10;
        const int hanySzalUtanLegyenVarakozas = 5;
        const int varkozasIdeje = 100; //milliszekundum, ezredmásodperc

        Thread[] szalak = new Thread[szalakSzama];

        for (int i = 0; i < szalakSzama; i++)
        {
            //szalak[i] = new Thread(SzalMetodus);
            szalak[i] = new Thread(MetodusBeallitas);

            szalak[i].Name = new String((char)(i + 'A'), 1);
            szalak[i].Start(i);

            if (i > hanySzalUtanLegyenVarakozas)
                Thread.Sleep(varkozasIdeje);
        }

        szalakFutnak = false;

        for (int i = 0; i < szalakSzama; i++)
            szalak[i].Join();

#endif

        Console.WriteLine("\n{0} olvasás, {1} írás történt.", olvasasokSzama, irasokSzama);

        Console.WriteLine("Main szál befejeződött.");
    }

    static void MetodusBeallitas(object szalAzonosito)
    {
        Random veletlenObjektum = new Random();
        double hanySzazalekLegyenVisszaallitas = 0.2;

        while (szalakFutnak)
        {
            double melyikMetodusLegyen = veletlenObjektum.NextDouble();

            if (melyikMetodusLegyen < hanySzazalekLegyenVisszaallitas)
                VisszaallitasMetodus(veletlenObjektum);
            else
                SzalMetodus(szalAzonosito);
        }
    }
}
