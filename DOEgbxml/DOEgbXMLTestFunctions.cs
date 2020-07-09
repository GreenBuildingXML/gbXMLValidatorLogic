using System;
using System.Collections.Generic;
using System.Xml;

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
            //added 07/07/2020

            report.testSummary = "";
            report.unit = Units;

            double testArea = 0.0;
            double standardArea = 0.0;

            for (int i = 0; i < StandardSurfaces.Count; i++)
            {
                if (StandardSurfaces[i].SurfaceType == "Shade")
                {


                }
            }
        }
    }
}
