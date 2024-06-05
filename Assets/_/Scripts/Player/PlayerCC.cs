using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(CharacterController))]
public class PlayerCC : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        cc.Move((InputHandler.Current.HeadTarget.rotation * transform.rotation * new Vector3(InputHandler.Current.MoveDirection.x, 0, InputHandler.Current.MoveDirection.y) * Time.fixedDeltaTime).OnlyXZ().normalized * moveSpeed);

        transform.Rotate(InputHandler.Current.RotateDirection * Vector3.up * Time.fixedDeltaTime * rotateSpeed);
    }

    private void Update()
    {
        InputHandler.Current.transform.position = Vector3.MoveTowards(InputHandler.Current.transform.position, transform.position, Time.deltaTime * moveSpeed);
        InputHandler.Current.transform.rotation = Quaternion.RotateTowards(InputHandler.Current.transform.rotation, transform.rotation, Time.deltaTime * rotateSpeed);
    }
}
