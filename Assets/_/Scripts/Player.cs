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

public class Player : NetworkBehaviour
{
    [SerializeField] private KCC kcc;

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

        kcc.SetManualUpdate(true);
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
            HeadPosition = XRHelper.Current.Head.position,
            HeadRotation = XRHelper.Current.Head.rotation,

            LeftHandPosition = XRHelper.Current.LeftHand.position,
            LeftHandRotation = XRHelper.Current.LeftHand.rotation,
            LeftGrabInfo = XRHelper.Current.LeftHandGrip ? LeftGrabber.Grab(FindGrabble(LeftGrabber.transform.position)) : default,

            RightHandPosition = XRHelper.Current.RightHand.position,
            RightHandRotation = XRHelper.Current.RightHand.rotation,
            RightGrabInfo = XRHelper.Current.RightHandGrip ? RightGrabber.Grab(FindGrabble(RightGrabber.transform.position)) : default,

            MoveDirection = XRHelper.Current.MoveDirection,
            RotateDirection = XRHelper.Current.RotateDirection,
        });
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            inputDataNetwork = inputData;

            kcc.AddLookRotation(new Vector2(0, inputDataNetwork.RotateDirection) * Runner.DeltaTime * 100);
            kcc.SetInputDirection(Head.rotation * new Vector3(inputDataNetwork.MoveDirection.x, 0, inputDataNetwork.MoveDirection.y) * Runner.DeltaTime * 5);
        }

        Head.SetPositionAndRotation(inputDataNetwork.HeadPosition, inputDataNetwork.HeadRotation);
        LeftGrabber.SetPosRotTarget(inputDataNetwork.LeftHandPosition, inputDataNetwork.LeftHandRotation);
        RightGrabber.SetPosRotTarget(inputDataNetwork.RightHandPosition, inputDataNetwork.RightHandRotation);

        kcc.ManualFixedUpdate();
    }

    public override void Render()
    {
        if (HasInputAuthority)
        {
            Head.SetPositionAndRotation(XRHelper.Current.Head.position, XRHelper.Current.Head.rotation);
        }

        XRHelper.Current.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
