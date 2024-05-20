using Fusion;
using UnityEngine;

[RequireComponent(typeof(Follower))]
public class Grabble : NetworkBehaviour
{
    private Follower follower;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);
        Object.RenderTimeframe = RenderTimeframe.Remote;

        follower = GetComponent<Follower>();
    }

    public void Follow(Transform target, FollowerType type)
    {
        follower.Follow(target, type);
        Object.RenderTimeframe = RenderTimeframe.Local;
    }

    public void UnFollow()
    {
        follower.UnFollow();
        Object.RenderTimeframe = RenderTimeframe.Remote;
    }
}
