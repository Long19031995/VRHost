using Fusion;
using UnityEngine;

public struct InputData : INetworkInput
{
    public Vector3 HeadPosition;
    public Quaternion HeadRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;
    public NetworkBool LeftHandGrip;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;
    public NetworkBool RightHandGrip;
}

public class Player : NetworkBehaviour
{
    [Networked] private InputData inputDataNetwork { get; set; }

    public Transform Head;
    public Hand LeftHand;
    public Hand RightHand;

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            Runner.GetComponent<NetworkEvents>().OnInput.AddListener(OnInput);
        }
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(new InputData()
        {
            HeadPosition = XROriginHelper.Current.Head.position,
            HeadRotation = XROriginHelper.Current.Head.rotation,

            LeftHandPosition = XROriginHelper.Current.LeftHand.position,
            LeftHandRotation = XROriginHelper.Current.LeftHand.rotation,
            LeftHandGrip = XROriginHelper.Current.LeftHandGrip,

            RightHandPosition = XROriginHelper.Current.RightHand.position,
            RightHandRotation = XROriginHelper.Current.RightHand.rotation,
            RightHandGrip = XROriginHelper.Current.RightHandGrip,
        });
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            inputDataNetwork = inputData;
        }

        Head.SetPositionAndRotation(inputDataNetwork.HeadPosition, inputDataNetwork.HeadRotation);
        RightHand.Grabber.SetTarget(inputDataNetwork.RightHandPosition, inputDataNetwork.RightHandRotation);
        LeftHand.Grabber.SetTarget(inputDataNetwork.LeftHandPosition, inputDataNetwork.LeftHandRotation);

        if (inputDataNetwork.RightHandGrip) RightHand.Grab();
        else RightHand.UnGrab();

        if (inputDataNetwork.LeftHandGrip) LeftHand.Grab();
        else LeftHand.UnGrab();
    }

    public override void Render()
    {
        if (HasInputAuthority)
        {
            Head.SetPositionAndRotation(XROriginHelper.Current.Head.position, XROriginHelper.Current.Head.rotation);
        }
    }
}
