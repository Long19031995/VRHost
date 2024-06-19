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

    private DataCache dataCache;
    private Grabble grabble;
    private Vector3 positionReal;
    private bool hasCollision;

    public override void Spawned()
    {
        InputAuthorityLost();

        Target = new GameObject("Target").transform;
        Target.SetParent(transform);

        dataCache = new DataCache(Runner);
    }

    public void InputAuthorityLost()
    {
        Runner.SetIsSimulated(Object, true);
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }

    public void SetPositionAndRotationTarget(Vector3 positionTarget, Quaternion rotationTarget)
    {
        Target.SetPositionAndRotation(positionTarget, rotationTarget);
    }

    public void SetPositionReal(Vector3 positionReal)
    {
        this.positionReal = positionReal;
    }

    public GrabInfo Grab(Grabble grabble)
    {
        if (grabInfo.IsDefault)
        {
            if (grabble == null || !grabble.GrabInfo.IsDefault) return default;

            var offset = Extension.GetPoseOffset(grabble.transform, transform);
            return new GrabInfo()
            {
                GrabberSide = side,
                GrabberId = Id,
                GrabbleId = grabble.Id,
                PositionOffset = offset.Position,
                RotationOffset = offset.Rotation
            };
        }

        return grabInfo;
    }

    public override void FixedUpdateNetwork()
    {
        rbNet.Rigidbody.SetVelocity(transform, Target, Runner.DeltaTime);

        if (GetInput(out InputData inputData))
        {
            grabInfo = side == GrabberSide.Left ? inputData.LeftGrabInfo : inputData.RightGrabInfo;
        }

        if (grabInfo.GrabberId == Id && dataCache.TryGet(grabInfo.GrabbleId, out Grabble newGrabble))
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

    public override void Render()
    {
        if (HasInputAuthority)
        {
            if (grabble != null)
            {
                visual.position = transform.position;
            }
            else if (hasCollision)
            {
                visual.position = Vector3.Lerp(visual.position, transform.position, Time.deltaTime * 15);
            }
            else
            {
                visual.position = positionReal;
            }
        }
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
