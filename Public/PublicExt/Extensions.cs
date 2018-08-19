using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;


[assembly: CommandClass(typeof(MyCAD1.Extensions))]



namespace MyCAD1
{
    public static class Extensions
    {


        public static double GetDistTo(this DBPoint curDBP, DBPoint tarDBP)
        {
            return curDBP.Position.DistanceTo(tarDBP.Position);
        }

        public static Line3d GetGenLine(this Line aline)
        {
            return new Line3d(aline.StartPoint,aline.EndPoint);
        }

        /// <summary>
        /// 获得Line中点Point3D.
        /// </summary>
        /// <param name="aline">目标线.</param>       

        public static Point3d GetMidPoint3d(this Line aline,double xx=0,double yy=0)
        {
            double x = 0.5 * (aline.StartPoint.X + aline.EndPoint.X);
            double y = 0.5 * (aline.StartPoint.Y + aline.EndPoint.Y);
            return new Point3d(x+xx, y+yy, 0);
        }

        public static Point2d GetXPoint2d(this Line aline,double part=0.5, double xx = 0, double yy = 0)
        {
            double x = aline.StartPoint.X + part * (aline.EndPoint.X - aline.StartPoint.X);
            double y = aline.StartPoint.Y + part * (aline.EndPoint.Y - aline.StartPoint.Y);
            return new Point2d(x + xx, y + yy);
        }
        /// <summary>
        /// 获得中点
        /// </summary>
        /// <param name="aline"></param>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <returns></returns>
        public static Point2d GetMidPoint2d(this Line aline, double xx = 0, double yy = 0)
        {
            double x = 0.5 * (aline.StartPoint.X + aline.EndPoint.X);
            double y = 0.5 * (aline.StartPoint.Y + aline.EndPoint.Y);
            return new Point2d(x+xx, y+yy);
        }

        /// <summary>
        /// 获得线段被矩形切割后线段
        /// </summary>
        /// <param name="aline">原线段</param>
        /// <param name="minPoint">左下角点</param>
        /// <param name="maxPoint">右上角点</param>
        /// <returns>新线段</returns>
        public static Line CutByRect(this Line aline,Point2d minPoint,Point2d maxPoint,bool isInf=true)
        {
            Line res = aline;
            Polyline BD = new Polyline();
            Point3dCollection pts = new Point3dCollection();

            double dx = maxPoint.X - minPoint.X;
            double dy = maxPoint.Y - minPoint.Y;
            BD.AddVertexAt(0, minPoint, 0, 0, 0);
            BD.AddVertexAt(1, minPoint.Convert2D(dx), 0, 0, 0);
            BD.AddVertexAt(2, maxPoint, 0, 0, 0);
            BD.AddVertexAt(3, minPoint.Convert2D(0, dy), 0, 0, 0);
            BD.Closed = true;

            if (aline.StartPoint.X == aline.EndPoint.X)
            {
                if(aline.StartPoint.X==minPoint.X )
                {
                    res.StartPoint = minPoint.Convert3D();
                    res.EndPoint = minPoint.Convert3D(0, dy);
                    return res;
                }
                if (aline.StartPoint.X == maxPoint.X)
                {
                    res.StartPoint = minPoint.Convert3D(dx);
                    res.EndPoint = maxPoint.Convert3D();
                    return res;
                }
            }
            if (isInf)
            {
                BD.IntersectWith(aline, Intersect.ExtendBoth, pts, IntPtr.Zero, IntPtr.Zero);
                res.StartPoint = pts[0];
                res.EndPoint = pts[1];
            }
            else
            {
                BD.IntersectWith(aline, Intersect.OnBothOperands, pts, IntPtr.Zero, IntPtr.Zero);
                switch (pts.Count)
                {
                    case 0:
                        break;

                    case 1:
                        if (aline.StartPoint.X > minPoint.X)
                        {
                            res.StartPoint = pts[0];
                        }
                        else
                        {
                            res.EndPoint = pts[0];
                        }
                        break;

                    case 2:
                        res.StartPoint = pts[0];
                        res.EndPoint = pts[1];
                        break;
                }
            }


            return res;
        }

        public static Line CutByDoubleLine(this Line aline, Point2d minPoint, Point2d maxPoint, bool isInf = true)
        {
            Line res = (Line)aline.Clone() ;
            Line A = new Line(minPoint.Convert3D(), minPoint.Convert3D(1));
            Line B = new Line(maxPoint.Convert3D(), maxPoint.Convert3D(1));
            Point3dCollection pts = new Point3dCollection();

            pts.Clear();
            aline.IntersectWith(A, Intersect.ExtendBoth, pts, IntPtr.Zero, IntPtr.Zero);
            res.StartPoint = pts[0];

            pts.Clear();
            aline.IntersectWith(B, Intersect.ExtendBoth, pts, IntPtr.Zero, IntPtr.Zero);
            res.EndPoint = pts[0];
            
            return res;
        }


        /// <summary>
        /// 获取点号
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static  int GetID(this DBPoint node)
        {
            int res = -1;
            if(node.XData is null)
            {
                return res;
            }
            else
            {
                return (Int16)node.XData.AsArray()[0].Value;
            }
        }


        public static void SetNodeId(this DBPoint pt,Database db,int id)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                RegAppTable acRegAppTbl = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                {
                    acRegAppTblRec.Name = "HKB";
                    tr.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                    acRegAppTbl.Add(acRegAppTblRec);
                    tr.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                }
                using (ResultBuffer rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "HKB"));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, id));
                    pt.XData = rb;
                }
            }
        }

        public static void SetElemXData(this Line li,Database db,int ni,int nj,int nk,int elem_id,string type)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                RegAppTable acRegAppTbl = tr.GetObject(db.RegAppTableId, OpenMode.ForRead) as RegAppTable;
                using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                {
                    acRegAppTblRec.Name = "HKB";
                    tr.GetObject(db.RegAppTableId, OpenMode.ForWrite);
                    acRegAppTbl.Add(acRegAppTblRec);
                    tr.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                }
                using (ResultBuffer rb = new ResultBuffer())
                {
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "HKB"));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16,ni));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, nj));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, nk));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataInteger16, elem_id));
                    rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString,type));
                    li.XData = rb;
                }
            }
        }


        




        /// <summary>
        /// 获得对称
        /// </summary>
        /// <param name="thePline">多线名称</param>       
        /// <param name="axis">对称轴</param>       
        public static Polyline GetMirror(this Polyline thePline, Line2d axis)
        {
            Polyline res = (Polyline)thePline.Clone();
            res.TransformBy(Matrix3d.Mirroring(axis.Convert3D()));
            return res;
        }


        /// <summary>
        /// 根据多线线段编号获取对应直线
        /// </summary>
        /// <param name="thePline"></param>
        /// <param name="SegID">线段编号</param>
        /// <returns>对应直线</returns>
        public static Line GetLine(this Polyline thePline, int SegID)
        {
            var seg = thePline.GetLineSegmentAt(SegID);
            Point3d p1 = seg.StartPoint;
            Point3d p2 = seg.EndPoint;
            Line res = new Line(p1, p2);            
            return res;
        }


        public static Line3d Convert3D (this Line2d theL2d)
        {
            return new Line3d(theL2d.StartPoint.Convert3D(), theL2d.EndPoint.Convert3D());
        }






        public static Point2d Convert2D(this Point3d theP3d, double x = 0, double y = 0)
        {
            return new Point2d(theP3d.X + x, theP3d.Y + y);
        }


        public static Point3d Convert3D(this Point3d theP3d, double x = 0, double y = 0,double z=0)
        {
            return new Point3d(theP3d.X + x, theP3d.Y + y,theP3d.Z+z);
        }


        public static Point3d Convert3D(this Point2d theP2d, double x = 0, double y = 0)
        {
            return new Point3d(theP2d.X + x, theP2d.Y + y,0);
        }

        public static Point2d Convert2D(this Point2d theP2d,double x=0,double y=0)
        {
            return new Point2d(theP2d.X + x, theP2d.Y + y);
        }

        
        public static Point2d MoveDistance(this Point2d theP2d,Vector2d theVec,double dist)
        {
            Vector2d newVec = new Vector2d(dist * Math.Cos(theVec.Angle), dist * Math.Sin(theVec.Angle));            
            return theP2d.TransformBy(Matrix2d.Displacement(newVec));
        }

        public static double GetK(this Line cL)
        {
            double k = 0;
            k = (cL.EndPoint.Y - cL.StartPoint.Y) / (cL.EndPoint.X - cL.StartPoint.X);
            return k;
        }
        



        






              



    }

}