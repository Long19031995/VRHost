using Fusion;
using UnityEngine;

public class Hand : NetworkBehaviour
{
    [Networked] private Vector3 mousePosition { get; set; }

    [SerializeField] private Grabber grabber;

    private bool isGrabbed { get; set; }
    private Camera cameraMain;

    private void Awake()
    {
        cameraMain = Camera.main;
    }

    public override void Spawned()
    {
        base.Spawned();

        Runner.SetIsSimulated(Object, true);
        Runner.GetComponent<NetworkEvents>().OnInput.AddListener(OnInput);
    }

    private void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var mousePosition = cameraMain.ScreenToWorldPoint(Input.mousePosition);
        var mousePressed = Input.GetMouseButton(0);

        input.Set(new InputData()
        {
            MousePosition = mousePosition,
            MousePressed = mousePressed
        });
    }

    private RaycastHit GetHit(Vector3 mousePosition)
    {
        var origin = new Vector3(mousePosition.x, mousePosition.y, cameraMain.transform.position.z);
        var direction = Vector3.forward * cameraMain.farClipPlane;
        Physics.Raycast(origin, direction, out RaycastHit hitInfo, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Grabble"));

        return hitInfo;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out InputData inputData))
        {
            if (inputData.MousePressed)
            {
                if (!isGrabbed)
                {
                    var hitInfo = GetHit(mousePosition);
                    if (hitInfo.collider)
                    {
                        var grabble = hitInfo.collider.GetComponentInChildren<Grabble>();
                        if (grabble)
                        {
                            isGrabbed = true;
                            grabber.Grab(grabble);
                        }
                    }
                }
            }
            else
            {
                isGrabbed = false;
                grabber.Ungrab();
            }
            mousePosition = new Vector3(inputData.MousePosition.x, inputData.MousePosition.y, 0);
        }

        transform.position = mousePosition;
    }
}

public struct InputData : INetworkInput
{
    public Vector3 MousePosition;
    public NetworkBool MousePressed;
}
