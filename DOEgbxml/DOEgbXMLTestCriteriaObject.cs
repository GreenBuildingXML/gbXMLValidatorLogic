using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOEgbXML
{
    public class DOEgbXMLTestCriteriaObject
    {
        public Dictionary<TestType, bool> TestCriteriaDictionary;
       
        public void InitializeTestCriteriaWithTestName(string testname)
        {
            TestCriteriaDictionary = new Dictionary<TestType, bool>();
            
           
            if (testname == "test1")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test , false);//uncheck ID
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume , true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count , false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count,false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count,false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
            }
            else if (testname == "test2")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);//shades must match
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                
            }
            else if (testname == "test3")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
            }
            else if (testname == "Test4")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, true);            //shades must match
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
            }
            else if (testname == "Test5")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, true);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, true);             //shading surface must match
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
            }
            else if (testname == "Test7")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
            }
            else if (testname == "Test8")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, true);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
            }
            else if (testname == "Test25")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count,false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count,false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
            }
            else if (testname == "Test28")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, true);           //shading count must match
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, true);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
            }
            else if (testname == "GenericPhase2")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Unique_Space_ID_Test, true);
                TestCriteriaDictionary.Add(TestType.Unique_Space_Boundary, true);
                TestCriteriaDictionary.Add(TestType.Space_Surfaces_CC, true);
                TestCriteriaDictionary.Add(TestType.Space_Surfaces_Planar, true);
                TestCriteriaDictionary.Add(TestType.Check_Space_Enclosure, true);
                TestCriteriaDictionary.Add(TestType.At_Least_4_Surfaces, true);
                TestCriteriaDictionary.Add(TestType.Two_Adj_Space_Id, true);
                TestCriteriaDictionary.Add(TestType.Required_Surface_Fields, true);
                TestCriteriaDictionary.Add(TestType.Surface_ID_Uniqueness, true);
                TestCriteriaDictionary.Add(TestType.Surface_Adj_Id_Match, true);
                TestCriteriaDictionary.Add(TestType.Surface_ID_SB_Match, true);
                TestCriteriaDictionary.Add(TestType.Surface_Tilt_Az_Check, true);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Surface_CC_Test, true);
                TestCriteriaDictionary.Add(TestType.Check_Surface_Enclosure, true);

            }
            else if (testname == "test15")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, true);
                TestCriteriaDictionary.Add(TestType.SpaceId_Match_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Volume, true);
                TestCriteriaDictionary.Add(TestType.Total_Surface_Count, true);
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Underground_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, true);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, true);
            }
        }
    }
}