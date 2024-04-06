using Fusion;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
public class RunnerHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    Dictionary<PlayerRef, Player> players = new Dictionary<PlayerRef, Player>();

    private void OnValidate()
    {
        if (runner == null) runner = GetComponent<NetworkRunner>();
    }

    private void Awake()
    {
        var events = runner.GetComponent<NetworkEvents>();
        events.PlayerJoined.AddListener(OnPlayerJoined);
        events.PlayerLeft.AddListener(OnPlayerLeft);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            var player = Resources.Load<Player>("Player");
            runner.Spawn(player, inputAuthority: playerRef);
            players.Add(playerRef, player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            if (players.TryGetValue(playerRef, out Player player))
            {
                runner.Despawn(player.GetComponent<NetworkObject>());
            }
        }
    }
}
