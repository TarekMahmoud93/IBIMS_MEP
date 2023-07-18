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
using Autodesk.Revit.DB.Architecture;
using System.IO;

namespace IBIMS_MEP
{
    [Transaction(TransactionMode.Manual)]
    public class ViewPortsRenum : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet element)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.Creation.Application app = commandData.Application.Application.Create;


            TaskDialogResult td(object g)
            {
                return TaskDialog.Show("s", g + " ");
            }
            ViewSheet viewsheet = uidoc.ActiveView as ViewSheet;
            var viewports = viewsheet.GetAllViewports().Select(vp => doc.GetElement(vp) as Viewport).ToList();
            var viewpoints = viewports.Select(vp => vp.GetBoxCenter()).ToList();


            var Yvalues = viewpoints.OrderBy(po => po.Y).Select(vp => vp.Y).ToList();
            Yvalues.Reverse();
            List<double> Yreferences = new List<double>();
            for (int i = 0; i < Yvalues.Count; i++)
            {
                if (i != Yvalues.Count - 1)
                {
                    Yreferences.Add(Yvalues[i] - Yvalues[i + 1]);
                }
            }
            double Yreference = Yreferences.Average();

            var OrderedPointsy = viewpoints.OrderBy(po => po.Y).ToList();
            OrderedPointsy.Reverse();

            List<Viewport> viewportsordered = new List<Viewport>();
            for (int i = 0; i < viewports.Count; i++)
            {
                List<XYZ> PointsToXOrder = new List<XYZ>();
                for (int j = 0; j < OrderedPointsy.Count; j++)
                {
                    if (j != OrderedPointsy.Count - 1)
                    {
                        if (OrderedPointsy[j].Y - OrderedPointsy[j + 1].Y > Yreference)
                        {
                            if (PointsToXOrder.Count != 0)
                            {
                                break;
                            }
                            else
                            {
                                PointsToXOrder.Add(OrderedPointsy[j]);
                                break;
                            }
                        }
                        else
                        {
                            if (j == 0)
                            {
                                PointsToXOrder.Add(OrderedPointsy[j]);
                                PointsToXOrder.Add(OrderedPointsy[j + 1]);
                            }
                            else
                            {
                                PointsToXOrder.Add(OrderedPointsy[j + 1]);
                            }
                        }
                    }
                    else
                    {
                        PointsToXOrder.Add(OrderedPointsy[j]);
                    }
                }

                XYZ PointToAdd = PointsToXOrder.OrderBy(px => px.X).ToList()[0];
                viewportsordered.Add(viewports[viewpoints.IndexOf(PointToAdd)]);
                OrderedPointsy.Remove(PointToAdd);
            }

            Renum rf = new Renum(); int number2 = 1;
            rf.ShowDialog();
            if(rf.DialogResult == System.Windows.Forms.DialogResult.Cancel) { return Result.Cancelled; }
            try { number2 = Convert.ToInt32(rf.textBox1.Text); }
            catch {}
            

            Transaction t = new Transaction(doc, "test");
            {
                t.Start();

                int number = 100000;
                for (int i = 0; i < viewportsordered.Count; i++)
                {
                    try
                    {
                        viewportsordered[i].ParametersMap.get_Item("Detail Number").Set(number.ToString());
                        if(rf.checkBox1.Checked) { doc.GetElement(viewportsordered[i].ViewId).Name = rf.textBox2.Text + number.ToString(); }
                        
                    }
                    catch { }
                    ++number;
                }

                
                for (int i = 0; i < viewportsordered.Count; i++)
                {
                    try
                    {
                        string num = number2.ToString(); 
                        if(number2 < 10) { num = "0" + number2.ToString(); }
                        viewportsordered[i].ParametersMap.get_Item("Detail Number").Set(num.ToString());
                        if (rf.checkBox1.Checked) { doc.GetElement(viewportsordered[i].ViewId).Name = rf.textBox2.Text + num.ToString(); }
                    }
                    catch { }

                    ++number2;
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
