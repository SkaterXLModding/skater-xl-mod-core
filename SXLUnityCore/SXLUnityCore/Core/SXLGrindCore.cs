using System.Collections.Generic;

using UnityEngine;

using Dreamteck.Splines;

public class GrindSegment
{
    public Transform grindRoot;
    public Vector3[] normals;
    public SplinePoint[] points;

    public GrindSegment()
    {

    }

    public GrindSegment(Transform parent, List<Vector3> splinePoints)
    {
        this.grindRoot = parent;
        this.points = new SplinePoint[splinePoints.Count];

        for (int i = 0; i < splinePoints.Count; i++)
        {
            this.points[i] = new SplinePoint(parent.TransformPoint(splinePoints[i]));
        }
    }
}


public static class SXLGrindCore
{
    public enum GrindSurface { BASE, CONCRETE, METAL, WOOD };


    public static string GetGrindAudioCue(GrindSurface surface)
    {
        switch (surface)
        {
            case GrindSurface.WOOD:
                return "Wood";
            case GrindSurface.CONCRETE:
                return "Concrete";
            case GrindSurface.METAL:
                return "Metal";
            default:
                return "Base";
        }
    }

    public static GrindSurface GetGrindAudioCue(GameObject obj)
    {
        // Determine Audio Cue -- Split out into separate function
        if (obj.name.ToLower().Contains("metal"))
            return GrindSurface.METAL;
        else if (obj.name.ToLower().Contains("wood"))
            return GrindSurface.WOOD;
        else if (obj.name.ToLower().Contains("concrete"))
            return GrindSurface.CONCRETE;
        else
            return GrindSurface.BASE;
    }

    public static void BuildCollisionShapes(this GrindSegment segment)
    {
        segment.normals = new Vector3[segment.points.Length];

        for (int i = 0; i < segment.points.Length - 1; i++)
        {
            // Get object to update position and convert it into collision shape
            GameObject go = segment.grindRoot.gameObject;
            go.transform.position = segment.points[i].position;
            go.name = string.Format("{0}_collision_{1}", segment.grindRoot.parent.gameObject.name, i);
            go.transform.LookAt(segment.points[i + 1].position);
            go.tag = segment.grindRoot.gameObject.tag;
            go.layer = 12;

            // Set collider size
            float z = Vector3.Distance(segment.points[i].position, segment.points[i + 1].position);
            BoxCollider bc = go.AddComponent<BoxCollider>();
            bc.size = new Vector3(0.05f, 0.05f, z);
            bc.center = (Vector3.forward * z) / 2f;
            bc.isTrigger = true;

            //DEBUG 
            /*
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.widthMultiplier = 0.1f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            var trans = bc.transform;
            var min = bc.center - bc.size * 0.5f;
            var max = bc.center + bc.size * 0.5f;
            line.SetPosition(0, trans.TransformPoint(new Vector3(min.x, min.y, min.z)));
            line.SetPosition(1, trans.TransformPoint(new Vector3(max.x, min.y, min.z)));
            */

            // Assign up vectors
            segment.normals[i] = go.transform.up;
        }
    }
    
    public static bool BuildSplineComputer(GrindSegment segment)
    {
        // Create Spline Computer component and settings for GrindSpline Object
        SplineComputer sc = segment.grindRoot.gameObject.AddComponent<SplineComputer>();
        sc.type = Spline.Type.Linear;
        sc.SetPoints(segment.points, SplineComputer.Space.World);
        sc.Evaluate(0.9);

        // Shift normal vectors up a position for spline points points
        segment.normals[segment.normals.Length - 1] = segment.normals[segment.normals.Length - 2];

        // Set point normals
        for (int i = 0; i < segment.points.Length; i++)
        {
            sc.SetPointNormal(i, sc.GetPoint(i, SplineComputer.Space.World).normal + segment.normals[i], SplineComputer.Space.World);
        }

        return true;
    }
}