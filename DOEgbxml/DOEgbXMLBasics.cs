using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VectorMath;

namespace DOEgbXML
{
    public class DOEgbXMLBasics
    {
        public class EdgeFamily
        {
            public List<Vector.MemorySafe_CartCoord> startendpt;
            public string sbdec;
            public List<EdgeFamily> relatedEdges;
        }

        public static bool SliversAllowed = true;

        public enum MeasurementUnits
        {
            cubicft,
            sqft,
            ft,
            spaces,
            levels,
            dimensionless
        }

        public class Tolerances
        {
            public const double ToleranceDefault = -999;
            public const double VolumeTolerance = 1.0;
            public const double AreaTolerance = 1.0;
            public const double SpaceAreaPercentageTolerance = 0.025;
            public const double lengthTolerance = 0.0001;

            public const double coordToleranceIP = 0.05;
            public const double coordToleranceM = 0.015;
            public const double dotproducttol = 0.0000005;

            //Level (aka - story) height difference tolerance in feet
            public const double LevelHeightTolerance = 0.1;
            public const double VectorAngleTolerance = 2.5;
            public const double SpaceAreaTolerance = 1;
            //all count tolerances
            public const double SpaceCountTolerance = 0;
            public const double LevelCountTolerance = 0;
            public const double SurfaceCountTolerance = 0;
            public const double ExteriorWallCountTolerance = 0;
            public const double InteriorWallCountTolerance = 0;
            public const double InteriorFloorCountTolerance = 0;
            public const double RoofCountTolerance = 0;
            public const double AirWallCountTolerance = 0;
            public const double OpeningCountTolerance = 0;
            public const double FixedWindowCountTolerance = 0;
            public const double OperableWindowCountTolerance = 0;
            public const double FixedSkylightCountTolerance = 0;
            public const double OperableSkylightCountTolerance = 0;
            public const double SlidingDoorCountTolerance = 0;
            public const double NonSlidingDoorCountTolerance = 0;
            public const double AirOpeningCountTolerance = 0;

            //surface tolerances
            public const double SurfaceHeightTolerance = 0.5; //feet
            public const double SurfaceWidthTolerance = 0.5; //feet
            public const double SurfaceTiltTolerance = 2.5; // degrees
            public const double SurfaceAzimuthTolerance = 2.5; //degrees
            public const double SurfaceInsPtXTolerance = 0.5; //feet
            public const double SurfaceInsPtYTolerance = 0.5; //feet
            public const double SurfaceInsPtZTolerance = 0.5; //feet
            public const double SurfacePLCoordTolerance = 0.5; //feet (3 inches)
            public const double SliverDimensionTolerance = 0.25; //feet
            public const double SurfaceAreaPercentageTolerance = 0.025;

            //opening tolerances
            public const double OpeningHeightTolerance = 0.5; //feet
            public const double OpeningWidthTolerance = 0.5; //feet
            public const double OpeningSurfaceInsPtXTolerance = 0.5; //feet
            public const double OpeningSurfaceInsPtYTolerance = 0.5; //feet
            public const double OpeningSurfaceInsPtZTolerance = 0.5; //feet
            public const double OpeningAreaPercentageTolerance = 0.025;

            //RP1810 new tolerances
            public const double RVALUE = 0.05; //ft2-F-hr/Btu
            public const double SPECIFICHEAT = 25;//BTUPerLbF
            public const double SCHEDULEDIFFERENCES = 0.1;//10% schedule differences
            public const double AreaPercentTolerance = 0.15; //10% tolerance
        }

        static public double FindTilt(Vector.MemorySafe_CartVect normalVector)
        {
            double calculatedTilt = -999;
            //may need to also take into account other factors that, at this stage, seem to not be important
            //building Direction of Relative North
            //zone Direction of Relative North
            //GlobalGeometryRules coordinate system
            //I may need to know this in the future then rotate the axis vectors I am making below

            //x-axis [1 0 0] points east, y-axis [0 1 0] points north, z-axis[0 0 1] points up to the sky
            //alignment with y axis means north pointing, alignment with z-axis means it is pointing up to the sky (like a flat roof)
            double nX = 0;
            double nY = 1;
            double nZ = 0;
            Vector.MemorySafe_CartVect northVector = new Vector.MemorySafe_CartVect(nX, nY, nZ);

            double uX = 0;
            double uY = 0;
            double uZ = 1;
            Vector.MemorySafe_CartVect upVector = new Vector.MemorySafe_CartVect(uX, uY, uZ);

            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = Vector.UnitVector(normalVector);
            //get tilt:  cross product of normal vector and upVector
            //since parallel and anti parallel vectors will return the same cross product [0,0,0] I need to filter out the antiparalll case
            if (normalVector.X == upVector.X * -1 && normalVector.Y == upVector.Y * -1 && normalVector.Z == upVector.Z * -1)
            {
                calculatedTilt = 180;
                return calculatedTilt;
            }
            else
            {
                Vector.MemorySafe_CartVect tiltVector = Vector.CrossProduct(normalVector, upVector);
                double tiltVectorMagnitude = Vector.VectorMagnitude(tiltVector);
                calculatedTilt = Math.Round(Math.Asin(tiltVectorMagnitude) * 180 / Math.PI, 2);
                return calculatedTilt;
            }
        }


        public static double FindAzimuth(Vector.MemorySafe_CartVect normalVector)
        {
            double calculatedAzimuth = -999;
            //may need to also take into account other factors that, at this stage, seem to not be important
            //building Direction of Relative North
            //zone Direction of Relative North
            //GlobalGeometryRules coordinate system
            //I may need to know this in the future then rotate the axis vectors I am making below

            //x-axis [1 0 0] points east, y-axis [0 1 0] points north, z-axis[0 0 1] points up to the sky
            //alignment with y axis means north pointing, alignment with z-axis means it is pointing up to the sky (like a flat roof)

            Vector.MemorySafe_CartVect northVector = new Vector.MemorySafe_CartVect(0, 1, 0);

            Vector.MemorySafe_CartVect southVector = new Vector.MemorySafe_CartVect(0, -1, 0);

            Vector.MemorySafe_CartVect eastVector = new Vector.MemorySafe_CartVect(1, 0, 0);

            Vector.MemorySafe_CartVect westVector = new Vector.MemorySafe_CartVect(-1, 0, 0);

            Vector.MemorySafe_CartVect upVector = new Vector.MemorySafe_CartVect(0, 0, 1);

            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = Vector.UnitVector(normalVector);
            //get X-Y projection of the normal vector
            //normalVector.Z = 0;
            //get azimuth:  cross product of normal vector x-y projection and northVector
            //1st quadrant
            if ((normalVector.X == 0 && normalVector.Y == 1) || (normalVector.X == 1 && normalVector.Y == 0) || (normalVector.X > 0 && normalVector.Y > 0))
            {
                //get azimuth:  cross product of normal vector x-y projection and northVector
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, northVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2);

                if(calculatedAzimuth == 0)
                {
                    //in case it is clash with the horizontal surface : floor or ceiling or roof
                    calculatedAzimuth = 360;
                }

                return calculatedAzimuth;
            }
            //second quadrant
            else if (normalVector.X < 0 && normalVector.Y > 0)
            {
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, westVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 270;
                return calculatedAzimuth;
            }
            //quadrant 3
            else if ((normalVector.X < 0 && normalVector.Y < 0) || (normalVector.X == -1 && normalVector.Y == 0))
            {
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, southVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 180;
                return calculatedAzimuth;
            }
            //quadrant 4
            else if ((normalVector.X > 0 && normalVector.Y < 0) || (normalVector.X == 0 && normalVector.Y == -1))
            {
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, eastVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 90;
                return calculatedAzimuth;
            }
            //this will happen to vectors that point straight down or straight up because we are only interested in the X-Y projection and set the Z to zero anyways
            else if (normalVector.X == 0 && normalVector.Y == 0)
            {
                calculatedAzimuth = 0;
                return calculatedAzimuth;
            }

            //get the 

            return calculatedAzimuth;
        }


        public static string getFaceDirection(double calculatedAzimuth)
        {

            if(calculatedAzimuth > 0 && calculatedAzimuth <= 22.5)
            {
                return "N";
            }
            if(calculatedAzimuth > 22.5 && calculatedAzimuth <= 67.5)
            {
                return "NE";
            }
            if(calculatedAzimuth > 67.5 && calculatedAzimuth <= 110.5)
            {
                return "E";
            }
            if(calculatedAzimuth > 110.5 && calculatedAzimuth <= 157.5)
            {
                return "SE";
            }
            if(calculatedAzimuth > 157.5 && calculatedAzimuth <= 202.5)
            {
                return "S";
            }
            if(calculatedAzimuth > 202.5 && calculatedAzimuth <= 247.5)
            {
                return "SW";
            }
            if(calculatedAzimuth > 247.5 && calculatedAzimuth <= 292.5)
            {
                return "W";
            }
            if(calculatedAzimuth > 292.5 && calculatedAzimuth <= 337.5)
            {
                return "NW";
            }
            if(calculatedAzimuth > 337.5 && calculatedAzimuth <= 360)
            {
                return "N";
            }

            return "UP";

        }

        public static void FindMatchingEdges(List<gbXMLSpaces.SpaceBoundary> sblist)
        {

            Dictionary<int, DOEgbXMLBasics.EdgeFamily> edges = new Dictionary<int, DOEgbXMLBasics.EdgeFamily>();
            int distinctedges = 0;
            foreach (gbXMLSpaces.SpaceBoundary sb in sblist)
            {
                int coordcount = sb.sbplane.pl.plcoords.Count;
                for (int i = 0; i < coordcount; i++)
                {
                    //test edge
                    DOEgbXMLBasics.EdgeFamily edge = new DOEgbXMLBasics.EdgeFamily();
                    edge.sbdec = sb.surfaceIdRef;
                    edge.relatedEdges = new List<DOEgbXMLBasics.EdgeFamily>();
                    edge.startendpt = new List<Vector.MemorySafe_CartCoord>();
                    if (edges.Count == 0)
                    {
                        edges[distinctedges] = edge;
                        edge.startendpt.Add(sb.sbplane.pl.plcoords[i]);
                        edge.startendpt.Add(sb.sbplane.pl.plcoords[i + 1]);
                        edge.sbdec = sb.surfaceIdRef;

                        distinctedges++;
                        continue;

                    }
                    //most edges work the same, in terms of the start and end point, except for the last edge (the else case)
                    if (i < coordcount - 1)
                    {
                        edge.startendpt.Add(sb.sbplane.pl.plcoords[i]);
                        edge.startendpt.Add(sb.sbplane.pl.plcoords[i + 1]);
                        //search through existing edges to try and find a perfect match
                        int edgecount = 0; //keeps track of how many guest edges in the dictionary I've searched through
                        foreach (KeyValuePair<int, DOEgbXMLBasics.EdgeFamily> kp in edges)
                        {

                            Vector.MemorySafe_CartCoord startpt = kp.Value.startendpt[0];
                            //tolerance needed?
                            if (startpt.X == edge.startendpt[0].X && startpt.Y == edge.startendpt[0].Y && startpt.Z == edge.startendpt[0].Z)
                            {
                                //found at least one perfect coordinate match, try to match the second
                                Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                if (endpt.X == edge.startendpt[1].X && endpt.Y == edge.startendpt[1].Y && endpt.Z == edge.startendpt[1].Z)
                                {
                                    //both match, means the match is perfect, so add it to the related surfaces list
                                    kp.Value.relatedEdges.Add(edge);
                                    break;
                                }
                                else
                                {
                                    //the edge may be unique, though it could still have neighboring relationships
                                    //draw vector A
                                    double Ax = endpt.X - edge.startendpt[1].X;
                                    double Ay = endpt.Y - edge.startendpt[1].Y;
                                    double Az = endpt.Z - edge.startendpt[1].Z;
                                    Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                    double Amag = Vector.VectorMagnitude(A);

                                    //take cross product to see if they are even in same plane
                                    double evX = endpt.X - startpt.X;
                                    double evY = endpt.Y - startpt.Y;
                                    double evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double evmag = Vector.VectorMagnitude(ev);
                                    Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                    double crossmag = Vector.VectorMagnitude(cross);
                                    //tolerance?
                                    if (crossmag == 0)
                                    {
                                        //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                        double Bx = startpt.X - edge.startendpt[1].X;
                                        double By = startpt.Y - edge.startendpt[1].Y;
                                        double Bz = startpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                        double Bmag = Vector.VectorMagnitude(B);
                                        //check to see if the test edge is inside the guest edge
                                        if (Amag < evmag && Bmag < evmag)
                                        {
                                            //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                            kp.Value.relatedEdges.Add(edge);
                                            //accumulate its own relationships
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }

                                        double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                        double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                        double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                        double edgemag = Vector.VectorMagnitude(edgevec);

                                        double Cx = startpt.X - edge.startendpt[1].X;
                                        double Cy = startpt.Y - edge.startendpt[1].Y;
                                        double Cz = startpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);

                                        double Dx = endpt.X - edge.startendpt[1].X;
                                        double Dy = endpt.Y - edge.startendpt[1].Y;
                                        double Dz = endpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                        double Dmag = Vector.VectorMagnitude(D);

                                        if (Dmag < edgemag && Cmag < edgemag)
                                        {
                                            //this means the test edge is longer than the guest edge, but they overlap
                                            kp.Value.relatedEdges.Add(edge);
                                            //the edge is still unique but accumulates a neighbor
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }

                                    }
                                    else
                                    {
                                        //this other point isn't relevant, and the edges don't coincide
                                        edgecount++;
                                        continue;
                                    }

                                }


                            }
                            else if (startpt.X == edge.startendpt[1].X && startpt.Y == edge.startendpt[1].Y && startpt.Z == edge.startendpt[1].Z)
                            {
                                //found at least one perfect coordinate match, try to match the second
                                Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                if (endpt.X == edge.startendpt[0].X && endpt.Y == edge.startendpt[0].Y && endpt.Z == edge.startendpt[0].Z)
                                {
                                    //both match, means the match is perfect, so add it to the related surfaces list
                                    kp.Value.relatedEdges.Add(edge);
                                    break;

                                }
                                else
                                {
                                    //the edge may be unique, though it could still have neighboring relationships
                                    double Ax = endpt.X - edge.startendpt[0].X;
                                    double Ay = endpt.Y - edge.startendpt[0].Y;
                                    double Az = endpt.Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                    double Amag = Vector.VectorMagnitude(A);

                                    //take cross product to see if they are even in same plane
                                    double evX = endpt.X - startpt.X;
                                    double evY = endpt.Y - startpt.Y;
                                    double evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double evmag = Vector.VectorMagnitude(ev);
                                    Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                    double crossmag = Vector.VectorMagnitude(cross);
                                    //tolerance?
                                    if (crossmag == 0)
                                    {
                                        //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                        double Bx = startpt.X - edge.startendpt[0].X;
                                        double By = startpt.Y - edge.startendpt[0].Y;
                                        double Bz = startpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                        double Bmag = Vector.VectorMagnitude(B);
                                        //check to see if the test edge is inside the guest edge
                                        if (Amag < evmag && Bmag < evmag)
                                        {
                                            //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                            kp.Value.relatedEdges.Add(edge);
                                            //accumulate its own relationships
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }

                                        double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                        double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                        double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                        double edgemag = Vector.VectorMagnitude(edgevec);

                                        double Cx = startpt.X - edge.startendpt[0].X;
                                        double Cy = startpt.Y - edge.startendpt[0].Y;
                                        double Cz = startpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);

                                        double Dx = endpt.X - edge.startendpt[0].X;
                                        double Dy = endpt.Y - edge.startendpt[0].Y;
                                        double Dz = endpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                        double Dmag = Vector.VectorMagnitude(D);

                                        if (Dmag < edgemag && Cmag < edgemag)
                                        {
                                            //this means the test edge is longer than the guest edge, but they overlap
                                            kp.Value.relatedEdges.Add(edge);
                                            //the edge is still unique but accumulates a neighbor
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //this other point isn't relevant, and the edges don't coincide
                                        edgecount++;
                                        continue;
                                    }
                                }

                            }
                            //neither points perfectly coincide, so we do an exhaustive overlap check.
                            else
                            {
                                Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                //are the two vectors even parallel?  because if they are not, no need to get more complex
                                double evX = endpt.X - startpt.X;
                                double evY = endpt.Y - startpt.Y;
                                double evZ = endpt.Z - startpt.Z;
                                Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                Vector.MemorySafe_CartVect edgev = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                if (Vector.VectorMagnitude(Vector.CrossProduct(ev, edgev)) != 0)
                                {
                                    //they are not even parallel so move on
                                    edgecount++;
                                    continue;
                                }

                                //try to determine if the two edges are parallel
                                //test edge point 1
                                double Ax = endpt.X - edge.startendpt[0].X;
                                double Ay = endpt.Y - edge.startendpt[0].Y;
                                double Az = endpt.Z - edge.startendpt[0].Z;
                                Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                double Amag = Vector.VectorMagnitude(A);

                                //take cross product to see if they are even in same plane
                                evX = endpt.X - startpt.X;
                                evY = endpt.Y - startpt.Y;
                                evZ = endpt.Z - startpt.Z;
                                Vector.MemorySafe_CartVect ev1 = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                double guestmag = Vector.VectorMagnitude(ev1);
                                Vector.MemorySafe_CartVect cross1 = Vector.CrossProduct(A, ev1);
                                double crossmag = Vector.VectorMagnitude(cross1);
                                //tolerance?
                                if (crossmag == 0)
                                {
                                    //we are at least parallel, now to check for a real intersection
                                    double Bx = startpt.X - edge.startendpt[0].X;
                                    double By = startpt.Y - edge.startendpt[0].Y;
                                    double Bz = startpt.Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                    double Bmag = Vector.VectorMagnitude(B);
                                    //check to see if the test edge's first point (index 0) is totally inside the guest edge
                                    if (Amag < guestmag && Bmag < guestmag)
                                    {
                                        //the start point of the test edge is inside the guest edge
                                        //test edge point 2 against guest edge point 2
                                        double Cx = endpt.X - edge.startendpt[1].X;
                                        double Cy = endpt.Y - edge.startendpt[1].Y;
                                        double Cz = endpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);
                                        Vector.MemorySafe_CartVect cross2 = Vector.CrossProduct(C, ev);
                                        crossmag = Vector.VectorMagnitude(cross2);
                                        if (crossmag == 0)
                                        {
                                            //we are at least parallel, in fact we have proven we are totall parallel, now intersect
                                            double Dx = startpt.X - edge.startendpt[1].X;
                                            double Dy = startpt.Y - edge.startendpt[1].Y;
                                            double Dz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);
                                            if (Cmag < guestmag && Dmag < guestmag)
                                            {
                                                //then it is inside as well, and test vector is engulfed by guest vector
                                                kp.Value.relatedEdges.Add(edge);
                                                //but the edge is still itself unique
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                continue;
                                            }
                                            else
                                            {
                                                //I am pretty sure that by default, they are still neighbors and this is no difference
                                                //it simply extends beyond one of the ends of the guest vector
                                                kp.Value.relatedEdges.Add(edge);
                                                //but the edge is still itself unique
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                continue;
                                            }


                                        }
                                        else
                                        {
                                            //we are not parallel, so this is not an adjacency match
                                            edgecount++;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //if test edge start point [index 0] is outside, is one of the guest points inside?
                                        //already computed B
                                        double Cx = startpt.X - edge.startendpt[1].X;
                                        double Cy = startpt.Y - edge.startendpt[1].Y;
                                        double Cz = startpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);

                                        edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                        edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                        edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                        double edgemag = Vector.VectorMagnitude(edgevec);

                                        if (Cmag < edgemag && Bmag < edgemag)
                                        {
                                            //the guest edge's start point is inside the test edge
                                            //guest edge point 2 
                                            double Dx = endpt.X - edge.startendpt[1].X;
                                            double Dy = endpt.Y - edge.startendpt[1].Y;
                                            double Dz = endpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);
                                            Vector.MemorySafe_CartVect cross3 = Vector.CrossProduct(D, edgevec);
                                            crossmag = Vector.VectorMagnitude(cross3);
                                            if (crossmag == 0)
                                            {
                                                //then we know the two edges are totall parallel and lined up
                                                //determine if the guest edge point 2 is inside the test edge or outside of it
                                                double Ex = startpt.X - edge.startendpt[1].X;
                                                double Ey = startpt.Y - edge.startendpt[1].Y;
                                                double Ez = startpt.Z - edge.startendpt[1].Z;
                                                Vector.MemorySafe_CartVect E = new Vector.MemorySafe_CartVect(Ex, Ey, Ez);
                                                double Emag = Vector.VectorMagnitude(E);
                                                if (Dmag < edgemag && Emag < edgemag)
                                                {
                                                    //it is inside
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    continue;
                                                }
                                                else
                                                {
                                                    //it is outside 
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                //we are not parallel, so this is not an adjacency match
                                                edgecount++;
                                                continue;
                                            }

                                        }
                                    }



                                }
                                else
                                {
                                    //they are not even parallel, so it is likely best just to shove on
                                    edgecount++;
                                    continue;
                                }


                            }
                        }
                        //this determines if it found a matching edge
                        if (edgecount == edges.Count)
                        {
                            edges.Add(distinctedges, edge);
                            distinctedges++;
                        }

                    }
                    //last edge end edge is the zero index   
                    else
                    {
                        edge.startendpt.Add(sb.sbplane.pl.plcoords[i]);
                        edge.startendpt.Add(sb.sbplane.pl.plcoords[0]);
                        int edgecount = 0; //keeps track of how many guest edges in the dictionary I've searched through
                        foreach (KeyValuePair<int, DOEgbXMLBasics.EdgeFamily> kp in edges)
                        {

                            Vector.MemorySafe_CartCoord startpt = kp.Value.startendpt[0];
                            //tolerance needed?
                            if (startpt.X == edge.startendpt[0].X && startpt.Y == edge.startendpt[0].Y && startpt.Z == edge.startendpt[0].Z)
                            {
                                //found at least one perfect coordinate match, try to match the second
                                Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                if (endpt.X == edge.startendpt[1].X && endpt.Y == edge.startendpt[1].Y && endpt.Z == edge.startendpt[1].Z)
                                {
                                    //both match, means the match is perfect, so add it to the related surfaces list
                                    kp.Value.relatedEdges.Add(edge);
                                    break;
                                }
                                else
                                {
                                    //the edge may be unique, though it could still have neighboring relationships
                                    //draw vector A
                                    double Ax = endpt.X - edge.startendpt[1].X;
                                    double Ay = endpt.Y - edge.startendpt[1].Y;
                                    double Az = endpt.Z - edge.startendpt[1].Z;
                                    Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                    double Amag = Vector.VectorMagnitude(A);

                                    //take cross product to see if they are even in same plane
                                    double evX = endpt.X - startpt.X;
                                    double evY = endpt.Y - startpt.Y;
                                    double evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double evmag = Vector.VectorMagnitude(ev);
                                    Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                    double crossmag = Vector.VectorMagnitude(cross);
                                    //tolerance?
                                    if (crossmag == 0)
                                    {
                                        //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                        double Bx = startpt.X - edge.startendpt[1].X;
                                        double By = startpt.Y - edge.startendpt[1].Y;
                                        double Bz = startpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                        double Bmag = Vector.VectorMagnitude(B);
                                        //check to see if the test edge is inside the guest edge
                                        if (Amag < evmag && Bmag < evmag)
                                        {
                                            //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                            kp.Value.relatedEdges.Add(edge);
                                            //accumulate its own relationships
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }

                                        double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                        double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                        double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                        double edgemag = Vector.VectorMagnitude(edgevec);

                                        double Cx = startpt.X - edge.startendpt[1].X;
                                        double Cy = startpt.Y - edge.startendpt[1].Y;
                                        double Cz = startpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);

                                        double Dx = endpt.X - edge.startendpt[1].X;
                                        double Dy = endpt.Y - edge.startendpt[1].Y;
                                        double Dz = endpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                        double Dmag = Vector.VectorMagnitude(D);

                                        if (Dmag < edgemag && Cmag < edgemag)
                                        {
                                            //this means the test edge is longer than the guest edge, but they overlap
                                            kp.Value.relatedEdges.Add(edge);
                                            //the edge is still unique but accumulates a neighbor
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }

                                    }
                                    else
                                    {
                                        //this other point isn't relevant, and the edges don't coincide
                                        edgecount++;
                                        continue;
                                    }

                                }


                            }
                            else if (startpt.X == edge.startendpt[1].X && startpt.Y == edge.startendpt[1].Y && startpt.Z == edge.startendpt[1].Z)
                            {
                                //found at least one perfect coordinate match, try to match the second
                                Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                if (endpt.X == edge.startendpt[0].X && endpt.Y == edge.startendpt[0].Y && endpt.Z == edge.startendpt[0].Z)
                                {
                                    //both match, means the match is perfect, so add it to the related surfaces list
                                    kp.Value.relatedEdges.Add(edge);
                                    break;

                                }
                                else
                                {
                                    //the edge may be unique, though it could still have neighboring relationships
                                    double Ax = endpt.X - edge.startendpt[0].X;
                                    double Ay = endpt.Y - edge.startendpt[0].Y;
                                    double Az = endpt.Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                    double Amag = Vector.VectorMagnitude(A);

                                    //take cross product to see if they are even in same plane
                                    double evX = endpt.X - startpt.X;
                                    double evY = endpt.Y - startpt.Y;
                                    double evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double evmag = Vector.VectorMagnitude(ev);
                                    Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                    double crossmag = Vector.VectorMagnitude(cross);
                                    //tolerance?
                                    if (crossmag == 0)
                                    {
                                        //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                        double Bx = startpt.X - edge.startendpt[0].X;
                                        double By = startpt.Y - edge.startendpt[0].Y;
                                        double Bz = startpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                        double Bmag = Vector.VectorMagnitude(B);
                                        //check to see if the test edge is inside the guest edge
                                        if (Amag < evmag && Bmag < evmag)
                                        {
                                            //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                            kp.Value.relatedEdges.Add(edge);
                                            //accumulate its own relationships
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }

                                        double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                        double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                        double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                        double edgemag = Vector.VectorMagnitude(edgevec);

                                        double Cx = startpt.X - edge.startendpt[0].X;
                                        double Cy = startpt.Y - edge.startendpt[0].Y;
                                        double Cz = startpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);

                                        double Dx = endpt.X - edge.startendpt[0].X;
                                        double Dy = endpt.Y - edge.startendpt[0].Y;
                                        double Dz = endpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                        double Dmag = Vector.VectorMagnitude(D);

                                        if (Dmag < edgemag && Cmag < edgemag)
                                        {
                                            //this means the test edge is longer than the guest edge, but they overlap
                                            kp.Value.relatedEdges.Add(edge);
                                            //the edge is still unique but accumulates a neighbor
                                            edge.relatedEdges.Add(kp.Value);
                                            edgecount++;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //this other point isn't relevant, and the edges don't coincide
                                        edgecount++;
                                        continue;
                                    }
                                }

                            }
                            //neither points perfectly coincide, so we do an exhaustive overlap check.
                            else
                            {
                                Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                //are the two vectors even parallel?  because if they are not, no need to get more complex
                                double evX = endpt.X - startpt.X;
                                double evY = endpt.Y - startpt.Y;
                                double evZ = endpt.Z - startpt.Z;
                                Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                Vector.MemorySafe_CartVect edgev = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                if (Vector.VectorMagnitude(Vector.CrossProduct(ev, edgev)) != 0)
                                {
                                    //they are not even parallel so move on
                                    edgecount++;
                                    continue;
                                }
                                //try to determine if the two edges are parallel

                                //test edge point 1
                                double Ax = endpt.X - edge.startendpt[0].X;
                                double Ay = endpt.Y - edge.startendpt[0].Y;
                                double Az = endpt.Z - edge.startendpt[0].Z;
                                Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                double Amag = Vector.VectorMagnitude(A);

                                //take cross product to see if they are even in same plane
                                evX = endpt.X - startpt.X;
                                evY = endpt.Y - startpt.Y;
                                evZ = endpt.Z - startpt.Z;
                                Vector.MemorySafe_CartVect ev1 = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                double guestmag = Vector.VectorMagnitude(ev);
                                Vector.MemorySafe_CartVect cross1 = Vector.CrossProduct(A, ev);
                                double crossmag = Vector.VectorMagnitude(cross1);
                                //tolerance?
                                if (crossmag == 0)
                                {
                                    //we are at least parallel, now to check for a real intersection
                                    double Bx = startpt.X - edge.startendpt[0].X;
                                    double By = startpt.Y - edge.startendpt[0].Y;
                                    double Bz = startpt.Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                    double Bmag = Vector.VectorMagnitude(B);
                                    //check to see if the test edge's first point (index 0) is totally inside the guest edge
                                    if (Amag < guestmag && Bmag < guestmag)
                                    {
                                        //the start point of the test edge is inside the guest edge
                                        //test edge point 2 against guest edge point 2
                                        double Cx = endpt.X - edge.startendpt[1].X;
                                        double Cy = endpt.Y - edge.startendpt[1].Y;
                                        double Cz = endpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);
                                        Vector.MemorySafe_CartVect cross2 = Vector.CrossProduct(C, ev);
                                        crossmag = Vector.VectorMagnitude(cross2);
                                        if (crossmag == 0)
                                        {
                                            //we are at least parallel, in fact we have proven we are totall parallel, now intersect
                                            double Dx = startpt.X - edge.startendpt[1].X;
                                            double Dy = startpt.Y - edge.startendpt[1].Y;
                                            double Dz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);
                                            if (Cmag < guestmag && Dmag < guestmag)
                                            {
                                                //then it is inside as well, and test vector is engulfed by guest vector
                                                kp.Value.relatedEdges.Add(edge);
                                                //but the edge is still itself unique
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                continue;
                                            }
                                            else
                                            {
                                                //I am pretty sure that by default, they are still neighbors and this is no difference
                                                //it simply extends beyond one of the ends of the guest vector
                                                kp.Value.relatedEdges.Add(edge);
                                                //but the edge is still itself unique
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                continue;
                                            }


                                        }
                                        else
                                        {
                                            //we are not parallel, so this is not an adjacency match
                                            edgecount++;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //if test edge start point [index 0] is outside, is one of the guest points inside?
                                        //already computed B
                                        double Cx = startpt.X - edge.startendpt[1].X;
                                        double Cy = startpt.Y - edge.startendpt[1].Y;
                                        double Cz = startpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                        double Cmag = Vector.VectorMagnitude(C);

                                        edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                        edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                        edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                        double edgemag = Vector.VectorMagnitude(edgevec);

                                        if (Cmag < edgemag && Bmag < edgemag)
                                        {
                                            //the guest edge's start point is inside the test edge
                                            //guest edge point 2 
                                            double Dx = endpt.X - edge.startendpt[1].X;
                                            double Dy = endpt.Y - edge.startendpt[1].Y;
                                            double Dz = endpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);
                                            Vector.MemorySafe_CartVect cross3 = Vector.CrossProduct(D, edgevec);
                                            crossmag = Vector.VectorMagnitude(cross3);
                                            if (crossmag == 0)
                                            {
                                                //then we know the two edges are totall parallel and lined up
                                                //determine if the guest edge point 2 is inside the test edge or outside of it
                                                double Ex = startpt.X - edge.startendpt[1].X;
                                                double Ey = startpt.Y - edge.startendpt[1].Y;
                                                double Ez = startpt.Z - edge.startendpt[1].Z;
                                                Vector.MemorySafe_CartVect E = new Vector.MemorySafe_CartVect(Ex, Ey, Ez);
                                                double Emag = Vector.VectorMagnitude(E);
                                                if (Dmag < edgemag && Emag < edgemag)
                                                {
                                                    //it is inside
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    continue;
                                                }
                                                else
                                                {
                                                    //it is outside 
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                //we are not parallel, so this is not an adjacency match
                                                edgecount++;
                                                continue;
                                            }

                                        }
                                    }



                                }
                                else
                                {
                                    //they are not even parallel, so it is likely best just to shove on
                                    edgecount++;
                                    continue;
                                }


                            }
                        }
                        //this determines if it found a matching edge
                        if (edgecount == edges.Count)
                        {
                            edges.Add(distinctedges, edge);
                            distinctedges++;
                        }
                    }

                }
            }
        }

        //this helps me to create an in-memory way to store the relationship between different edges.
        public static DOEgbXMLReportingObj FindMatchingEdges(DOEgbXML.gbXMLSpaces.ClosedShell cs, DOEgbXMLReportingObj report)
        {
            report.MessageList.Add("Starting test to find edge relationships for space closed shell polyloops.");
            try
            {
                Dictionary<int, DOEgbXMLBasics.EdgeFamily> edges = new Dictionary<int, DOEgbXMLBasics.EdgeFamily>();
                int distinctedges = 0;
                for (int plcount = 0; plcount < cs.ploops.Count(); plcount++)
                {
                    gbXMLSpaces.PolyLoop pl = cs.ploops[plcount];
                    int coordcount = pl.plcoords.Count;
                    for (int i = 0; i < coordcount; i++)
                    {
                        //test edge
                        DOEgbXMLBasics.EdgeFamily edge = new DOEgbXMLBasics.EdgeFamily();
                        edge.sbdec = plcount.ToString();
                        edge.relatedEdges = new List<DOEgbXMLBasics.EdgeFamily>();
                        edge.startendpt = new List<Vector.MemorySafe_CartCoord>();
                        if (edges.Count == 0)
                        {
                            edges[distinctedges] = edge;
                            edge.startendpt.Add(pl.plcoords[i]);
                            edge.startendpt.Add(pl.plcoords[i + 1]);
                            edge.sbdec = plcount.ToString();

                            distinctedges++;
                            continue;

                        }
                        //most edges work the same, in terms of the start and end point, except for the last edge (the else case)
                        if (i < coordcount - 1)
                        {
                            edge.startendpt.Add(pl.plcoords[i]);
                            edge.startendpt.Add(pl.plcoords[i + 1]);
                            //search through existing edges to try and find a perfect match
                            int edgecount = 0; //keeps track of how many guest edges in the dictionary I've searched through
                            foreach (KeyValuePair<int, DOEgbXMLBasics.EdgeFamily> kp in edges)
                            {

                                Vector.MemorySafe_CartCoord startpt = kp.Value.startendpt[0];
                                //tolerance needed?
                                if (startpt.X == edge.startendpt[0].X && startpt.Y == edge.startendpt[0].Y && startpt.Z == edge.startendpt[0].Z)
                                {
                                    //found at least one perfect coordinate match, try to match the second
                                    Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                    if (endpt.X == edge.startendpt[1].X && endpt.Y == edge.startendpt[1].Y && endpt.Z == edge.startendpt[1].Z)
                                    {
                                        //both match, means the match is perfect, so add it to the related surfaces list
                                        kp.Value.relatedEdges.Add(edge);
                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                        report.MessageList.Add("Edge "+startcoord+" , "+endcoord+" found a perfect match");
                                        break;
                                    }
                                    else
                                    {
                                        //the edge may be unique, though it could still have neighboring relationships
                                        //draw vector A
                                        double Ax = endpt.X - edge.startendpt[1].X;
                                        double Ay = endpt.Y - edge.startendpt[1].Y;
                                        double Az = endpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                        double Amag = Vector.VectorMagnitude(A);

                                        //take cross product to see if they are even in same plane
                                        double evX = endpt.X - startpt.X;
                                        double evY = endpt.Y - startpt.Y;
                                        double evZ = endpt.Z - startpt.Z;
                                        Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                        double evmag = Vector.VectorMagnitude(ev);
                                        Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                        double crossmag = Vector.VectorMagnitude(cross);
                                        //tolerance?
                                        if (crossmag == 0)
                                        {
                                            //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                            double Bx = startpt.X - edge.startendpt[1].X;
                                            double By = startpt.Y - edge.startendpt[1].Y;
                                            double Bz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                            double Bmag = Vector.VectorMagnitude(B);
                                            //check to see if the test edge is inside the guest edge
                                            if (Amag < evmag && Bmag < evmag)
                                            {
                                                //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                                kp.Value.relatedEdges.Add(edge);
                                                //accumulate its own relationships
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }

                                            double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                            double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                            double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                            double edgemag = Vector.VectorMagnitude(edgevec);

                                            double Cx = startpt.X - edge.startendpt[1].X;
                                            double Cy = startpt.Y - edge.startendpt[1].Y;
                                            double Cz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);

                                            double Dx = endpt.X - edge.startendpt[1].X;
                                            double Dy = endpt.Y - edge.startendpt[1].Y;
                                            double Dz = endpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);

                                            if (Dmag < edgemag && Cmag < edgemag)
                                            {
                                                //this means the test edge is longer than the guest edge, but they overlap
                                                kp.Value.relatedEdges.Add(edge);
                                                //the edge is still unique but accumulates a neighbor
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            //this other point isn't relevant, and the edges don't coincide
                                            edgecount++;
                                            continue;
                                        }

                                    }


                                }
                                else if (startpt.X == edge.startendpt[1].X && startpt.Y == edge.startendpt[1].Y && startpt.Z == edge.startendpt[1].Z)
                                {
                                    //found at least one perfect coordinate match, try to match the second
                                    Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                    if (endpt.X == edge.startendpt[0].X && endpt.Y == edge.startendpt[0].Y && endpt.Z == edge.startendpt[0].Z)
                                    {
                                        //both match, means the match is perfect, so add it to the related surfaces list
                                        kp.Value.relatedEdges.Add(edge);
                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a perfect match");
                                        break;

                                    }
                                    else
                                    {
                                        //the edge may be unique, though it could still have neighboring relationships
                                        double Ax = endpt.X - edge.startendpt[0].X;
                                        double Ay = endpt.Y - edge.startendpt[0].Y;
                                        double Az = endpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                        double Amag = Vector.VectorMagnitude(A);

                                        //take cross product to see if they are even in same plane
                                        double evX = endpt.X - startpt.X;
                                        double evY = endpt.Y - startpt.Y;
                                        double evZ = endpt.Z - startpt.Z;
                                        Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                        double evmag = Vector.VectorMagnitude(ev);
                                        Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                        double crossmag = Vector.VectorMagnitude(cross);
                                        //tolerance?
                                        if (crossmag == 0)
                                        {
                                            //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                            double Bx = startpt.X - edge.startendpt[0].X;
                                            double By = startpt.Y - edge.startendpt[0].Y;
                                            double Bz = startpt.Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                            double Bmag = Vector.VectorMagnitude(B);
                                            //check to see if the test edge is inside the guest edge
                                            if (Amag < evmag && Bmag < evmag)
                                            {
                                                //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                                kp.Value.relatedEdges.Add(edge);
                                                //accumulate its own relationships
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }

                                            double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                            double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                            double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                            double edgemag = Vector.VectorMagnitude(edgevec);

                                            double Cx = startpt.X - edge.startendpt[0].X;
                                            double Cy = startpt.Y - edge.startendpt[0].Y;
                                            double Cz = startpt.Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);

                                            double Dx = endpt.X - edge.startendpt[0].X;
                                            double Dy = endpt.Y - edge.startendpt[0].Y;
                                            double Dz = endpt.Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);

                                            if (Dmag < edgemag && Cmag < edgemag)
                                            {
                                                //this means the test edge is longer than the guest edge, but they overlap
                                                kp.Value.relatedEdges.Add(edge);
                                                //the edge is still unique but accumulates a neighbor
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            //this other point isn't relevant, and the edges don't coincide
                                            edgecount++;
                                            continue;
                                        }
                                    }

                                }
                                //neither points perfectly coincide, so we do an exhaustive overlap check.
                                else
                                {
                                    Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                    //are the two vectors even parallel?  because if they are not, no need to get more complex
                                    double evX = endpt.X - startpt.X;
                                    double evY = endpt.Y - startpt.Y;
                                    double evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                    double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                    double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect edgev = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                    if (Vector.VectorMagnitude(Vector.CrossProduct(ev, edgev)) != 0)
                                    {
                                        //they are not even parallel so move on
                                        edgecount++;
                                        continue;
                                    }

                                    //try to determine if the two edges are parallel
                                    //test edge point 1
                                    double Ax = endpt.X - edge.startendpt[0].X;
                                    double Ay = endpt.Y - edge.startendpt[0].Y;
                                    double Az = endpt.Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                    double Amag = Vector.VectorMagnitude(A);

                                    //take cross product to see if they are even in same plane
                                    evX = endpt.X - startpt.X;
                                    evY = endpt.Y - startpt.Y;
                                    evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev1 = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double guestmag = Vector.VectorMagnitude(ev1);
                                    Vector.MemorySafe_CartVect cross1 = Vector.CrossProduct(A, ev1);
                                    double crossmag = Vector.VectorMagnitude(cross1);
                                    //tolerance?
                                    if (crossmag == 0)
                                    {
                                        //we are at least parallel, now to check for a real intersection
                                        double Bx = startpt.X - edge.startendpt[0].X;
                                        double By = startpt.Y - edge.startendpt[0].Y;
                                        double Bz = startpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                        double Bmag = Vector.VectorMagnitude(B);
                                        //check to see if the test edge's first point (index 0) is totally inside the guest edge
                                        if (Amag < guestmag && Bmag < guestmag)
                                        {
                                            //the start point of the test edge is inside the guest edge
                                            //test edge point 2 against guest edge point 2
                                            double Cx = endpt.X - edge.startendpt[1].X;
                                            double Cy = endpt.Y - edge.startendpt[1].Y;
                                            double Cz = endpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);
                                            Vector.MemorySafe_CartVect cross2 = Vector.CrossProduct(C, ev);
                                            crossmag = Vector.VectorMagnitude(cross2);
                                            if (crossmag == 0)
                                            {
                                                //we are at least parallel, in fact we have proven we are totall parallel, now intersect
                                                double Dx = startpt.X - edge.startendpt[1].X;
                                                double Dy = startpt.Y - edge.startendpt[1].Y;
                                                double Dz = startpt.Z - edge.startendpt[1].Z;
                                                Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                                double Dmag = Vector.VectorMagnitude(D);
                                                if (Cmag < guestmag && Dmag < guestmag)
                                                {
                                                    //then it is inside as well, and test vector is engulfed by guest vector
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    string startcoord = MakeCoordString(edge.startendpt[0]);
                                                    string endcoord = MakeCoordString(edge.startendpt[1]);
                                                    report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                    continue;
                                                }
                                                else
                                                {
                                                    //I am pretty sure that by default, they are still neighbors and this is no difference
                                                    //it simply extends beyond one of the ends of the guest vector
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    string startcoord = MakeCoordString(edge.startendpt[0]);
                                                    string endcoord = MakeCoordString(edge.startendpt[1]);
                                                    report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                    continue;
                                                }


                                            }
                                            else
                                            {
                                                //we are not parallel, so this is not an adjacency match
                                                edgecount++;
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            //if test edge start point [index 0] is outside, is one of the guest points inside?
                                            //already computed B
                                            double Cx = startpt.X - edge.startendpt[1].X;
                                            double Cy = startpt.Y - edge.startendpt[1].Y;
                                            double Cz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);

                                            edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                            edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                            edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                            double edgemag = Vector.VectorMagnitude(edgevec);

                                            if (Cmag < edgemag && Bmag < edgemag)
                                            {
                                                //the guest edge's start point is inside the test edge
                                                //guest edge point 2 
                                                double Dx = endpt.X - edge.startendpt[1].X;
                                                double Dy = endpt.Y - edge.startendpt[1].Y;
                                                double Dz = endpt.Z - edge.startendpt[1].Z;
                                                Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                                double Dmag = Vector.VectorMagnitude(D);
                                                Vector.MemorySafe_CartVect cross3 = Vector.CrossProduct(D, edgevec);
                                                crossmag = Vector.VectorMagnitude(cross3);
                                                if (crossmag == 0)
                                                {
                                                    //then we know the two edges are totall parallel and lined up
                                                    //determine if the guest edge point 2 is inside the test edge or outside of it
                                                    double Ex = startpt.X - edge.startendpt[1].X;
                                                    double Ey = startpt.Y - edge.startendpt[1].Y;
                                                    double Ez = startpt.Z - edge.startendpt[1].Z;
                                                    Vector.MemorySafe_CartVect E = new Vector.MemorySafe_CartVect(Ex, Ey, Ez);
                                                    double Emag = Vector.VectorMagnitude(E);
                                                    if (Dmag < edgemag && Emag < edgemag)
                                                    {
                                                        //it is inside
                                                        kp.Value.relatedEdges.Add(edge);
                                                        //but the edge is still itself unique
                                                        edge.relatedEdges.Add(kp.Value);
                                                        edgecount++;
                                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                        continue;

                                                    }
                                                    else
                                                    {
                                                        //it is outside 
                                                        kp.Value.relatedEdges.Add(edge);
                                                        //but the edge is still itself unique
                                                        edge.relatedEdges.Add(kp.Value);
                                                        edgecount++;
                                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    //we are not parallel, so this is not an adjacency match
                                                    edgecount++;
                                                    continue;
                                                }

                                            }
                                        }



                                    }
                                    else
                                    {
                                        //they are not even parallel, so it is likely best just to shove on
                                        edgecount++;
                                        continue;
                                    }


                                }
                            }
                            //this determines if it found a matching edge
                            if (edgecount == edges.Count)
                            {
                                edges.Add(distinctedges, edge);
                                distinctedges++;
                            }

                        }
                        //last edge end edge is the zero index   
                        else
                        {
                            edge.startendpt.Add(pl.plcoords[i]);
                            edge.startendpt.Add(pl.plcoords[0]);
                            int edgecount = 0; //keeps track of how many guest edges in the dictionary I've searched through
                            foreach (KeyValuePair<int, DOEgbXMLBasics.EdgeFamily> kp in edges)
                            {

                                Vector.MemorySafe_CartCoord startpt = kp.Value.startendpt[0];
                                //tolerance needed?
                                if (startpt.X == edge.startendpt[0].X && startpt.Y == edge.startendpt[0].Y && startpt.Z == edge.startendpt[0].Z)
                                {
                                    //found at least one perfect coordinate match, try to match the second
                                    Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                    if (endpt.X == edge.startendpt[1].X && endpt.Y == edge.startendpt[1].Y && endpt.Z == edge.startendpt[1].Z)
                                    {
                                        //both match, means the match is perfect, so add it to the related surfaces list
                                        kp.Value.relatedEdges.Add(edge);
                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a perfect match");
                                        break;
                                    }
                                    else
                                    {
                                        //the edge may be unique, though it could still have neighboring relationships
                                        //draw vector A
                                        double Ax = endpt.X - edge.startendpt[1].X;
                                        double Ay = endpt.Y - edge.startendpt[1].Y;
                                        double Az = endpt.Z - edge.startendpt[1].Z;
                                        Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                        double Amag = Vector.VectorMagnitude(A);

                                        //take cross product to see if they are even in same plane
                                        double evX = endpt.X - startpt.X;
                                        double evY = endpt.Y - startpt.Y;
                                        double evZ = endpt.Z - startpt.Z;
                                        Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                        double evmag = Vector.VectorMagnitude(ev);
                                        Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                        double crossmag = Vector.VectorMagnitude(cross);
                                        //tolerance?
                                        if (crossmag == 0)
                                        {
                                            //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                            double Bx = startpt.X - edge.startendpt[1].X;
                                            double By = startpt.Y - edge.startendpt[1].Y;
                                            double Bz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                            double Bmag = Vector.VectorMagnitude(B);
                                            //check to see if the test edge is inside the guest edge
                                            if (Amag < evmag && Bmag < evmag)
                                            {
                                                //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                                kp.Value.relatedEdges.Add(edge);
                                                //accumulate its own relationships
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }

                                            double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                            double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                            double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                            double edgemag = Vector.VectorMagnitude(edgevec);

                                            double Cx = startpt.X - edge.startendpt[1].X;
                                            double Cy = startpt.Y - edge.startendpt[1].Y;
                                            double Cz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);

                                            double Dx = endpt.X - edge.startendpt[1].X;
                                            double Dy = endpt.Y - edge.startendpt[1].Y;
                                            double Dz = endpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);

                                            if (Dmag < edgemag && Cmag < edgemag)
                                            {
                                                //this means the test edge is longer than the guest edge, but they overlap
                                                kp.Value.relatedEdges.Add(edge);
                                                //the edge is still unique but accumulates a neighbor
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            //this other point isn't relevant, and the edges don't coincide
                                            edgecount++;
                                            continue;
                                        }

                                    }


                                }
                                else if (startpt.X == edge.startendpt[1].X && startpt.Y == edge.startendpt[1].Y && startpt.Z == edge.startendpt[1].Z)
                                {
                                    //found at least one perfect coordinate match, try to match the second
                                    Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                    if (endpt.X == edge.startendpt[0].X && endpt.Y == edge.startendpt[0].Y && endpt.Z == edge.startendpt[0].Z)
                                    {
                                        //both match, means the match is perfect, so add it to the related surfaces list
                                        kp.Value.relatedEdges.Add(edge);
                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a perfect match");
                                        break;

                                    }
                                    else
                                    {
                                        //the edge may be unique, though it could still have neighboring relationships
                                        double Ax = endpt.X - edge.startendpt[0].X;
                                        double Ay = endpt.Y - edge.startendpt[0].Y;
                                        double Az = endpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                        double Amag = Vector.VectorMagnitude(A);

                                        //take cross product to see if they are even in same plane
                                        double evX = endpt.X - startpt.X;
                                        double evY = endpt.Y - startpt.Y;
                                        double evZ = endpt.Z - startpt.Z;
                                        Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                        double evmag = Vector.VectorMagnitude(ev);
                                        Vector.MemorySafe_CartVect cross = Vector.CrossProduct(A, ev);
                                        double crossmag = Vector.VectorMagnitude(cross);
                                        //tolerance?
                                        if (crossmag == 0)
                                        {
                                            //then we are at least parallel or antiparallel, now see if the point resides on the edge or outside of it
                                            double Bx = startpt.X - edge.startendpt[0].X;
                                            double By = startpt.Y - edge.startendpt[0].Y;
                                            double Bz = startpt.Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                            double Bmag = Vector.VectorMagnitude(B);
                                            //check to see if the test edge is inside the guest edge
                                            if (Amag < evmag && Bmag < evmag)
                                            {
                                                //this means it lies on the plane at least, so it shares, but it is also still independent because a perfect match wasn't found
                                                kp.Value.relatedEdges.Add(edge);
                                                //accumulate its own relationships
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }

                                            double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                            double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                            double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                            double edgemag = Vector.VectorMagnitude(edgevec);

                                            double Cx = startpt.X - edge.startendpt[0].X;
                                            double Cy = startpt.Y - edge.startendpt[0].Y;
                                            double Cz = startpt.Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);

                                            double Dx = endpt.X - edge.startendpt[0].X;
                                            double Dy = endpt.Y - edge.startendpt[0].Y;
                                            double Dz = endpt.Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                            double Dmag = Vector.VectorMagnitude(D);

                                            if (Dmag < edgemag && Cmag < edgemag)
                                            {
                                                //this means the test edge is longer than the guest edge, but they overlap
                                                kp.Value.relatedEdges.Add(edge);
                                                //the edge is still unique but accumulates a neighbor
                                                edge.relatedEdges.Add(kp.Value);
                                                edgecount++;
                                                string startcoord = MakeCoordString(edge.startendpt[0]);
                                                string endcoord = MakeCoordString(edge.startendpt[1]);
                                                report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            //this other point isn't relevant, and the edges don't coincide
                                            edgecount++;
                                            continue;
                                        }
                                    }

                                }
                                //neither points perfectly coincide, so we do an exhaustive overlap check.
                                else
                                {
                                    Vector.MemorySafe_CartCoord endpt = kp.Value.startendpt[1];
                                    //are the two vectors even parallel?  because if they are not, no need to get more complex
                                    double evX = endpt.X - startpt.X;
                                    double evY = endpt.Y - startpt.Y;
                                    double evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                    double edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                    double edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect edgev = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                    if (Vector.VectorMagnitude(Vector.CrossProduct(ev, edgev)) != 0)
                                    {
                                        //they are not even parallel so move on
                                        edgecount++;
                                        continue;
                                    }
                                    //try to determine if the two edges are parallel

                                    //test edge point 1
                                    double Ax = endpt.X - edge.startendpt[0].X;
                                    double Ay = endpt.Y - edge.startendpt[0].Y;
                                    double Az = endpt.Z - edge.startendpt[0].Z;
                                    Vector.MemorySafe_CartVect A = new Vector.MemorySafe_CartVect(Ax, Ay, Az);
                                    double Amag = Vector.VectorMagnitude(A);

                                    //take cross product to see if they are even in same plane
                                    evX = endpt.X - startpt.X;
                                    evY = endpt.Y - startpt.Y;
                                    evZ = endpt.Z - startpt.Z;
                                    Vector.MemorySafe_CartVect ev1 = new Vector.MemorySafe_CartVect(evX, evY, evZ);
                                    double guestmag = Vector.VectorMagnitude(ev);
                                    Vector.MemorySafe_CartVect cross1 = Vector.CrossProduct(A, ev);
                                    double crossmag = Vector.VectorMagnitude(cross1);
                                    //tolerance?
                                    if (crossmag == 0)
                                    {
                                        //we are at least parallel, now to check for a real intersection
                                        double Bx = startpt.X - edge.startendpt[0].X;
                                        double By = startpt.Y - edge.startendpt[0].Y;
                                        double Bz = startpt.Z - edge.startendpt[0].Z;
                                        Vector.MemorySafe_CartVect B = new Vector.MemorySafe_CartVect(Bx, By, Bz);
                                        double Bmag = Vector.VectorMagnitude(B);
                                        //check to see if the test edge's first point (index 0) is totally inside the guest edge
                                        if (Amag < guestmag && Bmag < guestmag)
                                        {
                                            //the start point of the test edge is inside the guest edge
                                            //test edge point 2 against guest edge point 2
                                            double Cx = endpt.X - edge.startendpt[1].X;
                                            double Cy = endpt.Y - edge.startendpt[1].Y;
                                            double Cz = endpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);
                                            Vector.MemorySafe_CartVect cross2 = Vector.CrossProduct(C, ev);
                                            crossmag = Vector.VectorMagnitude(cross2);
                                            if (crossmag == 0)
                                            {
                                                //we are at least parallel, in fact we have proven we are totall parallel, now intersect
                                                double Dx = startpt.X - edge.startendpt[1].X;
                                                double Dy = startpt.Y - edge.startendpt[1].Y;
                                                double Dz = startpt.Z - edge.startendpt[1].Z;
                                                Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                                double Dmag = Vector.VectorMagnitude(D);
                                                if (Cmag < guestmag && Dmag < guestmag)
                                                {
                                                    //then it is inside as well, and test vector is engulfed by guest vector
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    string startcoord = MakeCoordString(edge.startendpt[0]);
                                                    string endcoord = MakeCoordString(edge.startendpt[1]);
                                                    report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                    continue;
                                                }
                                                else
                                                {
                                                    //I am pretty sure that by default, they are still neighbors and this is no difference
                                                    //it simply extends beyond one of the ends of the guest vector
                                                    kp.Value.relatedEdges.Add(edge);
                                                    //but the edge is still itself unique
                                                    edge.relatedEdges.Add(kp.Value);
                                                    edgecount++;
                                                    string startcoord = MakeCoordString(edge.startendpt[0]);
                                                    string endcoord = MakeCoordString(edge.startendpt[1]);
                                                    report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                    continue;
                                                }


                                            }
                                            else
                                            {
                                                //we are not parallel, so this is not an adjacency match
                                                edgecount++;
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            //if test edge start point [index 0] is outside, is one of the guest points inside?
                                            //already computed B
                                            double Cx = startpt.X - edge.startendpt[1].X;
                                            double Cy = startpt.Y - edge.startendpt[1].Y;
                                            double Cz = startpt.Z - edge.startendpt[1].Z;
                                            Vector.MemorySafe_CartVect C = new Vector.MemorySafe_CartVect(Cx, Cy, Cz);
                                            double Cmag = Vector.VectorMagnitude(C);

                                            edgeX = edge.startendpt[1].X - edge.startendpt[0].X;
                                            edgeY = edge.startendpt[1].Y - edge.startendpt[0].Y;
                                            edgeZ = edge.startendpt[1].Z - edge.startendpt[0].Z;
                                            Vector.MemorySafe_CartVect edgevec = new Vector.MemorySafe_CartVect(edgeX, edgeY, edgeZ);
                                            double edgemag = Vector.VectorMagnitude(edgevec);

                                            if (Cmag < edgemag && Bmag < edgemag)
                                            {
                                                //the guest edge's start point is inside the test edge
                                                //guest edge point 2 
                                                double Dx = endpt.X - edge.startendpt[1].X;
                                                double Dy = endpt.Y - edge.startendpt[1].Y;
                                                double Dz = endpt.Z - edge.startendpt[1].Z;
                                                Vector.MemorySafe_CartVect D = new Vector.MemorySafe_CartVect(Dx, Dy, Dz);
                                                double Dmag = Vector.VectorMagnitude(D);
                                                Vector.MemorySafe_CartVect cross3 = Vector.CrossProduct(D, edgevec);
                                                crossmag = Vector.VectorMagnitude(cross3);
                                                if (crossmag == 0)
                                                {
                                                    //then we know the two edges are totall parallel and lined up
                                                    //determine if the guest edge point 2 is inside the test edge or outside of it
                                                    double Ex = startpt.X - edge.startendpt[1].X;
                                                    double Ey = startpt.Y - edge.startendpt[1].Y;
                                                    double Ez = startpt.Z - edge.startendpt[1].Z;
                                                    Vector.MemorySafe_CartVect E = new Vector.MemorySafe_CartVect(Ex, Ey, Ez);
                                                    double Emag = Vector.VectorMagnitude(E);
                                                    if (Dmag < edgemag && Emag < edgemag)
                                                    {
                                                        //it is inside
                                                        kp.Value.relatedEdges.Add(edge);
                                                        //but the edge is still itself unique
                                                        edge.relatedEdges.Add(kp.Value);
                                                        edgecount++;
                                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        //it is outside 
                                                        kp.Value.relatedEdges.Add(edge);
                                                        //but the edge is still itself unique
                                                        edge.relatedEdges.Add(kp.Value);
                                                        edgecount++;
                                                        string startcoord = MakeCoordString(edge.startendpt[0]);
                                                        string endcoord = MakeCoordString(edge.startendpt[1]);
                                                        report.MessageList.Add("Edge " + startcoord + " , " + endcoord + " found a match");
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    //we are not parallel, so this is not an adjacency match
                                                    edgecount++;
                                                    continue;
                                                }

                                            }
                                        }



                                    }
                                    else
                                    {
                                        //they are not even parallel, so it is likely best just to shove on
                                        edgecount++;
                                        continue;
                                    }


                                }
                            }
                            //this determines if it found a matching edge
                            if (edgecount == edges.Count)
                            {
                                edges.Add(distinctedges, edge);
                                distinctedges++;
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                report.longMsg = e.ToString();
                report.MessageList.Add("This test has failed unexpectedly.");
                report.passOrFail = false;
                return report;
            }

            return report;
        }

        public DOEgbXMLReportingObj ValidateEdges(Dictionary<int, EdgeFamily> edges, DOEgbXMLReportingObj report)
        {
            //March 4, 2014
            //this routine attempts to ensure the edged all line up
            //it looks for overlaps in the edge family and for end point matching.
            report.MessageList.Add("Validating edge families.");
            foreach (KeyValuePair<int, EdgeFamily> kpedge in edges)
            {
                for (int reledge = 0; reledge < kpedge.Value.relatedEdges.Count; reledge++)
                {
                    EdgeFamily e = kpedge.Value.relatedEdges[reledge];
                    Vector.MemorySafe_CartCoord startpt = e.startendpt[0];
                    Vector.MemorySafe_CartCoord endpt = e.startendpt[1];
                    for (int matchedge = reledge + 1; matchedge < kpedge.Value.relatedEdges.Count; matchedge++)
                    {
                        EdgeFamily neighbor = kpedge.Value.relatedEdges[matchedge];
                        Vector.MemorySafe_CartCoord neighstartpt = neighbor.startendpt[0];
                        Vector.MemorySafe_CartCoord neighendpt = neighbor.startendpt[1];
                        if (startpt.X == neighstartpt.X && startpt.Y == neighstartpt.Y && startpt.Z == neighstartpt.Z)
                        {

                        }
                        else if (Math.Abs(startpt.X - neighstartpt.X) < report.tolerance && Math.Abs(startpt.Y - neighstartpt.Y) < report.tolerance && Math.Abs(startpt.Z - neighstartpt.Z) < report.tolerance)
                        {

                        }
                        else
                        {
                            continue;
                        }


                    }
                }
            }
            return report;
        }

        private static string MakeCoordString(Vector.MemorySafe_CartCoord coord)
        {
            return "(" + coord.X + "," + coord.Y + "," + coord.Z + ")";
        }

        #region utility functions for calculating surface area.

        /**
         * Calculate the surface area use planar coordinates of a surface.
         * Requires at least 3 planar coordinates, or else will return -1
         * 
         */
        public static double computeArea(List<Vector.MemorySafe_CartCoord> PlCoords)
        {

            //have to be more than 3 points
            if (PlCoords.Count < 3)
            {
                return -1;
            }

            int numPoints = PlCoords.Count;
            Vector.CartVect p1, p2;
            Vector.CartVect sum = new Vector.CartVect(0, 0, 0);
            for (int i = 0; i < numPoints; i++)
            {
                p1 = new Vector.CartVect();
                p2 = new Vector.CartVect();
                p1.X = PlCoords[i].X;
                p1.Y = PlCoords[i].Y;
                p1.Z = PlCoords[i].Z;

                if (i < numPoints - 1)
                {
                    p2.X = PlCoords[i + 1].X;
                    p2.Y = PlCoords[i + 1].Y;
                    p2.Z = PlCoords[i + 1].Z;
                }
                else
                {
                    p2.X = PlCoords[0].X;
                    p2.Y = PlCoords[0].Y;
                    p2.Z = PlCoords[0].Z;
                }


                Vector.CartVect crossPoint = cross(p1, p2);

                sum.X = sum.X + crossPoint.X;
                sum.Y = sum.Y + crossPoint.Y;
                sum.Z = sum.Z + crossPoint.Z;
            }

            Vector.CartVect normal = getNorm(PlCoords);
            normalize(normal);

            double surfaceArea = dot(sum, normal);

            return Math.Round(Math.Abs(surfaceArea / 2), 2);

        }

        public static Vector.CartVect getNorm(List<Vector.MemorySafe_CartCoord> PlCoords)
        {
            Vector.CartVect p1 = new Vector.CartVect();
            p1.X = PlCoords[0].X;
            p1.Y = PlCoords[0].Y;
            p1.Z = PlCoords[0].Z;
            Vector.CartVect p2 = new Vector.CartVect();
            p2.X = PlCoords[1].X;
            p2.Y = PlCoords[1].Y;
            p2.Z = PlCoords[1].Z;
            Vector.CartVect p3 = new Vector.CartVect();
            p3.X = PlCoords[2].X;
            p3.Y = PlCoords[2].Y;
            p3.Z = PlCoords[2].Z;

            Vector.CartVect vector21 = makeVector(p1, p2);
            Vector.CartVect vector31 = makeVector(p1, p3);

            return cross(vector21, vector31);
        }

        /*
         *Origin at first point in coords
        */
        public static Vector.CartVect makeVector(Vector.CartVect p1, Vector.CartVect p2)
        {
            Vector.CartVect p = new Vector.CartVect();
            p.X = p2.X - p1.X;
            p.Y = p2.Y - p1.Y;
            p.Z = p2.Z - p1.Z;
            return p;
        }

        public static double dot(Vector.CartVect vector1, Vector.CartVect vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }


        /*
         * find the middle point of two cardition points.
         */
        public static Vector.CartVect cross(Vector.CartVect vector1, Vector.CartVect vector2)
        {
            double crossX = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            double crossY = vector1.Z * vector2.X - vector1.X * vector2.Z;
            double crossZ = vector1.X * vector2.Y - vector1.Y * vector2.X;

            Vector.CartVect p = new Vector.CartVect();
            p.X = crossX;
            p.Y = crossY;
            p.Z = crossZ;

            return p;
        }


        /*
         * Normalize the vector to the origin
         */
        public static void normalize(Vector.CartVect vector)
        {
            double len = computeLength(vector, new Vector.CartVect(0, 0, 0));
            vector.X = vector.X / len;
            vector.Y = vector.Y / len;
            vector.Z = vector.Z / len;
        }

        /*
         * Calculate the length between two points
         */
        public static double computeLength(Vector.CartVect vector1, Vector.CartVect vector2)
        {
            double len = 0;
            len += Math.Pow((vector1.X - vector2.X), 2);
            len += Math.Pow((vector1.Y - vector2.Y), 2);
            len += Math.Pow((vector1.Z - vector2.Z), 2);

            return Math.Sqrt(len);
        }
        #endregion
    }
}