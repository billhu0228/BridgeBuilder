using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictoriaBridge
{
    class MCTWriter
    {
        public static void Beginning(StreamWriter file)
        {
            file.WriteLine("*UNIT");
            file.WriteLine("N, mm, KJ, C");
            file.WriteLine("*PROJINFO");
            file.WriteLine("USER=Bill");
            file.WriteLine("ADDRESS=CCCC");
            file.WriteLine("*MATERIAL");
            file.WriteLine("1, CONC , C50+  , 0, 0, , C, NO, 0.05, 2, 3.45e4,0.2, 1.0000e-5, {0:E4},0", (2.7e-5));
            file.WriteLine("2, STEEL, Q420  , 0, 0, , C, NO, 0.02, 1, GB12(S)    ,       , Q420");
            file.WriteLine("3, STEEL, Q345  , 0, 0, , C, NO, 0.02, 1, GB12(S)    ,       , Q345");
            file.WriteLine("4, STEEL, Q345+ , 0, 0, , C, NO, 0.02, 2, 2.06e5,0.3, 1.2000e-5,{0:E4},0", (7.698e-5 * 1.12));
            file.WriteLine("9, STEEL, Riged , 0, 0, , C, NO, 0.02, 2, 2.06e9,0.3, 1.2000e-5,{0:E4},0", (1e-15));
            file.WriteLine("*LOAD-GROUP");
            file.WriteLine("AllLoad");
            file.WriteLine("*BNDR-GROUP");
            file.WriteLine("AllBNDR, 0");
        }


        public static void CompositeSection(StreamWriter file)
        {
            file.WriteLine("*SECT-PSCVALUE");
            file.WriteLine("SECT= 502, COMPOSITE-GEN, MainBeam, CC, 0, 0, 0, 0, 0, 0, YES, CP_G, YES, YES, 1");
            file.WriteLine("  PART=1");
            file.WriteLine("  878080, 878080, 0, 0, 9.87922e+011, 1.03596e+012, 5.3335e+013");
            file.WriteLine("  8768.1, 8767.9, 1420, 1420, 0, 0, 0, 0, 8750, 1420");
            file.WriteLine("  -8750, 8750, 8750, -8750, 1400, 1400, -1400, -1400");
            file.WriteLine("  878080, 878080, 0, 0, 9.87922e+011, 1.03596e+012, 5.3335e+013");
            file.WriteLine("  8768.1, 8767.9, 1420, 1420, 0, 0, 0, 0, 8750, 1420");
            file.WriteLine("  -8750, 8750, 8750, -8750, 1400, 1400, -1400, -1400");
            file.WriteLine("  1, YES, NO, 206000, 0.31");
            file.WriteLine("  VERTEX=-8750, 1400, -8250, 1400, -7750, 1400, -7250, 1400, -6750, 1400, 6750, 1400");
            file.WriteLine("         7250, 1400, 7750, 1400, 8250, 1400, 8750, 1400, -8250, 1180");
            file.WriteLine("         -7750, 1180, -7250, 1180, 7250, 1180, 7750, 1180, 8250, 1180");
            file.WriteLine("         -6750, 1150, 6750, 1150, -8750, 1000, -8530, 1000, -6970, 1000");
            file.WriteLine("         -6750, 1000, 6750, 1000, 6970, 1000, 8530, 1000, 8750, 1000");
            file.WriteLine("         -8750, 500, -8530, 500, -6970, 500, -6750, 500, 6750, 500");
            file.WriteLine("         6970, 500, 8530, 500, 8750, 500, -8750, 0, -8530, 0, -6970, 0");
            file.WriteLine("         -6750, 0, 6750, 0, 6970, 0, 8530, 0, 8750, 0, -8750, -500");
            file.WriteLine("         -8530, -500, -6970, -500, -6750, -500, 6750, -500, 6970, -500");
            file.WriteLine("         8530, -500, 8750, -500, -8750, -1000, -8530, -1000, -6970, -1000");
            file.WriteLine("         -6750, -1000, 6750, -1000, 6970, -1000, 8530, -1000, 8750, -1000");
            file.WriteLine("         -8250, -1180, -7750, -1180, -7250, -1180, 7250, -1180");
            file.WriteLine("         7750, -1180, 8250, -1180, -8750, -1400, -8250, -1400");
            file.WriteLine("         -7750, -1400, -7250, -1400, -6750, -1400, 6750, -1400");
            file.WriteLine("         7250, -1400, 7750, -1400, 8250, -1400, 8750, -1400");
            file.WriteLine("  LINE= 0, 1, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 1, 2, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 2, 3, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 3, 4, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 5, 6, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 6, 7, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 7, 8, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 8, 9, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 1, 10, 22, 1, 8249.9, 0");
            file.WriteLine("  LINE= 2, 11, 22, 1, 7749.9, 0");
            file.WriteLine("  LINE= 3, 12, 22, 1, 7249.9, 0");
            file.WriteLine("  LINE= 6, 13, 22, 1, 7250.1, 0");
            file.WriteLine("  LINE= 7, 14, 22, 1, 7750.1, 0");
            file.WriteLine("  LINE= 8, 15, 22, 1, 8250.1, 0");
            file.WriteLine("  LINE= 4, 16, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 5, 17, 36, 1, 6747.85, 0");
            file.WriteLine("  LINE= 0, 18, 36, 1, 8749.9, 0");
            file.WriteLine("  LINE= 9, 25, 36, 1, 8748.7, 0");
            file.WriteLine("  LINE= 16, 21, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 17, 22, 36, 1, 6749.7, 0");
            file.WriteLine("  LINE= 18, 19, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 20, 21, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 22, 23, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 24, 25, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 18, 26, 36, 1, 8749.9, 0");
            file.WriteLine("  LINE= 21, 29, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 22, 30, 36, 1, 6749.7, 0");
            file.WriteLine("  LINE= 25, 33, 36, 1, 8749.7, 0");
            file.WriteLine("  LINE= 26, 27, 22, 1, 500, 0");
            file.WriteLine("  LINE= 28, 29, 22, 1, 500, 0");
            file.WriteLine("  LINE= 30, 31, 22, 1, 500, 0");
            file.WriteLine("  LINE= 32, 33, 22, 1, 500, 0");
            file.WriteLine("  LINE= 26, 34, 36, 1, 8749.9, 0");
            file.WriteLine("  LINE= 29, 37, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 30, 38, 36, 1, 6749.7, 0");
            file.WriteLine("  LINE= 33, 41, 36, 1, 8749.7, 0");
            file.WriteLine("  LINE= 34, 35, 22, 1, 6.88419e-006, 0");
            file.WriteLine("  LINE= 36, 37, 22, 1, 6.88419e-006, 0");
            file.WriteLine("  LINE= 38, 39, 22, 1, 6.88419e-006, 0");
            file.WriteLine("  LINE= 40, 41, 22, 1, 6.88419e-006, 0");
            file.WriteLine("  LINE= 34, 42, 36, 1, 8749.9, 0");
            file.WriteLine("  LINE= 37, 45, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 38, 46, 36, 1, 6749.7, 0");
            file.WriteLine("  LINE= 41, 49, 36, 1, 8749.7, 0");
            file.WriteLine("  LINE= 42, 43, 22, 1, 500, 0");
            file.WriteLine("  LINE= 44, 45, 22, 1, 500, 0");
            file.WriteLine("  LINE= 46, 47, 22, 1, 500, 0");
            file.WriteLine("  LINE= 48, 49, 22, 1, 500, 0");
            file.WriteLine("  LINE= 42, 50, 36, 1, 8749.9, 0");
            file.WriteLine("  LINE= 45, 53, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 46, 54, 36, 1, 6749.7, 0");
            file.WriteLine("  LINE= 49, 57, 36, 1, 8749.7, 0");
            file.WriteLine("  LINE= 50, 51, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 52, 53, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 54, 55, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 56, 57, 22, 1, 1000, 0");
            file.WriteLine("  LINE= 50, 64, 36, 1, 8749.9, 0");
            file.WriteLine("  LINE= 53, 68, 36, 1, 6749.9, 0");
            file.WriteLine("  LINE= 54, 69, 36, 1, 6748.7, 0");
            file.WriteLine("  LINE= 57, 73, 36, 1, 8748.7, 0");
            file.WriteLine("  LINE= 58, 65, 22, 1, 8249.9, 0");
            file.WriteLine("  LINE= 59, 66, 22, 1, 7749.9, 0");
            file.WriteLine("  LINE= 60, 67, 22, 1, 7249.9, 0");
            file.WriteLine("  LINE= 61, 70, 22, 1, 7250.1, 0");
            file.WriteLine("  LINE= 62, 71, 22, 1, 7750.1, 0");
            file.WriteLine("  LINE= 63, 72, 22, 1, 8250.1, 0");
            file.WriteLine("  LINE= 64, 65, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 65, 66, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 66, 67, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 67, 68, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 69, 70, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 70, 71, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 71, 72, 40, 1, 1400, 0");
            file.WriteLine("  LINE= 72, 73, 40, 1, 1400, 0");
            file.WriteLine("  PART=2");
            file.WriteLine("  3.37495e+006, 1.07482e+006, 2.80584e+006, 2.81997e+006, 6.95147e+010, 1.75779e+010, 5.12555e+013");
            file.WriteLine("  6750.1, 6749.9, 124.999, 125.001, 0, 0, 27499.6, 0, 8750, 2695");
            file.WriteLine("  -6750, 6750, 6750, -6750, 130, 130, -120, -120");
            file.WriteLine("  1.4433e+006, 1.9529e+006, 718531, 212184, 1.011e+012, 1.59791e+012, 6.1919e+013");
            file.WriteLine("  8768.1, 8767.9, 920, 1920, 0, 0, 27499.6, 0, 8750, 1900");
            file.WriteLine("  -8750, 8750, 8750, -8750, 900, 900, -1900, -1900");
            file.WriteLine("  0.318471, NO, YES, 34500, 0.2");
            file.WriteLine("  OPOLY=-6750, 1400, -6750, 1150, 6750, 1150, 6750, 1400");
        }






        static public void CableIniForce(StreamWriter file, Bridge curbridge)
        {

            double[] InitialForce = new double[] { 479.4, 903.773, 513.214, 1030.95, 545.788, 1167.29, 576.325,
                    1311.16, 608.109, 1459.52, 637.38, 1596.48, 669.816, 1694.04, 696.516, 1692.24, 725.306, 1497.56,
                    752.983, 1063.51, 779.927, 410.86 };

            file.WriteLine("*INIFORCE");
            double earthkeyx1 = -30000;
            double earthkeyx2 = -80000;
            List<double> earthxlist = Bridge.Linsteps(earthkeyx1, earthkeyx2, -5000).ToList();
            List<int> towerUX = new List<int>();
            List<int> beamUZ = new List<int>();
            int tmp = 0;
            foreach (double ex in earthxlist)
            {
                tmp++;
                var nsel = from nn in curbridge.NodeList where nn.Position.X == ex select nn.XData.AsArray()[0].Value;
                var esel = from ee in curbridge.ElemList
                           where (short)ee.XData.AsArray()[0].Value == (short)nsel.First() ||
                           (short)ee.XData.AsArray()[0].Value == (short)nsel.Last()
                           select ee;

                Line e1 = esel.ElementAt(0);
                Line e2 = esel.ElementAt(1);
                file.WriteLine("{0},AXIAL,{1}", e1.XData.AsArray()[3].Value, InitialForce[tmp * 2 - 2] * 1000);
                file.WriteLine("{0},AXIAL,{1}", e2.XData.AsArray()[3].Value, InitialForce[tmp * 2 - 2] * 1000);

                int n1 = (short)e1.XData.AsArray()[1].Value;
                int n2 = (short)e2.XData.AsArray()[1].Value;
                var esel2 = from ee in curbridge.ElemList
                            where ((short)ee.XData.AsArray()[0].Value == n1 || (short)ee.XData.AsArray()[0].Value == n2) &&
                            ((string)ee.XData.AsArray()[4].Value == "主索")
                            select ee;
                e1 = esel2.ElementAt(0);
                e2 = esel2.ElementAt(1);
                file.WriteLine("{0},AXIAL,{1}", e1.XData.AsArray()[3].Value, InitialForce[tmp * 2 - 1] * 1000);
                file.WriteLine("{0},AXIAL,{1}", e2.XData.AsArray()[3].Value, InitialForce[tmp * 2 - 1] * 1000);
            }



        }


        static public void CableForceForLC(StreamWriter file, Bridge curbridge)
        {
            double[] InitialForce = new double[] { 479.4, 903.773, 513.214, 1030.95, 545.788, 1167.29, 576.325,
                    1311.16, 608.109, 1459.52, 637.38, 1596.48, 669.816, 1694.04, 696.516, 1692.24, 725.306, 1497.56,
                    752.983, 1063.51, 779.927, 410.86 };
            // 索拉力
            double earthkeyx1 = -30000;
            double earthkeyx2 = -80000;
            List<double> earthxlist = Bridge.Linsteps(earthkeyx1, earthkeyx2, -5000).ToList();
            List<int> towerUX = new List<int>();
            List<int> beamUZ = new List<int>();
            int tmp = 0;
            foreach (double ex in earthxlist)
            {
                tmp++;
                var nsel = from nn in curbridge.NodeList where nn.Position.X == ex select nn.XData.AsArray()[0].Value;
                var esel = from ee in curbridge.ElemList
                           where (short)ee.XData.AsArray()[0].Value == (short)nsel.First() ||
                           (short)ee.XData.AsArray()[0].Value == (short)nsel.Last()
                           select ee;
                Line e1 = esel.ElementAt(0);
                Line e2 = esel.ElementAt(1);                
                file.WriteLine("*PRETENSION");
                file.WriteLine("{0},{1:F0}, ", e1.XData.AsArray()[3].Value, InitialForce[tmp*2-2]*1000);
                file.WriteLine("{0},{1:F0}, ", e2.XData.AsArray()[3].Value, InitialForce[tmp * 2 - 2] * 1000);
                int n1 = (short)e1.XData.AsArray()[1].Value;
                int n2 = (short)e2.XData.AsArray()[1].Value;
                towerUX.Add(n1);
                var esel2 = from ee in curbridge.ElemList
                            where ((short)ee.XData.AsArray()[0].Value == n1 || (short)ee.XData.AsArray()[0].Value == n2) &&
                            ((string)ee.XData.AsArray()[4].Value == "主索")
                            select ee;
                e1 = esel2.ElementAt(0);
                e2 = esel2.ElementAt(1);
                file.WriteLine("{0},{1:F0}, ", e1.XData.AsArray()[3].Value,InitialForce[tmp * 2 - 1] * 1000);
                file.WriteLine("{0},{1:F0}, ", e2.XData.AsArray()[3].Value, InitialForce[tmp * 2 - 1] * 1000);
                beamUZ.Add((short)e1.XData.AsArray()[1].Value);
            }
        }






        static public void CableForce(StreamWriter file, Bridge curbridge)
        {
            file.WriteLine("*STLDCASE");
            for(int jj = 1; jj <= 11; jj++)
            {
                file.WriteLine("BackTension-{0},USER,",jj);
                file.WriteLine("FrontTension-{0},USER,", jj);
            }
            // 索拉力
            double earthkeyx1 = -30000;
            double earthkeyx2 = -80000;
            List<double> earthxlist = Bridge.Linsteps(earthkeyx1, earthkeyx2, -5000).ToList();
            List<int> towerUX = new List<int>();
            List<int> beamUZ = new List<int>();
            int tmp = 0;
            foreach (double ex in earthxlist)
            {
                tmp++;

                var nsel = from nn in curbridge.NodeList where nn.Position.X == ex select nn.XData.AsArray()[0].Value;
                var esel = from ee in curbridge.ElemList
                           where (short)ee.XData.AsArray()[0].Value == (short)nsel.First() ||
                           (short)ee.XData.AsArray()[0].Value == (short)nsel.Last()
                           select ee;

                Line e1 = esel.ElementAt(0);
                Line e2 = esel.ElementAt(1);

                file.WriteLine("*USE-STLD,{0}Tension-{1}", "Back", tmp);
                file.WriteLine("*PRETENSION");
                file.WriteLine("{0},{1:F0}, ", e1.XData.AsArray()[3].Value, 1000);
                file.WriteLine("{0},{1:F0}, ", e2.XData.AsArray()[3].Value, 1000);
                int n1 = (short)e1.XData.AsArray()[1].Value;
                int n2 = (short)e2.XData.AsArray()[1].Value;
                towerUX.Add(n1);
                var esel2 = from ee in curbridge.ElemList
                            where ((short)ee.XData.AsArray()[0].Value == n1 || (short)ee.XData.AsArray()[0].Value == n2) &&
                            ((string)ee.XData.AsArray()[4].Value == "主索")
                            select ee;
                e1 = esel2.ElementAt(0);
                e2 = esel2.ElementAt(1);
                file.WriteLine("*USE-STLD,{0}Tension-{1}", "Front", tmp);
                file.WriteLine("*PRETENSION");
                file.WriteLine("{0},1000, ", e1.XData.AsArray()[3].Value);
                file.WriteLine("{0},1000, ", e2.XData.AsArray()[3].Value);
                beamUZ.Add((short)e1.XData.AsArray()[1].Value);
            }

            file.WriteLine("*LOADCOMB");
            file.WriteLine("NAME = CableCheck, GEN, ACTIVE, 0, 0, , 0, 0");
            file.Write("ST,DeadLoad,1,");//, ST, T1, 1, ST, T2, 1");
            tmp = 0;
            for (int i = 1; i <= 11; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    string nameT = j == 0 ? "Front" : "Back";
                    file.Write("ST,{0}Tension-{1},1", nameT, i);
                    tmp++;
                    if (tmp % 4 == 0 || (i == 11 && j == 1))
                    {
                        file.Write("\n");
                    }
                    else
                    {
                        file.Write(",");
                    }
                }
            }

            //==============================================================================================
            // 未知荷载系数
            //==============================================================================================
            file.WriteLine("*UNKCONS");
            foreach (int towernode in curbridge.TowerControlList)
            {
                file.WriteLine(" UX{0},DISP,{0},0,0,NO,YES,50,YES,-50", towernode);
            }
            foreach (int beamnode in beamUZ)
            {
                file.WriteLine(" UZ{0},DISP,{0},0,2,NO,YES,50,YES,-50", beamnode);
            }


            //file.WriteLine("*UNKFACTOR");
            //file.WriteLine("NAME=TestA, CableCheck, LINEAR, BOTH, NO");
            //tmp = 0;
            //foreach (int beamnode in beamUZ)
            //{
            //    file.Write(" UZ{0}", beamnode);
            //    tmp++;
            //    if (tmp % 4 == 0)
            //    {
            //        file.Write("\n");
            //    }
            //    else
            //    {
            //        file.Write(",");
            //    }
            //}
            //tmp = 0;
            //foreach (int towernode in towerUX)
            //{
            //    file.Write(" UX{0}", towernode);
            //    tmp++;
            //    if (tmp % 4 == 0)
            //    {
            //        file.Write("\n");
            //    }
            //    else
            //    {
            //        file.Write(",");
            //    }
            //}

            //// file.WriteLine(" UX408, UZ563");
            //file.WriteLine(" FrontTension-1, 1, BackTension-1, 1, FrontTension-2, 1");
            //file.WriteLine(" BackTension-2, 1, FrontTension-3, 1, BackTension-3, 1");
            //file.WriteLine(" FrontTension-4, 1, BackTension-4, 1, FrontTension-5, 1");
            //file.WriteLine(" BackTension-5, 1, FrontTension-6, 1, BackTension-6, 1");
            //file.WriteLine(" FrontTension-7, 1, BackTension-7, 1, FrontTension-8, 1");
            //file.WriteLine(" BackTension-8, 1, FrontTension-9, 1, BackTension-9, 1");
            //file.WriteLine(" FrontTension-10, 1, BackTension-10, 1, FrontTension-11, 1");
            //file.WriteLine(" BackTension-11, 1");




        }





    }
}
