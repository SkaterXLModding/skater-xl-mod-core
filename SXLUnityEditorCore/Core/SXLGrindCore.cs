using System.Collections.Generic;

using UnityEngine;


public class GrindSegment
{
    public Transform grindRoot;
    public Vector3[] normals;
    public Vector3[] points;

    public GrindSegment()
    {
        this.normals = new Vector3[0];
        this.points = new Vector3[0];
    }

    public GrindSegment(Transform parent, List<Vector3> inputs)
    {
        this.grindRoot = parent;
        this.points = new Vector3[inputs.Count];

        for (int i = 0; i < inputs.Count; i++)
        {
            this.points[i] = inputs[i];
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
}