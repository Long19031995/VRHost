using Fusion;
using Fusion.Addons.KCC;
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

[DefaultExecutionOrder(0)]
public class PlayerKCC : NetworkBehaviour
{
    [Networked] private InputData inputDataNetwork { get; set; }

    [SerializeField] private KCC kcc;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotateSpeed = 100;

    [SerializeField] private Transform head;
    [SerializeField] private Grabber leftGrabber;
    [SerializeField] private Grabber rightGrabber;

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
            HeadPosition = InputHandler.Current.HeadTarget.position,
            HeadRotation = InputHandler.Current.HeadTarget.rotation,

            LeftHandPosition = InputHandler.Current.LeftHandTarget.position,
            LeftHandRotation = InputHandler.Current.LeftHandTarget.rotation,
            LeftGrabInfo = InputHandler.Current.LeftHandGrip ? leftGrabber.Grab(FindGrabble(leftGrabber.transform.position)) : default,

            RightHandPosition = InputHandler.Current.RightHandTarget.position,
            RightHandRotation = InputHandler.Current.RightHandTarget.rotation,
            RightGrabInfo = InputHandler.Current.RightHandGrip ? rightGrabber.Grab(FindGrabble(rightGrabber.transform.position)) : default,

            MoveDirection = InputHandler.Current.MoveDirection,
            RotateDirection = InputHandler.Current.RotateDirection,
        });
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            inputDataNetwork = inputData;

            kcc.SetInputDirection(kcc.Data.TransformRotation * new Vector3(inputData.MoveDirection.x, 0, inputData.MoveDirection.y) * Runner.DeltaTime * moveSpeed);
            kcc.AddLookRotation(new Vector2(0, inputData.RotateDirection) * Runner.DeltaTime * rotateSpeed);
        }

        head.SetPositionAndRotation(inputDataNetwork.HeadPosition, inputDataNetwork.HeadRotation);
        leftGrabber.SetPosRotTarget(inputDataNetwork.LeftHandPosition, inputDataNetwork.LeftHandRotation);
        rightGrabber.SetPosRotTarget(inputDataNetwork.RightHandPosition, inputDataNetwork.RightHandRotation);
    }
}
