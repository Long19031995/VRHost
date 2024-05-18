using Fusion;
using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(Follower))]
public class Grabber : NetworkBehaviour
{
    private Follower follower;
    private Grabble grabble;
    private Transform target;

    public override void Spawned()
    {
        follower = GetComponent<Follower>();

        target = new GameObject("Target").transform;
        target.SetParent(transform);

        follower.Follow(target, FollowerType.Velocity, false);
    }

    public void SetTarget(Vector3 position, Quaternion rotation)
    {
        target.SetPositionAndRotation(position, rotation);
    }

    public void Grab(Grabble grabble)
    {
        follower.Follow(grabble.transform, FollowerType.Instant);
        grabble.Follow(target, FollowerType.Force);
        this.grabble = grabble;
    }

    public void UnGrab()
    {
        if (grabble)
        {
            follower.Follow(target, FollowerType.Velocity, false);
            grabble.UnFollow();
            grabble = null;
        }
    }
}
