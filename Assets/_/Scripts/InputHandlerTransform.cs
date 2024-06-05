using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputHandlerTransformType
{
    Head,
    LeftHand,
    RightHand
}

[Serializable]
public class InputHandlerTransformProperty
{
    public InputActionProperty Position;
    public InputActionProperty Rotation;
}

public class InputHandlerTransform : MonoBehaviour
{
    [SerializeField] private InputHandlerTransformType type;
    [SerializeField] private InputHandlerTransformProperty property = new InputHandlerTransformProperty();

    public InputHandlerTransformType Type { get; private set; }

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
        var bindingPosition = "";
        var bindingRotation = "";

        switch (type)
        {
            case InputHandlerTransformType.Head:
                bindingPosition = "<XRHMD>/centerEyePosition";
                bindingRotation = "<XRHMD>/centerEyeRotation";
                break;
            case InputHandlerTransformType.LeftHand:
                bindingPosition = "<XRController>{LeftHand}/devicePosition";
                bindingRotation = "<XRController>{LeftHand}/deviceRotation";
                break;
            case InputHandlerTransformType.RightHand:
                bindingPosition = "<XRController>{RightHand}/devicePosition";
                bindingRotation = "<XRController>{RightHand}/deviceRotation";
                break;
        }

        property.Position = new InputActionProperty(new InputAction("Position", binding: bindingPosition, expectedControlType: "Vector3"));
        property.Rotation = new InputActionProperty(new InputAction("Rotation", binding: bindingRotation, expectedControlType: "Quaternion"));
    }

    private void OnEnable()
    {
        property.Position.action.performed += OnPositionPerformed;
        property.Rotation.action.performed += OnRotationPerformed;

        property.Position.action.Enable();
        property.Rotation.action.Enable();
    }

    private void OnPositionPerformed(InputAction.CallbackContext context)
    {
        transform.position = context.ReadValue<Vector3>();
    }

    private void OnRotationPerformed(InputAction.CallbackContext context)
    {
        transform.rotation = context.ReadValue<Quaternion>();
    }

    private void OnDisable()
    {
        property.Position.action.Disable();
        property.Rotation.action.Disable();

        property.Position.action.performed -= OnPositionPerformed;
        property.Rotation.action.performed -= OnRotationPerformed;
    }
}
