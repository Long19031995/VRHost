using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hand : NetworkBehaviour
{
    public ActionBasedController Controller;

    [Networked] public bool IsGrabbed { get; set; }

    [SerializeField] private LayerMask layerMask;

    private Collider[] colliders = new Collider[1];

    private Grabble grabble;

    private void OnValidate()
    {
        if (Controller == null) Controller = GetComponentInParent<ActionBasedController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (HasInputAuthority)
        {
            var grip = Controller.selectActionValue.action.ReadValue<float>();
            IsGrabbed = grip == 1;
        }

        if (IsGrabbed && grabble == null)
        {
            Runner.GetPhysicsScene().OverlapSphere(transform.position, 1, colliders, layerMask, QueryTriggerInteraction.Ignore);
            if (colliders[0] != null)
            {
                if (colliders[0].TryGetComponent(out grabble) && !grabble.isGrabbed)
                {
                    grabble.SetTarget(this);
                }
            }
        }
        else if (grabble != null)
        {
            grabble.RemoveTarget(this);
            grabble = null;
        }
    }
}
