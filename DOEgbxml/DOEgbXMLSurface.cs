using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VectorMath;
using System.Xml;

namespace DOEgbXML
{
    public class SurfaceDefinitions
    {
        //creates instances of an object that store information about surfaces in a gbXML file
        public string SurfaceType;
        public string SurfaceId;
        public string ConstructionId;
        public List<string> AdjSpaceId;
        public double Azimuth;
        public string orientation;
        public double area = 0.0; //initialize to 0.0
        public double Tilt;
        public double Height;
        public double Width;
        public Vector.CartCoord InsertionPoint;
        public List<Vector.MemorySafe_CartCoord> PlCoords;
        public Vector.MemorySafe_CartVect PlRHRVector;
        //initialize the list
        public List<SubSurfaceDefinition> subSurfaceList = new List<SubSurfaceDefinition>();

        #region utility functions for RP-1810
        //find the orientation of the surface.
        //The orientation is defined according to ...
        public string surfaceOrientation()
        {
            if(orientation == null)
            {
                Vector.MemorySafe_CartVect normVect = Vector.convertToMemorySafeVector(DOEgbXMLBasics.getNorm(PlCoords));
                double calculatedAzimuth = DOEgbXMLBasics.FindAzimuth(normVect);
                orientation = DOEgbXMLBasics.getFaceDirection(calculatedAzimuth);
            }

            return orientation;

        }

        public double computeArea()
        {
            if (area > 0)
            {
                return area;
            }

            area = DOEgbXMLBasics.computeArea(PlCoords);
            if(area == -1)
            {
                //send warning: PlCoords has less than 3 coordinates
                return 0;
            }
            else
            {
                return area;

            }
        }

        public void addSubSurface(SubSurfaceDefinition subSurface)
        {
            subSurface.parentID = SurfaceId;
            subSurface.parentTilt = Tilt;
            subSurface.parentAzimuth = Azimuth;
            subSurface.parentHeight = Height;

            subSurfaceList.Add(subSurface);
        }

        #endregion

        public static DOEgbXMLPhase2Report SurfaceAdjSpaceIdTest(List<string> surfaceIds, List<SurfaceDefinitions> surfaces, DOEgbXMLPhase2Report report)
        {
            report.testSummary = "This test ensures that each surface adjacent space id is not assigned to any random or undeclared space id.";
            try
            {
                foreach (SurfaceDefinitions s in surfaces)
                {
                    List<string> ml = new List<string>();
                    ml.Add(s.SurfaceId + " testing begins to ensure adjacent space ids match Space ids.");
                    //test each surface
                    foreach (string ajid in s.AdjSpaceId)
                    {
                        if(surfaceIds.Contains(ajid))
                        {
                            //good
                            ml.Add("PASS: "+ajid+ " match has been found for :" + s.SurfaceId);
                            report.TestPassedDict[s.SurfaceId] = true;
                        }
                        else
                        {
                            //bad
                            ml.Add("FAIL: There is not Space id called: " + ajid);
                            report.TestPassedDict[s.SurfaceId] = false;
                        }

                    }
                    report.MessageList[s.SurfaceId] = ml;
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }

            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAILED: Surfaces have adjacent space ids that are not declared at the Space level.";
                report.passOrFail = false;
                
            }
            else
            {
                report.longMsg = "TEST PASSED: All Surfaces have adjacent space ids that match to a Space id already described.";
                report.passOrFail = true;
                
            }
            return report;
        }

        public static DOEgbXMLReportingObj SurfaceCCTest(List<SurfaceDefinitions> surfaces, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that each Surface has coordinate descriptions that wind in a counter clockwise order.";
            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            try
            {
                report.MessageList.Add("Starting counter-clockwise winding test for Surface elements");
                foreach (SurfaceDefinitions s in surfaces)
                {
                    report.MessageList.Add("Testing Surface id: " + s.SurfaceId);
                    var wd = Vector.isCounterClockwise(s.PlCoords);
                    if (wd == Vector.WalkDirection.Clockwise)
                    {
                        report.MessageList.Add("CLOCKWISE WINDING DETECTED: Surface id: " + s.SurfaceId + " is wound in a clockwise order.");
                        report.TestPassedDict.Add(s.SurfaceId, false);
                    }
                    else
                    {
                        report.MessageList.Add("PASS: Surface id: " + s.SurfaceId + " is wound in a counter-clockwise order.");
                        report.TestPassedDict.Add(s.SurfaceId, true);
                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            if(report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAILED: Surfaces have been detected with clockwise winding orders.";
                report.passOrFail = false;
                return report;
            }
            else
            {
                report.longMsg = "TEST PASSED: All Surfaces have counter-clockwise winding orders.";
                report.passOrFail = true;
                return report;
            }
        }

        public static DOEgbXMLPhase2Report SurfaceCCTest(Dictionary<string,List<SurfaceDefinitions>> enclosure, DOEgbXMLPhase2Report report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that each Surface has PolyLoop coordinate descriptions that wind in a counter clockwise order.";
            
            report.TestPassedDict = new Dictionary<string, bool>();
            try
            {
                
                foreach (KeyValuePair<string,List<SurfaceDefinitions>> kp in enclosure)
                {
                    List<string> ml = new List<string>();
                    

                    //this fakespacecount is a dummy because NamedSurfaceCCTest requires an integer.
                    int fakespacecount = 0;
                    Dictionary<string,List<Vector.MemorySafe_CartCoord>> sbcc = new Dictionary<string,List<Vector.MemorySafe_CartCoord>>();
                    foreach (SurfaceDefinitions s in kp.Value)
                    {
                        List<Vector.MemorySafe_CartCoord> pl = new List<Vector.MemorySafe_CartCoord>();
                        //different conditions if interior or exterior
                        if (s.AdjSpaceId.Count() == 1)
                        {
                            foreach (Vector.MemorySafe_CartCoord p in s.PlCoords)
                            {
                                Vector.MemorySafe_CartCoord msp = new Vector.MemorySafe_CartCoord(p.X, p.Y, p.Z);
                                pl.Add(msp);
                            }
                            sbcc[s.SurfaceId] = pl;
                            continue;
                        }
                        //implied that the count is equal to 2
                        else
                        {
                            for (int i = 0; i < s.AdjSpaceId.Count(); i++)
                            {
                                if (s.AdjSpaceId[i] == s.AdjSpaceId[i + 1])
                                {
                                    //slab on grade
                                    //treat normally
                                    foreach (Vector.MemorySafe_CartCoord p in s.PlCoords)
                                    {
                                        Vector.MemorySafe_CartCoord msp = new Vector.MemorySafe_CartCoord(p.X, p.Y, p.Z);
                                        pl.Add(msp);
                                    }
                                    sbcc[s.SurfaceId] = pl;
                                    break;
                                }
                                else
                                {
                                    //some other sort of typical interior surface.  outward normal points away from first, toward second.
                                    //if the adj id value of first is equal to space id, leave alone
                                    //otherwise reverse
                                    if (s.AdjSpaceId[i] == kp.Key)
                                    {
                                        foreach (Vector.MemorySafe_CartCoord p in s.PlCoords)
                                        {
                                            Vector.MemorySafe_CartCoord msp = new Vector.MemorySafe_CartCoord(p.X, p.Y, p.Z);
                                            pl.Add(msp);
                                        }
                                        sbcc[s.SurfaceId] = pl;
                                        break;
                                    }
                                    else
                                    {
                                        s.PlCoords.Reverse();
                                        foreach (Vector.MemorySafe_CartCoord p in s.PlCoords)
                                        {
                                            Vector.MemorySafe_CartCoord msp = new Vector.MemorySafe_CartCoord(p.X, p.Y, p.Z);
                                            pl.Add(msp);
                                        }
                                        sbcc[s.SurfaceId] = pl;
                                        s.PlCoords.Reverse();
                                        break;

                                    }
                                }
                            }
                        }
                    }
                    
                    Vector.MemorySafe_CartCoord sbcentroid = Vector.FindVolumetricCentroid(sbcc);
                    Dictionary<string, bool> surfres = Vector.NamedSurfacesCCWound(sbcentroid, sbcc,fakespacecount,false);

                    
                    foreach (KeyValuePair<string, bool> surfkp in surfres)
                    {
                        //since I have already checked to see if the names of the ids are unique, there is no reason to re-check something that has been checked.
                        //I could alternatively stry and store things at the Space level, but this doesn't really make sense, since this is specifically a Surface test
                        if(report.TestPassedDict.ContainsKey(surfkp.Key)) continue;
                        if (surfkp.Value == true)
                        {
                            ml.Add("PASS: Surface Boundary with id " + surfkp.Key + " is wound in a counterclockwise order.");
                        }
                        else
                        {
                            ml.Add("FAIL: Surface Boundary with id " + surfkp.Key + " is wound in a clockwise order.");
                        }
                    }
                    report.MessageList[kp.Key] = ml;

                }
        
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            //if we have made it this far, that is good.
            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAILED: Surfaces have been detected with clockwise winding orders.  We suggest reviewing if these errors have occurred on interior ";
                report.longMsg += " or exterior surfaces.  If interior surfaces, likely this will not be a problem unless the order of insulation materials for your interior";
                report.longMsg += " surface is important.  Exterior surfaces pointing with incorrect orientation is a problem.  In either case, we suggest you contact your BIM authoring tool ";
                report.longMsg += " to let them know there is an error.";
                report.passOrFail = false;
                return report;
            }
            else
            {
                report.longMsg = "TEST PASSED: All Surfaces have counter-clockwise winding orders.";
                report.passOrFail = true;
                return report;
            }
        }

        public static DOEgbXMLReportingObj TestSurfacePlanarTest(List<SurfaceDefinitions> TestSurfaces, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that each Surface has coordinate descriptions that form planar objects.";
            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            try
            {
                report.MessageList.Add("Starting non-planar test for Surface elements");
                foreach (SurfaceDefinitions ts in TestSurfaces)
                {
                    report.MessageList.Add("Testing Surface id: " + ts.SurfaceId);
                    Dictionary<string, List<Vector.CartVect>> surfaceXProducts = new Dictionary<string, List<Vector.CartVect>>();
                    List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                    for (int i = 0; i < ts.PlCoords.Count - 2; i++)
                    {
                        //Get the Cross Product
                        VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(ts.PlCoords[i], ts.PlCoords[i + 1]);
                        VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(ts.PlCoords[i + 1], ts.PlCoords[i + 2]);
                        Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                        xProd = Vector.UnitVector(xProd);
                        xProducts.Add(xProd);
                    }
                    surfaceXProducts.Add(ts.SurfaceId, xProducts);
                    for (int j = 0; j < xProducts.Count - 1; j++)
                    {
                        //parallel and anti parallel
                        if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                        {
                            report.MessageList.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                            report.TestPassedDict.Add(ts.SurfaceId, true);
                            continue;
                        }
                        //anti-parallel
                        else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                        {
                            report.MessageList.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                            report.TestPassedDict.Add(ts.SurfaceId, true);
                            continue;
                        }
                        else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < .0001 && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < .0001 &&
                            Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < 0.0001)
                        {
                            report.MessageList.Add("PASS: Coordinates for this surface form a nearly planar surface that is within allowable tolerances.");
                            report.TestPassedDict.Add(ts.SurfaceId, true);
                            continue;
                        }
                        else
                        {
                            report.MessageList.Add("NON PLANAR SURFACE DETECTED:  Coordinates for this surface do not appear to form a planar surface.");
                            report.TestPassedDict.Add(ts.SurfaceId, false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report TestSurfacePlanarTest(List<SurfaceDefinitions> TestSurfaces, DOEgbXMLPhase2Report report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that each Surface has coordinate descriptions that do not violate the principles of planarity.";
            report.TestPassedDict = new Dictionary<string, bool>();
            report.passOrFail = true;
            try
            {
                
                foreach (SurfaceDefinitions ts in TestSurfaces)
                {
                    List<string> ml = new List<string>();
                    ml.Add(ts.SurfaceId + " begin testing planarity of surface polygon definition.");
                    Dictionary<string, List<Vector.CartVect>> surfaceXProducts = new Dictionary<string, List<Vector.CartVect>>();
                    List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                    for (int i = 0; i < ts.PlCoords.Count - 2; i++)
                    {
                        //Get the Cross Product
                        VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(ts.PlCoords[i], ts.PlCoords[i + 1]);
                        VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(ts.PlCoords[i + 1], ts.PlCoords[i + 2]);
                        Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                        xProd = Vector.UnitVector(xProd);
                        xProducts.Add(xProd);
                    }
                    surfaceXProducts.Add(ts.SurfaceId, xProducts);
                    List<bool> xpbool = new List<bool>();
                    for (int j = 0; j < xProducts.Count - 1; j++)
                    {
                        //parallel and anti parallel
                        if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                        {
                            xpbool.Add(true);
                            continue;
                        }
                        //anti-parallel
                        else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                        {
                            xpbool.Add(true);
                            continue;
                        }
                        else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < report.vectorangletol && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < report.vectorangletol &&
                            Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < report.vectorangletol)
                        {
                            xpbool.Add(true);
                            continue;
                        }
                        else
                        {
                            
                            xpbool.Add(false);
                        }
                    }
                    if (xpbool.Contains(false))
                    {
                        report.TestPassedDict[ts.SurfaceId] = false;
                        ml.Add("FAIL:  Non-planar surface detected.  Coordinates for this surface do not appear to form a planar surface.");
                        report.passOrFail = false;
                    }
                    else
                    {
                        report.TestPassedDict[ts.SurfaceId] = true;
                        ml.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                    }
                    report.MessageList[ts.SurfaceId] = ml;
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }

            //if we have made it this far presumably we can test to see if everything is in fact planar
            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "FAIL:  Non planar surfaces have been detected.  Please contact your CAD vendor to correct.";
                report.passOrFail = false;
            }
            else
            {
                report.longMsg = "PASS:  All surfaces are planar or nearly planar within allowable tolerances.";
                report.passOrFail = true;
            }
            return report;
        }

        public static DOEgbXMLReportingObj SurfaceIDUniquenessTest(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //report pass or fail attribute should always be set to false
            report.MessageList = new List<string>();
            List<string> surfaceIdList = new List<string>();
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    //looks to see if the surfaceId is already included in the list of space IDs being generated
                    string surfaceId="";
                    XmlAttributeCollection xmlatts = node.Attributes;
                    foreach (XmlAttribute att in xmlatts)
                    {
                        if (att.Name == "id")
                        {
                            surfaceId = att.Value;
                        }
                    }
                    if (surfaceIdList.Contains(surfaceId))
                    {
                        //not unique
                        report.MessageList.Add("Surface ID: " + surfaceId + " is not unique.");
                        report.longMsg = "This test for unique surface id attributes has failed.  All Surface Id attributes should be unique.";
                        report.passOrFail = false;
                        return report;
                    }
                    surfaceIdList.Add(surfaceId);
                    report.MessageList.Add("Surface ID: " + surfaceId + " is unique.");
                }
                report.longMsg = "All surfaces have unique id attributes.";
                report.passOrFail = true;
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report SurfaceIDUniquenessTest(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //report pass or fail attribute should always be set to false
            
            List<string> surfaceIdList = new List<string>();
            try
            {
                report.testSummary = "This test ensures that all Surface elements in the gbXML file are unique.  This is a requirement of all gbXML files.  ";
                report.testSummary += "It might not appear important at first glance, but having unique Surface id elements ensures that information about Surfaces in the file ";
                report.testSummary += "can be located without any confusion.  Naming two different surfaces with the same id would make certain analyses impossible to properly conduct.";
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    List<string> ml = new List<string>();
                    //looks to see if the surfaceId is already included in the list of space IDs being generated
                    string surfaceId = "";
                    XmlAttributeCollection xmlatts = node.Attributes;
                    foreach (XmlAttribute att in xmlatts)
                    {
                        if (att.Name == "id")
                        {
                            surfaceId = att.Value;
                        }
                    }
                    if (surfaceIdList.Contains(surfaceId))
                    {
                        //not unique
                        ml.Add("FAIL: The id " + surfaceId + " is not a unique id.");
                    }
                    //unique
                    else
                    {
                        surfaceIdList.Add(surfaceId);
                        ml.Add("PASS: The id " + surfaceId + " is a unique id.");
                        report.MessageList[surfaceId] = ml;
                    }
                }
                if (report.TestPassedDict.ContainsValue(false))
                {
                    report.passOrFail = false;
                    report.longMsg = "TEST FAILED: This test has failed.  All Surface elements in this gbXML file do not have id attributes that are unique.";

                }
                else
                {
                    report.longMsg = "TEST PASSED: All Surface elements in this gbXML file have unique id attributes.";
                    report.passOrFail = true;
                }

            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        

        public static DOEgbXMLReportingObj AtLeast4Surfaces(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //pass or fail attribute should always be set to false
            report.testSummary = "This test ensures that the minimum number of surfaces have been declared in the gbXML file.";

            report.MessageList = new List<string>();
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                int count = nodes.Count;
                if (count >= 4)
                {
                    report.MessageList.Add("There are " + count.ToString() + " surfaces in this gbXML file.");
                    report.longMsg = "This gbXML file has the minimum required number of surfaces (4, i.e - a tetrahedra).";
                    report.passOrFail = true;
                }
                else
                {
                    report.MessageList.Add("There are " + count.ToString() + " surfaces in this gbXML file.");
                    report.longMsg = "This gbXML file DOES NOT have the minimum required number of surfaces (4, i.e - a tetrahedra).";
                    report.passOrFail = false;
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report AtLeast4Surfaces(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //pass or fail attribute should always be set to false
            report.testSummary = "This test ensures that the minimum number of surfaces have been declared in the gbXML file.";
            try
            {
                
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                int count = nodes.Count;
                if (count >= 4)
                {
                    
                    report.longMsg = "TEST PASSED: This gbXML file has the minimum required number of surfaces (4, i.e - a tetrahedra).";
                    report.passOrFail = true;
                }
                else
                {
                    
                    report.longMsg = "TEST FAILED: This gbXML file DOES NOT have the minimum required number of surfaces (4, i.e - a tetrahedra).";
                    report.passOrFail = false;
                }

            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj AtMost2SpaceAdjId(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //pass or fail attribute should always be set to false
            report.testSummary = "A simple test to ensure that each Surface element has at least 1 but no more than 2 allowed AdjacentSpaceIds.";

            try
            {
                List<string> ml = new List<string>();
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                int count = 0;
                report.passOrFail = true;
                foreach (XmlNode surface in nodes)
                {
                    foreach (XmlNode chld in surface.ChildNodes)
                    {
                        if (chld.Name == "AdjacentSpaceId")
                        {
                            count += 1;
                        }
                    }
                    //the adjacent space id is required, but must be less than 2
                    if (count > 0 && count <= 2)
                    {
                        ml.Add("There are " + count.ToString() + " AdjacentSpaceId nodes in this gbXML file.");
                        report.longMsg = "This gbXML file is within the required allowance of AdjacentSpaceId nodes (2, i.e - to describe each enclosed neighbor).";
                    }
                    else
                    {
                        ml.Add("There are " + count.ToString() + " AdjacentSpaceId nodes in this gbXML file.");
                        report.longMsg = "This gbXML file is NOT within the required allowance of AdjacentSpaceId nodes (2, i.e - to describe each enclosed neighbor).";
                        report.passOrFail = false;
                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report AtMost2SpaceAdjId(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //pass or fail attribute should always be set to false
            report.passOrFail = true;
            report.testSummary = "A simple test to ensure that each Surface element has at least 1 but no more than 2 allowed AdjacentSpaceIds.";
            try
            {
                
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                string surfacetype = "";
                foreach (XmlNode surface in nodes)
                {
                    int count = 0;
                    List<string> ml = new List<string>();
                    string surfaceId="";
                    XmlAttributeCollection xmlatts = surface.Attributes;
                    foreach (XmlAttribute att in xmlatts)
                    {
                        if (att.Name == "id")
                        {
                            surfaceId += att.Value;
                        }

                        if(att.Name == "surfaceType")
                        {
                            surfacetype = att.Value;
                            break;
                        }
                    }
                    foreach (XmlNode chld in surface.ChildNodes)
                    {
                        if (chld.Name == "AdjacentSpaceId")
                        {
                            count += 1;
                        }
                    }
                    //the adjacent space id is required, but must be less than 2
                    if (count > 0 && count <= 2)
                    {
                        ml.Add("PASS:  There are " + count.ToString() + " AdjacentSpaceId nodes for this Surface.");
                        report.MessageList[surfaceId] = ml;
                    }
                    else
                    {
                        if (surfacetype == "Shade")
                        {
                            continue;
                        }
                        else
                        {
                            ml.Add("FAIL:  There are " + count.ToString() + " AdjacentSpaceId nodes for this Surface.");
                            report.MessageList[surfaceId] = ml;
                            report.passOrFail = false;
                        }
                    }
                }
                if (report.passOrFail)
                {
                    report.longMsg = "TEST PASSED:  This gbXML file is has Surface elements all within the required maximum allotment of AdjacentSpaceId nodes (2, i.e - to describe each enclosed neighbor).";
                }
                else
                {
                    report.longMsg = "TEST FAILED:  This gbXML file has instances where the allowed number of AdjacentSpaceId nodes (2, i.e - to describe each enclosed neighbor) has been exceeded.  Contact your BIM/CAD authoring tool to let them know to correct the issue.";
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj RequiredSurfaceFields(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //pass or fail attribute should always be set to false
            //Feb 6 2014 - gbXML XSD version 5.11
            //this test ensures that each surface element has the required attributes and elements, and that all enumerations are properly declared
            report.testSummary = "This test ensures that all required fields are present in every Surface.";
            report.MessageList = new List<string>();
            try
            {
                report.passOrFail = true;
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    //surfaceID for reporting
                    string surfaceId = "";
                    XmlAttributeCollection xmlatts = node.Attributes;
                    foreach (XmlAttribute att in xmlatts)
                    {
                        if (att.Name == "id")
                        {
                            surfaceId = att.Value;
                            report.MessageList.Add("Surface: " + surfaceId + " has the required id attribute.");
                        }
                        else if (att.Name == "surfaceType")
                        {
                            report.MessageList.Add("Surface: " + surfaceId + " has the required surfaceType attribute.");
                            string val = att.Value;
                            if (val == "InteriorWall") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "ExteriorWall") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "Roof") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "InteriorFloor") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "Shade") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "UndergroundWall") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "UndergroundSlab") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "Ceiling") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "Air") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "UndergroundCeiling") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "RaisedFloor") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "SlabOnGrade") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "FreeStandingColumn") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else if (val == "EmbeddedColumn") { report.MessageList.Add("Valid surfaceType enum:" + val); continue; }
                            else
                            {
                                report.MessageList.Add("INVALID surfaceType enumeration:" + val);
                                report.longMsg="An invalid surfaceType enumeration has been declared.  Please review the latest XSD for valid enumerations.";
                                report.passOrFail = false;
                            }
                        }
                    }
                    XmlNodeList childnodes = node.ChildNodes;
                    foreach (XmlNode child in childnodes)
                    {
                        Dictionary<string, bool> surfReq = new Dictionary<string, bool>();
                        surfReq.Add("RectangularGeometry", false);
                        surfReq.Add("PlanarGeometry", false);

                        //find RectangularGeometry node (reqiured)
                        if (child.Name == "RectangularGeometry")
                        {
                            report.MessageList.Add("SUCCESS: The required RectangularGeometry Element located.");
                            
                            //is any of this really needed, doesn't the XSD validation already do this?
                            XmlNodeList rgchilds = child.ChildNodes;
                            Dictionary<string, bool> rgReq = new Dictionary<string, bool>();
                            rgReq.Add("Azimuth", false);
                            rgReq.Add("CartesianPoint", false);
                            rgReq.Add("Tilt", false);
                            rgReq.Add("Height", false);
                            rgReq.Add("Width", false);
                            //rgReq.Add("PolyLoop", false);

                            foreach(XmlNode rgchild in rgchilds)
                            {
                                string elName = rgchild.Name;
                                if (elName == "Azimuth")
                                {
                                    report.MessageList.Add("SUCCESS: Required Azimuth Element located.");
                                    rgReq["Azimuth"] = true;
                                    continue;
                                }
                                else if (elName == "CartesianPoint")
                                {
                                    report.MessageList.Add("SUCCESS: Required Cartesian Element located.");
                                    rgReq["CartesianPoint"] = true;
                                    continue;
                                }
                                else if (elName == "Tilt")
                                {
                                    report.MessageList.Add("SUCCESS: Required Tilt Element located.");
                                    rgReq["Tilt"] = true;
                                    continue;
                                }
                                else if (elName == "Height")
                                {
                                    report.MessageList.Add("SUCCESS: Required Height Element located.");
                                    rgReq["Height"] = true;
                                    continue;
                                }
                                else if (elName == "Width")
                                {
                                    report.MessageList.Add("SUCCESS: Required Width Element located.");
                                    rgReq["Width"] = true;
                                    continue;
                                }
                                else if (elName == "PolyLoop")
                                {
                                    //this is optional, not sure if we want to report anything here
                                }

                            }

                            if (rgReq.ContainsValue(false))
                            {
                                //failure
                                report.longMsg = "FAIL:  Not all required Rectangular Geometry elements are present.";
                                report.passOrFail = false;
                                return report;
                            }
                            else
                            {
                                report.MessageList.Add("PASS:  Rectangular Geometry element has all required elements.");
                                surfReq["RectangularGeometry"] = true;
                            }
                            continue;
                        }
                        //find PlanarGeometry node (required)
                        else if (child.Name == "PlanarGeometry")
                        {
                            report.MessageList.Add("The required PlanarGeometry Element has been successfully located.");

                            //check the quality of the planar Geometry needed?  Don't I already do this elsewhere?
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;

        }

        public static DOEgbXMLPhase2Report RequiredSurfaceFields(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //pass or fail attribute should always be set to false
            //Feb 6 2014 - gbXML XSD version 5.11
            //this test ensures that each surface element has the required attributes and elements, and that all enumerations are properly declared
            report.testSummary = "This test ensures that all required fields are present in every Surface element of your gbXML file.";
            
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {

                    List<string> ml = new List<string>();
                    //surfaceID for reporting
                    string surfaceId = "";
                    if (node.Attributes.GetNamedItem("id") != null)
                    {
                        surfaceId = node.Attributes.GetNamedItem("id").Value;
                        ml.Add("PASS: " + surfaceId + " has the required id attribute.");
                    }
                    else
                    {
                        ml.Add("FAIL: " + surfaceId + " is missing the required id attribute.");
                    }

                    if(node.Attributes.GetNamedItem("surfaceType") != null)
                    {
                        ml.Add("PASS: " + surfaceId + " has the required surfaceType attribute.");
                        string val = node.Attributes.GetNamedItem("surfaceType").Value;
                        if (val == "InteriorWall") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "ExteriorWall") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "Roof") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "InteriorFloor") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "Shade") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "UndergroundWall") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "UndergroundSlab") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "Ceiling") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "Air") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "UndergroundCeiling") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "RaisedFloor") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "SlabOnGrade") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "FreeStandingColumn") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else if (val == "EmbeddedColumn") { ml.Add("PASS: Valid surfaceType enum:" + val); }
                        else
                        {
                            ml.Add("FAIL: Invalid surfaceType enumeration:" + val);
                            report.longMsg = "TEST FAILED: For Surface with" +surfaceId+ " an invalid surfaceType enumeration has been declared.  Please review the latest XSD for valid enumerations.";
                            report.passOrFail = false;
                        }
                    }
                    else
                    {
                        ml.Add("FAIL: " + surfaceId + " is missing the required surfaceType attribute.");
                    }

                    XmlNodeList childnodes = node.ChildNodes;
                    foreach (XmlNode child in childnodes)
                    {
                        if (child.Name == "Name") { continue; }
                        if (child.Name == "AdjacentSpaceId") { continue; }
                        Dictionary<string, bool> surfReq = new Dictionary<string, bool>();
                        surfReq.Add("RectangularGeometry", false);
                        surfReq.Add("PlanarGeometry", false);

                        //find RectangularGeometry node (reqiured)
                        if (child["RectangularGeometry"] != null)
                        {
                            ml.Add("PASS: The required RectangularGeometry Element has been located.");

                            //is any of this really needed, doesn't the XSD validation already do this?
                            XmlNodeList rgchilds = child.ChildNodes;
                            Dictionary<string, bool> rgReq = new Dictionary<string, bool>();
                            rgReq.Add("Azimuth", false);
                            rgReq.Add("CartesianPoint", false);
                            rgReq.Add("Tilt", false);
                            rgReq.Add("Height", false);
                            rgReq.Add("Width", false);
                            //rgReq.Add("PolyLoop", false);

                            foreach (XmlNode rgchild in rgchilds)
                            {
                                string elName = rgchild.Name;
                                if (rgchild["Azimuth"] != null)
                                {
                                    ml.Add("PASS: Required Azimuth Element located.");
                                    rgReq["Azimuth"] = true;
                                }
                                else
                                {
                                    ml.Add("FAIL: Required Azimuth Element not found.");
                                    rgReq["Azimuth"] = false;
                                }

                                if (rgchild["CartesianPoint"] != null)
                                {
                                    ml.Add("PASS: Required Cartesian Element located.");
                                    rgReq["CartesianPoint"] = true;
                                }
                                else
                                {
                                    ml.Add("FAIL: Required Cartesian Element not found.");
                                    rgReq["CartesianPoint"] = false;
                                }

                                if (rgchild["Tilt"] != null)
                                {
                                    ml.Add("PASS: Required Tilt Element located.");
                                    rgReq["Tilt"] = true;
                                }
                                else
                                {
                                    ml.Add("FAIL: Required Tilt Element not found.");
                                    rgReq["Tilt"] = false;
                                }

                                if (rgchild["Height"] != null)
                                {
                                    ml.Add("PASS: Required Height Element located.");
                                    rgReq["Height"] = true;

                                }
                                else
                                {
                                    ml.Add("FAIL: Required Height Element not found.");
                                    rgReq["Height"] = false;
                                }

                                if (rgchild["Width"] != null)
                                {
                                    ml.Add("PASS: Required Width Element located.");
                                    rgReq["Width"] = true;
                                }
                                else
                                {
                                    ml.Add("FAIL: Required Width Element not found.");
                                    rgReq["Width"] = false;
                                }
                                if (rgchild["PolyLoop"] != null)
                                {
                                    //this is optional, not sure if we want to report anything here
                                }

                            }

                            if (rgReq.ContainsValue(false))
                            {
                                //failure
                                report.longMsg = "FAIL:  Not all required Rectangular Geometry elements are present.";
                                report.passOrFail = false;
                                return report;
                            }
                            else
                            {
                                ml.Add("PASS:  Rectangular Geometry element has all required elements.");
                                surfReq["RectangularGeometry"] = true;
                            }
                        }
                        //find PlanarGeometry node (required)
                        else
                        {
                            ml.Add("FAIL: The required RectangularGeometry Element cannot be located.");
                        }
                        if (child["PlanarGeometry"] != null)
                        {
                            ml.Add("PASS: The required PlanarGeometry Element has been successfully located.");
                            //April 14, 2014
                            //We have opted not to check the quality of the planar geometry because this is done elsewhere
                        }
                        else
                        {
                            ml.Add("FAIL: The required PlanarGeometry Element has cannot be located.");
                        }
                    }
                    report.MessageList[surfaceId] = ml;
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;

        }

        public static DOEgbXMLReportingObj SurfaceMatchesSpaceBoundary(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            report.testSummary = "Testing whether the Surface Elements and Space Boundary have matching PolyLoops and ID attributes";
            report.testSummary += "  It is a requirement that Surface ID and Space Boundary surfaceIdRef have the same value.";

            report.TestPassedDict = new Dictionary<string, bool>();
            //pass or fail attribute should always be set to false
            //Feb 5, 2014 - This test ensures that the Surface element definition matches the Space Boundary Definition.  They should be equal
            report.MessageList = new List<string>();
            try
            {
                List<SurfaceDefinitions> surfList = DOEgbXML.XMLParser.GetFileSurfaceDefs(gbXMLDoc, gbXMLnsm);
                List<gbXMLSpaces.SpaceBoundary> sbList = gbXMLSpaces.GetSpaceBoundaryList(gbXMLDoc, gbXMLnsm);


                foreach (SurfaceDefinitions surface in surfList)
                {
                    //surfaceID for reporting
                    string surfaceId = surface.SurfaceId;
                    List<Vector.MemorySafe_CartCoord> surfaceCoords = surface.PlCoords;
                    //we have to make this assumption
                    string sbID = surfaceId;
                    List<Vector.CartCoord> sbCoords = new List<Vector.CartCoord>();
                    for (int sb = 0; sb < sbList.Count; sb++ )
                    {
                        if (sbList[sb].surfaceIdRef == sbID)
                        {
                            report.MessageList.Add("SUCCESS: Surface ID: " + surfaceId + "has found a matching SpaceBoundary with same ID.");
                            report.MessageList.Add("Attempting to match their PolyLoops.");
                            int matchedCoordCount = 0;
                            for (int surfcd = 0; surfcd < surfaceCoords.Count; surfcd++)
                            {
                                Vector.MemorySafe_CartCoord surfcoord = surfaceCoords[surfcd];
                                for (int i = 0; i < sbList[sb].sbplane.pl.plcoords.Count; i++)
                                {
                                    Vector.MemorySafe_CartCoord sbcoord = sbList[sb].sbplane.pl.plcoords[i];
                                    //find a vertex match.  They should match exactly!  No tolerances.  No duplicate vertexes allowed!
                                    if (surfcoord.X == sbcoord.X && surfcoord.Y == sbcoord.Y && surfcoord.Z == sbcoord.Z)
                                    {
                                        report.MessageList.Add("SUCCESS:  Surface coordinate perfect match, for coordinate: (" + surfcoord.X + "," + surfcoord.Y + "," + surfcoord.Z + ")");
                                        matchedCoordCount++;
                                        continue;
                                    }
                                }
                                if (matchedCoordCount == surfaceCoords.Count)
                                {
                                    report.MessageList.Add("SUCCESS: Surface has matched all coordinates with the SurfaceBoundary Coordinates.");
                                    continue;
                                }
                                else
                                {
                                    matchedCoordCount = 0;
                                    for (int i = 0; i < sbList[sb].sbplane.pl.plcoords.Count; i++)
                                    {
                                        Vector.MemorySafe_CartCoord sbcoord = sbList[sb].sbplane.pl.plcoords[i];
                                        report.MessageList.Add("PROBLEM:  Potential floating point error issue in gbXML file, there are " + surfaceCoords.Count + " cuurdinates but only " + matchedCoordCount + " were matched.");
                                        report.MessageList.Add("Trying to match with a small allowable floating point tolerance.");
                                        if (Math.Abs((surfcoord.X - sbcoord.X)) < report.tolerance && Math.Abs((surfcoord.Y - sbcoord.Y)) < report.tolerance && Math.Abs((surfcoord.Z - sbcoord.Z)) < report.tolerance)
                                        {
                                            report.MessageList.Add("SUCCESS:  Surface coordinate match WITHIN tolerance, for coordinate: (" + surfcoord.X + "," + surfcoord.Y + "," + surfcoord.Z + ")");
                                            matchedCoordCount++;
                                            continue;
                                        }
                                    }
                                    if (matchedCoordCount == surfaceCoords.Count)
                                    {
                                        report.MessageList.Add("SUCCESS: Surface has matched all coordinates with the SurfaceBoundary Coordinates.");
                                        report.TestPassedDict.Add(surfaceId, true);
                                        continue;
                                    }
                                    else
                                    {
                                        //could not match the coordinates in any way shape or form
                                        report.MessageList.Add("FAILURE:  Surface ID:" +surfaceId+" could not match its coordinates with the SpaceBoundary Coordinates.");
                                        report.TestPassedDict.Add(surfaceId, false);
                                        //for now we want to continue and just report that this did not work, in the future, this could be a hard fail.
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (sb == sbList.Count - 1)
                            {
                                report.MessageList.Add("FAILURE: Could not find a match of Ids between Surface and Space Boundary for Surface ID: " +surfaceId);
                                report.TestPassedDict.Add(surfaceId, false);
                                //for now we want to continue and just report that this did not work, in the future, this could be a hard fail.
                                continue;
                            }
                            else
                            {
                                //try to find in the next loop around
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            //if we have made it here, we've passed all the tests and should have made it through.
            if(report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAIL:  There were some errors found when matching the Surface and Space Boudaries.";
                report.passOrFail = false;
                return report;
            }
            else
            {
                report.longMsg = "TEST PASSED:  All Surfaces found a SpaceBoundary counterpart.";
                report.passOrFail = true;
                return report;
            }
        }

        public static DOEgbXMLPhase2Report SurfaceMatchesSpaceBoundary(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            report.testSummary = "Testing whether the Surface Elements and Space Boundary have matching PolyLoops and ID attributes";
            report.testSummary += "  It is a requirement that Surface ID and Space Boundary surfaceIdRef have the same value.";

            report.TestPassedDict = new Dictionary<string, bool>();
            //pass or fail attribute should always be set to false
            //Feb 5, 2014 - This test ensures that the Surface element definition matches the Space Boundary Definition.  They should be equal
            
            try
            {
                List<SurfaceDefinitions> surfList = DOEgbXML.XMLParser.GetFileSurfaceDefs(gbXMLDoc, gbXMLnsm);
                List<gbXMLSpaces.SpaceBoundary> sbList = gbXMLSpaces.GetSpaceBoundaryList(gbXMLDoc, gbXMLnsm);
                

                foreach (gbXMLSpaces.SpaceBoundary sb in sbList)
                {
                    List<string> ml = new List<string>();
                    //surfaceID for reporting
                    string sbId = sb.surfaceIdRef;
                    List<Vector.MemorySafe_CartCoord> sbCoords = sb.sbplane.pl.plcoords;
                    //we have to make this assumption
                    string surfaceID = sbId;
                    List<Vector.CartCoord> surfaceCoords = new List<Vector.CartCoord>();
                    int matchedCoordCount = 0;
                    for (int surfnum = 0; surfnum < surfList.Count; surfnum++)
                    {
                        if (surfList[surfnum].SurfaceId == surfaceID)
                        {
                            matchedCoordCount = 0;
                            for (int sbct = 0; sbct < sbCoords.Count; sbct++)
                            {
                                Vector.MemorySafe_CartCoord sbcoord = sbCoords[sbct];
                                for (int i = 0; i < surfList[surfnum].PlCoords.Count; i++)
                                {
                                    Vector.MemorySafe_CartCoord surfcoord = surfList[surfnum].PlCoords[i];
                                    //find a vertex match.  They should match exactly!  No tolerances.  No duplicate vertexes allowed!
                                    if (surfcoord.X == sbcoord.X && surfcoord.Y == sbcoord.Y && surfcoord.Z == sbcoord.Z)
                                    {
                                        ml.Add("PERFECT MATCH:  Surface coordinate perfect match, for coordinate: (" + surfcoord.X + "," + surfcoord.Y + "," + surfcoord.Z + ")");
                                        matchedCoordCount++;
                                        break;
                                    }
                                    else if (Math.Abs(surfcoord.X - sbcoord.X) < report.tolerance && Math.Abs(surfcoord.Y - sbcoord.Y) < report.tolerance && Math.Abs(surfcoord.Z - sbcoord.Z) < report.tolerance)
                                    {
                                        ml.Add("MATCH:  Surface coordinates match within allowable tolerance, for coordinate: (" + surfcoord.X + "," + surfcoord.Y + "," + surfcoord.Z + ")");
                                        matchedCoordCount++;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (surfnum == surfList.Count - 1)
                            {
                                ml.Add("FAILURE: Could not find a match of Ids between Surface and Space Boundary for Surface ID: " + sbId);
                                report.TestPassedDict.Add(sbId, false);
                                //for now we want to continue and just report that this did not work, in the future, this could be a hard fail.
                                continue;
                            }
                            else
                            {
                                //try to find in the next loop around
                                continue;
                            }
                        }
                        //we are done
                        if (matchedCoordCount == sbCoords.Count)
                        {
                            ml.Add("PASS: Surface has matched all coordinates with the SurfaceBoundary Coordinates.");
                            if(report.TestPassedDict.ContainsKey(sbId))
                            {
                                //this is my special way of allowing a surface boundary to be represented twice
                                report.TestPassedDict.Add(sbId+"-2", true);
                                report.MessageList[sbId+"-2"] = ml;
                            }
                            else
                            {
                                report.TestPassedDict.Add(sbId, true);
                                report.MessageList[sbId] = ml;
                            }
                            break;
                        }
                        else
                        {
                            //could not match the coordinates in any way shape or form
                            ml.Add("FAIL:  Surface could not match its coordinates with the SpaceBoundary Coordinates.");
                            if (report.TestPassedDict.ContainsKey(sbId))
                            {
                                report.TestPassedDict.Add(sbId+"-2", false);
                                report.MessageList[sbId+"-2"] = ml;
                            }
                            else
                            {
                                report.TestPassedDict.Add(sbId, false);
                                report.MessageList[sbId] = ml;
                            }
                            //for now we want to continue and just report that this did not work, in the future, this could be a hard fail.
                            break;
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            //if we have made it here, we've passed all the tests and should have made it through.
            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAIL:  There were some errors found when matching the Surface and Space Boundaries coordinates.";
                report.passOrFail = false;
                return report;
            }
            else
            {
                report.longMsg = "TEST PASSED:  All Surfaces found a SpaceBoundary counterpart.";
                report.passOrFail = true;
                return report;
            }
        }

        public static DOEgbXMLReportingObj SurfaceSelfIntersectionTest(List<SurfaceDefinitions> surfaces, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that each surface, defined inside the gbXML Space element, is not self-intersecting.";
            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            try
            {
                report.MessageList.Add("Beginning test to check each PolyLoop of Space objects to ensure they do not form self-intersecting polygons.");
                report.MessageList.Add("Self-intersecting polygons are not allowed.");

                foreach (SurfaceDefinitions surface in surfaces)
                {
                    report.MessageList.Add("Testing Surface id: " + surface.SurfaceId + ".");
                    var wd = Vector.isCounterClockwise(surface.PlCoords);
                    if (wd == Vector.WalkDirection.Counterclockwise)
                    {
                        report.MessageList.Add("PASS: Surface id: " + surface.SurfaceId + " polyloop description is not self-intersecting.");
                        report.TestPassedDict.Add(surface.SurfaceId, true);
                    }
                    else
                    {
                        report.MessageList.Add("SELF INTERSECTION DETECTED: Surface id: " + surface.SurfaceId + " is self-intersecting.");
                        report.TestPassedDict.Add(surface.SurfaceId, false);
                        
                    }

                 }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj SurfaceEnclosureTest(Dictionary<string, List<SurfaceDefinitions>> enclosure, DOEgbXMLReportingObj report)
        {
            try
            {
                foreach (KeyValuePair<string, List<SurfaceDefinitions>> kp in enclosure)
                {
                    report.MessageList.Add("Checking surfaces associated with Space: " + kp.Key);
                    
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj SurfaceTiltAndAzCheck(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //pass or fail attribute should always be set to false
            //Feb 6 2014 - gbXML XSD version 5.11
            //this test ensures that each surface element has the required attributes and elements, and that all enumerations are properly declared
            report.MessageList = new List<string>();
            try
            {
                List<SurfaceDefinitions> surfList = DOEgbXML.XMLParser.GetFileSurfaceDefs(gbXMLDoc, gbXMLnsm);

                foreach (SurfaceDefinitions surface in surfList)
                {
                    //calculate azimuth and tilt, without considering the CADModelAzimuth
                    Vector.MemorySafe_CartVect normal = surface.PlRHRVector;
                    double calculatedTilt = DOEgbXMLBasics.FindTilt(normal);
                    double calculatedAzimuth = DOEgbXMLBasics.FindAzimuth(normal);

                    if (calculatedTilt == surface.Tilt)
                    {
                        //perfect
                        report.MessageList.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Tilt: "+calculatedTilt.ToString()+" that is identical to Surface's Tilt Element Text: " + surface.Tilt.ToString());
                        report.TestPassedDict.Add(surface.SurfaceId, true);
                    }
                    else if (Math.Abs(calculatedTilt - surface.Tilt) < report.tolerance)
                    {
                        //good
                        report.MessageList.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Tilt: " + calculatedTilt.ToString() + " that is within tolerance of the Surface's Tilt Element Text: " + surface.Tilt.ToString());
                        report.TestPassedDict.Add(surface.SurfaceId, true);
                    }
                    else
                    {
                        //bad
                        report.MessageList.Add("FAIL: Surface's planar geometry polyloop forms RHR with Tilt: " + calculatedTilt.ToString() + " that is not within tolerance of Surface's Tilt Element Text: " + surface.Tilt.ToString());
                        report.TestPassedDict.Add(surface.SurfaceId, false);
                        //we stop here, this is the best I can seem to do here for now.
                        continue;
                    }

                    if (calculatedAzimuth == surface.Azimuth)
                    {
                        report.MessageList.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Azimuth: " + calculatedAzimuth.ToString() + " that is identical to Surface's Azimuth Element Text: " + surface.Azimuth.ToString());
                        report.TestPassedDict.Add(surface.SurfaceId, true);
                    }
                    else if (Math.Abs(calculatedAzimuth - surface.Azimuth) < report.tolerance)
                    {
                        //good
                        report.MessageList.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Azimuth: " + calculatedAzimuth.ToString() + " that is within tolerance of the Surface's Tilt Element Text: " + surface.Azimuth.ToString());
                    }
                    else
                    {
                        //bad
                        report.MessageList.Add("FAIL: Surface's planar geometry polyloop forms RHR with Azimuth: " + calculatedAzimuth.ToString() + " that is not within tolerance of Surface's Azimuth Element Text: " + surface.Azimuth.ToString());
                        report.TestPassedDict.Add(surface.SurfaceId, false);
                        //we stop here, this is the best I can seem to do here for now.
                        continue;
                    }

                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }

            //if we made it this far, we did not run into any exceptions
            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAIL:  There were some errors found when comparing the Surfaces' Planar Geometry Definition and the Surfaces' Tilt and Azimuth.";
                report.passOrFail = false;
                return report;
            }
            else
            {
                report.longMsg = "TEST PASSED:  All Surfaces' Tilt and Azimuth Values appear to coincide with the Planar Geometry Definition Provided.";
                report.passOrFail = true;
                return report;
            }
        }

        public static DOEgbXMLPhase2Report SurfaceTiltAndAzCheck(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //pass or fail attribute should always be set to false
            //Feb 6 2014 - gbXML XSD version 5.11
            //this test ensures that each surface element has the required attributes and elements, and that all enumerations are properly declared
            report.testSummary = "This test checks the polygon geometry definition wrapped in each space, and ensures the tilt and azimuth definitions coincide.";
            try
            {
                List<SurfaceDefinitions> surfList = DOEgbXML.XMLParser.GetFileSurfaceDefs(gbXMLDoc, gbXMLnsm);

                foreach (SurfaceDefinitions surface in surfList)
                {
                    bool tiltPassed = false;
                    bool azPassed = false;
                    //calculate azimuth and tilt, without considering the CADModelAzimuth
                    Vector.MemorySafe_CartVect normal = surface.PlRHRVector;
                    double calculatedTilt = DOEgbXMLBasics.FindTilt(normal);
                    double calculatedAzimuth = DOEgbXMLBasics.FindAzimuth(normal);
                    List<string> ml = new List<string>();
                    ml.Add(surface.SurfaceId + " start testing tilt and azimuth matchs to polygon's PolyLoop definition in Rectangular Geometry.");
                    if (calculatedTilt == surface.Tilt)
                    {
                        //perfect
                        ml.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Tilt: " + calculatedTilt.ToString() + " that is identical to Surface's Tilt Element Text: " + surface.Tilt.ToString());
                        tiltPassed = true;
                    }
                    else if (Math.Abs(calculatedTilt - surface.Tilt) < report.vectorangletol)
                    {
                        //good
                        ml.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Tilt: " + calculatedTilt.ToString() + " that is within tolerance of the Surface's Tilt Element Text: " + surface.Tilt.ToString());
                        tiltPassed = true;
                    }
                    else
                    {
                        //bad
                        ml.Add("FAIL: Surface's planar geometry polyloop forms RHR with Tilt: " + calculatedTilt.ToString() + " that is not within tolerance of Surface's Tilt Element Text: " + surface.Tilt.ToString());
                        
                        //we stop here, this is the best I can seem to do here for now.
                        continue;
                    }

                    if (calculatedAzimuth == surface.Azimuth)
                    {
                        ml.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Azimuth: " + calculatedAzimuth.ToString() + " that is identical to Surface's Azimuth Element Text: " + surface.Azimuth.ToString());
                        azPassed = true;
                    }
                    else if (Math.Abs(calculatedAzimuth - surface.Azimuth) < report.tolerance)
                    {
                        //good
                        ml.Add("SUCCESS: Surface's planar geometry polyloop forms RHR with Azimuth: " + calculatedAzimuth.ToString() + " that is within tolerance of the Surface's Tilt Element Text: " + surface.Azimuth.ToString());
                        azPassed = true;
                    }
                    else
                    {
                        //bad
                        ml.Add("FAIL: Surface's planar geometry polyloop forms RHR with Azimuth: " + calculatedAzimuth.ToString() + " that is not within tolerance of Surface's Azimuth Element Text: " + surface.Azimuth.ToString());
                        
                        //we stop here, this is the best I can seem to do here for now.
                        continue;
                    }
                    report.MessageList[surface.SurfaceId] = ml;
                    if (azPassed && tiltPassed)
                    {
                        report.TestPassedDict[surface.SurfaceId] = true;
                    }
                    else
                    {
                        report.TestPassedDict[surface.SurfaceId] = false;
                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = ("SORRY, we have run into an unexpected issue:" + e.ToString());
                report.passOrFail = false;
                return report;
            }

            //if we made it this far, we did not run into any exceptions
            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "TEST FAIL:  There were some errors found when comparing the Surfaces' Planar Geometry Definition and the Surfaces' Tilt and Azimuth.";
                report.passOrFail = false;
                return report;
            }
            else
            {
                report.longMsg = "TEST PASSED:  All Surfaces' Tilt and Azimuth Values appear to coincide with the Planar Geometry Definition Provided.";
                report.passOrFail = true;
                return report;
            }
        }
    }


    public class SubSurfaceDefinition
    {
        //creates instances of an object that store information about the subsurface in a gbXML file
        public string id;
        public string openingType;
        public string name;
        public double tilt;
        public double width;
        public double height;
        public double area;
        public double azimuth;
        public string orientation;
        public Vector.CartCoord InsertionPoint;
        public Vector.MemorySafe_CartVect plRHRVect;
        public List<Vector.MemorySafe_CartCoord> PlCoords;

        //parent data
        public string parentID;
        public double parentTilt;
        public double parentAzimuth;
        public double parentHeight;

        //constructor?
        public SubSurfaceDefinition(XmlNode surfaceNode)
        {
            //get id and surfaceType
            XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
            foreach (XmlAttribute at in spaceAtts)
            {
                if (at.Name == "id")
                {
                    id = at.Value;
                }
                else if (at.Name == "openingType")
                {
                    openingType = at.Value;
                }
            }
            if (surfaceNode.HasChildNodes)
            {
                XmlNodeList surfChildNodes = surfaceNode.ChildNodes;
                foreach (XmlNode node in surfChildNodes)
                {
                    if (node.Name == "RectangularGeometry")
                    {
                        if (node.HasChildNodes)
                        {
                            XmlNodeList rectGeomChildren = node.ChildNodes;
                            foreach (XmlNode rgChildNode in rectGeomChildren)
                            {
                                if (rgChildNode.Name == "Azimuth") { azimuth = Convert.ToDouble(rgChildNode.InnerText); }
                                else if (rgChildNode.Name == "CartesianPoint")
                                {
                                    if (rgChildNode.HasChildNodes)
                                    {
                                        XmlNodeList coordinates = rgChildNode.ChildNodes;
                                        int pointCount = 1;
                                        foreach (XmlNode coordinate in coordinates)
                                        {
                                            switch (pointCount)
                                            {
                                                case 1:
                                                    InsertionPoint.X = Convert.ToDouble(coordinate.InnerText);
                                                    break;
                                                case 2:
                                                    InsertionPoint.Y = Convert.ToDouble(coordinate.InnerText);
                                                    break;
                                                case 3:
                                                    InsertionPoint.Z = Convert.ToDouble(coordinate.InnerText);
                                                    break;
                                            }
                                            pointCount++;
                                        }
                                    }
                                }
                                else if (rgChildNode.Name == "Tilt") { tilt = Convert.ToDouble(rgChildNode.InnerText); }
                                else if (rgChildNode.Name == "Height") { height = Convert.ToDouble(rgChildNode.InnerText); }
                                else if (rgChildNode.Name == "Width") { width = Convert.ToDouble(rgChildNode.InnerText); }
                            }
                        }
                    }
                    else if (node.Name == "PlanarGeometry")
                    {
                        XmlNode polyLoop = node.FirstChild;
                        if (polyLoop.HasChildNodes)
                        {
                            XmlNodeList cartesianPoints = polyLoop.ChildNodes;
                            foreach (XmlNode coordinatePt in cartesianPoints)
                            {
                                Vector.CartCoord coord = new Vector.CartCoord();
                                if (coordinatePt.HasChildNodes)
                                {
                                    XmlNodeList coordinates = coordinatePt.ChildNodes;
                                    int pointCount = 1;
                                    foreach (XmlNode coordinate in coordinatePt)
                                    {

                                        switch (pointCount)
                                        {
                                            case 1:
                                                coord.X = Convert.ToDouble(coordinate.InnerText);
                                                break;
                                            case 2:
                                                coord.Y = Convert.ToDouble(coordinate.InnerText);
                                                break;
                                            case 3:
                                                coord.Z = Convert.ToDouble(coordinate.InnerText);
                                                break;
                                        }
                                        pointCount++;
                                    }
                                    Vector.MemorySafe_CartCoord memsafecoord = Vector.convertToMemorySafeCoord(coord);
                                    PlCoords.Add(memsafecoord);
                                }
                            }
                        }
                    }
                }
            }
            plRHRVect = Vector.GetMemRHR(PlCoords);
        }

        //find the orientation of the surface.
        //The orientation is defined according to ...
        public string surfaceOrientation()
        {
            if (orientation == null)
            {
                Vector.MemorySafe_CartVect normVect = Vector.convertToMemorySafeVector(DOEgbXMLBasics.getNorm(PlCoords));
                double calculatedAzimuth = DOEgbXMLBasics.FindAzimuth(normVect);
                orientation = DOEgbXMLBasics.getFaceDirection(calculatedAzimuth);
            }

            return orientation;

        }

        public double computeArea()
        {
            if (area > 0)
            {
                return area;
            }

            area = DOEgbXMLBasics.computeArea(PlCoords);
            if (area == -1)
            {
                //send warning: PlCoords has less than 3 coordinates
                return 0;
            }
            else
            {
                return area;

            }
        }


    }

    class SurfaceResults
    {
        public int matchCount;
        public Dictionary<string, List<string>> SurfaceIdMatch;
    }
}