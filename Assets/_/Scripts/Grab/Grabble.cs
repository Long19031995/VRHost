using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    [Networked] private GrabInfo grabInfo { get; set; }
    private GrabDataCache dataCache;
    private Grabber grabber;
    private Vector3 posOffset;
    private Quaternion rotOffset;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        dataCache = new GrabDataCache(Runner);
    }

    public void SetGrabInfo(GrabInfo newGrabInfo, bool hasInput = true)
    {
        grabInfo = newGrabInfo;
        Object.RenderTimeframe = hasInput ? RenderTimeframe.Local : RenderTimeframe.Remote;

        if (grabInfo.IsDefault) grabber = null;
    }

    public GrabInfo GetGrabInfo()
    {
        return grabInfo;
    }

    public override void FixedUpdateNetwork()
    {
        if (dataCache.TryGet(grabInfo.GrabberId, out Grabber newGrabber))
        {
            if (newGrabber != grabber)
            {
                posOffset = transform.InverseTransformPoint(newGrabber.transform.position);
                rotOffset = Quaternion.Inverse(newGrabber.transform.rotation) * transform.rotation;
            }

            grabber = newGrabber;
        }

        if (grabber != null)
        {
            rbNet.Rigidbody.SetVelocity(transform, grabber.Target.transform, grabInfo.PositionOffset, grabInfo.RotationOffset, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }

    public override void Render()
    {
        if (grabber != null)
        {
            grabber.transform.position = transform.TransformPoint(posOffset);
            grabber.transform.rotation = transform.rotation * Quaternion.Inverse(rotOffset);
        }
    }
}
