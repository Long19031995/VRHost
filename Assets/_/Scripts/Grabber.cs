using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;

    [Networked] private GrabInfo grabInfo { get; set; }

    private Transform target;

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

    public bool TryGrab(out GrabInfo newGrabInfo)
    {
        if (grabInfo.IsNull)
        {
            if (Extension.TryFindGrabble(transform.position, out var grabble))
            {
                newGrabInfo = new GrabInfo()
                {
                    GrabberId = Id,
                    GrabbleId = grabble.Id,
                    PositionOffset = transform.InverseTransformPoint(grabble.transform.position),
                    RotationOffset = Quaternion.Inverse(grabble.transform.rotation) * transform.rotation
                };
                return true;
            }
            else
            {
                newGrabInfo = default;
                return false;
            }
        }
        else
        {
            newGrabInfo = grabInfo;
        }

        return false;
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
    }
}
