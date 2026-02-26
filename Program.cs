using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lufthansa
{
    internal class Program
    {
        public struct Csomag
        {
            public string id;
            public double m;
            public double v;
        }
        public struct csaladiCsomagok
        {
            public double sm;
            public double sv;
            public List<Csomag> csomagok;
        }
        public struct kontener
        {
            public int id;
            public int k;
            public double am;
            public double av;
            public List<csaladiCsomagok> bcscs;
        }
        static List<kontener> inicializalas()
        {
            List<kontener> ks = new List<kontener>();
            for(int i = 0; i< 5; i++)
            {
                kontener k = new kontener();
                k.id = i;
                k.k = i - 2;
                k.am = 0;
                k.av = 0;
                k.bcscs = new List<csaladiCsomagok>();
            }
            return ks;
        }
        static bool belefere(kontener k)
        {
            return k.am < 1500 && k.av < 6;
        }
        static Dictionary<string,csaladiCsomagok> beolvasas(string path)
        {
            Dictionary<string, csaladiCsomagok> cscs = new Dictionary<string, csaladiCsomagok>();
            StreamReader sr = new StreamReader(path);
            while (!sr.EndOfStream)
            {
                string[] sor = sr.ReadLine().Split(';');
                Csomag cs = new Csomag();
                cs.id = sor[0];
                cs.m = Convert.ToDouble(sor[2].Replace('.',','));
                cs.v = Convert.ToDouble(sor[3].Replace('.',','));
                if (cscs.ContainsKey(sor[1]))
                {
                    cscs[sor[1]].csomagok.Add(cs);
                    csaladiCsomagok uj = cscs[sor[1]];
                    uj.sm = cscs[sor[1]].sm + cs.m;
                    uj.sv = cscs[sor[1]].sv + cs.v;
                    cscs[sor[1]] = uj;
                }
                else
                {
                    csaladiCsomagok uj = new csaladiCsomagok();
                    uj.sm = cs.m;
                    uj.sv = cs.v;
                    uj.csomagok = new List<Csomag>();
                    uj.csomagok.Add(cs);
                    cscs.Add(sor[1], uj);
                }
            }
            sr.Close();
            return cscs;
        }
        static void Main(string[] args)
        {
            Dictionary<string, csaladiCsomagok> cscs = beolvasas("../../csomagok.csv");
            List<kontener> ks = inicializalas();
            Console.ReadKey();
        }
    }
}
