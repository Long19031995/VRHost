using Fusion;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class GrabberRender : NetworkBehaviour
{
    private Grabber grabber;
    private Grabble grabble => grabber.Grabble;
    private float t = 0;
    private Transform handTarget;
    private bool isMoving;

    [Networked] private InputData inputDataNetwork { get; set; }

    public override void Spawned()
    {
        grabber = GetComponent<Grabber>();

        handTarget = new GameObject("Hand Target").transform;
        handTarget.SetParent(transform);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData inputData))
        {
            inputDataNetwork = inputData;
        }
    }

    public override void Render()
    {
        if (HasInputAuthority) HandleRenderLocal();
        else if (IsProxy) HandleRenderProxy();
    }

    private void HandleRenderLocal()
    {
        handTarget = grabber.Side == GrabberSide.Left ? InputHandler.Current.LeftHandTarget : InputHandler.Current.RightHandTarget;
        isMoving = InputHandler.Current.IsMoving;

        HandleRender();
    }

    private void HandleRenderProxy()
    {
        var handPosition = grabber.Side == GrabberSide.Left ? inputDataNetwork.LeftHandPosition : inputDataNetwork.RightHandPosition;
        var handRotation = grabber.Side == GrabberSide.Left ? inputDataNetwork.LeftHandRotation : inputDataNetwork.RightHandRotation;
        handTarget.SetPositionAndRotation(handPosition, handRotation);

        isMoving = inputDataNetwork.MoveDirection != Vector2.zero || inputDataNetwork.RotateDirection != Vector2.zero;

        HandleRender();
    }

    private void HandleRender()
    {
        Vector3 position;
        Quaternion rotation;
        t = Mathf.MoveTowards(t, isMoving ? 1 : 0, (isMoving ? 20 : 2) * Time.deltaTime);

        if (grabble == null)
        {
            position = Vector3.Lerp(grabber.transform.position, handTarget.position, t);
            rotation = Quaternion.Lerp(grabber.transform.rotation, handTarget.rotation, t);
            grabber.Visual.SetPositionAndRotation(position, rotation);
        }
        else
        {
            var pose = Extension.GetPoseTarget(handTarget, grabber.GrabInfo.PositionOffset, grabber.GrabInfo.RotationOffset);
            position = Vector3.Lerp(grabble.transform.position, pose.Position, t);
            rotation = Quaternion.Lerp(grabble.transform.rotation, pose.Rotation, t);
            grabble.Visual.SetPositionAndRotation(position, rotation);

            position = grabble.Visual.position + grabber.Visual.position - grabber.Visual.TransformPoint(grabber.GrabInfo.PositionOffset);
            rotation = grabble.Visual.rotation * grabber.GrabInfo.RotationOffset;
            grabber.Visual.SetPositionAndRotation(position, rotation);
        }
    }
}
