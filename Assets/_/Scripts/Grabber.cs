using Fusion;
using UnityEngine;

[RequireComponent(typeof(Follower))]
public class Grabber : NetworkBehaviour
{
    private Follower follower;
    private Grabble grabble;
    private Transform target;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        target = new GameObject("Target").transform;
        target.SetParent(transform);

        follower = GetComponent<Follower>();
        follower.Follow(target, FollowerType.Velocity, false);
    }

    public void SetTarget(Vector3 position, Quaternion rotation)
    {
        target.SetPositionAndRotation(position, rotation);
    }

    public void Grab(Grabble newGrabble)
    {
        if (!grabble)
        {
            grabble = newGrabble;

            follower.Follow(grabble.transform, FollowerType.Instant);
            grabble.Follow(target, FollowerType.Velocity);

            if (HasStateAuthority) grabble.Object.AssignInputAuthority(Object.InputAuthority);
        }
    }

    public void UnGrab()
    {
        if (grabble)
        {
            grabble.UnFollow();
            follower.Follow(target, FollowerType.Velocity, false);

            grabble = null;
        }
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }
}
