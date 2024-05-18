using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public enum FollowerType
{
    Instant,
    Velocity,
    Force
}

public class Follower : NetworkBehaviour
{
    [SerializeField] private FollowerType type;
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float forceSpeed = 20f;
    [SerializeField] private float torqueSpeed = 0.08f;

    private Transform target;
    private bool hasOffset;
    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    public void Follow(Transform target, FollowerType type = FollowerType.Instant, bool hasOffset = true)
    {
        this.target = target;
        this.type = type;
        this.hasOffset = hasOffset;

        if (hasOffset)
        {
            positionOffset = target.InverseTransformPoint(transform.position);
            rotationOffset = target.rotation * Quaternion.Inverse(transform.rotation);
        }
    }

    public void UnFollow()
    {
        target = null;
    }

    public override void FixedUpdateNetwork()
    {
        if (target)
        {
            if (type == FollowerType.Velocity)
            {
                FollowVelocity();
            }

            if (type == FollowerType.Force)
            {
                FollowForce();
            }
        }
    }

    public override void Render()
    {
        if (target)
        {
            if (type == FollowerType.Instant)
            {
                FollowInstant();
            }
        }
    }

    private (Vector3, Quaternion) GetPositionAndRotationTarget()
    {
        var positionTarget = hasOffset ? target.TransformPoint(positionOffset) : target.position;
        var rotationTarget = hasOffset ? target.rotation * Quaternion.Inverse(rotationOffset) : target.rotation;

        return (positionTarget, rotationTarget);
    }

    private void FollowInstant()
    {
        var (positionTarget, rotationTarget) = GetPositionAndRotationTarget();
        transform.SetPositionAndRotation(positionTarget, rotationTarget);
    }

    private void FollowVelocity()
    {
        var (positionTarget, rotationTarget) = GetPositionAndRotationTarget();

        var rb = rbNet.Rigidbody;

        var direction = positionTarget - rb.position;
        rb.velocity = direction / Runner.DeltaTime * speed;

        var directionAngular = Extension.GetDirectionAngular(rb.rotation, rotationTarget);
        rb.angularVelocity = directionAngular / Runner.DeltaTime * speed;
    }

    private void FollowForce()
    {
        var (positionTarget, rotationTarget) = GetPositionAndRotationTarget();

        var rb = rbNet.Rigidbody;

        var direction = positionTarget - rb.position;
        var force = (direction / Runner.DeltaTime - rb.velocity) * forceSpeed;
        rb.AddForce(force);

        var directionAngular = Extension.GetDirectionAngular(rb.rotation, rotationTarget);
        var torque = (directionAngular / Runner.DeltaTime - rb.angularVelocity) * torqueSpeed;
        rb.AddTorque(torque);
    }
}
