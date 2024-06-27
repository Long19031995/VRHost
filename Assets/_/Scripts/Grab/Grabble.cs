using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private Transform visual;

    public Transform Visual => visual;

    private Grabber grabber;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
    }

    public GrabInfo GetGrabInfo(Transform target, NetworkBehaviourId grabberId, GrabberSide grabberSide)
    {
        if (grabber != null) return default;

        var positionOffset = target.InverseTransformPoint(visual.position);
        var rotationOffset = Quaternion.Inverse(visual.rotation) * target.rotation;

        return new GrabInfo()
        {
            GrabberSide = grabberSide,
            GrabberId = grabberId,
            GrabbleId = Id,
            PositionOffset = positionOffset,
            RotationOffset = rotationOffset
        };
    }

    public void SetGrabber(Grabber newGrabber, bool hasInput)
    {
        grabber = newGrabber;

        Object.RenderTimeframe = hasInput ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }

    public override void FixedUpdateNetwork()
    {
        if (grabber != null)
        {
            var positionTarget = grabber.Target.TransformPoint(grabber.GrabInfo.PositionOffset);
            var rotationTarget = grabber.Target.rotation * Quaternion.Inverse(grabber.GrabInfo.RotationOffset);

            rbNet.Rigidbody.SetVelocity(transform.position, transform.rotation, positionTarget, rotationTarget, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }

    public override void Render()
    {
        visual.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
