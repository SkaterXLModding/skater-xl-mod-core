using System.Collections.Generic;

using UnityEngine;
using UnityEditor;


public class SXLGrind : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] public SXLGrindCore.GrindSurface grindSurface = SXLGrindCore.GrindSurface.METAL;
    [SerializeField] public GrindSegment segment = new GrindSegment();
    [HideInInspector] public List<Vector3> pointPos = new List<Vector3>();
    [Space(10)]
    public bool initilizeOnAwake = false;

    public bool debugGrind = true;
    public Color debugColor = Color.red;
    public float debugThickness = 10f;

    void OnDrawGizmos()
    {
        if (this.debugGrind && this.pointPos.Count > 0)
        {
            List<Vector3> pos = new List<Vector3>();
            foreach (Vector3 vec in this.pointPos)
            {
                Vector3 pointWorld = this.transform.TransformPoint(vec);
                pos.Add(pointWorld);
                Gizmos.color = this.debugColor;
                Gizmos.DrawWireSphere(pointWorld, 0.05f);
            }
            Handles.color = this.debugColor;
            Handles.DrawAAPolyLine(this.debugThickness, pos.ToArray());
        }
    }
}