using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices.Core;

[assembly: CommandClass(typeof(VictoriaBridge.Test))]
namespace VictoriaBridge
{
    class Test
    {




    [CommandMethod("ceshi")]
        public void AttachXDataToSelectionSetObjects()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            Document acDoc = Application.DocumentManager.MdiActiveDocument;

            string appName = "MY_APP";
            string xdataStr = "This is some xdata";

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Request objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt = acDoc.Editor.GetSelection();

                // If the prompt status is OK, objects were selected
                if (acSSPrompt.Status == PromptStatus.OK)
                {
                    // Open the Registered Applications table for read
                    RegAppTable acRegAppTbl;
                    acRegAppTbl = acTrans.GetObject(acCurDb.RegAppTableId, OpenMode.ForRead) as RegAppTable;

                    // Check to see if the Registered Applications table record for the custom app exists
                    if (acRegAppTbl.Has(appName) == false)
                    {
                        using (RegAppTableRecord acRegAppTblRec = new RegAppTableRecord())
                        {
                            acRegAppTblRec.Name = appName;

                            acTrans.GetObject(acCurDb.RegAppTableId, OpenMode.ForWrite);
                            acRegAppTbl.Add(acRegAppTblRec);
                            acTrans.AddNewlyCreatedDBObject(acRegAppTblRec, true);
                        }
                    }

                    // Define the Xdata to add to each selected object
                    using (ResultBuffer rb = new ResultBuffer())
                    {
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, appName));
                        rb.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, xdataStr));

                        SelectionSet acSSet = acSSPrompt.Value;

                        // Step through the objects in the selection set
                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            // Open the selected object for write
                            Entity acEnt = acTrans.GetObject(acSSObj.ObjectId,
                                                                OpenMode.ForWrite) as Entity;

                            // Append the extended data to each object
                            acEnt.XData = rb;
                        }
                    }
                }

                // Save the new object to the database
                acTrans.Commit();

                // Dispose of the transaction
            }
        }



    }
}
