using UnityEngine;

public static class Extensions
{
    public static float ManhattanSize2D(this Vector3 vec)
    {
        return Mathf.Abs(vec.x) + Mathf.Abs(vec.y);
    }
}
