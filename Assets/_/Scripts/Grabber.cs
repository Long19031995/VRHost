using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    [Networked] private GrabInfo grabInfo { get; set; }

    private Transform target;
    public Transform Target => target;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        target = new GameObject("Target").transform;
        target.SetParent(transform);
    }

    public void SetPosRotTarget(Vector3 posTarget, Quaternion rotTarget)
    {
        target.SetPositionAndRotation(posTarget, rotTarget);
    }

    public GrabInfo Grab(Grabble grabble)
    {
        GrabInfo newGrabInfo;

        if (grabInfo.IsNull)
        {
            if (grabble != null)
            {
                var (posOffset, rotOffset) = Extension.GetPosRotOffset(grabble.transform, transform);

                newGrabInfo = new GrabInfo()
                {
                    GrabberId = Id,
                    GrabbleId = grabble.Id,
                    PositionOffset = posOffset,
                    RotationOffset = rotOffset
                };
            }
            else
            {
                newGrabInfo = default;
            }
        }
        else
        {
            newGrabInfo = grabInfo;
        }

        return newGrabInfo;
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;

        rbNet.Rigidbody.SetVelocity(transform, target, Runner.DeltaTime);

        if (GetInput(out InputData inputData))
        {
            grabInfo = inputData.GrabInfo;
        }

        if (grabInfo.GrabberId == Id)
        {
            if (Runner.TryFindBehaviour(grabInfo.GrabbleId, out Grabble grabble))
            {
                if (grabble.TrySetGrabInfo(grabInfo))
                {
                    if (HasStateAuthority)
                    {
                        grabble.Object.AssignInputAuthority(Object.InputAuthority);
                    }
                }
            }
        }
        else if (grabInfo.GrabbleId != NetworkBehaviourId.None)
        {
            if (Runner.TryFindBehaviour(grabInfo.GrabbleId, out Grabble grabble))
            {
                if (grabble.Object.InputAuthority == Object.InputAuthority)
                {
                    if (HasStateAuthority)
                    {
                        grabble.Object.RemoveInputAuthority();
                    }
                }
            }

            grabInfo = default;
        }
    }
}
