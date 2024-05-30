using UnityEngine;

[DefaultExecutionOrder(0)]
[RequireComponent(typeof(CharacterController))]
public class PlayerCC : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;

    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        cc.Move(new Vector3(InputHandler.Current.MoveDirection.x, 0, InputHandler.Current.MoveDirection.y) * Time.fixedDeltaTime * moveSpeed);
    }

    private void Update()
    {
        InputHandler.Current.transform.position = transform.position;
    }
}
