using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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
            Bridge CurBridge=new Bridge();
            //CurBridge.ToAnsys_Node(@"G:\HongKong\1 Ansys\");
            //CurBridge.ToAnsys_Elem(@"G:\HongKong\1 Ansys\");
            //CurBridge.ToAnsys_Sect(@"G:\HongKong\1 Ansys\");
            
            //CurBridge.ToDwg(@"G:\HongKong\1 Ansys\fucking.Dwg");
            //int f = 1;
            
        }
    }
}
