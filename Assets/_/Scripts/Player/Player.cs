using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(3)]
public class Player : NetworkBehaviour
{
    [SerializeField] private KCC kcc;

    public override void Render()
    {
        var positionTarget = kcc.transform.position - kcc.transform.rotation * InputHandler.Current.Head.localPosition.OnlyXZ();
        var rotationTarget = kcc.transform.rotation;

        transform.SetPositionAndRotation(positionTarget, rotationTarget);

        if (HasInputAuthority)
        {
            InputHandler.Current.transform.SetPositionAndRotation(positionTarget, rotationTarget);
        }
    }
}
