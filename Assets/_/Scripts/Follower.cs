using Fusion;
using UnityEngine;

public class Follower : NetworkBehaviour
{
    private Transform target;
    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    public void Follow(Transform target)
    {
        this.target = target;
        positionOffset = target.InverseTransformPoint(transform.position);
        rotationOffset = Quaternion.Inverse(transform.rotation) * target.rotation;
    }

    public void UnFollow()
    {
        target = null;
        positionOffset = default;
        rotationOffset = default;
    }

    public override void Render()
    {
        if (target != null)
        {
            var position = target.TransformPoint(positionOffset);
            var rotation = Quaternion.Inverse(rotationOffset) * target.rotation;
            transform.SetPositionAndRotation(position, rotation);
        }
    }
}
