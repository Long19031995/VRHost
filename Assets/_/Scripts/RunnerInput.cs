using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerInput : MonoBehaviour
{
    private void Awake()
    {
        var events = GetComponent<NetworkEvents>();
        events.OnInput.AddListener(OnInput);
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var inputData = new InputData();

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z;
        inputData.MousePosition = mousePosition;
        inputData.Buttons.Set(ButtonNetwork.LeftMouse, Input.GetMouseButton(0));
        inputData.Far = Camera.main.farClipPlane;

        input.Set(inputData);
    }
}

public struct InputData : INetworkInput
{
    public Vector3 MousePosition;
    public NetworkButtons Buttons;
    public float Far;
}

public enum ButtonNetwork
{
    LeftMouse = 0
}
