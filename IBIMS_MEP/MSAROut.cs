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
using Autodesk.Revit.DB.Events;
using System.IO;
using System.IO.Compression;

namespace IBIMS_MEP
{
    [Transaction(TransactionMode.Manual)]

    public class MSAROut : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet element)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;
            Autodesk.Revit.Creation.Application app = commandData.Application.Application.Create;

            FilteredElementCollector fecex = new FilteredElementCollector(doc).OfClass(typeof(ExportDWGSettings));
            ExportDWGSettings eds = null;
            foreach (ExportDWGSettings ex in fecex)
            {
                if (ex.Name == "CAD EXPORT MHT")
                {
                    eds = ex; break;
                }
            }
            if(eds == null) { TaskDialog.Show("Error", "Export sitting of dwg first"); return Result.Cancelled; }
            DWGExportOptions op = eds.GetDWGExportOptions();
            op.MergedViews = true;
            FilteredElementCollector fec = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet));
            List<string> sheetsar = new List<string>(); List<string> sheets = new List<string>();
            IList<ElementId> ids = new List<ElementId>(); IList<ViewSheet> vsheets = new List<ViewSheet>();
            foreach (ViewSheet vs in fec)
            {
                if (vs != null)
                {
                    string s = vs.SheetNumber + " - " + vs.Name;
                    sheetsar.Add(s); ids.Add(vs.Id); sheets.Add(s); vsheets.Add(vs);
                }
            }
            if (ids.Count == 0)
            {
                TaskDialog.Show("No Sheets", "There are no sheets to Export/Print");
                return Result.Failed;
            }
            sheetsar.Sort();
            IList<ElementId> idsar = new List<ElementId>();
            IList<ViewSheet> vsheetsar = new List<ViewSheet>();
            foreach (string s in sheetsar)
            {
                int o = 0;
                foreach (string ss in sheets)
                {
                    if (s == ss)
                    {
                        idsar.Add(ids[o]);
                        vsheetsar.Add(vsheets[o]);
                        break;
                    }
                    o++;
                }
            }
            Pdfrnm form = new Pdfrnm();
            form.sheets = sheetsar;
            form.ShowDialog();
            List<int> inds = form.inds;
            inds.Sort();
            PrintManager pm = doc.PrintManager;
            PaperSize ps = null;
            foreach (PaperSize p in pm.PaperSizes)
            {
                if (p.Name == "A0")
                {
                    ps = p;
                }
            }
            //=================================================================
            using (Transaction trans = new Transaction(doc, "IBIMS Sheets Outing"))
            {
                trans.Start();
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last();
                string basefolder = "C:\\Users\\" + userName + "\\Desktop\\Out\\PDF";
                string cadfol = "C:\\Users\\" + userName + "\\Desktop\\Out\\CAD";
                foreach (var file in new DirectoryInfo(basefolder).GetFiles())
                {
                    file.Delete();
                }
                foreach (var file in new DirectoryInfo(cadfol).GetFiles())
                {
                    file.Delete();
                }
                pm.PrintRange = PrintRange.Select;
                ViewSheetSetting vss = pm.ViewSheetSetting;
                try
                {
                    pm.SelectNewPrintDriver("PDF24");
                }
                catch { pm.SelectNewPrintDriver("Adobe PDF"); }
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.HiddenLineViews = HiddenLineViewsType.VectorProcessing;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.RasterQuality = RasterQualityType.Presentation;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.PaperSize = ps;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.PageOrientation = PageOrientationType.Landscape;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.HideScopeBoxes = true;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.HideUnreferencedViewTags = true;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.HideCropBoundaries = true;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.HideReforWorkPlanes = true;
                pm.PrintSetup.CurrentPrintSetting.PrintParameters.ColorDepth = ColorDepthType.BlackLine;
                pm.PrintToFile = true; 
                pm.CombinedFile = true;
                doc.PrintManager.PrintSetup.CurrentPrintSetting = pm.PrintSetup.CurrentPrintSetting;

                List<string> pdfnames = new List<string>();
                List<string> names = new List<string>();
                List<List<string>> exeldata = new List<List<string>>();
                ViewSheet vsrandom = null;
                foreach (int i in inds)
                {
                    List<string> temp = new List<string>();
                    IList<ElementId> vsids = new List<ElementId>();
                    vsids.Add(idsar[i]);
                    ViewSheet vs = vsheetsar[i]; vsrandom = vs;

                    string s1 = vs.LookupParameter("Floor").AsString();
                    string s3 = vs.LookupParameter("Disc").AsString();
                    string s4 = vs.LookupParameter("Sheet Number").AsString();
                    string s5 = vs.LookupParameter("Revision").AsString();
                    string name = "MSAR-SAB-PA-IC-ZN1-BN02-" + s1 + "-SPD-" + s3 + "-" + s4 + "-" + s5;
                    string pdfname = ""; names.Add(name);
                    ViewSet taViewSet = new ViewSet();
                    taViewSet.Insert((View)doc.GetElement(idsar[i]));
                    pdfname = basefolder + "\\" + name + ".pdf"; pdfnames.Add(pdfname);
                    pm.PrintToFileName = pdfname;
                    pm.Apply();
                    doc.Print(taViewSet, true);
                    doc.Export(cadfol, name, vsids, op);
                }
                trans.Commit();
            }
            return Result.Succeeded;
        }

    }
}


