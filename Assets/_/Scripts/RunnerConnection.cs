using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkEvents))]
public class RunnerConnection : MonoBehaviour
{
    [SerializeField] private NetworkEvents events;
    [SerializeField] private NetworkObject[] objectsAssignInput;
    [SerializeField] private NetworkObject[] objectsSpawn;

    private void OnValidate()
    {
        if (!events) events = GetComponent<NetworkEvents>();
    }

    private void Awake()
    {
        events.PlayerJoined.AddListener(OnPlayerJoined);
        events.PlayerLeft.AddListener(OnPlayerLeft);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            foreach (var obj in objectsAssignInput)
            {
                obj.AssignInputAuthority(playerRef);
            }

            foreach (var obj in objectsSpawn)
            {
                var objInstance = runner.Spawn(obj, inputAuthority: playerRef);
                runner.SetPlayerObject(playerRef, objInstance);
            }
        }
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            var obj = runner.GetPlayerObject(playerRef);
            runner.Despawn(obj);
        }
    }
}
