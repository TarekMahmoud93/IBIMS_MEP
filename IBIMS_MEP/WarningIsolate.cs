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
using System.Xml.Linq;

namespace IBIMS_MEP
{

    [Transaction(TransactionMode.Manual)]


    public class WarningIsolate : IExternalCommand
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

            //IList<Element> SlabEdgs = new FilteredElementCollector(doc).OfClass(typeof(SlabEdge)).ToList();
            //IList<IList<Element>> JoinedSlabEdges = new List<IList<Element>>(); 
            //foreach(Element se in SlabEdgs)
            //{
            //    BoundingBoxXYZ bbx = se.get_BoundingBox(doc.ActiveView);
            //    IList<Element> fiss = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WherePasses(new BoundingBoxIntersectsFilter(new Outline(bbx.Min,bbx.Max))).ToList(); 
            //    IList<Element> floors = new FilteredElementCollector(doc).OfClass(typeof(Floor)).WherePasses(new BoundingBoxIntersectsFilter(new Outline(bbx.Min, bbx.Max))).ToList();
            //    IList<Element> walls = new FilteredElementCollector(doc).OfClass(typeof(Wall)).WherePasses(new BoundingBoxIntersectsFilter(new Outline(bbx.Min, bbx.Max))).ToList();
            //    foreach (Element e in floors) { fiss.Add(e); }
            //    foreach(Element e in walls) { fiss.Add(e); }
            //    JoinedSlabEdges.Add(fiss);
            //}

            IList<ElementId> eids = new List<ElementId>();
            IList<ElementId> eids2 = new List<ElementId>();
            TransactionGroup tg = new TransactionGroup(doc, "CMS");
            tg.Start();

            //using (Transaction tra = new Transaction(doc, "slabedges"))
            //{
            //    tra.Start();int sec = 0;
            //    foreach (Element se in SlabEdgs)
            //    {
            //        foreach (Element e in JoinedSlabEdges[sec])
            //        {
            //            try
            //            {
            //                if (!JoinGeometryUtils.AreElementsJoined(doc, se, e))
            //                {
            //                    JoinGeometryUtils.JoinGeometry(doc, se, e);
            //                }
            //            }
            //            catch{ }
            //        }
            //            sec++;
            //    }
            //    tra.Commit();
            //}

            using (Transaction tr = new Transaction(doc, "CMS"))
            {
                tr.Start();

                foreach (FailureMessage fm in doc.GetWarnings())
                {
                    //if (fm.GetDescriptionText() == "Highlighted walls overlap. One of them may be ignored when Revit finds room boundaries. Use Cut Geometry to embed one wall within the other.")
                    //{
                    //    Element e1 = doc.GetElement(fm.GetFailingElements().ToList()[0]);
                    //    Element e2 = doc.GetElement(fm.GetFailingElements().ToList()[1]);
                    //    if (e1 != null && e2 != null)
                    //    {
                    //        if (e1.Name == "Insulation" && e2.Name == "Insulation")
                    //        {
                    //            double a1 = e1.LookupParameter("Area").AsDouble(); double a2 = e2.LookupParameter("Area").AsDouble();
                    //            if (a1 > a2) { doc.Delete(e2.Id); }
                    //            else { doc.Delete(e1.Id); }
                    //        }
                    //        else if (e1.Name == "Insulation") { doc.Delete(e1.Id); }
                    //        else if (e2.Name == "Insulation") { doc.Delete(e2.Id); }
                    //        else { eids2.Add(e1.Id); eids2.Add(e2.Id); }
                    //    }
                    //}
                    /*else */if (fm.GetDescriptionText() == "Highlighted elements are joined but do not intersect.")
                    {
                        Element e1 = doc.GetElement(fm.GetFailingElements().ToList()[0]);
                        Element e2 = doc.GetElement(fm.GetFailingElements().ToList()[1]);
                        if (e1 != null && e2 != null)
                        {
                            JoinGeometryUtils.UnjoinGeometry(doc, e1, e2);
                        }
                    }
                    else if (fm.GetDescriptionText() == "Elements have duplicate \"Mark\" values.")
                    {
                        foreach (ElementId e in fm.GetFailingElements().ToList())
                        {
                            doc.GetElement(e).LookupParameter("Mark").Set("");
                        }
                    }
                    else if (fm.GetDescriptionText() == "Elements have duplicate \"Type Mark\" values.")
                    {
                        foreach (ElementId e in fm.GetFailingElements().ToList())
                        {
                            doc.GetElement(e).LookupParameter("Type Mark").Set("");
                        }
                    }
                    else if (fm.GetDescriptionText() == "The analytical model for the structural element cannot be created by the auto-detect method. Adjust the analytical element or change the tolerance of the analytical model settings if the model is needed.")
                    {
                        foreach (ElementId e in fm.GetFailingElements().ToList())
                        {
                            eids.Add(e);
                        }
                    }
                    else
                    {
                        foreach (ElementId e in fm.GetFailingElements().ToList())
                        {
                            eids2.Add(e);
                        }
                    }
                    foreach (ElementId e in eids)
                    {
                        doc.GetElement(e).LookupParameter("Enable Analytical Model").Set(1);
                    }
                    
                }
                tr.Commit();
            }

            using (Transaction tr = new Transaction(doc, "CMS"))
            {
                tr.Start();
                doc.ActiveView.IsolateElementsTemporary(eids2);

                tr.Commit();
            }
            tg.Assimilate();
                return Result.Succeeded;
        }
    }
}