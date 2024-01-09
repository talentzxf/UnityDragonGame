using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarSpawner: SimulationBehaviour, IPlayerJoined
    {
        public Transform spawnPoint;
        public GameObject dragonAvatarPrefab;
        
        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                NetworkObject no = Runner.Spawn(dragonAvatarPrefab, spawnPoint.position, spawnPoint.rotation, player);
                var controller = no.GetComponent<PlayerMovementController>();
                EnemyManager.Instance.SetLocalPlayer(controller);
                
                Physics.SyncTransforms(); // Need to sync transforms, or character controller will always reset character position to 0,0,0;
            }
        }
    }
}