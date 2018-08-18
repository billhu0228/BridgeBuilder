using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices.Core;
using MyCAD1;
using System.IO;

namespace VictoriaBridge
{
    class Bridge
    {
        // private
        List<DBPoint> NodeList = new List<DBPoint>();
        List<Line> ElemList = new List<Line>();
        Dictionary<int,double> ArchHeightList = new Dictionary<int,double>();
        Dictionary<int, double> ArchThickList = new Dictionary<int, double>();
        int GlobalNodeId = 1;
        int GlobalElemId = 1;
        int GlobalSectId = 1;
        double[] Xlist=new double[] { };
        double []Zlist = new double[] { };

        //Static Geometry
        static double BeamStep=2500;
        static int BeamNpts = 72;
        static int CableStep = 4;
        static int CableNpts = 16;
        static double CrossBeamLength = 6000;
        static double TapeBeamLength = 4000;

        // public paras
        double Length = BeamStep * BeamNpts + (CrossBeamLength + TapeBeamLength) * 2;


        /// <summary>
        /// 构造函数
        /// </summary>
        public Bridge()
        {
            Xlist = GetXList();
            Zlist = new double[] {0,-17900,-35800};
            GenArchPoints();
            GenBeamPoints();
            GenLink();
            return;
        }







        /// <summary>
        /// 初始化x序列
        /// </summary>
        /// <returns></returns>
        double [] GetXList()
        {            
            List<double> part1 = new List<double> { 0, CrossBeamLength };
            List<double> part2 = Linsteps(CrossBeamLength + TapeBeamLength, CrossBeamLength + TapeBeamLength + BeamStep * BeamNpts, BeamStep).ToList();
            List<double> part3 = new List<double> { Length - CrossBeamLength, Length };
            part1.AddRange(part2);
            part1.AddRange(part3);
            return part1.ToArray();
        }


        




        


        /// <summary>
        /// 线性数组
        /// </summary>
        /// <param name="st"></param>
        /// <param name="ed"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        double [] Linsteps(double st,double ed, double step)
        {
            List<double> tmp=new List<double>();            
            for(int i = 0; st + i * step <= ed;i++)
            {
                tmp.Add(st + i * step);
            }
            return tmp.ToArray();            
        }


        /// <summary>
        /// 生成拱肋节点、单元
        /// </summary>
        /// <returns></returns>
        void GenArchPoints()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            DBPoint Ni, Nj;
            Line Elem=new Line();
            DBPoint Node;
            for(int j = 0; j < 2; j++)
            {
                foreach (double x0 in Xlist)
                {
                    double xx = x0 / 1000;
                    double yy = 0.9397 * Math.Sqrt(200 * xx - xx * xx + 7963.93) - 0.9397 * Math.Sqrt(7963.93);
                    double zz = 0;
                    if (j == 0)
                    {
                        zz = 0.3420 * Math.Sqrt(200 * xx - xx * xx + 7963.93) - 0.3420 * Math.Sqrt(7963.93);                        
                    }
                    else
                    {
                        zz = 0.3420 * Math.Sqrt(200 * xx - xx * xx + 7963.93) - 0.3420 * Math.Sqrt(7963.93);
                        zz = -zz - 35.8;
                    }
                    Point3d pt = new Point3d(xx * 1000, yy * 1000, zz * 1000);
                    Node = new DBPoint(pt);
                    Node.SetNodeId(db, GlobalNodeId);
                    NodeList.Add(Node);
                    GlobalNodeId++;

                    // 单元
                    Nj = Node;
                    if (x0 != Xlist[0])
                    {                        
                        var previesid = from n in NodeList where n.GetID() == Nj.GetID()-1 select n;
                        Ni = previesid.ToList()[0];
                        Elem = new Line(Ni.Position, Nj.Position);
                        int nk = j == 0 ? 9999 : 8888;
                        Elem.SetElemXData(db, Ni.GetID(), Nj.GetID(), nk, GlobalElemId,"拱肋");
                        ElemList.Add(Elem);
                        double dx = Math.Max(Math.Abs(Nj.Position.X - 100000), Math.Abs(Ni.Position.X - 100000));                        
                        double height = 3000 + dx/100000 * 500;
                        ArchHeightList.Add(GlobalElemId, height);
                        if (Math.Abs(x0 - 100000) >= 40000)
                        {
                            ArchThickList.Add(GlobalElemId, 70);
                        }
                        else if (Math.Abs(x0 - 100000) >= 20000)
                        {
                            ArchThickList.Add(GlobalElemId, 60);
                        }
                        else
                        {
                            ArchThickList.Add(GlobalElemId, 50);
                        }
                            GlobalElemId++;
                    }
                }
            }

            return;
        }



        void GenLink()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            DBPoint Ni = new DBPoint();
            DBPoint Nj=new DBPoint();
            Line Elem = new Line();
            foreach (double x0 in Xlist)
            {
                for(int j = 0; j < 2; j++)
                {
                    if (x0 >= 20000 && x0 <= 180000 && j == 0)
                    {
                        if ((x0 - 20000) % (CableStep * BeamStep) == 0)
                        {
                            var nisel = from nn in NodeList
                                        where nn.Position.X == x0 && nn.Position.Y != 0 && nn.Position.Z > 0
                                        select nn;
                            Ni = nisel.ToList()[0];
                            var njsel = from nn in NodeList
                                        where nn.Position.X == x0 && nn.Position.Y == 0 && nn.Position.Z == 0
                                        select nn;
                            Nj = njsel.ToList()[0];
                        }
                    }
                    else if (x0 >= 20000 && x0 <= 180000 && j != 0)
                    {
                        if ((x0 - 20000) % (CableStep * BeamStep) == 0)
                        {
                            var nisel = from nn in NodeList
                                        where nn.Position.X == x0 && nn.Position.Y != 0 && nn.Position.Z < 0
                                        select nn;
                            Ni = nisel.ToList()[0];
                            var njsel = from nn in NodeList
                                        where nn.Position.X == x0 && nn.Position.Y == 0 && nn.Position.Z == -35800
                                        select nn;
                            Nj = njsel.ToList()[0];
                        }
                    }
                    else
                    {
                        continue;
                    }
                    Elem = new Line(Ni.Position, Nj.Position);
                    Elem.SetElemXData(db, Ni.GetID(), Nj.GetID(), 0, GlobalElemId, "吊杆");
                    ElemList.Add(Elem);
                    GlobalElemId++;
                }
    
            }
        }


        /// <summary>
        /// 生成桥面系
        /// </summary>
        void GenBeamPoints()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            DBPoint Ni, Nj;
            Line Elem = new Line();
            DBPoint Node;
            foreach(double x0 in Xlist)
            {
                foreach(double z0 in Zlist)
                {
                    if ((x0 == Xlist[0] || x0 == Xlist[Xlist.Length - 1]) && (z0 != Zlist[1]))
                    {
                        continue;
                    }
                    Node = new DBPoint(new Point3d(x0, 0, z0));
                    Node.SetNodeId(db, GlobalNodeId);
                    NodeList.Add(Node);
                    GlobalNodeId++;
                }
            }

            foreach(double x0 in Xlist)
            {
                foreach(double z0 in Zlist)
                {
                    if (z0 == Zlist[0])
                    {
                        continue;
                    }
                    else
                    {
                        var njsel = from nn in NodeList
                                    where nn.Position.X == x0 && nn.Position.Z == z0
                                    select nn;
                        Nj = njsel.ToList()[0];

                        int index = Zlist.ToList().FindIndex(item => item.Equals(z0));
                        var nisel = from nj in NodeList
                                    where nj.Position.X == x0 && nj.Position.Z == Zlist[index - 1]
                                    select nj;
                        Ni = nisel.ToList()[0];
                        Elem = new Line(Ni.Position, Nj.Position);
                        Elem.SetElemXData(db, Ni.GetID(), Nj.GetID(),7777,GlobalElemId, "刚臂");
                        ElemList.Add(Elem);
                        GlobalElemId++;
                    }

                }
            }
            foreach (double x0 in Xlist)
            {
                double z0 = -17900;
                if (x0 != Xlist[0])
                {
                    var njsel = from nn in NodeList
                                where nn.Position.X == x0 && nn.Position.Z == z0
                                select nn;
                    Nj = njsel.ToList()[0];
                    int index = Xlist.ToList().FindIndex(item => item.Equals(x0));
                    var nisel = from nj in NodeList
                                where nj.Position.X == Xlist[index - 1] && nj.Position.Z == z0
                                select nj;
                    Ni = nisel.ToList()[0];
                    Elem = new Line(Ni.Position, Nj.Position);
                    Elem.SetElemXData(db, Ni.GetID(), Nj.GetID(), 7777, GlobalElemId, "主梁");

                    //if (z0==Zlist[2])
                    //{
                    //    Elem.SetElemXData(db, Ni.GetID(), Nj.GetID(),7777, GlobalElemId,"主梁");
                    //}
                    //else
                    //{
                    //    Elem.SetElemXData(db, Ni.GetID(), Nj.GetID(),7777,GlobalElemId, "刚臂");
                    //}

                    ElemList.Add(Elem);
                    GlobalElemId++;
                }


            }

        }


        public void ToDwg(string filepath)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;            
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                foreach(DBPoint pt in NodeList)
                {
                    modelSpace.AppendEntity(pt);
                    tr.AddNewlyCreatedDBObject(pt, true);
                }
                tr.Commit();
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                foreach (Line li in ElemList)
                {
                    modelSpace.AppendEntity(li);
                    tr.AddNewlyCreatedDBObject(li, true);
                }
                tr.Commit();
            }         

            db.SaveAs(filepath, DwgVersion.Current);
            return;
        }


        /// <summary>
        /// 输出ansys命令流
        /// </summary>
        /// <param name="filepath"></param>
        public void ToAnsys_Node(string filepath)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "Node.inp"),false))
            {
                file.WriteLine("/prep7");
                file.WriteLine("n,7777,100000,100000,-17900");
                file.WriteLine("n,8888,100000,{0},{1}", 100 * Math.Sin(70 / 180 * Math.PI), -35800-100 * Math.Cos(70 / 180 * Math.PI));
                file.WriteLine("n,9999,100000,{0},{1}",100*Math.Sin(70/180*Math.PI), 100 * Math.Cos(70 / 180 * Math.PI));
                
                foreach (DBPoint pt in NodeList)
                {
                    short n = (short)pt.XData.AsArray()[0].Value;
                    file.WriteLine(string.Format("n,{0},{1:F3},{2:F3},{3:F3}",n,pt.Position.X,pt.Position.Y,pt.Position.Z));
                }                               
            }
        }

        public void ToAnsys_Elem(string filepath)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "Elem.inp"), false))
            {
                file.WriteLine("/prep7");

                foreach (Line li in ElemList)
                {
                    short ni = (short)li.XData.AsArray()[0].Value;
                    short nj = (short)li.XData.AsArray()[1].Value;
                    short nk = (short)li.XData.AsArray()[2].Value;
                    if ((string)li.XData.AsArray()[4].Value == "主梁")
                    {
                        file.WriteLine("mat,1");
                        file.WriteLine("type,1");
                        file.WriteLine("secn,200");
                        file.WriteLine(string.Format("e,{0},{1},{2}", ni, nj, nk));
                    }
                    else if ((string)li.XData.AsArray()[4].Value == "刚臂")
                    {
                        file.WriteLine("mat,9");
                        file.WriteLine("type,1");
                        file.WriteLine("secn,202");
                        file.WriteLine(string.Format("e,{0},{1},{2}", ni, nj, nk));
                    }
                    else if ((string)li.XData.AsArray()[4].Value == "吊杆")
                    {
                        file.WriteLine("mat,2");
                        file.WriteLine("type,2");
                        file.WriteLine("secn,201");
                        file.WriteLine(string.Format("e,{0},{1}", ni, nj));
                    }
                    else if ((string)li.XData.AsArray()[4].Value == "拱肋")
                    {
                        file.WriteLine("mat,1");
                        file.WriteLine("type,1");
                        file.WriteLine("secn,{0}", li.XData.AsArray()[3].Value);
                        file.WriteLine(string.Format("e,{0},{1},{2}", ni, nj, nk));
                    }

                }
            }
        }

        public void ToAnsys_Sect(string filepath)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "Section.inp"), false))
            {
                file.WriteLine("/prep7");

                foreach (var item in ArchHeightList)
                {
                    double thick = ArchThickList[item.Key];
                    file.WriteLine("SECTYPE,{0},BEAM,HREC,,0",item.Key);
                    file.WriteLine("SECDATA,{0:F2},{1:F2},{2:F2},{2:F2},{2:F2},{3:F2}",
                        item.Value, item.Value, thick, thick, thick, thick);
                }
            }
        }


    }
}
