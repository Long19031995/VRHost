using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    [Networked] private GrabInfo grabInfo { get; set; }
    private GrabDataCache dataCache;

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

    public GrabInfo GetGrabInfo()
    {
        return grabInfo;
    }

    public override void FixedUpdateNetwork()
    {
        if (!grabInfo.IsDefault && dataCache.TryGet(grabInfo.GrabberId, out Grabber grabber) && grabber != null)
        {
            rbNet.Rigidbody.SetVelocity(transform, grabber.Target.transform, grabInfo.PositionOffset, grabInfo.RotationOffset, Runner.DeltaTime);
            rbNet.Rigidbody.velocity /= 2;
        }
    }
}
