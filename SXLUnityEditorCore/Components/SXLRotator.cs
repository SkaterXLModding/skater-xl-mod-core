using UnityEngine;

[ExecuteAlways]
public class SXLRotator: MonoBehaviour
{
    public enum Axis { X, Y, Z, XY, XZ, YZ, XYZ};
    public Axis rotationAxis;
    public enum Direction { CLOCKWISE, COUNTERCLOCKWISE }
    public Direction rotationDirection;
    public float rotationSpeed;

    public bool viewInEditor = true;

    private Quaternion _bindRot;

    void OnDrawGizmos()
    {
        if (this.viewInEditor)
        {
            Update();
            return;
        }
        this.transform.rotation = this._bindRot;
    }

    void OnEnabled()
    {
        this._bindRot = transform.rotation;
    }

    void Update()
    {
        float dir = this.rotationDirection == Direction.CLOCKWISE ? 1f : -1f;
        if (Application.isPlaying || this.viewInEditor)
        {
            this.transform.Rotate(this.GetAxisSelection() * this.rotationSpeed * dir);
        }
    }

    private Vector3 GetAxisSelection()
    {
        switch (rotationAxis)
        {
            case Axis.X:
                return Vector3.right;
            case Axis.Y:
                return Vector3.up;
            case Axis.Z:
                return Vector3.forward;
            case Axis.XY:
                return new Vector3(1, 1, 0);
            case Axis.XZ:
                return new Vector3(1, 0, 1);
            case Axis.YZ:
                return new Vector3(0, 1, 1);
            case Axis.XYZ:
                return Vector3.one;
            default:
                return Vector3.zero;
        }
    }


}
