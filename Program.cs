using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
            public string id;
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
            for(int i = 0; i < 5; i++)
            {
                kontener k = new kontener();
                k.id = i+1;
                k.k = i - 2;
                k.am = 0;
                k.av = 0;
                k.bcscs = new List<csaladiCsomagok>();
                ks.Add(k);
            }
            return ks;
        }
        static bool belefere(kontener k, double m, double v)
        {
            return k.am+m < 1500 && k.av+v < 6;
        }
        static string kadikElem(Dictionary<string,csaladiCsomagok> cscs,int k)
        {
            List<string> ids = new List<string>();
            List<double> sms = new List<double>();
            foreach (var item in cscs)
            {
                ids.Add(item.Key);
                sms.Add(item.Value.sm);
            }
            for(int i = 0; i< ids.Count; i++)
            {
                for(int j = i; j< ids.Count; j++)
                {
                    if (sms[i] < sms[j])
                    {
                        string cs1 = ids[i];
                        ids[i] = ids[j];
                        ids[j] = cs1;
                        double cs2 = sms[i];
                        sms[i] = sms[j];
                        sms[j] = cs2;
                    }
                }
            }
            return ids[k];
        }
        static double cg(List<kontener> ks)
        {
            double valami = 0;
            double valami2 = 0;
            for(int i = 0; i < ks.Count; i++)
            {
                valami += ks[i].am * ks[i].k;
                valami2 += ks[i].am;
            }
            return valami / valami2;
        }
        static void szim(Dictionary<string,csaladiCsomagok> cscs, List<kontener> ks)
        {
            for(int i = 0; i< cscs.Count; i++)
            {
                csaladiCsomagok akt = cscs[kadikElem(cscs, i)];
                int minj = 0;
                double mincg = double.PositiveInfinity;
                for (int j = 0; j < ks.Count; j++)
                {
                    kontener k = ks[j];
                    if (belefere(ks[j],akt.sm,akt.sv))
                    {
                        k.k = ks[j].k;
                        k.am = ks[j].am + akt.sm;
                        k.av = ks[j].av + akt.sv;
                        k.bcscs = ks[j].bcscs;
                        ks[j] = k;
                        if(Math.Abs(mincg) >= Math.Abs(cg(ks)))
                        {
                            minj = j;
                            mincg = cg(ks);
                        }
                        k.k = ks[j].k;
                        k.am = ks[j].am - akt.sm;
                        k.av = ks[j].av - akt.sv;
                        ks[j] = k;
                    }
                }
                kontener ku = new kontener();
                ku.id = ks[minj].id;
                ku.k = ks[minj].k;
                ku.am = ks[minj].am+akt.sm;
                ku.av = ks[minj].av+akt.sv;
                ku.bcscs = ks[minj].bcscs;
                ku.bcscs.Add(akt);
                ks[minj] = ku;
            }
            Console.WriteLine("--- LUFTHANSA JÁRAT RAKODÁSI TERV ---");
            for(int i = 0; i<ks.Count; i++)
            {
                Console.WriteLine($"Kontener {ks[i].id} (Erőkar: {ks[i].k}): {ks[i].am} kg / {ks[i].av} m^3 - {ks[i].bcscs.Count} csalad");
            }
            Console.WriteLine($"A repülőgép VÉGSŐ súlypontja (CG): {Math.Round(cg(ks),4)}");
            Console.WriteLine("A gép tökéletes egyensúlyban van. Felszállás engedélyezve!");
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
                    uj.id = sor[1];
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
            szim(cscs,ks);
            Console.ReadKey();
        }
    }
}
