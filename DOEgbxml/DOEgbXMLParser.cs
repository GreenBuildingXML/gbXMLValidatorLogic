using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using VectorMath;
using System.Web;

namespace DOEgbXML
{
    class OpeningDefinitions
    {
        //creates instances of an object that store information about surfaces in a gbXML file
        public string OpeningType;
        public string OpeningId;
        public string ParentSurfaceId;
        public double ParentAzimuth;
        public double ParentTilt;
        public double Azimuth;
        public double Tilt;
        public double Height;
        public double Width;
        public double surfaceArea;
        public Vector.CartCoord InsertionPoint;
        public List<Vector.MemorySafe_CartCoord> PlCoords;
        public Vector.MemorySafe_CartVect PlRHRVector;
    }

    public class XMLParser
    {

        //this is the output string
        public string output;
        public string log;
        public string table;
        bool overallPassTest = true;
        DOEgbXMLTestCriteriaObject TestCriteria;
        DOEgbXMLTestDetail TestDetail;
        gbXMLMatches globalMatchObject;
        string TestToRun;
        XmlDocument gbXMLStandardFile;
        XmlDocument gbXMLTestFile;
        public string summaryTable;
        public List<DOEgbXMLReportingObj> ReportList;

        //a List of strings with all the test files that I want to test
        //eventually this will be a dynamically created list based on what has been uploaded.
        //this list should be the list of all the test

        static Dictionary<string, string> filepaths = new Dictionary<string, string>()
        {
            //WX 0420-2020 made change for testing
            { "test1", "../../tests/test1.gbxml" }
        };


        //Shell Geometry RHR Dictionaries
        static Dictionary<string, Dictionary<double, VectorMath.Vector.CartVect>> TFShellGeomRHRes = new Dictionary<string, Dictionary<double, VectorMath.Vector.CartVect>>();
        static Dictionary<string, Dictionary<double, VectorMath.Vector.CartVect>> SFShellGeomRHRes = new Dictionary<string, Dictionary<double, VectorMath.Vector.CartVect>>();

        //minimum number of points to define a plane
        static int minPlanePoints = 3;
        //value to hold, starting arbitrarily large
        static double StoryHeightMin = 100.0;

        #region Set Up
        //upload a file via asp.net
        //create a namespace
        //determine the type of file/units
        public int LoadFile(XmlReader xr, XmlDocument doc)
        {
            try
            {
                doc.Load(xr);
                return 0;
            }
            catch (Exception e)
            {
                throw;
            }
            
        }

        public XmlNamespaceManager getnsmanager(XmlDocument xmldoc)
        {
            try
            {
                XmlNamespaceManager nsm = new XmlNamespaceManager(xmldoc.NameTable);
                nsm.AddNamespace("gbXMLv5", "http://www.gbxml.org/schema");
                return nsm;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public int getunits(XmlNamespaceManager xmlns, XmlDocument xmldoc)
        {
            bool isMetric = false;
            XmlNodeList nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML", xmlns);
            foreach (XmlNode Node in nodes)
            {
                XmlAttributeCollection spaceAtts = Node.Attributes;
                foreach (XmlAttribute at in spaceAtts)
                {
                    if (at.Name == "volumeUnit")
                    {
                        string type = at.Value;
                        if (type == "CubicMeters")
                        {
                            isMetric = true;
                        }
                        else { isMetric = false; }
                    }
                    else if (at.Name == "areaUnit")
                    {
                        string type = at.Value;
                        if (type == "CubicMeters")
                        {
                            isMetric = true;
                        }
                        else { isMetric = false; }
                    }
                    else if (at.Name == "lengthUnit")
                    {
                        string type = at.Value;
                        if (type == "Meters")
                        {
                            isMetric = true;
                        }
                        else { isMetric = false; }
                    }
                }
            }
            //maybe use enumeration here
            if (isMetric) return 1;
            else return 0;
        }


        public void StartTest(XmlReader xmldoc, string testToRun, string username)
        {
            TestToRun = testToRun;
            globalMatchObject = new gbXMLMatches();
            globalMatchObject.Init();

            //first create a list of lists that is indexed identically to the drop down list the user selects
            TestDetail = new DOEgbXMLTestDetail();
            //then populate the list of lists.  All indexing is done "by hand" in InitializeTestResultStrings()
            TestDetail.InitializeTestResultStrings();

            //create report list reportlist will store all the test result
            ReportList = new List<DOEgbXMLReportingObj>();

            //Load an XML File for the test at hand
            gbXMLTestFile = new XmlDocument();
            gbXMLTestFile.Load(xmldoc);

            gbXMLStandardFile = new XmlDocument();
            //gbXMLStandardFile.Load(filepaths[testToRun]);
            //TestFileIsAvailable function will load the file.
            if (!TestFileIsAvailable())
                return;

            //Define the namespace
            XmlNamespaceManager gbXMLns1 = new XmlNamespaceManager(gbXMLTestFile.NameTable);
            gbXMLns1.AddNamespace("gbXMLv5", "http://www.gbxml.org/schema");
            XmlNamespaceManager gbXMLns2 = new XmlNamespaceManager(gbXMLStandardFile.NameTable);
            gbXMLns2.AddNamespace("gbXMLv5", "http://www.gbxml.org/schema");

            //is the test file metric or not?
            bool isMetric = false;
            XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML", gbXMLns1);
            foreach (XmlNode Node in nodes)
            {
                XmlAttributeCollection spaceAtts = Node.Attributes;
                foreach (XmlAttribute at in spaceAtts)
                {
                    if (at.Name == "volumeUnit")
                    {
                        string type = at.Value;
                        if (type == "CubicMeters")
                        {
                            isMetric = true;
                        }
                        else { isMetric = false; }
                    }
                    else if (at.Name == "areaUnit")
                    {
                        string type = at.Value;
                        if (type == "CubicMeters")
                        {
                            isMetric = true;
                        }
                        else { isMetric = false; }
                    }
                    else if (at.Name == "lengthUnit")
                    {
                        string type = at.Value;
                        if (type == "Meters")
                        {
                            isMetric = true;
                        }
                        else { isMetric = false; }
                    }
                }
            }

            //if isMetric is true, then we would like to convert the test file's numbers to US-IP units
            //TODORP-1810: The method is essentially returning an empty file - NOT IMPLEMENTED
            if(isMetric) gbXMLTestFile = ConvertMetricToUS(gbXMLTestFile);

            List<XmlDocument> gbXMLdocs = new List<XmlDocument>();
            gbXMLdocs.Add(gbXMLTestFile);
            gbXMLdocs.Add(gbXMLStandardFile);
            List<XmlNamespaceManager> gbXMLnsm = new List<XmlNamespaceManager>();
            gbXMLnsm.Add(gbXMLns1);
            gbXMLnsm.Add(gbXMLns2);

            //Create a Log file that logs the success or failure of each test.
            //Eventually maybe I want to create a little HTML factory
         
            output = "";
            log = "";
            table += "<div class='container'>" +
                    "<h3>" + "Test Sections" + "</h3>";
            table += "<table class='table table-bordered'>";
            table += "<tr class='info'>" +
                                   "<td>" + "Test Section Name" + "</td>" +
                                   "<td>" + "Standard Result" + "</td>" +
                                   "<td>" + "Test File Result" + "</td>" +
                                   "<td>" + "Tolerances" + "</td>" +
                                   "<td>" + "Pass/Fail" + "</td>" +
                                   "</tr>";

            string units;
            DOEgbXMLReportingObj report = new DOEgbXMLReportingObj();
            report.standResult = new List<string>();
            report.testResult = new List<string>();
            report.idList = new List<string>();
            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();

            //Set up the Global Pass/Fail criteria for the test case file
            TestCriteria = new DOEgbXMLTestCriteriaObject();
            TestCriteria.InitializeTestCriteriaWithTestName(testToRun);

            //Set up method global data for testing use
            List<SurfaceDefinitions> standardSurfaces = GetFileSurfaceDefs(gbXMLStandardFile, gbXMLns2);
            List<SurfaceDefinitions> testSurfaces = GetFileSurfaceDefs(gbXMLTestFile, gbXMLns1);
            List<DOEgbXMLConstruction> standardConstructions = GetConstructionDefs(gbXMLStandardFile, gbXMLns2);
            List<DOEgbXMLConstruction> testConstructions = GetConstructionDefs(gbXMLTestFile, gbXMLns1);

            //Test 2 execute
            report.tolerance = DOEgbXMLBasics.Tolerances.AreaTolerance;
            report.testType = TestType.Building_Area;
            units = DOEgbXMLBasics.MeasurementUnits.sqft.ToString();
            report = GetBuildingArea(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Building Area Test Passed: ", report, true);

            //Test 3 execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.SpaceCountTolerance;
            report.testType = TestType.Space_Count;
            units = DOEgbXMLBasics.MeasurementUnits.spaces.ToString();
            report = GetBuildingSpaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Building Space Quantity Count Test Passed: ", report, true);


            ////Test 4 execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.LevelCountTolerance;
            report.testType = TestType.Building_Story_Count;
            units = DOEgbXMLBasics.MeasurementUnits.levels.ToString();
            report = GetBuildingStoryCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Building Story Count Test Passed: ", report, true);


            //Test 5 execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.LevelHeightTolerance;
            report.testType = TestType.Building_Story_Z_Height;
            units = DOEgbXMLBasics.MeasurementUnits.ft.ToString();
            report = GetStoryHeights(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Building Story Z-Height Test: ", report, true);


            //Test 6 execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.VectorAngleTolerance;
            report.testType = TestType.Building_Story_PolyLoop_RHR;
            units = "degrees";
            report = TestBuildingStoryRHR(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Building Story PolyLoop Right Hand Rule Test Result:", report, true);


            //String spShellGeometrySurfaceNum = TestShellGeomSurfaceNum(gbXMLTestFile, gbXMLns);

            //Space Tests .............................................................
            //Test 7 execute
            //only needs to test the test file
            report.Clear();
            report.testType = TestType.SpaceId_Match_Test;
            report = UniqueSpaceIdTest(gbXMLdocs, gbXMLnsm, report);
            AddToOutPut("SpaceId Match Test: ", report, true);


            //Test 8 execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.SpaceAreaTolerance;
            report.testType = TestType.Space_Area;
            units = DOEgbXMLBasics.MeasurementUnits.sqft.ToString();
            report = TestSpaceAreas(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Space Areas Test: ", report, true);


            //Test 9 execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.VolumeTolerance;
            report.testType = TestType.Space_Volume;
            units = DOEgbXMLBasics.MeasurementUnits.cubicft.ToString();
            report = TestSpaceVolumes(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Space Volumes Test: ", report, true);


            //Test 10 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.VectorAngleTolerance;
            report.testType = TestType.Shell_Geom_RHR;
            units = "degrees";
            report = TestShellGeomPLRHR(gbXMLdocs, gbXMLnsm, report, units);
            //AddToOutPut("Shell Geometry RHR Test: ",report);

            //Surface Element tests
            //Test 11 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.SurfaceCountTolerance;
            report.testType = TestType.Total_Surface_Count;
            units = "";
            report = GetSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Surface Count Test Result: ", report, true);


            //Surface Element tests
            //Test 12 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.ExteriorWallCountTolerance;
            report.testType = TestType.Exterior_Wall_Surface_Count;
            units = "";
            report = GetEWSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Exterior Wall Surface Count Test Result: ", report, true);

            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.SurfaceCountTolerance;
            report.testType = TestType.Underground_Surface_Count;
            units = "";
            report = GetUGSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Underground Wall Count Test Result: ", report, true);

            //Surface Element tests
            //Test 13 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.InteriorWallCountTolerance;
            report.testType = TestType.Interior_Wall_Surface_Count;
            units = "";
            report = GetIWSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Interior Wall Surface Count Test Result: ", report, true);

            //Surface Element tests
            //Test 13 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.InteriorFloorCountTolerance;
            report.testType = TestType.Interior_Floor_Surface_Count;
            units = "";
            report = GetIFSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Interior Floor Surface Count Test Result: ", report, true);


            //Surface Element tests
            //Test 14 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.InteriorWallCountTolerance;
            report.testType = TestType.Roof_Surface_Count;
            units = "";
            report = GetRoofSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Roof Surface Count Test Result: ", report, true);


            //Surface Element tests
            //Test 15 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.InteriorWallCountTolerance;
            report.testType = TestType.Shading_Surface_Count;
            units = "";
            report = GetShadeSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Shading Surface Count Test Result: ", report, true);


            //Test 16 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.AirWallCountTolerance;
            report.testType = TestType.Air_Surface_Count;
            units = "";
            report = GetAirSurfaceCount(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Air Surface Count Test Result: ", report, true);


            #region surface detailed test
            //Jan 31-2012:  We may not want to perform these if the surface counts fail, but for now, we will include these tests
            //Detailed Surface Checks
            //Store Surface Element Information
            List<SurfaceDefinitions> TestSurfaces = new List<SurfaceDefinitions>();
            XmlDocument TestFile = gbXMLdocs[0];
            XmlNamespaceManager TestNSM = gbXMLnsm[0];
            List<SurfaceDefinitions> StandardSurfaces = new List<SurfaceDefinitions>();
            XmlDocument StandardFile = gbXMLdocs[1];
            XmlNamespaceManager StandardNSM = gbXMLnsm[1];
            TestSurfaces = GetFileSurfaceDefs(TestFile, TestNSM);
            StandardSurfaces = GetFileSurfaceDefs(StandardFile, StandardNSM);
            string TestSurfaceTable = " <div class='container'><table class='table table-bordered'>";
            TestSurfaceTable += "<tr class='info'>" +
                                   "<td>" + "Test Section Name" + "</td>" +
                                   "<td>" + "Stand Surface ID" + "</td>" +
                                   "<td>" + "Test Surface ID" + "</td>" +
                                   "<td>" + "Stand Surface Tilt" + "</td>" +
                                   "<td>" + "Test Surface Tilt" + "</td>" +
                                   "<td>" + "Stand Surface Azimuth" + "</td>" +
                                   "<td>" + "Test Surface Azimuth" + "</td>" +
                                    "<td>" + "Stand Surface Height" + "</td>" +
                                   "<td>" + "Test Surface Height" + "</td>" +
                                    "<td>" + "Stand Surface Width" + "</td>" +
                                   "<td>" + "Test Surface Width" + "</td>" +
                                   "<td>" + "Pass/Fail" + "</td>" +
                                   "</tr>";
            //Test Surfaces Planar Test
            //all polyloops must be such that the surface defined by the coordinates is planar
            report.Clear();
            report.testType = TestType.Surface_Planar_Test;
            report = TestSurfacePlanarTest(TestSurfaces, report);

            if (!report.passOrFail)
            {
                AddToOutPut("Test File Planar Surface Check: ", report, true);
                report.Clear();
            }
            //only run detailed surface checks if the surfaces are planar
            else
            {
                //<For each surface in the Standard File, try to find a match for this surface in the test file>
                //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
                //Execute Tests


                //  globalMatchObject.MatchedSurfaceIds = new Dictionary<string, List<string>>();
                int i = 1;

                foreach (SurfaceDefinitions surface in StandardSurfaces)
                {
                    report.Clear();
                    //multiple tolerances used
                    report.testType = TestType.Detailed_Surface_Checks;
                    report.subTestIndex = i;

                    //multiple units used, so ignore units
                    report = GetPossibleSurfaceMatches(surface, TestSurfaces, report);

                    AddToOutPut("Test 17 for Surface number " + i + " Result: ", report, false);

                    foreach (SurfaceDefinitions ts in TestSurfaces)
                    {
                        if (globalMatchObject.MatchedSurfaceIds.ContainsKey(surface.SurfaceId))
                        {
                            foreach (string id in globalMatchObject.MatchedSurfaceIds[surface.SurfaceId])
                            {
                                if (ts.SurfaceId == id)
                                {
                                    if (report.passOrFail)
                                        TestSurfaceTable += "<tr class='success'>" +
                                            "<td>" + "<a href='TestDetailPage.aspx?type=" + (int)report.testType + "&subtype=" + report.subTestIndex + "' target='_blank'>" +
                                            "Detailed Surface Checks " + report.subTestIndex + "</a>" + "</td>" +

                                            "<td>" + surface.SurfaceId + "</td>" +
                                            "<td>" + ts.SurfaceId + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", surface.Tilt) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", ts.Tilt) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", surface.Azimuth) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", ts.Azimuth) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", surface.Height) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", ts.Height) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", surface.Width) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", ts.Width) + "</td>" +
                                            "<td>" + "Pass" + "</td>" +
                                            "</tr>";
                                }

                            }

                        }
                    }
                    //if didn't find match means it failed the test
                    if (!report.passOrFail)
                        TestSurfaceTable += "<tr class='error'>" +
                                          "<td>" + "<a href='TestDetailPage.aspx?type=" + (int)report.testType + "&subtype=" + report.subTestIndex + "' target='_blank'>" +
                                          "Detailed Surface Checks " + report.subTestIndex + "</a>" + "</td>" +
                                          "<td>" + surface.SurfaceId + "</td>" +
                                          "<td>" + "---</td>" +
                                          "<td>" + String.Format("{0:#,0.00}", surface.Tilt) + "</td>" +
                                          "<td>" + "---</td>" +
                                          "<td>" + String.Format("{0:#,0.00}", surface.Azimuth) + "</td>" +
                                          "<td>" + "---</td>" +
                                          "<td>" + String.Format("{0:#,0.00}", surface.Height) + "</td>" +
                                          "<td>" + "---</td>" +
                                          "<td>" + String.Format("{0:#,0.00}", surface.Width) + "</td>" +
                                          "<td>" + "---</td>" +
                                          "<td>" + "Fail" + "</td>" +
                                          "</tr>";
                    i += 1;
                }

                TestSurfaceTable += "</table></div><br/>";
            }
            #endregion

            //test 18 CountFixedWindows
            //Test 18 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.FixedWindowCountTolerance;
            report.testType = TestType.Fixed_Windows_Count;
            units = "";
            report = CountFixedWindows(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Fixed Windows Count Test Result: ", report, true);

            //test 19 CountOperableWindows
            //Test 19 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.OperableWindowCountTolerance;
            report.testType = TestType.Operable_Windows_Count;
            units = "";
            report = CountOperableWindows(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Operable Windows Count Test Result: ", report, true);

            //test 20 CountFixedSkylights
            //Test 20 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.FixedSkylightCountTolerance;
            report.testType = TestType.Fixed_Skylight_Count;
            units = "";
            report = CountFixedSkylights(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Fixed Skylight Count Test Result: ", report, true);

            //test 21 CountOperableSkylights
            //Test 21 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.OperableSkylightCountTolerance;
            report.testType = TestType.Operable_Skylight_Count;
            units = "";
            report = CountOperableSkylights(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Operable Skylight Count Test Result: ", report, true);


            //test 22 CountSlidingDoors
            //Test 22 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.SlidingDoorCountTolerance;
            report.testType = TestType.Sliding_Doors_Count;
            units = "";
            report = CountSlidingDoors(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Sliding Doors Count Test Result: ", report, true);

            //test 23 CountNonSlidingDoors
            //Test 23 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.NonSlidingDoorCountTolerance;
            report.testType = TestType.Non_Sliding_Doors_Count;
            units = "";
            report = CountNonSlidingDoors(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Non Sliding Doors Count Test Result: ", report, true);

            //test 24 CountAirOpenings
            //Test 24 Execute
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.AirOpeningCountTolerance;
            report.testType = TestType.Air_Openings_Count;
            units = "";
            report = CountAirOpenings(gbXMLdocs, gbXMLnsm, report, units);
            AddToOutPut("Air Openings Count Test Result: ", report, true);

            /*
             * New test sets added for RP-1810 project
             */
            //test 25 Compare Exterior Wall Area
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.AreaTolerance;
            report.testType = TestType.Exterior_Wall_Area;
            units = DOEgbXMLBasics.MeasurementUnits.sqft.ToString();
            report = DOEgbXMLTestFunctions.TestSurfaceAreaByType(testSurfaces,
                standardSurfaces, report, units,"ExteriorWall");
            AddToOutPut("Exterior Wall Area Test Results: ", report, true);

            //test 26 Compare roof area
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.AreaTolerance;
            report.testType = TestType.Roof_Area;
            units = DOEgbXMLBasics.MeasurementUnits.sqft.ToString();
            report = DOEgbXMLTestFunctions.TestSurfaceAreaByType(testSurfaces,
                standardSurfaces, report, units, "Roof");
            AddToOutPut("Roof Area Test Results: ", report, true);

            //test 27 Compare SlabOnGrade area
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.AreaTolerance;
            report.testType = TestType.SlabOnGrade_Area;
            units = DOEgbXMLBasics.MeasurementUnits.sqft.ToString();
            report = DOEgbXMLTestFunctions.TestSurfaceAreaByType(testSurfaces,
                standardSurfaces, report, units, "SlabOnGrade");
            AddToOutPut("Slab on Grade Area Test Results: ", report, true);

            //test 28 Compare shading area
            report.Clear();
            report.tolerance = DOEgbXMLBasics.Tolerances.AreaTolerance;
            report.testType = TestType.Shade_Area;
            units = DOEgbXMLBasics.MeasurementUnits.sqft.ToString();
            report = DOEgbXMLTestFunctions.TestShadeSurfaceArea(testSurfaces, standardSurfaces, report, units);
            AddToOutPut("Shade Area Test Results: ", report, true);

            //test 29 material and assembly test
            report.Clear();
            //TODO RP-1810 Need to think about the tolerance for the material assembly.
            report.tolerance = DOEgbXMLBasics.Tolerances.dotproducttol;
            report.testType = TestType.Assembly_Test;
            report = DOEgbXMLTestFunctions.TestMaterialAssembly(testConstructions, standardConstructions, report, units);
            AddToOutPut("Assembly test results: ", report, true);

            #region opening detailed test
            //openings detailed tests
            List<OpeningDefinitions> TestOpenings = new List<OpeningDefinitions>();
            XmlDocument testFile = gbXMLdocs[0];
            XmlNamespaceManager testNSM = gbXMLnsm[0];
            List<OpeningDefinitions> StandardOpenings = new List<OpeningDefinitions>();
            XmlDocument standardFile = gbXMLdocs[1];
            XmlNamespaceManager standardNSM = gbXMLnsm[1];
            TestOpenings = GetFileOpeningDefs(TestFile, TestNSM);
            StandardOpenings = GetFileOpeningDefs(StandardFile, StandardNSM);

            string TestOpeningTable = "";
            report.Clear();
            report.testType = TestType.Opening_Planar_Test;
            report = TestOpeningPlanarTest(TestOpenings, report);

            if (!report.passOrFail)
            {
                AddToOutPut("Test File Planar Opening Check: ", report, true);
                report.Clear();
            }
            //only run detailed opening checks if the opening are planar
            else
            {
                TestOpeningTable = "<div class='container'><table class='table table-bordered'>";
                TestOpeningTable += "<tr class='info'>" +
                                       "<td>" + "Test Section Name" + "</td>" +
                                        "<td>" + "Standard Opening Id" + "</td>" +
                                        "<td>" + "Test Opening Id" + "</td>" +
                                        "<td>" + "Standard Parent Surface Id" + "</td>" +
                                        "<td>" + "Test Parent Surface Id" + "</td>" +
                                        "<td>" + "Standard Parent Azimuth" + "</td>" +
                                        "<td>" + "Test Parent Azimuth" + "</td>" +
                                        "<td>" + "Standard Parent Tilt" + "</td>" +
                                        "<td>" + "Test Parent Tilt" + "</td>" +
                                        "<td>" + "Standard Surface Area" + "</td>" +
                                        "<td>" + "Test Surface Area" + "</td>" +
                                        "<td>" + "Pass/Fail" + "</td>" +
                                       "</tr>";

                globalMatchObject.MatchedOpeningIds = new Dictionary<string, List<string>>();
                int i = 1;
                //if no openings remove the table.
                if (StandardOpenings.Count < 1)
                    TestOpeningTable = "";
                //compare the openings
                foreach (OpeningDefinitions opening in StandardOpenings)
                {
                    report.Clear();

                    report.testType = TestType.Detailed_Opening_Checks;
                    report.subTestIndex = i;

                    report = GetPossibleOpeningMatches(opening, TestOpenings, report);

                    AddToOutPut("Test 17 for Opening number " + i + " Result: ", report, false);

                    foreach (OpeningDefinitions to in TestOpenings)
                    {
                        if (globalMatchObject.MatchedOpeningIds.ContainsKey(opening.OpeningId))
                        {
                            foreach (string id in globalMatchObject.MatchedOpeningIds[opening.OpeningId])
                            {
                                if (to.OpeningId == id)
                                {
                                    if (report.passOrFail)
                                        TestOpeningTable += "<tr class='success'>" +
                                            "<td>" + "<a href='TestDetailPage.aspx?type=" + (int)report.testType + "&subtype=" + report.subTestIndex + "' target='_blank'>" +
                                            "Detailed Opening Checks " + report.subTestIndex + "</a>" + "</td>" +
                                           "<td>" + opening.OpeningId + "</td>" +
                                            "<td>" + to.OpeningId + "</td>" +
                                            "<td>" + opening.ParentSurfaceId + "</td>" +
                                            "<td>" + to.ParentSurfaceId + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", opening.ParentAzimuth) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", to.ParentAzimuth) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", opening.ParentTilt) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", to.ParentTilt) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", opening.surfaceArea) + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", to.surfaceArea) + "</td>" +
                                            "<td>" + "Pass" + "</td>" +
                                            "</tr>";
                                }

                            }

                        }
                    }
                    //if didn't find match means it failed the test
                    if (!report.passOrFail)
                        TestOpeningTable += "<tr class='error'>" +
                                          "<td>" + "<a href='TestDetailPage.aspx?type=" + (int)report.testType + "&subtype=" + report.subTestIndex + "' target='_blank'>" +
                                          "Detailed Opening Checks " + report.subTestIndex + "</a>" + "</td>" +
                                          "<td>" + opening.OpeningId + "</td>" +
                                          "<td>" + "---" + "</td>" +
                                            "<td>" + opening.ParentSurfaceId + "</td>" +
                                            "<td>" + "---" + "</td>" +
                                            "<td>" + String.Format("{0:#,0.00}", opening.ParentAzimuth) + "</td>" +
                                            "<td>" + "---" + "</td>" +
                                             "<td>" + String.Format("{0:#,0.00}", opening.ParentTilt) + "</td>" +
                                            "<td>" + "---" + "</td>" +
                                             "<td>" + String.Format("{0:#,0.00}", opening.surfaceArea) + "</td>" +
                                            "<td>" + "---" + "</td>" +
                                          "<td>" + "Fail" + "</td>" +
                                          "</tr>";
                    i += 1;

                }
            }
            TestOpeningTable += "</table></div><br/>";
            #endregion

            //close table
            table += "</table></div><br/>";
            //add TestSurfaceTable
            table += TestSurfaceTable + TestOpeningTable;

            CreateSummaryTable();

        }
        private void AddToOutPut(string title, DOEgbXMLReportingObj report, bool createTable)
        {
            //add report to report list
            //have to deep copy the report before put report in the list
            DOEgbXMLReportingObj tmpreport = report.Copy();
            ReportList.Add(tmpreport);

            //title
            output += "<h3>" + title + "</h3>";
            log += title + System.Environment.NewLine;

            //message
            var passTest = report.TestPassedDict.Values;
            bool individualTestBool = true;
            foreach (bool testResult in passTest)
            {
                if (testResult == false)
                {
                    individualTestBool = false;
                    break;
                }
            }
            if (report.passOrFail && individualTestBool)
                output += "<h4 class='text-success'>" + report.longMsg + "</h4>";
            else
            {
                output += "<h4 class='text-error'>" + report.longMsg + "</h4>";
                overallPassTest = false;
            }

            log += report.longMsg + System.Environment.NewLine;

            //message list, print out each message in the list if there are any
            if (report.MessageList.Count > 0)
                for (int i = 0; i < report.MessageList.Count; i++)
                {
                    output += "<p  class='text-info'>" + report.MessageList[i] + "</p>";
                    log += report.MessageList[i] + System.Environment.NewLine;
                }

            output += "<br/>";
            log += System.Environment.NewLine;

            //create table row
            if (createTable)
            {

                if (report.standResult.Count == 0)
                {
                    report.standResult.Add("---");
                    report.testResult.Add("---");
                    report.idList.Add("");
                }

                //for eachout put
                for (int i = 0; i < report.standResult.Count; i++)
                {
                    bool sameString = false;
                    if (report.standResult[i] == report.testResult[i])
                        sameString = true;

                    //check if test pass or fail
                    if ((report.passOrFail && individualTestBool) || sameString)
                        table += "<tr class='success'>";
                    else
                    {
                        table += "<tr class='error'>";
                        overallPassTest = false;
                    }

                    table += "<td>" + "<a href='TestDetailPage.aspx?type=" + (int)report.testType + "&subtype=" + report.subTestIndex + "' target='_blank'>" + title + " " + report.idList[i] + "</a>" + "</td>";

                    if ((report.passOrFail && individualTestBool) || sameString)
                    {
                        table += "<td>" + report.standResult[i] + " " + report.unit + "</td>" +
                                 "<td>" + report.testResult[i] + " " + report.unit + "</td>" +
                                "<td>" + "&plusmn" + report.tolerance + " " + report.unit + "</td>" +
                                "<td>Pass</td>" +
                                "</tr>";
                    }
                    else
                        table += "<td>" + report.standResult[i] + " " + report.unit + "</td>" +
                                 "<td>" + report.testResult[i] + " " + report.unit + "</td>" +
                                "<td>" + "&plusmn" + report.tolerance + " " + report.unit + "</td>" +
                                "<td>Fail</td>" +
                                "</tr>";
                  
                }
            }

        }
        private void CreateSummaryTable()
        {
            //create overall summary table
            //find the right testdetail
            //check if the user pass the test
            bool passTest = true;
            bool aceTest = true;
            foreach (DOEgbXMLReportingObj tmpreport in ReportList)
            {
                Console.WriteLine(tmpreport.testType);
                if (TestCriteria.TestCriteriaDictionary.ContainsKey(tmpreport.testType))
                {
                    Console.WriteLine("This criteria is tested, results (test/standard): " + String.Join("\n", tmpreport.testResult)+"/"+String.Join("\n",tmpreport.standResult));
                    if (TestCriteria.TestCriteriaDictionary[tmpreport.testType] && !tmpreport.passOrFail)
                        passTest = false;
                    if (!TestCriteria.TestCriteriaDictionary[tmpreport.testType] && !tmpreport.passOrFail)
                        aceTest = false;
                }
                else if (tmpreport.testType == TestType.Detailed_Surface_Checks)
                {

                }

                else
                {

                }
            }
            foreach (DOEgbXMLTestDetail detail in TestDetail.TestDetailList)
                if (detail.testName == TestToRun)
                {
                    summaryTable = "<h3>Result Summary</h3>";
                    summaryTable += "<div class='container'><table class='table table-bordered'>";

                    summaryTable += "<tr class='success'>" +
                                    "<td>" + "gbXML schema Test" + "</td>" +
                                    "<td>" + "" + "</td>" +
                                    "<td>" + "Pass" + "</td>" +
                                    "</tr>";

                    if (passTest && aceTest)
                        summaryTable += "<tr class='success'>";
                    else if (passTest)
                        summaryTable += "<tr class='warning'>";
                    else
                        summaryTable += "<tr class='error'>";

                    summaryTable += "<td>" + "gbXML Test" + "</td>" +
                                    "<td>" + detail.shortTitle + "</td>";

                    if (passTest && aceTest)
                        summaryTable += "<td>" + detail.passString + "</td>" + "</tr>";
                    else if (passTest)
                        summaryTable += "<td>" + "You pass the test with minor errors" + "</td>" + "</tr>";
                    else
                        summaryTable += "<td>" + detail.failString + "</td>" + "</tr>";

                    summaryTable += "</table></div><br/>";
                    break;
                }
        }
        private bool TestFileIsAvailable()
        {
            //check if the file available
            if (filepaths.ContainsKey(TestToRun))
            {
                gbXMLStandardFile.Load(filepaths[TestToRun]);
                return true;
            }
            else
            {
                //create overall summary table
                summaryTable = "<h3>Result Summary</h3>";
                summaryTable += "<div class='container'><table class='table table-bordered'>";

                summaryTable += "<tr class='success'>" +
                                "<td>" + "gbXML schema Test" + "</td>" +
                                "<td>" + "" + "</td>" +
                                "<td>" + "Pass" + "</td>" +
                                "</tr>";

                summaryTable += "<tr class='error'>";

                summaryTable += "<td>" + "gbXML Test" + "</td>" +
                                "<td>" + "Test File Currently Not available" + "</td>";

                summaryTable += "<td>" + "Error Error Error" + "</td>" + "</tr>";

                summaryTable += "</table></div><br/>";
                return false;
            }
        }
        #endregion

        private XmlDocument ConvertMetricToUS(XmlDocument mdoc)
        {
            XmlDocument ipdoc = new XmlDocument();

            return ipdoc;
        }

        #region Test Functions
        private DOEgbXMLReportingObj GetUGSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"UndergroundWall\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this SurfaceType in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013


            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "UndergroundWall")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " exterior wall surfaces in the Standard File and " + resultsArray[i - 1] + " exterior wall surfaces in the Test File.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private DOEgbXMLReportingObj GetPossibleOpeningMatches(OpeningDefinitions standardOpening, List<OpeningDefinitions> TestOpenings, DOEgbXMLReportingObj report)
        {
            report.testSummary = "This test checks the geometric accuracy of each opening in your test file against the standard file.";
            report.testSummary += "  For each opening (window, door, skylight) this validator seeks out a similar opening in your test file and";
            //match surfaces at this stage so we know which surface is associated with the window
            report.testSummary += "  The validator first seeks to find all openings that have a parent surface (roof, external wall, etc.) with";
            report.testSummary += " the same azimuth and tilt.  If it finds more than one opening candidate that matches the parent surface tilt and azimuth,";
            report.testSummary += " the validator will make all of these openings possible candidates.";
            report.testSummary += "  The validator then takes these candidates and looks at their polyloop coordinates. ";
            report.testSummary += " and will keep only those openings that have similar polyLoop coordinates";
            report.testSummary += " Next it matches the area, then the width and height, if applicable, and finally checks the insertion";
            report.testSummary += " point coordinates.  If all of these come back within tolerance, the opening has found a match.";
            report.testSummary += "  Otherwise, the test will fail.";
            report.testSummary += "  The summary at the bottom of the page will show the logic of how the test arrived at its conclusion.";


            bool matchedParentAz = false;
            bool matchedParentTilt = false;
            bool matchedPolyLoopCoords = false;

            List<OpeningDefinitions> possibleMatches = new List<OpeningDefinitions>();
            List<OpeningDefinitions> possibleMatches2 = new List<OpeningDefinitions>();
            try
            {
                //find match of parent surface 
                //try matching based on the surface matches
                //if that does not work, then just try to match the parent tilt and parent azimuth to one another
                int i = 0;
                report.MessageList.Add("Starting Parent Azimuth and Tilt Match test....");
                report.MessageList.Add("</br>");
                while (true)
                {
                    //reset
                    matchedParentAz = false;
                    matchedParentTilt = false;
                    OpeningDefinitions testOpening = TestOpenings[i];
                    if (testOpening.ParentAzimuth == standardOpening.ParentAzimuth && testOpening.ParentTilt == standardOpening.ParentTilt)
                    {
                        report.MessageList.Add("Candidate Found.  Test file opening has EXACTLY matched its parent surface azimuth and tilt with the standard opening parent surface azimuth and tilt.");
                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + "'s [parent, azimuth, tilt]: [" + testOpening.ParentSurfaceId + ", " + testOpening.ParentAzimuth + ", " + testOpening.ParentTilt + "]");
                        report.MessageList.Add("Standard Opening " + standardOpening.OpeningId + "'s [parent, azimuth, tilt]: [" + standardOpening.ParentSurfaceId + "," + standardOpening.ParentAzimuth + ", " + standardOpening.ParentTilt + "]");

                        matchedParentAz = true;
                        matchedParentTilt = true;
                    }
                    else
                    {
                        double azDifference = Math.Abs(testOpening.ParentAzimuth - standardOpening.ParentAzimuth);
                        double tiltDifference = Math.Abs(testOpening.ParentTilt - standardOpening.ParentTilt);
                        if (azDifference < DOEgbXMLBasics.Tolerances.SurfaceAzimuthTolerance && tiltDifference < DOEgbXMLBasics.Tolerances.SurfaceTiltTolerance)
                        {
                            report.MessageList.Add("Candidate found.  Test file opening HAS matched WITHIN ALLOWABLE TOLERANCE its parent surface azimuth and tilt with the standard opening parent surface azimuth and tilt.");
                            report.MessageList.Add("Test Opening " + testOpening.OpeningId + "'s [parent, azimuth, tilt]: [" + testOpening.ParentSurfaceId + ", " + testOpening.ParentAzimuth + ", " + testOpening.ParentTilt + "]");
                            report.MessageList.Add("Standard Opening " + standardOpening.OpeningId + "'s [parent, azimuth, tilt]: [" + standardOpening.ParentSurfaceId + "," + standardOpening.ParentAzimuth + ", " + standardOpening.ParentTilt + "]");

                            matchedParentAz = true;
                            matchedParentTilt = true;
                        }
                        else
                        {
                            report.MessageList.Add("Candidate rejected.  Test file opening HAS NOT matched WITHIN ALLOWABLE TOLERANCE its parent surface azimuth and tilt with the standard opening parent surface azimuth and tilt.");
                            report.MessageList.Add("Test Opening " + testOpening.OpeningId + "'s [parent, azimuth, tilt]: [" + testOpening.ParentSurfaceId + ", " + testOpening.ParentAzimuth + ", " + testOpening.ParentTilt + "]");
                            report.MessageList.Add("Standard Opening " + standardOpening.OpeningId + "'s [parent, azimuth, tilt]: [" + standardOpening.ParentSurfaceId + "," + standardOpening.ParentAzimuth + ", " + standardOpening.ParentTilt + "]");
                            report.MessageList.Add("</br>");
                        }
                    }

                    if (matchedParentAz && matchedParentTilt)
                    {
                        possibleMatches.Add(testOpening);
                        report.MessageList.Add("Successful Match Candidate Identified.");
                        report.MessageList.Add("</br>");
                    }
                    i++;

                    if (i == TestOpenings.Count)
                    {
                        if (possibleMatches.Count == 0)
                        {
                            //no candidates found
                            report.MessageList.Add("No candidates found in the test file to match standard file opening " + standardOpening.OpeningId);
                            report.passOrFail = false;
                            report.longMsg = "Test to find suitable opening candidate in the test file has failed.  Parent Tilt and Azimuth matches could not be established.";
                            //no need to go further
                            return report;
                        }
                        break;
                    }

                }
                report.MessageList.Add("</br>");
                report.MessageList.Add("Starting Opening PolyLoop Coordinate Match test.........");
                i = 0;
                while (true)
                {
                    OpeningDefinitions testOpening = possibleMatches[i];
                    //continue to next test

                    //continue the next batch of tests
                    //polyloop absolute coordinates
                    //check the polyLoop coordinates
                    foreach (Vector.MemorySafe_CartCoord standardPolyLoopCoord in standardOpening.PlCoords)
                    {
                        report = GetOpeningPolyLoopCoordMatch(standardPolyLoopCoord, testOpening, report, standardOpening.OpeningId);
                        if (report.passOrFail)
                        {
                            matchedPolyLoopCoords = true;
                            continue;
                        }
                        else
                        {
                            report.MessageList.Add("Could not find a coordinate match in the test opening polyloop.");
                            matchedPolyLoopCoords = false;
                            break;
                        }
                    }
                    //if matchePolyLoopCoords comes back true, then a candidate has been found that matches all polyloop coords within tolerance
                    if (matchedPolyLoopCoords == true)
                    {
                        possibleMatches2.Add(testOpening);
                    }
                    i++;

                    if (i == possibleMatches.Count)
                    {
                        if (possibleMatches2.Count == 0)
                        {
                            report.MessageList.Add("No candidates found in the test file to match standard file opening " + standardOpening.OpeningId);
                            report.passOrFail = false;
                            report.longMsg = "Test to find suitable opening candidate in the test file has failed.  Parent Tilt and Azimuth matches were established, but these candidates did not produce good polyLoop coordinate matches.";
                            //no need to go further
                            return report;
                        }
                        break;
                    }
                }
                //next set of tests 
                //polyloop area tests
                report.MessageList.Add("</br>");
                report.MessageList.Add("Starting Opening Surface Area Match test.........");
                possibleMatches.Clear();
                i = 0;
                while (true)
                {
                    #region
                    OpeningDefinitions testOpening = possibleMatches2[i];

                    if (Math.Abs(standardOpening.PlRHRVector.X) == 1 && standardOpening.PlRHRVector.Y == 0 && standardOpening.PlRHRVector.Z == 0)
                    {
                        List<Vector.MemorySafe_CartCoord> coordList = new List<Vector.MemorySafe_CartCoord>();
                        foreach (Vector.MemorySafe_CartCoord coord in standardOpening.PlCoords)
                        {
                            //only take the Y and Z coordinates and throw out the X because we can assume that they are all the same
                            Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(0, coord.Y, coord.Z);
                            coordList.Add(simple);

                        }
                        double area = Math.Abs(GetAreaFrom2DPolyLoop(coordList));
                        standardOpening.surfaceArea = area;
                        if (area == -999)
                        {
                            //these messages should never occur and are a sign of some sort of serious, as of yet unknown error
                            //March 20 2013
                            report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                            report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");
                            report.longMsg = "Fatal error.  Please contact gbXML administrator";
                            report.passOrFail = false;
                            return report;

                        }
                        double testOpeningArea = 0;

                        if (Math.Abs(testOpening.PlRHRVector.X) == 1 && testOpening.PlRHRVector.Y == 0 &&
                                testOpening.PlRHRVector.Z == 0)
                        {
                            List<Vector.MemorySafe_CartCoord> testCoordList = new List<Vector.MemorySafe_CartCoord>();
                            foreach (Vector.MemorySafe_CartCoord coord in testOpening.PlCoords)
                            {
                                Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(0, coord.Y, coord.Z);
                                testCoordList.Add(simple);
                            }
                            testOpeningArea = Math.Abs(GetAreaFrom2DPolyLoop(testCoordList));
                            testOpening.surfaceArea = testOpeningArea;
                            if (testOpeningArea == -999)
                            {
                                //these messages should never occur and are a sign of some sort of serious, as of yet unknown error
                                //March 20 2013
                                report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                report.longMsg = "Fatal error.  Please contact gbXML administrator";
                                report.passOrFail = false;
                                return report;
                            }
                            double difference = Math.Abs(area) - Math.Abs(testOpeningArea);
                            if (difference < Math.Abs(area) * DOEgbXMLBasics.Tolerances.OpeningAreaPercentageTolerance)
                            {

                                if (difference == 0)
                                {
                                    //then it perfectly matches, go on to check the poly loop coordinates
                                    //then check the insertion point
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard opening: " + standardOpening.OpeningId + " exactly.");
                                    possibleMatches.Add(testOpening);
                                }
                                else
                                {
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard opening: " + standardOpening.OpeningId + " within the allowable area percentage tolerance.");
                                    possibleMatches.Add(testOpening);
                                }
                            }
                            else
                            {
                                report.MessageList.Add("The standard file opening cannot find a match for its surface area of opening: " + standardOpening.OpeningId + " through a comparison of its polyloop coordinates with test opening: " + testOpening.OpeningId);
                                //don't return here, it will be returned below
                            }
                        }
                        else
                        {
                            //by definition, the Window opening should always use coordinates that create a normal vector that points in the 
                            //positive or negative X direction.  If the test file does not do this, then this is in violation of the 
                            //gbXML spec
                            report.longMsg = ("This test has failed because the test opening" + testOpening.OpeningId + "has polyloop coordinates ");
                            report.longMsg += (" that do not have the same normal vector as the standard opening.");
                            report.passOrFail = false;
                        }
                    }
                    else if (standardOpening.PlRHRVector.X == 0 && Math.Abs(standardOpening.PlRHRVector.Y) == 1 && standardOpening.PlRHRVector.Z == 0)
                    {
                        List<Vector.MemorySafe_CartCoord> coordList = new List<Vector.MemorySafe_CartCoord>();
                        foreach (Vector.MemorySafe_CartCoord coord in standardOpening.PlCoords)
                        {
                            //only take the Y and Z coordinates and throw out the X because we can assume that they are all the same
                            Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, 0, coord.Z);
                            coordList.Add(simple);

                        }
                        double area = Math.Abs(GetAreaFrom2DPolyLoop(coordList));
                        standardOpening.surfaceArea = area;
                        if (area == -999)
                        {
                            //these messages should never occur and are a sign of some sort of serious, as of yet unknown error
                            //March 20 2013
                            report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                            report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");
                            report.longMsg = "Fatal error.  Please contact gbXML administrator";
                            report.passOrFail = false;
                            return report;

                        }
                        double testOpeningArea = 0;

                        if (testOpening.PlRHRVector.X == 0 && Math.Abs(testOpening.PlRHRVector.Y) == 1 &&
                                testOpening.PlRHRVector.Z == 0)
                        {
                            List<Vector.MemorySafe_CartCoord> testCoordList = new List<Vector.MemorySafe_CartCoord>();
                            foreach (Vector.MemorySafe_CartCoord coord in testOpening.PlCoords)
                            {
                                Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, 0, coord.Z);
                                testCoordList.Add(simple);
                            }
                            testOpeningArea = Math.Abs(GetAreaFrom2DPolyLoop(testCoordList));
                            testOpening.surfaceArea = testOpeningArea;
                            if (testOpeningArea == -999)
                            {
                                //these messages should never occur and are a sign of some sort of serious, as of yet unknown error
                                //March 20 2013
                                report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                report.longMsg = "Fatal error.  Please contact gbXML administrator";
                                report.passOrFail = false;
                                return report;
                            }
                            double difference = Math.Abs(area) - Math.Abs(testOpeningArea);
                            if (difference < Math.Abs(area) * DOEgbXMLBasics.Tolerances.OpeningAreaPercentageTolerance)
                            {

                                if (difference == 0)
                                {
                                    //then it perfectly matches, go on to check the poly loop coordinates
                                    //then check the insertion point
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard Opening: " + standardOpening.OpeningId + " exactly.");
                                    possibleMatches.Add(testOpening);
                                }
                                else
                                {
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard Opening: " + standardOpening.OpeningId + " within the allowable area percentage tolerance.");
                                    possibleMatches.Add(testOpening);
                                }
                            }
                            else
                            {
                                report.MessageList.Add("The standard file opening cannot find a match for its surface area of Opening: " + standardOpening.OpeningId + " through a comparison of its polyloop coordinates with test Opening: " + testOpening.OpeningId);
                                //don't return here, it will be returned below
                            }
                        }
                        else
                        {
                            //by definition, the Window opening should always use coordinates that create a normal vector that points in the 
                            //positive or negative X direction.  If the test file does not do this, then this is in violation of the 
                            //gbXML spec
                            report.longMsg = ("This test has failed because the test opening" + testOpening.OpeningId + "has polyloop coordinates ");
                            report.longMsg += (" that do not have the same normal vector as the standard opening.");
                            report.passOrFail = false;
                        }
                    }
                    else if (standardOpening.PlRHRVector.X == 0 && standardOpening.PlRHRVector.Y == 0 && Math.Abs(standardOpening.PlRHRVector.Z) == 1)
                    {
                        List<Vector.MemorySafe_CartCoord> coordList = new List<Vector.MemorySafe_CartCoord>();
                        foreach (Vector.MemorySafe_CartCoord coord in standardOpening.PlCoords)
                        {
                            //only take the X and Y coordinates and throw out the Z because we can assume that they are all the same
                            Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, coord.Y, 0);
                            coordList.Add(simple);

                        }
                        double area = Math.Abs(GetAreaFrom2DPolyLoop(coordList));
                        standardOpening.surfaceArea = area;
                        if (area == -999)
                        {
                            report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                            report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");

                        }
                        double testOpeningArea = 0;

                        if (testOpening.PlRHRVector.X == 0 && testOpening.PlRHRVector.Y == 0 &&
                                                        Math.Abs(testOpening.PlRHRVector.Z) == 1)
                        {
                            List<Vector.MemorySafe_CartCoord> testCoordList = new List<Vector.MemorySafe_CartCoord>();
                            foreach (Vector.MemorySafe_CartCoord coord in testOpening.PlCoords)
                            {
                                Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, coord.Y, 0);
                                testCoordList.Add(simple);
                            }
                            testOpeningArea = Math.Abs(GetAreaFrom2DPolyLoop(testCoordList));
                            testOpening.surfaceArea = testOpeningArea;
                            if (testOpeningArea == -999)
                            {
                                //these messages should never occur and are a sign of some sort of serious, as of yet unknown error
                                //March 20 2013
                                report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                report.longMsg = "Fatal error.  Please contact gbXML administrator";
                                report.passOrFail = false;
                                return report;
                            }
                            double difference = Math.Abs(area) - Math.Abs(testOpeningArea);
                            if (difference < Math.Abs(area) * DOEgbXMLBasics.Tolerances.OpeningAreaPercentageTolerance)
                            {

                                if (difference == 0)
                                {
                                    //then it perfectly matches, go on to check the poly loop coordinates
                                    //then check the insertion point
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard Opening: " + standardOpening.OpeningId + " exactly.");
                                    possibleMatches.Add(testOpening);
                                }
                                else
                                {
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard Opening: " + standardOpening.OpeningId + " within the allowable area percentage tolerance.");
                                    possibleMatches.Add(testOpening);
                                }
                            }
                            else
                            {
                                report.MessageList.Add("The standard file opening cannot find a match for its surface area of Opening: " + standardOpening.OpeningId + " through a comparison of its polyloop coordinates with test Opening: " + testOpening.OpeningId);
                                //don't return here, it will be returned below
                            }
                        }
                        else
                        {
                            //by definition, the Window opening should always use coordinates that create a normal vector that points in the 
                            //positive or negative X direction.  If the test file does not do this, then this is in violation of the 
                            //gbXML spec
                            report.longMsg = ("This test has failed because the test opening" + testOpening.OpeningId + "has polyloop coordinates ");
                            report.longMsg += (" that do not have the same normal vector as the standard opening.");
                            report.passOrFail = false;
                        }

                    }
                    //the opening is not aligned along a reference frame axis
                    else
                    {
                        report.MessageList.Add("This standard Opening is not aligned along a reference plane axis, and will be rotated into a new coordinate frame.");
                        report.MessageList.Add("Commencing rotation to 2-D.");
                        //New Z Axis for this plane is the normal vector, does not need to be created
                        //Get New Y Axis which is the surface Normal Vector cross the original global reference X unit vector (all unit vectors please
                        Vector.CartVect tempY = new Vector.CartVect();
                        Vector.MemorySafe_CartVect globalReferenceX = new Vector.MemorySafe_CartVect(1,0,0);
                        tempY = Vector.CrossProductNVRetMSMS(standardOpening.PlRHRVector, globalReferenceX);
                        tempY = Vector.UnitVector(tempY);
                        Vector.MemorySafe_CartVect localY = Vector.convertToMemorySafeVector(tempY);

                        //new X axis is the localY cross the surface normal vector
                        Vector.CartVect tempX = new Vector.CartVect();
                        tempX = Vector.CrossProductNVRetMSMS(localY, standardOpening.PlRHRVector);
                        tempX = Vector.UnitVector(tempX);
                        Vector.MemorySafe_CartVect localX = Vector.convertToMemorySafeVector(tempX);

                        //convert the polyloop coordinates to a local 2-D reference frame
                        //using a trick employed by video game programmers found here http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
                        List<Vector.MemorySafe_CartCoord> translatedCoordinates = new List<Vector.MemorySafe_CartCoord>();
                        Vector.MemorySafe_CartCoord newOrigin = new Vector.MemorySafe_CartCoord(0,0,0);

                        translatedCoordinates.Add(newOrigin);
                        for (int j = 1; j < standardOpening.PlCoords.Count; j++)
                        {
                            //randomly assigns the first polyLoop coordinate as the origin
                            Vector.MemorySafe_CartCoord origin = standardOpening.PlCoords[0];
                            //captures the components of a vector drawn from the new origin to the 
                            Vector.CartVect distance = new Vector.CartVect();
                            distance.X = standardOpening.PlCoords[j].X - origin.X;
                            distance.Y = standardOpening.PlCoords[j].Y - origin.Y;
                            distance.Z = standardOpening.PlCoords[j].Z - origin.Z;
                            Vector.CartCoord translatedPt = new Vector.CartCoord();
                            //x coordinate is distance vector dot the new local X axis
                            translatedPt.X = distance.X * localX.X + distance.Y * localX.Y + distance.Z * localX.Z;
                            //y coordinate is distance vector dot the new local Y axis
                            translatedPt.Y = distance.X * localY.X + distance.Y * localY.Y + distance.Z * localY.Z;
                            translatedPt.Z = 0;
                            Vector.MemorySafe_CartCoord memsafetranspt = Vector.convertToMemorySafeCoord(translatedPt);
                            translatedCoordinates.Add(memsafetranspt);

                        }
                        double area = GetAreaFrom2DPolyLoop(translatedCoordinates);
                        standardOpening.surfaceArea = area;
                        if (area == -999)
                        {
                            report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                            report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");

                        }
                        //get the area of the test candidates using the polyloop coordinates
                        Vector.CartVect testtempY = new Vector.CartVect();
                        Vector.MemorySafe_CartVect testglobalReferenceX = new Vector.MemorySafe_CartVect(1,0,0);

                        testtempY = Vector.CrossProductNVRetMSMS(testOpening.PlRHRVector, testglobalReferenceX);
                        testtempY = Vector.UnitVector(tempY);
                        Vector.MemorySafe_CartVect testlocalY = Vector.convertToMemorySafeVector(testtempY);
                        

                        //new X axis is the localY cross the surface normal vector
                        Vector.CartVect testtempX = new Vector.CartVect();
                        testtempX = Vector.CrossProductNVRetMSMS(testlocalY, testOpening.PlRHRVector);
                        testtempX = Vector.UnitVector(testtempX);
                        Vector.MemorySafe_CartVect testlocalX = Vector.convertToMemorySafeVector(testtempX);


                        //convert the polyloop coordinates to a local 2-D reference frame
                        //using a trick employed by video game programmers found here http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
                        List<Vector.MemorySafe_CartCoord> testtranslatedCoordinates = new List<Vector.MemorySafe_CartCoord>();
                        Vector.MemorySafe_CartCoord newOriginTest = new Vector.MemorySafe_CartCoord(0,0,0);
                        testtranslatedCoordinates.Add(newOriginTest);
                        for (int j = 1; j < testOpening.PlCoords.Count; j++)
                        {
                            //randomly assigns the first polyLoop coordinate as the origin
                            Vector.MemorySafe_CartCoord origin = testOpening.PlCoords[0];
                            //captures the components of a vector drawn from the new origin to the 
                            Vector.CartVect distance = new Vector.CartVect();
                            distance.X = testOpening.PlCoords[j].X - origin.X;
                            distance.Y = testOpening.PlCoords[j].Y - origin.Y;
                            distance.Z = testOpening.PlCoords[j].Z - origin.Z;
                            Vector.CartCoord translatedPt = new Vector.CartCoord();
                            //x coordinate is distance vector dot the new local X axis
                            translatedPt.X = distance.X * localX.X + distance.Y * localX.Y + distance.Z * localX.Z;
                            //y coordinate is distance vector dot the new local Y axis
                            translatedPt.Y = distance.X * localY.X + distance.Y * localY.Y + distance.Z * localY.Z;
                            translatedPt.Z = 0;
                            Vector.MemorySafe_CartCoord memsafetranspt = Vector.convertToMemorySafeCoord(translatedPt);
                            testtranslatedCoordinates.Add(memsafetranspt);

                        }
                        double testOpeningArea = GetAreaFrom2DPolyLoop(translatedCoordinates);
                        testOpening.surfaceArea = testOpeningArea;
                        if (testOpeningArea == -999)
                        {
                            report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                        }
                        double difference = Math.Abs(area) - Math.Abs(testOpeningArea);
                        if (difference < Math.Abs(area) * DOEgbXMLBasics.Tolerances.OpeningAreaPercentageTolerance)
                        {

                            if (difference == 0)
                            {
                                //then it perfectly matches, go on to check the poly loop coordinates
                                //then check the insertion point
                                report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard Opening: " + standardOpening.OpeningId + " exactly.");
                                possibleMatches.Add(testOpening);
                            }
                            else
                            {
                                report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " polyloop surface area matches the polyLoop surface area of the standard Opening: " + standardOpening.OpeningId + " within the allowable area percentage tolerance.");
                                possibleMatches.Add(testOpening);
                            }
                        }
                        else
                        {
                            report.MessageList.Add("The standard file opening cannot find a match for its surface area of Opening: " + standardOpening.OpeningId + " through a comparison of its polyloop coordinates with test Opening: " + testOpening.OpeningId);
                            //don't return here, it will be returned below
                        }

                    }
                    i++;
                    if (i == possibleMatches2.Count)
                    {
                        if (possibleMatches.Count == 0)
                        {
                            report.MessageList.Add("No area match could be found for standard opening: " + standardOpening.OpeningId + ".");
                            report.longMsg = "The search routine has ended and could not find a match for opening: " + standardOpening.OpeningId +
                                ".  Attempt to match the area of the standard file with test file openings failed.";
                            return report;

                        }
                        else
                        {
                            //you are good to go with some more matches
                            report.MessageList.Add("Area matching SUCCESS for standard file Opening id: " + standardOpening.OpeningId);
                            report.MessageList.Add("Commencing comparisons of height, width, and insertion point.");
                            break;
                        }
                    }

                    #endregion
                }
                //test the width and height, if applicable
                report.MessageList.Add("</br>");
                report.MessageList.Add("Starting Width and Height Match test.........");
                possibleMatches2.Clear();
                i = 0;
                //surface area using the coordinates of the polyloop.  We already assume that they are planar, as previously tested
                while (true)
                {
                    //see if the openings are regular
                    bool isStandardRegular = IsOpeningRegular(standardOpening);
                    bool isTestRegular = IsOpeningRegular(possibleMatches[i]);
                    //if they are...go ahead and use width and height, otherwise the values are not reliable
                    if (isStandardRegular)
                    {
                        //output something
                        if (isTestRegular)
                        {
                            //output something
                            //perform tests

                            OpeningDefinitions testOpening = possibleMatches[i];
                            double testWidth = testOpening.Width;
                            double standardWidth = standardOpening.Width;
                            double testHeight = testOpening.Height;
                            double standardHeight = standardOpening.Height;
                            double widthDifference = Math.Abs(testWidth - standardWidth);
                            double heightDiffefence = Math.Abs(testHeight - standardHeight);

                            if (widthDifference <= DOEgbXMLBasics.Tolerances.OpeningWidthTolerance)
                            {
                                if (widthDifference == 0)
                                {
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " reported Width value matches the Width value of the standard Opening: " + standardOpening.OpeningId + " exactly.");
                                }
                                else
                                {
                                    report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " reported Width value matches the Width value of the standard Opening: " + standardOpening.OpeningId + " within the allowable tolerance.");
                                }
                                //check the height
                                if (heightDiffefence <= DOEgbXMLBasics.Tolerances.OpeningHeightTolerance)
                                {
                                    if (heightDiffefence == 0)
                                    {
                                        report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " reported Height value matches the Height value of the standard Opening: " + standardOpening.OpeningId + " exactly.");
                                        possibleMatches2.Add(testOpening);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("The test Opening: " + testOpening.OpeningId + " reported Height value matches the Height value of the standard Opening: " + standardOpening.OpeningId + " within the allowable tolerance.");
                                        possibleMatches2.Add(testOpening);
                                    }
                                }
                                else
                                {
                                    //fail, did not match height
                                    report.MessageList.Add("The standard file Opening: " + standardOpening.OpeningId + "The standard file opening cannot find a match for its surface area of Opening: " + standardOpening.OpeningId + " after comparison its Height value with test opening: " + testOpening.OpeningId);
                                    report.passOrFail = false;
                                    continue;
                                }
                            }
                            else
                            {
                                //failed, did not match width
                                report.MessageList.Add("The standard file Opening: " + standardOpening.OpeningId + " cannot find a match for its width after comparison the width value of test Opening: " + testOpening.OpeningId);
                                report.passOrFail = false;
                                continue;
                            }
                        }
                        else
                        {
                            //let them know the the test opening is not a square or rectangle, but the standard file opening is
                            //go ahead and break out of the while loop because we aren't testing for width and height
                            report.MessageList.Add("The standard file Opening: " + standardOpening.OpeningId + " is a rectangle or square, but the test file Opening: " + standardOpening.OpeningId + " is not.  Cannot test for a valid width and height.");
                            report.MessageList.Add("Searching for another test Opening.");
                            continue;
                        }
                    }
                    else
                    {
                        //tell them that the widths and Heights will Not be checked
                        //because the standard file opening is not a square or rectangle
                        report.MessageList.Add("Will not be testing for the Width and Height values for standard Opening: " + standardOpening.OpeningId + ".  The Opening is not shaped like a rectangle or square.");
                        report.MessageList.Add("Going on to check insertion point accuracy.");
                        //needed to transfer values over to possibleMatches2, so deep copy
                        possibleMatches2 = new List<OpeningDefinitions>(possibleMatches);
                        break;
                    }
                    i++;
                    if (possibleMatches.Count == i)
                    {
                        //means that there is no match for width and height
                        if (possibleMatches2.Count == 0)
                        {
                            report.MessageList.Add("There is no match found for the width and height for Opening: " + standardOpening.OpeningId);
                            report.passOrFail = false;
                            report.longMsg = "The opening test has ended at the search for width and height values equal to standard Opening: " + standardOpening.OpeningId;
                            return report;
                        }
                        break;
                    }

                }
                report.MessageList.Add("</br>");
                report.MessageList.Add("Starting Insertion Point Coordinate Match test.........");
                possibleMatches.Clear();
                //test the insertion point coordinates
                i = 0;
                while (true)
                {
                    OpeningDefinitions testOpening = possibleMatches2[i];
                    double diffX = Math.Abs(testOpening.InsertionPoint.X - standardOpening.InsertionPoint.X);
                    double diffY = Math.Abs(testOpening.InsertionPoint.Y - standardOpening.InsertionPoint.Y);
                    double diffZ = Math.Abs(testOpening.InsertionPoint.Z - standardOpening.InsertionPoint.Z);

                    if (diffX <= DOEgbXMLBasics.Tolerances.OpeningSurfaceInsPtXTolerance && diffY <= DOEgbXMLBasics.Tolerances.OpeningSurfaceInsPtYTolerance &&
                        diffZ <= DOEgbXMLBasics.Tolerances.OpeningSurfaceInsPtZTolerance)
                    {
                        if (diffX == 0)
                        {
                            //perfect X coordinate match
                            report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point X-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                            if (diffY == 0)
                            {
                                //perfect Y coordinate match
                                report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point Y-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                if (diffZ == 0)
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point Z-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                    possibleMatches.Add(testOpening);

                                }
                                else
                                {
                                    // Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point Z-Coordinate when compared with Test opening: " + testOpening.OpeningId);
                                    //we continue because we search for other matches if there are any
                                    possibleMatches.Add(testOpening);

                                }
                            }
                            else
                            {
                                //y-coordinate is within tolerance
                                report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point Y-Coordinate when compared with Test opening: " + testOpening.OpeningId);
                                if (diffZ == 0)
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point Z-Coordinate when compared with Test opening: " + testOpening.OpeningId);
                                    possibleMatches.Add(testOpening);

                                }
                                else
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point Z-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                    //we continue because we search for other matches if there are any
                                    possibleMatches.Add(testOpening);

                                }
                            }

                        }
                        // X is within tolerance
                        else
                        {
                            report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point X-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                            if (diffY == 0)
                            {
                                //perfect Y coordinate match
                                report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point Y-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                if (diffZ == 0)
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point Z-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                    possibleMatches.Add(testOpening);

                                }
                                else
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point Z-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                    //we continue because we search for other matches if there are any
                                    possibleMatches.Add(testOpening);

                                }
                            }
                            else
                            {
                                //y-coordinate is within tolerance
                                report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point Y-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                if (diffZ == 0)
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a perfect match for its insertion point Z-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                    possibleMatches.Add(testOpening);

                                }
                                else
                                {
                                    //perfect Z coordinate match
                                    report.MessageList.Add("Standard Opening: " + standardOpening.OpeningId + " has found a match within allowable tolerances for its insertion point Z-Coordinate when compared with test Opening: " + testOpening.OpeningId);
                                    //we continue because we search for other matches if there are any
                                    possibleMatches.Add(testOpening);

                                }
                            }
                        }
                    }
                    report.MessageList.Add("Standard Opening Ins Pt: (" + standardOpening.InsertionPoint.X.ToString() + "," + standardOpening.InsertionPoint.Y.ToString() + "," + standardOpening.InsertionPoint.Z.ToString() + ")");
                    report.MessageList.Add("Test File Opening Ins Pt: (" + testOpening.InsertionPoint.X.ToString() + "," + testOpening.InsertionPoint.Y.ToString() + "," + testOpening.InsertionPoint.Z.ToString() + ")");
                    i++;
                    if (possibleMatches2.Count == i)
                    {
                        if (possibleMatches.Count == 1)
                        {
                            List<string> openingMatch = new List<string>();
                            openingMatch.Add(possibleMatches[0].OpeningId);
                            report.MessageList.Add("Standard file Opening: " + standardOpening.OpeningId + " is matched to test file Opening: " + testOpening.OpeningId);
                            globalMatchObject.MatchedOpeningIds.Add(standardOpening.OpeningId, openingMatch);
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            if (possibleMatches.Count == 0)
                            {
                                report.MessageList.Add("Standard file Opening: " + standardOpening.OpeningId + " found no match for insertion point in the test file of the remaining candidates.");
                                report.passOrFail = false;
                                return report;
                            }
                            else
                            {
                                report.MessageList.Add("Standard file Opening: " + standardOpening.OpeningId + " is matched to multiple openings:");
                                foreach (OpeningDefinitions opening in possibleMatches)
                                {
                                    report.MessageList.Add("Test Opening:" + opening.OpeningId + "matched insertion point");
                                }
                                //resolve by trying to match to the standard opening and test opening parent surfaces.
                                //for the standard opening
                                if (globalMatchObject.MatchedSurfaceIds.ContainsKey(standardOpening.ParentSurfaceId))
                                {
                                    List<string> possibleSurfaceMatches = globalMatchObject.MatchedSurfaceIds[standardOpening.ParentSurfaceId];
                                    if (possibleSurfaceMatches.Count == 1)
                                    {
                                        //then a match was found originally during get possible surface matches.  That is good, we only want one
                                        foreach (OpeningDefinitions openingRemaining in possibleMatches)
                                        {
                                            if (openingRemaining.ParentSurfaceId == possibleSurfaceMatches[0])
                                            {
                                                //this is the match we want
                                                //else we would have to continue
                                                report.MessageList.Add("The test Opening: " + openingRemaining.OpeningId + " has been matched to the standard Opening: " + standardOpening.OpeningId +
                                                    ".  Their parent surface ids have been matched.  Thus the conflict has been resolved.  (Standard opening parent surface Id, test opening parent surface Id" + standardOpening.ParentSurfaceId + "," + openingRemaining.ParentSurfaceId);
                                                report.passOrFail = true;
                                                List<string> openingMatch = new List<string>();
                                                openingMatch.Add(possibleMatches[0].OpeningId);
                                                globalMatchObject.MatchedOpeningIds.Add(standardOpening.OpeningId, openingMatch);
                                                return report;
                                            }
                                            else
                                            {
                                                //do nothing.  Maybe report that the parent Surface Id does not match the standard Opening
                                                report.MessageList.Add("Test Opening:" + openingRemaining.OpeningId + " does not match the standard Opening: " + standardOpening.OpeningId +
                                                    ".  Their parent surface ids do not coincide.  (Standard Opening parent surface id, test Opening parent surface id)" + standardOpening.ParentSurfaceId + "," + openingRemaining.ParentSurfaceId);
                                            }
                                        }
                                    }
                                }
                                report.passOrFail = false;
                                return report;
                            }
                        }
                    }

                }

                //finished

            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
            }
            return report;
        }

        private List<OpeningDefinitions> GetFileOpeningDefs(XmlDocument TestFile, XmlNamespaceManager TestNSM)
        {
            List<OpeningDefinitions> openings = new List<OpeningDefinitions>();
            try
            {

                XmlNodeList nodes = TestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", TestNSM);
                foreach (XmlNode openingNode in nodes)
                {
                    //initialize a new instance of the class
                    OpeningDefinitions openingDef = new OpeningDefinitions();

                    openingDef.PlCoords = new List<Vector.MemorySafe_CartCoord>();
                    openingDef.InsertionPoint = new Vector.CartCoord();
                    

                    //get parent id
                    XmlAttributeCollection parentSurfaceAttributes = openingNode.ParentNode.Attributes;
                    foreach (XmlAttribute parentAt in parentSurfaceAttributes)
                    {
                        if (parentAt.Name == "id")
                        {
                            openingDef.ParentSurfaceId = parentAt.Value;
                            break;
                        }
                    }
                    //get Parent Azimuth and Tilt
                    XmlNode surfaceParentNode = openingNode.ParentNode;
                    if (surfaceParentNode.HasChildNodes)
                    {
                        XmlNodeList surfaceParentNodesChillun = surfaceParentNode.ChildNodes;
                        foreach (XmlNode chileNode in surfaceParentNodesChillun)
                        {
                            if (chileNode.Name == "RectangularGeometry")
                            {
                                if (chileNode.HasChildNodes)
                                {
                                    foreach (XmlNode grandchileNode in chileNode)
                                    {
                                        if (grandchileNode.Name == "Tilt") { openingDef.ParentTilt = Convert.ToDouble(grandchileNode.InnerText); }
                                        else if (grandchileNode.Name == "Azimuth") { openingDef.ParentAzimuth = Convert.ToDouble(grandchileNode.InnerText); }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                    //get surface Id and Opening Type
                    XmlAttributeCollection openingAtts = openingNode.Attributes;
                    foreach (XmlAttribute at in openingAtts)
                    {
                        if (at.Name == "id")
                        {
                            openingDef.OpeningId = at.Value;
                        }
                        else if (at.Name == "openingType")
                        {
                            openingDef.OpeningType = at.Value;
                        }
                    }
                    if (openingNode.HasChildNodes)
                    {
                        XmlNodeList surfChildNodes = openingNode.ChildNodes;
                        foreach (XmlNode node in surfChildNodes)
                        {

                            if (node.Name == "RectangularGeometry")
                            {
                                if (node.HasChildNodes)
                                {
                                    XmlNodeList rectGeomChildren = node.ChildNodes;
                                    foreach (XmlNode rgChildNode in rectGeomChildren)
                                    {
                                        if (rgChildNode.Name == "Azimuth") { openingDef.Azimuth = Convert.ToDouble(rgChildNode.InnerText); }
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
                                                            openingDef.InsertionPoint.X = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                        case 2:
                                                            openingDef.InsertionPoint.Y = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                        case 3:
                                                            openingDef.InsertionPoint.Z = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                    }
                                                    pointCount++;
                                                }
                                            }
                                        }
                                        else if (rgChildNode.Name == "Tilt") { openingDef.Tilt = Convert.ToDouble(rgChildNode.InnerText); }
                                        else if (rgChildNode.Name == "Height") { openingDef.Height = Convert.ToDouble(rgChildNode.InnerText); }
                                        else if (rgChildNode.Name == "Width") { openingDef.Width = Convert.ToDouble(rgChildNode.InnerText); }
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
                                            openingDef.PlCoords.Add(memsafecoord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Vector.MemorySafe_CartVect plRHRVect = Vector.GetMemRHR(openingDef.PlCoords);
                    openingDef.PlRHRVector = plRHRVect;
                    //may want to forego the above since the orientation is embedded in the parent object.  It may be smarter to just include the azimuth and tilt of the parent object?
                    openings.Add(openingDef);
                }

                return openings;
            }
            catch (Exception e)
            {
                return openings;
            }
        }

        private DOEgbXMLReportingObj TestSurfacePlanarTest(List<SurfaceDefinitions> TestSurfaces, DOEgbXMLReportingObj report)
        {
            //ensure that each set of RHR tests result in parallel or anti-parallel resultant vectors, or else fail the test

            foreach (SurfaceDefinitions ts in TestSurfaces)
            {
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
                        continue;
                    }
                    //anti-parallel
                    else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                    {
                        continue;
                    }
                    else if (Math.Abs(xProducts[j].X) - Math.Abs(xProducts[j + 1].X) < .0001 && Math.Abs(xProducts[j].Y) - Math.Abs(xProducts[j + 1].Y) < .0001 &&
                        Math.Abs(xProducts[j].Z) - Math.Abs(xProducts[j + 1].Z) < 0.0001)
                    {
                        continue;
                    }
                    else
                    {
                        report.MessageList.Add("Test file's Surface, id: " + ts.SurfaceId + " has polyLoop coordinates that do not form a planar surface.  This fails the detailed surface tests and will not continue.");
                        report.passOrFail = false;
                        report.longMsg = "Detailed surface test failed during the planar surface checks.  Without planar surfaces, this test cannot be safely executed.";
                        return report;
                    }
                }
            }
            report.MessageList.Add("All test file's surfaces have polyloop descriptions that describe a planar surface.  Planar surface test succeeded.");
            report.passOrFail = true;
            return report;

        }

        private DOEgbXMLReportingObj TestOpeningPlanarTest(List<OpeningDefinitions> TestOpenings, DOEgbXMLReportingObj report)
        {
            //ensure that each set of RHR tests result in parallel or anti-parallel resultant vectors, or else fail the test

            foreach (OpeningDefinitions to in TestOpenings)
            {
                Dictionary<string, List<Vector.CartVect>> surfaceXProducts = new Dictionary<string, List<Vector.CartVect>>();
                List<Vector.CartVect> xProducts = new List<Vector.CartVect>();
                for (int i = 0; i < to.PlCoords.Count - 2; i++)
                {
                    //Get the Cross Product
                    VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(to.PlCoords[i], to.PlCoords[i + 1]);
                    VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(to.PlCoords[i + 1], to.PlCoords[i + 2]);
                    Vector.CartVect xProd = Vector.CrossProduct(v1, v2);
                    xProd = Vector.UnitVector(xProd);
                    xProducts.Add(xProd);
                }
                surfaceXProducts.Add(to.OpeningId, xProducts);
                for (int j = 0; j < xProducts.Count - 1; j++)
                {
                    //parallel
                    if (xProducts[j].X == xProducts[j + 1].X && xProducts[j].Y == xProducts[j + 1].Y && xProducts[j].Z == xProducts[j + 1].Z)
                    {
                        continue;
                    }
                    //anti-parallel
                    else if (xProducts[j].X == -1 * xProducts[j + 1].X && xProducts[j].Y == -1 * xProducts[j + 1].Y && xProducts[j].Z == -1 * xProducts[j + 1].Z)
                    {
                        continue;
                    }
                    else
                    {
                        report.MessageList.Add("Test file's Opening, id: " + to.OpeningId +
                            " has polyLoop coordinates that do not form a planar surface.  This fails the detailed surface tests and will not continue.");
                        report.passOrFail = false;
                        report.longMsg = "Detailed opening test failed during the planar surface checks.  Without planar polygons, this test cannot be safely executed.";
                        return report;
                    }
                }
            }
            report.MessageList.Add("All test file's surfaces have polyloop descriptions that describe a plana polygon.  Planar opening test succeeded.");
            report.passOrFail = true;
            return report;

        }

        public static DOEgbXMLReportingObj GetBuildingArea(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test compares the values stored in the Building Area node of the standard and test gbXML files.";
            report.testSummary += "  This Building area is the sum total of the areas of all spaces created in gbXML.";
            report.testSummary += "  For example, if a small building has five spaces of area = 100 square feet each, then the sum of that area is";
            report.testSummary += "  5 x 100 = 500 square feet.  The building area value would be 500 square feet.";
            report.testSummary += "  We have built a tolerance in this test, meaning the building areas do not need to match perfectly in the";
            report.testSummary += " standard file and test file.  As long as your test file's value for Building Area is +/- this tolerance, the";
            report.testSummary += " test will pass.  Using the previous example, if the allowable tolerance is 1% (1% of 500 is 5 sf), then the test file may have a building area ranging from 495 to 505 square feet, and will still be declared to pass this test.";

            report.unit = Units;

            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    var node = gbXMLDocs[i].SelectSingleNode("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Area", gbXMLnsm[i]);
                    string area = node.InnerText;
                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = area;
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Convert.ToDouble(resultsArray[i]) - Convert.ToDouble(resultsArray[(i - 1)]);
                        if (Math.Abs(difference) == 0)
                        {
                            report.longMsg = "The test file's " + report.testType + "matches the standard file Building Area exactly.";
                            report.passOrFail = true;
                            return report;
                        }

                        else if (Math.Abs(difference) <= report.tolerance)
                        {
                            report.longMsg = "The test file's " + report.testType + " is within the allowable tolerance of = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The test file's " + report.testType + "  is not within the allowable tolerance of " + report.tolerance.ToString() + " " + Units + "The difference between the standard and test file is " + difference.ToString() + ".";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }
                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        public static DOEgbXMLReportingObj GetBuildingSpaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test compares the number of spaces (it counts them) in the standard and test files.  It does this";
            report.testSummary = " by counting the number of occurrences of the Space element in the gbXML files.  The number of spaces should";
            report.testSummary = " match exactly.  If you test has failed, this is because it is required that the space count match.  If the number";
            report.testSummary = " of spaces does not match, there could be a number of reasons for this, but most likely, the test file has";
            report.testSummary = " not been constructed as per the instructions provided by the gbXML Test Case Manual.";

            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", gbXMLns);
                    int nodecount = nodes.Count;
                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = " The test file's " + report.testType + " matches the standard file exactly.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = " The test file's " + report.testType + " matches the standard file " + report.testType + ", the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The test file's " + report.testType + " is not within the allowable tolerance of " + report.tolerance.ToString() + " " + Units + " The difference between the standard and test file is " + difference.ToString() + " " + Units;
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }
                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate Building " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        public static DOEgbXMLReportingObj GetBuildingStoryCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test compares the number of stories (it counts them) in the standard and test files.  It does so by";
            report.testSummary += " counting the number of occurances of a Building Storey element in the gbXML files.";
            report.testSummary += "  The number of stories should match exactly.  If your test failed, the number of stories in your file does";
            report.testSummary += " not match the standard file.  If the number of stories does not match, ";
            report.testSummary += " most likely, the test file has not been constructed as per the instructions provided by the";
            report.testSummary += " gbXML Test Case Manual.";
            report.testSummary += "  In some instances, it is not required that the number of stories match.  If you notice that the number";
            report.testSummary += " of stories do not match, but the test summary showed your file passed, then this is normal.  Refer to the pass/fail";
            report.testSummary += " summary sheet for more information.";

            report.unit = Units;

            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:BuildingStorey", gbXMLns);
                    int nodecount = nodes.Count;
                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = " The test file's " + report.testType + " matches the standard file exactly.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = " The test file's " + report.testType + " matches the standard file " + report.testType + ", the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The test file's " + report.testType + " is not within the allowable tolerance of " + report.tolerance.ToString() + " " + Units + " The difference between the standard and test file is " + difference.ToString() + " " + Units;
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }
                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg += " Failed to locate Building " + report.testType + " in the XML file.";
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            return report;
        }

        public static DOEgbXMLReportingObj GetStoryHeights(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added March 14 2013
            report.testSummary = "This test compares Z-coordinates in each one of the levels of the standard and test file.  It does so by";
            report.testSummary += " gathering the Z-coordinate of a Building Storey element's PolyLoop in the gbXML files.";
            report.testSummary += "  The z-heights should match exactly.  If this test has failed, then one of the z-heights in your file does";
            report.testSummary += " not match the standard file.  There is no tolerance for error in this test.  If any of the z-heights do not match, ";
            report.testSummary += " most likely, the test file has not been constructed as per the instructions provided by the";
            report.testSummary += " gbXML Test Case Manual.";
            report.testSummary += "  In some instances, it is not required that the z-heights match.  If you notice that this test has failed";
            report.testSummary += "  but your file overall has still passed, then this is as designed.  Refer to the pass/fail";
            report.testSummary += " summary sheet for more information.";

            report.unit = Units;

            //small dictionaries I make to keep track of the story level names and heights
            //standard file
            Dictionary<string, string> standardStoryHeight = new Dictionary<string, string>();
            //Test File
            Dictionary<string, string> testStoryHeight = new Dictionary<string, string>();
            string key = null;
            string val = null;
            string standLevel = "";

            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                try
                {
                    //assuming that this will be plenty large for now
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];
                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:BuildingStorey", gbXMLns);
                    int nodecount = nodes.Count;
                    foreach (XmlNode node in nodes)
                    {
                        XmlNodeList childNodes = node.ChildNodes;
                        foreach (XmlNode childNode in childNodes)
                        {
                            if (childNode.Name.ToString() == "Level") { key = childNode.InnerText; }
                            else if (childNode.Name.ToString() == "Name") { val = childNode.InnerText; }
                            else { continue; }
                            if (i % 2 != 0)
                            {
                                if (key != null && val != null)
                                {
                                    testStoryHeight.Add(key, val);
                                    key = null;
                                    val = null;
                                }
                            }
                            else
                            {
                                if (key != null && val != null)
                                {
                                    standardStoryHeight.Add(key, val);
                                    key = null;
                                    val = null;
                                }
                            }
                        }
                    }
                    //reporting 
                    if (i % 2 != 0)
                    {
                        if (standardStoryHeight.Count == 0)
                        {
                            report.longMsg = "Test cannot be completed.  Standard File Level Count returns Zero.";
                            report.passOrFail = false;
                            return report;
                        }
                        else if (testStoryHeight.Count == 0)
                        {
                            report.longMsg = "Test cannot be completed.  Test File Level Count returns Zero.";
                            report.passOrFail = false;
                            return report;
                        }
                        else
                        {
                            //set pass to true
                            report.passOrFail = true;
                            int count = 0;
                            foreach (KeyValuePair<string, string> standardPair2 in standardStoryHeight)
                            {
                                count++;
                                double difference;
                                StoryHeightMin = 10000;
                                string equivLevel = "";
                                if (testStoryHeight.ContainsKey(standardPair2.Key))
                                {
                                    report.MessageList.Add("Matched Standard File's " + standardPair2.Value + " with Test File's " + testStoryHeight[standardPair2.Key] + " @ " + standardPair2.Key + Units.ToString() + " Exactly");
                                    report.TestPassedDict.Add(standardPair2.Value, true);
                                    /*
                                     * ADD test results when find exact match - WX 07072020 RP-1810
                                     */
                                    report.standResult.Add(standardPair2.Key);
                                    //Because matched exactly`, we can just use standardPair2.Key to report the test value
                                    report.testResult.Add(standardPair2.Key);
                                    report.idList.Add(Convert.ToString(count));
                                    /*
                                     * END of edit
                                     */
                                    continue;
                                }
                                //this part will only be executed if there is a difference in Z level testStoryHeight does not contain the same key as standaerdPair2.
                                foreach (KeyValuePair<string, string> testPair in testStoryHeight)
                                {
                                    //setup standard result and test result
                                    report.standResult.Add(standardPair2.Key);
                                    report.testResult.Add(testPair.Key);
                                    report.idList.Add(Convert.ToString(count));

                                    difference = Math.Abs(Convert.ToDouble(standardPair2.Key) - Convert.ToDouble(testPair.Key));
                                    //store all levels and the difference between them
                                    if (StoryHeightMin > difference)
                                    {
                                        StoryHeightMin = difference;
                                        standLevel = standardPair2.Value;
                                    }
                                }
                                if (StoryHeightMin < report.tolerance)
                                {
                                    report.MessageList.Add("Matched Standard File's " + standardPair2.Value + " @ " + standardPair2.Key + Units.ToString() + " within the Tolerance allowed");
                                    report.TestPassedDict.Add(standLevel, true);
                                }
                                else
                                {
                                    report.MessageList.Add("Standard File's " + standardPair2.Value + " equivalent was not found in the test file.  The closest level in the test file was found at " + equivLevel + " in the test file.  The difference in heights was " + StoryHeightMin.ToString() + Units.ToString());
                                    report.TestPassedDict.Add(standLevel, false);
                                }

                            }
                            return report;
                        }
                    }
                }

                catch (Exception e)
                {
                    report.longMsg = e.ToString();
                    report.MessageList.Add(" Failed to locate Building " + report.testType + " in the XML file.");
                    report.passOrFail = false;
                    return report;
                }

            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        public static DOEgbXMLReportingObj TestBuildingStoryRHR(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test analyzes each of the story PolyLoop coordinates in the standard and test files.  These PolyLoop ";
            report.testSummary += "coordinates define the z-height and orientation of each story plane.  This test analyzes the normal vector ";
            report.testSummary += "created by the PolyLoop coordinates.  The PolyLoop coordinates must be sequenced in a counterclockwise manner ";
            report.testSummary += " such that when the right hand rule is applied to this sequence of coordinates, a resultant normal vector ";
            report.testSummary += " will point in the +z direction.";
            report.testSummary += "  If the PolyLoop coordinates do not form vectors that point in the +Z direction";
            report.testSummary += " (when the right hand rule is applied), then this test will fail.  It is assumed that the vectors that define";
            report.testSummary += " the story plane will be parallel to the X-Y axis.The tolerance is always zero for this test, ";
            report.testSummary += "meaning the resulting unit vector will point in the positive Z direction with no margin for error.";

            report.unit = Units;

            //stores the level's z heights
            List<string> LevelZs = new List<string>();
            //stores the list of z heights for both files
            List<List<string>> fileLevelZz = new List<List<string>>();
            //stores the RHR x product and the corresonding z height for a level
            Dictionary<string, VectorMath.Vector.CartVect> levelVct = new Dictionary<string, VectorMath.Vector.CartVect>();
            //stores a list of the RHR x product and corresponding z height for both files
            List<Dictionary<string, VectorMath.Vector.CartVect>> fileLevelVct = new List<Dictionary<string, VectorMath.Vector.CartVect>>();

            VectorMath.Vector.CartVect vector = new VectorMath.Vector.CartVect();

            int errorCount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {


                try
                {
                    //refresh
                    LevelZs.Clear();
                    levelVct.Clear();

                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];
                    //maybe it would be good if the test result spits out the name of the story?  TBD
                    XmlNodeList PlanarGeometry = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:BuildingStorey/gbXMLv5:PlanarGeometry", gbXMLns);
                    XmlNodeList PolyLoops = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:BuildingStorey/gbXMLv5:PlanarGeometry/gbXMLv5:PolyLoop", gbXMLns);
                    int nodecount = PolyLoops.Count;
                    //get the normals for each level in the Standard File
                    //get the z-coordinate for each level (we assume that the levels are going to be parallel to Z
                    LevelZs = GetLevelZs(PlanarGeometry, LevelZs);
                    foreach (string level in LevelZs)
                    {
                        //a simple attempt to filter out exceptions, which could be returned in some instances
                        if (level.Length < 10)
                        {
                            vector = GetPolyLoopXProduct(PlanarGeometry, level, report.testType);
                            string levelValue = level;
                            //if (i == 0) { levelValue += "-T"; }
                            //else { levelValue += "-S"; }
                            levelVct.Add(levelValue, vector);
                        }
                    }
                    fileLevelZz.Add(LevelZs);
                    fileLevelVct.Add(levelVct);

                    //reporting 
                    if (i % 2 != 0)
                    {
                        Dictionary<string, VectorMath.Vector.CartVect> standDict = fileLevelVct[1];
                        Dictionary<string, VectorMath.Vector.CartVect> testDict = fileLevelVct[0];
                        foreach (KeyValuePair<string, VectorMath.Vector.CartVect> pair in standDict)
                        {

                            if (testDict.ContainsKey(pair.Key))
                            {
                                report.MessageList.Add("While searching for matching building levels, there has been a Successful match.  Building Story Level " + pair.Key + " in the Standard file found a match in the Test File.");
                                report.passOrFail = true;
                                //perform cross product again of the two vectors in question.  The result should be a zero since they should be parallel
                                VectorMath.Vector.CartVect rhrTestVector = VectorMath.Vector.CrossProduct(testDict[pair.Key], standDict[pair.Key]);
                                if (rhrTestVector.X == 0 && rhrTestVector.Y == 0 && rhrTestVector.Z == 0)
                                {
                                    report.MessageList.Add("For this level match, there is Normal Vector Test Success.  The right hand rule test identified a parallel normal vector for Level " + pair.Key + " in both the Standard and Test gbXML Files.");
                                    report.passOrFail = true;
                                }
                                else
                                {
                                    VectorMath.Vector.CartVect rhrTestVectorU = VectorMath.Vector.UnitVector(rhrTestVector);
                                    //create a test to determine the angular difference between the two vectors is within tolerance
                                    //|A||B|cos theta = A x B

                                    //if the angle is within the allowable tolerance, then pass the test with a note that the vectors
                                    //were not parallel
                                }

                            }
                            else
                            {
                                report.MessageList.Add("The right hand rule test for Level " + pair.Key + " in the Standard File could not be completed.  A match for this level could not be found in the test file.");
                                report.passOrFail = false;
                                errorCount++;
                            }
                        }

                    }
                    else { continue; }

                    //need to comapre and have if then statement depending on the outcome of the accuracy tests
                    if (errorCount == 0)
                    {
                        report.longMsg = "Test Success:  Building Stories RHR in the Test File match the RHR in the Standard File for all Levels.";
                    }
                    else
                    {
                        report.longMsg = "Not all levels in the Standard File found equivalent levels and normal vectors in the Test File.";
                    }
                    return report;
                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to complete RHR Test for the Building Storey Nodes.  Exception noted.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            return report;
        }

        //this is a support function used by the GetLevelHeights function above.  It is not directly,
        //iteslf, a test
        private static List<string> GetLevelZs(XmlNodeList PlanarGeometry, List<string> LevelZs)
        {
            string result = "";
            int polyLoopCount = 0;
            try
            {
                int nodecount = PlanarGeometry.Count;
                VectorMath.Vector.CartCoord[] vCoords = new VectorMath.Vector.CartCoord[nodecount];
                foreach (XmlNode PolyLoops in PlanarGeometry)
                {
                    //gathers all the cartesian points in a given polyloop
                    foreach (XmlNode cartesianPoints in PolyLoops)
                    {

                        //test the polyloop RHR convention
                        //count the total number of cartesian coordinates
                        int coordcount = cartesianPoints.ChildNodes.Count;
                        //I may want to test the number of coordinates to make sure it matches
                        //I do want to ensure I have a minimum number of coords
                        if (coordcount < minPlanePoints)
                        {
                            result += "Insufficient number of cartesian points to define a plane";
                            LevelZs.Add(result);
                            return LevelZs;
                        }
                        else
                        {
                            int cartPtCount = 0;
                            //gets a set of XYZ coordinates, one at a time
                            foreach (XmlNode coordinates in cartesianPoints.ChildNodes)
                            {
                                //I will only test one Z-coordinate in each set of coordinates
                                if (cartPtCount < 1)
                                {
                                    VectorMath.Vector.CartCoord vC = new VectorMath.Vector.CartCoord();
                                    vCoords[polyLoopCount] = vC;
                                }
                                else { break; }

                                int crdCount = 1;
                                //gets each coordinate one at a time
                                foreach (XmlNode coordinate in coordinates.ChildNodes)
                                {
                                    double coord = Convert.ToDouble(coordinate.InnerText);
                                    switch (crdCount)
                                    {
                                        case 1:
                                            vCoords[polyLoopCount].X = coord;
                                            break;
                                        case 2:
                                            vCoords[polyLoopCount].Y = coord;
                                            break;
                                        case 3:
                                            vCoords[polyLoopCount].Z = coord;
                                            break;
                                        default:
                                            break;
                                    }
                                    crdCount++;
                                }
                                cartPtCount++;
                            }
                        }

                    }
                    polyLoopCount++;
                }
                //create the List that holds the z-values of each level
                for (int z = 0; z < nodecount; z++)
                {
                    LevelZs.Add(vCoords[z].Z.ToString());
                }

                return LevelZs;
            }

            catch (Exception e)
            {
                result += e.ToString();
                LevelZs.Add(result);
                return LevelZs;
            }
        }
        //this is a simple way to get the polyLoop X product.
        //this is a support function used by the Function TestBuildingStory RHR above
        //This X Product routine is the first attempt to produce a X product from coordinates  Since the coordinates used to define
        //a level plane never create an irregular polygon, this scheme worked.  
        //it will only assuredly work properly for a triangle, square, or rectangle.  Shapes other than these should use subsequent XProduct
        //functions as created below.
        //Created by CHarriman, Senior Product Manager Carmel Software
        //Nov 2012
        public static VectorMath.Vector.CartVect GetPolyLoopXProduct(XmlNodeList PlanarGeometry, string level, TestType testType)
        {
            int cartPtCount = 0;
            VectorMath.Vector.CartVect xProd = new VectorMath.Vector.CartVect();
            //gathers all the cartesian points in a given polyloop
            int nodecount = PlanarGeometry.Count;
            VectorMath.Vector.CartCoord[] vCoords = new VectorMath.Vector.CartCoord[3];
            foreach (XmlNode PolyLoops in PlanarGeometry)
            {
                foreach (XmlNode cartesianPoints in PolyLoops)
                {

                    //test the polyloop RHR convention
                    //count the total number of cartesian coordinates
                    int coordcount = cartesianPoints.ChildNodes.Count;
                    //I may want to test the number of coordinates to make sure it matches, or if it has a minimum number of coords
                    if (coordcount < minPlanePoints)
                    {
                        //result += "Insufficient number of cartesian points to define a plane";
                        return xProd;
                    }
                    else
                    {
                        cartPtCount = 0;
                        //gets a set of XYZ coordinates, one at a time
                        foreach (XmlNode coordinates in cartesianPoints.ChildNodes)
                        {
                            if (cartPtCount < 3)
                            {
                                VectorMath.Vector.CartCoord vC = new VectorMath.Vector.CartCoord();
                                vCoords[cartPtCount] = vC;
                            }
                            else { break; }

                            int crdCount = 1;
                            //gets each coordinate one at a time
                            //filtering through the inner children of the PolyLoop
                            foreach (XmlNode coordinate in coordinates.ChildNodes)
                            {
                                double coord = Convert.ToDouble(coordinate.InnerText);
                                switch (crdCount)
                                {
                                    case 1:
                                        vCoords[cartPtCount].X = coord;
                                        break;
                                    case 2:
                                        vCoords[cartPtCount].Y = coord;
                                        break;
                                    case 3:
                                        vCoords[cartPtCount].Z = coord;
                                        break;
                                    default:
                                        break;
                                }
                                if (vCoords[cartPtCount].Z.ToString() == level) { break; };
                                crdCount++;
                            }

                            cartPtCount++;
                        }

                    }
                }
                if (vCoords[(cartPtCount - 1)].Z.ToString() == level) { break; }
            }
            //Get the Cross Product
            VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(vCoords[0], vCoords[1]);
            VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(vCoords[1], vCoords[2]);
            xProd = VectorMath.Vector.CrossProduct(v1, v2);
            xProd = Vector.UnitVector(xProd);
            return xProd;

        }

        //this test was originally invented for the case where the proposed and test cases did not have to be identical
        //it was designed simply to ensure that only the TEST file had unique SpaceId values.
        //Created by CHarriman Senior Product Manager Carmel Software 
        //Nov 2012
        public static DOEgbXMLReportingObj UniqueSpaceIdTest(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test reviews the test file's Space id values, and ensures that they are all unique.  If there are any duplicate Space id values, then this test will fail.  If there are duplicates, the remainder of the tests in the testbed are not executed and the test will end here until the test file is properly updated.  Each Space id must be unique for the test bed to successfully execute.  If you have failed this test, please review the documents for this test and resubmit the test.";

            report.MessageList = new List<string>();
            report.TestPassedDict = new Dictionary<string, bool>();
            List<string> standardIdList = new List<string>();
            List<string> testIDList = new List<string>();
            report.standResult = new List<string>();
            report.testResult = new List<string>();
            report.idList = new List<string>();
            // report.testType = "UniqueId";
            try
            {
                for (int i = 0; i < gbXMLDocs.Count; i++)
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", gbXMLns);
                    foreach (XmlNode node in nodes)
                    {
                        //looks to see if the spaceId is already included in the list of space IDs being generated
                        string spaceId;
                        if (i % 2 != 0)
                        {
                            spaceId = node.Attributes[0].Value.ToString();
                            testIDList.Add(spaceId);
                        }
                        else
                        {
                            spaceId = node.Attributes[0].Value.ToString();
                            standardIdList.Add(spaceId);
                        }
                    }
                }
                //now that I have all of the spaceIds, I will loop through and make sure I have perfect matches
                //the order of the spaces is not enforced
                //create a list that holds the index of the standardIdList when a match is found
                //the list should be the same length as standardIdlist and each value should be unique
                List<int> indexFound = new List<int>();
                for (int j = 0; j < standardIdList.Count; j++)
                {
                    string standardId = standardIdList[j];
                    foreach (string testspaceId in testIDList)
                    {
                        if (testspaceId == standardId)
                        {
                            indexFound.Add(j);
                            report.MessageList.Add("The standard file space id: " + standardId + "has found a spaceId match in the test file.");

                        }
                    }
                }
                //search the list to make sure that it is unique and has the proper count
                if (indexFound.Count == standardIdList.Count)
                {
                    report.MessageList.Add("The standard file has found a match only once in the test file.  All spaceIds have been matched.");
                    report.passOrFail = true;
                    report.longMsg = "SpaceId Match test has passed.";
                    return report;
                }
                else
                {
                    report.passOrFail = false;
                    string index = "";
                    foreach (int p in indexFound)
                    {
                        index += p.ToString() + ", ";
                    }
                    report.MessageList.Add(index);
                    report.longMsg = "SpaceId Match test has failed.";
                    return report;
                }

                //if (standardIdList.Contains(spaceId))
                //{
                //    report.testResult.Add("Not Unique");

                //    report.longMsg = "Unique SpaceID Test Failed.  " + spaceId + " is already included once in the test file.";
                //    report.passOrFail = false;
                //    report.TestPassedDict[spaceId] = false;
                //    return report;
                //}
                //else
                //{
                //    report.testResult.Add("Is Unique");

                //    spaceId = node.Attributes[0].Value.ToString();
                //    standardIdList.Add(spaceId);
                //    report.passOrFail = true;
                //    report.TestPassedDict.Add(spaceId, true);
                //    report.MessageList.Add(spaceId + " is unique.");
                //}
            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.passOrFail = false;
                return report;
            }
            return report;
        }

        public static DOEgbXMLReportingObj TestSpaceAreas(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary = "This test compares the square footage of spaces in the test and standard files.  It does this by searching";
            report.testSummary += "for a unique Space id in both the test and standard files, and finding a match.  Once a match is found, it then";
            report.testSummary += " finds the square footage reported for the Space area, and compares them to ensure they are the same or";
            report.testSummary += " within tolerance. For example, if the standard file has a Space with id = \"Space-1\" with an area of";
            report.testSummary += "250 square feet, then this test searches through the test file for a Space with the identical id.";
            report.testSummary += "  Once this space has been located, the test then compares the Area to 250 square feet.  ";
            report.testSummary += "If they are identical, the test is done, and passes.  We have built a tolerance in this test, meaning the";
            report.testSummary += " areas do not need to match perfectly in the standard file and test file.  As long as your test file's value";
            report.testSummary += " for Space Area is +/- this tolerance, the test will pass.  Using the previous example, if the allowable";
            report.testSummary += " tolerance is 1% (1% of 250 is 2.5 sf), then the test file may have a space area ranging from 247.5 to 252.5";
            report.testSummary += " square feet, and the test will still delcare \"Pass\".";


            report.unit = Units;
            report.passOrFail = true;
            string spaceId = "";
            //assuming that this will be plenty large for now
            Dictionary<string, double> standardFileAreaDict = new Dictionary<string, double>();
            Dictionary<string, double> testFileAreaDict = new Dictionary<string, double>();

            try
            {
                for (int i = 0; i < gbXMLDocs.Count; i++)
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList spaceNodes = gbXMLDocs[i].SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space/gbXMLv5:Area", gbXMLnsm[i]);
                    //make lists of the areas in each project
                    foreach (XmlNode spaceNode in spaceNodes)
                    {
                        string area = spaceNode.InnerText;
                        if (i % 2 != 0)
                        {
                            spaceId = spaceNode.ParentNode.Attributes[0].Value;
                            testFileAreaDict.Add(spaceId, Convert.ToDouble(area));
                        }
                        else
                        {
                            spaceId = spaceNode.ParentNode.Attributes[0].Value;
                            standardFileAreaDict.Add(spaceId, Convert.ToDouble(area));
                        }
                    }
                }
                var standardKeys = standardFileAreaDict.Keys;

                foreach (string key in standardKeys)
                {
                    if (testFileAreaDict.ContainsKey(key))
                    {
                        double testFileSpaceArea = testFileAreaDict[key];
                        double standardFileSpaceArea = standardFileAreaDict[key];


                        report.standResult.Add(Convert.ToString(standardFileSpaceArea));
                        report.testResult.Add(Convert.ToString(testFileSpaceArea));
                        report.idList.Add(key);

                        double difference = Math.Abs(testFileSpaceArea - standardFileSpaceArea);
                        if (difference == 0)
                        {
                            report.MessageList.Add("For Space Id: " + key + ".  Success finding matching space area.  The Standard File and the Test File both have a space with an area = " + testFileSpaceArea.ToString() + " " + Units + ". ");
                            report.TestPassedDict.Add(key, true);
                        }
                        else if (difference < report.tolerance)
                        {
                            report.MessageList.Add("For Space Id: " + key + ".  Success finding matching space area.  The Standard File space area of " + standardFileSpaceArea.ToString() + " and the Test File space area of " + testFileSpaceArea.ToString() + " " + Units + " is within the allowable tolerance of " + report.tolerance.ToString() + " " + Units);
                            report.TestPassedDict.Add(key, true);
                        }
                        else
                        {
                            report.MessageList.Add("For space Id: " + key + ".  Failure to find an space area match.  THe area equal to  = " + standardFileSpaceArea.ToString() + " " + Units + " in the Standard File could not be found in the Test File. ");
                            report.TestPassedDict.Add(key, false);
                        }
                    }
                    else
                    {
                        report.standResult.Add("---");
                        report.testResult.Add("Could not be matched");
                        report.idList.Add(key);
                        //failure to match spaceIds
                        report.MessageList.Add("Test File and Standard File space names could not be matched.  SpaceId: " + key + " could not be found in the test file.");
                        report.passOrFail = false;
                        return report;
                    }
                }
                return report;
            }

            catch (Exception e)
            {
                report.MessageList.Add(e.ToString());
                report.longMsg = "Failed to complete the " + report.testType + ".  See exceptions noted.";
                report.passOrFail = false;
                return report;
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        public static DOEgbXMLReportingObj TestSpaceVolumes(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.passOrFail = true;
            string spaceId = "";
            report.unit = Units;
            //assuming that this will be plenty large for now
            Dictionary<string, double> standardFileVolumeDict = new Dictionary<string, double>();
            Dictionary<string, double> testFileVolumeDict = new Dictionary<string, double>();

            try
            {
                for (int i = 0; i < gbXMLDocs.Count; i++)
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList spaceNodes = gbXMLDocs[i].SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space/gbXMLv5:Volume", gbXMLnsm[i]);
                    //make lists of the areas in each project
                    foreach (XmlNode spaceNode in spaceNodes)
                    {
                        string volume = spaceNode.InnerText;
                        if (i % 2 != 0)
                        {
                            spaceId = spaceNode.ParentNode.Attributes[0].Value;
                            testFileVolumeDict.Add(spaceId, Convert.ToDouble(volume));
                        }
                        else
                        {
                            spaceId = spaceNode.ParentNode.Attributes[0].Value;
                            standardFileVolumeDict.Add(spaceId, Convert.ToDouble(volume));
                        }
                    }
                }
                var standardKeys = standardFileVolumeDict.Keys;
                foreach (string key in standardKeys)
                {
                    if (testFileVolumeDict.ContainsKey(key))
                    {
                        double standardFileVolume = standardFileVolumeDict[key];
                        double testFileVolume = testFileVolumeDict[key];

                        report.standResult.Add(Convert.ToString(standardFileVolume));
                        report.testResult.Add(Convert.ToString(testFileVolume));
                        report.idList.Add(key);

                        double difference = Math.Abs(testFileVolume - standardFileVolume);
                        if (difference == 0)
                        {
                            report.MessageList.Add("For Space Id: " + key + ".  Success finding matching space volume.  The Standard and Test Files both have identical volumes: " + testFileVolume + " " + Units + "for Space Id: " + key);
                            report.TestPassedDict.Add(key, true);
                        }
                        else if (difference < report.tolerance)
                        {
                            report.MessageList.Add("For Space Id: " + key + ".  Success finding matching space volume.  The Standard Files space volume of " + standardFileVolume.ToString() + " " + Units + "and the Test File space volume: " + testFileVolume + " are within the allowed tolerance of" + report.tolerance.ToString() + " " + Units + ".");
                            report.TestPassedDict.Add(key, true);
                        }
                        else
                        {
                            //at the point of failure, the test will return with details about which volume failed.
                            report.MessageList.Add("For Space Id: " + key + ".  Failure to find a volume match.  The Volume in the Test File equal to: " + testFileVolume.ToString() + " " + Units + " was not within the allowed tolerance.  SpaceId: " + key + " in the Standard file has a volume: " + standardFileVolume.ToString() + " .");
                            report.TestPassedDict.Add(key, false);
                        }
                    }
                    else
                    {
                        report.standResult.Add("Space Id: " + key);
                        report.testResult.Add("Could not be matched");
                        report.idList.Add("");

                        //at the point of failure, the test will return with details about which volume failed.
                        report.MessageList.Add("Test File and Standard File space names could not be matched.  SpaceId: " + key + " could not be found in the test file.");
                        report.passOrFail = false;
                        return report;
                    }
                }
                return report;
            }

            catch (Exception e)
            {
                report.MessageList.Add(e.ToString());
                report.longMsg = " Failed to complete the " + report.testType + ".  See exceptions noted.";
                report.passOrFail = false;
                return report;
            }

            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        //this function was abandoned until the second phase
        //Created Dec 2012 by CHarriman Senior Product Manager Carmel Software Corp

        public static DOEgbXMLReportingObj TestShellGeomPLRHR(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            string result = "";
            string floorarea = "";
            report.unit = Units;
            List<Vector.CartVect> VectList = new List<Vector.CartVect>();

            //keeps a dictionary of the shell geometry points for each space of the test file  key = spaceId, value = List of Coordinates
            Dictionary<string, List<Vector.CartCoord>> shellGeomPtsTF = new Dictionary<string, List<Vector.CartCoord>>();
            //keeps a dictionary of the shell geometry points for each space of the standard file  key = spaceId, value = List of Coordinates
            Dictionary<string, List<Vector.CartCoord>> shellGeomPtsSF = new Dictionary<string, List<Vector.CartCoord>>();
            //keeps a dictinary of the RHR vectors of the Test file
            Dictionary<string, List<VectorMath.Vector.CartVect>> shellGeomRHRTF = new Dictionary<string, List<VectorMath.Vector.CartVect>>();
            //keeps a dictionary of the RHR vectors of the Standard file
            Dictionary<string, List<VectorMath.Vector.CartVect>> shellGeomRHRSF = new Dictionary<string, List<VectorMath.Vector.CartVect>>();

            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];
                    //for each space, gather some kind of qualifying information
                    XmlNodeList spaceNodes = gbXMLDocs[i].SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Building/gbXMLv5:Space", gbXMLns);
                    foreach (XmlNode space in spaceNodes)
                    {
                        XmlNodeList spaceChildren = space.ChildNodes;
                        foreach (XmlNode spaceChild in spaceChildren)
                        {
                            if (spaceChild.Name == "ShellGeometry")
                            {
                                XmlNode closedShell = spaceChild.FirstChild;
                                switch (i)
                                {
                                    case 0:
                                        shellGeomPtsTF = GetShellGeomPts(closedShell);
                                        break;
                                    case 1:
                                        shellGeomPtsSF = GetShellGeomPts(closedShell);
                                        break;
                                    default:
                                        break;
                                }
                                //Determine if matches found everywhere

                                //Test the two sets of data points to find matches return the coordinate matches in the same order they
                                //are presented in the gbXML file

                                switch (i)
                                {
                                    case 0:
                                        shellGeomRHRTF = GetShellGeomPolyRHR(shellGeomPtsTF);
                                        break;
                                    case 1:
                                        shellGeomRHRSF = GetShellGeomPolyRHR(shellGeomPtsSF);
                                        break;
                                    default:
                                        break;

                                }

                            }

                        }
                    }


                }
                catch (Exception e)
                {

                }
                return report;
            }
            return report;
        }
        //this is a support tool for TestShellGeomPLRHR
        public static Dictionary<string, List<VectorMath.Vector.CartCoord>> GetShellGeomPts(XmlNode closedShell)
        {
            Dictionary<string, List<VectorMath.Vector.CartCoord>> PtsDict = new Dictionary<string, List<Vector.CartCoord>>();

            string spaceId = "none";

            int cartPtCount;
            try
            {
                //get the name of the space for which this point is defined
                XmlNode spaceNode = closedShell.ParentNode;
                XmlAttributeCollection spaceAtts = spaceNode.Attributes;
                foreach (XmlAttribute at in spaceAtts)
                {
                    if (at.Name == "id")
                    {
                        spaceId = at.Value;
                        break;
                    }

                }
                //keep track of the number of polyloops in the closed shell
                int pLCount = 1;
                //store the geometry points
                foreach (XmlNode PolyLoops in closedShell)
                {
                    List<VectorMath.Vector.CartCoord> vCoords = new List<VectorMath.Vector.CartCoord>();
                    List<Vector.CartCoord> PtsList = new List<Vector.CartCoord>();
                    cartPtCount = 0;
                    foreach (XmlNode cartesianPoints in PolyLoops)
                    {
                        //reset surface area and unitRHR (this is how I know that there may be a problem 
                        //and these values are returned as points.  It is not perfect
                        Vector.CartCoord Pts = new Vector.CartCoord();
                        Pts.X = -999;
                        Pts.Y = -999;
                        Pts.Z = -999;
                        PtsList.Add(Pts);
                        int crdCount = 1;
                        //gets a set of XYZ coordinates, one at a time
                        foreach (XmlNode coordinate in cartesianPoints.ChildNodes)
                        {
                            double coord = Convert.ToDouble(coordinate.InnerText);
                            switch (crdCount)
                            {
                                case 1:

                                    PtsList[cartPtCount].X = coord;
                                    break;
                                case 2:

                                    PtsList[cartPtCount].Y = coord;
                                    break;
                                case 3:

                                    PtsList[cartPtCount].Z = coord;
                                    break;
                                default:
                                    break;


                            }
                            crdCount++;
                        }
                        cartPtCount++;
                    }
                    string spaceSurface = spaceId + "/" + pLCount.ToString();

                    PtsDict.Add(spaceSurface, PtsList);
                    pLCount++;
                    //PtsList.Clear();
                }
            }
            catch (Exception e)
            {

            }
            //I may want to test the number of coordinates to make sure it matches, or if it has a minimum number of coords
            return PtsDict;
        }

        //<Get Shell Geometry Poly Loop RHR>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //Designed to take a Dictionary <key=spaceId/SurfNum, value = List of cartesian coordinates that have been pulled 
        //from a gbXML geometry format file.
        //This list of cart coordinates are associated with a key whoese string name = spaceId+surface number
        //This key was generated in the function GetShellGeomPts
        //------------------</p>
        //Each set of points are then turned into vectors, which are then put through a cross product to determine the 
        //normal vector.  we only arbitrarily take the first three points in the list, which potentially could cause some issue.
        //This is planned to be fixed in a future release.
        //The normal vector calculated is the value in the key value pair, the key being the spaceId+surfaceNumber,
        //The Dictionary is returned with it includes a key value pair for each surface it has analyzed.
        //Therefore, if the Dictionary sent to it has 12 key value pairs, then it will return 12 key value pairs as well.
        //This is not checked for explicitly but mentioned for clarity.
        public static Dictionary<string, List<VectorMath.Vector.CartVect>> GetShellGeomPolyRHR(Dictionary<string, List<VectorMath.Vector.CartCoord>> PtsList)
        {
            //reg expressions
            string iDPatt = "(.+)[^/][0-9]";
            string numPatt = "[0-9]+";

            //initialize variables needed in this method
            VectorMath.Vector.CartVect unitRHR = new VectorMath.Vector.CartVect();
            List<Vector.CartCoord> vCoords = new List<Vector.CartCoord>();
            List<Vector.CartVect> vVect = new List<Vector.CartVect>();
            string spaceId = "none";
            string spacenum = "";
            //dictionary that will be returned by this method
            Dictionary<string, List<VectorMath.Vector.CartVect>> plRHRDict = new Dictionary<string, List<VectorMath.Vector.CartVect>>();

            //begin iterating through the Cartesian Points passed into the method (PtsList)
            for (int i = 0; i < PtsList.Count; i++)
            {
                //get the identification strings associated with each list of points in the dictionary passed to the method
                string spaceSurf = PtsList.Keys.ElementAt(i);
                foreach (Match match in Regex.Matches(spaceSurf, iDPatt))
                {
                    spaceId = match.ToString();
                }
                string spaceSurf2 = PtsList.Keys.ElementAt(i);
                foreach (Match match in Regex.Matches(spaceSurf2, numPatt))
                {
                    spacenum = match.ToString();
                }

                //take the list of coordinates and store them locally 
                //this step does not need to be taken, but it does simplify the coding a little bit.
                foreach (Vector.CartCoord coord in PtsList.Values.ElementAt(i))
                {
                    vCoords.Add(coord);
                }
                //just arbitrarily take the first 3 coordinates
                //this can lead to bad results, but is used until the next release of the software
                VectorMath.Vector.CartVect v1 = VectorMath.Vector.CreateVector(vCoords[0], vCoords[1]);
                VectorMath.Vector.CartVect v2 = VectorMath.Vector.CreateVector(vCoords[1], vCoords[2]);
                unitRHR = VectorMath.Vector.CrossProduct(v1, v2);
                unitRHR = Vector.UnitVector(unitRHR);
                vVect.Add(unitRHR);
                vCoords.Clear();

            }
            plRHRDict.Add(spaceId, vVect);
            return plRHRDict;
        }

        //<Get Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of all surface elements in a test file and standard file
        //If the number of surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements in the test and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears in both files.  If the ";
            report.testSummary += "quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test may pass even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    int nodecount = nodes.Count;
                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference;
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }
                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }
        //<Get Exterior Wall Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of surface elements whose surfaceType attribute = "ExteriorWall in a test file and standard file
        //If the number of surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetEWSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"ExteriorWall\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this SurfaceType in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test summary may show \"Pass\" even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "ExteriorWall")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " exterior wall surfaces in the Standard File and " + resultsArray[i - 1] + " exterior wall surfaces in the Test File.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }
        //<Get Interior Wall Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of surface elements whose surfaceType attribute = "InterirWall" in a test file and standard file
        //If the number of surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetIWSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"InteriorWall\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this SurfaceType in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test summary may show \"Pass\" even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;

            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "InteriorWall")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {

                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The Test File's" + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " interior wall surfaces in the standard file and " + resultsArray[i - 1] + " interior wall surfaces in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }
        //<Get Interior Floor Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of surface elements whose surfaceType attribute = "InteriorFloor" in a test file and standard file
        //If the number of surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetIFSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"InteriorFloor\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test summary may show \"Pass\" even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "InteriorFloor")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The Test File's" + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " interior floor surfaces in the standard file and " + resultsArray[i - 1] + " interior floor surfaces in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        //<Get Roof Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of surface elements whose surfaceType attribute = "Roof" in a test file and standard file
        //If the number of roof surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetRoofSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"Roof\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test summary may show \"Pass\" even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;

            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "Roof")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The Test File's" + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " roof surfaces in the standard file and " + resultsArray[i - 1] + " roof surfaces in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        //<Get Shade Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of surface elements whose surfaceType attribute = "Shade" in a test file and standard file
        //If the number of surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetShadeSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"Shade\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test summary may show \"Pass\" even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;

            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "Shade")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                            if (difference == 0)
                            {
                                report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                                report.passOrFail = true;
                                return report;
                            }
                            else if (difference <= report.tolerance)
                            {
                                report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                                report.passOrFail = true;
                                return report;
                            }
                            else
                            {
                                report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                    + ".  " + resultsArray[i] + " shading surfaces in the standard file and " + resultsArray[i - 1] + " shading surfaces in the test file.";
                                report.passOrFail = false;
                                return report;
                            }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        //<Get Air Surface Count>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //A simple method that reports the number of surface elements whose surfaceType attribute = "Air" in a test file and standard file
        //If the number of surface elements is not the same, the method returns false and displays the difference in the number of surfaces.
        public static DOEgbXMLReportingObj GetAirSurfaceCount(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test compares the total number of Surface elements with the SurfaceType=\"Air\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Surface>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the surface counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013
            report.testSummary += "  You may notice that this test has failed, but overall your file has passed.  This is because the surface";
            report.testSummary += " count may not be a perfect indicator of accuracy.  So overall, the test summary may show \"Pass\" even though this test failed.";
            report.testSummary += "  Refer to the pass/fail summary sheet for more information.";

            report.unit = Units;

            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "surfaceType")
                            {
                                string type = at.Value;
                                if (type == "Air")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference == 0)
                        {
                            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                            report.passOrFail = true;
                            return report;
                        }
                        else if (difference <= report.tolerance)
                        {
                            report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " air surfaces in the standard file and " + resultsArray[i - 1] + " air surfaces in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        //deprecated? Seems pretty redundant and no where referenced.
        public static List<SurfaceDefinitions> MakeSurfaceList(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            List<SurfaceDefinitions> surfaces = new List<SurfaceDefinitions>();
            try
            {

                XmlNodeList nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", xmlns);
                foreach (XmlNode surfaceNode in nodes)
                {
                    //initialize a new instance of the class
                    SurfaceDefinitions surfDef = new SurfaceDefinitions();
                    surfDef.AdjSpaceId = new List<string>();
                    surfDef.PlCoords = new List<Vector.MemorySafe_CartCoord>();
                    surfDef.InsertionPoint = new Vector.CartCoord();

                    //get id and surfaceType
                    XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                    foreach (XmlAttribute at in spaceAtts)
                    {
                        if (at.Name == "id")
                        {
                            surfDef.SurfaceId = at.Value;
                        }
                        else if (at.Name == "surfaceType")
                        {
                            surfDef.SurfaceType = at.Value;
                        }
                    }
                    if (surfaceNode.HasChildNodes)
                    {
                        XmlNodeList surfChildNodes = surfaceNode.ChildNodes;
                        foreach (XmlNode node in surfChildNodes)
                        {
                            if (node.Name == "AdjacentSpaceId")
                            {
                                XmlAttributeCollection adjSpaceIdAt = node.Attributes;
                                foreach (XmlAttribute at in adjSpaceIdAt)
                                {
                                    if (at.Name == "spaceIdRef")
                                    {
                                        surfDef.AdjSpaceId.Add(at.Value);
                                    }
                                }
                            }
                            else if (node.Name == "RectangularGeometry")
                            {
                                if (node.HasChildNodes)
                                {
                                    XmlNodeList rectGeomChildren = node.ChildNodes;
                                    foreach (XmlNode rgChildNode in rectGeomChildren)
                                    {
                                        if (rgChildNode.Name == "Azimuth") { surfDef.Azimuth = Convert.ToDouble(rgChildNode.InnerText); }
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
                                                            surfDef.InsertionPoint.X = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                        case 2:
                                                            surfDef.InsertionPoint.Y = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                        case 3:
                                                            surfDef.InsertionPoint.Z = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                    }
                                                    pointCount++;
                                                }
                                            }
                                        }
                                        else if (rgChildNode.Name == "Tilt") { surfDef.Tilt = Convert.ToDouble(rgChildNode.InnerText); }
                                        else if (rgChildNode.Name == "Height") { surfDef.Height = Convert.ToDouble(rgChildNode.InnerText); }
                                        else if (rgChildNode.Name == "Width") { surfDef.Width = Convert.ToDouble(rgChildNode.InnerText); }
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
                                            surfDef.PlCoords.Add(memsafecoord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Vector.MemorySafe_CartVect plRHRVect = Vector.GetMemRHR(surfDef.PlCoords);
                    surfDef.PlRHRVector = plRHRVect;
                    surfaces.Add(surfDef);
                }
                return surfaces;
            }
            catch (Exception e)
            {
                return surfaces;
            }

        }
        //<Get Surface Definitions in a gbXML file>
        //Written Jan 31, 2013 by Chien Si Harriman, Senior Product Manager, Carmel Software Corporation
        //This method will take each surface element and convert the xml language into an instance of a SurfaceDefinition
        //Each surface is converted in this way, with the resulting instance being stored in a list that is returned for later use
        //----------------------</p>
        //This is an important method because it stores all of the information about a surface in a gbXML file in a list
        //This list can later be recalled to perform analytics on the surfaces and the data contained within
        public static List<SurfaceDefinitions> GetFileSurfaceDefs(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            List<SurfaceDefinitions> surfaces = new List<SurfaceDefinitions>();
            try
            {

                XmlNodeList nodes = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface", xmlns);
                foreach (XmlNode surfaceNode in nodes)
                {
                    //initialize a new instance of the class
                    SurfaceDefinitions surfDef = new SurfaceDefinitions();
                    surfDef.AdjSpaceId = new List<string>();
                    surfDef.PlCoords = new List<Vector.MemorySafe_CartCoord>();
                    surfDef.InsertionPoint = new Vector.CartCoord();


                    //get id and surfaceType
                    XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                    foreach (XmlAttribute at in spaceAtts)
                    {
                        if (at.Name == "id")
                        {
                            surfDef.SurfaceId = at.Value;
                        }
                        else if (at.Name == "surfaceType")
                        {
                            surfDef.SurfaceType = at.Value;
                        }else if (at.Name == "constructionIdRef")
                        {
                            //RP-1810: add construction id for material testing
                            surfDef.ConstructionId = at.Value;
                        }
                    }
                    if (surfaceNode.HasChildNodes)
                    {
                        XmlNodeList surfChildNodes = surfaceNode.ChildNodes;
                        foreach (XmlNode node in surfChildNodes)
                        {
                            if (node.Name == "AdjacentSpaceId")
                            {
                                XmlAttributeCollection adjSpaceIdAt = node.Attributes;
                                foreach (XmlAttribute at in adjSpaceIdAt)
                                {
                                    if (at.Name == "spaceIdRef")
                                    {
                                        surfDef.AdjSpaceId.Add(at.Value);
                                    }
                                }
                            }
                            else if (node.Name == "RectangularGeometry")
                            {
                                if (node.HasChildNodes)
                                {
                                    XmlNodeList rectGeomChildren = node.ChildNodes;
                                    foreach (XmlNode rgChildNode in rectGeomChildren)
                                    {
                                        if (rgChildNode.Name == "Azimuth") { surfDef.Azimuth = Convert.ToDouble(rgChildNode.InnerText); }
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
                                                            surfDef.InsertionPoint.X = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                        case 2:
                                                            surfDef.InsertionPoint.Y = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                        case 3:
                                                            surfDef.InsertionPoint.Z = Convert.ToDouble(coordinate.InnerText);
                                                            break;
                                                    }
                                                    pointCount++;
                                                }
                                            }
                                        }
                                        else if (rgChildNode.Name == "Tilt") { surfDef.Tilt = Convert.ToDouble(rgChildNode.InnerText); }
                                        else if (rgChildNode.Name == "Height") { surfDef.Height = Convert.ToDouble(rgChildNode.InnerText); }
                                        else if (rgChildNode.Name == "Width") { surfDef.Width = Convert.ToDouble(rgChildNode.InnerText); }
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
                                            surfDef.PlCoords.Add(memsafecoord);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Vector.MemorySafe_CartVect plRHRVect = Vector.GetMemRHR(surfDef.PlCoords);
                    surfDef.PlRHRVector = plRHRVect;
                    surfaces.Add(surfDef);
                }
                return surfaces;
            }
            catch (Exception e)
            {
                return surfaces;
            }

        }

        private static List<DOEgbXMLConstruction> GetConstructionDefs(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            List<DOEgbXMLConstruction> constructionList = new List<DOEgbXMLConstruction>();
            Dictionary<String, DOEgbXMLLayer> layerMap = GetLayerMap(xmldoc, xmlns);
            try
            {
                XmlNodeList constructions = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Construction", xmlns);
                foreach (XmlNode constructionNode in constructions)
                {
                    //initialize a new instance of the class
                    DOEgbXMLConstruction constructionDef = new DOEgbXMLConstruction();
                    //get id attribute
                    XmlAttributeCollection constructAtts = constructionNode.Attributes;
                    foreach (XmlAttribute at in constructAtts)
                    {
                        if (at.Name == "id")
                        {
                            constructionDef.id = at.Value;
                        }
                    }

                    //get data in the child nodes
                    if (constructionNode.HasChildNodes)
                    {
                        XmlNodeList constructionChildNodes = constructionNode.ChildNodes;
                        foreach (XmlNode node in constructionChildNodes)
                        {
                            if (node.Name == "Name")
                            {
                                constructionDef.name = node.InnerText;
                            }
                            else if (node.Name == "Absorptance")
                            {
                                constructionDef.absorptance = Convert.ToDouble(node.InnerText);
                            }
                            else if (node.Name == "LayerId")
                            {
                                //Currently we assume only one layer is allowed in the construction.
                                String layerID = node.Value;
                                if (layerMap.ContainsKey(layerID))
                                {
                                    constructionDef.layer = layerMap[layerID];
                                }
                            }
                            else if (node.Name == "U-value")
                            {
                                constructionDef.uvalue = Convert.ToDouble(node.InnerText);
                            }
                            else if (node.Name == "Description")
                            {
                                constructionDef.description = node.InnerText;
                            }
                        }
                    }
                    constructionList.Add(constructionDef);
                }
                return constructionList;
            }
            catch (Exception e)
            {
                //return empty?
                return constructionList;
            }
            
        }

        //get the layer map
        private static Dictionary<String, DOEgbXMLLayer> GetLayerMap(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            XmlNodeList layers = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Layer", xmlns);
            Dictionary<String, DOEgbXMLMaterial> materialMap = GetMaterialDict(xmldoc, xmlns);
            Dictionary<String, DOEgbXMLLayer> layerMap = new Dictionary<String, DOEgbXMLLayer>();
            foreach (XmlNode layerNode in layers)
            {
                DOEgbXMLLayer layer = new DOEgbXMLLayer();
                XmlAttributeCollection layerAtts = layerNode.Attributes;
                //get attributes
                foreach (XmlAttribute at in layerAtts)
                {
                    if (at.Name == "id")
                    {
                        layer.id = at.Value;
                    }//if
                }//foreach

                //get all the childNodes;
                if (layerNode.HasChildNodes)
                {
                    XmlNodeList layerChildNodes = layerNode.ChildNodes;

                    foreach (XmlNode node in layerChildNodes)
                    {
                        // list of materials with element: MaterialId
                        //for safe, need to check exact match
                        if(node.Name == "MaterialId")
                        {
                            XmlAttributeCollection materialAttr = node.Attributes;
                            foreach (XmlAttribute att in materialAttr)
                            {
                                if(att.Name == "materialIdRef")
                                {
                                    String materialID = att.Value;
                                    //make sure material map has the key
                                    if (materialMap.ContainsKey(materialID))
                                    {
                                        layer.addMaterialToLayer(materialMap[materialID]);
                                    }//if
                                }//if
                            }//foreach
                        }//if
                    }//foreach
                }
                else
                {
                    //add warning message - layer does not have materials.
                }
                layerMap.Add(layer.id, layer);
            }

            return layerMap;
        }

        private static Dictionary<String, DOEgbXMLMaterial> GetMaterialDict(XmlDocument xmldoc, XmlNamespaceManager xmlns)
        {
            XmlNodeList materialList = xmldoc.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Material", xmlns);
            Dictionary<String, DOEgbXMLMaterial> materialMap = new Dictionary<String, DOEgbXMLMaterial>();
            foreach(XmlNode materialNode in materialList)
            {
                DOEgbXMLMaterial material = new DOEgbXMLMaterial();
                XmlAttributeCollection materialAtts = materialNode.Attributes;
                //get attributes
                foreach (XmlAttribute at in materialAtts)
                {
                    if (at.Name == "id")
                    {
                        material.id = at.Value;
                    }//if
                }//foreach
                //fill in the child data
                //get data in the child nodes
                if (materialNode.HasChildNodes)
                {
                    XmlNodeList materialChildNodes = materialNode.ChildNodes;
                    foreach(XmlNode childNode in materialChildNodes)
                    {
                        if(childNode.Name == "Name")
                        {
                            material.name = childNode.InnerText;
                        }else if(childNode.Name == "Description")
                        {
                            material.description = childNode.InnerText;
                        }else if(childNode.Name == "R-value")
                        {
                            material.rvalue = Convert.ToDouble(childNode.InnerText);
                        }else if(childNode.Name == "Thickness")
                        {
                            material.thickness = Convert.ToDouble(childNode.InnerText);
                        }else if(childNode.Name == "Conductivity")
                        {
                            material.conductivity = Convert.ToDouble(childNode.InnerText);
                        }else if(childNode.Name == "Density")
                        {
                            material.density = Convert.ToDouble(childNode.InnerText);
                        }else if(childNode.Name == "SpecificHeat")
                        {
                            material.specificheat = Convert.ToDouble(childNode.InnerText);
                        }//if
                    }//foreach
                }//if
                materialMap.Add(material.id, material);
            }
            return materialMap;
        }

        private static Vector.CartVect GetPLRHR(List<Vector.CartCoord> plCoords)
        {
            Vector.CartVect plRHRVect = new Vector.CartVect();
            //this list will store all of the rhr values returned by any arbitrary polyloop
            List<Vector.CartVect> RHRs = new List<Vector.CartVect>();

            int coordCount = plCoords.Count;
            for (int i = 0; i < coordCount - 2; i++)
            {
                Vector.CartVect v1 = Vector.CreateVector(plCoords[i], plCoords[i + 1]);
                Vector.CartVect v2 = Vector.CreateVector(plCoords[i + 1], plCoords[i + 2]);
                Vector.CartVect uv = Vector.UnitVector(Vector.CrossProduct(v1, v2));
                RHRs.Add(uv);
            }
            int RHRVectorCount = RHRs.Count;
            List<Vector.CartVect> distinctRHRs = new List<Vector.CartVect>();
            //the Distinct().ToList() routine did not work because, we believe, the item in the list is not recognized by Distinct()
            //distinctRHRs = RHRs.Distinct().ToList();
            //so we took the following approach to try and find unique vectors and store them
            distinctRHRs.Add(RHRs[0]);
            for (int j = 1; j < RHRVectorCount; j++)
            {
                foreach (Vector.CartVect distinctVector in distinctRHRs)
                {
                    //this could contain wacky RHRs that are removed below
                    if (RHRs[j].X != distinctVector.X && RHRs[j].Y != distinctVector.Y && RHRs[j].Z != distinctVector.Z)
                    {
                        distinctRHRs.Add(RHRs[j]);
                    }
                }
            }

            int RHRDistinctVectCount = distinctRHRs.Count;
            if (RHRDistinctVectCount == 1)
            {
                plRHRVect = distinctRHRs[0];
                return plRHRVect;
            }
            else
            {
                Dictionary<int, Vector.CartVect> uniqueVectorCount = new Dictionary<int, Vector.CartVect>();
                //determine which vector shows up the most often
                foreach (Vector.CartVect distinctVector in distinctRHRs)
                {
                    int count = 0;
                    foreach (Vector.CartVect vect in RHRs)
                    {
                        if (distinctVector.X == vect.X && distinctVector.Y == vect.Y && distinctVector.Z == vect.Z)
                        {
                            count++;
                        }
                    }
                    uniqueVectorCount.Add(count, distinctVector);
                }

                //returns the vector that has the largest count
                //get the largest integer in the list of 
                //may also be able to use 
                //uniqueVectorCount.Keys.Max();
                List<int> keysList = uniqueVectorCount.Keys.ToList();
                keysList.Sort();
                int max = 0;

                foreach (int key in keysList)
                {
                    if (key > max) { max = key; }
                }
                plRHRVect = uniqueVectorCount[max];
                return plRHRVect;
            }
        }

        private DOEgbXMLReportingObj GetPossibleSurfaceMatches(SurfaceDefinitions surface, List<SurfaceDefinitions> TestSurfaces, DOEgbXMLReportingObj report)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Mar 14 2013
            report.testSummary = "This test tries to match each Surface element in the standard file with an equivalent in your test file";
            report.testSummary += "  To be as flexible about what constitutes a \"Good Match\", this test finds a pool of possible candidate ";
            report.testSummary += "surfaces in your test file and then begins to eliminate them as they fail different tests.";
            report.testSummary += "  At the end, there should be only one surface candidate remaining that constitutes a good match.  ";
            report.testSummary += "You can see the result of this filtering process by reviewing the mini-report that is provided for you below.";
            report.testSummary += "</br>";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added March 14 2013
            report.testSummary += "  The search routine first tries to find all surfaces that have the same SurfaceType and adjacentSpaceIds.";
            report.testSummary += "  Everytime there is a match found in the test file, meeting these criteria, a message will appear in the ";
            report.testSummary += "mini-report, indicating that a match has been found.";
            report.testSummary += "  There may be more than one match in your test file.";
            report.testSummary += "  If there are no matches found for SurfaceType and AdjacencyId, this message will be printed (and the test will end as failed):";
            report.testSummary += "  In the test file, no matches could be found in the standard file that have the same AdjacentSpaceId(s) and SurfaceType.";
            report.testSummary += "</br>";
            report.testSummary += "  If this set of tests is successful, the routine next tries to remove those surfaces that do not meet";
            report.testSummary += " the tilt and azimuth tolerances.  Let's pretend for example that the tilt and azimuth for the standard surface";
            report.testSummary += " in question are both 90 degrees.  If the tilt and azimuth test tolerance are 1 degree, then the search";
            report.testSummary += " routine will only keep those walls that have 89<=tilt<=91 && <=89azimuth<=91 && match the SurfaceType and";
            report.testSummary += " adjacency relationships.";
            report.testSummary += " The mini-report will let you know which surfaces pass the tilt and azimuth test and which do not.";
            report.testSummary += "</br>";
            report.testSummary += "  Next the search routine takes any of the remaining surface candidates that have passed all the tests so far, ";
            report.testSummary += "and tries to determine if the Surface Areas defined by the polyLoops match to within a pre-defined % tolerance.";
            report.testSummary += "</br>";
            report.testSummary += " the final tests are to physically test the coordinates of the polyloop and insertion point to make sure";
            report.testSummary += " that a match for the standard surface can be found.";
            report.testSummary += " You should see additional messages telling you which surface in your test file matches, or doesn't match";
            report.testSummary += " the standard surface being searched against.  If there is no match, the mini-report tells you.";
            report.testSummary += "  By making the tests this way, it is hoped that you can see exactly why your test file is failing against";
            report.testSummary += " the standard file's surface definitions.";

            try
            {
                report.MessageList.Add("Standard Surface Id: " + surface.SurfaceId);
                report.MessageList.Add("</br>");
                //initialize the return list
                //alternate between these two to filter out bad matches
                List<SurfaceDefinitions> possiblesList1 = new List<SurfaceDefinitions>();
                List<SurfaceDefinitions> possiblesList2 = new List<SurfaceDefinitions>();

                bool adjSpaceIdMatch = false;
                //try to find a surface in the test file that has the same:
                //adjacent space Id signature
                //surfaceType
                //free list is 1
                //list 2 is not used
                foreach (SurfaceDefinitions testSurface in TestSurfaces)
                {
                    //has to have the same number of Adjacent Space Ids
                    if (testSurface.AdjSpaceId.Count == surface.AdjSpaceId.Count)
                    {
                        //an exception for a shading device
                        if (surface.AdjSpaceId.Count == 0) { adjSpaceIdMatch = true; }

                        foreach (string testAdjSpaceId in testSurface.AdjSpaceId)
                        {
                            //loop only returns true if all AdjSpaceIds in both surfaces are identical
                            adjSpaceIdMatch = false;
                            foreach (string standardAdjSpaceId in surface.AdjSpaceId)
                            {
                                if (testAdjSpaceId == standardAdjSpaceId) { adjSpaceIdMatch = true; }
                            }
                        }
                        //if adjacent space Ids match and the surface types match, note this 
                        if (adjSpaceIdMatch && testSurface.SurfaceType == surface.SurfaceType)
                        {
                            report.MessageList.Add("AdjancentSpaceId(s) and surfaceType Match.");
                            report.MessageList.Add("Surface id: " + testSurface.SurfaceId + " is a candidate.");
                            report.MessageList.Add("</br>");
                            possiblesList1.Add(testSurface);
                        }
                    }
                }
                if (possiblesList1.Count == 1)
                {
                    report.MessageList.Add("Based on a comparison of the surface Type and Adjacent SpaceIds, there is " + possiblesList1.Count.ToString() + " surface in the test file that is a possible match for " + surface.SurfaceId + " of the Standard File.");
                    report.MessageList.Add("<br/>");
                }
                else if (possiblesList1.Count > 1)
                {
                    report.MessageList.Add("Based on a comparison of the surface Type and Adjacent SpaceIds, there are " + possiblesList1.Count.ToString() + " surface in the test file that are possible matches for " + surface.SurfaceId + " of the Standard File.");
                    report.MessageList.Add("<br/>");
                }
                else
                {
                    report.longMsg = "In the test file, no matches could be found in the standard file that have the same AdjacentSpaceId(s) and SurfaceType.";
                    report.passOrFail = false;
                    return report;
                }
                //begin to filter back this list
                //tilt
                //azimuth
                //list 1 is analyzed
                //list 2 is free

                if (possiblesList1.Count > 0)
                {
                    foreach (SurfaceDefinitions testSurface in possiblesList1)
                    {
                        double tiltDifference = Math.Abs(testSurface.Tilt - surface.Tilt);
                        double azimuthDifference = Math.Abs(testSurface.Azimuth - surface.Azimuth);
                        //if the tilt and azimuth is outside of tolerance
                        if (tiltDifference > DOEgbXMLBasics.Tolerances.SurfaceTiltTolerance || azimuthDifference > DOEgbXMLBasics.Tolerances.SurfaceAzimuthTolerance)
                        {
                            report.MessageList.Add("Test file's Surface id: " + testSurface.SurfaceId + " azimuth and tilt match FAILED: ");
                            report.MessageList.Add("Test file surface (Azimuth, Tilt): (" + testSurface.Azimuth.ToString() + "," + testSurface.Tilt.ToString() + ")");
                            report.MessageList.Add("Standard file surface (Azimuth, Tilt): (" + surface.Azimuth.ToString() + "," + surface.Tilt.ToString());
                            report.MessageList.Add(testSurface.SurfaceId + " has been removed as a candidate for matching.");
                            report.MessageList.Add("</br>");
                            continue;
                        }
                        //if the tilt and azimuth is within tolerance
                        else
                        {
                            //add to the free List
                            possiblesList2.Add(testSurface);
                            if (tiltDifference == 0 && azimuthDifference == 0)
                            {
                                report.MessageList.Add("Test surface with id: " + testSurface.SurfaceId + " matches the standard surface tilt and azimuth exactly.");
                                report.MessageList.Add("Test file surface (Azimuth, Tilt): (" + testSurface.Azimuth.ToString() + "," + testSurface.Tilt.ToString() + ")");
                                report.MessageList.Add("Standard file surface (Azimuth, Tilt): (" + surface.Azimuth.ToString() + "," + surface.Tilt.ToString());

                                report.MessageList.Add("</br>");
                            }
                            else
                            {
                                report.MessageList.Add("Test surface with id: " + testSurface.SurfaceId + " is within the azimuth and tilt tolerances of " + DOEgbXMLBasics.Tolerances.SurfaceAzimuthTolerance + " and " + DOEgbXMLBasics.Tolerances.SurfaceTiltTolerance + ", respectively.  It matches the standard file surface within the allowable tolerance.");
                                report.MessageList.Add("Test file surface (Azimuth, Tilt): (" + testSurface.Azimuth.ToString() + "," + testSurface.Tilt.ToString() + ")");
                                report.MessageList.Add("Standard file surface (Azimuth, Tilt): (" + surface.Azimuth.ToString() + "," + surface.Tilt.ToString());

                                report.MessageList.Add("</br>");
                            }
                        }
                    }
                }
                // report to the user that no matches could be found
                else
                {
                    report.longMsg = "In the test file, surfaces could be found that match the standard file's AdjacentSpaceId and SurfaceType, but of these matches, none could be identified that also have a tilt or azimuth that exactly matches the standard file's, or is within the allowable tolerance.";
                    report.passOrFail = false;
                    return report;
                }
                //clear the first list
                possiblesList1.Clear();
                //start to loop through the new refined list
                //generally want to look at the polyLoop coordinates
                //list 2 is analyzed
                //list 1 is free
                report.MessageList.Add("Starting Surface Area Match tests......");
                report.MessageList.Add("</br>");
                if (possiblesList2.Count > 0)
                {
                    //simple method from this point forward is just to simply start doing a polyloop check
                    //check the standard surface PolyLoop and the test Surface(s) polyloop(s)
                    //check the absolute coordinates of the testSurface(s) polyloop(s)

                    if (possiblesList2.Count == 1)
                    {
                        report.MessageList.Add("Only one Surface Candidate remaining from the original test pool.");
                        report.MessageList.Add("<br/>");
                        //meaning there is only one candidate still available
                        //go on to test the polyLoop coordinates and the insertion point
                        possiblesList1.Add(possiblesList2[0]);


                    }
                    //more than one candidate still exists even after the adjacency test, surfaceType test, and tilt and azimuth tests, so filter through
                    else
                    {
                        //The user should be able to determine, based on output which surfaces are left for consideration
                        //Option 1:  (easiest) find the one best candidate
                        //do so based on an area match, matching the area of the test surface with the area of the test surface 
                        //(without regard for absolute polyloop coordinates)

                        //We find the area using area formulas for both regular polygons and irregular polygons
                        //first we check for the type of surface that it is (regular polygon or not), and we then take it from there
                        //in the case of a rectangular polygon, we only count rectangles or squares as regular, everything else is 
                        //assumed to be irregular, though this does not fit the classic definition of a classic polygon.  
                        //The language is just semantics

                        //first try to find if the standard file has a regular rectangular or square profile
                        report.MessageList.Add("Checking if the surface is a square or rectangle.");
                        bool isRegular = IsSurfaceRegular(surface);
                        foreach (SurfaceDefinitions regSurface in possiblesList2)
                        {
                            //ensures likewise that all the test surface candidates are regular, 
                            //if they are not, then the entire set is assumed to be irregular
                            isRegular = IsSurfaceRegular(regSurface);
                            if (isRegular == false) break;
                        }
                        if (isRegular)
                        {
                            //we take a shortcut and use the width and height as a way to simplify the area checking scheme
                            //we assume that the width and height are properly entered in this simplified case
                            report.MessageList.Add("Rectangle or Square = TRUE");
                            report.MessageList.Add("Comparisons of the Width and Height values will be used as a proxy for surface Area.");
                            foreach (SurfaceDefinitions testsurface in possiblesList2)
                            {
                                //it first analyzes the test file to see if slivers are present.  If they are, it will fail the test
                                //if slivers are not allowed for the test.  This is the first time we check for slivers
                                if (testsurface.Width <= DOEgbXMLBasics.Tolerances.SliverDimensionTolerance || testsurface.Height <= DOEgbXMLBasics.Tolerances.SliverDimensionTolerance)
                                {
                                    if (!DOEgbXMLBasics.SliversAllowed)
                                    {
                                        report.MessageList.Add("This test does not allow slivers less than " + DOEgbXMLBasics.Tolerances.SliverDimensionTolerance + " ft.  A sliver has been detected.  Test surface id: " + testsurface.SurfaceId + " is a sliver.");
                                        report.passOrFail = false;
                                        return report;
                                    }
                                }
                                //otherwise, if the sliver test passes
                                double widthDiff = Math.Abs(testsurface.Width - surface.Width);
                                double heightDiff = Math.Abs(testsurface.Height - surface.Height);
                                if (widthDiff > DOEgbXMLBasics.Tolerances.SurfaceWidthTolerance ||
                                    heightDiff > DOEgbXMLBasics.Tolerances.SurfaceHeightTolerance)
                                {
                                    report.MessageList.Add("Test file's Surface id: " + testsurface.SurfaceId + " width and height do not both match the standard file surface id: " + surface.SurfaceId + ".  This surface has been removed as a candidate.");
                                    continue;
                                }
                                else
                                {
                                    //this surface is a candidate
                                    possiblesList1.Add(testsurface);
                                    if (widthDiff == 0 && heightDiff == 0)
                                    {
                                        report.MessageList.Add("Test file surface with id: " + testsurface.SurfaceId + " matches the width and height exactly of the standard file.");
                                        //go ahead and now check the polyLoop coordinates, and then the insertion point
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test file surface with id: " + testsurface.SurfaceId + " is within the width and height tolerances of " + DOEgbXMLBasics.Tolerances.SurfaceWidthTolerance + " ft and " + DOEgbXMLBasics.Tolerances.SurfaceHeightTolerance + "ft, respectively.");
                                        //go ahead and now check the polyloop coordinates, and then the insertion point
                                    }
                                }
                            }
                        }
                        //It is not "regular".  Find the one surface with the area that most closely matches, and then check its polyloops
                        //1. get the polyloop area of the standard file's surface polyloops
                        //2. get the area of the test file surface candidates using the polyloop coordinates
                        else
                        {
                            report.MessageList.Add("The surface is not a square or rectangle.");
                            report.MessageList.Add("PolyLoop coordinates will be used to calculate the area.");
                            //there are two basic cases, one where we get the area using greens theorem when the surface is parallel
                            //to one of the axes of the project global reference frame
                            //and the second where the surface is not parallel to one of the axes of the global reference frame
                            //Surface normal Parallel to global reference frame X Axis
                            if (Math.Abs(surface.PlRHRVector.X) == 1 && surface.PlRHRVector.Y == 0 && surface.PlRHRVector.Z == 0)
                            {
                                List<Vector.MemorySafe_CartCoord> coordList = new List<Vector.MemorySafe_CartCoord>();
                                foreach (Vector.MemorySafe_CartCoord coord in surface.PlCoords)
                                {
                                    //only take the Y and Z coordinates and throw out the X because we can assume that they are all the same
                                    Vector.MemorySafe_CartCoord simplecoord = new Vector.MemorySafe_CartCoord(0,coord.Y,coord.Z);
                                    coordList.Add(simplecoord);

                                }
                                double area = GetAreaFrom2DPolyLoop(coordList);
                                if (area == -999)
                                {
                                    report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                                    report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                    report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");

                                }
                                double testSurfacesArea = 0;

                                foreach (SurfaceDefinitions testSurface in possiblesList2)
                                {
                                    if (Math.Abs(testSurface.PlRHRVector.X) == 1 && testSurface.PlRHRVector.Y == 0 &&
                                        testSurface.PlRHRVector.Z == 0)
                                    {
                                        List<Vector.MemorySafe_CartCoord> testCoordList = new List<Vector.MemorySafe_CartCoord>();
                                        foreach (Vector.MemorySafe_CartCoord coord in testSurface.PlCoords)
                                        {
                                            Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(0, coord.Y, coord.Z);
                                            testCoordList.Add(simple);
                                        }
                                        testSurfacesArea = GetAreaFrom2DPolyLoop(testCoordList);
                                        if (testSurfacesArea == -999)
                                        {
                                            report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                        }
                                        double difference = Math.Abs(area - testSurfacesArea);
                                        if (difference < area * DOEgbXMLBasics.Tolerances.SurfaceAreaPercentageTolerance)
                                        {
                                            possiblesList1.Add(testSurface);
                                            if (difference == 0)
                                            {
                                                //then it perfectly matches, go on to check the poly loop coordinates
                                                //then check the insertion point
                                                report.MessageList.Add("The test surface: " + testSurface.SurfaceId + " polyloop surface area matches the polyLoop surface area of the standard surface: " + surface.SurfaceId + " exactly.");
                                            }
                                            else
                                            {
                                                report.MessageList.Add("The test surface: " + testSurface.SurfaceId + " polyloop surface area matches the polyLoop surface area of the standard surface: " + surface.SurfaceId + " within the allowable area percentage tolerance.");
                                            }
                                        }
                                        else
                                        {
                                            report.MessageList.Add("The test surface cannot find a match for its surface area as defined in the polyLoop coordinates");
                                            //don't return here, it will be returned below
                                        }
                                    }
                                    else
                                    {
                                        //do nothing, it will be handled by the more general case and then translated to a 2-D surface
                                    }
                                }


                            }
                            //Surface normal Parallel to global reference frame y Axis
                            else if (surface.PlRHRVector.X == 0 && Math.Abs(surface.PlRHRVector.Y) == 1 && surface.PlRHRVector.Z == 0)
                            {
                                List<Vector.MemorySafe_CartCoord> coordList = new List<Vector.MemorySafe_CartCoord>();
                                foreach (Vector.MemorySafe_CartCoord coord in surface.PlCoords)
                                {
                                    //only take the X and Z coordinates and throw out the Y because we can assume that they are all the same
                                    Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, 0, coord.Z);
                                    coordList.Add(simple);

                                }
                                double area = GetAreaFrom2DPolyLoop(coordList);
                                if (area == -999)
                                {
                                    report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                                    report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                    report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");

                                }
                                double testSurfacesArea = 0;

                                foreach (SurfaceDefinitions testSurface in possiblesList2)
                                {
                                    if (Math.Abs(testSurface.PlRHRVector.X) == 0 && Math.Abs(testSurface.PlRHRVector.Y) == 1 && testSurface.PlRHRVector.Z == 0)
                                    {
                                        List<Vector.MemorySafe_CartCoord> testCoordList = new List<Vector.MemorySafe_CartCoord>();
                                        foreach (Vector.MemorySafe_CartCoord coord in testSurface.PlCoords)
                                        {
                                            Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, 0, coord.Z);
                                            testCoordList.Add(simple);
                                        }
                                        testSurfacesArea = GetAreaFrom2DPolyLoop(testCoordList);
                                        if (testSurfacesArea == -999)
                                        {
                                            report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                        }
                                        double difference = Math.Abs(area - testSurfacesArea);
                                        if (difference < area * DOEgbXMLBasics.Tolerances.SurfaceAreaPercentageTolerance)
                                        {
                                            possiblesList1.Add(testSurface);
                                            if (difference == 0)
                                            {
                                                //then it perfectly matches, go on to check the poly loop coordinates
                                                //then check the insertion point
                                                report.MessageList.Add("The test surface: " + testSurface.SurfaceId + " polyloop surface area matches the polyLoop surface area of the standard surface: " + surface.SurfaceId + " exactly.");
                                            }
                                            else
                                            {
                                                report.MessageList.Add("The test surface: " + testSurface.SurfaceId + " polyloop surface area matches the polyLoop surface area of the standard surface: " + surface.SurfaceId + " within the allowable area percentage tolerance.");
                                            }
                                        }
                                        else
                                        {
                                            report.MessageList.Add("The test surface cannot find a match for its surface area as defined in the polyLoop coordinates");
                                            //don't return here, it will be returned below
                                        }
                                    }
                                    else
                                    {
                                        //do nothing, it will be handled by the more general code below and translated to 2D
                                    }
                                }
                            }
                            else if (surface.PlRHRVector.X == 0 && surface.PlRHRVector.Y == 0 && Math.Abs(surface.PlRHRVector.Z) == 1)
                            {
                                List<Vector.MemorySafe_CartCoord> coordList = new List<Vector.MemorySafe_CartCoord>();
                                foreach (Vector.MemorySafe_CartCoord coord in surface.PlCoords)
                                {
                                    //only take the X and Y coordinates and throw out the Z because we can assume that they are all the same
                                    Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, coord.Y, 0);
                                    coordList.Add(coord);

                                }
                                double area = GetAreaFrom2DPolyLoop(coordList);
                                if (area == -999)
                                {
                                    report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                                    report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                    report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");

                                }
                                double testSurfacesArea = 0;

                                foreach (SurfaceDefinitions testSurface in possiblesList2)
                                {
                                    if (Math.Abs(testSurface.PlRHRVector.X) == 0 && testSurface.PlRHRVector.Y == 0 && Math.Abs(testSurface.PlRHRVector.Z) == 1)
                                    {
                                        List<Vector.MemorySafe_CartCoord> testCoordList = new List<Vector.MemorySafe_CartCoord>();
                                        foreach (Vector.MemorySafe_CartCoord coord in testSurface.PlCoords)
                                        {
                                            Vector.MemorySafe_CartCoord simple = new Vector.MemorySafe_CartCoord(coord.X, coord.Y, 0);
                                            testCoordList.Add(coord);
                                        }
                                        testSurfacesArea = GetAreaFrom2DPolyLoop(testCoordList);
                                        if (testSurfacesArea == -999)
                                        {
                                            report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                            report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                        }
                                        double difference = Math.Abs(area - testSurfacesArea);
                                        if (difference < area * DOEgbXMLBasics.Tolerances.SurfaceAreaPercentageTolerance)
                                        {
                                            possiblesList1.Add(testSurface);
                                            if (difference == 0)
                                            {
                                                //then it perfectly matches, go on to check the poly loop coordinates
                                                //then check the insertion point
                                                report.MessageList.Add("The test surface: " + testSurface.SurfaceId + " polyloop surface area matches the polyLoop surface area of the standard surface: " + surface.SurfaceId + " exactly.");
                                            }
                                            else
                                            {
                                                report.MessageList.Add("The test surface: " + testSurface.SurfaceId + " polyloop surface area matches the polyLoop surface area of the standard surface: " + surface.SurfaceId + " within the allowable area percentage tolerance.");
                                            }
                                        }
                                        else
                                        {
                                            report.MessageList.Add("The test surface cannot find a match for its surface area as defined in the polyLoop coordinates");
                                            //don't return here, it will be returned below
                                        }
                                    }
                                    else
                                    {
                                        //do nothing.  The code below will handle the more general case where it is not aligned with reference frame axes
                                    }
                                }
                            }
                            //the surface is not aligned with one of the reference frame axes, which requires a bit more work to determine the right answer.
                            else
                            {
                                report.MessageList.Add("The standard surface is not aligned along an axis, and will be rotated into a new coordinate frame");
                                //New Z Axis for this plane is the normal vector, does not need to be created
                                //Get New Y Axis which is the surface Normal Vector cross the original global reference X unit vector (all unit vectors please
                                Vector.CartVect tempY = new Vector.CartVect();
                                Vector.MemorySafe_CartVect globalReferenceX = new Vector.MemorySafe_CartVect(1,0,0);


                                tempY = Vector.CrossProductNVRetMSMS(surface.PlRHRVector, globalReferenceX);
                                tempY = Vector.UnitVector(tempY);
                                Vector.MemorySafe_CartVect localY = Vector.convertToMemorySafeVector(tempY);
                                

                                //new X axis is the localY cross the surface normal vector
                                Vector.CartVect tempX = new Vector.CartVect();
                                tempX = Vector.CrossProductNVRetMSMS(localY, surface.PlRHRVector);
                                tempX = Vector.UnitVector(tempX);
                                Vector.MemorySafe_CartVect localX = Vector.convertToMemorySafeVector(tempX);

                                //convert the polyloop coordinates to a local 2-D reference frame
                                //using a trick employed by video game programmers found here http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
                                List<Vector.MemorySafe_CartCoord> translatedCoordinates = new List<Vector.MemorySafe_CartCoord>();
                                Vector.MemorySafe_CartCoord newOrigin = new Vector.MemorySafe_CartCoord(0,0,0);

                                translatedCoordinates.Add(newOrigin);
                                for (int j = 1; j < surface.PlCoords.Count; j++)
                                {
                                    //randomly assigns the first polyLoop coordinate as the origin
                                    Vector.MemorySafe_CartCoord origin = surface.PlCoords[0];
                                    //captures the components of a vector drawn from the new origin to the 
                                    Vector.CartVect distance = new Vector.CartVect();
                                    distance.X = surface.PlCoords[j].X - origin.X;
                                    distance.Y = surface.PlCoords[j].Y - origin.Y;
                                    distance.Z = surface.PlCoords[j].Z - origin.Z;
                                    Vector.CartCoord translatedPt = new Vector.CartCoord();
                                    //x coordinate is distance vector dot the new local X axis
                                    translatedPt.X = distance.X * localX.X + distance.Y * localX.Y + distance.Z * localX.Z;
                                    //y coordinate is distance vector dot the new local Y axis
                                    translatedPt.Y = distance.X * localY.X + distance.Y * localY.Y + distance.Z * localY.Z;
                                    translatedPt.Z = 0;
                                    Vector.MemorySafe_CartCoord memsafetranspt = Vector.convertToMemorySafeCoord(translatedPt);
                                    translatedCoordinates.Add(memsafetranspt);

                                }
                                double area = GetAreaFrom2DPolyLoop(translatedCoordinates);
                                if (area == -999)
                                {
                                    report.MessageList.Add("The coordinates of the standard file polyloop has been incorrectly defined.");
                                    report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                    report.MessageList.Add("Test may be inaccurate and requires gbXML.org support");

                                }
                                //get the area of the test candidates using the polyloop coordinates
                                foreach (SurfaceDefinitions testSurface in possiblesList2)
                                {
                                    Vector.CartVect testtempY = new Vector.CartVect();
                                    Vector.MemorySafe_CartVect testglobalReferenceX = new Vector.MemorySafe_CartVect(1,0,0);

                                    testtempY = Vector.CrossProductNVRetMSMS(surface.PlRHRVector, testglobalReferenceX);
                                    testtempY = Vector.UnitVector(testtempY);
                                    Vector.MemorySafe_CartVect testlocalY = Vector.convertToMemorySafeVector(testtempY);


                                    //new X axis is the localY cross the surface normal vector
                                    Vector.CartVect testtempX = new Vector.CartVect();
                                    testtempX = Vector.CrossProductNVRetMSMS(testlocalY, surface.PlRHRVector);
                                    testtempX = Vector.UnitVector(testtempX);
                                    Vector.MemorySafe_CartVect testlocalX = Vector.convertToMemorySafeVector(testtempX);

                                    //convert the polyloop coordinates to a local 2-D reference frame
                                    //using a trick employed by video game programmers found here http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
                                    List<Vector.MemorySafe_CartCoord> testtranslatedCoordinates = new List<Vector.MemorySafe_CartCoord>();
                                    Vector.MemorySafe_CartCoord newOriginTest = new Vector.MemorySafe_CartCoord(0,0,0);

                                    testtranslatedCoordinates.Add(newOriginTest);
                                    for (int j = 1; j < surface.PlCoords.Count; j++)
                                    {
                                        //randomly assigns the first polyLoop coordinate as the origin
                                        Vector.MemorySafe_CartCoord origin = testSurface.PlCoords[0];
                                        //captures the components of a vector drawn from the new origin to the 
                                        Vector.CartVect distance = new Vector.CartVect();
                                        distance.X = testSurface.PlCoords[j].X - origin.X;
                                        distance.Y = testSurface.PlCoords[j].Y - origin.Y;
                                        distance.Z = testSurface.PlCoords[j].Z - origin.Z;
                                        Vector.CartCoord translatedPt = new Vector.CartCoord();
                                        //x coordinate is distance vector dot the new local X axis
                                        translatedPt.X = distance.X * localX.X + distance.Y * localX.Y + distance.Z * localX.Z;
                                        //y coordinate is distance vector dot the new local Y axis
                                        translatedPt.Y = distance.X * localY.X + distance.Y * localY.Y + distance.Z * localY.Z;
                                        translatedPt.Z = 0;
                                        Vector.MemorySafe_CartCoord memsafetranspt = Vector.convertToMemorySafeCoord(translatedPt);
                                        testtranslatedCoordinates.Add(memsafetranspt);

                                    }
                                    double testarea = GetAreaFrom2DPolyLoop(translatedCoordinates);
                                    if (testarea == -999)
                                    {
                                        report.MessageList.Add("The coordinates of the test file polyloop has been incorrectly defined.");
                                        report.MessageList.Add("The coordinates should be 2D and could not be translated to 2D");
                                    }
                                    double difference = Math.Abs(area - testarea);
                                    if (difference < area * DOEgbXMLBasics.Tolerances.SurfaceAreaPercentageTolerance)
                                    {
                                        possiblesList1.Add(testSurface);
                                        //within reason
                                        if (difference == 0)
                                        {
                                            report.MessageList.Add
                                                ("The test surface: " + testSurface.SurfaceId +
                                                " polyloop surface area matches the polyLoop surface area of the standard surface: "
                                                + surface.SurfaceId + " exactly.");
                                        }
                                        else
                                        {
                                            report.MessageList.Add
                                                ("The test surface: " + testSurface.SurfaceId +
                                                " polyloop surface area matches the polyLoop surface area of the standard surface: "
                                                + surface.SurfaceId + " within the allowable area percentage tolerance.");
                                        }
                                    }
                                    else
                                    {
                                        //not within reason, so the test will fail
                                        //don't return yet, it will be returned below when possiblesList1 is found empty
                                    }
                                }
                            }
                        }
                    }

                    possiblesList2.Clear();
                    //polyLoop absolute coordinates
                    //list 1 is analyzed
                    //list 2 is free
                    report.MessageList.Add("</br>");
                    report.MessageList.Add("Starting PolyLoop coordinate comparisons.......");
                    report.MessageList.Add("</br>");
                    if (possiblesList1.Count > 0)
                    {

                        foreach (SurfaceDefinitions testSurface in possiblesList1)
                        {
                            //check the polyLoop coordinates
                            foreach (Vector.MemorySafe_CartCoord standardPolyLoopCoord in surface.PlCoords)
                            {
                                report = GetPolyLoopCoordMatch(standardPolyLoopCoord, testSurface, report, surface.SurfaceId);
                                if (report.passOrFail)
                                {
                                    continue;
                                }
                                else
                                {
                                    report.MessageList.Add("Could not find a coordinate match in the test surface polyloop.");
                                    break;
                                }
                            }
                            if (report.passOrFail)
                            {
                                possiblesList2.Add(testSurface);
                            }
                        }
                    }
                    else
                    {
                        report.longMsg = "In the test file, no surfaces could be found that match standard file;s Surface Id: " + surface.SurfaceId + " AdjacentSpaceId(s), SurfaceType, Tilt, Azimuth, and Surface Area.  Failed when attempting to match the surface area.";
                        report.passOrFail = false;
                        return report;
                    }
                    possiblesList1.Clear();
                    report.MessageList.Add("</br>");
                    report.MessageList.Add("Starting Insertion Point Coordinate comparisons.......");
                    report.MessageList.Add("</br>");
                    if (possiblesList2.Count > 0)
                    {
                        //check the insertion point coordinate
                        foreach (SurfaceDefinitions testSurface in possiblesList2)
                        {
                            //now match the differences
                            double insPtXDiff = Math.Abs(testSurface.InsertionPoint.X - surface.InsertionPoint.X);
                            double insPtYDiff = Math.Abs(testSurface.InsertionPoint.Y - surface.InsertionPoint.Y);
                            double insPtZDiff = Math.Abs(testSurface.InsertionPoint.Z - surface.InsertionPoint.Z);
                            if (insPtXDiff > DOEgbXMLBasics.Tolerances.SurfaceInsPtXTolerance || insPtYDiff > DOEgbXMLBasics.Tolerances.SurfaceInsPtYTolerance || insPtZDiff > DOEgbXMLBasics.Tolerances.SurfaceInsPtZTolerance)
                            {
                                report.MessageList.Add("Test file's Surface id: " + testSurface.SurfaceId + " insertion point coordinates do not both match the standard file surface id: " + surface.SurfaceId + ".  It has been removed as a candidate.");
                                continue;
                            }
                            else
                            {
                                //possible match
                                possiblesList1.Add(testSurface);
                                if (insPtXDiff == 0 && insPtYDiff == 0 && insPtZDiff == 0)
                                {
                                    //perfect match
                                    report.MessageList.Add("Test file's Surface with id: " + testSurface.SurfaceId + " matches the insertion point in the standard file exactly.");
                                }
                                else
                                {
                                    //perfect match
                                    report.MessageList.Add(" Test file's Surface with id: " + testSurface.SurfaceId + " has an insertion point that is within the allowable tolerances of X:" + DOEgbXMLBasics.Tolerances.SurfaceInsPtXTolerance + " ft, Y:" + DOEgbXMLBasics.Tolerances.SurfaceInsPtYTolerance + "ft, Z:" + DOEgbXMLBasics.Tolerances.SurfaceInsPtZTolerance + "ft.");
                                }
                            }

                        }
                    }
                    else
                    {
                        report.longMsg = "In the test file, no surfaces could be found that match standard file;s Surface Id: " + surface.SurfaceId + " AdjacentSpaceId(s), SurfaceType, Tilt, Azimuth, Surface Area, and PolyLoop Coordinates.  Failed when matching PolyLoop coordinates.";
                        report.passOrFail = false;
                        return report;
                    }
                    if (possiblesList1.Count == 1)
                    {
                        report.longMsg = "Advanced Surface Test found a match for Standard file surface id: " + surface.SurfaceId + " in the test file.  Only one match was found to be within all the tolerances allowed.  Surface id: " + possiblesList2[0].SurfaceId + ".";
                        report.passOrFail = true;
                        List<string> testFileSurfIds = new List<string>();
                        foreach (SurfaceDefinitions surf in possiblesList1) { testFileSurfIds.Add(surf.SurfaceId); }

                        globalMatchObject.MatchedSurfaceIds.Add(surface.SurfaceId, testFileSurfIds);
                        return report;
                    }
                    else if (possiblesList1.Count == 0)
                    {
                        report.longMsg = "In the test file, no surfaces could be found that match standard file;s Surface Id: " + surface.SurfaceId + " AdjacentSpaceId(s), SurfaceType, Tilt, Azimuth, Surface Area, PolyLoop Coordinates, and Insertion Point.  Failed when attempting to match the insertion point coordinates.";
                        report.passOrFail = false;
                        return report;
                    }
                    else if (possiblesList1.Count > 1)
                    {
                        report.longMsg = "Advanced Surface Test found more than one match for Standard file surface id: " + surface.SurfaceId + " in the test file.  It was not possible to determine only one unique surface.";
                        report.passOrFail = false;
                        //List<string> testFileSurfIds = new List<string>();
                        //foreach (SurfaceDefinitions surf in possiblesList1) { testFileSurfIds.Add(surf.SurfaceId); }
                        //report.MatchedSurfaceIds.Add(surface.SurfaceId, testFileSurfIds);
                        return report;
                    }
                    return report;
                }
                return report;

            }
            catch (Exception e)
            {
                report.longMsg = (e.ToString());
                return report;
            }
        }

        private static DOEgbXMLReportingObj GetPolyLoopCoordMatch(Vector.MemorySafe_CartCoord standardPolyLoopCoord, SurfaceDefinitions testSurface, DOEgbXMLReportingObj report, string standardSurfaceId)
        {
            List<Vector.MemorySafe_CartCoord> possibleMatch = new List<Vector.MemorySafe_CartCoord>();
            List<Vector.MemorySafe_CartCoord> exactMatch = new List<Vector.MemorySafe_CartCoord>();
            report.MessageList.Add("Testing Polyloop coordinates for Standard surface " + standardSurfaceId);
            report.MessageList.Add(" X: " + standardPolyLoopCoord.X.ToString() + ", Y: " + standardPolyLoopCoord.Y.ToString() + ", Z: " + standardPolyLoopCoord.Z.ToString());
            foreach (Vector.MemorySafe_CartCoord testPolyLoopCoord in testSurface.PlCoords)
            {

                //find an appropriate match
                double diffX = Math.Abs(testPolyLoopCoord.X - standardPolyLoopCoord.X);
                if (diffX < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                {
                    //found a perfect X Match
                    if (diffX == 0)
                    {
                        //test Y
                        double diffY = Math.Abs(testPolyLoopCoord.Y - standardPolyLoopCoord.Y);
                        if (diffY < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                        {
                            //perfect Y Match
                            if (diffY == 0)
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " exactly");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        exactMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        //not a perfect Z match but within bounds
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " X and Y coordinates exactly.  Z coordinate within allowable tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                else
                                {
                                    //z coordinate not within tolerance
                                    continue;
                                }
                            }
                            //Y Match is within the allowable tolerance
                            else
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " in the X and Z coordinates, exactly.  Y coordinate is within tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " X exactly.  Y and Z coordinates are within tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                else
                                {
                                    //z coordinate is not within tolerance
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            //a y match could not be found within tolerance
                            continue;
                        }

                    }
                    else
                    {
                        //not a perfect X match, but within tolerance
                        //test Y
                        double diffY = Math.Abs(testPolyLoopCoord.Y - standardPolyLoopCoord.Y);
                        if (diffY < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                        {
                            //perfect Y Match
                            if (diffY == 0)
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " Y and Z coordinate exactly.  X is within tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " Y coordinate exactly.  X and Z is within tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                else
                                {
                                    //z is not matched so continue
                                    continue;
                                }
                            }
                            // the Y match is not perfect but within tolerance
                            else
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + " Z coordinate exactly.  The X and Y coordinates are within tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId + ": Found polyLoop coordinate that matches Standard Surface " + standardSurfaceId + ".  The X, Y, and Z coordinates are within tolerance.");
                                        report.MessageList.Add("Test Surface " + testSurface.SurfaceId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                // no match found for the Z
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        //no match could be found for the Y
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    //not a match found for the X and continue
                    continue;
                }
            }
            if (exactMatch.Count > 1)
            {
                report.MessageList.Add("Error, overlapping polyLoop coordinates found in the Test Surface PolyLoop.");
                report.passOrFail = false;
                return report;
            }
            else if (exactMatch.Count == 1)
            {
                report.MessageList.Add("One coordinate candidate found.  Exact match");
                report.passOrFail = true;
                return report;
            }
            if (possibleMatch.Count > 1)
            {
                report.MessageList.Add("No exact solution for a match of the polyLoop coordinate.  More than one coordinate candidate found.");
                report.passOrFail = false;
                return report;
            }
            else if (possibleMatch.Count == 1)
            {
                report.MessageList.Add("One coordinate candidate found.");
                report.passOrFail = true;
                return report;
            }
            else
            {
                report.MessageList.Add("No coordinate candidate found.");
                report.passOrFail = false;
                return report;
            }

        }

        private static DOEgbXMLReportingObj GetOpeningPolyLoopCoordMatch(Vector.MemorySafe_CartCoord standardPolyLoopCoord, OpeningDefinitions testOpening, DOEgbXMLReportingObj report, string standardOpeningId)
        {
            List<Vector.MemorySafe_CartCoord> possibleMatch = new List<Vector.MemorySafe_CartCoord>();
            List<Vector.MemorySafe_CartCoord> exactMatch = new List<Vector.MemorySafe_CartCoord>();
            report.MessageList.Add("Testing Polyloop coordinates for Standard opening " + standardOpeningId);
            report.MessageList.Add(" X: " + standardPolyLoopCoord.X.ToString() + ", Y: " + standardPolyLoopCoord.Y.ToString() + ", Z: " + standardPolyLoopCoord.Z.ToString());
            foreach (Vector.MemorySafe_CartCoord testPolyLoopCoord in testOpening.PlCoords)
            {

                //find an appropriate match
                double diffX = Math.Abs(testPolyLoopCoord.X - standardPolyLoopCoord.X);
                if (diffX < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                {
                    //found a perfect X Match
                    if (diffX == 0)
                    {
                        //test Y
                        double diffY = Math.Abs(testPolyLoopCoord.Y - standardPolyLoopCoord.Y);
                        if (diffY < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                        {
                            //perfect Y Match
                            if (diffY == 0)
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " exactly");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        exactMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        //not a perfect Z match but within bounds
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " X and Y coordinates exactly.  Z coordinate within allowable tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                else
                                {
                                    //z coordinate not within tolerance
                                    continue;
                                }
                            }
                            //Y Match is within the allowable tolerance
                            else
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " in the X and Z coordinates, exactly.  Y coordinate is within tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " X exactly.  Y and Z coordinates are within tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                else
                                {
                                    //z coordinate is not within tolerance
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            //a y match could not be found within tolerance
                            continue;
                        }

                    }
                    else
                    {
                        //not a perfect X match, but within tolerance
                        //test Y
                        double diffY = Math.Abs(testPolyLoopCoord.Y - standardPolyLoopCoord.Y);
                        if (diffY < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                        {
                            //perfect Y Match
                            if (diffY == 0)
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " Y and Z coordinate exactly.  X is within tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " Y coordinate exactly.  X and Z is within tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                else
                                {
                                    //z is not matched so continue
                                    continue;
                                }
                            }
                            // the Y match is not perfect but within tolerance
                            else
                            {
                                double diffZ = Math.Abs(testPolyLoopCoord.Z - standardPolyLoopCoord.Z);
                                if (diffZ < DOEgbXMLBasics.Tolerances.SurfacePLCoordTolerance)
                                {
                                    //perfect Z match
                                    if (diffZ == 0)
                                    {
                                        report.MessageList.Add("Test opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + " Z coordinate exactly.  The X and Y coordinates are within tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                    else
                                    {
                                        report.MessageList.Add("Test opening " + testOpening.OpeningId + ": Found polyLoop coordinate that matches Standard Opening " + standardOpeningId + ".  The X, Y, and Z coordinates are within tolerance.");
                                        report.MessageList.Add("Test Opening " + testOpening.OpeningId);
                                        report.MessageList.Add(" X: " + testPolyLoopCoord.X.ToString() + ", Y: " + testPolyLoopCoord.Y.ToString() + ", Z: " + testPolyLoopCoord.Z.ToString());
                                        possibleMatch.Add(testPolyLoopCoord);
                                    }
                                }
                                // no match found for the Z
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        //no match could be found for the Y
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    //not a match found for the X and continue
                    continue;
                }
            }
            if (exactMatch.Count > 1)
            {
                report.MessageList.Add("Error, overlapping polyLoop coordinates found in the Test Opening PolyLoop.");
                report.passOrFail = false;
                return report;
            }
            else if (exactMatch.Count == 1)
            {
                report.MessageList.Add("One coordinate candidate found.  Exact match");
                report.passOrFail = true;
                return report;
            }
            if (possibleMatch.Count > 1)
            {
                report.MessageList.Add("No exact solution for a match of the polyLoop coordinate.  More than one coordinate candidate found.");
                report.passOrFail = false;
                return report;
            }
            else if (possibleMatch.Count == 1)
            {
                report.MessageList.Add("One coordinate candidate found.");
                report.passOrFail = true;
                return report;
            }
            else
            {
                report.MessageList.Add("No coordinate candidate found.");
                report.passOrFail = false;
                return report;
            }

        }



        //March15 2013
        //by CHarriman Senior Product Manager Carmel Software Corporation
        //this is a function only used internally.  It is used to verify if a surface object only has four coordinates, and if those coordinates form
        //a square or rectangle.
        private static bool IsSurfaceRegular(SurfaceDefinitions Surface)
        {
            //tests to see if all candidate surfaces and the standard surface are regular (rectangular polygons)

            bool isRegularPolygon = true;
            //see if the standard surface has four coordinates defining its polyloop (one marker of a rectangle)
            int standSurfaceCoordinateCount = Surface.PlCoords.Count;
            if (standSurfaceCoordinateCount == 4)
            {
                //check the two potentially parallel sides, to ensure they are indeed parallel
                Vector.CartVect v1 = Vector.CreateVector(Surface.PlCoords[0], Surface.PlCoords[1]);
                Vector.CartVect v2 = Vector.CreateVector(Surface.PlCoords[2], Surface.PlCoords[3]);
                Vector.CartVect v1xv2 = Vector.CrossProduct(v1, v2);
                v1xv2 = Vector.UnitVector(v1xv2);
                double magnitudev1xv2 = Vector.VectorMagnitude(v1xv2);
                Vector.CartVect v3 = Vector.CreateVector(Surface.PlCoords[1], Surface.PlCoords[2]);
                Vector.CartVect v4 = Vector.CreateVector(Surface.PlCoords[3], Surface.PlCoords[0]);
                Vector.CartVect v3xv4 = Vector.CrossProduct(v3, v4);
                v3xv4 = Vector.UnitVector(v3xv4);
                double magnitudev3xv4 = Vector.VectorMagnitude(v3xv4);
                //the unit vector will not be a number NaN if the Cross product detects a zero vector (indicating parallel vectors)
                if (double.IsNaN(magnitudev1xv2) && double.IsNaN(magnitudev3xv4))
                {
                    isRegularPolygon = true;
                }
                else
                {
                    isRegularPolygon = false;
                }
            }
            else
            {
                //might as well stop here because 
                isRegularPolygon = false;
                return isRegularPolygon;
            }
            return isRegularPolygon;

        }

        //March15 2013
        //by CHarriman Senior Product Manager Carmel Software Corporation
        //this is a function only used internally.  It is used to verify if an opening object only has four coordinates, and if those coordinates form
        //a square or rectangle.
        private static bool IsOpeningRegular(OpeningDefinitions Opening)
        {
            //tests to see if all candidate surfaces and the standard surface are regular (rectangular polygons)

            bool isRegularPolygon = true;
            //see if the standard surface has four coordinates defining its polyloop (one marker of a rectangle)
            int standSurfaceCoordinateCount = Opening.PlCoords.Count;
            if (standSurfaceCoordinateCount == 4)
            {
                //check the two potentially parallel sides, to ensure they are indeed parallel
                Vector.CartVect v1 = Vector.CreateVector(Opening.PlCoords[0], Opening.PlCoords[1]);
                Vector.CartVect v2 = Vector.CreateVector(Opening.PlCoords[2], Opening.PlCoords[3]);
                Vector.CartVect v1xv2 = Vector.CrossProduct(v1, v2);
                v1xv2 = Vector.UnitVector(v1xv2);
                double magnitudev1xv2 = Vector.VectorMagnitude(v1xv2);
                Vector.CartVect v3 = Vector.CreateVector(Opening.PlCoords[1], Opening.PlCoords[2]);
                Vector.CartVect v4 = Vector.CreateVector(Opening.PlCoords[3], Opening.PlCoords[0]);
                Vector.CartVect v3xv4 = Vector.CrossProduct(v3, v4);
                v3xv4 = Vector.UnitVector(v3xv4);
                double magnitudev3xv4 = Vector.VectorMagnitude(v3xv4);
                //the unit vector will not be a number NaN if the Cross product detects a zero vector (indicating parallel vectors)
                if (double.IsNaN(magnitudev1xv2) && double.IsNaN(magnitudev3xv4))
                {
                    isRegularPolygon = true;
                }
                else
                {
                    isRegularPolygon = false;
                }
            }
            else
            {
                //might as well stop here because 
                isRegularPolygon = false;
                return isRegularPolygon;
            }
            return isRegularPolygon;

        }
        //February 20 2013
        //Created by Chien Si Harriman Senior Product Manager for the Carmel Software Corporation
        //Currently the tool assumes that the polyloop is a valid one (counterclockwise coordinates)  Previous checks ensure this is the case?
        //and the segments of the polygon are not self-intersecting  (there are no previous tests for this as of the date above)
        private static double GetAreaFrom2DPolyLoop(List<Vector.MemorySafe_CartCoord> coordList)
        {
            int count = coordList.Count;
            double areaprod = 0;
            bool XisZero = true;
            bool YisZero = true;
            bool ZisZero = true;
            //the following calculates the area of any irregular polygon
            foreach (Vector.MemorySafe_CartCoord coord in coordList)
            {
                if (coord.X != 0) XisZero = false;
                if (coord.Y != 0) YisZero = false;
                if (coord.Z != 0) ZisZero = false;
            }
            if (!XisZero && !YisZero && !ZisZero) return -999;

            if (XisZero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        areaprod += (coordList[i].Y * coordList[i + 1].Z - coordList[i].Z * coordList[i + 1].Y);
                    }
                    else if (i == count - 1)
                    {
                        areaprod += (coordList[i].Y * coordList[0].Z - coordList[i].Z * coordList[0].Y);
                    }
                }
                areaprod = areaprod / 2;
            }
            else if (YisZero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[i + 1].Z - coordList[i].Z * coordList[i + 1].X);
                    }
                    else if (i == count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[0].Z - coordList[i].Z * coordList[0].X);
                    }
                }
                areaprod = areaprod / 2;
            }
            else if (ZisZero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[i + 1].Y - coordList[i].Y * coordList[i + 1].X);
                    }
                    else if (i == count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[0].Y - coordList[i].Y * coordList[0].X);
                    }
                }
                areaprod = areaprod / 2;
            }
            return areaprod;
        }

        private static DOEgbXMLReportingObj CountFixedWindows(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"FixedWindow\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type FixedWindow counts are the same, or the test fails.";
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML 
            //added Feb 13 2013

            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "FixedWindow")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " fixed windows in the standard file and " + resultsArray[i - 1] + " fixed windows in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private static DOEgbXMLReportingObj CountOperableWindows(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"OperableWindow\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type FixedWindow counts are the same, or the test fails.";
            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "OperableWindow")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " operable windows in the standard file and " + resultsArray[i - 1] + " operable windows in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private static DOEgbXMLReportingObj CountFixedSkylights(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"FixedSkylight\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type FixedSkylight counts are the same, or the test fails.";
            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "FixedSkylight")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " fixed skylights in the standard file and " + resultsArray[i - 1] + " fixed skylights in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private static DOEgbXMLReportingObj CountOperableSkylights(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"OperableSkylights\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type OperableSkylights counts are the same, or the test fails.";
            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "OperableSkylight")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " operable skylights in the standard file and " + resultsArray[i - 1] + " operable skylights in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private static DOEgbXMLReportingObj CountSlidingDoors(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"SlidingDoor\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type SlidingDoor counts are the same, or the test fails.";
            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "SlidingDoor")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " sliding doors in the standard file and " + resultsArray[i - 1] + " sliding doors in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private static DOEgbXMLReportingObj CountNonSlidingDoors(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"NonSlidingDoor\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type NonSlidingDoor counts are the same, or the test fails.";
            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "NonSlidingDoor")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " non-sliding doors in the standard file and " + resultsArray[i - 1] + " non-sliding doors in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }

        private static DOEgbXMLReportingObj CountAirOpenings(List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "This test compares the total number of Opening elements with the openingType=\"Air\" in the test";
            report.testSummary += " and standard files.  It does this by";
            report.testSummary += " simply counting up the total number of times that a \"<Opening>\" tag appears with this Surface Type in both files.";
            report.testSummary += "  If the quantities are the same, this test passes, if different, it will fail.  ";
            report.testSummary += "The tolerance is zero for this test.  In other words, the Opening of type Air counts are the same, or the test fails.";
            report.unit = Units;
            //assuming that this will be plenty large for now
            string[] resultsArray = new string[50];
            int nodecount = 0;
            for (int i = 0; i < gbXMLDocs.Count; i++)
            {
                nodecount = 0;
                try
                {
                    XmlDocument gbXMLTestFile = gbXMLDocs[i];
                    XmlNamespaceManager gbXMLns = gbXMLnsm[i];

                    XmlNodeList nodes = gbXMLTestFile.SelectNodes("/gbXMLv5:gbXML/gbXMLv5:Campus/gbXMLv5:Surface/gbXMLv5:Opening", gbXMLns);
                    foreach (XmlNode surfaceNode in nodes)
                    {
                        XmlAttributeCollection spaceAtts = surfaceNode.Attributes;
                        foreach (XmlAttribute at in spaceAtts)
                        {
                            if (at.Name == "openingType")
                            {
                                string type = at.Value;
                                if (type == "Air")
                                {
                                    nodecount++;
                                }
                                break;
                            }
                        }
                    }

                    //need to test for accuracy of result if accurate then pass, if not, how much inaccuracy and return this result 
                    resultsArray[i] = nodecount.ToString();
                    if (i % 2 != 0)
                    {
                        //setup standard result and test result
                        report.standResult.Add(resultsArray[i]);
                        report.testResult.Add(resultsArray[i - 1]);
                        report.idList.Add("");

                        double difference = Math.Abs(Convert.ToInt32(resultsArray[i]) - Convert.ToInt32(resultsArray[(i - 1)]));
                        if (difference <= report.tolerance)
                        {
                            report.longMsg = "The " + report.testType + " matches standard file, the difference was within tolerance = " + report.tolerance.ToString() + " " + Units;
                            report.passOrFail = true;
                            return report;
                        }
                        else
                        {
                            report.longMsg = "The " + report.testType + " does not match standard file, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                                + ".  " + resultsArray[i] + " air openings in the standard file and " + resultsArray[i - 1] + " air openings in the test file.";
                            report.passOrFail = false;
                            return report;
                        }
                    }
                    else { continue; }

                }
                catch (Exception e)
                {
                    report.MessageList.Add(e.ToString());
                    report.longMsg = " Failed to locate " + report.testType + " in the XML file.";
                    report.passOrFail = false;
                    return report;
                }
            }
            report.longMsg = "Fatal " + report.testType + " Test Failure";
            report.passOrFail = false;
            return report;
        }
        #endregion
    }
}
