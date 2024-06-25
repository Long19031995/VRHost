using Fusion;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class GrabberRenderHandler : NetworkBehaviour
{
    private Grabber grabber;
    private Grabble grabble => grabber.Grabble;
    private float t = 0;

    public override void Spawned()
    {
        grabber = GetComponent<Grabber>();
    }

    public override void Render()
    {
        if (HasInputAuthority)
        {
            Vector3 position;
            Quaternion rotation;
            Transform handTarget = grabber.Side == GrabberSide.Left ? InputHandler.Current.LeftHandTarget : InputHandler.Current.RightHandTarget;
            t = Mathf.MoveTowards(t, InputHandler.Current.IsMoving ? 1 : 0, (InputHandler.Current.IsMoving ? 10 : 3) * Time.deltaTime);

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
}
