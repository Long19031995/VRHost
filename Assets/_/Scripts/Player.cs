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
}

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
            HeadPosition = XRHelper.Current.Head.position,
            HeadRotation = XRHelper.Current.Head.rotation,

            LeftHandPosition = XRHelper.Current.LeftHand.position,
            LeftHandRotation = XRHelper.Current.LeftHand.rotation,
            LeftGrabInfo = XRHelper.Current.LeftHandGrip ? LeftGrabber.Grab(FindGrabble(LeftGrabber.transform.position)) : default,

            RightHandPosition = XRHelper.Current.RightHand.position,
            RightHandRotation = XRHelper.Current.RightHand.rotation,
            RightGrabInfo = XRHelper.Current.RightHandGrip ? RightGrabber.Grab(FindGrabble(RightGrabber.transform.position)) : default,
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

    public override void Render()
    {
        if (HasInputAuthority)
        {
            Head.SetPositionAndRotation(XRHelper.Current.Head.position, XRHelper.Current.Head.rotation);
        }
    }
}
