using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkPhysics : NetworkBehaviour
{
    [SerializeField] private bool isHold;
    [SerializeField] private Collider collider;
    [SerializeField] private Rigidbody rigidbody;

    private void OnValidate()
    {
        if (!collider) collider = GetComponent<Collider>();
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
    }

    public override void Spawned()
    {
        base.Spawned();

        Runner.SetIsSimulated(Object, true);

        if (!Object.HasInputAuthority)
        {
            Object.RenderTimeframe = RenderTimeframe.Remote;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out InputData inputData))
        {
            if (inputData.Buttons.IsSet(ButtonNetwork.LeftMouse))
            {
                if (!isHold) isHold = GetIsHold(inputData.MousePosition, Vector3.forward);
            }
            else
            {
                isHold = false;
            }

            if (isHold)
            {
                rigidbody.VelocityFollow(inputData.MousePosition, Quaternion.identity, Runner.DeltaTime);
            }
        }
    }

    private bool GetIsHold(Vector3 origin, Vector3 direction)
    {
        int deep = 1000;
        Runner.GetPhysicsScene().Raycast(new Vector3(origin.x, origin.y, -deep / 2), direction * deep, out RaycastHit hitInfo);
        return hitInfo.collider == collider;
    }

    public override void Render()
    {
        base.Render();


    }
}
