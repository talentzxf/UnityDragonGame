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
            NetworkObject no = Runner.Spawn(playerPrefab, null, null, player);
            var controller = no.GetComponent<PlayerMovementController>();
            controller.avatarGO.transform.position = spawnPoint.position;
            controller.avatarGO.transform.rotation = spawnPoint.rotation;
            controller.SwitchTo<PlayerGroundMovement>();

            Camera camera = Camera.main;
            FirstPersonCamera fpsCamera = camera.GetComponent<Camera>().GetComponent<FirstPersonCamera>();
            fpsCamera.SetCameraTarget(controller.avatarGO);

            if (Runner.IsSharedModeMasterClient)
            {
                // Spawn the dragons.
                NetworkObject dragonNo = Runner.Spawn(dragonPrefab, dragonSpawnPoint.position,
                    dragonSpawnPoint.rotation, player);
            }

            Physics.SyncTransforms(); // Need to sync transforms, or character controller will always reset character position to 0,0,0;
        }
    }
}