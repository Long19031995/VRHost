using Fusion;
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
    public float RotateDirection;
}

[DefaultExecutionOrder(1)]
public class Player : NetworkBehaviour
{
    [Networked] private InputData inputDataNetwork { get; set; }

    public Transform Head;
    public Grabber LeftGrabber;
    public Grabber RightGrabber;

    public override void Spawned()
    {
        Runner.SetIsSimulated(Object, true);

        if (HasInputAuthority)
        {
            Runner.GetComponent<NetworkEvents>().OnInput.AddListener(OnInput);
        }
    }

    public Grabble FindGrabble(Vector3 point)
    {
        var colliders = new Collider[1];
        if (Physics.OverlapSphereNonAlloc(point, 0.1f, colliders, 1 << LayerMask.NameToLayer("Grabble")) > 0)
        {
            var grabble = colliders[0].GetComponentInChildren<Grabble>();
            return grabble;
        }

        return null;
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(new InputData()
        {
            HeadPosition = InputHandler.Current.Head.position,
            HeadRotation = InputHandler.Current.Head.rotation,

            LeftHandPosition = InputHandler.Current.LeftHand.position,
            LeftHandRotation = InputHandler.Current.LeftHand.rotation,
            LeftGrabInfo = InputHandler.Current.LeftHandGrip ? LeftGrabber.Grab(FindGrabble(LeftGrabber.transform.position)) : default,

            RightHandPosition = InputHandler.Current.RightHand.position,
            RightHandRotation = InputHandler.Current.RightHand.rotation,
            RightGrabInfo = InputHandler.Current.RightHandGrip ? RightGrabber.Grab(FindGrabble(RightGrabber.transform.position)) : default,

            MoveDirection = InputHandler.Current.MoveDirection,
            RotateDirection = InputHandler.Current.RotateDirection,
        });
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            inputDataNetwork = inputData;
        }

        Head.SetPositionAndRotation(inputDataNetwork.HeadPosition, inputDataNetwork.HeadRotation);
        LeftGrabber.SetPosRotTarget(inputDataNetwork.LeftHandPosition, inputDataNetwork.LeftHandRotation);
        RightGrabber.SetPosRotTarget(inputDataNetwork.RightHandPosition, inputDataNetwork.RightHandRotation);
    }
}
