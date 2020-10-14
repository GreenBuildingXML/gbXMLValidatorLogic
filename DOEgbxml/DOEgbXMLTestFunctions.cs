using System;
using System.Collections.Generic;
using System.Xml;
using VectorMath;

namespace DOEgbXML
{
    public class DOEgbXMLTestFunctions

    {
        /**
         * 
         * Test to match window areas between standard and test.
         * If the type set to null, the method will compare all openings
         * else if type is set to one of the opening type (e.g. operable window), the method will only compare the area of 
         * operable windows between standard and test models.
         * 
         */
        public static DOEgbXMLReportingObj TestWindowAreaByType(List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string Units, string Type)
        {
            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Surface;

            double testArea = 0.0;
            double standardArea = 0.0;

            for (int i = 0; i < StandardSurfaces.Count; i++)
            {
                SurfaceDefinitions sd = StandardSurfaces[i];
                if(sd.subSurfaceList.Count > 0)
                {
                    foreach(SubSurfaceDefinition ssd in sd.subSurfaceList)
                    {
                        if (Type == null)
                        {
                            standardArea += ssd.computeArea();
                        }
                        else if(ssd.openingType == Type)
                        {
                            standardArea += ssd.computeArea();

                        }
                    }
                }
            }

            for (int i = 0; i < TestSurfaces.Count; i++)
            {
                SurfaceDefinitions sd = TestSurfaces[i];
                if (sd.subSurfaceList.Count > 0)
                {
                    foreach (SubSurfaceDefinition ssd in sd.subSurfaceList)
                    {
                        if (Type == null)
                        {
                            testArea += ssd.computeArea();
                        }
                        else if (ssd.openingType == Type)
                        {
                            testArea += ssd.computeArea();
                        }
                    }
                }
            }

            double difference = Math.Abs(standardArea - testArea);
            report.testResult.Add(testArea.ToString());
            report.standResult.Add(standardArea.ToString());
            report.idList.Add("");

            if (difference == 0)
            {
                report.longMsg = "The Test File's " + report.testType + " matches the Standard File exactly, the difference is zero.";
                report.passOrFail = true;
                report.outputType = OutPutEnum.Matched;
                return report;
            }
            else if (difference <= report.tolerance)
            {
                report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                report.passOrFail = true;
                report.outputType = OutPutEnum.Warning;
                return report;
            }
            else
            {
                report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                        + ".  " + standardArea + " (" + Type + ") surfaces in the Standard File and " + testArea + " (" + Type +") surfaces in the Test File.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
        }

        public static DOEgbXMLReportingObj TestPlenumSpaceVolume(List<gbXMLSpaces> testSpaces, List<gbXMLSpaces> standardSpaces, DOEgbXMLReportingObj report, String Units)
        {
            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Space;

            double testVolume = 0.0;
            double standardVolume = 0.0;

            for(int i=0; i<testSpaces.Count; i++)
            {
                if(testSpaces[i].spaceType == "Plenum")
                {
                    testVolume += testSpaces[i].volume;
                }
            }

            for(int i=0; i<standardSpaces.Count; i++)
            {
                if(standardSpaces[i].spaceType == "Plenum")
                {
                    standardVolume += standardSpaces[i].volume;
                }
            }

            double difference = Math.Abs(standardVolume - testVolume);
            report.testResult.Add(testVolume.ToString());
            report.standResult.Add(standardVolume.ToString());
            report.idList.Add("");

            if (difference == 0)
            {
                report.longMsg = "The Test File's " + report.testType + " matches the Standard File exactly, the difference is zero.";
                report.passOrFail = true;
                report.outputType = OutPutEnum.Matched;
                return report;
            }
            else if (difference <= report.tolerance)
            {
                report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                report.passOrFail = true;
                report.outputType = OutPutEnum.Warning;
                return report;
            }
            else
            {
                report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                        + ".  " + standardVolume + " in the Standard File and " + testVolume + " in the Test File.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
        }

        /**
         * Test whether the zone names in the test model match the standard model
         * 
         */
        public static DOEgbXMLReportingObj TestZoneNameMatch(List<gbXMLSpaces> testSpaces,List<gbXMLSpaces> standardSpaces,DOEgbXMLReportingObj report, String Units)
        {
            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Space;

            int mismatchCounter = 0;

            for(int i=0; i<standardSpaces.Count; i++)
            {
                gbXMLSpaces stdSpace = standardSpaces[i];
                gbXMLSpaces matchedSpace = null;
                if (stdSpace.spaceType != "Plenum")
                {
                    //search for match
                    for (int j = 0; j < testSpaces.Count; j++)
                    {
                        //We are not checking the plenum 
                        if (stdSpace.name == testSpaces[j].name)
                        {
                            matchedSpace = testSpaces[j];
                        }
                    }
                    if (matchedSpace == null)
                    {
                        mismatchCounter++;
                        string msg = "Cannot find a match space name in Test Model for the space: <a class='" + stdSpace.id + "'>" + stdSpace.name + "</a>.";
                        report.MessageList.Add(msg);
                        report.testResult.Add("NA");
                        report.standResult.Add(stdSpace.name);
                        report.idList.Add(i + "");
                    }
                    else
                    {
                        string msg = "Find a match space name in Test Model <a class='" + matchedSpace.id + "'></a>  for the space:" + stdSpace.name + ".";
                        report.MessageList.Add(msg);
                        report.testResult.Add(stdSpace.name);
                        report.standResult.Add(stdSpace.name);
                        report.idList.Add(i + "");
                    }
                }
            }

            if (mismatchCounter == 0)
            {
                report.longMsg = "The Test File's " + report.testType + " matches the Standard File exactly, the difference is zero.";
                report.passOrFail = true;
                report.outputType = OutPutEnum.Matched;
                return report;
            }
            else
            {
                report.longMsg = "Spaces in The Test File's " + report.testType + " do not match those in the Standard File within the allowable tolerance, the difference between the two files is " + mismatchCounter + ". ";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
        }

        public static DOEgbXMLReportingObj TestSpaceVolumeMatch(List<gbXMLSpaces> testSpaces, List<gbXMLSpaces> standardSpaces, DOEgbXMLReportingObj report, String Units)
        {
            report.passOrFail = true;
            report.unit = Units;
            report.Type = ReportParamType.Space;
            //assuming that this will be plenty large for now
            Dictionary<string, double> standardFileVolumeDict = new Dictionary<string, double>();
            Dictionary<string, double> testFileVolumeDict = new Dictionary<string, double>();

            for(int i=0; i<testSpaces.Count; i++)
            {
                gbXMLSpaces testSpace = testSpaces[i];

                gbXMLSpaces standardSpace = null;
                for(int j=0; j<standardSpaces.Count; j++)
                {
                    if(standardSpaces[j].name == testSpaces[i].name)
                    {
                        standardSpace = standardSpaces[j];
                    }
                }

                if (standardSpace == null)
                {
                    report.longMsg = "Cannot find a matching space in standard space for space: <a class='" + testSpace.id+"'> "+ testSpace.name + "</a>.";
                    report.passOrFail = false;
                    report.outputType = OutPutEnum.Failed;
                    return report;
                }

                report.standResult.Add(Convert.ToString(standardSpace.volume));
                report.testResult.Add(Convert.ToString(testSpace.volume));
                report.idList.Add(testSpace.id);

                double difference = Math.Abs(testSpace.volume - standardSpace.volume);
                if (difference == 0)
                {
                    report.MessageDict.Add(testSpace.id, "For Space <a class'" + testSpace.id + ">" + testSpace.name + "</a>. Success finding matching space volume.  The Standard and Test Files both have identical volumes: " + testSpace.volume + " " + Units);
                    report.TestPassedDict.Add(testSpace.id, true);
                    report.OutputTypeDict.Add(testSpace.id, OutPutEnum.Matched);
                }
                else if (difference < report.tolerance)
                {
                    report.MessageDict.Add(testSpace.id, "For Space <a class'" + testSpace.id + ">" + testSpace.name + "</a>. Success finding matching space volume.  The Standard Files space volume of " + standardSpace.volume + " " + Units + "and the Test File space volume: " + testSpace.volume + " are within the allowed tolerance of" + report.tolerance.ToString() + " " + Units + ".");
                    report.TestPassedDict.Add(testSpace.id, true);
                    report.OutputTypeDict.Add(testSpace.id, OutPutEnum.Warning);
                }
                else
                {
                    //at the point of failure, the test will return with details about which volume failed.
                    report.MessageDict.Add(testSpace.id, "For Space <a class'" + testSpace.id + ">" + testSpace.name + "</a>. Failure to find a volume match.  The Volume in the Test File equal to: " + testSpace.volume + " " + Units + " was not within the allowed tolerance.  SpaceId: <a class'" + standardSpace.id + "'>" + standardSpace.name + "</a> in the Standard file has a volume: " + standardSpace.volume + " .");
                    report.TestPassedDict.Add(testSpace.id, false);
                    report.OutputTypeDict.Add(testSpace.id, OutPutEnum.Failed);
                    report.passOrFail = false;
                    report.outputType = OutPutEnum.Failed;
                    return report;
                }
            }
            return report;
        }

        public static DOEgbXMLReportingObj TestSurfaceCountByType(List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string Units, String Type)
        {
            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Surface;

            int testSurfaceCount = 0;
            int standardSurfaecCount = 0;

            for(int i=0; i<StandardSurfaces.Count; i++)
            {
                if (StandardSurfaces[i].SurfaceType == Type)
                {
                    standardSurfaecCount += 1;
                }
            }

            for (int i = 0; i < TestSurfaces.Count; i++)
            {
                if (TestSurfaces[i].SurfaceType == Type)
                {
                    testSurfaceCount += 1;
                }
            }

            int difference = Math.Abs(standardSurfaecCount - testSurfaceCount);
            report.testResult.Add(testSurfaceCount.ToString());
            report.standResult.Add(standardSurfaecCount.ToString());
            report.idList.Add("");

            if (difference == 0)
            {
                report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly, the difference is zero.";
                report.passOrFail = true;
                report.outputType = OutPutEnum.Matched;
                return report;
            }
            else if (difference <= report.tolerance)
            {
                report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                report.passOrFail = true;
                report.outputType = OutPutEnum.Warning;
                return report;
            }
            else
            {
                report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                        + ".  " + standardSurfaecCount + " surfaces in the Standard File and " + testSurfaceCount + " surfaces in the Test File.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
        }

        private static Dictionary<String, Double> getSurfaceAreaMapOrientation(List<SurfaceDefinitions> surfaces)
        {
            Dictionary<String, Double> surfaceAreaMapOrientation = new Dictionary<String, Double>();
            foreach (SurfaceDefinitions surfDef in surfaces)
            {
                string orientation = surfDef.surfaceOrientation();
                if (!surfaceAreaMapOrientation.ContainsKey(orientation))
                {
                    surfaceAreaMapOrientation.Add(orientation, 0.0);
                }
                surfaceAreaMapOrientation[orientation] += surfDef.computeArea();
            }

            return surfaceAreaMapOrientation;
        }


        /**
         * 
         * This test function is used to check test surface area by orientation will match those in the standard surface area.
         * e.g. north face surface area
         * 
         **/
        public static DOEgbXMLReportingObj TestWallAreaByOrientation(List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string Units)
        {
            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Surface;

            Dictionary<String, Double> standardSurfaceAreaMapOrientation = getSurfaceAreaMapOrientation(StandardSurfaces);
            Dictionary<String, Double> testSurfaceAreaMapOrientation = getSurfaceAreaMapOrientation(TestSurfaces);

            //now loop through and standard dictionary and try to find the match keys from the test dictionary
            report.passOrFail = true; //assume it is passed first.
            report.outputType = OutPutEnum.Matched;
            foreach (KeyValuePair<String, Double> kvp in standardSurfaceAreaMapOrientation)
            {
                string key = kvp.Key;
                double value = kvp.Value;

                double testValue = 0.0;
                if (testSurfaceAreaMapOrientation.ContainsKey(key))
                {
                    testValue = testSurfaceAreaMapOrientation[key];
                }

                report.testResult.Add(testValue.ToString());
                report.standResult.Add(value.ToString());
                report.idList.Add(key);
                double difference = Math.Abs(value - testValue);

                if (difference == 0)
                {
                    report.MessageDict.Add(key, "The orientation: " + key + " in the Test File matches the Standard File exactly, the difference is zero.");
                    report.TestPassedDict.Add(key, true);
                    report.OutputTypeDict.Add(key, OutPutEnum.Matched);
                }
                else if (difference <= report.tolerance)
                {
                    report.MessageDict.Add(key, "The orientation: " + key + " in the Test File matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units);
                    report.TestPassedDict.Add(key, true);
                    report.OutputTypeDict.Add(key, OutPutEnum.Warning);
                }
                else
                {
                    report.MessageDict.Add(key, "The orientation: " + key + " in the Test File does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                            + ".  " + value + " in the Standard File and " + testValue + " in the Test File.");
                    report.TestPassedDict.Add(key, false);
                    report.OutputTypeDict.Add(key, OutPutEnum.Failed);
                }
            }


            if (report.passOrFail)
            {
                report.longMsg = "The test model wall surface areas by orientations match the standard model or within the tolerance.";
            }
            else
            {
                report.longMsg = "The test model wall surface area in one or multiple orientations do not match the ones in the standard model, check message list for detail information";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
            }

            return report;
        }

        /*
        * This method compares the surface areas between test file and standard file by surface type
        * For example, it can be used to compare the exterior wall surface area.
        */
        public static DOEgbXMLReportingObj TestSurfaceAreaByType(List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string Units, String Type)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML
            //added 07/07/2020
            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Surface;

            double testArea = 0.0;
            double standardArea = 0.0;

            for (int i = 0; i < StandardSurfaces.Count; i++)
            {
                if (StandardSurfaces[i].searchForMatchedType(Type))
                {
                    standardArea += StandardSurfaces[i].computeArea();
                }
            }

            for (int i = 0; i < TestSurfaces.Count; i++)
            {
                if (TestSurfaces[i].searchForMatchedType(Type))
                {
                    testArea += TestSurfaces[i].computeArea();
                }
            }

            double difference = Math.Abs(standardArea - testArea);
            report.testResult.Add(testArea.ToString());
            report.standResult.Add(standardArea.ToString());
            report.idList.Add("");

            if (difference == 0)
            {
                report.longMsg = "The Test File's " + report.testType + " matches the Standard File exactly, the difference is zero.";
                report.passOrFail = true;
                report.outputType = OutPutEnum.Matched;
                return report;
            }
            else if (difference <= report.tolerance)
            {
                report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                report.passOrFail = true;
                report.outputType = OutPutEnum.Warning;
                return report;
            }
            else
            {
                report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                        + ".  " + standardArea + " (" + Type + ") surfaces in the Standard File and " + testArea + " (" + Type +") surfaces in the Test File.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
        }
        
        public static DOEgbXMLReportingObj TestShadeSurfaceArea(List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string Units)
        {
            //this summary is text that describes to a lay user what this test does, and how it works functionally.  The user should have some familiarity with the basic knowledge of gbXML
            //This method is designed for test case 13: balcony and test case 14: roof overhang
            //Therefore, the method checks the shading surface area on 10' level and try to match the total shading surface area
            //added 07/07/2020

            report.testSummary = "";
            report.unit = Units;
            report.Type = ReportParamType.Surface;

            double testArea = 0.0;
            double standardArea = 0.0;

            for (int i = 0; i < StandardSurfaces.Count; i++)
            {
                if (StandardSurfaces[i].SurfaceType == "Shade")
                {
                    //check the coordinates to determine whether this shade is at level 10'
                    SurfaceDefinitions surfDef = StandardSurfaces[i];

                    List<Vector.MemorySafe_CartCoord> plCoords = surfDef.PlCoords;

                    Double lowestZ = Double.MaxValue;
                    for(int j=0; j<plCoords.Count; j++)
                    {
                        Double z = plCoords[j].Z;
                        if(lowestZ < z)
                        {
                            lowestZ = z;
                        }
                    }

                    //add surface area
                    if(lowestZ == 10)
                    {
                        standardArea += StandardSurfaces[i].computeArea();
                    }

                }
            }

            for (int i = 0; i < TestSurfaces.Count; i++)
            {
                if (TestSurfaces[i].SurfaceType == "Shade")
                {
                    //check the coordinates to determine whether this shade is at level 10'
                    SurfaceDefinitions surfDef = TestSurfaces[i];
                    List<Vector.MemorySafe_CartCoord> plCoords = surfDef.PlCoords;

                    Double lowestZ = Double.MaxValue;
                    for (int j = 0; j < plCoords.Count; j++)
                    {
                        Double z = plCoords[j].Z;
                        if (lowestZ < z)
                        {
                            lowestZ = z;
                        }
                    }

                    //add surface area
                    if (lowestZ == 10)
                    {
                        testArea += TestSurfaces[i].computeArea();
                    }

                }
            }

            //test if matches
            double difference = Math.Abs(standardArea - testArea);
            report.testResult.Add(testArea.ToString());
            report.standResult.Add(standardArea.ToString());
            report.idList.Add("");

            if (difference == 0)
            {
                report.longMsg = "The Test File's " + report.testType + " matches the Standard File exactly, the difference is zero.";
                report.passOrFail = true;
                report.outputType = OutPutEnum.Matched;
                return report;
            }
            else if (difference <= report.tolerance)
            {
                report.longMsg = "The Test File's " + report.testType + " matches Standard File within the allowable tolerance, the difference between the two files is " + report.tolerance.ToString() + " " + Units;
                report.passOrFail = true;
                report.outputType = OutPutEnum.Warning;
                return report;
            }
            else
            {
                report.longMsg = "The Test File's " + report.testType + " does not match Standard File, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + difference
                        + ".  " + standardArea + " shade surfaces in the Standard File and " + testArea + " shade surfaces in the Test File.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
        }

        /**
         * The function used to test the construction assemblies
         * 
         * Pre condition: Each surface category (wallnorth, wallsouth, walleast, wallwest, roof, floor) shall have the same construction id.
         * 
         * @TestConstruction - List of DOEgbXMLConstruction that contains the construction data extracted from a user submitted test model
         * @StandardConstruction - List of DOEgbXMLConstruction that contains the construction data extracted from the standard model
         * @TestSurfaces - A list of SurfaceDefinitions that contains the surfaces extracted from a user uploaded test model
         * @StandardSurfaces - A list of SurfaceDefinitions that contains the surfaces extracted from the standard model.
         * @Report - the DOEgbXMLReportingObj write in report message.
         * @Units - units string
         * @WallOnly - boolean flag, true - test only the wall construcitons, false - test all constructions
         * 
         */
        public static DOEgbXMLReportingObj TestMaterialAssembly(List<DOEgbXMLConstruction> TestConstructions, List<DOEgbXMLConstruction> StandardConstructions,
            List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string units)
        {
            //RP-1810 Weili: first assemble a surface to construction map
            //In this case, we use dictionary to map surfaces including:
            //ExteriorWall (by orientation), Roof and Floor - future test can expand this list.
            Dictionary<String, DOEgbXMLConstruction> StandardSurfaceToConstructionMap = new Dictionary<String, DOEgbXMLConstruction>();
            Dictionary<String, DOEgbXMLConstruction> TestSurfaceToConstructionMap = new Dictionary<String, DOEgbXMLConstruction>();
            report.Type = ReportParamType.Space;

            //make standard surface map
            foreach(SurfaceDefinitions sd in StandardSurfaces){
                String constructionID = sd.ConstructionId;
                foreach(DOEgbXMLConstruction constrct in StandardConstructions)
                {
                    if (constructionID == constrct.id)
                    {
                        String type = sd.SurfaceType;
                        if(type == "ExteriorWall")
                        {
                            type += sd.surfaceOrientation();
                        }
                        StandardSurfaceToConstructionMap.Add(type, constrct);
                    }
                }
            }

            //make test surface map
            foreach (SurfaceDefinitions sd in TestSurfaces)
            {
                String constructionID = sd.ConstructionId;
                foreach (DOEgbXMLConstruction constrct in TestConstructions)
                {
                    if (constructionID == constrct.id)
                    {
                        String type = sd.SurfaceType;
                        if (type == "ExteriorWall")
                        {
                            type += sd.surfaceOrientation();
                        }
                        TestSurfaceToConstructionMap.Add(type, constrct);
                    }
                }
            }


            //now compare the two types of constructions.
            //initialize the message list
            List<String> messageList = new List<string>();
            foreach (KeyValuePair<String, DOEgbXMLConstruction> entry in StandardSurfaceToConstructionMap)
            {

                if (TestSurfaceToConstructionMap.ContainsKey(entry.Key))
                {
                    //find match surface - now check construction
                    DOEgbXMLConstruction standardConst = StandardSurfaceToConstructionMap[entry.Key];
                    DOEgbXMLConstruction testConst = TestSurfaceToConstructionMap[entry.Key];

                    //compare and give a report

                    Boolean compareResult = standardConst.compare(testConst, messageList);

                    report.testResult.Add(testConst.name);
                    report.standResult.Add(standardConst.name);
                    report.idList.Add(entry.Key);

                    if (compareResult == false)
                    {
                        report.longMsg = "The Test File's " + report.testType + " does not match the Standard File exactly. It failed the test.";
                        report.MessageList = messageList;
                        report.passOrFail = false;
                        report.outputType = OutPutEnum.Failed;
                        return report;
                    }
                }
            }
            
            report.longMsg = "The Test File's " + report.testType + " matches the Standard File exactly. It pass the test.";
            report.MessageList = messageList;
            report.passOrFail = true;
            report.outputType = OutPutEnum.Matched;
            
            return report;
        }


        //this test only works for Curved wall test - other test case should not touch this test case.
        public static DOEgbXMLReportingObj TestCurvedWallSurfaceArea(List<SurfaceDefinitions> TestSurfaces, DOEgbXMLReportingObj report, string Units)
        {
            report.passOrFail = true;
            report.outputType = OutPutEnum.Matched;
            report.longMsg = "The Test Wall has passed the Curved Wall test.";
            report.Type = ReportParamType.MultiSurfaces;

            //1. first determine the surface areas
            double n = 0.0; //0-22.5
            double ne = 0.0; //22.5 - 67.5
            double e = 0.0; //67.5 - 110.5
            double se = 0.0;//110.5 - 157.5
            double s = 0.0; // 157.5 - 180
            double w = 0.0; // 270

            Dictionary<string, string> orientationToSurfaceMap = new Dictionary<string, string>();
            foreach(SurfaceDefinitions sf in TestSurfaces)
            {
                if(sf.SurfaceType == "ExteriorWall")
                {
                    string orientation = sf.surfaceOrientation();
                    if (!orientationToSurfaceMap.ContainsKey(orientation))
                    {
                        orientationToSurfaceMap.Add(orientation, sf.SurfaceId);
                    }
                    else
                    {
                        string newId = orientationToSurfaceMap[orientation] + "," + sf.SurfaceId;
                        orientationToSurfaceMap[orientation] = newId;
                    }
                }
            }

            Dictionary<String, Double> testSurfaceAreaMapOrientation = getSurfaceAreaMapOrientation(TestSurfaces);

            foreach (KeyValuePair<String, Double> kvp in testSurfaceAreaMapOrientation)
            {
                string key = kvp.Key;
                double value = kvp.Value;

                if (key.Equals("N"))
                {
                    n += value;
                }else if (key.Equals("NE"))
                {
                    ne += value;
                }else if (key.Equals("E"))
                {
                    e += value;
                }else if (key.Equals("SE"))
                {
                    se += value;
                }else if (key.Equals("S"))
                {
                    s += value;
                }else if (key.Equals("W"))
                {
                    w += value;
                }
            }

            //test if matches
            double nsdifference = Math.Abs((n - s)/n);
            string reportKeyId = orientationToSurfaceMap["N"] + "," + orientationToSurfaceMap["S"];

            if (nsdifference == 0)
            {
                report.MessageDict.Add(reportKeyId, "The north wall surface area matches the south wall surface area, the difference is zero. (North: " + n + ") South: " + s + ")" );
                report.TestPassedDict.Add(reportKeyId, true);
                report.OutputTypeDict.Add(reportKeyId, OutPutEnum.Matched);
            }
            else if (nsdifference <= report.tolerance)
            {
                report.MessageDict.Add(reportKeyId, "The north wall surface area matches the south wall surface area, the difference is within the allowable tolerance. (North: " + n + ") South: " + s + ")");
                report.TestPassedDict.Add(reportKeyId, true);
                report.OutputTypeDict.Add(reportKeyId, OutPutEnum.Warning);
            }
            else
            {
                report.MessageDict.Add(reportKeyId, "The north wall surface area does not match the south wall surface area, the difference was not within tolerance = " + report.tolerance * 100 + "%" + ".  Difference of: " + nsdifference
                        + ".  " + n + " north walls surface area and " + s + " south wall surface area.");
                report.TestPassedDict.Add(reportKeyId, false);
                report.OutputTypeDict.Add(reportKeyId, OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "The north wall surface area does not match the south wall surface area.";
            }

            //test if matches
            double NESEdifference = Math.Abs((ne - se) / ne);
            reportKeyId = orientationToSurfaceMap["NE"] + "," + orientationToSurfaceMap["SE"];

            if (NESEdifference == 0)
            {
                report.MessageDict.Add(reportKeyId, "The north-east wall surface area matches the south-east wall surface area, the difference is zero. (North East: " + ne + ") South East: " + se + ")");
                report.TestPassedDict.Add(reportKeyId, true);
                report.OutputTypeDict.Add(reportKeyId, OutPutEnum.Matched);
            }
            else if (NESEdifference <= report.tolerance)
            {
                report.MessageDict.Add(reportKeyId, "The north-east wall surface area matches the south-east wall surface area, the difference is within the allowable tolerance. (North East: " + ne + ") South East: " + se + ")");
                report.TestPassedDict.Add(reportKeyId, true);
                report.OutputTypeDict.Add(reportKeyId, OutPutEnum.Warning);
            }
            else
            {
                report.MessageDict.Add(reportKeyId, "The north-east wall surface area does not match the south-east wall surface area, the difference was not within tolerance = " + report.tolerance * 100 + "%" + ".  Difference of: " + NESEdifference
                        + ".  " + ne + " north-east walls surface area and " + se + " south-east wall surface area.");
                report.TestPassedDict.Add(reportKeyId, false);
                report.OutputTypeDict.Add(reportKeyId, OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "The north-east wall surface area does not match the south-east wall surface area.";
            }

            //now check roof perimeter and the suppose area vs. actual area
            double actualArea = n + s + ne + se + e + w;
            double calculatedArea = 0.0;
            //retrieve all the exterior wall surfaces
            double linearLength = 0.0;
            foreach(SurfaceDefinitions sf in TestSurfaces)
            {
                if (sf.SurfaceType.Equals("ExteriorWall"))
                {
                    List<Vector.MemorySafe_CartCoord> plCoords = sf.PlCoords;
                    List<Vector.MemorySafe_CartCoord> higherCoords = new List<Vector.MemorySafe_CartCoord>();
                    for(int i=0; i<plCoords.Count; i++)
                    {
                        Vector.MemorySafe_CartCoord coord = plCoords[i];
                        if(coord.Z >= 10) //hard-code,we need a higher value.
                        {
                            higherCoords.Add(coord);
                        }
                    }
                    //find out the length of two points.
                    if(higherCoords.Count >= 2)
                    {
                        for(int j=0; j<higherCoords.Count-1; j++)
                        {
                            Vector.MemorySafe_CartCoord currentCoord = higherCoords[j];
                            Vector.MemorySafe_CartCoord nextCoord = higherCoords[j + 1];
                            linearLength += Math.Sqrt(Math.Pow(currentCoord.X - nextCoord.X, 2) + Math.Pow(currentCoord.Y - nextCoord.Y, 2) + Math.Pow(currentCoord.Z - nextCoord.Z, 2));
                        }
                    }
                }
            }
            calculatedArea = linearLength * 10;//10 is the height;
            //test if matches
            double areaMatch = Math.Abs((actualArea - calculatedArea) / actualArea);
            if (areaMatch == 0)
            {
                report.MessageDict.Add("area", "The calculated area matches the actual area, the difference is zero. (Calculated area: " + calculatedArea + "), Actual area: " + actualArea + ")");
                report.TestPassedDict.Add("area", true);
                report.OutputTypeDict.Add("area", OutPutEnum.Matched);
            }
            else if (areaMatch <= report.tolerance)
            {
                report.MessageDict.Add("area", "The calculated area matches the actual area, the difference is within the allowable tolerance. (Calculated area: " + calculatedArea + "), Actual area: " + actualArea + ")");
                report.TestPassedDict.Add("area", true);
                report.OutputTypeDict.Add("area", OutPutEnum.Warning);
            }
            else
            {
                report.MessageDict.Add("area", "The calculated area does not match the actual area, the difference was not within tolerance = " + report.tolerance.ToString() + " " + Units + ".  Difference of: " + calculatedArea
                        + ".  " + calculatedArea + ", calculated area and " + actualArea + ", calculated area.");
                report.TestPassedDict.Add("area", false);
                report.OutputTypeDict.Add("area", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "The calculated area does not match the actual area.";
            }

            return report;
        }

        public static DOEgbXMLReportingObj TestHVACSystem(DOEgbXMLReportingObj report, List<XmlDocument> gbXMLDocs, List<XmlNamespaceManager> gbXMLnsm, string Units)
        {
            //initialize the report
            report.passOrFail = true;
            report.outputType = OutPutEnum.Matched;
            report.longMsg = "The Test Model's HVAC matches Standard Model's HVAC exactly.";
            report.Type = ReportParamType.HVAC;

            //Step 1. get HVAC system
            DOEgbXMLPTAC testPTAC = new DOEgbXMLPTAC(gbXMLDocs[0], gbXMLnsm[0]);
            DOEgbXMLPTAC standardPTAC = new DOEgbXMLPTAC(gbXMLDocs[1], gbXMLnsm[1]);
            List<string> errorMessageList = testPTAC.errorMessageList;

            if(errorMessageList.Count > 0)
            {
                foreach(string s in errorMessageList)
                {
                    report.MessageList.Add(s);
                }
                report.longMsg = "The Test Model's HVAC are incomplete, the process is halted, Check detail message for the errors.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }

            //Step 1.2 - compare HVACs
            if (testPTAC.DesignCoolTemp != 74)
            {
                report.MessageDict.Add("CoolingTemperature", "Design cooling temperature does not equal to 74F");
                report.TestPassedDict.Add("CoolingTemperature", false);
                report.OutputTypeDict.Add("CoolingTemperature", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("CoolingTemperature", "Design cooling temperature equals to 74F");
                report.TestPassedDict.Add("CoolingTemperature", true);
                report.OutputTypeDict.Add("CoolingTemperature", OutPutEnum.Matched);
            }

            if(testPTAC.DesignHeatTemp != 70)
            {
                report.MessageDict.Add("HeatingTemperature", "Design cooling temperature does not equal to 70F");
                report.TestPassedDict.Add("HeatingTemperature", false);
                report.OutputTypeDict.Add("HeatingTemperature", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("HeatingTemperature", "Design cooling temperature equals to 70F");
                report.TestPassedDict.Add("HeatingTemperature", true);
                report.OutputTypeDict.Add("HeatingTemperature", OutPutEnum.Matched);
            }
            if(testPTAC.DesignOAFlowPerPerson != 5.3)
            {
                report.MessageDict.Add("DesignOAFlowPerPerson", "Design outdoor air flow per person does not equal to 5.3 CFM/person");
                report.TestPassedDict.Add("DesignOAFlowPerPerson", false);
                report.OutputTypeDict.Add("DesignOAFlowPerPerson", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("DesignOAFlowPerPerson", "Design outdoor air flow per person equals to 5.3 CFM/person");
                report.TestPassedDict.Add("DesignOAFlowPerPerson", true);
                report.OutputTypeDict.Add("DesignOAFlowPerPerson", OutPutEnum.Matched);
            }
            if (testPTAC.DesignOAFlowPerArea != 0.059)
            {
                report.MessageDict.Add("DesignOAFlowPerArea", "Design outdoor air flow per area does not equal to 0.059 CFM/sq.ft.");
                report.TestPassedDict.Add("DesignOAFlowPerArea", false);
                report.OutputTypeDict.Add("DesignOAFlowPerArea", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("DesignOAFlowPerArea", "Design outdoor air flow per area equals to 0.059 CFM/sq.ft.");
                report.TestPassedDict.Add("DesignOAFlowPerArea", true);
                report.OutputTypeDict.Add("DesignOAFlowPerArea", OutPutEnum.Matched);
            }

            //fan checks
            if (testPTAC.fan.FanControl != "Cycling")
            {
                report.MessageDict.Add("FanControl", "Fan control should be 'Cycling', but the test model has control of: " + testPTAC.fan.FanControl);
                report.TestPassedDict.Add("FanControl", false);
                report.OutputTypeDict.Add("FanControl", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("FanControl", "Fan control in test model is 'Cycling'");
                report.TestPassedDict.Add("FanControl", true);
                report.OutputTypeDict.Add("FanControl", OutPutEnum.Matched);
            }

            if (testPTAC.fan.MotorInStream != 1)
            {
                report.MessageDict.Add("MotorInAirStream", "Fan Motor in Air Stream shall equal to 1, the test model is: " + testPTAC.fan.MotorInStream);
                report.TestPassedDict.Add("MotorInAirStream", false);
                report.OutputTypeDict.Add("MotorInAirStream", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("MotorInAirStream", "Fan Motor in Air Stream equals to 1");
                report.TestPassedDict.Add("MotorInAirStream", true);
                report.OutputTypeDict.Add("MotorInAirStream", OutPutEnum.Matched);
            }

            if(testPTAC.fan.AirStreamFraction != 0.9)
            {
                report.MessageDict.Add("AirStreamFraction", "Fan air stream fraction shall equal to 0.9, the test model is: " + testPTAC.fan.AirStreamFraction);
                report.TestPassedDict.Add("AirStreamFraction", false);
                report.OutputTypeDict.Add("AirStreamFraction", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("AirStreamFraction", "Fan air stream fraction equals to 0.9");
                report.TestPassedDict.Add("AirStreamFraction", true);
                report.OutputTypeDict.Add("AirStreamFraction", OutPutEnum.Matched);
            }

            if (testPTAC.fan.DeltaP != 75)
            {
                report.MessageDict.Add("DeltaP", "Fan delta P shall equal to 75 Pa, the test model is: " + testPTAC.fan.AirStreamFraction + " Pa");
                report.TestPassedDict.Add("DeltaP", false);
                report.OutputTypeDict.Add("DeltaP", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("DeltaP", "Fan delta P equals to 75 Pa");
                report.TestPassedDict.Add("DeltaP", true);
                report.OutputTypeDict.Add("DeltaP", OutPutEnum.Matched);
            }
            if (testPTAC.fan.FanEff != 0.7)
            {
                report.MessageDict.Add("FanEfficiency", "Fan efficiency shall equal to 0.7, the test model is: " + testPTAC.fan.FanEff + " Pa");
                report.TestPassedDict.Add("FanEfficiency", false);
                report.OutputTypeDict.Add("FanEfficiency", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("FanEfficiency", "Fan efficiency equals to 0.7");
                report.TestPassedDict.Add("FanEfficiency", true);
                report.OutputTypeDict.Add("FanEfficiency", OutPutEnum.Matched);
            }

            //cooling coil
            if (testPTAC.CoolCoil.CoilEff != 3.0)
            {
                report.MessageDict.Add("CoolingCoilEfficiency", "Cooling coil efficiency shall equal to COP 3.0, the test model is: " + testPTAC.CoolCoil.CoilEff);
                report.TestPassedDict.Add("CoolingCoilEfficiency", false);
                report.OutputTypeDict.Add("CoolingCoilEfficiency", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("CoolingCoilEfficiency", "Cooling coil efficiency equals to COP 3.0");
                report.TestPassedDict.Add("CoolingCoilEfficiency", true);
                report.OutputTypeDict.Add("CoolingCoilEfficiency", OutPutEnum.Matched);
            }
            if (testPTAC.CoolCoil.Capacity != 85)
            {
                report.MessageDict.Add("CoolingCoilCapacity", "Cooling coil capacity shall equal to 85 kBtu/hr, the test model is: " + testPTAC.CoolCoil.Capacity + " kBtu/hr");
                report.TestPassedDict.Add("CoolingCoilCapacity", false);
                report.OutputTypeDict.Add("CoolingCoilCapacity", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("CoolingCoilCapacity", "Cooling coil capacity equals to 85 kBtu/hr");
                report.TestPassedDict.Add("CoolingCoilCapacity", true);
                report.OutputTypeDict.Add("CoolingCoilCapacity", OutPutEnum.Matched);
            }

            //heating coil
            if (testPTAC.HeatCoil.CoilEff != 0.8)
            {
                report.MessageDict.Add("HeatingCoilEfficiency", "Heating coil efficiency shall equal to 0.8, the test model is: " + testPTAC.HeatCoil.CoilEff);
                report.TestPassedDict.Add("HeatingCoilEfficiency", false);
                report.OutputTypeDict.Add("HeatingCoilEfficiency", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("HeatingCoilEfficiency", "Heating coil efficiency equals to 0.8");
                report.TestPassedDict.Add("HeatingCoilEfficiency", true);
                report.OutputTypeDict.Add("HeatingCoilEfficiency", OutPutEnum.Matched);
            }
            if (testPTAC.HeatCoil.Capacity != 102)
            {
                report.MessageDict.Add("HeatingCoilCapacity", "Heating coil capacity shall equal to 102 kBtu/hr, the test model is: " + testPTAC.HeatCoil.Capacity + " kBtu/hr");
                report.TestPassedDict.Add("HeatingCoilCapacity", false);
                report.OutputTypeDict.Add("HeatingCoilCapacity", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("HeatingCoilCapacity", "Heating coil capacity equals to 102 kBtu/hr");
                report.TestPassedDict.Add("HeatingCoilCapacity", true);
                report.OutputTypeDict.Add("HeatingCoilCapacity", OutPutEnum.Matched);
            }
            if (testPTAC.HeatCoil.ResourceType != "NaturalGas")
            {
                report.MessageDict.Add("HeatingCoilResource", "Heating coil resource shall be NaturalGas, the test model is: " + testPTAC.HeatCoil.ResourceType);
                report.TestPassedDict.Add("HeatingCoilResource", false);
                report.OutputTypeDict.Add("HeatingCoilResource", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "One of the HVAC variable does not match between test model and standard model";
            }
            else
            {
                report.MessageDict.Add("HeatingCoilResource", "Heating coil resource is NaturalGas");
                report.TestPassedDict.Add("HeatingCoilResource", true);
                report.OutputTypeDict.Add("HeatingCoilResource", OutPutEnum.Matched);
            }

            //compare the HVAC operation schedule
            DOEgbXMLSchedule testSchedule = new DOEgbXMLSchedule(gbXMLDocs[0], gbXMLnsm[0]);
            //check if the process is successful.
            Dictionary<string, string> scheduleMsg = testSchedule.ErrorMessage;
            if(scheduleMsg.Count > 0)
            {
                foreach (KeyValuePair<string, string> msgKVP in scheduleMsg)
                {
                    report.MessageList.Add(msgKVP.Value);
                }
                report.longMsg = "The Test File's Operation Schedules are incomplete, the process is halted, Check detail message for the errors.";
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                return report;
            }
            
            DOEgbXMLSchedule standardSchedule = new DOEgbXMLSchedule(gbXMLDocs[1], gbXMLnsm[1]);
            string testOperationSchedule = testPTAC.OperationScheduleId;
            string standardOperationSchedule = standardPTAC.OperationScheduleId;

            string testOperationType = testSchedule.SchedTypeMap[testOperationSchedule];
            string standardOperationType = standardSchedule.SchedTypeMap[standardOperationSchedule];

            if (!testOperationType.Equals(standardOperationType, StringComparison.InvariantCultureIgnoreCase))
            {
                report.MessageDict.Add("testOperationSchedule", "The Test Operation schedule: <a class='" + testOperationSchedule + "'>" + testOperationSchedule + "</a> type is different from standard operation schedule. " +
                    "Test operation schedule type: " + testOperationType + ", Stanard operation schedule type: " + standardOperationType);
                report.TestPassedDict.Add("testOperationSchedule", false);
                report.OutputTypeDict.Add("testOperationSchedule", OutPutEnum.Failed);
            }

            List<double> testAnnualOperationSchedule = testSchedule.SchedValueMap[testOperationSchedule];
            List<double> standardAnnualOperationSchedule = standardSchedule.SchedValueMap[standardOperationSchedule];
            //calculate the RSME
            if(testAnnualOperationSchedule.Count != standardAnnualOperationSchedule.Count)
            {
                report.MessageDict.Add("testOperationSchedule", "The Test operation schedule: <a class='" + testOperationSchedule + "'>" + testOperationSchedule + "</a> is not completed. Number of Datapoint in the test operation schedule: " + testAnnualOperationSchedule.Count +
                    "; Number of Datapoints in the standard operation schedule: " + standardAnnualOperationSchedule.Count);
                report.TestPassedDict.Add("testOperationSchedule", false);
                report.OutputTypeDict.Add("testOperationSchedule", OutPutEnum.Failed);
                report.passOrFail = false;
                report.outputType = OutPutEnum.Failed;
                report.longMsg = "The number of hours in Test Model Operation Schedule is: " + testAnnualOperationSchedule.Count + " it does not match the Standard Model: " + standardAnnualOperationSchedule.Count;
            }
            else
            {
                double sum = 0.0;
                for(int i=0; i<testAnnualOperationSchedule.Count; i++)
                {
                    double differences = testAnnualOperationSchedule[i] - standardAnnualOperationSchedule[i];
                    sum = sum + Math.Pow(differences, 2);
                }

                double rmse = Math.Sqrt(sum);
                if(rmse == 0)
                {
                    report.MessageDict.Add("schedule", "The Test operation schedule: <a class='" + testOperationSchedule + "'>" + testOperationSchedule + "</a> exactly matches the standard schedule");
                    report.TestPassedDict.Add("schedule", true);
                    report.OutputTypeDict.Add("schedule", OutPutEnum.Matched);
                }else if(rmse < report.tolerance)
                {
                    report.MessageDict.Add("schedule", "The Test operation schedule: <a class='" + testOperationSchedule + "'>" + testOperationSchedule + "</a> matches the standard schedule withint the tolerance, Tolerance (RMSE): " + report.tolerance);
                    report.TestPassedDict.Add("schedule", true);
                    report.OutputTypeDict.Add("schedule", OutPutEnum.Warning);
                }
                else
                {
                    report.MessageDict.Add("schedule", "The Test operation schedule: <a class='" + testOperationSchedule + "'>" + testOperationSchedule + "</a> does not match the standard schedule, The RMSE is: " + rmse + ", higher than the tolerance: " + report.tolerance);
                    report.TestPassedDict.Add("schedule", false);
                    report.OutputTypeDict.Add("schedule", OutPutEnum.Failed);
                    report.longMsg = "The Test Model's Operation Schedules does not match the Operation Schedule in the Standard Model.";
                    report.passOrFail = false;
                    report.outputType = OutPutEnum.Failed;
                }
            }

            return report;
        }
    }
}
