using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB.Structure;
using Document = Autodesk.Revit.DB.Document;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace IBIMS_MEP
{
    [Transaction(TransactionMode.Manual)]


    public class ViewPortsMove : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet element)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.Creation.Application app = commandData.Application.Application.Create;

            DialogResult td(object g)
            {
                return MessageBox.Show(g + " ");
            }

            IList<Reference> reffs = uidoc.Selection.PickObjects(ObjectType.Element);
            sheets shform = new sheets();   
            IList<Element> allshets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).ToList();
            foreach(Element el in allshets)
            {
                shform.shets.Add(((ViewSheet)el).SheetNumber + "  " + ((ViewSheet)el).Name); 
            }
            shform.ShowDialog();
            ViewSheet TarVS  = allshets[shform.i] as ViewSheet;
            ViewSheet VSH = doc.ActiveView as ViewSheet;
            if(VSH == null) { td("Please Goto ViewSheet First.");return Result.Cancelled; }

            using (Transaction tr = new Transaction(doc, "test"))
            {
                tr.Start();
                foreach (Reference r in reffs)
                {
                    Viewport vp = doc.GetElement(r) as Viewport;
                    if(vp != null)
                    {
                        XYZ P = vp.GetBoxCenter();
                        View v = doc.GetElement(vp.ViewId) as View;
                        doc.Delete(vp.Id);
                        Viewport newvp = Viewport.Create(doc, TarVS.Id, v.Id, P);
                    }
                }
                tr.Commit();
            }
            

            return Result.Succeeded;
        }
    }
}
