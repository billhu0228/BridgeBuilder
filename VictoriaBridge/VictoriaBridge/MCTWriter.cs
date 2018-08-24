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
            file.WriteLine("*SECT-PSCVALUE ");
            file.WriteLine(" SECT= 601, COMPOSITE-GEN, 36-25, CC, 0, 0, 0, 0, 0, 0, YES, NO, CP_G, YES, YES, 1");
            file.WriteLine("  PART=1");
            file.WriteLine("  722886, 722886, 0, 0, 7.53769e+011, 8.92737e+011, 4.38807e+013");
            file.WriteLine("  8762.47, 8762.53, 1418, 1418, 0, 0, 0, 0, 8750.03, 1418");
            file.WriteLine("  -8750.03, 8749.97, 8749.97, -8750.03, 1400, 1400, -1400, -1400");
            file.WriteLine("  722886, 722886, 0, 0, 7.53769e+011, 8.92737e+011, 4.38807e+013");
            file.WriteLine("  8762.47, 8762.53, 1418, 1418, 0, 0, 0, 0, 8750.03, 1418");
            file.WriteLine("  -8750.03, 8749.97, 8749.97, -8750.03, 1400, 1400, -1400, -1400");
            file.WriteLine("  1, YES, NO, 206000, 0.31");
            file.WriteLine("  VERTEX=-8750, 1400, -8250, 1400, -7750, 1400, -7250, 1400, -6750, 1400, 6750, 1400");
            file.WriteLine("         7250, 1400, 7750, 1400, 8250, 1400, 8750, 1400, -6750, 1275");
            file.WriteLine("         -8250, 1180, -7750, 1180, -7250, 1180, 7250, 1180, 7750, 1180");
            file.WriteLine("         8250, 1180, -8750, 1000, -8530, 1000, -6970, 1000, -6750, 1000");
            file.WriteLine("         6749.6, 1000, 6750, 1000, 6969.6, 1000, 8529.6, 1000");
            file.WriteLine("         8749.6, 1000, 8750, 1000, -8750, 500, -8530, 500, -6970, 500");
            file.WriteLine("         -6750, 500, 6749.6, 500, 6750, 500, 6969.6, 500, 8529.6, 500");
            file.WriteLine("         8749.6, 500, 8750, 500, -8750, 0, -8530, 0, -6970, 0, -6750, 0");
            file.WriteLine("         6749.6, 0, 6750, 0, 6969.6, 0, 8529.6, 0, 8749.6, 0, 8750, 0");
            file.WriteLine("         -8750, -500, -8530, -500, -6970, -500, -6750, -500, 6749.6, -500");
            file.WriteLine("         6750, -500, 6969.6, -500, 8529.6, -500, 8749.6, -500, 8750, -500");
            file.WriteLine("         -8750, -1000, -8530, -1000, -6970, -1000, -6750, -1000");
            file.WriteLine("         6749.6, -1000, 6750, -1000, 6969.6, -1000, 8529.6, -1000");
            file.WriteLine("         8749.6, -1000, 8750, -1000, -8250, -1180, -7750, -1180");
            file.WriteLine("         -7250, -1180, 7250, -1180, 7750, -1180, 8250, -1180, -8750, -1400");
            file.WriteLine("         -8250, -1400, -7750, -1400, -7250, -1400, -6750, -1400");
            file.WriteLine("         6750, -1400, 7250, -1400, 7750, -1400, 8250, -1400, 8750, -1400");
            file.WriteLine("  LINE= 0, 1, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 1, 2, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 2, 3, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 3, 4, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 5, 6, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 6, 7, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 7, 8, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 8, 9, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 4, 10, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 1, 11, 22, 1, 8250.03, 1.05902e-007");
            file.WriteLine("  LINE= 2, 12, 22, 1, 7750.03, 1.05902e-007");
            file.WriteLine("  LINE= 3, 13, 22, 1, 7250.03, 1.05902e-007");
            file.WriteLine("  LINE= 6, 14, 22, 1, 7249.97, 1.05902e-007");
            file.WriteLine("  LINE= 7, 15, 22, 1, 7749.97, 1.05902e-007");
            file.WriteLine("  LINE= 8, 16, 22, 1, 8249.97, 1.05902e-007");
            file.WriteLine("  LINE= 0, 17, 25, 1, 8750.03, 1.20343e-007");
            file.WriteLine("  LINE= 5, 22, 25, 1, 6749.97, 1.20343e-007");
            file.WriteLine("  LINE= 9, 26, 25, 1, 8749.97, 1.20343e-007");
            file.WriteLine("  LINE= 10, 20, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 17, 18, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 19, 20, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 21, 22, 25, 1, 1000, 1.20343e-007");
            file.WriteLine("  LINE= 22, 23, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 24, 25, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 17, 27, 25, 1, 8750.03, 1.20343e-007");
            file.WriteLine("  LINE= 20, 30, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 22, 32, 25, 1, 6749.97, 1.20343e-007");
            file.WriteLine("  LINE= 26, 36, 25, 1, 8749.97, 1.20343e-007");
            file.WriteLine("  LINE= 27, 28, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 29, 30, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 31, 32, 25, 1, 500, 1.20343e-007");
            file.WriteLine("  LINE= 32, 33, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 34, 35, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 27, 37, 25, 1, 8750.03, 1.20343e-007");
            file.WriteLine("  LINE= 30, 40, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 32, 42, 25, 1, 6749.97, 1.20343e-007");
            file.WriteLine("  LINE= 36, 46, 25, 1, 8749.97, 1.20343e-007");
            file.WriteLine("  LINE= 37, 38, 22, 1, 8.88178e-013, 1.05902e-007");
            file.WriteLine("  LINE= 39, 40, 22, 1, 8.88178e-013, 1.05902e-007");
            file.WriteLine("  LINE= 41, 42, 25, 1, 8.88178e-013, 1.20343e-007");
            file.WriteLine("  LINE= 42, 43, 22, 1, 8.88178e-013, 1.05902e-007");
            file.WriteLine("  LINE= 44, 45, 22, 1, 8.88178e-013, 1.05902e-007");
            file.WriteLine("  LINE= 37, 47, 25, 1, 8750.03, 1.20343e-007");
            file.WriteLine("  LINE= 40, 50, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 42, 52, 25, 1, 6749.97, 1.20343e-007");
            file.WriteLine("  LINE= 46, 56, 25, 1, 8749.97, 1.20343e-007");
            file.WriteLine("  LINE= 47, 48, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 49, 50, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 51, 52, 25, 1, 500, 1.20343e-007");
            file.WriteLine("  LINE= 52, 53, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 54, 55, 22, 1, 500, 1.05902e-007");
            file.WriteLine("  LINE= 47, 57, 25, 1, 8750.03, 1.20343e-007");
            file.WriteLine("  LINE= 50, 60, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 52, 62, 25, 1, 6749.97, 1.20343e-007");
            file.WriteLine("  LINE= 56, 66, 25, 1, 8749.97, 1.20343e-007");
            file.WriteLine("  LINE= 57, 58, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 59, 60, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 61, 62, 25, 1, 1000, 1.20343e-007");
            file.WriteLine("  LINE= 62, 63, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 64, 65, 22, 1, 1000, 1.05902e-007");
            file.WriteLine("  LINE= 57, 73, 25, 1, 8750.03, 1.20343e-007");
            file.WriteLine("  LINE= 60, 77, 25, 1, 6750.03, 1.20343e-007");
            file.WriteLine("  LINE= 62, 78, 25, 1, 6749.97, 1.20343e-007");
            file.WriteLine("  LINE= 66, 82, 25, 1, 8749.97, 1.20343e-007");
            file.WriteLine("  LINE= 67, 74, 22, 1, 8250.03, 1.05902e-007");
            file.WriteLine("  LINE= 68, 75, 22, 1, 7750.03, 1.05902e-007");
            file.WriteLine("  LINE= 69, 76, 22, 1, 7250.03, 1.05902e-007");
            file.WriteLine("  LINE= 70, 79, 22, 1, 7249.97, 1.05902e-007");
            file.WriteLine("  LINE= 71, 80, 22, 1, 7749.97, 1.05902e-007");
            file.WriteLine("  LINE= 72, 81, 22, 1, 8249.97, 1.05902e-007");
            file.WriteLine("  LINE= 73, 74, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 74, 75, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 75, 76, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 76, 77, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 78, 79, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 79, 80, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 80, 81, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  LINE= 81, 82, 36, 1, 1400, 1.73294e-007");
            file.WriteLine("  PART=2");
            file.WriteLine("  3.3749e+006, 1.1178e+006, 0, 0, 7.04177e+010, 1.75776e+010, 5.12533e+013");
            file.WriteLine("  6749.8, 6749.8, 125, 125, 0, 0, 0, 0, 8749.8, 2693");
            file.WriteLine("  -6749.8, 6749.8, 6749.8, -6749.8, 0, 0, 0, 0");
            file.WriteLine("  1.30448e+006, 1.84069e+006, 624307, 232862, 7.67716e+011, 1.4197e+012, 5.27132e+013");
            file.WriteLine("  8762.57, 8762.43, 849.548, 1986.45, 0, 0, 0, 0, 8749.93, 1968.45");
            file.WriteLine("  -8749.93, 8750.07, 8750.07, -8749.93, 831.548, 831.548, -1968.45, -1968.45");
            file.WriteLine("  0.33121, NO, NO, 35500, 0.2");
            file.WriteLine("  VERTEX=-6750, 1275, 6749.6, 1275");
            file.WriteLine("  LINE= 0, 1, 250, 1, 706.548, 3.55566e-009");
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
