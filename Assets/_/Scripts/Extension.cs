using UnityEngine;

public static class Extension
{
    public static PoseHand GetPoseOffset(Transform current, Transform target)
    {
        var positionOffset = target.InverseTransformPoint(current.position);
        var rotationOffset = Quaternion.Inverse(current.rotation) * target.rotation;

        return new PoseHand(positionOffset, rotationOffset);
    }

    public static PoseHand GetPoseTarget(Transform current, Vector3 positionOffset, Quaternion rotationOffset)
    {
        var positionTarget = current.TransformPoint(positionOffset);
        var rotationTarget = current.rotation * Quaternion.Inverse(rotationOffset);

        return new PoseHand(positionTarget, rotationTarget);
    }

    public static void SetVelocity(this Rigidbody rb, PoseHand current, PoseHand target, float deltaTime)
    {
        rb.velocity = GetVelocity(current.Position, target.Position, deltaTime);
        rb.angularVelocity = GetAngularVelocity(current.Rotation, target.Rotation, deltaTime);
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
