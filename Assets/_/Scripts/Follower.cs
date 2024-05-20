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
    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        ResetRenderTimeframe();
    }

    private void ResetRenderTimeframe()
    {
        if (Object.HasInputAuthority) Object.RenderTimeframe = RenderTimeframe.Local;
        else Object.RenderTimeframe = RenderTimeframe.Remote;
    }

    public void Follow(Transform target, FollowerType type = FollowerType.Instant, bool hasOffset = true)
    {
        this.target = target;
        this.type = type;

        positionOffset = hasOffset ? target.InverseTransformPoint(transform.position) : Vector3.zero;
        rotationOffset = hasOffset ? Quaternion.Inverse(transform.rotation) * target.rotation : Quaternion.identity;

        Object.RenderTimeframe = RenderTimeframe.Local;
    }

    public void UnFollow()
    {
        target = null;

        ResetRenderTimeframe();
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
        var positionTarget = target.TransformPoint(positionOffset);
        var rotationTarget = target.rotation * Quaternion.Inverse(rotationOffset);

        transform.SetPositionAndRotation(positionTarget, rotationTarget);
    }

    private void FollowVelocity()
    {
        var positionTarget = target.TransformPoint(positionOffset);
        var rotationTarget = target.rotation * Quaternion.Inverse(rotationOffset);

        var direction = (positionTarget - transform.position) / 2;
        rbNet.Rigidbody.velocity = direction / Runner.DeltaTime;

        (rotationTarget * Quaternion.Inverse(transform.rotation)).ToAngleAxis(out var angle, out var axis);
        if (angle > 180f) angle -= 360f;
        var directionAngular = angle * Mathf.Deg2Rad * axis;
        rbNet.Rigidbody.angularVelocity = directionAngular / Runner.DeltaTime;
    }
}
