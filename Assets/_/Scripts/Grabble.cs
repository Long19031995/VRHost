using Fusion;
using UnityEngine;

[RequireComponent(typeof(Follower))]
public class Grabble : NetworkBehaviour
{
    private Follower follower;
    private Transform target;

    public bool HasTarget => target != null;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        follower = GetComponent<Follower>();
    }

    public void Follow(Transform newTarget, FollowerType type)
    {
        target = newTarget;
        follower.Follow(newTarget, type);
    }

    public void UnFollow()
    {
        target = null;
        follower.UnFollow();
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }
}
