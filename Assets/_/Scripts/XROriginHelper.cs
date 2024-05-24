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

    private void Awake()
    {
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
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
