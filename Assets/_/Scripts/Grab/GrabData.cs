using Fusion;
using System.Collections.Generic;
using UnityEngine;

public enum GrabberSide
{
    Left,
    Right
}

public struct GrabInfo : INetworkStruct
{
    public GrabberSide GrabberSide;

    public NetworkBehaviourId GrabberId;
    public NetworkBehaviourId GrabbleId;

    public Vector3 PositionOffset;
    public Quaternion RotationOffset;

    public readonly bool IsDefault => GrabberId == NetworkBehaviourId.None && GrabbleId == NetworkBehaviourId.None;
}

public class GrabDataCache
{
    private Dictionary<NetworkBehaviourId, NetworkBehaviour> datasCached = new Dictionary<NetworkBehaviourId, NetworkBehaviour>();
    private NetworkRunner runner;

    public GrabDataCache(NetworkRunner runner)
    {
        this.runner = runner;
    }

    public bool TryGet<T>(NetworkBehaviourId id, out T t) where T : NetworkBehaviour
    {
        if (datasCached.ContainsKey(id))
        {
            t = datasCached[id] as T;
            return true;
        }

        if (runner.TryFindBehaviour(id, out NetworkBehaviour behaviour))
        {
            datasCached[id] = behaviour;
            t = behaviour as T;
            return true;
        }

        t = null;
        return false;
    }
}