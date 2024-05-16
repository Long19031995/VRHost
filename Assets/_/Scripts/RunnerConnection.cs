using Fusion;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerConnection : MonoBehaviour
{
    [SerializeField] private XROriginHelper xrOriginHelper;
    [SerializeField] private NetworkObject grabberPrefab;
    [SerializeField] private NetworkObject grabblePrefab;

    private Dictionary<PlayerRef, NetworkObject> playerObjects = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        var events = GetComponent<NetworkEvents>();
        events.PlayerJoined.AddListener(OnPlayerJoined);
        events.PlayerLeft.AddListener(OnPlayerLeft);
        events.OnSceneLoadDone.AddListener(OnSceneLoadDone);
    }

    private void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.IsServer)
        {
            runner.Spawn(grabblePrefab);
        }

        if (runner.IsPlayer)
        {
            var position = new Vector3(Random.Range(0, 0), 0, Random.Range(0, 0));
            var rotation = Quaternion.identity;
            Instantiate(xrOriginHelper, position, rotation);
        }
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            var obj = runner.Spawn(grabberPrefab, inputAuthority: playerRef);
            playerObjects.Add(playerRef, obj);
        }
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            if (playerObjects.TryGetValue(playerRef, out var obj))
            {
                runner.Despawn(obj);
            }
        }
    }
}
