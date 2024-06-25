using Fusion;
using System.Collections.Generic;
using UnityEngine;

public struct InputData : INetworkInput
{
    public Vector3 HeadPosition;
    public Quaternion HeadRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;
    public GrabInfo LeftGrabInfo;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;
    public GrabInfo RightGrabInfo;

    public Vector2 MoveDirection;
    public Vector2 RotateDirection;
}

public struct PoseHand
{
    public Vector3 Position;
    public Quaternion Rotation;

    public PoseHand(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

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

public static class DataCache
{
    private static Dictionary<NetworkBehaviourId, NetworkBehaviour> datasCached = new Dictionary<NetworkBehaviourId, NetworkBehaviour>();

    public static bool TryGet<T>(NetworkRunner runner, NetworkBehaviourId id, out T t) where T : NetworkBehaviour
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