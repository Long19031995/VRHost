using Fusion;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerConnection : MonoBehaviour
{
    [SerializeField] private XRHelper xrHelper;
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private NetworkObject playerKCCPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerObjects = new Dictionary<PlayerRef, NetworkObject>();
    private Dictionary<PlayerRef, NetworkObject> playerKCCObjects = new Dictionary<PlayerRef, NetworkObject>();

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
            Instantiate(xrHelper, position, rotation);
        }
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            var player = runner.Spawn(playerPrefab, inputAuthority: playerRef);
            playerObjects.Add(playerRef, player);

            var playerKCC = runner.Spawn(playerKCCPrefab, inputAuthority: playerRef);
            playerKCCObjects.Add(playerRef, playerKCC);

            player.GetComponent<Player>().PlayerKCC = playerKCC.GetComponent<PlayerKCC>();
        }
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            if (playerObjects.TryGetValue(playerRef, out var player))
            {
                runner.Despawn(player);
            }

            if (playerKCCObjects.TryGetValue(playerRef, out var playerKCC))
            {
                runner.Despawn(playerKCC);
            }
        }
    }
}
