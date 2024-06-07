using System.Collections;
using UnityEngine;

public enum FollowerType
{
    Instant,
    Velocity
}

[DefaultExecutionOrder(1)]
public class Follower : MonoBehaviour
{
    [SerializeField] private FollowerType type;
    [SerializeField] private Transform target;

    private Rigidbody rb;

    private void OnValidate()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(OnValidateCoroutine());
        }
    }

    private IEnumerator OnValidateCoroutine()
    {
        if (type == FollowerType.Instant)
        {
            if (gameObject.TryGetComponent(out Rigidbody rb))
            {
                yield return new WaitForEndOfFrame();
                DestroyImmediate(rb);
            }
        }
        else
        {
            if (!gameObject.TryGetComponent(out Rigidbody rb))
            {
                yield return new WaitForEndOfFrame();
                gameObject.AddComponent<Rigidbody>();
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (type == FollowerType.Velocity)
        {
            rb.SetVelocity(transform, target, Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        if (type == FollowerType.Instant)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }
}
