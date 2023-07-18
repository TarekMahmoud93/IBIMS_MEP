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
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Electrical;
using System.Windows.Forms;
using System.Security.Cryptography;
using View = Autodesk.Revit.DB.View;
using Document = Autodesk.Revit.DB.Document;
using Form = System.Windows.Forms.Form;

namespace IBIMS_MEP
{
    public class filterselectionlinkpipes : ISelectionFilter
    {
        public Document docu;
        public bool AllowElement(Element elem)
        {
            docu = ((RevitLinkInstance)elem).GetLinkDocument();
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            Element ee = docu.GetElement(reference.LinkedElementId);
            if ( ee is Pipe || ee is Duct || ee is Conduit || ee is CableTray || ee is Floor || ee is Wall || ee.Category.Name== "Structural Framing" || ee.Category.Name== "Structural Columns" || ee is FamilyInstance)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }



    [Transaction(TransactionMode.Manual)]
    public class Penetration : IExternalCommand
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

            FilteredElementCollector allsymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
            List<string> DRs = new List<string> { "01- SABMECH_8043_MP_PIPE_CDP", "MSAR_MHT_PL_PIPE_Drainage","MSAR_MHT_PL_PIPE_Sanitary", "MSAR_MHT_PL_PIPE_Vent"
            , "SABMECH_8043_FA_Sanitary" , "SABMECH_8043_FA_Vent" , "SABMECH_8043_FP_PIPE_Drainage"
            , "SABMECH_8043_MP_PIPE_CDP", "SABMECH_8043_PD_PIPE_ Pressurized Drainage", "SABMECH_8043_PD_PIPE_Floor Drainage"
            , "SABMECH_8043_PD_PIPE_Greasy Vent" , "SABMECH_8043_PD_PIPE_Greasy Waste" , "SABMECH_8043_PD_PIPE_Kitchen Grease Water L/L"
            , "SABMECH_8043_PD_PIPE_Pressurized" , "SABMECH_8043_PD_PIPE_Primary Storm Water", "SABMECH_8043_PD_PIPE_Rain Water",
            "SABMECH_8043_PD_PIPE_Sanitary" , "SABMECH_8043_PD_PIPE_Sanitary Vent", "SABMECH_8043_PD_PIPE_Secondary Storm Water",
            "SABMECH_8043_PD_PIPE_Soil" , "SABMECH_8043_PD_PIPE_Soil Pressurized", "SABMECH_8043_PD_PIPE_Waste",
            "SABMECH_8043_PL_PIPE_Drainage" , "SABMECH_8043_PL_PIPE_Sanitary", "SABMECH_8043_PL_PIPE_Soil Water",
            "SABMECH_8043_PL_PIPE_Storm Water" , "SABMECH_8043_PL_PIPE_Vent", "SABMECH_8043_PL_PIPE_Waste Water"};

            List<string> WSs = new List<string> { "Domestic Cool Cold Water", "Domestic Hot Water", "MSAR_MHT_PL_PIPE_DCW"
            , "MSAR_MHT_PL_PIPE_DHW" , "MSAR_MHT_PL_PIPE_DHWR" , "MSAR_MHT_PL_PIPE_Domestic Cool Cold Water"
            , "MSAR_MHT_PL_PIPE_Domestic Flush Water", "MSAR_MHT_PL_PIPE_Domestic Hot Water", "MSAR_MHT_PL_PIPE_Domestic Hot Water Return"
            , "MSAR_MHT_PL_PIPE_Domestic Raw Water", "MSAR_MHT_PL_PIPE_Irrigation", "MSAR_MHT_PL_PIPE_Water Truck-Fill"
            , "MSAR_MHT_PL_PIPE_Domestic Soft Water", "MSAR_MHT_PL_PIPE_Raw Water Treatment", "SABMECH_8043_FA_Domestic Cold Water"
            , "MSAR_MHT_PL_PIPE_Domestic Tempered Hot Water", "MSAR_MHT_PL_PIPE_Soft Cold Water", "SABMECH_8043_FA_Domestic Hot Water"
            , "SABMECH_8043_PD_PIPE_Domestic Cold Water", "SABMECH_8043_PL_PIPE_Domestic Cold Water", "SABMECH_8043_PL_PIPE_Domestic Flush Water"
            , "SABMECH_8043_PD_PIPE_Domestic Hot Water", "SABMECH_8043_PL_PIPE_Domestic Cold Water1", "SABMECH_8043_PL_PIPE_Domestic Hot Water"
            , "SABMECH_8043_PD_PIPE_Domestic Hot Water Return", "SABMECH_8043_PL_PIPE_Domestic Cool Cold Water", "SABMECH_8043_PL_PIPE_Domestic Hot Water Return",
            "SABMECH_8043_PL_PIPE_Irrigation"};

            List<string> CHWs = new List<string> { "02- SABMECH_8043_MP_PIPE_CHWR", "03- SABMECH_8043_MP_PIPE_CHWS" ,
            "MSAR_MHT_PL_PIPE_Chilled Water Return", "SABMECH_8043_FA_Hydronic Supply" ,"SABMECH_8043_MP_PIPE_Hydronic Return", "SABMECH_8043_PD_PIPE_Chilled Water Return",
            "MSAR_MHT_PL_PIPE_Chilled Water Supply", "SABMECH_8043_MP_PIPE_CHWR" ,"SABMECH_8043_MP_PIPE_REF", "SABMECH_8043_PD_PIPE_Chilled Water Supply",
            "SABMECH_8043_FA_Hydronic Return", "SABMECH_8043_MP_PIPE_CHWS" ,"SABMECH_8043_MP_PIPE_REF", "SABMECH_8043_PL_PIPE_Chilled Water Return","SABMECH_8043_PL_PIPE_Chilled Water Supply"};


            List<string> FFs = new List<string> { "MSAR_MHT_FP_PIPE_Fire Protection", "MSAR_MHT_PL_PIPE_Fire Protection Dry", "MSAR_MHT_PL_PIPE_Fire Protection Other",
            "MSAR_MHT_PL_PIPE_Fire Protection Pre-Action","SABMECH_8043_FA_Fire Protection Dry","SABMECH_8043_FA_Fire Protection Other",
            "SABMECH_8043_FA_Fire Protection Pre-Action","SABMECH_8043_FA_Fire Protection Wet","SABMECH_8043_Fire Protection",
            "SABMECH_8043_FP_FIRE_Fire Protection Dry","SABMECH_8043_FP_FIRE_Fire Protection Pre-Action","SABMECH_8043_FP_FIRE_Fire Protection Wet",
            "SABMECH_8043_FP_FIRE_Fire Protection Other","SABMECH_8043_FP_PIPE_Fire Protection","SABMECH_8043_PD_PIPE_Fire Protection",
            "SABMECH_8043_PD_PIPE_Fire Protection Dry","SABMECH_8043_PD_PIPE_Fire Protection Other","SABMECH_8043_PD_PIPE_Fire Protection Pre-Action",
            "SABMECH_8043_PL_PIPE_Fire  Protection","SABMECH_8043_PL_PIPE_Fire Protection Dry","SABMECH_8043_PL_PIPE_Fire Protection Other",
            "SABMECH_8043_PL_PIPE_Fire Protection Pre-Action"};

            List<List<string>> PSTs = new List<List<string>> { DRs, WSs, CHWs, FFs };
            FamilySymbol dr = null, ws = null, chw = null, ff = null, con = null; ; FamilySymbol fssd = null, fssct = null , fssDR = null;

            FilteredWorksetCollector fwc = new FilteredWorksetCollector(doc);
            List<string> worksetS = new List<string>();
            List<Workset> worksets = new List<Workset>();
            List<WorksetId> worksetIDs = new List<WorksetId>();
            foreach (Workset wsT in fwc) { if(wsT.Name == "02- DRAINAGE BW") { worksetIDs.Add(wsT.Id); break; } } // 0
            foreach (Workset wsT in fwc) { if(wsT.Name == "03- WATER BW") { worksetIDs.Add(wsT.Id);  break; } } // 1
            foreach (Workset wsT in fwc) { if(wsT.Name == "05- HVAC PIPES BW") { worksetIDs.Add(wsT.Id); break; } } // 2
            foreach (Workset wsT in fwc) { if(wsT.Name == "04- FIRE BW") { worksetIDs.Add(wsT.Id); break; } } // 3
            foreach (Workset wsT in fwc) { if(wsT.Name == "01- DUCT BW") { worksetIDs.Add(wsT.Id); break; } } // 4
            foreach (Workset wsT in fwc) { if (wsT.Name == "07- LTPW BW") { worksetIDs.Add(wsT.Id); break; } } // 5

            foreach (FamilySymbol fs in allsymbols)
            {
                if (fs.Family.Name == "MSAR_MHT_ST_CIRC_OPENING")
                {
                    if (fs.Name == "DR")
                    {
                        dr = fs;
                    }
                    else if (fs.Name == "WS")
                    {
                        ws = fs;
                    }
                    else if (fs.Name == "CHW")
                    {
                        chw = fs;
                    }
                    else if (fs.Name == "FP")
                    {
                        ff = fs;
                    }
                    else if (fs.Name == "COND")
                    {
                        con = fs;
                    }
                }
                else if (fs.Family.Name == "MSAR_MHT_ST_REC_OPENING")
                {
                    if (fs.Name == "Duct")
                    {
                        fssd = fs;
                    }
                    else if (fs.Name == "CT")
                    {
                        fssct = fs;
                    }
                    else if (fs.Name == "DR")
                    {
                        fssDR = fs;
                    }
                }
            }
            List<FamilySymbol> FSS = new List<FamilySymbol>() { dr, ws, chw, ff ,con};
            foreach (FamilySymbol fy in FSS)
            {
                if (fy == null || fssd == null)
                {
                    TaskDialog.Show("Load Families", "Please Load Families First");
                    return Result.Failed;
                }
            }

            IList<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Select(x => ((Level)x)).ToList();
            List<Level> ordlevels = levels.OrderBy(x=>x.Elevation).ToList();
            Level ElevationSleeve(XYZ Z)
            {
                int ilvl = 0; Level lv = null;
                foreach (Level lvl in ordlevels)
                {
                    if (lvl.Elevation - Z.Z>=0)
                    {
                        if (ilvl == 0)
                        {
                            lv = ordlevels[0];
                            break;
                        }
                        else
                        {
                            lv = ordlevels[ilvl - 1];
                            break;
                        }
                    }
                    ilvl++;
                }
                if(lv == null) { lv = ordlevels.Last(); }
                return lv;
            }

            IList<Element> AllLinks = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).ToList();
            PenetrationFM f9 = new PenetrationFM();
            IList<Element> AllLinksss = new List<Element>();
            IList<Document> Alldocss = new List<Document>();
            if (AllLinks.Count == 0) { td("No Links Loaded!!"); return Result.Failed; }
            foreach (Element el in AllLinks)
            {
                string aq = doc.GetElement(el.GetTypeId()).Name;
                if (!f9.linksnames.Contains(aq)) { f9.linksnames.Add(aq); AllLinksss.Add(el); Alldocss.Add(((RevitLinkInstance)el).GetLinkDocument()); }
            }
            f9.ShowDialog();
            bool sel = f9.sel;
            if (f9.DialogResult == DialogResult.Cancel) { return Result.Cancelled; }
            IList<Element> selLinks = new List<Element>();
            List<string> seldocsnames = new List<string>();
            foreach (int i in f9.linksinds)
            {
                selLinks.Add(AllLinksss[i]);
                seldocsnames.Add(((RevitLinkInstance)AllLinksss[i]).GetLinkDocument().PathName.Trim());
            }

            IList<Element> AllSTR = new List<Element>();
            IList<RevitLinkInstance> AllRLI = new List<RevitLinkInstance>();
            List<Element> floors = new List<Element>();
            if (!sel)
            {
                foreach (Element sl in selLinks)
                {
                    Document doca = ((RevitLinkInstance)sl).GetLinkDocument();
                    if (doca != null)
                    {
                        floors = new FilteredElementCollector(doca).OfClass(typeof(Floor)).ToList();
                        IList<Element> aLLWalls = new FilteredElementCollector(doca).OfClass(typeof(Wall)).Where(x => ((Wall)x).Width >= (50 / 304.8)).ToList();
                        IList<Element> ALLBEAMS = new FilteredElementCollector(doca).OfClass(typeof(FamilyInstance)).Where(x => x.Category.Name == "Structural Framing").ToList();
                        foreach (Element el in aLLWalls)
                        {
                            AllSTR.Add(el); AllRLI.Add((RevitLinkInstance)sl);
                        }
                        foreach (Element el in ALLBEAMS)
                        {
                            AllSTR.Add(el); AllRLI.Add((RevitLinkInstance)sl);
                        }
                        foreach (Element el in floors)
                        {
                            AllSTR.Add(el); AllRLI.Add((RevitLinkInstance)sl);
                        }
                    }
                }
            }

            List<string> floordrainfamilies = new List<string>() { "01- Area Drain", "01- FLOOR DRAIN", "1- FFD" , "02- SHOWER DRAIN", "Intercepting_Drain-Watts-FD-440_Series", "KA_FD",
            "SABPLMG_1160_PD_PFIX_Area Drain 1","SABPLMG_1160_PD_PFIX_Area Drain 2","SABPLMG_1160_PD_PFIX_Funnel Floor Drain Type2","SABPLMG_1160_PD_PFIX_Parking Drain","SABPLMG_1160_PD_PFIX_Shower Floor Drain"};

            // ======================  Pipes Ducts Selection =========================== 
            
            IList<Element> AllEles = new List<Element>();
            IList<Element> floordrains =new List<Element>();
            List<string> linkchars = new List<string>();    
            if (sel)
            {
                IList<Reference> reffss = uidoc.Selection.PickObjects(ObjectType.LinkedElement, new filterselectionlinkpipes(), "Select Elements in RVT Link");
                foreach (Reference rf1 in reffss)
                {
                    Document doca = ((RevitLinkInstance)doc.GetElement(rf1.ElementId)).GetLinkDocument();
                    if (!seldocsnames.Contains(doca.PathName.Trim())) { continue; }
                    Element fl = doca.GetElement(rf1.LinkedElementId);
                    string lnkname = doc.GetElement(((RevitLinkInstance)doc.GetElement(rf1.ElementId)).GetTypeId()).Name;
                    if (fl is Pipe || fl is Duct || fl is CableTray || fl is Conduit)
                    {
                        AllEles.Add(fl); linkchars.Add(lnkname.Split('_')[6]);

                    }
                    else if (lnkname.Split('_').Count() >= 7 && fl is FamilyInstance)
                    {
                        if (lnkname.Split('_')[6] == "DRGW" && floordrainfamilies.Contains(((FamilyInstance)fl).Symbol.Family.Name))
                        {
                            floordrains.Add(fl); 
                        }
                    }
                    else if (fl is Floor || fl is Wall)
                    {
                        AllSTR.Add(fl); AllRLI.Add((RevitLinkInstance)doc.GetElement(rf1.ElementId));
                    }
                }
            }
            else
            {
                foreach (Element li in selLinks)
                {
                    RevitLinkInstance rli = li as RevitLinkInstance;
                    Autodesk.Revit.DB.Document doca = rli.GetLinkDocument();
                    if (doca != null)
                    {
                        IList<Element> ducts = new FilteredElementCollector(doca).OfClass(typeof(Duct)).ToList();
                        IList<Element> cables = new FilteredElementCollector(doca).OfClass(typeof(CableTray)).ToList();
                        IList<Element> conduits = new FilteredElementCollector(doca).OfClass(typeof(Conduit)).ToList();
                        IList<Element> pipes = new FilteredElementCollector(doca).OfClass(typeof(Pipe)).ToList();
                        foreach (Element e in ducts) { AllEles.Add(e); linkchars.Add(doc.GetElement(rli.GetTypeId()).Name.Split('_')[6]); }
                        foreach (Element e in cables) { AllEles.Add(e); linkchars.Add(doc.GetElement(rli.GetTypeId()).Name.Split('_')[6]); }
                        foreach (Element e in pipes) { AllEles.Add(e); linkchars.Add(doc.GetElement(rli.GetTypeId()).Name.Split('_')[6]); }
                        foreach (Element e in conduits) { AllEles.Add(e); linkchars.Add(doc.GetElement(rli.GetTypeId()).Name.Split('_')[6]); }
                    }
                }

            }
            List<FamilyInstance> fisFL = new List<FamilyInstance>();
            List<FamilyInstance> fisWL = new List<FamilyInstance>();
            List<WorksetId> finwork = new List<WorksetId>();
            List<string> ss = new List<string>();
            List<double> dias = new List<double>() { 10, 15, 20, 25, 32, 40, 50, 65, 80, 90, 100, 125, 150, 200 };
            List<double> diasDR = new List<double>() { 15, 20, 25, 32, 40, 50, 75, 110, 160, 200, 250, 315, 355 };
            IList<Curve> curves = new List<Curve>(); List<WorksetId> worksetid = new List<WorksetId>();
            List<double> ALLDIAS = new List<double>();
            List<Element> AllELemnts = new List<Element>();
            List<List<double>> ALLBHs = new List<List<double>>();
            IList<FamilySymbol> FamilySymbolss = new List<FamilySymbol>();
            List<string> familysymbolnamesuniq= new List<string>(); int ae = 0;
            foreach (Element e in AllEles)
            {
                LocationCurve lc = ((LocationCurve)e.Location); Curve c = null;
                if (lc != null) { c = lc.Curve; }
                else { ae++;  continue; }
                double B = 0, H = 0, I = 0;
                double dia = 0; int id = 0; int st = 0; FamilySymbol fss = null; // dr ws chw ff con 

                if (e is Pipe || e is Conduit)
                {
                    double diaa = 0; 
                    if (e is Pipe)
                    {
                        string name = linkchars[ae];
                        if(name == "DRGW")       { fss = dr;  worksetid.Add(worksetIDs[0]); }
                        else if (name == "WATR") { fss = ws;  worksetid.Add(worksetIDs[1]); }
                        else if (name == "PIPE") { fss = chw; worksetid.Add(worksetIDs[2]); }
                        else if (name == "FIRE") { fss = ff;  worksetid.Add(worksetIDs[3]); }   
                        else                     { fss = dr;  worksetid.Add(worksetIDs[0]); } /// Not Defined
                        diaa = e.LookupParameter("Outside Diameter").AsDouble() + 2 * (e.LookupParameter("Insulation Thickness").AsDouble());
                    }
                    else
                    {
                        diaa = e.LookupParameter("Outside Diameter").AsDouble(); fss = FSS[4]; st = 4;
                        worksetid.Add(worksetIDs[5]);
                    }
                    List<double> DIAS = new List<double>();
                    if (fss == dr) { DIAS = diasDR; }
                    else { DIAS = dias; }
                    foreach (double d in diasDR) // loop in steel pipes 
                    {
                        if (Math.Round(diaa * 304.8) <= d)
                        {
                            if (Math.Round(diaa * 304.8) == d)
                            {
                                if (id != diasDR.Count - 1)
                                {
                                    dia = diasDR[id + 1] / 304.8;
                                }
                                else
                                {
                                    dia = d / 304.8;
                                }
                            }
                            else
                            {
                                dia = d / 304.8;
                            }
                            break;
                        }
                        id++;
                    }
                    curves.Add(c); ALLDIAS.Add(dia); FamilySymbolss.Add(fss); ALLBHs.Add(new List<double>() { 0, 0, 0 }); AllELemnts.Add(e);          
                }
                else
                {
                    try
                    {
                        B = e.LookupParameter("Width").AsDouble();
                        H = e.LookupParameter("Height").AsDouble();
                    }
                    catch { ae++; continue; }
                    try
                    {
                        I = e.LookupParameter("Insulation Thickness").AsDouble();
                    }
                    catch { }
                    if(e is CableTray) { fss = fssct; worksetid.Add(worksetIDs[5]); } else { fss = fssd; worksetid.Add(worksetIDs[4]);}
                    curves.Add(c); ALLDIAS.Add(0); FamilySymbolss.Add(fss); ALLBHs.Add(new List<double>() { B, H, I }); AllELemnts.Add(e);
                }
                if(fss == null) { fss = FSS[0]; }
                ae++;
            }

            foreach (Element fd in floordrains)
            {
                XYZ p = ((LocationPoint)fd.Location).Point;
                Curve c = Line.CreateBound(p.Add(XYZ.BasisZ * 4), p.Add(-XYZ.BasisZ * 4));
                curves.Add(c); ALLDIAS.Add(0); FamilySymbolss.Add(fssDR); ALLBHs.Add(new List<double>() { 200/304.8, 200 / 304.8, 0 }); AllELemnts.Add(fd); worksetid.Add(worksetIDs[0]);
            }

            double iss = 1;
            List<int> counts = new List<int>();
            foreach(string sa in familysymbolnamesuniq) { counts.Add(1); }
            //IList<Element> allslevees = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_GenericModel)).Where(x => x.Name == "VOIDS_SLEEVES").ToList();
            //double iss =-1;
            //foreach (Element c in allslevees)
            //{
            //    double i = -500;
            //    try
            //    {
            //        i=Convert.ToDouble(c.LookupParameter("Sleeve No").AsString());
            //    }
            //    catch { }
            //    if (i != -500)
            //    {
            //        if (i > iss)
            //        {
            //            iss = i;    
            //        }
            //    }
            //}

            //if (iss == -1) { iss = 1; }   // No Sleeves Founded
            //else { iss++; }

            Fpb Fpb = new Fpb();
            Fpb.pbr.Minimum = 0; Fpb.pbr.Maximum = AllSTR.Count; Fpb.pbr.Step = 1;
            Fpb.Lb.Text = "0 / " + AllSTR.Count;
            Fpb.Show(); int iallstr = 0;

            List<XYZ> locs = new List<XYZ>();
            TransactionGroup tg = new TransactionGroup(doc);
            tg.SetName("Penetration");
            tg.Start();
            using (Transaction tr = new Transaction(doc, "Pentration"))
            {
                tr.Start();
                foreach (FamilySymbol fy in FSS)
                {
                    fy.Activate();
                }
                fssd.Activate();
                fssct.Activate();

                int iwc = Convert.ToInt32(iss);
                foreach (Element w in AllSTR)
                {
                    Options optt = app.NewGeometryOptions();
                    Solid so = null;
                    optt.ComputeReferences = true;
                    GeometryElement gele = w.get_Geometry(optt);
                    foreach (GeometryObject geo in gele)
                    {
                        Solid g = geo as Solid;
                        if (g != null && g.Volume != 0)
                        {
                            so = g;
                            break;
                        }
                    }
                    if (so == null)
                    {
                        iallstr++;
                        Fpb.Lb.Text = iallstr + " / " + AllSTR.Count;
                        Fpb.pbr.PerformStep(); Fpb.pbr.Refresh();
                        Fpb.Refresh();
                        continue;
                    }
                    int ic = 0; XYZ SlDir = null;
                    foreach (Curve c in curves)
                    {
                        Face face = null; double dep = 0; XYZ loc = null; XYZ dir = null;
                        SolidCurveIntersectionOptions scio = new SolidCurveIntersectionOptions();
                        scio.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;
                        SolidCurveIntersection sci = null;
                        try
                        {
                            sci = so.IntersectWithCurve(c, scio);
                        }
                        catch { }
                        if (sci != null)
                        {
                            if (sci.SegmentCount > 0)
                            {
                                Curve cc = sci.GetCurveSegment(0);
                                if (w is Wall) { dep = ((Wall)w).Width; }
                                else if (w is Floor) { dep = w.LookupParameter("Thickness").AsDouble(); }
                                else { dep = cc.Length; }
                                if (cc is Line)
                                {
                                    if (Math.Round(Math.Abs(((Line)cc).Direction.Normalize().Z), 1) == 1) //Floor
                                    {
                                        SlDir = XYZ.BasisX;
                                    }
                                    else //Wall
                                    {
                                        SlDir = XYZ.BasisZ;
                                    }
                                }
                                XYZ pis = sci.GetCurveSegment(0).GetEndPoint(0); XYZ pie = sci.GetCurveSegment(0).GetEndPoint(1);
                                XYZ pcs = c.GetEndPoint(0); XYZ pce = c.GetEndPoint(1);
                                if (pis.X != pcs.X && pis.Y != pcs.Y && pis.X != pce.X && pis.Y != pce.Y)
                                {
                                    loc = pis; dir = Line.CreateBound(pis, pie).Direction.Normalize();
                                }
                                else { loc = pie; dir = Line.CreateBound(pie, pis).Direction.Normalize(); }
                                foreach (Face f in so.Faces)
                                {
                                    IntersectionResultArray ira = new IntersectionResultArray();
                                    SetComparisonResult scr = f.Intersect(c, out ira);
                                    if (ira != null)
                                    {
                                        if (!ira.IsEmpty)
                                        {
                                            face = f; break;
                                        }
                                    }
                                }
                            }
                        }

                        if (face != null && FamilySymbolss[ic] != null)
                        {
                            if (face.Reference != null)
                            {
                                Reference fcref = face.Reference.CreateLinkReference(AllRLI[iallstr]); 
                                try
                                {
                                    bool found = false;
                                    foreach (XYZ lo in locs)
                                    {
                                        if (Math.Round(lo.X, 2) == Math.Round(loc.X, 2) && Math.Round(lo.Y, 2) == Math.Round(loc.Y, 2) & Math.Round(lo.Z, 2) == Math.Round(loc.Z, 2))
                                        {
                                            found = true; break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        FamilyInstance fi = doc.Create.NewFamilyInstance(fcref, loc, SlDir, FamilySymbolss[ic]);
                                        if (AllELemnts[ic] is Pipe || AllELemnts[ic] is Conduit)
                                        {
                                            fi.LookupParameter("Diameter").Set(ALLDIAS[ic]);
                                            fi.LookupParameter("Depth").Set(dep + (50 / 304.8));
                                        }
                                        else
                                        {
                                            fi.LookupParameter("b").Set(ALLBHs[ic][0] + 2 * (ALLBHs[ic][2] + (50 / 304.8)));
                                            fi.LookupParameter("h").Set(ALLBHs[ic][1] + 2 * (ALLBHs[ic][2] + (50 / 304.8)));
                                            fi.LookupParameter("Depth").Set(dep + (50 / 304.8));
                                            fi.Location.Rotate(((Line)c), Math.PI / 2);
                                        }
                                        if (fi != null)
                                        {
                                            fisWL.Add(fi); locs.Add(loc); iwc++;
                                            //int idfs = familysymbolnamesuniq.IndexOf(FamilySymbolss[ic].Name);
                                            //int icc = counts[idfs];
                                            //sys = FamilySymbolss[ic].Name;
                                            //string mark = sys + shp + "-" + iwc;
                                            //fi.LookupParameter("Mark").Set(iwc.ToString());
                                            fi.LookupParameter("Schedule Level").Set(ElevationSleeve(loc).Id);
                                            fi.LookupParameter("Comments").Set(ElevationSleeve(loc).Name.ToString());
                                            finwork.Add(worksetid[ic]);
                                            fi.LookupParameter("Workset").Set(worksetid[ic].IntegerValue);
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        ic++;
                    }
                    iallstr++;
                    Fpb.Lb.Text = iallstr + " / " + AllSTR.Count;
                    Fpb.pbr.PerformStep(); Fpb.pbr.Refresh();
                    Fpb.Refresh();
                }
                tr.Commit();
            }
            Fpb.Close();
            td("Finished !!" + "\n" + "All voids count is:  " + fisWL.Count);

            tg.Assimilate();

            //using (Transaction qq = new Transaction(doc, "aa"))
            //{
            //    qq.Start(); int qo = 0;
            //    foreach (Element fi in fisWL) { try { fi.LookupParameter("Workset").Set(finwork[qo].IntegerValue); } catch { } qo++; }
            //    qq.Commit();
            //}
            return Result.Succeeded;
        }
    }
}
