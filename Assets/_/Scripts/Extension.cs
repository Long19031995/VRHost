using UnityEngine;

public static class Extension
{
    public static void SetVelocity(this Rigidbody rb, Transform current, Transform target, float deltaTime)
    {
        rb.velocity = GetVelocity(current.position, target.position, deltaTime);
        rb.angularVelocity = GetAngularVelocity(current.rotation, target.rotation, deltaTime);
    }

    public static Vector3 GetVelocity(Vector3 current, Vector3 target, float deltaTime)
    {
        var direction = (target - current) / 2;
        return direction / deltaTime;
    }

    public static Vector3 GetAngularVelocity(Quaternion current, Quaternion target, float deltaTime)
    {
        (target * Quaternion.Inverse(current)).ToAngleAxis(out var angle, out var axis);
        var directionAngular = angle * Mathf.Deg2Rad * axis;
        return directionAngular / deltaTime;
    }

    public static bool TryFindGrabble(Vector3 point, out Grabble grabble)
    {
        var colliders = new Collider[1];
        if (Physics.OverlapSphereNonAlloc(point, 10, colliders, 1 << LayerMask.NameToLayer("Grabble")) > 0)
        {
            grabble = colliders[0].GetComponentInChildren<Grabble>();
            return grabble != null;
        }

        grabble = null;
        return false;
    }
}
