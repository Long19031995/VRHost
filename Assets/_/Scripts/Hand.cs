using Fusion;
using UnityEngine;

public class HandNetwork : NetworkBehaviour
{
    [Networked] public float GripValue { get; set; }

    private Collider[] colliders = new Collider[1];
    private Grabble grabble;

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GripValue == 1)
        {
            if (grabble == null)
            {
                Runner.GetPhysicsScene().OverlapSphere(transform.position, 1, colliders, 1 << LayerMask.NameToLayer("Grabble"), QueryTriggerInteraction.Ignore);
                if (colliders[0] != null)
                {
                    if (colliders[0].TryGetComponent(out grabble))
                    {
                        if (grabble.isGrabbed)
                        {
                            grabble = null;
                        }
                        else
                        {
                            grabble.SetTarget(this);
                        }
                    }
                }
            }
        }
        else
        {
            if (grabble != null)
            {
                grabble.RemoveTarget(this);
                grabble = null;
            }
        }
    }
}
