using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOEgbXML
{
    public enum TestType
    {
        None,
        Building_Area,
        Space_Count,
        Building_Story_Count,
        Building_Story_Z_Height,
        Building_Story_PolyLoop_RHR,
        SpaceId_Match_Test,
        Space_Area,
        Space_Volume,
        Total_Surface_Count,
        Exterior_Wall_Surface_Count,
        Underground_Surface_Count,
        Interior_Wall_Surface_Count,
        Interior_Floor_Surface_Count,
        Roof_Surface_Count,
        Shading_Surface_Count,
        Air_Surface_Count,
        Surface_Planar_Test, //phase 1 and phase 2
        Detailed_Surface_Checks,
        Fixed_Windows_Count,
        Operable_Windows_Count,
        Fixed_Skylight_Count,
        Operable_Skylight_Count,
        Sliding_Doors_Count,
        Non_Sliding_Doors_Count,
        Air_Openings_Count,
        Opening_Planar_Test,
        Detailed_Opening_Checks,
        Shell_Geom_RHR,
        //phase 2 tests
        Unique_Space_ID_Test,
        Unique_Space_Boundary,
        Space_Surfaces_CC,
        Space_Surfaces_Planar,
        Check_Space_Enclosure,
        //surface tests
        At_Least_4_Surfaces,
        Two_Adj_Space_Id,
        Required_Surface_Fields,
        Surface_ID_Uniqueness,
        Surface_Adj_Id_Match,
        Surface_ID_SB_Match,
        Surface_Tilt_Az_Check,
        Surface_CC_Test,
        Check_Surface_Enclosure,
        Exterior_Wall_Area,
        Roof_Area,
        SlabOnGrade_Area,
        Shade_Area,
        Assembly_Test
    }
    public class DOEgbXMLenum
    {
    }
}