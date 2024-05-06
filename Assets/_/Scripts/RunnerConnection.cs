using Fusion;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerConnection : MonoBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerObjects = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        var events = GetComponent<NetworkEvents>();
        events.PlayerJoined.AddListener(OnPlayerJoined);
        events.PlayerLeft.AddListener(OnPlayerLeft);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            var playerInstance = runner.Spawn(playerPrefab, inputAuthority: playerRef);
            playerObjects.Add(playerRef, playerInstance);
        }
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            if (playerObjects.TryGetValue(playerRef, out var playerInstance))
            {
                runner.Despawn(playerInstance);
            }
        }
    }
}
