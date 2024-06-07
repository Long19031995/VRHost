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
        input.transform.SetPositionAndRotation(transform.position, transform.rotation);

        var rotateDirection = input.RotateDirection * transform.up;
        transform.Rotate(rotateSpeed * Time.fixedDeltaTime * rotateDirection);

        var moveDirection = input.HeadTarget.rotation * new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y);
        cc.Move(moveSpeed * Time.fixedDeltaTime * moveDirection.OnlyXZ().normalized);
    }
}
