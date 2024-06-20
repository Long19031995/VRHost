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
    public InputActionProperty Velocity;
}

public class InputHandlerTransform : MonoBehaviour
{
    [SerializeField] private InputHandlerTransformType type;
    [SerializeField] private InputHandlerTransformProperty property = new InputHandlerTransformProperty();

    public InputHandlerTransformType Type { get; private set; }

    public Vector3 Velocity { get; private set; }

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
        var bindingVelocity = "";

        switch (type)
        {
            case InputHandlerTransformType.Head:
                bindingPosition = "<XRHMD>/centerEyePosition";
                bindingRotation = "<XRHMD>/centerEyeRotation";
                bindingVelocity = "<XRHMD>/{DeviceVelocity}";
                break;
            case InputHandlerTransformType.LeftHand:
                bindingPosition = "<XRController>{LeftHand}/devicePosition";
                bindingRotation = "<XRController>{LeftHand}/deviceRotation";
                bindingVelocity = "<XRController>{LeftHand}/{DeviceVelocity}";
                break;
            case InputHandlerTransformType.RightHand:
                bindingPosition = "<XRController>{RightHand}/devicePosition";
                bindingRotation = "<XRController>{RightHand}/deviceRotation";
                bindingVelocity = "<XRController>{RightHand}/{DeviceVelocity}";
                break;
        }

        property.Position = new InputActionProperty(new InputAction("Position", binding: bindingPosition, expectedControlType: "Vector3"));
        property.Rotation = new InputActionProperty(new InputAction("Rotation", binding: bindingRotation, expectedControlType: "Quaternion"));
        property.Velocity = new InputActionProperty(new InputAction("Velocity", binding: bindingVelocity, expectedControlType: "Vector3"));
    }

    private void OnEnable()
    {
        property.Position.action.performed += OnPositionPerformed;
        property.Rotation.action.performed += OnRotationPerformed;
        property.Velocity.action.performed += OnVelocityPerformed;

        property.Position.action.Enable();
        property.Rotation.action.Enable();
        property.Velocity.action.Enable();
    }

    private void OnPositionPerformed(InputAction.CallbackContext context)
    {
        transform.localPosition = context.ReadValue<Vector3>();
    }

    private void OnRotationPerformed(InputAction.CallbackContext context)
    {
        transform.localRotation = context.ReadValue<Quaternion>();
    }

    private void OnVelocityPerformed(InputAction.CallbackContext context)
    {
        Velocity = context.ReadValue<Vector3>();
    }

    private void OnDisable()
    {
        property.Position.action.Disable();
        property.Rotation.action.Disable();
        property.Velocity.action.Disable();

        property.Position.action.performed -= OnPositionPerformed;
        property.Rotation.action.performed -= OnRotationPerformed;
        property.Velocity.action.performed -= OnVelocityPerformed;
    }
}
