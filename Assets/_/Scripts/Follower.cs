using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public enum FollowerType
{
    Instant,
    Velocity
}

public class Follower : NetworkBehaviour
{
    [SerializeField] private FollowerType type;
    [SerializeField] private NetworkRigidbody3D rbNet;

    private Transform target;
    private Vector3 posOffset;
    private Quaternion rotOffset;

    private Rigidbody rb => rbNet.Rigidbody;

    public void Follow(Transform target, FollowerType type = FollowerType.Instant, bool hasOffset = true)
    {
        this.target = target;
        this.type = type;

        posOffset = hasOffset ? target.InverseTransformPoint(transform.position) : Vector3.zero;
        rotOffset = hasOffset ? Quaternion.Inverse(transform.rotation) * target.rotation : Quaternion.identity;
    }

    public void UnFollow()
    {
        target = null;
    }

    public override void FixedUpdateNetwork()
    {
        if (target)
        {
            if (type == FollowerType.Instant) FollowInstant();
            else FollowVelocity();
        }
    }

    public override void Render()
    {
        if (target && type == FollowerType.Instant) FollowInstant();
    }

    private void FollowInstant()
    {
        var positionTarget = target.TransformPoint(posOffset);
        var rotationTarget = target.rotation * Quaternion.Inverse(rotOffset);

        transform.SetPositionAndRotation(positionTarget, rotationTarget);
    }

    private void FollowVelocity()
    {
        var positionTarget = target.TransformPoint(posOffset);
        var rotationTarget = target.rotation * Quaternion.Inverse(rotOffset);

        var direction = (positionTarget - transform.position) / 2;
        rb.velocity = direction / Runner.DeltaTime;

        (rotationTarget * Quaternion.Inverse(transform.rotation)).ToAngleAxis(out var angle, out var axis);
        if (angle > 180f) angle -= 360f;
        var directionAngular = angle * Mathf.Deg2Rad * axis;
        rb.angularVelocity = directionAngular / Runner.DeltaTime;
    }
}
