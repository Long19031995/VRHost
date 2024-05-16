using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    private Vector3 positionTarget;
    private Quaternion rotationTarget;

    public void SetPositionAndRotation(Vector3 positionTarget, Quaternion rotationTarget)
    {
        this.positionTarget = positionTarget;
        this.rotationTarget = rotationTarget;
    }

    public override void FixedUpdateNetwork()
    {
        var rb = rbNet.Rigidbody;

        var direction = positionTarget - rb.position;
        var force = Extension.GetForce(rb.velocity, direction, Runner.DeltaTime, 2) * 30;
        rb.AddForce(force);

        var directionAngular = Extension.GetDirectionAngular(rb.rotation, rotationTarget);
        var torque = Extension.GetForce(rb.angularVelocity, directionAngular, Runner.DeltaTime, 1) / 25;
        rb.AddTorque(torque);
    }
}
