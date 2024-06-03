using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Grabber : NetworkBehaviour, IInputAuthorityLost
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private GrabberSide side;

    public Transform Target { get; private set; }

    [Networked] private GrabInfo grabInfo { get; set; }
    private GrabDataCache dataCache;
    private Grabble grabble;

    public override void Spawned()
    {
        InputAuthorityLost();

        Target = new GameObject("Target").transform;
        Target.SetParent(transform);

        dataCache = new GrabDataCache(Runner);
    }

    public void InputAuthorityLost()
    {
        Runner.SetIsSimulated(Object, true);
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;
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

    public void FixedUpdate()
    {
        rbNet.Rigidbody.SetVelocity(transform, Target, Time.fixedDeltaTime);
    }

    public override void FixedUpdateNetwork()
    {
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
