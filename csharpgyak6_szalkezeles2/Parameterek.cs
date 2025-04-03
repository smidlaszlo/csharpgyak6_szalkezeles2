using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace csharpgyak6_szalkezeles2
{
    class Parameterek
    {
        static void Main(string[] args)
        {
            //Thread parametereben egy fuggvenyt kell megadni
            Thread szal1 = new Thread(Szam.SzamokatKiir);
            szal1.Start();
            szal1.Join();

            //a parameter valojaban egy delegatum, egy type-safe fv. pointer
            //a szal1-ben is automatikusan delegatumot keszitett a keretrendszer a fv. nevebol
            Thread szal2 = new Thread(new ThreadStart(Szam.SzamokatKiir));
            szal2.Start();
            szal2.Join();

            //hasznalhatjuk a delegate kulcsszot is
            Thread szal3 = new Thread(delegate() { Szam.SzamokatKiir(); });
            szal3.Start();
            szal3.Join();

            //hasznalhatunk lambda kifejezest
            Thread szal4 = new Thread(() => Szam.SzamokatKiir());
            szal4.Start();
            szal4.Join();

            //nem csak statikus fuggvenyt hasznalhatunk
            Szam szam = new Szam();
            Thread szal5 = new Thread(szam.Kiir);
            szal5.Start();
            szal5.Join();

            //parameterrel
            object objektum = 2;
            Thread szal6 = new Thread(Szam.SzamokatKiir2);
            szal6.Start(objektum);
            szal6.Join();

            //ParameterizedThreadStart delegatum hasznalataval
            //szal6-ban is automatikusan elkeszult a new ParameterizedThreadStart(Szam.SzamokatKiir2)
            //a parameternek object-nek kell lennie, ezert ez mar nem lesz type-safe
            ParameterizedThreadStart parameteresSzalInditas = new ParameterizedThreadStart(Szam.SzamokatKiir2);
            Thread szal7 = new Thread(parameteresSzalInditas);
            szal7.Start(objektum);
            szal7.Join();

            //parametert type-safe modon atadni ugy lehet, hogy
            //a fv-t es a parametert egysegbe zarom
            int egeszSzam = 2;
            Szam2 szam2 = new Szam2(egeszSzam);
            Thread szal8 = new Thread(new ThreadStart(szam2.SzamokatKiir));
            szal8.Start();
            szal8.Join();

            //adat visszaterese a fuggvenybol callback metodussal
            //letrehozas lepesei
            //1. letrehozni a delegatumot az osztalyon kivul
            //public delegate void Osszegez(int parameter);
            
            //2. hasznalni a masik osztalyban a delegatumot

            //3. ezen osztalyon belul letrehozni a feldolgozo fv-t, amire mutat a delegatum
            //4. itt is hasznalni a delegatumot
            OsszegezCallback callbackFuggveny = new OsszegezCallback(EredenytKiir);

            Szam3 szam3 = new Szam3(egeszSzam, callbackFuggveny);
            Thread szal9 = new Thread(new ThreadStart(szam3.SzamokatOsszegez));
            szal9.Start();
            szal9.Join();

        }

        public static void EredenytKiir(int osszeg)
        {
            Console.WriteLine("Eredmeny: " + osszeg);
        }
    }

    class Szam
    {
        public void Kiir()
        {
            Console.WriteLine("konstans szoveget kiir");
        }
        
        public static void SzamokatKiir()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(i);    
            }
        }            

        public static void SzamokatKiir2(object parameter)
        {
            int szam = 0;
            if (int.TryParse(parameter.ToString(), out szam))
	        {
                for (int i = 0; i < szam; i++)
                {
                    Console.WriteLine(i);
                }		 
	        }

        
        }
    }

    //parameteres fv. a szalban
    class Szam2
    {
        private int parameter;

        public Szam2(int parameter)
        {
            this.parameter = parameter;
        }

        public void SzamokatKiir()
        {
            for (int i = 0; i < parameter; i++)
            {
                Console.WriteLine(i);
            }
        }            

        
    }

    //a fv. visszateresi erteke is szukseges a szalkezelesnel
    public delegate void OsszegezCallback(int parameter);

    class Szam3
    {
        private int parameter;
        OsszegezCallback callbackFuggeny;

        public Szam3(int parameter, OsszegezCallback callbackFuggeny)
        {
            this.parameter = parameter;
            this.callbackFuggeny = callbackFuggeny;
        }

        public void SzamokatOsszegez()
        {
            int szumma = 0;
            for (int i = 0; i < parameter; i++)
            {
                szumma += i + 1; ;
            }

            if (callbackFuggeny != null)
            {
                callbackFuggeny(szumma);
            }
        }

    }

}
