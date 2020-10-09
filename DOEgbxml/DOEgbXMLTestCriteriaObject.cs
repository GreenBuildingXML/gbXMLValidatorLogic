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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, true);//need check air surface
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);

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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test4")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, true);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, true);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test5")
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
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);//shading surface must match
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, true);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, true);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test6")
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
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);//shading surface must match
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, true);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test7")
            {
                DOEgbXMLBasics.SliversAllowed = true;
                TestCriteriaDictionary.Add(TestType.Building_Area, true);
                TestCriteriaDictionary.Add(TestType.Space_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Count, true);
                TestCriteriaDictionary.Add(TestType.Building_Story_Z_Height, false);
                TestCriteriaDictionary.Add(TestType.Building_Story_PolyLoop_RHR, false);
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, true);
            }
            else if (testname == "test8")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, true);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, true);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test9")
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
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, true);//need at least 1 interior surface
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, true);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);

            }
            else if (testname == "test10")
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
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, true);//need at least 1 interior surface
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, true);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, true);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test11")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test12")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test13")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, true);//check balcony
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, true);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, true);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test14")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, true);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test15")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, true);
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
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, true);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test16")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, true);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, true);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test17")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, false);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
                TestCriteriaDictionary.Add(TestType.Curved_Wall_Test, true);
            }
            else if (testname == "test18")
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
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, true);
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
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, true);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, false);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
            else if (testname == "test19")
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
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Surface_Count, true);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Roof_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Shading_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Surface_Count, false);
                TestCriteriaDictionary.Add(TestType.Surface_Planar_Test, true);
                TestCriteriaDictionary.Add(TestType.Detailed_Surface_Checks, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Windows_Count, false);
                TestCriteriaDictionary.Add(TestType.Fixed_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Operable_Skylight_Count, false);
                TestCriteriaDictionary.Add(TestType.Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Non_Sliding_Doors_Count, false);
                TestCriteriaDictionary.Add(TestType.Air_Openings_Count, false);
                TestCriteriaDictionary.Add(TestType.Opening_Planar_Test, false);
                TestCriteriaDictionary.Add(TestType.Detailed_Opening_Checks, false);
                //As of Feb 13 2013, this test is for a future release.  Placeholder only
                TestCriteriaDictionary.Add(TestType.Shell_Geom_RHR, false);
                //As of July 07 2020, this test is added as requirements for certification L2 test
                //RP-1810
                TestCriteriaDictionary.Add(TestType.Exterior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Roof_Area, true);
                TestCriteriaDictionary.Add(TestType.SlabOnGrade_Area, true);
                TestCriteriaDictionary.Add(TestType.Shade_Area, false);
                TestCriteriaDictionary.Add(TestType.Window_Area_Test, true);
                TestCriteriaDictionary.Add(TestType.Interior_Wall_Area, true);
                TestCriteriaDictionary.Add(TestType.Interior_Floor_Area, false);
                TestCriteriaDictionary.Add(TestType.Ceiling_Area, false);
                TestCriteriaDictionary.Add(TestType.Assembly_Test, false);
                TestCriteriaDictionary.Add(TestType.Space_Name_Test, true);
                TestCriteriaDictionary.Add(TestType.Plenum_Volume_Test, false);
                TestCriteriaDictionary.Add(TestType.Air_Area, false);
                TestCriteriaDictionary.Add(TestType.HVAC_Test, false);
            }
        }
    }
}