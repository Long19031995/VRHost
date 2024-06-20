using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private Transform interpolate;

    public GrabInfo GrabInfo => grabInfo;

    [Networked] private GrabInfo grabInfo { get; set; }
    private DataCache dataCache;
    private Grabber grabber;
    private PoseHand offset;

    private Vector3 positionOld;
    private Quaternion rotationOld;
    private bool isPlayerMoving;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        dataCache = new DataCache(Runner);
    }

    public void SetGrabInfo(GrabInfo newGrabInfo, bool hasInput = true)
    {
        grabInfo = newGrabInfo;
        Object.RenderTimeframe = hasInput ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }

    public void SetIsPlayerMoving(bool isPlayerMoving)
    {
        this.isPlayerMoving = isPlayerMoving;
    }

    public override void FixedUpdateNetwork()
    {
        if (grabInfo.IsDefault)
        {
            grabber = null;
            return;
        }

        if (dataCache.TryGet(grabInfo.GrabberId, out Grabber newGrabber) && newGrabber != grabber)
        {
            offset = Extension.GetPoseOffset(newGrabber.transform, transform);
        }
        grabber = newGrabber;

        if (grabber != null)
        {
            var poseTarget = Extension.GetPoseTarget(grabber.Target, grabInfo.PositionOffset, grabInfo.RotationOffset);
            var poseCurrent = new PoseHand(transform.position, transform.rotation);

            rbNet.Rigidbody.SetVelocity(poseCurrent, poseTarget, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }

    public override void Render()
    {
        if (grabber != null)
        {
            if (isPlayerMoving)
            {
                var position = Vector3.Lerp(positionOld, transform.position, Time.deltaTime * 15);
                var rotation = Quaternion.Lerp(rotationOld, transform.rotation, Time.deltaTime * 15);
                interpolate.SetPositionAndRotation(position, rotation);
            }

            var grabberTarget = Extension.GetPoseTarget(interpolate, offset.Position, offset.Rotation);
            grabber.transform.SetPositionAndRotation(grabberTarget.Position, grabberTarget.Rotation);
        }
        else
        {
            interpolate.SetPositionAndRotation(transform.position, transform.rotation);
        }

        positionOld = interpolate.position;
        rotationOld = interpolate.rotation;
    }
}
