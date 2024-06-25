using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private GrabberSide side;

    public Transform Target => target;

    [Networked] private GrabInfo grabInfo { get; set; }

    private Transform target;
    private Grabble grabble;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        target = new GameObject("Target").transform;
        target.SetParent(transform);
    }

    public void SetPositionAndRotationTarget(Vector3 positionTarget, Quaternion rotationTarget)
    {
        target.SetPositionAndRotation(positionTarget, rotationTarget);
    }

    public GrabInfo Grab(Grabble grabble)
    {
        if (grabble == null) return default;

        if (!grabInfo.IsDefault) return grabInfo;

        return grabble.GetGrabInfo(this, side);
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;

        rbNet.Rigidbody.SetVelocity(transform, target, Runner.DeltaTime);
        rbNet.Rigidbody.velocity /= 2;

        if (GetInput(out InputData inputData))
        {
            grabInfo = side == GrabberSide.Left ? inputData.LeftGrabInfo : inputData.RightGrabInfo;
        }

        if (grabInfo.GrabberId == Id && DataCache.TryGet(Runner, grabInfo.GrabbleId, out Grabble newGrabble))
        {
            grabble = newGrabble;
            grabble.SetGrabInfo(grabInfo, this, HasInputAuthority);
        }
        else if (grabInfo.IsDefault)
        {
            grabble?.SetGrabInfo(default, null, HasInputAuthority);
            grabble = null;
        }
    }
}
