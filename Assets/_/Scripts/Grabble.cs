using Fusion;
using UnityEngine;

[RequireComponent(typeof(Follower))]
public class Grabble : NetworkBehaviour
{
    private Follower follower;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        follower = GetComponent<Follower>();
    }

    public void Follow(Transform target, FollowerType type)
    {
        follower.Follow(target, type);
    }

    public void UnFollow()
    {
        follower.UnFollow();
    }

    public override void FixedUpdateNetwork()
    {
        Object.RenderTimeframe = HasInputAuthority ? RenderTimeframe.Local : RenderTimeframe.Remote;
    }
}
