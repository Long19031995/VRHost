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

        var offset = Extension.GetPoseOffset(visual, target);
        return new GrabInfo()
        {
            GrabberSide = grabberSide,
            GrabberId = grabberId,
            GrabbleId = Id,
            PositionOffset = offset.Position,
            RotationOffset = offset.Rotation
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
            var poseCurrent = new PoseHand(transform.position, transform.rotation);
            var poseTarget = Extension.GetPoseTarget(grabber.Target, grabber.GrabInfo.PositionOffset, grabber.GrabInfo.RotationOffset);

            rbNet.Rigidbody.SetVelocity(poseCurrent, poseTarget, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }

    public override void Render()
    {
        visual.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
