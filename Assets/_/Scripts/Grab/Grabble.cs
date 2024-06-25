using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    [Networked] private GrabInfo grabInfo { get; set; }

    private Grabber grabber;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
    }

    public GrabInfo GetGrabInfo(Grabber grabber, GrabberSide side)
    {
        if (!grabInfo.IsDefault) return default;

        var offset = Extension.GetPoseOffset(transform, grabber.Target);
        return new GrabInfo()
        {
            GrabberSide = side,
            GrabberId = grabber.Id,
            GrabbleId = Id,
            PositionOffset = offset.Position,
            RotationOffset = offset.Rotation
        };
    }

    public void SetGrabInfo(GrabInfo newGrabInfo, Grabber newGrabber, bool hasInput)
    {
        grabInfo = newGrabInfo;
        grabber = newGrabber;

        Object.RenderTimeframe = hasInput ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }

    public override void FixedUpdateNetwork()
    {
        if (grabber != null)
        {
            var poseTarget = Extension.GetPoseTarget(grabber.Target, grabInfo.PositionOffset, grabInfo.RotationOffset);
            var poseCurrent = new PoseHand(transform.position, transform.rotation);

            rbNet.Rigidbody.SetVelocity(poseCurrent, poseTarget, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }
}
