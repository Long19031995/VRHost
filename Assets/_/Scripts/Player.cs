using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public struct InputData : INetworkInput
{
    public Vector2 Direction;
    public float Yaw;

    public Vector3 HeadLocalPosition;
    public Quaternion HeadLocalRotation;
    public Vector3 LeftLocalPosition;
    public Quaternion LeftLocalRotation;
    public Vector3 RightLocalPosition;
    public Quaternion RightLocalRotation;

    public float LeftGrip;
    public float RightGrip;
}

[RequireComponent(typeof(KCC))]
[RequireComponent(typeof(RigDevice))]
public class Player : NetworkBehaviour
{
    [Networked] private InputData inputDataNetwork { get; set; }

    [SerializeField] private KCC kcc;
    [SerializeField] private float speedTranslation;
    [SerializeField] private float speedRotation;
    [SerializeField] private Transform headNetwork;
    [SerializeField] private HandNetwork leftHandNetwork;
    [SerializeField] private HandNetwork rightHandNetwork;

    private InputActions inputActions;
    private RigDevice rigDevice;

    public override void Spawned()
    {
        base.Spawned();

        if (HasInputAuthority)
        {
            var events = Runner.GetComponent<NetworkEvents>();
            events.OnInput.AddListener(OnInput);

            inputActions = new InputActions();
            inputActions.Enable();

            Camera.main.transform.SetParent(headNetwork);

            rigDevice = GetComponent<RigDevice>();
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        if (HasInputAuthority)
        {
            inputActions.Disable();
        }
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var inputData = new InputData();

        var move = inputActions.XRILeftHandLocomotion.Move.ReadValue<Vector2>();
        inputData.Direction = new Vector2(move.x, move.y);

        var rotate = inputActions.XRIRightHandLocomotion.Turn.ReadValue<Vector2>();
        inputData.Yaw = rotate.x;

        var leftGrip = inputActions.XRILeftHandInteraction.SelectValue.ReadValue<float>();
        inputData.LeftGrip = leftGrip;

        var rightGrip = inputActions.XRIRightHandInteraction.SelectValue.ReadValue<float>();
        inputData.RightGrip = rightGrip;

        rigDevice.SetLocalPose();

        inputData.HeadLocalPosition = rigDevice.HeadLocalPosition;
        inputData.HeadLocalRotation = rigDevice.HeadLocalRotation;

        inputData.LeftLocalPosition = rigDevice.LeftLocalPosition;
        inputData.LeftLocalRotation = rigDevice.LeftLocalRotation;

        inputData.RightLocalPosition = rigDevice.RightLocalPosition;
        inputData.RightLocalRotation = rigDevice.RightLocalRotation;

        input.Set(inputData);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out InputData inputData))
        {
            inputDataNetwork = inputData;

            leftHandNetwork.GripValue = inputData.LeftGrip;
            rightHandNetwork.GripValue = inputData.RightGrip;

            kcc.AddLookRotation(Runner.DeltaTime * speedRotation * new Vector2(0, inputData.Yaw));
            kcc.SetInputDirection(kcc.Data.TransformRotation * new Vector3(inputData.Direction.x, 0, inputData.Direction.y) * speedTranslation * Runner.DeltaTime);
        }
    }

    public override void Render()
    {
        base.Render();

        if (HasInputAuthority)
        {
            headNetwork.transform.SetLocalPositionAndRotation(rigDevice.HeadLocalPosition, rigDevice.HeadLocalRotation);
            leftHandNetwork.transform.SetLocalPositionAndRotation(rigDevice.LeftLocalPosition, rigDevice.LeftLocalRotation);
            rightHandNetwork.transform.SetLocalPositionAndRotation(rigDevice.RightLocalPosition, rigDevice.RightLocalRotation);
        }
        else
        {
            headNetwork.transform.SetLocalPositionAndRotation(inputDataNetwork.HeadLocalPosition, inputDataNetwork.HeadLocalRotation);
            leftHandNetwork.transform.SetLocalPositionAndRotation(inputDataNetwork.LeftLocalPosition, inputDataNetwork.LeftLocalRotation);
            rightHandNetwork.transform.SetLocalPositionAndRotation(inputDataNetwork.RightLocalPosition, inputDataNetwork.RightLocalRotation);
        }
    }
}
