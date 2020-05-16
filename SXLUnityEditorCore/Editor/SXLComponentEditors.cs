using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SXLGrind))]
public class GrindableComponentEditor : Editor
{
    Vector2 scrollSplinePosition;
    Vector2 scrollPointPosition;
    private int selectedPoint = -1;

    private void AddSplinePoint(SXLGrind grind)
    {
        Vector3 newPt = grind.pointPos.Count == 0 ? grind.transform.localPosition : grind.pointPos[grind.pointPos.Count - 1];
        grind.pointPos.Add(newPt + (Vector3.up * 0.25f));
        SelectPoint(grind.pointPos.Count - 1);
        UpdateSpline(grind);
    }

    private void RemoveSplinePoint(SXLGrind grind, int idx)
    {
        grind.pointPos.RemoveAt(idx);
        int selection = grind.pointPos.Count == 0 ? -1 : grind.pointPos.Count - 1;
        SelectPoint(selection);
        UpdateSpline(grind);
    }

    private void UpdateSpline(SXLGrind grind)
    {
        grind.segment = new GrindSegment(grind.transform, grind.pointPos);
    }

    private void SelectPoint(int index)
    {
        this.selectedPoint = index;
    }

    private void CheckHonduneMethod(SXLGrind grind)
    {
        if (grind.transform.childCount == 0)
        {
            return;
        }

        List<Vector3> childPos = new List<Vector3>();
        foreach (Transform child in grind.transform)
        {
            childPos.Add(child.localPosition);
        }
        grind.pointPos = childPos;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SXLGrind grind = (SXLGrind)target;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+ Add Point")) { AddSplinePoint(grind); }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Load Hondune Grind Points")) { CheckHonduneMethod(grind); }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        scrollPointPosition = GUILayout.BeginScrollView(scrollPointPosition, GUILayout.Height(120));
        if (grind.pointPos.Count > 0)
        {
            for (int i = 0; i < grind.pointPos.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Grind_Point_{i}");
                if (GUILayout.Button("Select")) { SelectPoint(i); }
                if (GUILayout.Button("Remove")) { RemoveSplinePoint(grind, i); }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }

    public void OnSceneGUI()
    {
        SXLGrind grind = (SXLGrind)target;

        if (this.selectedPoint != -1)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 grindPointPos = Handles.PositionHandle(grind.transform.TransformPoint(grind.pointPos[this.selectedPoint]), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(grind, "Undo Movement");
                grind.pointPos[this.selectedPoint] = grind.transform.InverseTransformPoint(grindPointPos); // Capture local offset
            }
        }
    }
}


[CustomEditor(typeof(SXLTeleportVolume))]
public class SXLTeleportVolumeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Select Teleport Endpoint"))
        {
            SXLTeleportVolume obj = (SXLTeleportVolume)target;
            Selection.objects = new GameObject[] { obj.transform.GetChild(0).gameObject };
        }
    }
}