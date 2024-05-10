using Fusion;
using UnityEngine;

public struct GrabInfo : INetworkStruct
{
    public NetworkBehaviourId GrabberId;
    public NetworkBehaviourId GrabbleId;
    public Vector3 PositionOffset;
    public Quaternion RotationOffset;
}

public class Grabber : NetworkBehaviour
{
    private Grabble grabble;

    public override void Spawned()
    {
        base.Spawned();

        Runner.SetIsSimulated(Object, true);
    }

    public void Grab(Grabble grabble)
    {
        this.grabble = grabble;

        var offsetPosition = transform.InverseTransformPoint(grabble.transform.position);
        var offsetRotation = Quaternion.Inverse(grabble.transform.rotation) * transform.rotation;

        grabble.Grab(this, offsetPosition, offsetRotation);
    }

    public void Ungrab()
    {
        if (grabble)
        {
            grabble.Ungrab();
            grabble = null;
        }
    }
}
