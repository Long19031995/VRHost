using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private float forceSpeed = 20;
    [SerializeField] private float torqueSpeed = 0.08f;

    private Vector3 positionTarget;
    private Quaternion rotationTarget;

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        positionTarget = position;
        rotationTarget = rotation;
    }

    public override void FixedUpdateNetwork()
    {
        if (rbNet)
        {
            var rb = rbNet.Rigidbody;

            var direction = positionTarget - rb.position;
            rb.AddForce((direction / Runner.DeltaTime - rb.velocity) * forceSpeed);

            var directionAngular = Extension.GetDirectionAngular(rb.rotation, rotationTarget);
            rb.AddTorque((directionAngular / Runner.DeltaTime - rb.angularVelocity) * torqueSpeed);
        }
    }
}
