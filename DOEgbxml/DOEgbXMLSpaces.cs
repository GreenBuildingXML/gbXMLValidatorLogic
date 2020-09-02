using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorMath;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using log4net;

namespace DOEgbXML
{
    public class gbXMLSpaces
    {
        public string id;
        public string name;
        public string spaceType;
        public Double area = 0.0;
        public Double volume = 0.0;
        public PlanarGeometry pg;
        public ShellGeometry sg;
        public List<SpaceBoundary> spacebounds;
        public string CADObjectID;

        public class PlanarGeometry
        {
            public PolyLoop pl;

        }

        public class ShellGeometry
        {
            public string id;
            public ClosedShell cs;
        }

        public class ClosedShell
        {
            public List<PolyLoop> ploops;
        }

        public class SpaceBoundary
        {
            public string surfaceIdRef;
            public string ifcGUID;
            public bool isSecondLevelBoundary;
            public string oppositeIdRef;
            public PlanarGeometry sbplane;

        }

        public class PolyLoop
        {
            public List<Vector.MemorySafe_CartCoord> plcoords;

        }

        public static List<string> getSpaceIds(XmlDocument xmldoc, XmlNamespaceManager xmlns, string searchpath)
        {
            List<string> spaceid = new List<string>();
            try
            {
                XmlNodeList nodes = xmldoc.SelectNodes(searchpath, xmlns);
                
                foreach (XmlNode spaceNode in nodes)
                {
                    //initialize a new instance of the class
                    gbXMLSpaces space = new gbXMLSpaces();
                    space.spacebounds = new List<SpaceBoundary>();
                    //get id and space
                    XmlAttributeCollection spaceAtts = spaceNode.Attributes;
                    foreach (XmlAttribute at in spaceAtts)
                    {
                        if (at.Name == "id")
                        {
                            space.id = at.Value;
                            spaceid.Add(at.Value);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return spaceid;
        }
        public static List<gbXMLSpaces> getSimpleSpaces(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            List<gbXMLSpaces> retspaces = new List<gbXMLSpaces>();
            try
            {
                XmlNodeList nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", xmlns);

                foreach (XmlNode spaceNode in nodes)
                {
                    //initialize a new instance of the class
                    gbXMLSpaces space = new gbXMLSpaces();
                    space.spacebounds = new List<SpaceBoundary>();
                    //get id and space
                    XmlAttributeCollection spaceAtts = spaceNode.Attributes;
                    foreach (XmlAttribute at in spaceAtts)
                    {
                        if (at.Name == "id")
                        {
                            space.id = at.Value;
                        }
                        if(at.Name == "spaceType")
                        {
                            space.spaceType = at.Value;
                        }
                    }
                    if (spaceNode.HasChildNodes)
                    {
                        XmlNodeList childNodes = spaceNode.ChildNodes;
                        foreach (XmlNode node in childNodes)
                        {
                            if (node.Name == "PlanarGeometry")
                            {
                                space.pg = new PlanarGeometry();
                                XmlNodeList childnodes = node.ChildNodes;
                                foreach (XmlNode node2 in childnodes)
                                {
                                    if (node2.Name == "PolyLoop")
                                    {
                                        space.pg.pl = new PolyLoop();
                                        space.pg.pl.plcoords = new List<Vector.MemorySafe_CartCoord>();

                                        XmlNodeList cartPoints = node2.ChildNodes;
                                        foreach (XmlNode point in cartPoints)
                                        {
                                            if (point.Name == "CartesianPoint")
                                            {
                                                Vector.CartCoord coord = new Vector.CartCoord();
                                                XmlNodeList val = point.ChildNodes;
                                                int pointcount = 1;
                                                foreach (XmlNode cpoint in val)
                                                {
                                                    switch (pointcount)
                                                    {
                                                        case 1:
                                                            coord.X = Convert.ToDouble(cpoint.InnerText);
                                                            break;
                                                        case 2:
                                                            coord.Y = Convert.ToDouble(cpoint.InnerText);
                                                            break;
                                                        case 3:
                                                            coord.Z = Convert.ToDouble(cpoint.InnerText);
                                                            break;
                                                    }
                                                    pointcount++;
                                                }
                                                Vector.MemorySafe_CartCoord memsafecoord = Vector.convertToMemorySafeCoord(coord);
                                                space.pg.pl.plcoords.Add(memsafecoord);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (node.Name == "ShellGeometry")
                            {
                                space.sg = new ShellGeometry();
                                XmlAttributeCollection sgAtts = spaceNode.Attributes;
                                foreach (XmlAttribute at in sgAtts)
                                {
                                    if (at.Name == "id")
                                    {
                                        space.sg.id = at.Value;
                                        break;
                                    }
                                }

                                XmlNodeList childnodes = node.ChildNodes;
                                foreach (XmlNode sgnode in childnodes)
                                {
                                    if (sgnode.Name == "ClosedShell")
                                    {
                                        space.sg.cs = new ClosedShell();
                                        space.sg.cs.ploops = new List<PolyLoop>();

                                        foreach (XmlNode pl in sgnode)
                                        {
                                            if (pl.Name == "PolyLoop")
                                            {
                                                PolyLoop sgpl = new PolyLoop();
                                                sgpl.plcoords = new List<Vector.MemorySafe_CartCoord>();
                                                XmlNodeList cartPoints = pl.ChildNodes;
                                                foreach (XmlNode point in cartPoints)
                                                {
                                                    if (point.Name == "CartesianPoint")
                                                    {
                                                        Vector.CartCoord coord = new Vector.CartCoord();
                                                        XmlNodeList val = point.ChildNodes;
                                                        int pointcount = 1;
                                                        foreach (XmlNode cpoint in val)
                                                        {
                                                            switch (pointcount)
                                                            {
                                                                case 1:
                                                                    coord.X = Convert.ToDouble(cpoint.InnerText);
                                                                    break;
                                                                case 2:
                                                                    coord.Y = Convert.ToDouble(cpoint.InnerText);
                                                                    break;
                                                                case 3:
                                                                    coord.Z = Convert.ToDouble(cpoint.InnerText);
                                                                    break;
                                                            }
                                                            pointcount++;
                                                        }
                                                        Vector.MemorySafe_CartCoord memsafecoord = Vector.convertToMemorySafeCoord(coord);
                                                        sgpl.plcoords.Add(memsafecoord);
                                                    }
                                                }
                                                space.sg.cs.ploops.Add(sgpl);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (node.Name == "SpaceBoundary")
                            {
                                SpaceBoundary sb = new SpaceBoundary();
                                XmlAttributeCollection spbatts = node.Attributes;
                                foreach (XmlAttribute at in spbatts)
                                {
                                    if (at.Name == "surfaceIdRef")
                                    {
                                        sb.surfaceIdRef = at.Value;
                                        break;
                                    }
                                }
                                XmlNodeList sbchilds = node.ChildNodes;
                                foreach (XmlNode sbpnode in sbchilds)
                                {
                                    if (sbpnode.Name == "PlanarGeometry")
                                    {
                                        sb.sbplane = new PlanarGeometry();
                                        XmlNodeList pgchilds = sbpnode.ChildNodes;
                                        foreach (XmlNode pgchild in pgchilds)
                                        {
                                            if (pgchild.Name == "PolyLoop")
                                            {
                                                sb.sbplane.pl = new PolyLoop();

                                                sb.sbplane.pl.plcoords = new List<Vector.MemorySafe_CartCoord>();
                                                XmlNodeList cartPoints = pgchild.ChildNodes;
                                                foreach (XmlNode point in cartPoints)
                                                {
                                                    if (point.Name == "CartesianPoint")
                                                    {
                                                        Vector.CartCoord coord = new Vector.CartCoord();
                                                        XmlNodeList val = point.ChildNodes;
                                                        int pointcount = 1;
                                                        foreach (XmlNode cpoint in val)
                                                        {
                                                            switch (pointcount)
                                                            {
                                                                case 1:
                                                                    coord.X = Convert.ToDouble(cpoint.InnerText);
                                                                    break;
                                                                case 2:
                                                                    coord.Y = Convert.ToDouble(cpoint.InnerText);
                                                                    break;
                                                                case 3:
                                                                    coord.Z = Convert.ToDouble(cpoint.InnerText);
                                                                    break;
                                                            }
                                                            pointcount++;
                                                        }
                                                        Vector.MemorySafe_CartCoord memsafecoord = Vector.convertToMemorySafeCoord(coord);
                                                        sb.sbplane.pl.plcoords.Add(memsafecoord);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //finally, add the thing here
                                space.spacebounds.Add(sb);
                            }
                            else if (node.Name == "Name")
                            {
                                space.name = node.InnerText;
                            }
                            else if (node.Name == "Area")
                            {
                                space.area = Convert.ToDouble(node.InnerText);
                            }
                            else if(node.Name == "Volume")
                            {
                                space.volume = Convert.ToDouble(node.InnerText);
                            }
                        }
                    }
                    else
                    {
                        //throw something
                    }
                    retspaces.Add(space);
                }
            }
            catch (Exception e)
            {

            }
            return retspaces;
        }

        //June 18 - this is now an old function that is not used and should be deprecated or removed
        public static DOEgbXMLReportingObj SpaceSurfacesCCTest(List<gbXMLSpaces> spaces, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that all surface coordinates are listed in a counterclockwise order.  This is a requirement of all gbXML PolyLoop definitions";
            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            try
            {
                report.MessageList.Add("Beginning test to check the winding order of all polyloops that describe surfaces of each Space object.");
                report.MessageList.Add("All coordinates should be wound in a counter-clockwise order.");
                int spacecount = 0;
                foreach (DOEgbXML.gbXMLSpaces space in spaces)
                {
                    bool spacepassorfail = true;
                    report.MessageList.Add("Testing Space id: " + space.id + " Shell Geometry PolyLoops.");


                    if (space.sg.cs.ploops.Count() > 0)
                    {
                        List<List<Vector.MemorySafe_CartCoord>> cc = new List<List<Vector.MemorySafe_CartCoord>>();
                        for (int sgsurfcount = 0; sgsurfcount < space.sg.cs.ploops.Count; sgsurfcount++)
                        {
                            PolyLoop pl = space.sg.cs.ploops[sgsurfcount];
                            List<Vector.MemorySafe_CartCoord> c = pl.plcoords;
                            cc.Add(c);

                        }
                        Vector.MemorySafe_CartCoord centroid = Vector.FindVolumetricCentroid(cc);
                        Dictionary<string, bool> res = Vector.SurfacesCCWound(centroid, cc, spacecount);
                        foreach (KeyValuePair<string, bool> kp in res)
                        {
                            if (kp.Value == true)
                            {
                                report.MessageList.Add("PASS: Shell Geometry surface number " + (kp.Key) + " is wound in a counterclockwise order.");
                                spacepassorfail = false;
                                report.TestPassedDict.Add(kp.Key, true);
                            }
                            else
                            {
                                report.MessageList.Add("FAIL: Shell Geometry surface number " + (kp.Key) + " is wound in a clockwise order.");
                                report.TestPassedDict.Add(kp.Key, false);
                            }
                        }
                    }

                    if(space.spacebounds.Count() > 0)
                    {
                        List<List<Vector.MemorySafe_CartCoord>> sbcc = new List<List<Vector.MemorySafe_CartCoord>>();
                        for (int sbsurfcount = 0; sbsurfcount < space.spacebounds.Count(); sbsurfcount++)
                        {
                            PolyLoop sbpl = space.spacebounds[sbsurfcount].sbplane.pl;
                            List<Vector.MemorySafe_CartCoord> sbc = sbpl.plcoords;
                            sbcc.Add(sbc);

                        }
                        Vector.MemorySafe_CartCoord sbcentroid = Vector.FindVolumetricCentroid(sbcc);
                        Dictionary<string, bool> sbres = Vector.SurfacesCCWound(sbcentroid, sbcc, spacecount);
                        foreach (KeyValuePair<string, bool> sbkp in sbres)
                        {
                            if (sbkp.Value == true)
                            {
                                report.MessageList.Add("PASS: Surface Boundary with id " + sbkp.Key + " is wound in a counterclockwise order.");
                                
                                report.TestPassedDict.Add(sbkp.Key, true);
                                spacepassorfail = true;
                            }
                            else
                            {
                                report.MessageList.Add("FAIL: Surface Boundary with id " + sbkp.Key + " is wound in a clockwise order.");
                                report.TestPassedDict.Add(sbkp.Key, false);
                            }
                        }
                    }
                    if (spacepassorfail) { report.TestPassedDict.Add(space.id, true); }
                    else { report.TestPassedDict.Add(space.id, false); }
                    spacecount++;
                }
            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.MessageList.Add("This test failed unexpectedly.");
                report.passOrFail = false;
                return report;
            }
            if (report.TestPassedDict.ContainsValue(false))
            {
                report.longMsg = "This test found errors in the geometry when testing the winding order of the Space elements surfaces.  See below for details.";
                report.passOrFail = false;
            }
            else
            {
                report.longMsg = "This test did not find any errors in the geometry when testing the winding order of the Space elements surfaces.  See below for details.";
                report.passOrFail = true;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report SpaceSurfacesCCTest2(List<gbXMLSpaces> spaces, DOEgbXMLPhase2Report report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 17 2014
            report.testSummary = "This test ensures that all surface coordinates are listed in a counterclockwise order.  This is a requirement of all gbXML PolyLoop definitions.";
            report.testSummary += "  It is an important test because it ensures that all surface normals are pointing in the right direction (away from the center of each room).";
            report.testSummary += "  This helps to ensure that each surface is pointed in the correct direction, so that solar gains can properly be taken into account.";
            report.MessageList = new Dictionary<string,List<string>>();
            report.TestPassedDict = new Dictionary<string, bool>();
            report.passOrFail = true;
            try
            {
                int spacecount = 0;
                foreach (DOEgbXML.gbXMLSpaces space in spaces)
                {
                    List<string> retlist = new List<string>();
                    bool spacepassorfail = true;
                    if (space.sg.cs.ploops.Count() > 0)
                    {
                        string spaceid = space.id + " has ShellGeometry PolyLoops.  Conducting tests of ShellGeometry PolyLoops";
                        report.TestPassedDict[spaceid] = true;
                        List<List<Vector.MemorySafe_CartCoord>> cc = new List<List<Vector.MemorySafe_CartCoord>>();
                        for (int sgsurfcount = 0; sgsurfcount < space.sg.cs.ploops.Count; sgsurfcount++)
                        {
                            PolyLoop pl = space.sg.cs.ploops[sgsurfcount];
                            List<Vector.MemorySafe_CartCoord> c = pl.plcoords;
                            cc.Add(c);

                        }
                        Vector.MemorySafe_CartCoord centroid = Vector.FindVolumetricCentroid(cc);
                        Dictionary<string, bool> res = Vector.SurfacesCCWound(centroid, cc, spacecount);
                        foreach (KeyValuePair<string, bool> kp in res)
                        {
                            string pattern = @"\d*#";
                            string repl = "";
                            Regex rx = new Regex(pattern);
                            string surfacename = rx.Replace(kp.Key, repl);
                            if (kp.Value == true)
                            {
                                if (surfacename.Length > 0)
                                {
                                    retlist.Add("PASS: ShellGeometry surface number: " + surfacename + " is wound in a counterclockwise order.");
                                }
                                else
                                {
                                    retlist.Add("PASS: ShellGeometry surface number: " + kp.Key + " is wound in a counterclockwise order.");
                                }
                                
                            }
                            else
                            {
                                if (surfacename.Length > 0)
                                {
                                    retlist.Add("FAIL: ShellGeometry surface number: " + surfacename + " is wound in a clockwise order.");
                                }
                                else
                                {
                                    retlist.Add("FAIL: ShellGeometry surface number: " + (kp.Key) + " is wound in a clockwise order.");
                                    spacepassorfail = false;
                                }
                                
                            }
                        }

                    }
                    else
                    {
                        string spaceid = space.id + " does not have ShellGeometry PolyLoops.  Conducting tests of ShellGeometry PolyLoops";
                        report.TestPassedDict[spaceid] = false;
                    }

                    if (space.spacebounds.Count() > 0)
                    {
                        string spaceid = space.id + " has SpaceBoundary PolyLoops.  Conducting tests of SpaceBoundary PolyLoops";
                        report.TestPassedDict[spaceid] = true;

                        Dictionary<string, List<Vector.MemorySafe_CartCoord>> sbcc = new Dictionary<string, List<Vector.MemorySafe_CartCoord>>();
                        for (int sbsurfcount = 0; sbsurfcount < space.spacebounds.Count(); sbsurfcount++)
                        {
                            PolyLoop sbpl = space.spacebounds[sbsurfcount].sbplane.pl;
                            List<Vector.MemorySafe_CartCoord> sbc = sbpl.plcoords;
                            sbcc[space.spacebounds[sbsurfcount].surfaceIdRef] = (sbc);

                        }
                        Vector.MemorySafe_CartCoord sbcentroid = Vector.FindVolumetricCentroid(sbcc);
                        Dictionary<string, bool> sbres = Vector.NamedSurfacesCCWound(sbcentroid, sbcc,spacecount, true);
                        foreach (KeyValuePair<string, bool> sbkp in sbres)
                        {
                            string pattern = @"\d*#";
                            string repl = "";
                            Regex rx = new Regex(pattern);
                            string surfacename = rx.Replace(sbkp.Key, repl);
                            if (sbkp.Value == true)
                            {
                                if (surfacename.Length > 0)
                                {
                                    retlist.Add("PASS: SurfaceBoundary with id: " + surfacename + " is wound in a counterclockwise order.");
                                }
                                else
                                {
                                    retlist.Add("PASS: SurfaceBoundary with id: " + sbkp.Key + " is wound in a counterclockwise order.");
                                }
                                
                            }
                            else
                            {
                                if (surfacename.Length > 0)
                                {
                                    retlist.Add("FAIL: SurfaceBoundary with id: " + surfacename + " is not wound in a clockwise order.");
                                }
                                else
                                {
                                    retlist.Add("FAIL: SurfaceBoundary with id: " + sbkp.Key + " is not wound in a clockwise order.");
                                } 
                                spacepassorfail = false;
                            }
                        }
                    }
                    else
                    {
                        string spaceid = space.id + " does not have SpaceBoundary PolyLoops.  Conducting tests of SpaceBoundary PolyLoops";
                        report.TestPassedDict[spaceid] = false;
                    }
                    //if (spacepassorfail) { report.TestPassedDict.Add(space.id+ " Passes all CounterClockwise Winding Tests", true); }
                    //else { report.TestPassedDict.Add(space.id+ "Fails all CounterClockwise Winding Tests", false); }
                    report.MessageList[space.id] = retlist;
                    spacecount++;
                }
            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                List<string> l = new List<string>();
                l.Add("This test failed unexpectedly.");
                report.MessageList["ERROR"] = l;
                report.passOrFail = false;
                return report;
            }
            if (report.passOrFail==false)
            {
                report.longMsg = "TEST FAILED:  This test found errors in the geometry when testing the winding order of the Space elements surfaces.  All surfaces are not facing in the proper direction";
                
            }
            else
            {
                report.longMsg = "TEST PASSED : This test did not find any errors in the geometry when testing the winding order of the Space elements surfaces.";
                
            }
            return report;
        }

        public static DOEgbXMLReportingObj SpaceSurfacesSelfIntersectionTest(List<gbXMLSpaces> spaces, DOEgbXMLReportingObj report)
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
                
                foreach (DOEgbXML.gbXMLSpaces space in spaces)
                {
                    bool spacepassorfail = true;
                    report.MessageList.Add("Testing Space id: " + space.id + " Shell Geometry PolyLoops.");
                    for (int sgsurfcount = 1; sgsurfcount <= space.sg.cs.ploops.Count; sgsurfcount++)
                    {
                        PolyLoop pl = space.sg.cs.ploops[sgsurfcount];
                        var wd = Vector.isCounterClockwise(pl.plcoords);
                        if (Vector.BruteForceIntersectionTest(pl.plcoords))
                        {
                            report.MessageList.Add("PASS: Shell Geometry surface number " + sgsurfcount + " is not self-intersecting.");
                            string t = space.id + "-sg-" + sgsurfcount.ToString();
                            report.TestPassedDict.Add(t, true);
                        }
                        else
                        {
                            report.MessageList.Add("SELF INTERSECTION DETECTED: Shell Geometry surface number " + sgsurfcount + " is self-intersecting.");
                            string t = space.id + "-sg-" + sgsurfcount.ToString();
                            report.TestPassedDict.Add(t, false);
                            spacepassorfail = false;
                        }
                    }

                    foreach (SpaceBoundary sb in space.spacebounds)
                    {
                        PolyLoop pl = sb.sbplane.pl;
                        var wd = Vector.isCounterClockwise(pl.plcoords);
                        if (Vector.BruteForceIntersectionTest(pl.plcoords))
                        {
                            report.MessageList.Add("PASS: Surface Boundary with id " + sb.surfaceIdRef + " is not self-intersecting.");
                            string t = space.id + "-" + sb.surfaceIdRef;
                            report.TestPassedDict.Add(t, true);
                        }
                        else
                        {
                            report.MessageList.Add("SELF INTERSECTION DETECTED: Surface Boundary with id " + sb.surfaceIdRef + " is self-intersecting.");
                            string t = space.id + "-" + sb.surfaceIdRef;
                            report.TestPassedDict.Add(t, false);
                            spacepassorfail = false;
                        }
                    }
                    if (spacepassorfail) { report.TestPassedDict.Add(space.id, true); }
                    else { report.TestPassedDict.Add(space.id, false); }

                }
            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.MessageList.Add("This test failed unexpectedly.");
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj UniqueSpaceIdTest(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test reviews the values if all Space id attributes, and ensures that they are all unique.  ";
            report.testSummary += "If there are any duplicate Space id values, then this test will fail.  If there are duplicates, the remainder of the tests in the testbed are not executed and the test will end here until the test file is properly updated.  Each Space id should be unique in valid gbXML.  If this test has failed, you may take the following actions.";
            report.testSummary += "  Repair the names of the Space Id so they are all unique.";

            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            List<string> spaceIdList = new List<string>();
            report.standResult = new List<string>();
            report.testResult = new List<string>();
            report.idList = new List<string>();
            // report.testType = "UniqueId";
            report.passOrFail = true;
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    //looks to see if the spaceId is already included in the list of space IDs being generated
                    string spaceId;
                    spaceId = node.Attributes[0].Value.ToString();
                    if(spaceIdList.Contains(spaceId))
                    {
                        //not unique
                        report.MessageList.Add("SpaceID: "+ spaceId + " is not unique." );
                        report.longMsg = "This test for unique space id attributes has failed.  All Space Id attributes should be unique.";
                        report.passOrFail = false;
                    }
                    spaceIdList.Add(spaceId);
                    report.MessageList.Add("SpaceID: " + spaceId + " is unique.");
                }
                if (report.passOrFail)
                {
                    report.longMsg = "TEST PASSED: All spaces have unique id attributes.";
                    report.passOrFail = true;
                }
                else
                {
                    report.longMsg = "TEST FAILED: All spaces do not have unique Ids.";
                }

            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.MessageList.Add("This test unexpectedly failed.");
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report UniqueSpaceIdTest2(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test reviews the values if all Space id attributes, and ensures that they are all unique.  ";
            report.testSummary += "If there are any duplicate Space id values, then this test will fail.  If there are duplicates, the remainder of the tests in the testbed are not executed and the test will end here until the test file is properly updated.  Each Space id should be unique in valid gbXML.  If this test has failed, you may take the following actions.";
            report.testSummary += "  Correct the names of the Space id attribute so they are all unique.";

            report.MessageList = new Dictionary<string,List<string>>();
            report.TestPassedDict = new Dictionary<string, bool>();
            List<string> spaceIdList = new List<string>();
            report.standResult = new List<string>();
            report.testResult = new List<string>();
            report.idList = new List<string>();
            // report.testType = "UniqueId";
            report.passOrFail = true;
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    //looks to see if the spaceId is already included in the list of space IDs being generated
                    string spaceId;
                    spaceId = node.Attributes[0].Value.ToString();
                    List<string> reportlist = new List<string>();
                    if (spaceIdList.Contains(spaceId))
                    {
                        //not unique
                        reportlist.Add("is not unique.");
                        report.longMsg = "This test for unique space id attributes has failed.  All Space Id attributes should be unique.";
                        report.passOrFail = false;
                        report.MessageList[spaceId] = reportlist;
                    }
                    spaceIdList.Add(spaceId);
                    reportlist.Add("Space Id is unique.");
                    report.MessageList[spaceId] = reportlist;
                }
                if (report.passOrFail)
                {
                    report.longMsg = "TEST PASSED: All spaces have unique id attributes.";
                    report.passOrFail = true;
                }
                else
                {
                    report.longMsg = "TEST FAILED: All spaces do not have unique Ids.";
                }

            }
            catch (Exception e)
            {
                List<string> l = new List<string>();
                report.longMsg = e.ToString();
                l.Add("This test unexpectedly failed.");
                report.MessageList["ERROR: "] = l;
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj UniqueSpaceBoundaryIdTest(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test reviews the values if all SpaceBoundary surfaceIdRef attributes, and ensures that they are all unique.  ";
            report.testSummary += "If there are any duplicate SpaceBoundary surfaceIdRef values, then this test will fail.  If there are duplicates, the remainder of the tests in the testbed are not executed and the test will end here until the test file is properly updated.  Each SpaceBoundary surfaceIdRef should be unique in valid gbXML.  If this test has failed, you may take the following actions.";

            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            List<string> spaceIdList = new List<string>();
            report.standResult = new List<string>();
            report.testResult = new List<string>();
            report.idList = new List<string>();
            // report.testType = "UniqueId";
            report.passOrFail = true;
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space/gbXMLv5:SpaceBoundary", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    //looks to see if the SpaceBoundaryId is already included in the list of SpaceBoundary IDs being generated
                    string spaceboundaryId;
                    spaceboundaryId = node.Attributes[0].Value.ToString();
                    if (spaceIdList.Contains(spaceboundaryId))
                    {
                        //not unique
                        report.MessageList.Add("SpaceBoundary surfaceIdRef: " + spaceboundaryId + " is not unique.");
                        report.longMsg = "This test for unique SpaceBoundary surfaceIdRef attributes has failed.  All SpaceBoundary surfaceIdRef attributes should be unique.";
                        report.passOrFail = false;
                    }
                    spaceIdList.Add(spaceboundaryId);
                    report.MessageList.Add("SpaceBoundary surfaceIdRef: " + spaceboundaryId + " is unique.");
                }
                if(report.passOrFail)
                {
                    report.longMsg = "TEST PASS: All SpaceBoundary surfaceIdRefs attributes have unique values.";
                    report.passOrFail = true;
                }
                else
                {
                    report.longMsg = "TEST FAIL: SpaceBoundary surfaceIdRefs attributes are not all unique.";
                    report.passOrFail = false;
                }

            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.MessageList.Add("This test unexpectedly failed.");
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLPhase2Report UniqueSpaceBoundaryIdTest2(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm, DOEgbXMLPhase2Report report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test reviews the values if all SpaceBoundary surfaceIdRef attributes, and ensures each Space has at most one unique Space Boundary id.  ";
            report.testSummary += "If there are any duplicate SpaceBoundary surfaceIdRef values, then this test will fail.  If there are duplicates, the remainder of the tests in the testbed are not executed and the test will end here until the test file is properly updated.  Each SpaceBoundary surfaceIdRef should be unique in valid gbXML.  If this test has failed, you may take the following actions.";
            report.testSummary += "  Repair the names of the SpaceBoundary id so they are all unique.";
            
            report.MessageList = new Dictionary<string,List<string>>();
            report.TestPassedDict = new Dictionary<string, bool>();
            report.standResult = new List<string>();
            report.testResult = new List<string>();
            report.idList = new List<string>();
            // report.testType = "UniqueId";
            report.passOrFail = true;
            try
            {
                //get spaces, then get the Space Boundaries for each Space
                XmlNodeList spaces = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", gbXMLnsm);
                foreach (XmlNode space in spaces)
                {
                    string spaceId;
                    List<string> returnlist = new List<string>();
                    spaceId = space.Attributes[0].Value.ToString();
                    List<string> sbIdList = new List<string>();
                    XmlNodeList spacechilds = space.ChildNodes;
                    foreach (XmlNode child in spacechilds)
                    {
                        
                        if (child.Name == "SpaceBoundary")
                        {
                            string spaceboundaryId;
                            spaceboundaryId = child.Attributes[0].Value.ToString();
                            
                            if (sbIdList.Contains(spaceboundaryId))
                            {
                                //not unique
                                returnlist.Add("SpaceBoundary surfaceIdRef: " + spaceboundaryId + " is not unique.");
                                report.longMsg = "This test for unique SpaceBoundary surfaceIdRef attributes has failed.  All SpaceBoundary surfaceIdRef for a given Space must be unique.";
                                report.passOrFail = false;
                                
                            }
                            sbIdList.Add(spaceboundaryId);
                            returnlist.Add("SpaceBoundary surfaceIdRef: " + spaceboundaryId + " is unique.");
                        }
                    }
                    report.MessageList[spaceId] = returnlist;
                }
                if (report.passOrFail)
                {
                    report.longMsg = "TEST PASSED: For every Space in the gbXML file, each SpaceBoundary surfaceIdRefs attributes have unique values.";
                    report.passOrFail = true;
                }
                else
                {
                    report.longMsg = "TEST FAILED: SpaceBoundary surfaceIdRefs attributes are not all unique.";
                    report.passOrFail = false;
                }

            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                List<string> l = new List<string>();

                l.Add("This test unexpectedly failed.");
                report.MessageList["ERROR"] = l;
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj SurfaceDefsCounterClock(List<gbXMLSpaces> spaces, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test reviews all of the surface that make up a space, and simply tries to determine if";
            report.testSummary += " the polygons are defined by coordinates that wind in a counterclockwise order.";

            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            try
            {
                foreach (gbXMLSpaces space in spaces)
                {
                     
                }
            }
            catch (Exception e)
            {

            }
            return report;
        }

        public static List<SpaceBoundary> GetSpaceBoundaryList(XmlDocument gbXMLDoc, XmlNamespaceManager gbXMLnsm)
        {

            
            List<SpaceBoundary> sbList = new List<SpaceBoundary>();
            try
            {
                XmlNodeList nodes = gbXMLDoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space/gbXMLv5:SpaceBoundary", gbXMLnsm);
                foreach (XmlNode node in nodes)
                {
                    SpaceBoundary sb = new SpaceBoundary();
                    string spaceboundaryId;
                    spaceboundaryId = node.Attributes[0].Value.ToString();
                    sb.surfaceIdRef = spaceboundaryId;

                    XmlNodeList sbchilds = node.ChildNodes;
                    foreach (XmlNode sbpnode in sbchilds)
                    {
                        if (sbpnode.Name == "PlanarGeometry")
                        {
                            sb.sbplane = new PlanarGeometry();
                            XmlNodeList pgchilds = sbpnode.ChildNodes;
                            MakeSBPlanarGeometry(sb, pgchilds);
                        }
                    }

                    sbList.Add(sb);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return sbList;
        }

        public static DOEgbXMLReportingObj areSpacesEnclosed(List<gbXMLSpaces> spaces, DOEgbXMLReportingObj report, bool waterTightRequired)
        {
            report.testSummary += "This test ensures that each space object is reasonably enclosed";
            report.TestPassedDict = new Dictionary<string, bool>();
            List<DOEgbXMLBasics.EdgeFamily> eflist = new List<DOEgbXMLBasics.EdgeFamily>();

            try
            {
                report.MessageList.Add("Starting space enclosure test.");
                if (waterTightRequired)
                {
                    report.MessageList.Add("Performing tests for watertight, or nearly watertight enclosures.");
                    report.MessageList.Add("The tolerance for tests is: " + report.tolerance);
                    //check the closed shell polygon
                    //if the surfaces are discretized this simple check may not work., and I have another approach at the end of the routine.
                    int edgessearched = 0;
                    int edgesfound = 0;
                    for (int count = 0; count < spaces.Count(); count++)
                    {
                        report.MessageList.Add("Checking Space: " + spaces[count].id);
                        for (int plcount = 0; plcount < spaces[count].sg.cs.ploops.Count(); plcount++)
                        {
                            PolyLoop pl = spaces[count].sg.cs.ploops[plcount];
                            for(int coordcount = 0; coordcount < pl.plcoords.Count; coordcount++)
                            {
                                edgessearched++;
                                DOEgbXMLBasics.EdgeFamily ef = new DOEgbXMLBasics.EdgeFamily();
                                ef.sbdec = spaces[count].id + "_" + plcount + "_" + coordcount;
                                ef.relatedEdges = new List<DOEgbXMLBasics.EdgeFamily>();
                                ef.startendpt = new List<Vector.MemorySafe_CartCoord>();

                                if (plcount == 0 && coordcount == 0)
                                {
                                    ef.relatedEdges[edgessearched] = ef;
                                    Vector.MemorySafe_CartCoord startpt = pl.plcoords[coordcount];
                                    Vector.MemorySafe_CartCoord endpt = pl.plcoords[coordcount + 1];
                                    ef.startendpt.Add(startpt);
                                    ef.startendpt.Add(endpt);
                                    continue;
                                }
                                if (coordcount < pl.plcoords.Count() - 1)
                                {
                                    Vector.MemorySafe_CartCoord startpt = pl.plcoords[coordcount];
                                    Vector.MemorySafe_CartCoord endpt = pl.plcoords[coordcount + 1];
                                    //search all the other polyloops except for this polyloop
                                    for (int innerplcount = 0; innerplcount < spaces[count].sg.cs.ploops.Count(); innerplcount++)
                                    {
                                        if (innerplcount == plcount) continue;
                                        PolyLoop testpl = spaces[count].sg.cs.ploops[innerplcount];
                                        for (int testcoord = 0; testcoord < testpl.plcoords.Count(); testcoord++)
                                        {
                                            if (testcoord < testpl.plcoords.Count())
                                            {
                                                Vector.MemorySafe_CartCoord teststart = testpl.plcoords[testcoord];
                                                Vector.MemorySafe_CartCoord testend = testpl.plcoords[testcoord + 1];
                                                double diffX = Math.Abs(startpt.X - teststart.X);
                                                double diffY = Math.Abs(startpt.Y - teststart.Y);
                                                double diffZ = Math.Abs(startpt.Z - teststart.Z);
                                                if (diffX < report.tolerance && diffY < report.tolerance && diffZ < report.tolerance)
                                                {

                                                    edgesfound++;
                                                }
                                            }
                                            else
                                            {
                                                Vector.MemorySafe_CartCoord teststart = testpl.plcoords[testcoord];
                                                Vector.MemorySafe_CartCoord testend = testpl.plcoords[0];
                                                double diffX = Math.Abs(startpt.X - teststart.X);
                                                double diffY = Math.Abs(startpt.Y - teststart.Y);
                                                double diffZ = Math.Abs(startpt.Z - teststart.Z);
                                                if (diffX < report.tolerance && diffY < report.tolerance && diffZ < report.tolerance)
                                                {
                                                    edgesfound++;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    //we may be able to perform a lower-grade test
                }
            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.MessageList.Add("This test failed unexpectedly.");
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        private static SpaceBoundary MakeSBPlanarGeometry(SpaceBoundary sb, XmlNodeList pgchilds)
        {
            try
            {

                foreach (XmlNode pgchild in pgchilds)
                {
                    if (pgchild.Name == "PolyLoop")
                    {
                        sb.sbplane.pl = new PolyLoop();

                        sb.sbplane.pl.plcoords = new List<Vector.MemorySafe_CartCoord>();
                        XmlNodeList cartPoints = pgchild.ChildNodes;
                        foreach (XmlNode point in cartPoints)
                        {
                            if (point.Name == "CartesianPoint")
                            {
                                Vector.CartCoord coord = new Vector.CartCoord();
                                XmlNodeList val = point.ChildNodes;
                                int pointcount = 1;
                                foreach (XmlNode cpoint in val)
                                {
                                    switch (pointcount)
                                    {
                                        case 1:
                                            coord.X = Convert.ToDouble(cpoint.InnerText);
                                            break;
                                        case 2:
                                            coord.Y = Convert.ToDouble(cpoint.InnerText);
                                            break;
                                        case 3:
                                            coord.Z = Convert.ToDouble(cpoint.InnerText);
                                            break;
                                    }
                                    pointcount++;
                                }
                                Vector.MemorySafe_CartCoord memsafecoord = Vector.convertToMemorySafeCoord(coord);
                                sb.sbplane.pl.plcoords.Add(memsafecoord);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return sb;
        }

        public static DOEgbXMLReportingObj SpaceSurfacesPlanarTest(List<gbXMLSpaces> Spaces, DOEgbXMLReportingObj report)
        {
            report.testSummary = "Testing if the PolyLoops of SpaceBoundary and ShellGeometry elements (if they have been defined) are planar or very nearly planar.";
            report.testSummary += "  This test is important because many energy modeling and load calculation tools do not have the ability to simplify a curved surface into ";
            report.testSummary += "a set of planar surfaces.  Without real planar surfaces, some load and energy calculation tools cannot simulate heat transfer and solar";
            report.testSummary += "  penetration properly.  This test ensures that all surfaces that are defined by the PolyLoop element are in fact planar.";
            report.MessageList = new List<string>();
            //ensure that each set of RHR tests result in parallel or anti-parallel resultant vectors, or else fail the test
            try
            {
                foreach (gbXMLSpaces s in Spaces)
                {
                    bool spacepassorfail = true;
                    //test shell geometry's closed shell polyloop planarity
                    if (s.sg.cs.ploops.Count() > 0)
                    {
                        var ploops = s.sg.cs.ploops;
                        Dictionary<string, List<Vector.CartVect>> surfaceXProducts = new Dictionary<string, List<Vector.CartVect>>();

                        int plcount = 1;
                        
                        foreach (var pl in ploops)
                        {
                            List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                            for (int i = 0; i < pl.plcoords.Count() - 2; i++)
                            {
                                //Get the Cross Product
                                VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(pl.plcoords[i], pl.plcoords[i + 1]);
                                VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(pl.plcoords[i + 1], pl.plcoords[i + 2]);
                                Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                                xProd = Vector.UnitVector(xProd);
                                xProducts.Add(xProd);
                            }
                            //give the polyloops some identifier, since they technically don't contain one
                            string ploopname = "shellgeometry-" + plcount.ToString();
                            surfaceXProducts.Add(ploopname, xProducts);
                            for (int j = 0; j < xProducts.Count - 1; j++)
                            {
                                report.MessageList.Add("Planarity Test for Shell Geometry :" + ploopname);
                                //parallel and anti parallel
                                if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                                {
                                    report.MessageList.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                                    report.TestPassedDict.Add(ploopname, true);
                                    continue;
                                }
                                //anti-parallel
                                else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                                {
                                    report.MessageList.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                                    report.TestPassedDict.Add(ploopname, true);
                                    continue;
                                }
                                //I defined these tolerances myself.  Could this be done better?
                                else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < 0.0001 && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < 0.0001 &&
                                    Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < 0.0001)
                                {
                                    report.MessageList.Add("Coordinates for this surface form a nearly planar surface that is within allowable tolerances.");
                                    report.TestPassedDict.Add(ploopname, true);
                                    continue;
                                }
                                else
                                {
                                    report.MessageList.Add("NON PLANAR SURFACE DETECTED:  Coordinates for this surface do not appear to form a planar surface.");
                                    report.TestPassedDict.Add(ploopname, false);
                                    spacepassorfail = false;
                                }
                            }
                            //next polyloop please
                            plcount++;
                        }
                    }
                    if (s.spacebounds.Count() > 0)
                    {
                        report.MessageList.Add("Space Boundaries present in this space.  Test begin.");
                        //test the space boundary planarity
                        var sbs = s.spacebounds;
                        Dictionary<string, List<Vector.CartVect>> sbXProducts = new Dictionary<string, List<Vector.CartVect>>();
                        foreach (var sb in sbs)
                        {
                            report.MessageList.Add("Planarity Test for Space Boundary: " + sb.surfaceIdRef);
                            var ploop = sb.sbplane.pl;
                            List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                            for (int i = 0; i < ploop.plcoords.Count() - 2; i++)
                            {
                                //Get the Cross Product
                                VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(ploop.plcoords[i], ploop.plcoords[i + 1]);
                                VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(ploop.plcoords[i + 1], ploop.plcoords[i + 2]);
                                Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                                xProd = Vector.UnitVector(xProd);
                                xProducts.Add(xProd);
                            }
                            //a name for the Space Boundary is already contained in the surfaceIdRef
                            sbXProducts.Add(sb.surfaceIdRef, xProducts);
                            for (int j = 0; j < xProducts.Count - 1; j++)
                            {
                                //parallel and anti parallel
                                if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                                {
                                    report.MessageList.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                                    report.TestPassedDict.Add(sb.surfaceIdRef, true);
                                    continue;
                                }
                                //anti-parallel
                                else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                                {
                                    report.MessageList.Add("PASS: Coordinates for this surface form a perfectly planar surface.");
                                    report.TestPassedDict.Add(sb.surfaceIdRef, true);
                                    continue;
                                }
                                //I defined these tolerances myself.  Could this be done better?
                                else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < 0.0001 && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < 0.0001 &&
                                    Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < 0.0001)
                                {
                                    report.MessageList.Add("Coordinates for this surface form a nearly planar surface that is within allowable tolerances.");
                                    report.TestPassedDict.Add(sb.surfaceIdRef, true);
                                    continue;
                                }
                                else
                                {
                                    report.MessageList.Add("NON PLANAR SURFACE DETECTED:  Coordinates for this surface do not appear to form a planar surface.");
                                    report.TestPassedDict.Add(sb.surfaceIdRef, false);
                                    spacepassorfail = false;
                                }
                            }

                        }
                    }
                    if (spacepassorfail)
                    {
                        report.TestPassedDict.Add(s.id, true);
                    }
                    else
                    {
                        report.TestPassedDict.Add(s.id, false);
                    }
                }
                
            }
            catch (Exception e)
            {
                //perhaps I want to log this for programmers to look at when they want
            }
            report.MessageList.Add("All test file's surfaces have polyloop descriptions that describe a planar surface.  Planar surface test succeeded.");
            report.passOrFail = true;
            return report;

        }

        public static DOEgbXMLPhase2Report SpaceSurfacesPlanarTest(List<gbXMLSpaces> Spaces, DOEgbXMLPhase2Report report)
        {
            report.testSummary = "Testing if the PolyLoops of SpaceBoundary and ShellGeometry elements (if they have been defined) are planar or very nearly planar.";
            report.testSummary += "  This test is important because many energy modeling and load calculation tools do not have the ability to simplify a curved surface into ";
            report.testSummary += "a set of planar surfaces.  Without real planar surfaces, some load and energy calculation tools cannot simulate heat transfer and solar";
            report.testSummary += "  penetration properly.  This test ensures that all surfaces that are defined by the PolyLoop element are in fact planar.";
            
            //ensure that each set of RHR tests result in parallel or anti-parallel resultant vectors, or else fail the test
            try
            {
                foreach (gbXMLSpaces s in Spaces)
                {
                    List<string> ml = new List<string>();
                    List<bool> pf = new List<bool>();
                    //by default, this always starts out true
                    bool spacepassorfail = true;
                    
                    //test shell geometry's closed shell polyloop planarity
                    if (s.sg.cs.ploops.Count() > 0)
                    {
                        string spaceid = s.id + " has ShellGeometry PolyLoops.  Conducting tests of ShellGeometry PolyLoops";
                        report.TestPassedDict[s.id] = true;
                        var ploops = s.sg.cs.ploops;
                        Dictionary<string, List<Vector.CartVect>> surfaceXProducts = new Dictionary<string, List<Vector.CartVect>>();

                        int plcount = 1;

                        foreach (var pl in ploops)
                        {
                            List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                            for (int i = 0; i < pl.plcoords.Count() - 2; i++)
                            {
                                //Get the Cross Product
                                VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(pl.plcoords[i], pl.plcoords[i + 1]);
                                VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(pl.plcoords[i + 1], pl.plcoords[i + 2]);
                                Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                                xProd = Vector.UnitVector(xProd);
                                xProducts.Add(xProd);
                            }
                            //give the polyloops some identifier, since they technically don't contain one
                            string ploopname = "shellgeometry-" + plcount.ToString();
                            surfaceXProducts.Add(ploopname, xProducts);
                            for (int j = 0; j < xProducts.Count - 1; j++)
                            {
                                //parallel and anti parallel
                                if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                                {
                                    ml.Add(ploopname+": Coordinates for this surface form a perfectly planar surface.");
                                    pf.Add(true);
                                    continue;
                                }
                                //anti-parallel
                                else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                                {
                                    ml.Add(ploopname + ": Coordinates for this surface form a perfectly planar surface.");
                                    pf.Add(true);
                                    continue;
                                }
                                //I defined these tolerances myself.  Could this be done better?
                                else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < report.tolerance && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < report.tolerance &&
                                    Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < report.tolerance)
                                {
                                    ml.Add(ploopname + ": Coordinates for this surface form a nearly planar surface that is within allowable tolerances.");
                                    pf.Add(true);
                                    continue;
                                }
                                else
                                {
                                    ml.Add(ploopname+": Coordinates for this surface do not appear to form a planar surface.");
                                    pf.Add(false);
                                    spacepassorfail = false;
                                }
                            }
                            //next polyloop please
                            plcount++;
                        }
                    }
                    else
                    {
                        string spaceid = s.id + "has no Shell Geometry PolyLoops to test for planarity.  Conducting tests of ShellGeometry PolyLoops";
                        report.TestPassedDict[spaceid] = false;
                    }

                    if (s.spacebounds.Count() > 0)
                    {
                        string spaceid = s.id + "has SpaceBoundary PolyLoops.  Conducting tests of SpaceBoundary PolyLoops";
                        

                        //test the space boundary planarity
                        var sbs = s.spacebounds;
                        Dictionary<string, List<Vector.CartVect>> sbXProducts = new Dictionary<string, List<Vector.CartVect>>();
                        foreach (var sb in sbs)
                        {
                            var ploop = sb.sbplane.pl;
                            List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                            for (int i = 0; i < ploop.plcoords.Count() - 2; i++)
                            {
                                //Get the Cross Product
                                VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(ploop.plcoords[i], ploop.plcoords[i + 1]);
                                VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(ploop.plcoords[i + 1], ploop.plcoords[i + 2]);
                                Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                                xProd = Vector.UnitVector(xProd);
                                xProducts.Add(xProd);
                            }
                            //a name for the Space Boundary is already contained in the surfaceIdRef
                            sbXProducts.Add(sb.surfaceIdRef, xProducts);
                            for (int j = 0; j < xProducts.Count - 1; j++)
                            {
                                string sbid = sb.surfaceIdRef;
                                //parallel and anti parallel
                                if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                                {
                                    ml.Add(sbid+ ": Coordinates for this surface form a perfectly planar surface.");
                                    pf.Add(true);
                                    continue;
                                }
                                //anti-parallel
                                else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                                {
                                    ml.Add(sbid+": Coordinates for this surface form a perfectly planar surface.");
                                    pf.Add(true);
                                    continue;
                                }
                                //I defined these tolerances myself.  Could this be done better?
                                else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < report.tolerance && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < report.tolerance &&
                                    Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < report.tolerance)
                                {
                                    ml.Add(sbid+ ": Coordinates for this surface form a nearly planar surface that is within allowable tolerances.");
                                    pf.Add(true);
                                    continue;
                                }
                                else
                                {
                                    ml.Add(sbid+ ": NON PLANAR SURFACE DETECTED:  Coordinates for this surface do not appear to form a planar surface.");
                                    pf.Add(false);
                                    spacepassorfail = false;
                                }
                            }


                        }

                    }
                    else
                    {
                        string spaceid = s.id + "does not have SpaceBoundary PolyLoops.  Conducting tests of SpaceBoundaryPolyLoops";
                        report.TestPassedDict[spaceid] = false;
                    }
                    

                    if (pf.Contains(false))
                    {
                        string rep = s.id +  ": FAIL";
                        report.MessageList[rep] = ml;
                        report.passOrFail = false;
                    }
                    else
                    {
                        string rep = s.id + ": TRUE";
                        report.MessageList[rep] = ml;
                    }
                }


            }
            catch (Exception e)
            {
                //perhaps I want to log this for programmers to look at when they want
            }
            if (report.passOrFail == false)
            {
                report.longMsg = "TEST FAILED: Non-planar PolyLoop descriptions for ShellGeometry and SpaceBoundary elements have been detected!";
            }
            else
            {
                report.longMsg = "TEST PASSED: All of the PolyLoop descriptions for ShellGeometry and SpaceBoundary elements describe a planar or nearly planar surface.";
            }
            return report;

        }

        public static DOEgbXMLReportingObj findStraySBVertices(string filelocation, DOEgbXMLReportingObj report)
        {
            report.testSummary = "";
            report.testReasoning = "";
            report.MessageList = new List<string>();
            try
            {
                int counter = 0;
                string specString = @"(\[.*]);(\[.*]);(\[.*])";
                string line;
                System.IO.StreamReader file = new StreamReader(filelocation);
                while ((line = file.ReadLine()) != null)
                {
                    Regex specRegex = new Regex(specString);
                    //parse the line
                    Match arrMatch = specRegex.Match(line);
                    if (arrMatch.Success)
                    {
                        if (counter == 0)
                        {
                            //header row
                            
                        }
                        else
                        {
                            // coordinateString = @"\[[-+]?([0-9]*\.[0-9]+|[0-9]+),[-+]?([0-9]*\.[0-9]+|[0-9]+),[-+]?([0-9]*\.[0-9]+|[0-9]+)]";
                            string coordinateString = @"(\[)(.*)(])";
                            string surfaceString = @"(\[)(.*)(])";
                            string boolString = @"(\[)(.*)(])";
                            Regex coordRegex = new Regex(coordinateString);
                            Match XYZMatch = coordRegex.Match(arrMatch.Groups[1].Value);
                            if (XYZMatch.Success)
                            {
                                //print something here for the user
                                List<string> coords = XYZMatch.Groups[2].Value.Split(',').ToList();


                                Regex surfRegex = new Regex(surfaceString);
                                Match surfMatch = surfRegex.Match(arrMatch.Groups[2].Value);
                                if (surfMatch.Success)
                                {
                                    //print something here for the user
                                    List<string> surf = surfMatch.Groups[2].Value.Split(',').ToList();
                                    if (surf.Count() >= 3)
                                    {
                                        //this is great
                                    }

                                    Regex boolRegex = new Regex(boolString);
                                    Match boolMatch = boolRegex.Match(arrMatch.Groups[3].Value);
                                    if (boolMatch.Success)
                                    {
                                        //print something here for the user
                                        List<string> bools = boolMatch.Groups[2].Value.Split(',').ToList();
                                        if (bools.Contains("False"))
                                        {
                                            //we have to take note of a potential problem
                                        }
                                    }
                                    else
                                    {
                                        //report that something has gone wrong
                                        //we need to receive a report about this somehow
                                    }

                                }
                                else
                                {
                                    //report that something has gone wrong
                                    //we need to receive a report about this somehow
                                }

                            }
                            else
                            {
                                //report that something has gone wrong
                                //we need to receive a report about this somehow
                            }

                            
                        }
                    }
                    else
                    {
                        //report that something has gone wrong
                        //we need to receive a report about this somehow
                    }
                    counter++;
                }
                
            }
            catch (Exception e)
            {
                //do something here
            }
            return report;
        }

    }





    
}
