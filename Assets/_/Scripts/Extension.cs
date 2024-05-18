using UnityEngine;

public static class Extension
{
    public static Vector3 GetDirectionAngular(Quaternion current, Quaternion target)
    {
        (target * Quaternion.Inverse(current)).ToAngleAxis(out var angle, out var axis);
        if (angle > 180f) angle -= 360f;

        return angle * Mathf.Deg2Rad * axis;
    }
}
