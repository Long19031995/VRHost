using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabble : NetworkBehaviour
{
    [Networked] private Vector3 OffsetPosition { get; set; }
    [Networked] private Quaternion OffsetRotation { get; set; }

    private Grabber grabber;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        base.Spawned();

        Runner.SetIsSimulated(Object, true);
    }

    public void Grab(Grabber grabber, Vector3 offsetPosition, Quaternion offsetRotation)
    {
        this.grabber = grabber;

        OffsetPosition = offsetPosition;
        OffsetRotation = offsetRotation;
    }

    public void Ungrab()
    {
        grabber = null;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (grabber)
        {
            var positionTarget = grabber.transform.TransformPoint(OffsetPosition);
            var rotationTarget = transform.rotation * OffsetRotation;

            rb.VelocityFollow(positionTarget, rotationTarget, Runner.DeltaTime);
        }
    }
}
