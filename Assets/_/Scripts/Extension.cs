using UnityEngine;

public static class Extension
{
    public static (Vector3, Quaternion) GetPosRotOffset(Transform current, Transform target)
    {
        var posOffset = target.InverseTransformPoint(current.position);
        var rotOffset = Quaternion.Inverse(current.rotation) * target.rotation;

        return (posOffset, rotOffset);
    }

    public static void SetVelocity(this Rigidbody rb, Transform current, Transform target, Vector3 posOffset, Quaternion rotOffset, float deltaTime)
    {
        var pos = target.TransformPoint(posOffset);
        var rot = target.rotation * Quaternion.Inverse(rotOffset);

        rb.velocity = GetVelocity(current.position, pos, deltaTime);
        rb.angularVelocity = GetAngularVelocity(current.rotation, rot, deltaTime);
    }

    public static void SetVelocity(this Rigidbody rb, Transform current, Transform target, float deltaTime)
    {
        rb.velocity = GetVelocity(current.position, target.position, deltaTime);
        rb.angularVelocity = GetAngularVelocity(current.rotation, target.rotation, deltaTime);
    }

    public static Vector3 GetVelocity(Vector3 current, Vector3 target, float deltaTime)
    {
        var direction = target - current;
        return direction / deltaTime;
    }

    public static Vector3 GetAngularVelocity(Quaternion current, Quaternion target, float deltaTime)
    {
        (target * Quaternion.Inverse(current)).ToAngleAxis(out var angle, out var axis);
        if (angle > 180) angle -= 360;
        var directionAngular = angle * Mathf.Deg2Rad * axis;
        if (float.IsInfinity(axis.x) || float.IsInfinity(axis.y) || float.IsInfinity(axis.z)) directionAngular = Vector3.zero;
        return directionAngular / deltaTime;
    }
}
