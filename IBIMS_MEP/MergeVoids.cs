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


namespace IBIMS_MEP
{
    public class filterselpent : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            if (e.Category != null)
            {
                if (((FamilyInstance)e).Symbol.Family.Name == "MSAR_MHT_ST_CIRC_OPENING" || ((FamilyInstance)e).Symbol.Family.Name == "MSAR_MHT_ST_REC_OPENING")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class MergeVoids : IExternalCommand
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

            filterselpent fil = new filterselpent();
            FilteredElementCollector allsymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
            FamilySymbol fssd = null;
            foreach (FamilySymbol fs in allsymbols)
            {
                if (fs.Family.Name == "MSAR_MHT_ST_REC_OPENING" && fs.Name == "JOINED")
                {
                    fssd = fs; break;
                }
            }
            if(fssd == null) { td("Please Load Rectangle Pentration Family First."); return Result.Failed; }
            FilteredElementCollector alllinks = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance));
            RevitLinkInstance GetRLI(string name)
            {
                RevitLinkInstance rlii = null;
                foreach (RevitLinkInstance r in alllinks)
                {
                    string nn = doc.GetElement(r.GetTypeId()).Name;
                    if (nn == name)
                    {
                        rlii = r; break;
                    }
                }
                return rlii;
            }
            double d = 120 / 304.8;
            IList<Reference> refs = uidoc.Selection.PickObjects(ObjectType.Element, fil);
            List<List<Element>> _slevs = new List<List<Element>>();
            List<string> Reffnames = new List<string>(); List<string> Linksnames = new List<string>();
            List<RevitLinkInstance> rlis = new List<RevitLinkInstance>();
            List<List<List<Curve>>> _circls = new List<List<List<Curve>>>();
            List<List<XYZ>> _centers = new List<List<XYZ>>();int wrong = 0;
            foreach (Reference re in refs)
            {
                Element e = doc.GetElement(re);
                XYZ FO = ((FamilyInstance)e).FacingOrientation;
                XYZ HO = ((FamilyInstance)e).HandOrientation;
                Parameter p = e.LookupParameter("Reff.Name");
                if (p == null) { wrong++; continue; }    
                string refname = e.LookupParameter("Reff.Name").AsString();
                string linkname = e.LookupParameter("Link Name").AsString();
                XYZ lo = ((LocationPoint)e.Location).Point;
                string cs = Case(e); Curve c = null; List<Curve> curves = new List<Curve>();
                XYZ loc = null; XYZ vec = null; XYZ vec2 = null; double bd = 0; double hd = 0;
                if (((FamilyInstance)e).Symbol.Family.Name == "MSAR_MHT_ST_CIRC_OPENING")
                {
                    double r = e.LookupParameter("Diameter").AsDouble();
                    bd = hd = r;
                }
                else
                {
                    bd = e.LookupParameter("b").AsDouble();
                    hd = e.LookupParameter("h").AsDouble();
                }

                if (Math.Round(Math.Abs(FO.Z), 1) == 1 || Math.Round(Math.Abs(HO.Z), 1) == 1) // wall 
                {
                    if (cs == "X")
                    {
                        loc = new XYZ(lo.X, 0, lo.Z);
                        vec = XYZ.BasisX; vec2 = XYZ.BasisZ;

                    }
                    else
                    {
                        loc = new XYZ(0, lo.Y, lo.Z);
                        vec = XYZ.BasisY; vec2 = XYZ.BasisZ;

                    }
                }
                else
                {
                    loc = new XYZ(lo.X, lo.Y, 0);
                    vec = XYZ.BasisX; vec2 = XYZ.BasisY;
                }

                XYZ p1 = loc.Add(vec * (bd + d) / 2).Add(vec2 * (hd + d) / 2);
                XYZ p2 = p1.Add(-vec2 * (hd + d));
                XYZ p3 = p2.Add(-vec * (bd + d));
                XYZ p4 = p3.Add(vec2 * (hd + d));
                Curve c1 = Line.CreateBound(p1, p2);
                Curve c2 = Line.CreateBound(p2, p3);
                Curve c3 = Line.CreateBound(p3, p4);
                Curve c4 = Line.CreateBound(p4, p1);
                curves = new List<Curve>() { c1, c2, c3, c4 };

                if (Reffnames.Count == 0)
                {
                    List<Element> tempslev = new List<Element>();
                    List<XYZ> tempcentr = new List<XYZ>();
                    List<List<Curve>> tempcurve = new List<List<Curve>>(); Linksnames.Add(linkname);
                    tempslev.Add(e); tempcurve.Add(curves); tempcentr.Add(lo); rlis.Add(GetRLI(linkname));
                    _slevs.Add(tempslev); _circls.Add(tempcurve); _centers.Add(tempcentr); Reffnames.Add(refname);
                }
                else
                {
                    bool add = false; int indd = 0;
                    foreach (string rn in Reffnames)
                    {
                        indd = Reffnames.IndexOf(rn);
                        if (refname.Trim() == Reffnames[indd].Trim() && linkname.Trim() == Linksnames[indd].Trim())
                        {
                            add = true; break;
                        }
                    }
                    if (add)
                    {
                        _slevs[indd].Add(e); _circls[indd].Add(curves); _centers[indd].Add(lo);
                    }
                    else
                    {
                        List<Element> tempslev = new List<Element>();
                        List<XYZ> tempcentr = new List<XYZ>();
                        List<List<Curve>> tempcurve = new List<List<Curve>>(); rlis.Add(GetRLI(linkname));
                        tempslev.Add(e); tempcurve.Add(curves); tempcentr.Add(lo); Reffnames.Add(refname);
                        _slevs.Add(tempslev); _circls.Add(tempcurve); _centers.Add(tempcentr); Linksnames.Add(linkname);
                    }
                }
            }

            List<List<ElementId>> Alldeletedslvs = new List<List<ElementId>>();
            using (Transaction tr = new Transaction(doc, "IBIMS Merge Voids"))
            {
                tr.Start();
                fssd.Activate();
                int ind = 0;
                foreach (List<List<Curve>> cirs in _circls)  // foreach Face Reffe
                {
                    bool INTERS = false;
                    List<ElementId> deletedslvs = new List<ElementId>();
                    string cs = Case(_slevs[ind][0]);
                    List<double> Zs = new List<double>(); List<double> XYs = new List<double>();
                    double dep = _slevs[ind][0].LookupParameter("Depth").AsDouble();
                    XYZ FO = ((FamilyInstance)_slevs[ind][0]).FacingOrientation;
                    XYZ HO = ((FamilyInstance)_slevs[ind][0]).HandOrientation;
                    foreach (List<Curve> c in cirs)   // foreach sleve
                    {
                        int i = cirs.IndexOf(c); 
                        XYZ loc = _centers[ind][i];
                        Element slv1 = _slevs[ind][i]; 
                        if (deletedslvs.Contains(_slevs[ind][i].Id)) { continue; }
                        foreach (List<Curve> cc in cirs)   // foreach sleve
                        {
                            int ii = cirs.IndexOf(cc);
                            XYZ loca = _centers[ind][ii];
                            Element slv2 = _slevs[ind][ii];
                            if (deletedslvs.Contains(_slevs[ind][ii].Id)) { continue; }
                            if (loca.X == loc.X && loca.Y == loc.Y && loca.Z == loc.Z)
                            {
                                continue;
                            }
                            foreach (Curve cu in c)  // foreach Line in curves
                            {
                                foreach (Curve cuu in cc)  // foreach Line in curves
                                {
                                    IntersectionResultArray iraa = new IntersectionResultArray();
                                    SetComparisonResult scrr = cu.Intersect(cuu, out iraa);
                                    if (iraa != null)
                                    {
                                        if (!iraa.IsEmpty)
                                        {
                                            double pav = 0; XYZ newloc = null;
                                            BoundingBoxXYZ bbi = slv1.get_BoundingBox(null);
                                            BoundingBoxXYZ bbii = slv2.get_BoundingBox(null); 
                                            XYZ vecc1 = null; XYZ vecc2 = null;
                                            if (Math.Round(Math.Abs(FO.Z), 1) == 1 || Math.Round(Math.Abs(HO.Z), 1) == 1) // wall 
                                            {
                                                vecc1 = XYZ.BasisZ;
                                                Zs.Add(bbi.Max.Z); Zs.Add(bbi.Min.Z); Zs.Add(bbii.Max.Z); Zs.Add(bbii.Min.Z);
                                                if (cs == "X")
                                                {
                                                    XYs.Add(bbi.Max.X); XYs.Add(bbi.Min.X); XYs.Add(bbii.Max.X); XYs.Add(bbii.Min.X);
                                                    newloc = new XYZ(0.5 * (XYs.Max() + XYs.Min()), pav, 0.5 * (Zs.Max() + Zs.Min())); vecc2 = XYZ.BasisX;
                                                }
                                                else
                                                {
                                                    XYs.Add(bbi.Max.Y); XYs.Add(bbi.Min.Y); XYs.Add(bbii.Max.Y); XYs.Add(bbii.Min.Y);
                                                    newloc = new XYZ(pav, 0.5 * (XYs.Max() + XYs.Min()), 0.5 * (Zs.Max() + Zs.Min())); vecc2 = XYZ.BasisY;
                                                }
                                            }
                                            else
                                            {
                                                Zs.Add(bbi.Max.Y); Zs.Add(bbi.Min.Y); Zs.Add(bbii.Max.Y); Zs.Add(bbii.Min.Y);
                                                XYs.Add(bbi.Max.X); XYs.Add(bbi.Min.X); XYs.Add(bbii.Max.X); XYs.Add(bbii.Min.X); vecc1 = XYZ.BasisY;
                                                newloc = new XYZ(0.5 * (XYs.Max() + XYs.Min()), 0.5 * (Zs.Max() + Zs.Min()), pav); vecc2 = XYZ.BasisX;
                                            }

                                            IList<Curve> curves = Fourcurves(Zs, XYs, newloc, vecc1, vecc2);
                                            deletedslvs.Add(slv1.Id); deletedslvs.Add(slv2.Id); INTERS = true;
                                            interss(curves);
                                            void interss(IList<Curve> fourcurves)
                                            {
                                                bool inters = false;
                                                foreach (Curve ccc in fourcurves)
                                                {
                                                    foreach (List<Curve> crs in _circls[ind])
                                                    {
                                                        foreach (Curve cuuu in crs)
                                                        {
                                                            int iii = _circls[ind].IndexOf(crs);
                                                            XYZ locc = _centers[ind][iii];
                                                            if (deletedslvs.Contains(_slevs[ind][iii].Id)) { continue; }
                                                            IntersectionResultArray ira = new IntersectionResultArray();
                                                            SetComparisonResult scr = cuuu.Intersect(ccc, out ira);
                                                            if (ira != null)
                                                            {
                                                                if (!ira.IsEmpty)
                                                                {
                                                                    BoundingBoxXYZ bbiii = _slevs[ind][iii].get_BoundingBox(null);
                                                                    if (Math.Round(Math.Abs(FO.Z), 1) == 1 || Math.Round(Math.Abs(HO.Z), 1) == 1) // wall 
                                                                    {
                                                                        vecc1 = XYZ.BasisZ;
                                                                        Zs.Add(bbiii.Max.Z); Zs.Add(bbiii.Min.Z);
                                                                        if (cs == "X")
                                                                        {
                                                                            XYs.Add(bbiii.Max.X); XYs.Add(bbiii.Min.X);
                                                                            newloc = new XYZ(0.5 * (XYs.Max() + XYs.Min()), pav, 0.5 * (Zs.Max() + Zs.Min())); vecc2 = XYZ.BasisX;
                                                                        }
                                                                        else
                                                                        {
                                                                            XYs.Add(bbiii.Max.Y); XYs.Add(bbiii.Min.Y);
                                                                            newloc = new XYZ(pav, 0.5 * (XYs.Max() + XYs.Min()), 0.5 * (Zs.Max() + Zs.Min())); vecc2 = XYZ.BasisY;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Zs.Add(bbiii.Max.Y); Zs.Add(bbiii.Min.Y);
                                                                        XYs.Add(bbiii.Max.X); XYs.Add(bbiii.Min.X); vecc2 = XYZ.BasisX;
                                                                        newloc = new XYZ(0.5 * (XYs.Max() + XYs.Min()), 0.5 * (Zs.Max() + Zs.Min()), pav); vecc1 = XYZ.BasisY;
                                                                    }
                                                                    inters = true; deletedslvs.Add(_slevs[ind][iii].Id);
                                                                }
                                                            }
                                                            if (inters) { break; }
                                                        }
                                                    }
                                                }
                                                IList<Curve> curves2 = Fourcurves(Zs, XYs, newloc, vecc1, vecc2);
                                                if (inters)
                                                {
                                                    interss(curves2);
                                                }
                                                else
                                                {
                                                    string rffname = rlis[ind].UniqueId + Reffnames[ind];
                                                    Reference ret = Reference.ParseFromStableRepresentation(doc, rffname);
                                                    FamilyInstance fi = doc.Create.NewFamilyInstance(ret, newloc, vecc1, fssd);
                                                    double B = curves2[1].Length - d; double H = curves2[0].Length - d;
                                                    fi.LookupParameter("b").Set(H); fi.LookupParameter("h").Set(B);
                                                    fi.LookupParameter("Depth").Set(dep);
                                                    string sq = fi.HostFace.ConvertToStableRepresentation(doc);
                                                    string[] sqs = sq.Split(':'); string reffnm = ":"; int io = 0;
                                                    foreach (string aq in sqs)
                                                    {
                                                        if (io != 0)
                                                        {
                                                            reffnm += aq;
                                                            if (io != sqs.Length - 1)
                                                            {
                                                                reffnm += ":";
                                                            }
                                                        }
                                                        io++;
                                                    }
                                                    fi.LookupParameter("Link Name").Set(Linksnames[ind]);
                                                    fi.LookupParameter("Reff.Name").Set(reffnm);
                                                    Zs = new List<double>(); XYs = new List<double>();
                                                }
                                            }
                                        }
                                    }
                                }
                                if (INTERS) { break; }
                            }
                            if (INTERS) { break; }
                        }
                    }
                    Alldeletedslvs.Add(deletedslvs);
                    ind++;
                }
                tr.Commit();
            }
            int merged = 0;
            foreach (List<ElementId> list in Alldeletedslvs)
            {
                merged+=list.Count;
            }

            td("Finished !!" + "\n"+ merged + "  Voids are merged" + "\n" + wrong + "  Voids are not the new Family.");

            IList<Curve> Fourcurves(List<double> Zs, List<double> XYs, XYZ newloc, XYZ vec1, XYZ vec2)
            {
                double b = XYs.Max() - XYs.Min(); double h = Zs.Max() - Zs.Min();
                XYZ p1 = newloc.Add(vec2 * (b + d) / 2).Add(vec1 * (h + d) / 2);
                XYZ p2 = p1.Add(-vec1 * (h + d));
                XYZ p3 = p2.Add(-vec2 * (b + d));
                XYZ p4 = p3.Add(vec1 * (h + d));
                Curve c1 = Line.CreateBound(p1, p2);
                Curve c2 = Line.CreateBound(p2, p3);
                Curve c3 = Line.CreateBound(p3, p4);
                Curve c4 = Line.CreateBound(p4, p1);
                IList<Curve> curves = new List<Curve>() { c1, c2, c3, c4 };
                return curves;
            }
            string Case(Element e)
            {
                string ca = "";
                XYZ fo = ((FamilyInstance)e).FacingOrientation;
                XYZ ho = ((FamilyInstance)e).HandOrientation;
                if (Math.Abs(Math.Round(ho.X, 2)) == 1 || Math.Abs(Math.Round(fo.X, 2)) == 1)
                {
                    ca = "X";
                }
                else
                {
                    ca = "Y";
                }
                return ca;
            }
            return Result.Succeeded;
        }
    }
}


