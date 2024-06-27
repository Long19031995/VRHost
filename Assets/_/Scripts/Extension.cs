using UnityEngine;

public static class Extension
{
    public static void SetVelocity(this Rigidbody rb, Vector3 positionCurrent, Quaternion rotationCurrent, Vector3 positionTarget, Quaternion rotationTarget, float deltaTime)
    {
        rb.velocity = (positionTarget - positionCurrent) / deltaTime;
        rb.angularVelocity = GetAngularVelocity(rotationCurrent, rotationTarget, deltaTime);
    }

    public static Vector3 GetAngularVelocity(Quaternion current, Quaternion target, float deltaTime)
    {
        (target * Quaternion.Inverse(current)).ToAngleAxis(out var angle, out var axis);

        return axis.IsInfinity() ? Vector3.zero : (angle > 180 ? angle - 360 : angle) * Mathf.Deg2Rad * axis / deltaTime;
    }

    public static bool IsInfinity(this Vector3 axis)
    {
        return float.IsInfinity(axis.x) || float.IsInfinity(axis.y) || float.IsInfinity(axis.z);
    }
}
