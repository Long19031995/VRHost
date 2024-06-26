using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

[DefaultExecutionOrder(1)]
[RequireComponent(typeof(GrabberRender))]
public class Grabber : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody3D rbNet;
    [SerializeField] private GrabberSide side;
    [SerializeField] private Transform visual;

    public GrabberSide Side => side;
    public Transform Visual => visual;
    public Transform Target => target;
    public Grabble Grabble => grabble;
    public GrabInfo GrabInfo => grabInfo;

    [Networked] private GrabInfo grabInfo { get; set; }

    private Transform target;
    private Grabble grabble;
    private Collider[] colliders;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        target = new GameObject("Target").transform;
        target.SetParent(transform);

        colliders = GetComponentsInChildren<Collider>();
    }

    public void SetPositionAndRotationTarget(Vector3 positionTarget, Quaternion rotationTarget)
    {
        target.SetPositionAndRotation(positionTarget, rotationTarget);
    }

    public GrabInfo Grab(Grabble grabble)
    {
        if (grabble == null) return default;

        if (!grabInfo.IsDefault) return grabInfo;

        return grabble.GetGrabInfo(visual, Id, side);
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
            grabble.SetGrabber(this, HasInputAuthority);
            SetTrigger(true);
        }
        else if (grabInfo.IsDefault)
        {
            grabble?.SetGrabber(null, HasInputAuthority);
            grabble = null;
            SetTrigger(false);
        }
    }

    private void SetTrigger(bool isTrigger)
    {
        foreach (var collider in colliders)
        {
            collider.isTrigger = isTrigger;
        }
    }
}
