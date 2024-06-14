using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    public GrabInfo GrabInfo => grabInfo;

    [Networked] private GrabInfo grabInfo { get; set; }
    private GrabDataCache dataCache;
    private Grabber grabber;
    private Pose offset;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        dataCache = new GrabDataCache(Runner);
    }

    public void SetGrabInfo(GrabInfo newGrabInfo, bool hasInput = true)
    {
        grabInfo = newGrabInfo;
        Object.RenderTimeframe = hasInput ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }

    public override void FixedUpdateNetwork()
    {
        if (dataCache.TryGet(grabInfo.GrabberId, out Grabber newGrabber) && newGrabber != grabber)
        {
            offset = Extension.GetPoseOffset(newGrabber.transform, transform);
        }
        grabber = newGrabber;

        if (grabber != null)
        {
            var poseTarget = Extension.GetPoseTarget(grabber.Target, grabInfo.PositionOffset, grabInfo.RotationOffset);
            var poseCurrent = new Pose(transform.position, transform.rotation);

            rbNet.Rigidbody.SetVelocity(poseCurrent, poseTarget, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }

    public override void Render()
    {
        if (grabber != null)
        {
            var target = Extension.GetPoseTarget(transform, offset.Position, offset.Rotation);
            grabber.transform.SetPositionAndRotation(target.Position, target.Rotation);
        }
    }
}
