using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public struct GrabInfo : INetworkStruct
{
    public NetworkBehaviourId GrabberId;
    public NetworkBehaviourId GrabbleId;
    public Vector3 PositionOffset;
    public Quaternion RotationOffset;

    public readonly bool IsNull => GrabberId == NetworkBehaviourId.None && GrabbleId == NetworkBehaviourId.None;
}

public class Grabble : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    [Networked] private GrabInfo grabInfo { get; set; }

    private Tick tickGrab = int.MaxValue;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
    }

    public bool TrySetGrabInfo(GrabInfo newGrabInfo)
    {
        if (grabInfo.IsNull)
        {
            grabInfo = newGrabInfo;
            tickGrab = Runner.Tick;
            return true;
        }

        return false;
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;

        if (GetInput(out InputData inputData))
        {
            Follow(inputData.GrabInfo);

            tickGrab = int.MaxValue;
        }
        else if (Runner.Tick >= tickGrab && Runner.Tick - tickGrab < 100)
        {
            Follow(grabInfo);
        }
    }

    private void Follow(GrabInfo grabInfo)
    {
        if (grabInfo.GrabbleId == Id)
        {
            if (Runner.TryFindBehaviour(grabInfo.GrabberId, out Grabber grabber))
            {
                rbNet.Rigidbody.SetVelocity(transform, grabber.Target.transform, grabInfo.PositionOffset, grabInfo.RotationOffset, Runner.DeltaTime);
            }
        }
    }
}
