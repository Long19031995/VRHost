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

    private string handSide => type == InputHandlerInteractionType.LeftHand ? "LeftHand" : "RightHand";

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
        property.Grip = new InputActionProperty(GetGripAction());
        property.Move = new InputActionProperty(GetMoveAction());
        property.Rotate = new InputActionProperty(GetRotateAction());
    }

    private InputAction GetGripAction()
    {
        var gripAction = new InputAction("Grip", type: InputActionType.Value, expectedControlType: "Axis");
        gripAction.AddBinding($"<XRController>{{{handSide}}}/{{Grip}}");
        return gripAction;
    }

    private InputAction GetMoveAction()
    {
        var moveAction = new InputAction("Move", type: InputActionType.Value, processors: "scaleVector2(x=0,y=1)", expectedControlType: "Vector2");
        moveAction.AddBinding($"<XRController>{{{handSide}}}/{{Primary2DAxis}}");
        moveAction.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s").With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        return moveAction;
    }

    private InputAction GetRotateAction()
    {
        var rotateAction = new InputAction("Rotate", type: InputActionType.Value, processors: "scaleVector2(x=1,y=0)", expectedControlType: "Vector2");
        rotateAction.AddBinding($"<XRController>{{{handSide}}}/{{Primary2DAxis}}");
        rotateAction.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s").With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
        return rotateAction;
    }

    private void OnEnable()
    {
        property.Grip.action.performed += OnGripPerformed;
        property.Grip.action.canceled += OnGripCanceled;

        property.Move.action.performed += OnMovePerformed;
        property.Move.action.canceled += OnMoveCanceled;

        property.Rotate.action.performed += OnRotatePerformed;
        property.Rotate.action.canceled += OnRotateCanceled;

        property.Grip.action.Enable();
        property.Move.action.Enable();
        property.Rotate.action.Enable();
    }

    private void OnGripPerformed(InputAction.CallbackContext context)
    {
        GripValue = context.ReadValue<float>();
    }

    private void OnGripCanceled(InputAction.CallbackContext context)
    {
        GripValue = 0;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveValue = context.ReadValue<Vector2>().y;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveValue = 0;
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        RotateValue = context.ReadValue<Vector2>().x;
    }

    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        RotateValue = 0;
    }

    private void OnDisable()
    {
        property.Grip.action.Disable();
        property.Move.action.Disable();
        property.Rotate.action.Disable();

        property.Grip.action.performed -= OnGripPerformed;
        property.Grip.action.canceled -= OnGripCanceled;

        property.Move.action.performed -= OnMovePerformed;
        property.Move.action.canceled -= OnMoveCanceled;

        property.Rotate.action.performed -= OnRotatePerformed;
        property.Rotate.action.canceled -= OnRotateCanceled;
    }
}
