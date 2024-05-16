using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROriginHelper : MonoBehaviour
{
    private static XROriginHelper current;
    public static XROriginHelper Current => current ??= FindObjectOfType<XROriginHelper>();

    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;

    public ActionBasedController LeftHandController;
    public ActionBasedController RightHandController;

    public bool LeftHandGrip => LeftHandController.selectAction.action.ReadValue<float>() == 1;
    public bool RightHandGrip => RightHandController.selectAction.action.ReadValue<float>() == 1;
}
