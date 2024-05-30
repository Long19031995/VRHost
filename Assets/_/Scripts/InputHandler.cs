using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

public enum InputPlatform
{
    PC,
    VR
}

public class InputHandler : MonoBehaviour
{
    private static InputHandler current;
    public static InputHandler Current => current ??= FindObjectOfType<InputHandler>();

    public InputPlatform Platform;
    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;

    public InputActions InputActions;

    public Vector3 HeadPosition => InputActions.XRIHead.Position.ReadValue<Vector3>();
    public Quaternion HeadRotation => InputActions.XRIHead.Rotation.ReadValue<Quaternion>();

    public Vector3 LeftHandPosition => InputActions.XRILeftHand.Position.ReadValue<Vector3>();
    public Quaternion LeftHandRotation => InputActions.XRILeftHand.Rotation.ReadValue<Quaternion>();

    public Vector3 RightHandPosition => InputActions.XRIRightHand.Position.ReadValue<Vector3>();
    public Quaternion RightHandRotation => InputActions.XRIRightHand.Rotation.ReadValue<Quaternion>();

    public bool LeftHandGrip => InputActions.XRILeftHandInteraction.Select.ReadValue<float>() == 1;
    public bool RightHandGrip => InputActions.XRIRightHandInteraction.Select.ReadValue<float>() == 1;

    public Vector2 MoveDirection => new Vector2(InputActions.XRILeftHandInteraction.RotateAnchor.ReadValue<Vector2>().x, InputActions.XRILeftHandInteraction.TranslateAnchor.ReadValue<Vector2>().y);
    public float RotateDirection => InputActions.XRIRightHandInteraction.RotateAnchor.ReadValue<Vector2>().x;

    private void Awake()
    {
        InputActions = new InputActions();

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

    private void OnEnable()
    {
        InputActions.Enable();
    }

    private void OnDisable()
    {
        InputActions.Disable();
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

    private void Update()
    {
        if (Platform == InputPlatform.VR)
        {
            if (Head) Head.SetPositionAndRotation(HeadPosition, HeadRotation);
            if (LeftHand) LeftHand.SetPositionAndRotation(LeftHandPosition, LeftHandRotation);
            if (RightHand) RightHand.SetPositionAndRotation(RightHandPosition, RightHandRotation);
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

        Head.SetPositionAndRotation(new Vector3(0, 1.6f, 0), Quaternion.identity);
        LeftHand.SetPositionAndRotation(new Vector3(-0.2f, 1.5f, 0.5f), Quaternion.identity);
        RightHand.SetPositionAndRotation(new Vector3(0.2f, 1.5f, 0.5f), Quaternion.identity);

        Camera.main.stereoTargetEye = StereoTargetEyeMask.None;
        Camera.main.fieldOfView = 60;

        StopXR();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
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
