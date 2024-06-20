using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;
using UnityEngine.XR;

[DefaultExecutionOrder(3)]
public class PlayerKCCOffset : NetworkBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform kcc;

    private bool isFirstVR;

    public override void Render()
    {
        var positionTarget = kcc.transform.position;
        var rotationTarget = kcc.transform.rotation;
        player.SetPositionAndRotation(positionTarget, rotationTarget);

        if (HasInputAuthority)
        {
            InputHandler.Current.transform.SetPositionAndRotation(positionTarget, rotationTarget);

            if (!isFirstVR && XRSettings.isDeviceActive && InputHandler.Current.HeadVelocity.sqrMagnitude > 0)
            {
                var positionOffset = kcc.transform.position.OnlyXZ() - InputHandler.Current.HeadTarget.position.OnlyXZ();
                InputHandler.Current.SetPositionOffset(positionOffset);

                isFirstVR = true;
            }
        }

    }
}
