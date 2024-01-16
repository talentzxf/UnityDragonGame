using System;
using Fusion;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarController : CharacterMovementController
    {
        [Networked] public bool isReady { set; get; }
        
        [Networked] private Color bodyColor { set; get; }
        [Networked] private Color hairColor { set; get; }

        public static DragonAvatarController LocalController = null;

        public void SetBodyColor(Color bodyColor)
        {
            this.bodyColor = bodyColor;
        }

        public void SetHairColor(Color hairColor)
        {
            this.hairColor = hairColor;
        }

        private SkinnedMeshRenderer smr;
        private void Awake()
        {
            smr = avatarGO.GetComponentInChildren<SkinnedMeshRenderer>();
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                if (collider is not CharacterController)
                {
                    collider.gameObject.tag = "Player"; // Setup tags.
                    collider.isTrigger = true;
                }
            }
        }

        private ChangeDetector _changeDetector;
        private NetworkObject _no;
        private VisualElement prepareUIEle;

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            if (prepareUIEle != null)
            {
                prepareUIEle.visible = false;
                prepareUIEle.style.visibility = Visibility.Hidden;
                prepareUIEle.parent.Remove(prepareUIEle);
                prepareUIEle = null;
            }
        }

        public void SetupPrepareUI()
        {
            int avatarLayerMask = LayerMask.NameToLayer("AvatarPreview");
            Utility.SetLayerRecursively(avatarGO, avatarLayerMask);

            // Create Camera
            GameObject camera = new GameObject("AvatarCamera_" + _no.InputAuthority.PlayerId);
            Camera cameraComp = camera.AddComponent<Camera>();
            var avatarTransform = avatarGO.transform;
            camera.transform.position = avatarTransform.position + 5.0f * avatarTransform.forward + 2.0f * avatarTransform.up;
            camera.transform.LookAt(avatarTransform.position + avatarTransform.up);
            
            cameraComp.fieldOfView = 60f;
            cameraComp.clearFlags = CameraClearFlags.Color;
            cameraComp.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            cameraComp.cullingMask = 1 << avatarLayerMask;

            RenderTexture rt = new RenderTexture(512, 512, 24)
            {
                format = RenderTextureFormat.ARGB32,
                depth = 24,
                useMipMap = false
            };
            cameraComp.targetTexture = rt;
            prepareUIEle = PrepareUI.Instance.SetupAvatarUI(Runner, _no.InputAuthority, rt);
            
            PrepareUI.Instance.RegisterDragonAvatarController(this);

            if (HasInputAuthority)
            {
                PrepareUI.Instance.onBodyColorPicked.AddListener((color)=>
                {
                    SetBodyColor(color);
                });
                
                PrepareUI.Instance.onHairColorPicked.AddListener((color) =>
                {
                    SetHairColor(color);
                });                
            }
        }

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _no = GetComponent<NetworkObject>();

            if (_no.HasInputAuthority)
            {
                LocalController = this;
            }

            needPrepareUI = true;
        }
        
        private bool needPrepareUI = false;

        private void Update()
        {
            if (needPrepareUI)
            {
                SetupPrepareUI();
                smr.materials[3].color = bodyColor;
                smr.materials[2].color = hairColor;
                needPrepareUI = false;
            }
            
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(bodyColor):
                        smr.materials[3].color = bodyColor;
                        break;
                    case nameof(hairColor):
                        smr.materials[2].color = hairColor;
                        break;
                }
            }
        }
    }
}