using System;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public Transform spawnPoint;
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject no = Runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, player);
            Physics.SyncTransforms(); // Need to sync transforms, or character controller will always reset character position to 0,0,0; 
        }
    }
}
