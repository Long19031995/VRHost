using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(3)]
public class Player : NetworkBehaviour
{
    [SerializeField] private KCC kcc;

    public override void Render()
    {
        transform.SetPositionAndRotation(kcc.transform.position, kcc.transform.rotation);
        if (HasInputAuthority)
        {
            InputHandler.Current.transform.SetPositionAndRotation(kcc.transform.position, kcc.transform.rotation);
        }
    }
}
