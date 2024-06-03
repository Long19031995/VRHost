using UnityEngine;

[DefaultExecutionOrder(1)]
[RequireComponent(typeof(Rigidbody))]
public class Hand : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.SetVelocity(transform, target, Time.fixedDeltaTime);
    }
}
