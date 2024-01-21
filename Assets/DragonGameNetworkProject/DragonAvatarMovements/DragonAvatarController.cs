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
        [SerializeField] public Texture readyImg;
        
        [Networked] public bool isReady { set; get; }
        
        [Networked] private Color bodyColor { set; get; }
        [Networked] private Color hairColor { set; get; }
        [Networked] private Color bellyColor { set; get; }
        
        [Networked] private bool isMasterController { set; get; }

        public static DragonAvatarController LocalController = null;

        public void SetBodyColor(Color bodyColor)
        {
            this.bodyColor = bodyColor;
        }

        public void SetHairColor(Color hairColor)
        {
            this.hairColor = hairColor;
        }

        public void SetBellyColor(Color bellyColor)
        {
            this.bellyColor = bellyColor;
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

            GameTimer.Instance.onGameCompleted.AddListener(() =>
            {
                SwitchTo<DragonAvatarStopMovement>();
            });
            
            NetworkEventsHandler.SelectedAsMasterClient.AddListener(() =>
            {
                isMasterController = true;
            });
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

        public RenderTexture TakeAvatarSnapshot()
        {
            int avatarLayerMask = LayerMask.NameToLayer("AvatarPreview");
            Utility.SetLayerRecursively(avatarGO, avatarLayerMask);

            // Create Camera
            var avatarSelectCameraGO = new GameObject("AvatarCamera_" + _no.InputAuthority.PlayerId);
            avatarSelectCameraGO.transform.parent = avatarGO.transform;
            Camera cameraComp = avatarSelectCameraGO.AddComponent<Camera>();

            GameObject light = new GameObject();
            Light lightComp = light.AddComponent<Light>();
            lightComp.type = LightType.Point;
            lightComp.intensity = 1.5f;
            lightComp.range = 30.0f;
            light.transform.parent = avatarSelectCameraGO.transform;
            light.transform.position = avatarSelectCameraGO.transform.position;
            light.transform.forward = avatarSelectCameraGO.transform.forward;

            var avatarTransform = avatarGO.transform;
            
            // camera.transform.position = avatarTransform.position + 5.0f * avatarTransform.forward + 2.0f * avatarTransform.up;
            avatarSelectCameraGO.transform.position = avatarTransform.position + new Vector3(1.23f, 3.18f, 5.06f);
            avatarSelectCameraGO.transform.LookAt(avatarTransform.position + avatarTransform.up);
            
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
            
            GamePlayState.Instance.onGamePreparing.AddListener(() =>
            {
                avatarSelectCameraGO.SetActive(false);
            });
            
            return rt;
        }

        private Image readyImage;
        public void SetupPrepareUI()
        {
            prepareUIEle = PrepareUI.Instance.SetupAvatarUI(Runner, _no.InputAuthority, TakeAvatarSnapshot());
            
            PrepareUI.Instance.RegisterDragonAvatarController(this);

            readyImage = Utility.CreateImageInAvatarView(readyImg);
            prepareUIEle.Add(readyImage);

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
                
                PrepareUI.Instance.onBellyColorPicked.AddListener((color) =>
                {
                    SetBellyColor(color);
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
                
                bodyColor = smr.materials[3].color;
                hairColor = smr.materials[2].color;
                bellyColor = smr.materials[4].color;
            }

            needPrepareUI = true;
        }
        
        private bool needPrepareUI = false;

        private void Update()
        {
            if (Runner == null || Runner.State != NetworkRunner.States.Running)
                return;
            
            if (needPrepareUI)
            {
                SetupPrepareUI();
                smr.materials[3].color = bodyColor;
                smr.materials[2].color = hairColor;
                smr.materials[4].color = bellyColor;
                needPrepareUI = false;
            }

            if (readyImage != null)
            {
                if (isReady || isMasterController)
                {
                    readyImage.visible = true;
                }
                else
                {
                    readyImage.visible = false;
                }
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
                    case nameof(bellyColor):
                        smr.materials[4].color = bellyColor;
                        break;
                }
            }
        }
    }
}