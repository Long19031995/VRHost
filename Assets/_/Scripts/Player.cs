using Fusion;
using UnityEngine;

public struct InputData : INetworkInput
{
    public Vector3 HeadPosition;
    public Quaternion HeadRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;

    public GrabInfo GrabInfo;
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
        if (Physics.OverlapSphereNonAlloc(point, 10, colliders, 1 << LayerMask.NameToLayer("Grabble")) > 0)
        {
            var grabble = colliders[0].GetComponentInChildren<Grabble>();
            return grabble;
        }

        return null;
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        GrabInfo grabInfo = default;

        if (XROriginHelper.Current.LeftHandGrip) grabInfo = LeftGrabber.Grab(FindGrabble(LeftGrabber.transform.position));
        if (XROriginHelper.Current.RightHandGrip) grabInfo = RightGrabber.Grab(FindGrabble(RightGrabber.transform.position));

        input.Set(new InputData()
        {
            HeadPosition = XROriginHelper.Current.Head.position,
            HeadRotation = XROriginHelper.Current.Head.rotation,

            LeftHandPosition = XROriginHelper.Current.LeftHand.position,
            LeftHandRotation = XROriginHelper.Current.LeftHand.rotation,

            RightHandPosition = XROriginHelper.Current.RightHand.position,
            RightHandRotation = XROriginHelper.Current.RightHand.rotation,

            GrabInfo = grabInfo,
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
            Head.SetPositionAndRotation(XROriginHelper.Current.Head.position, XROriginHelper.Current.Head.rotation);
        }
    }
}
