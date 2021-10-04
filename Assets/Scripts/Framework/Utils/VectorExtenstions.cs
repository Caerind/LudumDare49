using UnityEngine;

public static class VectorExtenstions
{
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector2 xz(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 ToVector3(this Vector2 v, float z = 0.0f)
    {
        return new Vector3(v.x, v.y, z);
    }
}
