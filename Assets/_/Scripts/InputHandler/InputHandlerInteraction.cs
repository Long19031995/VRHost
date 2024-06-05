using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputHandlerInteractionType
{
    LeftHand,
    RightHand
}

[Serializable]
public class InputHandlerInteractionProperty
{
    public InputActionProperty Grip;
    public InputActionProperty Move;
    public InputActionProperty Rotate;
}

public class InputHandlerInteraction : MonoBehaviour
{
    [SerializeField] private InputHandlerInteractionType type;

    [SerializeField] private InputHandlerInteractionProperty property = new InputHandlerInteractionProperty();

    public InputHandlerInteractionType Type { get; private set; }

    public float GripValue { get; private set; }
    public float MoveValue { get; private set; }
    public float RotateValue { get; private set; }

    private void OnValidate()
    {
        if (Type != type)
        {
            Type = type;
            Reset();
        }
    }

    private void Reset()
    {
        var bindingGripValue = "";
        var bindingMoveValue = "";
        var bindingRotateValue = "";

        switch (type)
        {
            case InputHandlerInteractionType.LeftHand:
                bindingGripValue = "<XRController>{LeftHand}/{Grip}";
                bindingMoveValue = "<XRController>{LeftHand}/{Primary2DAxis}";
                bindingRotateValue = "<XRController>{LeftHand}/{Primary2DAxis}";
                break;
            case InputHandlerInteractionType.RightHand:
                bindingGripValue = "<XRController>{RightHand}/{Grip}";
                bindingMoveValue = "<XRController>{RightHand}/{Primary2DAxis}";
                bindingRotateValue = "<XRController>{RightHand}/{Primary2DAxis}";
                break;
        }

        property.Grip = new InputActionProperty(new InputAction("Grip", binding: bindingGripValue, expectedControlType: "Axis"));
        property.Move = new InputActionProperty(new InputAction("Move", binding: bindingMoveValue, processors: "scaleVector2(x=0,y=1)", expectedControlType: "Vector2"));
        property.Rotate = new InputActionProperty(new InputAction("Rotate", binding: bindingRotateValue, processors: "scaleVector2(x=1,y=0)", expectedControlType: "Vector2"));
    }

    private void OnEnable()
    {
        property.Grip.action.performed += OnGripPerformed;
        property.Move.action.performed += OnMovePerformed;
        property.Rotate.action.performed += OnRotatePerformed;

        property.Grip.action.Enable();
        property.Move.action.Enable();
        property.Rotate.action.Enable();
    }

    private void OnGripPerformed(InputAction.CallbackContext context)
    {
        GripValue = context.ReadValue<float>();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveValue = context.ReadValue<Vector2>().y;
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        RotateValue = context.ReadValue<Vector2>().x;
    }

    private void OnDisable()
    {
        property.Grip.action.Disable();
        property.Move.action.Disable();
        property.Rotate.action.Disable();

        property.Grip.action.performed -= OnGripPerformed;
        property.Move.action.performed -= OnMovePerformed;
        property.Rotate.action.performed -= OnMovePerformed;
    }
}
