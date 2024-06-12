using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(Rigidbody))]
public class PlayerRB : MonoBehaviour
{
    private Rigidbody rb;
    private InputHandler input => InputHandler.Current;

    private Vector3 posOffset;
    private Quaternion rotOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        InputHandler.Current.transform.SetPositionAndRotation(transform.position, transform.rotation);

        posOffset += new Vector3(input.MoveDirection.x * Time.deltaTime, 0, input.MoveDirection.y * Time.deltaTime);
        rotOffset *= Quaternion.Euler(0, input.RotateDirection * Time.deltaTime, 0);
    }

    private void FixedUpdate()
    {
        rb.velocity = Extension.GetVelocity(transform.position, transform.position + posOffset, Time.fixedDeltaTime);
        rb.angularVelocity = Extension.GetAngularVelocity(transform.rotation, transform.rotation * rotOffset, Time.fixedDeltaTime);

        posOffset = default;
        rotOffset = default;
    }
}
