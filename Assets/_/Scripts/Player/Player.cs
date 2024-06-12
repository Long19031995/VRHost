using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

[DefaultExecutionOrder(3)]
public class Player : NetworkBehaviour
{
    [SerializeField] private KCC kcc;

    public override void Render()
    {
        if (HasInputAuthority)
        {
            InputHandler.Current.transform.position = transform.position = kcc.transform.position;
            InputHandler.Current.transform.rotation = transform.rotation = kcc.transform.rotation;
        }
    }
}
