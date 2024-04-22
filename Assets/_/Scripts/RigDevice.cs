using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RigDevice : MonoBehaviour
{
    public Vector3 HeadLocalPosition;
    public Quaternion HeadLocalRotation;
    public Vector3 LeftLocalPosition;
    public Quaternion LeftLocalRotation;
    public Vector3 RightLocalPosition;
    public Quaternion RightLocalRotation;

    public void SetLocalPose()
    {
        SetLocalPoseFromDevice(InputDeviceCharacteristics.HeadMounted, out HeadLocalPosition, out HeadLocalRotation);
        SetLocalPoseFromDevice(InputDeviceCharacteristics.Left, out LeftLocalPosition, out LeftLocalRotation);
        SetLocalPoseFromDevice(InputDeviceCharacteristics.Right, out RightLocalPosition, out RightLocalRotation);
    }

    public void SetLocalPoseFromDevice(InputDeviceCharacteristics characteristics, out Vector3 localPosition, out Quaternion localRotation)
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
        if (devices.Count > 0)
        {
            devices[0].TryGetFeatureValue(CommonUsages.devicePosition, out localPosition);
            devices[0].TryGetFeatureValue(CommonUsages.deviceRotation, out localRotation);
        }
        else
        {
            localPosition = default;
            localRotation = default;
        }
    }
}
