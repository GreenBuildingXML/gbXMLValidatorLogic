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

            int mismatchCounter = 0;

            for(int i=0; i<standardSpaces.Count; i++)
            {
                gbXMLSpaces stdSpace = standardSpaces[i];
                Boolean matchFlag = false;
                //search for match
                for (int j = 0; j < testSpaces.Count; j++)
                {
                    //We are not checking the plenum 
                    if (stdSpace.spaceType != "Plenum")
                    {
                        if(stdSpace.name == testSpaces[j].name)
                        {
                            matchFlag = true;
                        }
                    }
                }

                if(!matchFlag)
                {
                    mismatchCounter++;
                    string msg = "Cannot find a match space name in Test Model for the space: " + stdSpace.name + ".";
                    report.MessageList.Add(msg);
                    report.testResult.Add("NA");
                    report.standResult.Add(stdSpace.name);
                    report.idList.Add(i+"");
                }
                else
                {
                    string msg = "Find a match space name in Test Model for the space: " + stdSpace.name + ".";
                    report.MessageList.Add(msg);
                    report.testResult.Add(stdSpace.name);
                    report.standResult.Add(stdSpace.name);
                    report.idList.Add(i + "");
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

        public static DOEgbXMLReportingObj TestSurfaceCountByType(List<SurfaceDefinitions> TestSurfaces, List<SurfaceDefinitions> StandardSurfaces,
            DOEgbXMLReportingObj report, string Units, String Type)
        {
            report.testSummary = "";
            report.unit = Units;

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

            Dictionary<String, Double> standardSurfaceAreaMapOrientation = new Dictionary<String, Double>();
            Dictionary<String, Double> testSurfaceAreaMapOrientation = new Dictionary<String, Double>();

            foreach(SurfaceDefinitions surfDef in TestSurfaces)
            {
                string orientation = surfDef.surfaceOrientation();
                if (!testSurfaceAreaMapOrientation.ContainsKey(orientation))
                {
                    testSurfaceAreaMapOrientation.Add(orientation, 0.0);
                }
                testSurfaceAreaMapOrientation[orientation] += surfDef.computeArea();
            }

            foreach(SurfaceDefinitions surfDef in StandardSurfaces)
            {
                string orientation = surfDef.surfaceOrientation();
                if (!standardSurfaceAreaMapOrientation.ContainsKey(orientation))
                {
                    standardSurfaceAreaMapOrientation.Add(orientation, 0.0);
                }
                standardSurfaceAreaMapOrientation[orientation] += surfDef.computeArea();
            }

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

            double testArea = 0.0;
            double standardArea = 0.0;

            for (int i = 0; i < StandardSurfaces.Count; i++)
            {
                if (StandardSurfaces[i].SurfaceType == Type)
                {
                    standardArea += StandardSurfaces[i].computeArea();
                }
            }

            for (int i = 0; i < TestSurfaces.Count; i++)
            {
                if (TestSurfaces[i].SurfaceType == Type)
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
    }
}
