using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public struct InputData : INetworkInput
{
    public float Translation;
    public float Rotation;
}

[RequireComponent(typeof(KCC))]
public class Player : NetworkBehaviour
{
    [SerializeField] private KCC kcc;
    [SerializeField] private LeftHand leftHand;
    [SerializeField] private RightHand rightHand;
    [SerializeField] private float speedTranslation;
    [SerializeField] private float speedRotation;

    private InputData inputData;

    private void OnValidate()
    {
        if (kcc == null) kcc = GetComponent<KCC>();
        if (leftHand == null) leftHand = GetComponentInChildren<LeftHand>();
        if (rightHand == null) rightHand = GetComponentInChildren<RightHand>();
    }

    public override void Spawned()
    {
        base.Spawned();

        if (HasInputAuthority)
        {
            var events = Runner.GetComponent<NetworkEvents>();
            events.OnInput.AddListener(OnInput);
            inputData = new InputData();
        }
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        inputData.Translation += leftHand.Controller.translateAnchorAction.action.ReadValue<Vector2>().y;
        inputData.Rotation += rightHand.Controller.rotateAnchorAction.action.ReadValue<Vector2>().x;

        input.Set(inputData);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out InputData inputData))
        {
            kcc.AddLookRotation(new Vector2(0, inputData.Rotation * speedRotation * Runner.DeltaTime));
            kcc.SetInputDirection(kcc.Data.TransformRotation * new Vector3(0, 0, inputData.Translation * speedTranslation * Runner.DeltaTime));
        }

        this.inputData.Translation = 0;
        this.inputData.Rotation = 0;
    }
}
