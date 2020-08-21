using System;
using System.Collections.Generic;
using System.Xml;
using VectorMath;

namespace DOEgbXML
{
    public class DOEgbXMLTestFunctions
    {
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

                    //test code: temp
                    StandardSurfaces[i].surfaceOrientation();
                }
            }

            for (int i = 0; i < TestSurfaces.Count; i++)
            {
                if (TestSurfaces[i].SurfaceType == Type)
                {
                    testArea += TestSurfaces[i].computeArea();

                    //test code: temp
                    TestSurfaces[i].surfaceOrientation();
                }
            }

            double difference = Math.Abs(standardArea - testArea);
            report.testResult.Add(testArea.ToString());
            report.standResult.Add(standardArea.ToString());
            report.idList.Add("");

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
                        + ".  " + standardArea + " exterior wall surfaces in the Standard File and " + testArea + " exterior wall surfaces in the Test File.";
                report.passOrFail = false;
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
                        + ".  " + standardArea + " shade surfaces in the Standard File and " + testArea + " shade surfaces in the Test File.";
                report.passOrFail = false;
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
                        report.longMsg = "The Test File's" + report.testType + " does not match the Standard File exactly. It failed the test.";
                        report.MessageList = messageList;
                        report.passOrFail = false;
                        return report;
                    }
                }
            }
            
            report.longMsg = "The Test File's" + report.testType + " matches the Standard File exactly. It pass the test.";
            report.MessageList = messageList;
            report.passOrFail = true;
            
            return report;
        }
    }
}
