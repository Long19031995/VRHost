using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class Grabber : NetworkBehaviour, IInputAuthorityLost
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private GrabberSide side;
    [SerializeField] private Transform visual;

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

    public override void FixedUpdateNetwork()
    {
        rbNet.Rigidbody.SetVelocity(transform, Target, Runner.DeltaTime);
        rbNet.Rigidbody.velocity /= 2;

        if (GetInput(out InputData inputData))
        {
            grabInfo = side == GrabberSide.Left ? inputData.LeftGrabInfo : inputData.RightGrabInfo;
        }

        if (grabInfo.GrabberId == Id && dataCache.TryGet(grabInfo.GrabbleId, out Grabble newGrabble))
        {
            grabble = newGrabble;
            grabble.SetGrabInfo(grabInfo, HasInputAuthority);
        }
        else if (grabInfo.IsDefault)
        {
            grabble?.SetGrabInfo(default, HasInputAuthority);
            grabble = null;
        }
    }

    public override void Render()
    {
        visual.position = grabble || hasCollision ? transform.position : posReal;
    }

    private Vector3 posReal;
    private bool hasCollision;

    public void SetPosReal(Vector3 posReal)
    {
        this.posReal = posReal;
    }

    private void OnCollisionStay(Collision collision)
    {
        hasCollision = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        hasCollision = false;
    }
}
