using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(CharacterController))]
public class PlayerCC : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private CharacterController cc;
    private InputHandler input => InputHandler.Current;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        var rotateDirection = input.RotateDirection * Vector3.up;
        transform.Rotate(rotateSpeed * Time.fixedDeltaTime * rotateDirection);

        var moveDirection = input.HeadTarget.rotation * new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y);
        moveDirection = moveDirection.magnitude > 0 ? moveDirection.OnlyXZ().normalized : Vector3.zero;
        cc.Move(moveSpeed * Time.fixedDeltaTime * moveDirection);
    }

    private void Update()
    {
        input.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
