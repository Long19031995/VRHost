using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class PlayerKCC : NetworkBehaviour
{
    [Networked] private InputData inputDataNetwork { get; set; }

    [SerializeField] private KCC kcc;

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
            var grabble = colliders[0].GetComponentInParent<Grabble>();
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
            LeftGrabInfo = InputHandler.Current.LeftHandGrip ? leftGrabber.Grab(FindGrabble(leftGrabber.Visual.position)) : default,

            RightHandPosition = InputHandler.Current.RightHandTarget.position,
            RightHandRotation = InputHandler.Current.RightHandTarget.rotation,
            RightGrabInfo = InputHandler.Current.RightHandGrip ? rightGrabber.Grab(FindGrabble(rightGrabber.Visual.position)) : default,

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

        kcc.SetInputDirection(inputDataNetwork.HeadRotation * new Vector3(inputDataNetwork.MoveDirection.x, 0, inputDataNetwork.MoveDirection.y));
        kcc.AddLookRotation(inputDataNetwork.RotateDirection);

        head.SetPositionAndRotation(inputDataNetwork.HeadPosition, inputDataNetwork.HeadRotation);
        leftGrabber.SetPositionAndRotationTarget(inputDataNetwork.LeftHandPosition, inputDataNetwork.LeftHandRotation);
        rightGrabber.SetPositionAndRotationTarget(inputDataNetwork.RightHandPosition, inputDataNetwork.RightHandRotation);
    }
}
