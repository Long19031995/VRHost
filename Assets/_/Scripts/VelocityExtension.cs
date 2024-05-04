using UnityEngine;

public static class VelocityExtension
{
    public static Vector3 AngularVelocityChange(this Quaternion previousRotation, Quaternion newRotation, float elapsedTime)
    {
        Quaternion rotationStep = newRotation * Quaternion.Inverse(previousRotation);
        rotationStep.ToAngleAxis(out float angle, out Vector3 axis);
        // Angular velocity uses eurler notation, bound to -180� / +180�
        if (angle > 180f)
        {
            angle -= 360f;
        }

        if (Mathf.Abs(angle) > Mathf.Epsilon)
        {
            float radAngle = angle * Mathf.Deg2Rad;
            Vector3 angularStep = axis * radAngle;
            Vector3 angularVelocity = angularStep / elapsedTime;
            if (!float.IsNaN(angularVelocity.x))
                return angularVelocity;
        }
        return Vector3.zero;
    }

    public static void VelocityFollow(this Rigidbody followerRb, Transform target, Vector3 positionOffset, Quaternion rotationOffset, float elapsedTime)
    {
        followerRb.VelocityFollow(target.TransformPoint(positionOffset), target.rotation * rotationOffset, elapsedTime);
    }

    public static void VelocityFollow(this Rigidbody followerRb, Vector3 targetPosition, Quaternion targetRotation, float elapsedTime)
    {
        Vector3 positionStep = targetPosition - followerRb.transform.position;
        Vector3 velocity = positionStep / elapsedTime;

        followerRb.velocity = velocity;
        followerRb.angularVelocity = followerRb.transform.rotation.AngularVelocityChange(newRotation: targetRotation, elapsedTime: elapsedTime);
    }
}