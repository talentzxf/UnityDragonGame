using Fusion;
using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarSpawner: SimulationBehaviour, IPlayerJoined
    {
        public Transform spawnPoint;
        public GameObject dragonAvatarPrefab;

        void SetLayerRecursively(GameObject obj, int layerMask)
        {
            obj.layer = layerMask;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layerMask);
            }
        }
        
        private void SetupPrepareUI(NetworkRunner runner, PlayerRef playerRef, GameObject avatar, DragonAvatarController controller)
        {
            int avatarLayerMask = LayerMask.NameToLayer("AvatarPreview");
            SetLayerRecursively(avatar, avatarLayerMask);

            // Create Camera
            GameObject camera = new GameObject("AvatarCamera_" + playerRef.PlayerId);
            Camera cameraComp = camera.AddComponent<Camera>();
            var avatarTransform = avatar.transform;
            camera.transform.position = avatarTransform.position + 5.0f * avatarTransform.forward + 2.0f * avatarTransform.up;
            camera.transform.LookAt(avatarTransform.position + avatarTransform.up);
            
            cameraComp.fieldOfView = 60f;
            cameraComp.clearFlags = CameraClearFlags.Color;
            cameraComp.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            cameraComp.cullingMask = 1 << avatarLayerMask;

            RenderTexture rt = new RenderTexture(512, 512, 24);
            rt.format = RenderTextureFormat.ARGB32;
            rt.depth = 24;
            rt.useMipMap = false;
            cameraComp.targetTexture = rt;
            PrepareUI.Instance.SetupAvatarUI(runner, playerRef, rt);
            PrepareUI.Instance.onBodyColorPicked.AddListener((color)=>
            {
                controller.SetBodyColor(color);
            });
                
            PrepareUI.Instance.onHairColorPicked.AddListener((color) =>
            {
                controller.SetHairColor(color);
            });
        }
        public void PlayerJoined(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                NetworkObject no = Runner.Spawn(dragonAvatarPrefab, spawnPoint.position, spawnPoint.rotation, player);
                var controller = no.GetComponent<DragonAvatarController>();
                // controller.SwitchTo<DragonAvatarGroundMovement>();
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

                SetupPrepareUI(Runner, no.InputAuthority, controller.avatarGO, controller);

                Physics.SyncTransforms(); // Need to sync transforms, or character controller will always reset character position to 0,0,0;
            }
        }
    }
}