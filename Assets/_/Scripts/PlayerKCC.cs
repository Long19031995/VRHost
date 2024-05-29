using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class PlayerKCC : NetworkBehaviour
{
    [SerializeField] private KCC kcc;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            kcc.AddLookRotation(new Vector2(0, inputData.RotateDirection) * Runner.DeltaTime * 100);
            kcc.SetInputDirection(inputData.HeadRotation * new Vector3(inputData.MoveDirection.x, 0, inputData.MoveDirection.y) * Runner.DeltaTime * 5);
        }
    }
}
