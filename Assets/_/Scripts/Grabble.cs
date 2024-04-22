using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabble : NetworkBehaviour
{
    public bool isGrabbed => handCurrent != null;

    [SerializeField] private HandNetwork handCurrent;
    [SerializeField] private Rigidbody rb;

    private Vector3 offsetPosition;
    private Quaternion offsetRotation;

    private void OnValidate()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Grabble");
    }

    public void SetTarget(HandNetwork hand)
    {
        handCurrent = hand;
        offsetPosition = hand.transform.InverseTransformPoint(transform.position);
        offsetRotation = Quaternion.Inverse(transform.rotation) * transform.rotation;
        rb.isKinematic = true;
    }

    public void RemoveTarget(HandNetwork hand)
    {
        if (hand == handCurrent)
        {
            handCurrent = null;
            rb.isKinematic = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (handCurrent != null)
        {
            transform.position = handCurrent.transform.TransformPoint(offsetPosition);
            transform.rotation = handCurrent.transform.rotation * offsetRotation;
        }
    }
}
