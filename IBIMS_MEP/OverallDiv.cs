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
using View = Autodesk.Revit.DB.View;
using System.Windows.Forms;

namespace IBimsAddins
{
    [Transaction(TransactionMode.Manual)]
    public class OverallDiv : IExternalCommand
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

            bool LUP(ViewSheet VS, string para, string value)
            {
                bool bol = true;
                try
                {
                    bol = VS.LookupParameter(para).Set(value);
                }
                catch { }
                return bol;
            }

            Form5 f5 = new Form5();

            IList<Element> fecv = new FilteredElementCollector(doc).OfClass(typeof(View)).ToList();
            List<Element> viewports = new List<Element>(); List<string> viewportsnms = new List<string>();
            foreach (Element e in fecv)
            {
                if (((View)e).ViewType == ViewType.Legend)
                {
                    viewports.Add(e); viewportsnms.Add(((View)e).Title);
                }
            }
            IList<Element> fecty = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).ToList();
            ElementType et = null;
            foreach (Element e in fecty)
            {
                if (e.Name == "NO TITLE")
                {
                    et = e as ElementType;break;
                }
            }
            IList<Element> fecvps = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).ToList();
            List<Element> Dupviews = new List<Element>(); List<string> Dupviewsnms = new List<string>();
            List<Element> templatviews = new List<Element>(); List<string> templatviewsnms = new List<string>();
            foreach (Element e in fecvps)
            {
                View v = e as View;
                if (v.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
                {
                    Dupviews.Add(v); Dupviewsnms.Add(v.Title);
                }
                if (v.IsTemplate)
                {
                    templatviews.Add(v); templatviewsnms.Add(v.Title);
                }
            }
            
            FilteredElementCollector ScopeBoxes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_VolumeOfInterest);
            ElementId sc1 = null, sc2 = null, sc3 = null, sc4 = null;
                foreach (Element sb in ScopeBoxes)
                {
                    if (sb.Name == "BN02-A")
                    {
                        sc1 = sb.Id;
                    }
                    else if (sb.Name == "BN02-B")
                    {
                        sc2 = sb.Id;
                    }
                    else if (sb.Name == "BN02-C")
                    {
                        sc3 = sb.Id;
                    }
                    else if (sb.Name == "BN02-D")
                    {
                        sc4 = sb.Id;
                    }
                }
            if(sc1 == null || sc2 == null || sc3 == null || sc4 == null) { td("Please Rename Scope Boxes First !!."); return Result.Failed; }
            IList < ElementId> SCS = new List<ElementId> {sc1,sc2,sc3,sc4}; 
            IList<Element> titlblocks = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_TitleBlocks)).ToList();
            List<string> titlblocksnms = new List<string>();
            foreach (Element e in titlblocks)
            {
                FamilySymbol fs = e as FamilySymbol;
                titlblocksnms.Add(fs.Family.Name + " : " + fs.Name);
            }
            f5.titleblocks = titlblocksnms;
            f5.parents = Dupviewsnms;
            f5.templates = templatviewsnms;
            f5.viewports = viewportsnms;
            f5.ShowDialog();
            List<string> alphas = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "O", "R" };
            if (f5.DialogResult == DialogResult.Cancel) { return Result.Cancelled; }
            ViewPlan parent = Dupviews[f5.perntid] as ViewPlan;
            using (Transaction tr = new Transaction(doc, "Overall Divisor"))
            {
                tr.Start();
                // ================= Parent 1:100  =================
                ViewSheet vsao = ViewSheet.Create(doc, titlblocks[f5.tbid].Id);
                vsao.Name = parent.Name;
                LUP(vsao, "Sheet Number", f5.ss.ToString());
                LUP(vsao, "Sheet Name", parent.Name);
                LUP(vsao, "Revision", f5.RN);
                LUP(vsao, "Disc", f5.syscod.ToString());
                LUP(vsao, "Floor", f5.lvlcod.ToString());
                LUP(vsao, "Sheet Issue Date", f5.D.ToString());
                LUP(vsao, "Minor Groups", "SHOPDRAWINGS");
                Viewport vppao = null;
                try
                {
                    vppao = Viewport.Create(doc, vsao.Id, parent.Id, new XYZ(1.5, 1.3, 0)); double xz2 = 0.5;
                    foreach (int ii in f5.inds)
                    {
                        Viewport vpao = Viewport.Create(doc, vsao.Id, viewports[ii].Id, new XYZ(xz2, 0.2, 0));
                        if (vpao != null && et != null) { vpao.ChangeTypeId(et.Id); }
                        xz2 += 0.5;
                    }
                    if (f5.templ) { parent.ViewTemplateId = templatviews[f5.templtid].Id; }
                }
                catch (ArgumentException) { td("Master Plan is already in sheet."+"\n"+"Process will continue without adding Master Plan in Sheet"); }

                // ================= parts 1:50  =================
                ElementId vid = parent.Duplicate(ViewDuplicateOption.WithDetailing);
                View AO = (View)doc.GetElement(vid); AO.Scale = f5.sc;
                AO.Name = parent.Name+" AO";
                for (int i = 1; i <= f5.np; i++)
                {
                    ElementId vidi = AO.Duplicate(ViewDuplicateOption.AsDependent);
                    View p = (View)doc.GetElement(vidi);
                    p.Name = parent.Name +" - "+ alphas[i - 1];
                    try
                    {
                        p.LookupParameter("Scope Box").Set(SCS[i - 1]);
                    }
                    catch { }
                    if (f5.templ) { p.ViewTemplateId = templatviews[f5.templtid].Id; }
                    if (f5.sheets)
                    {
                        ViewSheet vs = ViewSheet.Create(doc, titlblocks[f5.tbid].Id);
                        vs.Name = p.Name; double xz = 0.5;
                        LUP(vs, "Sheet Number", f5.ss.ToString() + alphas[i - 1]);
                        LUP(vs, "Sheet Name", p.Name);
                        LUP(vs, "Revision", f5.RN);
                        LUP(vs, "Disc", f5.syscod.ToString());
                        LUP(vs, "Floor", f5.lvlcod.ToString());
                        LUP(vs, "Sheet Issue Date", f5.D.ToString());
                        LUP(vs, "Minor Groups", "SHOPDRAWINGS");
                        vs.SheetNumber= f5.ss.ToString() + alphas[i - 1];
                        Viewport vpp = Viewport.Create(doc, vs.Id, p.Id, new XYZ(1.5, 1.3, 0));
                        foreach (int ii in f5.inds)
                        {
                            Viewport vp = Viewport.Create(doc, vs.Id, viewports[ii].Id, new XYZ(xz, 0.2, 0));
                            if(vp!= null && et!=null) { vp.ChangeTypeId(et.Id); }
                            xz += 0.5;
                        }
                    }
                }
                tr.Commit();
            }

            return Result.Succeeded;
        }
    }
}