using Fusion;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerConnection : MonoBehaviour
{
    [SerializeField] private XROriginHelper xrOriginHelper;
    [SerializeField] private NetworkObject playerPrefab;

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
        if (runner.IsPlayer)
        {
            var random = 0;
            var position = new Vector3(Random.Range(-random, random), 0, Random.Range(random, random));
            var rotation = Quaternion.identity;
            Instantiate(xrOriginHelper, position, rotation);
        }
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            var obj = runner.Spawn(playerPrefab, inputAuthority: playerRef);
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
