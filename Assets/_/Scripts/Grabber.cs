using Fusion;
using UnityEngine;

public class Grabber : NetworkBehaviour
{
    [Networked] private Vector3 mousePosition { get; set; }

    [SerializeField] private LayerMask layerMask;

    private Grabble grabble;

    public override void Spawned()
    {
        base.Spawned();

        Runner.SetIsSimulated(Object, true);
        if (!Object.HasInputAuthority) Object.RenderTimeframe = RenderTimeframe.Remote;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out InputData inputData))
        {
            var mousePosition = inputData.MousePosition;
            mousePosition.z = 0;
            this.mousePosition = mousePosition;

            if (inputData.Buttons.IsSet(ButtonNetwork.LeftMouse))
            {
                if (!grabble)
                    grabble = GetGrabble(inputData.MousePosition, Vector3.forward, inputData.Far);
            }
            else
            {
                grabble = null;
            }
        }

        transform.position = mousePosition;
    }

    private Grabble GetGrabble(Vector3 origin, Vector3 direction, float far)
    {
        Physics.Raycast(origin, direction * far, out RaycastHit hitInfo, float.PositiveInfinity, layerMask);

        if (hitInfo.collider)
            return hitInfo.collider.GetComponentInChildren<Grabble>();

        return null;
    }
}
