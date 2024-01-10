using Fusion;
using Unity.VisualScripting;
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
                var controller = no.GetComponent<DragonAvatarController>();
                controller.SwitchTo<DragonAvatarGroundMovement>();
                EnemyManager.Instance.SetLocalPlayer(controller);
                
                Camera camera = Camera.main;
                FirstPersonCamera fpsCamera = camera.GetComponent<FirstPersonCamera>();

                Transform neckBone = Utility.RecursiveFind(no.transform, "Neck");
                if (neckBone != null)
                {
                    fpsCamera.SetCameraTargetByTransform(neckBone);
                }
                else
                {
                    fpsCamera.SetCameraTarget(controller.avatarGO);
                }
                
                Physics.SyncTransforms(); // Need to sync transforms, or character controller will always reset character position to 0,0,0;
            }
        }
    }
}