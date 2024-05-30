using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(KCC))]
public class PlayerKCC : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotateSpeed = 100;

    private KCC kcc;

    public override void Spawned()
    {
        kcc = GetComponent<KCC>();
        kcc.SetManualUpdate(true);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            kcc.SetInputDirection(kcc.Data.TransformRotation * new Vector3(inputData.MoveDirection.x, 0, inputData.MoveDirection.y) * Runner.DeltaTime * moveSpeed);
            kcc.AddLookRotation(new Vector2(0, inputData.RotateDirection) * Runner.DeltaTime * rotateSpeed);
        }

        kcc.ManualFixedUpdate();
    }

    public override void Render()
    {
        kcc.ManualRenderUpdate();

        if (HasInputAuthority)
        {
            InputHandler.Current.transform.position = transform.position;
        }
    }
}
