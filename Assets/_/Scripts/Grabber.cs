using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private float speed = 0.5f;

    private Vector3 positionTarget;
    private Quaternion rotationTarget;

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        positionTarget = position;
        rotationTarget = rotation;
    }

    public override void FixedUpdateNetwork()
    {
        var rb = rbNet.Rigidbody;

        var direction = positionTarget - rb.position;
        rb.velocity = direction / Runner.DeltaTime * speed;

        var directionAngular = Extension.GetDirectionAngular(rb.rotation, rotationTarget);
        rb.angularVelocity = directionAngular / Runner.DeltaTime * speed;
    }
}
