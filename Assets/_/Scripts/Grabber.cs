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
        follower = GetComponent<Follower>();

        target = new GameObject("Target").transform;

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
}
