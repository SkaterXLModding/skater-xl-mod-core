using System.Collections.Generic;
using UnityEngine;


public static class SXLExtensions
{

}

public class Generic<T>
{
    public Generic()
    {

    }
}

public class TransformStore
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public TransformStore()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        localScale = Vector3.one;
    }
}

public static class TransformSerialization
{
    public static TransformStore SaveLocal(this Transform transform)
    {
        return new TransformStore()
        {
            position = transform.localPosition,
            rotation = transform.localRotation,
            localScale = transform.localScale
        };
    }

    public static TransformStore SaveWorld(this Transform transform)
    {
        return new TransformStore()
        {
            position = transform.position,
            rotation = transform.rotation,
            localScale = transform.localScale
        };
    }

    public static void LoadLocal(this Transform transform, TransformStore store)
    {
        transform.localPosition = store.position;
        transform.localRotation = store.rotation;
        transform.localScale = store.localScale;
    }

    public static void LoadWorld(this Transform transform, TransformStore store)
    {
        transform.position = store.position;
        transform.rotation = store.rotation;
        transform.localScale = store.localScale;
    }
}
