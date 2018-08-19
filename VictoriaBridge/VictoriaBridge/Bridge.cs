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
        List<DBPoint> NodeList = new List<DBPoint>();
        List<Line> ElemList = new List<Line>();
        Dictionary<int, double> TowerHeightList = new Dictionary<int, double>();
        Dictionary<int, double> TowerThickList = new Dictionary<int, double>();

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

            BackLineA = new Arc(new Point3d(90073.5, 0, -35567.4), 73181.6, 151.661 / 180.0 * Math.PI, 184.248 / 180.0 * Math.PI);
            BackLineB = new Arc(new Point3d(205573.2, 0, -76400.8), 195138.5, 137.631 / 180.0 * Math.PI, 157.215 / 180.0 * Math.PI);
            BackLineC = new Line(new Point3d(61400.7, 0, 55103.2), new Point3d(79099.4, 0, 76150.9));
            BackLineA.Normal = new Vector3d(0, -1, 0);
            BackLineB.Normal = new Vector3d(0, -1, 0);
            BackLineC.Normal = new Vector3d(0, -1, 0);
            FrontA = new Line(new Point3d(32718.6, 0, -40988), new Point3d(44730.9, 0, -18943.2));
            FrontB = new Arc(new Point3d(70575.2, 0, -35198.2), 30531.2, 96.971 / 180.0 * Math.PI, 148.0 / 180.0 * Math.PI);
            FrontC = new Line(new Point3d(66869.8, 0, -4892.7), new Point3d(58014.8, 0, -1873.8));
            FrontD = new Arc(new Point3d(64515.1, 0, 17133.5), 20088.1, 179.908 / 180.0 * Math.PI, 251.12 / 180.0 * Math.PI);
            FrontE = new Arc(new Point3d(109213.1, 0, 15539.8), 64804, 157.104 / 180.0 * Math.PI, 178.609 / 180.0 * Math.PI);

            FrontA.Normal = new Vector3d(0, -1, 0);
            FrontB.Normal = new Vector3d(0, -1, 0);
            FrontC.Normal = new Vector3d(0, -1, 0);
            FrontD.Normal = new Vector3d(0, -1, 0);
            FrontE.Normal = new Vector3d(0, -1, 0);

        }


        void GenerateTower()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;            
            Line Elem = new Line();
            DBPoint Node;

            List<double> tmp = Linsteps(FrontA.StartPoint.Z, FrontA.EndPoint.Z, ElemSize).ToList();
            tmp = tmp.Concat(Linsteps(FrontA.EndPoint.Z, FrontB.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontB.StartPoint.Z, FrontC.EndPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontC.EndPoint.Z, FrontD.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontD.StartPoint.Z, FrontE.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(FrontE.StartPoint.Z, BackLineB.StartPoint.Z, ElemSize)).ToList();
            tmp = tmp.Concat(Linsteps(BackLineB.StartPoint.Z, BackLineC.EndPoint.Z, 1913.4)).ToList();

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
                if (GlobalElemId>84)
                {
                    ;
                }
                xback = GetBackXCoord(zi);
                xfront = GetFrontXCoord(zi);
                thisLayer.Clear();
                if (zi <= 0)
                {
                    yleft = -11000;
                    yright = 11000;
                }
                else
                {
                    yleft =- Math.Sqrt(zi * (-11000 * 11000) / 76150.9 + 11000 * 11000);
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
            List<double> towerX = Linsteps(BackLineA.StartPoint.X, FrontC.StartPoint.X, ElemSize).ToList();
            double dx = FrontC.StartPoint.X - BackLineA.StartPoint.X;
            double dz = FrontC.StartPoint.Z - BackLineA.StartPoint.Z;
            int ni=0, nj=0;
            for (double yy = -11000; yy<= 11000; yy += 22000)
            {
                foreach (double xx in towerX)
                {
                    double zz = BackLineA.StartPoint.Z + (xx - BackLineA.StartPoint.X) / dx * dz;
                    pt = new Point3d(xx, yy, zz);
                    Node = new DBPoint(pt);
                    if (xx == towerX[0])
                    {
                        ni = GetClosestDBP(Node);
                        continue;
                    }
                    if (IsInNodeList(Node)<0)
                    {
                        Node.SetNodeId(db, GlobalNodeId);
                        NodeList.Add(Node);
                        GlobalNodeId++;                       
                    }
                    nj = IsInNodeList(Node);
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
        }




        void GenerateMainBeam()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Line Elem = new Line();
            DBPoint Node = new DBPoint();
            Point3d pt = new Point3d();
            double zz = FrontC.EndPoint.Z;
            double keyx1 = GetBackXCoord(zz);
            double keyx2 = GetFrontXCoord(zz);

            List<double> tmp = Linsteps(0, keyx1, 2000).ToList();
            tmp = tmp.Concat(Linsteps(keyx1, keyx2, 2000)).ToList();
            tmp = tmp.Concat(Linsteps(keyx2, 90600, 2000)).ToList();
            tmp = tmp.Concat(Linsteps(90600, 213000, 13600/4)).ToList();
            tmp = tmp.Concat(Linsteps(213000, 220000, 2000)).ToList();


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
                pt = new Point3d(xx, 0, zz);
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
                if((xx>= 90600 && xx<=213000 && (xx - 90600) % 13600 == 0))
                {
                    for(double yy = -7750; yy <= 15500; yy += 15500)
                    {
                        pt = new Point3d(xx, yy, zz);
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
                        pt = new Point3d(xx, yy, 0);
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
            
   
            List<double> earthxlist = Linsteps(-18000,-63000,-5000).ToList();
            List<double> towerzlist = Linsteps(BackLineB.StartPoint.Z, BackLineB.StartPoint.Z+9* 1913.4, 1913.4).ToList();
            List<double> beamxlist = Linsteps(90600, 213000, 13600).ToList();
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
                    double ty = dir*Math.Sqrt(tz * (-11000 * 11000) / 76150.9 + 11000 * 11000);
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
                t = 40;
            }
            else 
            {
                h = 2;
                t = 32;
            }
            return new double[] { 1000 * h, t };

        }

        double[] GetBackTowerSection(double zz)
        {
            double h = 0;
            double t = 0;
            if (zz <= BackLineA.StartPoint.Z)
            {
                h = 8 - (zz - BackLineA.EndPoint.Z) / (BackLineA.StartPoint.Z - BackLineA.EndPoint.Z) * 2;
                t = 60 - (zz - BackLineA.EndPoint.Z) / (BackLineA.StartPoint.Z - BackLineA.EndPoint.Z) * 20;
            }
            else if  (zz <= BackLineB.StartPoint.Z)
            {
                h=6- (zz - BackLineB.EndPoint.Z) / (BackLineB.StartPoint.Z - BackLineB.EndPoint.Z) * 2;
                t = 40;
            }
            else
            {
                h = 4;
                t = 40;
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
        double[] Linsteps(double st, double ed, double step, bool includeEnd = true)
        {
            List<double> tmp = new List<double>();
            for (int i = 0; Math.Abs(ed-(st + i * step) )>=Math.Abs( step); i++)
            {
                tmp.Add(st + i * step);
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





        public void ToMidasMct(string filepath)
        {
            
            using (StreamWriter file = new StreamWriter(Path.Combine(filepath, "VictoriaBridge.mct"), 
                false, System.Text.Encoding.GetEncoding("gbk")))
            {
                //==============================================================================================
                // Begin
                //==============================================================================================
                file.WriteLine("*UNIT");
                file.WriteLine("N, mm, KJ, C");
                file.WriteLine("*MATERIAL");
                file.WriteLine("1, CONC , C50+  , 0, 0, , C, NO, 0.05, 2, 3.45e4,0.2, 1.0000e-5, {0:E4},0", (2.7e-5));
                file.WriteLine("2, STEEL, Q420  , 0, 0, , C, NO, 0.02, 1, GB12(S)    ,       , Q420");
                file.WriteLine("3, STEEL, Q345  , 0, 0, , C, NO, 0.02, 1, GB12(S)    ,       , Q345");
                file.WriteLine("4, STEEL, Q345+ , 0, 0, , C, NO, 0.02, 2, 2.06e5,0.3, 1.2000e-5,{0:E4},0", (7.698e-5 * 1.12));
                file.WriteLine("*LOAD-GROUP");
                file.WriteLine("自重\n");
                //==============================================================================================
                // Section
                //==============================================================================================
                file.WriteLine("*SECTION");
                foreach(int key in TowerHeightList.Keys)
                {
                    if ((string)ElemList[GetElemListIndex(key)].XData.AsArray()[4].Value == "桥塔纵梁")
                    {
                        file.WriteLine("{0},DBUSER,TowerS-{1},CB,0,0,0,0,0,0,YES,BSTF,2,{2:F3},4000,{3:F3},{4:F3}," +
                            "500,220,22,500,220,22,7,{5}", key, key, TowerHeightList[key], TowerThickList[key], TowerThickList[key],
                            ((int)TowerHeightList[key] / 500.0));
                    }
                    else
                    {
                        file.WriteLine("{0},DBUSER,TowerS-{1},CC,0,0,0,0,0,0,YES,BSTF,2,{2:F3},4000,{3:F3},{4:F3}," +
                            "500,220,22,500,220,22,7,{5}", key, key, TowerHeightList[key], TowerThickList[key], TowerThickList[key],
                            ((int)TowerHeightList[key] / 500.0));
                    }


                }
                file.WriteLine("501, DBUSER, cable, CC, 0, 0, 0, 0, 0, 0, YES, SR , 2, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0");
                //467, DBUSER    , cable             , CC, 0, 0, 0, 0, 0, 0, YES, SR , 2, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0

                //file.WriteLine("{0},DBUSER,TowerS-{},CC,0,0,0,0,0,0,YES,BSTF,2,{高度},4000,{顶底板厚度},{腹板}," +
                // "500,220,22,500,220,22,7,{腹板加劲个数}");
                //==============================================================================================
                // Node
                //==============================================================================================
                file.WriteLine("*NODE");
                foreach(DBPoint node in NodeList)
                {
                    int n =(short) node.XData.AsArray()[0].Value;
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
                        file.WriteLine("{0},BEAM,4,12,{1},{2},0", e, ni, nj, GetFixAng(elem));
                    }
                    else if (typestring == "刚臂")
                    {
                        file.WriteLine("{0},BEAM,4,99,{1},{2},0", e, ni, nj);
                    }
                    else if (typestring == "主索")
                    {
                        file.WriteLine("{0},TENSTR,4,501,{1},{2},0,1,0,0,NO", e, ni, nj);
                    }
                    else if (typestring == "背索")
                    {
                        file.WriteLine("{0},TENSTR,4,501,{1},{2},0,1,0,0,NO", e, ni, nj);
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
                var nsel = from nd in NodeList where nd.Position.Z == FrontA.StartPoint.Z select nd.XData.AsArray()[0].Value;
                foreach(short ni in nsel)
                {
                    string fixid = "111111";
                    file.WriteLine("{0},{1},",ni, fixid);
                }

                nsel = from nd in NodeList where nd.Position.X <0 select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "111111";
                    file.WriteLine("{0},{1},", ni, fixid);
                }

                nsel = from nd in NodeList where (nd.Position.X ==0) select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "111000";
                    file.WriteLine("{0},{1},", ni, fixid);
                }

                nsel = from nd in NodeList where (nd.Position.X == 220000)  select nd.XData.AsArray()[0].Value;
                foreach (short ni in nsel)
                {
                    string fixid = "011000";
                    file.WriteLine("{0},{1},", ni, fixid);
                }
                file.WriteLine("*ELASTICLINK");
                double zz = FrontC.EndPoint.Z;
                double keyx1 = GetBackXCoord(zz);
                double keyx2 = GetFrontXCoord(zz);
                nsel = from nd in NodeList where (nd.Position.X == keyx1)&&(nd.Position.Y==11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 1,{0},{1},GEN,0,10e4,0, 0, 0, 0, 0, NO, 0.5, 0.5,",nsel.ElementAt(0),nsel.ElementAt(1));
                nsel = from nd in NodeList where (nd.Position.X == keyx1) && (nd.Position.Y == -11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 2,{0},{1},GEN,0,10e4,0, 0, 0, 0, 0, NO, 0.5, 0.5,", nsel.ElementAt(0), nsel.ElementAt(1));
                nsel = from nd in NodeList where (nd.Position.X == keyx2) && (nd.Position.Y ==11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 3,{0},{1},GEN,0,10e4,0, 0, 0, 0, 0, NO, 0.5, 0.5,", nsel.ElementAt(0), nsel.ElementAt(1));
                nsel = from nd in NodeList where (nd.Position.X == keyx2) && (nd.Position.Y == -11000) select nd.XData.AsArray()[0].Value;
                file.WriteLine(" 4,{0},{1},GEN,0,10e4,0, 0, 0, 0, 0, NO, 0.5, 0.5,", nsel.ElementAt(0), nsel.ElementAt(1));


                //                *ELASTICLINK

                //   1,   158,   414, GEN  ,     0, 1111, 0, 0, 0, 0, 0, NO, 0.5, 0.5, 
                //==============================================================================================
                // Load
                //==============================================================================================
                file.WriteLine("*STLDCASE");
                file.WriteLine("自重 , CS, ");
                file.WriteLine("二期荷载, CS, ");
                file.WriteLine("梯度升温, TPG, ");
                file.WriteLine("梯度降温, TPG, ");
                file.WriteLine("整体升温, T , ");
                file.WriteLine("整体降温, T , ");
                file.WriteLine("有车横风, W , ");
                file.WriteLine("制动力, BRK, ");
                file.WriteLine("混凝土湿重, CS, ");
                file.WriteLine("*LOADTOMASS, XYZ, YES, YES, YES, YES, 9806");
                file.WriteLine("自重, 1, 二期荷载, 1");
                file.WriteLine("*USE-STLD, 自重");
                file.WriteLine("*SELFWEIGHT, 0, 0, -1, 自重");


                //==============================================================================================
                file.WriteLine("*ENDDATA");
            }
        }



    }
}
