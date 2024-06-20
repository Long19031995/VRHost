using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;
using UnityEngine.XR;

[DefaultExecutionOrder(3)]
public class PlayerKCCOffset : NetworkBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform kcc;

    private Vector3 offset;
    private bool isFirstVR;

    public override void Render()
    {
        if (HasInputAuthority)
        {
            var positionTarget = kcc.transform.position - offset;
            var rotationTarget = kcc.transform.rotation;

            player.SetPositionAndRotation(positionTarget, rotationTarget);
            InputHandler.Current.transform.SetPositionAndRotation(positionTarget, rotationTarget);

            if (XRSettings.isDeviceActive && !isFirstVR && InputHandler.Current.HeadVelocity.sqrMagnitude > 0)
            {
                isFirstVR = true;
                offset = InputHandler.Current.HeadTarget.position.OnlyXZ() - kcc.transform.position.OnlyXZ();
            }
        }

    }
}
