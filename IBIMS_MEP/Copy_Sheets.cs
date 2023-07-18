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
using Form = System.Windows.Forms.Form;
using ComboBox = System.Windows.Forms.ComboBox;
using View = Autodesk.Revit.DB.View;
using Autodesk.Revit.DB.Visual;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace IBIMS_MEP
{
    public class dublicate : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
           return DuplicateTypeAction.UseDestinationTypes;
        }
    }
    [Transaction(TransactionMode.Manual)]

    public class Copy_Sheets : IExternalCommand
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
            sheetcopyform sf = new sheetcopyform();
            sf.label5.Text = doc.Title;
            foreach (Document dd in commandData.Application.Application.Documents)
            {
                if (dd.Title.Trim() != doc.Title.Trim() && !dd.IsLinked) { sf.docs.Add(dd); sf.comboBox1.Items.Add(dd.Title); }

            }
            if(sf.docs.Count == 0) { td("No other Documents opened!");return Result.Succeeded; }
            IList<Element> TBdoc = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
            .WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_TitleBlocks)).ToList();
            IList<Element> allsheets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).ToList();
            List<string> titlenamesdoc = new List<string>();
            foreach (ViewSheet e in allsheets)
            { 
                sf.sheets.Items.Add(e.SheetNumber + " >> " + e.Name);
                if (e.GetAllPlacedViews().Count > 0)
                {
                    foreach (Element t in TBdoc)
                    {
                        if (doc.GetElement(t.OwnerViewId).Name == e.Name)
                        {
                            if (!titlenamesdoc.Contains(((FamilyInstance)t).Symbol.FamilyName))
                            {
                                titlenamesdoc.Add(((FamilyInstance)t).Symbol.FamilyName);
                            }
                            break;
                        }
                    }
                }
            }
            
            string s = "";
            foreach(string ss in titlenamesdoc) { s += ss+"\n"; }
            sf.ShowDialog();
            if (sf.DialogResult == DialogResult.Cancel) { return Result.Cancelled; }
            Document doca = sf.docs[sf.comboBox1.SelectedIndex];
            IList<Element> TBdoca = new FilteredElementCollector(doca).OfClass(typeof(FamilySymbol))
            .WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_TitleBlocks)).ToList();
            foreach (string ft in titlenamesdoc)
            {
                bool found = false;
                foreach (Element t in TBdoca)
                {
                    if (((FamilySymbol)t).FamilyName == ft)
                    {
                        found=true; break;
                    }
                }
                if (!found) { td("Please Load in Target document ALL Used TitleBlocks in Source Document."+ "\n"+ft);return Result.Succeeded; }
            }
            if (sf.sheets.CheckedItems.Count == 0) { td("There is no Sheets selected to Copy!.");return Result.Succeeded; }
            IList<ViewSheet> selsheets = new List<ViewSheet>(); 
            foreach (int i in sf.sheets.CheckedIndices) 
            {
                selsheets.Add((ViewSheet)allsheets[i]); 
            }
            using (Transaction tr = new Transaction(doca, "Copy Sheets"))
            {
                tr.Start();
                foreach (ViewSheet vsh in selsheets)
                {
                    ElementId tbid = null; string name = ""; string famname = ""; ViewSheet newvsh = null;
                    foreach (Element t in TBdoc)
                    {
                        if (doc.GetElement(t.OwnerViewId).Name == vsh.Name)
                        {
                            name = t.Name; famname = ((FamilyInstance)t).Symbol.FamilyName; break;
                        }
                    }
                    foreach (Element t in TBdoca)
                    {
                        if (t.Name == name && ((FamilySymbol)t).FamilyName == famname) { tbid = t.Id; break; }
                    }
                    try {  newvsh = ViewSheet.Create(doca, tbid); } catch { }
                    if(newvsh == null) {  continue; }
                    foreach (int i in sf.elements.CheckedIndices)
                    {
                        if (i == 0) // parameters
                        {
                            try { newvsh.SheetNumber = vsh.SheetNumber; } catch { }
                            try { newvsh.Name = vsh.Name; } catch { }
                            try { newvsh.LookupParameter("Approved By").Set(vsh.LookupParameter("Approved By").AsString()); } catch { }
                            try { newvsh.LookupParameter("Checked By").Set(vsh.LookupParameter("Checked By").AsString()); } catch { }
                            try { newvsh.LookupParameter("Drawn By").Set(vsh.LookupParameter("Drawn By").AsString()); } catch { }
                            try { newvsh.LookupParameter("DATE").Set(vsh.LookupParameter("DATE").AsString()); } catch { }
                            try { newvsh.LookupParameter("SCALE").Set(vsh.LookupParameter("SCALE").AsString()); } catch { }
                            try { newvsh.LookupParameter("REV").Set(vsh.LookupParameter("REV").AsString()); } catch { }
                        }
                        else if (i == 1) // Lines
                        {
                            IList<Element> lines = new FilteredElementCollector(doc,vsh.Id).OfClass(typeof(CurveElement)).ToList();
                            Copyelems(lines);
                        }
                        else if (i == 2) // Text Notes
                        {
                            IList<Element> txts = new FilteredElementCollector(doc, vsh.Id).OfClass(typeof(TextNote)).ToList();
                            Copyelems(txts);

                        }
                        else if (i == 3) // Schaduals
                        {
                            IList<Element> schs = new FilteredElementCollector(doc, vsh.Id).OfClass(typeof(ScheduleSheetInstance)).ToList();
                            Copyelems(schs);
                        }
                        else if (i == 4) // Legends
                        {
                            IList<Element> legs = new FilteredElementCollector(doc, vsh.Id).OfClass(typeof(Viewport)).Where(x => ((View)doc.GetElement(((Viewport)x).ViewId)).ViewType == ViewType.Legend).ToList();
                            
                            Copyelems(legs);
                        }
                    }
                    void Copyelems(IList<Element> elems)
                    {
                        CopyPasteOptions co = new CopyPasteOptions();
                        co.SetDuplicateTypeNamesHandler(new dublicate());
                        if (elems.Count > 0)
                        {
                            if (elems.Count > 0) { ElementTransformUtils.CopyElements(vsh, elems.Select(x => x.Id).ToList(), newvsh, Transform.Identity, co); }
                        }
                    }
                }

                tr.Commit();
            }

            return Result.Succeeded;
        }
    }
}