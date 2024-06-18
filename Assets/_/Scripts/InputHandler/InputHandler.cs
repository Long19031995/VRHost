using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private InputPlatform platform;

    [Header("Target")]
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    [Header("Input")]
    [SerializeField] private InputHandlerTransform headInput;
    [SerializeField] private InputHandlerInteraction leftHandInput;
    [SerializeField] private InputHandlerInteraction rightHandInput;

    public Transform Head => headInput.transform;
    public Transform LeftHand => leftHandInput.transform;
    public Transform RightHand => rightHandInput.transform;
    public Transform HeadTarget => headTarget;
    public Transform LeftHandTarget => leftHandTarget;
    public Transform RightHandTarget => rightHandTarget;
    public bool LeftHandGrip => leftHandInput.GripValue == 1;
    public bool RightHandGrip => rightHandInput.GripValue == 1;
    public Vector2 MoveDirection => new Vector2(leftHandInput.RotateValue, leftHandInput.MoveValue);
    public float RotateDirection => rightHandInput.RotateValue;

    private void Awake()
    {
        switch (platform)
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
        platform = InputPlatform.VR;

        Camera.main.stereoTargetEye = StereoTargetEyeMask.Both;

        StartCoroutine(StartXR());
    }

    private void SwitchToPC()
    {
        platform = InputPlatform.PC;

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
    private int count;

    private void OnGUI()
    {
        if (xrControllerLeft == null)
        {
            xrControllerLeft = Instantiate(Resources.Load<GameObject>("XRControllerLeft"), leftHandInput.transform).transform;
            xrControllerLeft.gameObject.SetActive(false);
        }

        if (xrControllerRight == null)
        {
            xrControllerRight = Instantiate(Resources.Load<GameObject>("XRControllerRight"), rightHandInput.transform).transform;
            xrControllerRight.gameObject.SetActive(false);
        }

        if (isShowXRController)
        {
            if (GUILayout.Button("Hide Controller"))
            {
                HideController();
            }
        }
        else
        {
            if (GUILayout.Button("Show Controller"))
            {
                ShowController();
            }
        }

        if (platform == InputPlatform.PC)
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

        if (Keyboard.current.f1Key.wasReleasedThisFrame && count++ % 2 == 0)
        {
            if (isShowXRController) HideController();
            else ShowController();
        }

        if (Keyboard.current.f2Key.wasReleasedThisFrame && count++ % 2 == 0)
        {
            if (platform == InputPlatform.PC) SwitchToVR();
            else SwitchToPC();
        }
    }

    private void HideController()
    {
        xrControllerLeft.gameObject.SetActive(false);
        xrControllerRight.gameObject.SetActive(false);
        isShowXRController = false;
    }

    private void ShowController()
    {
        xrControllerLeft.gameObject.SetActive(true);
        xrControllerRight.gameObject.SetActive(true);
        isShowXRController = true;
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
