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
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VictoriaBridge
{
    class Bridge
    {
        // private
        public List<DBPoint> NodeList = new List<DBPoint>();
        public List<Line> ElemList = new List<Line>();
        Dictionary<int, double> TowerHeightList = new Dictionary<int, double>();
        Dictionary<int, double> TowerThickList = new Dictionary<int, double>();
        public List<int> TowerControlList = new List<int>();
        double ElemSize = 1000;

        int GlobalNodeId = 1;
        int GlobalElemId = 1;
        //int GlobalSectId = 1;
        //double[] Xlist = new double[] { };
        //double[] Zlist = new double[] { };

        //Static Geometry
        static double BeamStep = 2500;
        static int BeamNpts = 72;
        static int CableStep = 4;
        static int CableNpts = 16;
        static double CrossBeamLength = 6000;
        static double TapeBeamLength = 4000;
        double beamlevel = 0;// FrontC.EndPoint.Z;
        double keyx1 = 0;
        double keyx2 = 0;
        double keyx3 = 93000;
        double keyx4 = 213000;
        double cabledx = 12000;
        double tlevel = 91641.1;
        // public paras
        double Length = BeamStep * BeamNpts + (CrossBeamLength + TapeBeamLength) * 2;

        // CadDBobject

        Arc BackLineA, BackLineB;
        Line BackLineC;
        Line FrontA, FrontC;
        Arc FrontB, FrontD, FrontE;


        /// <summary>
        /// 构造函数
        /// </summary>
        public Bridge()
        {
            InitGeometry();
            GenerateTower();
            GenerateTowerBeam();
            GenerateMainBeam();
            GenerateLink();
            return;
        }















        /// <summary>
        ///  初始化几何信息
        /// </summary>
        void InitGeometry()
        {

            BackLineA = new Arc(new Point3d(147559.4, 0, -23239.0), 131668.3, 171.735 / 180.0 * Math.PI, 187.747 / 180.0 * Math.PI);
            BackLineB = new Arc(new Point3d(153875.7, 0, -23499.1), 137958.0, 140.687 / 180.0 * Math.PI, 172.005 / 180.0 * Math.PI);
            BackLineC = new Line(new Point3d(47138.2, 0,63905.2), new Point3d(63110.9, 0,83905.2));
            BackLineA.Normal = new Vector3d(0, -1, 0);
            BackLineB.Normal = new Vector3d(0, -1, 0);
            BackLineC.Normal = new Vector3d(0, -1, 0);
            FrontA = new Line(new Point3d(28645.4, 0, -40988), new Point3d(37494.7, 0, -15140.6));
            FrontB = new Arc(new Point3d(53242.1, 0, -21176.5), 16864.5, 89.897 / 180.0 * Math.PI, 159.028 / 180.0 * Math.PI);
            FrontC = new Line(new Point3d(53272.3, 0,-4312), new Point3d(51422.9, 0, -4007.7));
            FrontD = new Arc(new Point3d(60418.4, 0, 23668.6), 29101.5, 190.992 / 180.0 * Math.PI,251.995 / 180.0 * Math.PI);
            FrontE = new Arc(new Point3d(88527, 0,24300.2), 57012.2, 145.728 / 180.0 * Math.PI,186.224 / 180.0 * Math.PI);

            FrontA.Normal = new Vector3d(0, -1, 0);
            FrontB.Normal = new Vector3d(0, -1, 0);
            FrontC.Normal = new Vector3d(0, -1, 0);
            FrontD.Normal = new Vector3d(0, -1, 0);
            FrontE.Normal = new Vector3d(0, -1, 0);
            beamlevel = 0;// FrontC.EndPoint.Z;
            keyx1 = GetBackXCoord(beamlevel);
            keyx2 = GetFrontXCoord(beamlevel);
        }


        void GenerateTower()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;            
            Line Elem = new Line();
            DBPoint Node;

            List<double> tmp = Linsteps(BackLineA.EndPoint.Z, FrontA.EndPoint.Z, ElemSize).ToList();
            tmp = tmp.Concat(Linsteps(FrontA.EndPoint.Z, FrontB.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontB.StartPoint.Z, FrontC.EndPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontC.EndPoint.Z,0, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(0, FrontD.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontD.StartPoint.Z, FrontE.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontE.StartPoint.Z, BackLineB.StartPoint.Z, 2500)).ToList();
            tmp = tmp.Concat(Linsteps(BackLineB.StartPoint.Z, BackLineC.EndPoint.Z, 2500)).ToList();

            List<double> zlist = new List<double>();
            foreach (double a in tmp)
            {
                if (zlist.Contains(a))
                {
                    continue;
                }
                zlist.Add(a);
            }


            double xback = 0, xfront = 0;
            double yleft = 0, yright = 0;
            List<int> thisLayer = new List<int>();
            List<int> lastLayer = new List<int>();
            
            Point3d pt;

            foreach (double zi in zlist)
            {

                xback = GetBackXCoord(zi);
                xfront = GetFrontXCoord(zi);
                thisLayer.Clear();
                if (zi <= beamlevel)
                {
                    yleft = -11000;
                    yright = 11000;
                }
                else
                {
         
                    yleft = -Math.Sqrt((zi - tlevel) * (11000 * 11000) / (beamlevel - tlevel));
                    yright = -yleft;
                    if(double.IsNaN(yleft) || double.IsNaN(yright))
                    {
                        yleft = 0;
                        yright = 0;
                    }
                }

                if (zi<FrontE.StartPoint.Z)
                {
                    pt = new Point3d(xback, yleft, zi);
                    Node = new DBPoint(pt);
                    if (IsInNodeList(Node) < 0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        thisLayer.Add(GlobalNodeId);
                        GlobalNodeId++;
                    }
                    else
                    {
                        thisLayer.Add(IsInNodeList(Node));
                    }


                    pt = new Point3d(xback, yright, zi);
                    Node = new DBPoint(pt);
                    if (IsInNodeList(Node) < 0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        thisLayer.Add(GlobalNodeId);
                        GlobalNodeId++;
                    }
                    else
                    {
                        thisLayer.Add(IsInNodeList(Node));
                    }

                    pt = new Point3d(xfront, yleft, zi);
                    Node = new DBPoint(pt);
                    if (IsInNodeList(Node) < 0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        thisLayer.Add(GlobalNodeId);
                        GlobalNodeId++;
                    }
                    else
                    {
                        thisLayer.Add(IsInNodeList(Node));
                    }

                    pt = new Point3d(xfront, yright, zi);
                    Node = new DBPoint(pt);
                    if (IsInNodeList(Node) < 0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        thisLayer.Add(GlobalNodeId);
                        GlobalNodeId++;
                    }
                    else
                    {
                        thisLayer.Add(IsInNodeList(Node));
                    }

                }
                else
                {
                    pt = new Point3d(xback, yleft, zi);
                    Node = new DBPoint(pt);
                    if (IsInNodeList(Node) < 0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        thisLayer.Add(GlobalNodeId);
                        GlobalNodeId++;
                    }
                    else
                    {
                        thisLayer.Add(IsInNodeList(Node));
                    }

                    pt = new Point3d(xback, yright, zi);
                    Node = new DBPoint(pt);
                    if (IsInNodeList(Node) < 0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        thisLayer.Add(GlobalNodeId);
                        GlobalNodeId++;
                    }
                    else
                    {
                        thisLayer.Add(IsInNodeList(Node));
                    }

                }
                // 增加桥塔控制点
                if (zi >= beamlevel)
                {
                    TowerControlList.Add(thisLayer[0]);
                }

                if (zi == zlist[0])
                {                    
                    lastLayer = new List<int> (thisLayer);
                    continue;
                }
                else
                {
                    int tmp_counter = 0;
                    int Ni = 0;
                    //foreach(int Ni in thisLayer)
                    foreach (int Nj in lastLayer)
                    {
                        if (lastLayer.Count != thisLayer.Count)
                        {
                            if (tmp_counter == 2 || tmp_counter == 3)
                            {
                                Ni = thisLayer[tmp_counter-2];
                            }
                            else
                            {
                                Ni= thisLayer[tmp_counter];
                            }
                        }
                        else
                        {
                            Ni = thisLayer[tmp_counter];
                        }
                                              
                        Elem = new Line(NodeList[GetNodeListIndex(Nj)].Position, NodeList[GetNodeListIndex(Ni)].Position);
                        string typename;
                        if (tmp_counter == 0 || tmp_counter == 1)
                        {
                            typename = "后塔";
                            TowerHeightList.Add(GlobalElemId, GetBackTowerSection(zi)[0]);
                            TowerThickList.Add(GlobalElemId, GetBackTowerSection(zi)[1]);
                        }
                        else
                        {
                            typename = "前塔";
                            TowerHeightList.Add(GlobalElemId, GetFrontTowerSection(zi)[0]);
                            TowerThickList.Add(GlobalElemId, GetFrontTowerSection(zi)[1]);
                        }

                        Elem.SetElemXData(db, Nj, Ni, 0, GlobalElemId, typename);
                        ElemList.Add(Elem);
                        GlobalElemId++;
                        tmp_counter++;
                    }
                    lastLayer = new List<int>(thisLayer);
                }
            }
        }



        void GenerateTowerBeam()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Line Elem = new Line();
            DBPoint Node=new DBPoint();
            Point3d pt = new Point3d();
            List<double> towerX = Linsteps(GetBackXCoord(FrontB.StartPoint.Z), FrontB.StartPoint.X, ElemSize).ToList();
            //double dx = FrontC.StartPoint.X - BackLineA.StartPoint.X;
            //double dz = FrontC.StartPoint.Z - BackLineA.StartPoint.Z;
            int ni=0, nj=0;
            for (double yy = -11000; yy<= 11000; yy += 22000)
            {
                foreach (double xx in towerX)
                {
                    double zz = FrontB.StartPoint.Z;
                    pt = new Point3d(xx, yy, zz);
                    Node = new DBPoint(pt);
                    if (xx == towerX[0])
                    {
                        ni = GetClosestDBP(Node);
                        continue;
                    }
                    else if (xx == towerX.Last())
                    {
                        nj = GetClosestDBP(Node);
                    }
                    else
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        GlobalNodeId++;
                        nj = IsInNodeList(Node);
                    }
                    
                    if (xx != towerX[0])
                    {                       
                        Elem = new Line(NodeList[GetNodeListIndex(ni)].Position, NodeList[GetNodeListIndex(nj)].Position);
                        string typename="桥塔纵梁";
                        TowerHeightList.Add(GlobalElemId, 5000);
                        TowerThickList.Add(GlobalElemId, 40);
                        Elem.SetElemXData(db, ni, nj, 0, GlobalElemId, typename);
                        ElemList.Add(Elem);
                        GlobalElemId++;                        
                    }
                    ni = nj;
                }
            }




            var nsel = from nd in NodeList where (nd.Position.Z == BackLineC.EndPoint.Z) select nd.XData.AsArray()[0].Value;
            
            Elem = new Line(NodeList[GetNodeListIndex((short)nsel.ElementAt(0))].Position, 
                NodeList[GetNodeListIndex((short)nsel.ElementAt(1))].Position);           
            TowerHeightList.Add(GlobalElemId, 4000);
            TowerThickList.Add(GlobalElemId, 40);
            Elem.SetElemXData(db, (short)nsel.ElementAt(0), (short)nsel.ElementAt(1), 0, GlobalElemId, "后塔");
            ElemList.Add(Elem);
            GlobalElemId++;
        }




        void GenerateMainBeam()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Line Elem = new Line();
            DBPoint Node = new DBPoint();
            Point3d pt = new Point3d();

            List<double> tmp = Linsteps(0, keyx1, 2000).ToList();
            tmp = tmp.Concat(Linsteps(keyx1, keyx2, 2000)).ToList();
            tmp = tmp.Concat(Linsteps(keyx2, keyx3, 2000)).ToList();
            tmp = tmp.Concat(Linsteps(keyx3, keyx4, cabledx / 4)).ToList();
            tmp = tmp.Concat(Linsteps(keyx4, 220000, 2000)).ToList();


            List<double> beamxlist = new List<double>();
            foreach (double a in tmp)
            {
                if (beamxlist.Contains(a))
                {
                    continue;
                }
                beamxlist.Add(a);
            }          

            int ni = GlobalNodeId, nj = 0;

            foreach (double xx in beamxlist)
            {
                pt = new Point3d(xx, 0, beamlevel);
                Node = new DBPoint(pt);
                Node.SetNodeId(db, GlobalNodeId);
                NodeList.Add(Node);
                nj = GlobalNodeId;
                GlobalNodeId++;                

                if (xx != beamxlist[0])
                {
                    Elem = new Line(NodeList[GetNodeListIndex(ni)].Position, NodeList[GetNodeListIndex(nj)].Position);
                    string typename = "主梁";
                    Elem.SetElemXData(db, ni, nj, 0, GlobalElemId, typename);
                    ElemList.Add(Elem);
                    GlobalElemId++;
                }
                ni = nj;


                int nside;
                if((xx>= keyx3 && xx<= keyx4 && (xx - keyx3) % cabledx == 0))
                {
                    for(double yy = -7750; yy <= 15500; yy += 15500)
                    {
                        pt = new Point3d(xx, yy, beamlevel);
                        Node = new DBPoint(pt);
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        nside = GlobalNodeId;
                        GlobalNodeId++;

                        Elem = new Line(NodeList[GetNodeListIndex(nside)].Position, NodeList[GetNodeListIndex(nj)].Position);
                        string typename = "刚臂";
                        Elem.SetElemXData(db, nside, nj, 0, GlobalElemId, typename);
                        ElemList.Add(Elem);
                        GlobalElemId++;

                    }
                }
                if ( xx == keyx1 || xx == keyx2)
                {
                    for (double yy = -11000; yy <= 11000; yy += 22000)
                    {
                        pt = new Point3d(xx, yy, 100);
                        Node = new DBPoint(pt);
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        nside = GlobalNodeId;
                        GlobalNodeId++;

                        Elem = new Line(NodeList[GetNodeListIndex(nside)].Position, NodeList[GetNodeListIndex(nj)].Position);
                        string typename = "刚臂";
                        Elem.SetElemXData(db, nside, nj, 0, GlobalElemId, typename);
                        ElemList.Add(Elem);
                        GlobalElemId++;

                    }
                }

            }

        }












        void GenerateLink()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Line Elem = new Line();
            DBPoint Node = new DBPoint();
            Point3d pt = new Point3d();

            double earthkeyx1 = -30000;
            double earthkeyx2 = -80000;

            List<double> earthxlist = Linsteps(earthkeyx1, earthkeyx2, -5000).ToList();
            List<double> towerzlist = Linsteps(FrontE.StartPoint.Z, FrontE.StartPoint.Z+10*2500,2500).ToList();
            List<double> beamxlist = Linsteps(keyx3, keyx4, cabledx).ToList();
            int nearth, ntower, nbeam;
            int comm = -1;
            foreach (double tz in towerzlist)
            {
                comm++;
                for (int dir = -1; dir <= 2; dir += 2)
                {
                    double ex = earthxlist[comm];
                    double bx = beamxlist[comm];
                    double ey = dir * 11000;

                    pt = new Point3d(ex, ey, 0);
                    Node = new DBPoint(pt);
                    Node.SetNodeId(db, GlobalNodeId);
                    NodeList.Add(Node);
                    nearth = GlobalNodeId;
                    GlobalNodeId++;                    

                    double tx = GetBackXCoord(tz);
                    double ty = dir* Math.Sqrt((tz - tlevel) * (11000 * 11000) / (beamlevel - tlevel));

                    
                    pt = new Point3d(tx, ty, tz);
                    Node = new DBPoint(pt);
                    ntower = GetClosestDBP(Node);

                    pt = new Point3d(bx, ey, 0);
                    Node = new DBPoint(pt);
                    nbeam = GetClosestDBP(Node);

                    if (nbeam == -1 || ntower == -1 || nearth == -1)
                    {
                        ;
                    }
                    Elem = new Line(NodeList[GetNodeListIndex(nearth)].Position, NodeList[GetNodeListIndex(ntower)].Position);
                    Elem.SetElemXData(db, nearth, ntower, 0, GlobalElemId, "背索");
                    ElemList.Add(Elem);
                    GlobalElemId++;

                    Elem = new Line(NodeList[GetNodeListIndex(ntower)].Position, NodeList[GetNodeListIndex(nbeam)].Position);
                    Elem.SetElemXData(db, ntower, nbeam, 0, GlobalElemId, "主索");
                    ElemList.Add(Elem);
                    GlobalElemId++;
                }              


            }
        }








        int GetClosestDBP(DBPoint pt)
        {
            var ndsel = from nd in NodeList select nd.GetDistTo(pt);
            var idxsel = from nd in NodeList where nd.GetDistTo(pt) == ndsel.Min() select nd;
            if (idxsel.Count() != 0)
            {
                return (short)idxsel.First().XData.AsArray()[0].Value;
            }
            return -1;
        }

        int IsInNodeList(DBPoint pt)
        {
            var ndsel = from nd in NodeList where nd.GetDistTo(pt) <1 select nd;
            //var dist = from nd in NodeList where true select nd.GetDistTo(pt);
            if (ndsel.Count()!= 0)
            {
                return (short)ndsel.First().XData.AsArray()[0].Value;
            }            
            return -1;           
        }





        double[] GetFrontTowerSection(double zz)
        {
            double h = 0;
            double t = 0;
            if (zz <= FrontA.EndPoint.Z)
            {
                h = 4.3 - (zz - FrontA.StartPoint.Z) / (FrontA.EndPoint.Z - FrontA.StartPoint.Z) * 2;
                t = 40 ;
            }
            else if (zz <= FrontB.StartPoint.Z)
            {
                h = 2.3;
                t = 36;
            }
            else 
            {
                h = 2;
                t = 28;
            }
            return new double[] { 1000 * h, t };

        }

        double[] GetBackTowerSection(double zz)
        {
            double h = 0;
            double t = 0;
            if (zz <= BackLineA.StartPoint.Z)
            {
                h = 8 -   (zz - BackLineA.EndPoint.Z) / (BackLineA.StartPoint.Z - BackLineA.EndPoint.Z) * 2;
                t = 55;
            }
            else if  (zz <= BackLineB.StartPoint.Z)
            {
                h=6- (zz - BackLineB.EndPoint.Z) / (BackLineB.StartPoint.Z - BackLineB.EndPoint.Z) * 2;
                t = 55-(zz - BackLineB.EndPoint.Z) / (BackLineB.StartPoint.Z - BackLineB.EndPoint.Z)*19;
            }
            else
            {
                h = 4;
                t = 36;
            }
            return new double[] { 1000*h, t };

        }

        double GetBackXCoord(double zz)
        {
            Line Ruler = new Line(new Point3d(0, 0, zz), new Point3d(1, 0, zz));
            Point3dCollection pts = new Point3dCollection();
            pts.Clear();
            Ruler.IntersectWith(BackLineA, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);
            if (pts.Count != 0)
            {
                return pts[0].X;
            }
            else
            {
                pts.Clear();
                Ruler.IntersectWith(BackLineB, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);
                if (pts.Count != 0)
                {
                    return pts[0].X;
                }
                else
                {
                    pts.Clear();
                    Ruler.IntersectWith(BackLineC, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);
                    if (pts.Count != 0)
                    {
                        return pts[0].X;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }


        double GetFrontXCoord(double zz)
        {
            Line Ruler = new Line(new Point3d(0, 0, zz), new Point3d(1, 0, zz));
            Point3dCollection pts = new Point3dCollection();


            foreach (Line fl in new List<Line> { FrontA, FrontC })
            {
                pts.Clear();
                Ruler.IntersectWith(fl, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);
                if (pts.Count != 0)
                {
                    return pts[0].X;
                }

            }
            foreach (Arc fl in new List<Arc> { FrontB, FrontD, FrontE })
            {
                pts.Clear();
                Ruler.IntersectWith(fl, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);
                if (pts.Count != 0)
                {
                    return pts[0].X;
                }

            }
            return 0;

        }




        double GetFixAng(Line Elem)
        {
            Plane plane = new Plane(Point3d.Origin, Elem.GetGenLine().Direction);
            Plane planez = new Plane(Point3d.Origin, Elem.GetGenLine().Direction, Vector3d.ZAxis);
            Plane planex = new Plane(Point3d.Origin, Elem.GetGenLine().Direction, Vector3d.XAxis);
            Vector3d Vz = plane.IntersectWith(planez).Direction;
            Vector3d Vx = plane.IntersectWith(planex).Direction;
            double ang = Vz.GetAngleTo(Vx);
            int dir=Elem.GetGenLine().Direction.Y < 0 ? -1 : 1;
            return dir*ang / Math.PI * 180;
        }


        /// <summary>
        /// 线性数组
        /// </summary>
        /// <param name="st"></param>
        /// <param name="ed"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static double[] Linsteps(double st, double ed, double step, bool includeEnd = true)
        {
            List<double> tmp = new List<double>();
            for (int i = 0; Math.Abs(ed-(st + i * step) )>=Math.Abs( step); i++)
            {
                tmp.Add(st + i * step);
            }
            if (tmp.Count == 0)
            {
                tmp.Add(st);
            }

            if (includeEnd && tmp.Last() != ed)
            {
                tmp.Add(ed);
            }
            return tmp.ToArray();
        }



        int GetNodeIdByPosition(Point3d pst)
        {
            var nisel = from nn in NodeList
                        where (double)nn.Position.DistanceTo(pst) <= 0.05
                        select nn;
            return (short)nisel.First().XData.AsArray()[0].Value;
        }




        int GetElemListIndex(int eID)
        {
            var nisel = from ee in ElemList
                        where (short)ee.XData.AsArray()[3].Value == eID
                        select ee;
            return ElemList.IndexOf(nisel.First());
        }

        int GetNodeListIndex(int ID)
        {
            var nisel = from nn in NodeList
                        where (short)nn.XData.AsArray()[0].Value == ID
                        select nn;
            return NodeList.IndexOf(nisel.First());
        }



        public void GeomToCad(string filepath)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                modelSpace.AppendEntity(BackLineA);
                modelSpace.AppendEntity(BackLineB);
                modelSpace.AppendEntity(BackLineC);
                modelSpace.AppendEntity(FrontA);
                modelSpace.AppendEntity(FrontB);
                modelSpace.AppendEntity(FrontC);
                modelSpace.AppendEntity(FrontD);
                modelSpace.AppendEntity(FrontE);
                tr.AddNewlyCreatedDBObject(BackLineA, true);
                tr.AddNewlyCreatedDBObject(BackLineB, true);
                tr.AddNewlyCreatedDBObject(BackLineC, true);
                tr.AddNewlyCreatedDBObject(FrontA, true);
                tr.AddNewlyCreatedDBObject(FrontB, true);
                tr.AddNewlyCreatedDBObject(FrontC, true);
                tr.AddNewlyCreatedDBObject(FrontD, true);
                tr.AddNewlyCreatedDBObject(FrontE, true);

                tr.Commit();
            }
            db.SaveAs(filepath, DwgVersion.Current);
            return;
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

                foreach (DBPoint pt in NodeList)
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
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "Node.inp"), false))
            {
                file.WriteLine("/prep7");
                file.WriteLine("n,7777,100000,100000,-17900");
                file.WriteLine("n,8888,100000,{0},{1}", 100 * Math.Sin(70 / 180 * Math.PI), -35800 - 100 * Math.Cos(70 / 180 * Math.PI));
                file.WriteLine("n,9999,100000,{0},{1}", 100 * Math.Sin(70 / 180 * Math.PI), 100 * Math.Cos(70 / 180 * Math.PI));

                foreach (DBPoint pt in NodeList)
                {
                    short n = (short)pt.XData.AsArray()[0].Value;
                    file.WriteLine(string.Format("n,{0},{1:F3},{2:F3},{3:F3}", n, pt.Position.X, pt.Position.Y, pt.Position.Z));
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




        public  void ToMidasMct_CableForce(string filepath)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "调索.mct"),
                false, System.Text.Encoding.GetEncoding("gbk")))
            {
                MCTWriter.CableForce(file, this);
            }
        }



        public void ToMidasMct(string filepath)
        {
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "VictoriaBridge.mct"),
                false, System.Text.Encoding.GetEncoding("gbk")))
            {
                //==============================================================================================
                // Begin
                //==============================================================================================
                MCTWriter.Beginning(file);
                //==============================================================================================
                // Section
                //==============================================================================================
                file.WriteLine("*SECTION");
                foreach (int key in TowerHeightList.Keys)
                {
                    if ((string)ElemList[GetElemListIndex(key)].XData.AsArray()[4].Value == "桥塔纵梁")
                    {
                        file.WriteLine("{0},DBUSER,TowerS-{1},CB,0,0,0,0,0,0,YES,NO,BSTF,2,{2:F3},4000,{3:F3},{4:F3}," +
                            "500,220,22,500,220,22,7,{5}", key, key, TowerHeightList[key], TowerThickList[key], TowerThickList[key],
                            ((int)TowerHeightList[key] / 500.0));
                    }
                    else
                    {
                        file.WriteLine("{0},DBUSER,TowerS-{1},CC,0,0,0,0,0,0,YES,NO,BSTF,2,{2:F3},4000,{3:F3},{4:F3}," +
                            "500,220,22,500,220,22,7,{5}", key, key, TowerHeightList[key], TowerThickList[key], TowerThickList[key],
                            ((int)TowerHeightList[key] / 500.0));
                    }
                }
                file.WriteLine("501, DBUSER, cable, CC, 0, 0, 0, 0, 0, 0, YES,NO, SR , 2,100, 0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("503, DBUSER, Rig, CC, 0, 0, 0, 0, 0, 0, YES,NO, SR , 2, 500, 0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("519,DBUSER,cable-19,CC,0,0,0,0,0,0,YES,NO,SR,2, 58, 0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("531,DBUSER,cable-31,CC,0,0,0,0,0,0,YES,NO,SR,2, 74, 0, 0, 0, 0, 0, 0, 0, 0, 0");               
                file.WriteLine("537,DBUSER,C15.2-37,CC,0,0,0,0,0,0,YES,NO,SR,2, 81,0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("543,DBUSER,C15.2-43,CC,0,0,0,0,0,0,YES,NO,SR,2, 87,0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("555,DBUSER,C15.2-55,CC,0,0,0,0,0,0,YES,NO,SR,2, 99,0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("561,DBUSER,C15.2-61,CC,0,0,0,0,0,0,YES,NO,SR,2,104,0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("573,DBUSER,C15.2-73,CC,0,0,0,0,0,0,YES,NO,SR,2,114,0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("585,DBUSER,C15.2-85,CC,0,0,0,0,0,0,YES,NO,SR,2,123,0, 0, 0, 0, 0, 0, 0, 0, 0");
                file.WriteLine("591,DBUSER,C15.2-91,CC,0,0,0,0,0,0,YES,NO,SR,2,127,0, 0, 0, 0, 0, 0, 0, 0, 0");
                MCTWriter.CompositeSection(file);

                //==============================================================================================
                // Node
                //==============================================================================================
                file.WriteLine("*NODE");
                foreach (DBPoint node in NodeList)
                {
                    int n = (short)node.XData.AsArray()[0].Value;
                    file.WriteLine("{0},{1:F3},{2:F3},{3:F3}", n, node.Position.X, node.Position.Y, node.Position.Z);
                }
                file.WriteLine("*ELEMENT");

                foreach (Line elem in ElemList)
                {
                    int e = (short)elem.XData.AsArray()[3].Value;
                    int ni = (short)elem.XData.AsArray()[0].Value;
                    int nj = (short)elem.XData.AsArray()[1].Value;
                    string typestring = (string)elem.XData.AsArray()[4].Value;
                    if (typestring == "主梁")
                    {
                        file.WriteLine("{0},BEAM,4,601,{1},{2},0", e, ni, nj, GetFixAng(elem));
                    }
                    else if (typestring == "刚臂")
                    {
                        file.WriteLine("{0},BEAM,9,503,{1},{2},0", e, ni, nj);
                    }
                    else if (typestring == "主索")
                    {
                        if (elem.Bounds.Value.MaxPoint.X <= 153000)
                        {
                            file.WriteLine("{0},TENSTR,4,531,{1},{2},0,1,0,0,NO", e, ni, nj);
                        }
                        //else if (elem.Bounds.Value.MaxPoint.X <= 153000)
                        //{
                        //    file.WriteLine("{0},TENSTR,4,512,{1},{2},0,1,0,0,NO", e, ni, nj);
                        //}
                        //else if (elem.Bounds.Value.MaxPoint.X <= 189000)
                        //{
                        //    file.WriteLine("{0},TENSTR,4,513,{1},{2},0,1,0,0,NO", e, ni, nj);
                        //}
                        else
                        {
                            file.WriteLine("{0},TENSTR,4,519,{1},{2},0,1,0,0,NO", e, ni, nj);
                        }

                    }
                    else if (typestring == "背索")
                    {
                        if (elem.Bounds.Value.MinPoint.X >= -45000)
                        {
                            file.WriteLine("{0},TENSTR,4,537,{1},{2},0,1,0,0,NO", e, ni, nj);
                        }
                        else
                        {
                            file.WriteLine("{0},TENSTR,4,543,{1},{2},0,1,0,0,NO", e, ni, nj);
                        }

                    }
                    else
                    {
                        file.WriteLine("{0},BEAM,4,{0},{1},{2},{3}", e, ni, nj, GetFixAng(elem));
                    }
                }

                // 575, TENSTR,    1,   467,   374,   515,     0,     1,     0,     0, NO

                //==============================================================================================
                // Constraint
                //==============================================================================================
                file.WriteLine("*CONSTRAINT");
                var nsel = from nd in NodeList where nd.Position.Z == BackLineA.EndPoint.Z select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "111111";
                    file.WriteLine("{0},{1},AllBNDR", ni, fixid);
                }

                nsel = from nd in NodeList where nd.Position.X < 0 select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "111111";
                    file.WriteLine("{0},{1},AllBNDR", ni, fixid);
                }

                nsel = from nd in NodeList where (nd.Position.X == 0) select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "111000";
                    file.WriteLine("{0},{1},AllBNDR", ni, fixid);
                }

                nsel = from nd in NodeList where (nd.Position.X == 220000) select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "011000";
                    file.WriteLine("{0},{1},AllBNDR", ni, fixid);
                }
                file.WriteLine("*ELASTICLINK");
                double zz = FrontC.EndPoint.Z;

                nsel = from nd in NodeList where (nd.Position.X == keyx1) && (nd.Position.Y == 11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 1,{0},{1},GEN,0,10e10,0, 0, 0, 0, 0, NO, 0.5, 0.5,AllBNDR", nsel.ElementAt(0), nsel.ElementAt(1));
                nsel = from nd in NodeList where (nd.Position.X == keyx1) && (nd.Position.Y == -11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 2,{0},{1},GEN,0,10e10,0, 0, 0, 0, 0, NO, 0.5, 0.5,AllBNDR", nsel.ElementAt(0), nsel.ElementAt(1));
                nsel = from nd in NodeList where (nd.Position.X == keyx2) && (nd.Position.Y == 11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 3,{0},{1},GEN,0,10e10,0, 0, 0, 0, 0, NO, 0.5, 0.5,AllBNDR", nsel.ElementAt(0), nsel.ElementAt(1));
                nsel = from nd in NodeList where (nd.Position.X == keyx2) && (nd.Position.Y == -11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 4,{0},{1},GEN,0,10e10,0, 0, 0, 0, 0, NO, 0.5, 0.5,AllBNDR", nsel.ElementAt(0), nsel.ElementAt(1));


                //                *ELASTICLINK
                //   1,   158,   414, GEN  ,     0, 1111, 0, 0, 0, 0, 0, NO, 0.5, 0.5, 
                //==============================================================================================
                // Load
                //==============================================================================================
                file.WriteLine("*GROUP");
                file.WriteLine("AllBridge, 1to{0}, 1to{1}, 0", GlobalNodeId - 1, GlobalElemId - 1);
                MCTWriter.CableIniForce(file, this);
                file.WriteLine("*STLDCASE");
                file.WriteLine("DeadLoad,USER,");  
                file.WriteLine("*LOADTOMASS, XYZ, YES, YES, YES, YES, 9806");
                file.WriteLine("DeadLoad, 1");
                file.WriteLine("*USE-STLD, DeadLoad");
                file.WriteLine("*SELFWEIGHT, 0, 0, -1,AllLoad");
                file.WriteLine("*CONLOAD");
                var nnsel = from nd in NodeList where nd.Position.Y == 0 select nd.XData.AsArray()[0].Value;
                ///人行道板 + 底座：6kN / m
                ///钢护栏：1kN / m
                ///铺装2.4ton/m3
                double ww = (6 + 1 + 1) * 2.0 * 220 * 1000;
                double liqing = 2.4 * 0.1 * 8.2 * 220.0 * 1000 * 9.8;
                double fn = (ww + liqing) / nnsel.Count() * -1.0;
                foreach (short n in nnsel)
                {                 
                    file.WriteLine("{0}, 0, 0, {1:F3}, 0, 0, 0,AllLoad ", n, fn);
                }
                //MCTWriter.CableForceForLC(file,this);
                //==============================================================================================
                file.WriteLine("*ENDDATA");
            }
        }



    }
}
