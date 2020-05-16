using System.Collections.Generic;

using UnityEngine;


public class SXLGrind : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] public SXLGrindCore.GrindSurface grindSurface = SXLGrindCore.GrindSurface.METAL;
    [SerializeField] public GrindSegment segment = new GrindSegment();
    [HideInInspector] public List<Vector3> pointPos = new List<Vector3>();
    [Space(10)]
    public bool initilizeOnAwake = false;

    private bool hasGrinds = false;

    // Functions
    public bool CreateGrindable()
    {
        this.segment = this.segment.points != null ? this.segment : new GrindSegment(this.transform, this.pointPos);

        if (this.segment.points.Length < 2)
        {
            return false;
        }

        this.gameObject.tag = SXLGrindCore.GetGrindAudioCue(this.grindSurface);
        this.segment.BuildCollisionShapes();

        return SXLGrindCore.BuildSplineComputer(this.segment);
    }

    void Awake()
    {
        if (this.initilizeOnAwake)
        {
            this.CreateGrindable();
        }
    }

    // Monobehaviour
    void Start()
    {
        if (!this.hasGrinds)
        {
            this.CreateGrindable();
        }
    }
}

