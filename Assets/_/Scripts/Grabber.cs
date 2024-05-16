using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField][Range(0f, 3f)] private float velocityExtra = 2;
    [SerializeField][Range(0f, 3f)] private float torqueExtra = 1;
    [SerializeField] string tagCollision;

    private Vector3 positionTarget;
    private Quaternion rotationTarget;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
        if (!HasInputAuthority) Object.RenderTimeframe = RenderTimeframe.Remote;
    }

    public void SetPositionAndRotation(Vector3 positionTarget, Quaternion rotationTarget)
    {
        this.positionTarget = positionTarget;
        this.rotationTarget = rotationTarget;
    }

    public override void FixedUpdateNetwork()
    {
        var rb = rbNet.Rigidbody;

        var direction = positionTarget - rb.position;
        var force = 15 * velocityExtra * Extension.GetForce(direction, rb.velocity, rb.mass, Runner.DeltaTime, velocityExtra);
        rb.AddForce(force);

        var directionAngular = Extension.GetDirectionAngular(rb.rotation, rotationTarget);
        var torque = 0.04f * torqueExtra * Extension.GetForce(directionAngular, rb.angularVelocity, rb.mass, Runner.DeltaTime, torqueExtra);
        rb.AddTorque(torque);
    }
}
