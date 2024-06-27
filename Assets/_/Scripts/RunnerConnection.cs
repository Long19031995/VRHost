using Fusion;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerConnection : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private NetworkObject playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerObjects = new Dictionary<PlayerRef, NetworkObject>();
    private int countPlayer;

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
            Instantiate(inputHandler);
        }
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            Vector3 positionSpawn;
            Quaternion rotationSpawn;

            countPlayer++;
            var offset = .6f;
            if (countPlayer == 1)
            {
                positionSpawn = new Vector3(0, 0, -offset);
                rotationSpawn = new Quaternion(0, 0, 0, 0);
            }
            else
            {
                positionSpawn = new Vector3(0, 0, offset);
                rotationSpawn = new Quaternion(0, 180, 0, 0);
            }

            var player = runner.Spawn(playerPrefab, positionSpawn, rotationSpawn, playerRef);
            playerObjects.Add(playerRef, player);
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
        }
    }
}
