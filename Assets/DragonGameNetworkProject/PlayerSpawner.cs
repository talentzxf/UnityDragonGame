using DragonGameNetworkProject;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public Transform spawnPoint;
    public GameObject playerPrefab;

    public Transform dragonSpawnPoint;
    public GameObject dragonPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject no = Runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, player);
            no.gameObject.GetComponent<CharacterMovementController>().SwitchTo<PlayerGroundMovement>();

            if (Runner.IsSharedModeMasterClient)
            { // Spawn the dragons.
                NetworkObject dragonNo = Runner.Spawn(dragonPrefab, dragonSpawnPoint.position, dragonSpawnPoint.rotation, player);
            }
            
            Physics.SyncTransforms(); // Need to sync transforms, or character controller will always reset character position to 0,0,0;
        }
    }
}