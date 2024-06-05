using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public enum InputPlatform
{
    PC,
    VR
}

[DefaultExecutionOrder(-1)]
public class InputHandler : MonoBehaviour
{
    private static InputHandler current;
    public static InputHandler Current => current ??= FindObjectOfType<InputHandler>();

    public InputPlatform Platform;

    [Header("Target")]
    public Transform HeadTarget;
    public Transform LeftHandTarget;
    public Transform RightHandTarget;

    [Header("Input")]
    [SerializeField] private InputHandlerInteraction LeftHandInput;
    [SerializeField] private InputHandlerInteraction RightHandInput;

    public bool LeftHandGrip => LeftHandInput.GripValue == 1;
    public bool RightHandGrip => RightHandInput.GripValue == 1;

    public Vector2 MoveDirection => new Vector2(LeftHandInput.RotateValue, LeftHandInput.MoveValue);
    public float RotateDirection => RightHandInput.RotateValue;

    private void Awake()
    {
        switch (Platform)
        {
            case InputPlatform.PC:
                SwitchToPC();
                break;
            case InputPlatform.VR:
                SwitchToVR();
                break;
        }
    }

    private void SwitchToVR()
    {
        Platform = InputPlatform.VR;

        Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;

        StartCoroutine(StartXR());
    }

    private void SwitchToPC()
    {
        Platform = InputPlatform.PC;

        Camera.main.stereoTargetEye = StereoTargetEyeMask.None;
        Camera.main.fieldOfView = 60;

        HeadTarget.parent.SetLocalPositionAndRotation(new Vector3(0, 1.6f, 0), Quaternion.identity);
        LeftHandTarget.parent.SetLocalPositionAndRotation(new Vector3(-0.2f, 1.5f, 0.5f), Quaternion.identity);
        RightHandTarget.parent.SetLocalPositionAndRotation(new Vector3(0.2f, 1.5f, 0.5f), Quaternion.identity);

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

    private void OnDestroy()
    {
        StopXR();
    }

#if UNITY_EDITOR
    private Transform xrControllerLeft;
    private Transform xrControllerRight;
    private bool isShowXRController;

    private void OnGUI()
    {
        if (xrControllerLeft == null)
        {
            xrControllerLeft = Instantiate(Resources.Load<GameObject>("XRControllerLeft"), LeftHandInput.transform).transform;
            xrControllerLeft.gameObject.SetActive(false);
        }

        if (xrControllerRight == null)
        {
            xrControllerRight = Instantiate(Resources.Load<GameObject>("XRControllerRight"), RightHandInput.transform).transform;
            xrControllerRight.gameObject.SetActive(false);
        }

        if (isShowXRController)
        {
            if (GUILayout.Button("Hide Controller"))
            {
                xrControllerLeft.gameObject.SetActive(false);
                xrControllerRight.gameObject.SetActive(false);
                isShowXRController = false;
            }
        }
        else
        {
            if (GUILayout.Button("Show Controller"))
            {
                xrControllerLeft.gameObject.SetActive(true);
                xrControllerRight.gameObject.SetActive(true);
                isShowXRController = true;
            }
        }

        if (Platform == InputPlatform.PC)
        {
            if (GUILayout.Button("Switch to VR"))
            {
                SwitchToVR();
            }
        }
        else
        {
            if (GUILayout.Button("Switch to PC"))
            {
                SwitchToPC();
            }
        }
    }
#endif

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
