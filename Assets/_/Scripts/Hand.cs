using Fusion;
using UnityEngine;

public class Hand : NetworkBehaviour
{
    public Grabber Grabber;

    public void Grab()
    {
        var colliders = new Collider[1];
        if (Physics.OverlapSphereNonAlloc(transform.position, 0.1f, colliders, 1 << LayerMask.NameToLayer("Grabble")) > 0)
        {
            var grabble = colliders[0].GetComponentInChildren<Grabble>();
            Grabber.Grab(grabble);
        }
    }

    public void UnGrab()
    {
        Grabber.UnGrab();
    }
}
