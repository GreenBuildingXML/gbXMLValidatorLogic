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

        public static DOEgbXMLReportingObj TestMaterialAssembly(List<DOEgbXMLConstruction> TestConstructions, List<DOEgbXMLConstruction> StandardConstructions, DOEgbXMLReportingObj report, string Units)
        {

            return null;
        }
    }
}
