using Habrador_Computational_Geometry;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Delaunay
{
    public static Mesh GenerateTriangulation(List<Vector3> hullPoints, List<List<Vector3>> holesPoints = null, List<Vector3> points = null)
    {
        if (hullPoints == null || hullPoints.Count < 3)
        {
            Debug.LogError("Invalid hullPoints");
            return null;
        }

        //Points
        List<MyVector2> points_2d = new List<MyVector2>();
        if (points != null)
        {
            points_2d = points.Select(v => v.ToMyVector2()).ToList();
        }

        //Hull
        List<MyVector2> hullPoints_2d = hullPoints.Select(x => x.ToMyVector2()).ToList();

        //Holes
        HashSet<List<MyVector2>> allHolePoints_2d = new HashSet<List<MyVector2>>();
        if (holesPoints != null)
        {
            foreach (List<Vector3> holePoints in holesPoints)
            {
                if (holePoints != null)
                {
                    List<MyVector2> holePoints_2d = holePoints.Select(x => x.ToMyVector2()).ToList();
                    allHolePoints_2d.Add(holePoints_2d);
                }
            }
        }

        //Normalize to range 0-1
        //We should use all points, including the constraints because the hole may be outside of the random points
        List<MyVector2> allPoints = new List<MyVector2>();
        allPoints.AddRange(hullPoints_2d);
        foreach (List<MyVector2> hole in allHolePoints_2d)
        {
            allPoints.AddRange(hole);
        }
        //allPoints.AddRange(points_2d); // TODO : Don't know if I should include this or not, probably, but not sure
        Normalizer2 normalizer = new Normalizer2(allPoints);

        List<MyVector2> points_2d_normalized = normalizer.Normalize(points_2d);
        List<MyVector2> hullPoints_2d_normalized = normalizer.Normalize(hullPoints_2d);
        HashSet<List<MyVector2>> allHolePoints_2d_normalized = new HashSet<List<MyVector2>>();
        foreach (List<MyVector2> hole in allHolePoints_2d)
        {
            List<MyVector2> hole_normalized = normalizer.Normalize(hole);
            allHolePoints_2d_normalized.Add(hole_normalized);
        }


        //
        // Generate the triangulation
        //

        //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        //timer.Start();

        HalfEdgeData2 triangleData_normalized = _Delaunay.ConstrainedBySloan(points_2d_normalized, hullPoints_2d_normalized, allHolePoints_2d_normalized, shouldRemoveTriangles: true, new HalfEdgeData2());

        //timer.Stop();
        //Debug.Log($"Generated a delaunay triangulation in {timer.ElapsedMilliseconds / 1000f} seconds");


        //UnNormalize
        HalfEdgeData2 triangleData = normalizer.UnNormalize(triangleData_normalized);

        //From half-edge to triangle
        HashSet<Triangle2> triangles_2d = _TransformBetweenDataStructures.HalfEdge2ToTriangle2(triangleData);

        //From triangulation to mesh

        //Make sure the triangles have the correct orientation
        triangles_2d = HelpMethods.OrientTrianglesClockwise(triangles_2d);

        //From 2d to 3d
        HashSet<Triangle3> triangles_3d = new HashSet<Triangle3>();
        foreach (Triangle2 t in triangles_2d)
        {
            triangles_3d.Add(new Triangle3(t.p1.ToMyVector3_Yis3D(), t.p2.ToMyVector3_Yis3D(), t.p3.ToMyVector3_Yis3D()));
        }

        return _TransformBetweenDataStructures.Triangle3ToCompressedMesh(triangles_3d);
    }
}
