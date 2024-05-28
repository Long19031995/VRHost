using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHelper : MonoBehaviour
{
    private static XRHelper current;
    public static XRHelper Current => current ??= FindObjectOfType<XRHelper>();

    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;

    public ActionBasedController LeftHandController;
    public ActionBasedController RightHandController;

    public bool LeftHandGrip => LeftHandController.selectAction.action.ReadValue<float>() == 1;
    public bool RightHandGrip => RightHandController.selectAction.action.ReadValue<float>() == 1;

    public Vector2 MoveDirection => new Vector2(LeftHandController.rotateAnchorAction.action.ReadValue<Vector2>().x, LeftHandController.translateAnchorAction.action.ReadValue<Vector2>().y);
    public float RotateDirection => RightHandController.rotateAnchorAction.action.ReadValue<Vector2>().x;

    private void Awake()
    {
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
    }

    private void OnDestroy()
    {
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }

#if !UNITY_EDITOR
    private bool isFocused;

    private void Update()
    {
        if (isFocused != Application.isFocused)
        {
            isFocused = Application.isFocused;

            if (isFocused)
            {
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
            else
            {
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }
    }
#endif
}
