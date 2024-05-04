using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public struct PoseDevice : INetworkStruct
{
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;
}

public class RigDevice : MonoBehaviour
{
    public Transform HeadTransform;
    public Transform LeftHandTransform;
    public Transform RightHandTransform;

    public PoseDevice PoseHead;
    public PoseDevice PoseLeftHand;
    public PoseDevice PoseRightHand;

    private void Update()
    {
        GetLocalPose();
        SetLocalPose();
    }

    public void GetLocalPose()
    {
        GetLocalPoseFromDevice(InputDeviceCharacteristics.HeadMounted, out PoseHead.LocalPosition, out PoseHead.LocalRotation);
        GetLocalPoseFromDevice(InputDeviceCharacteristics.Left, out PoseLeftHand.LocalPosition, out PoseLeftHand.LocalRotation);
        GetLocalPoseFromDevice(InputDeviceCharacteristics.Right, out PoseRightHand.LocalPosition, out PoseRightHand.LocalRotation);
    }

    private void GetLocalPoseFromDevice(InputDeviceCharacteristics characteristics, out Vector3 localPosition, out Quaternion localRotation)
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

    public void SetLocalPose()
    {
        if (HeadTransform != null)
        {
            HeadTransform.SetLocalPositionAndRotation(PoseHead.LocalPosition, PoseHead.LocalRotation);
        }

        if (LeftHandTransform != null)
        {
            LeftHandTransform.SetLocalPositionAndRotation(PoseLeftHand.LocalPosition, PoseLeftHand.LocalRotation);
        }

        if (RightHandTransform != null)
        {
            RightHandTransform.SetLocalPositionAndRotation(PoseRightHand.LocalPosition, PoseRightHand.LocalRotation);
        }
    }
}
