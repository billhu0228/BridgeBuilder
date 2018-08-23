using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using MyCAD1;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(VictoriaBridge.Program))]


namespace VictoriaBridge
{
    class Program
    {
        [CommandMethod("main")]
        public static void Main()
        {
            // 基本句柄
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Point3d pt = Point3d.Origin.Convert3D();
            Bridge bridge = new Bridge();
            //bridge.GeomToCad("Geom.dwg");
            //bridge.ToDwg("Node.dwg");
            bridge.ToMidasMct(@"F:\VicFallsBridge\Midas\斜拉桥方案\");
            bridge.ToMidasMct_CableForce(@"F:\VicFallsBridge\Midas\斜拉桥方案\");


            ed.WriteMessage("LoveKitty");
            
        }
    }
}
