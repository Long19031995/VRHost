using UnityEngine;

public static class Extension
{
    public static Vector3 GetDirectionAngular(Quaternion current, Quaternion target)
    {
        (target * Quaternion.Inverse(current)).ToAngleAxis(out var angle, out var axis);
        if (angle > 180f) angle -= 360f;

        return angle * Mathf.Deg2Rad * axis;
    }

    public static Vector3 GetForce(Vector3 current, Vector3 target, float deltaTime, float extra)
    {
        return (target - deltaTime * extra * current) / deltaTime;
    }
}
