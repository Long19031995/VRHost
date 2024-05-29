using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

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
        StartCoroutine(StartXR());
    }

    private void OnDestroy()
    {
        StopXR();
    }

    IEnumerator StartXR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
        }
    }

    void StopXR()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
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
                StartCoroutine(StartXR());
            }
            else
            {
                StopXR();
            }
        }
    }
#endif
}
