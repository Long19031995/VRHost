using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public Rigidbody Cube;
    public RigDevice RigDevice;
    public InputActions InputActions;
    public bool isFollow;

    private void Awake()
    {
        InputActions = new InputActions();
    }

    private void OnEnable()
    {
        InputActions.Enable();
    }

    private void OnDisable()
    {
        InputActions.Disable();
    }

    private void Update()
    {
        isFollow = Keyboard.current.bKey.isPressed;
    }

    private void FixedUpdate()
    {
        if (isFollow)
        {
            Cube.VelocityFollow(RigDevice.LeftHandTransform, Vector3.zero, Quaternion.identity, Time.fixedDeltaTime);
        }
    }
}
