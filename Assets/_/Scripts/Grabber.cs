using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private GrabberSide side;

    public Transform Target { get; private set; }

    [Networked] private GrabInfo grabInfo { get; set; }
    private GrabDataCache dataCache;
    private Grabble grabble;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        Target = new GameObject("Target").transform;
        Target.SetParent(transform);

        dataCache = new GrabDataCache(Runner);
    }

    public void SetPosRotTarget(Vector3 posTarget, Quaternion rotTarget)
    {
        Target.SetPositionAndRotation(posTarget, rotTarget);
    }

    public GrabInfo Grab(Grabble grabble)
    {
        return grabInfo.IsDefault ? GetGrabInfo(grabble) : grabInfo;
    }

    private GrabInfo GetGrabInfo(Grabble grabble)
    {
        if (grabble == null) return default;
        else if (!grabble.GetGrabInfo().IsDefault) return default;

        var (posOffset, rotOffset) = Extension.GetPosRotOffset(grabble.transform, transform);
        return new GrabInfo()
        {
            GrabberSide = side,
            GrabberId = Id,
            GrabbleId = grabble.Id,
            PositionOffset = posOffset,
            RotationOffset = rotOffset
        };
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;

        rbNet.Rigidbody.SetVelocity(transform, Target, Runner.DeltaTime);

        if (GetInput(out InputData inputData))
        {
            grabInfo = side == GrabberSide.Left ? inputData.LeftGrabInfo : inputData.RightGrabInfo;
        }

        if (grabInfo.GrabberId == Id && dataCache.TryGet(grabInfo.GrabbleId, out Grabble newGrabble) && newGrabble != null)
        {
            grabble = newGrabble;
            grabble.SetGrabInfo(grabInfo, HasInputAuthority);
        }
        else if (grabInfo.IsDefault && grabble != null)
        {
            grabble.SetGrabInfo(default, HasInputAuthority);
            grabble = null;
        }
    }
}
