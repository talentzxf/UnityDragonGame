using System;
using System.Runtime.InteropServices;
using DragonGameNetworkProject.DragonMovements;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    class DragonAvatarCollideReactor : MonoBehaviour
    {
        public UnityEvent<Collision> collideAction = new();

        private void OnCollisionEnter(Collision collision)
        {
            collideAction?.Invoke(collision);
        }
    }

    public class DragonAvatarFlyingMovement : AbstractCharacterMovement
    {
        [SerializeField] private float MaxSpeed = 30f; //max speed for basic movement
        [SerializeField] private float BoostSpeedAcc = 5.0f;
        [SerializeField] private AudioClip flapAC;

        private int isFlying = Animator.StringToHash("IsFlying");
        private int isDashing = Animator.StringToHash("IsDashing");
        private int flyingSpeed = Animator.StringToHash("FlyingSpeed");
        private int dashProgress = Animator.StringToHash("DashProgress");

        private float projectDistance = 10f;
        private Rigidbody rb;

        public float flyingGravityPortion = 0.01f;

        private InputHandler _inputHandler = new();

        private FirstPersonCamera fpsCamera;
        private Canvas canvas;
        private RectTransform canvasRect;
        private Transform Cam;
        private Camera CamComp;

        private float camSwitchRotationSpeed = 5.0f;

        private AudioSource _avatarAudioSource;
        
        private Image frontSightImg;
        private RectTransform frontSightRT;

        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                rb = ccTransform.GetComponent<Rigidbody>();

                Cam = Camera.main.transform;
                CamComp = Cam.GetComponent<Camera>();
                fpsCamera = Cam.GetComponent<FirstPersonCamera>();

                var canvasGO = GameObject.Find("Canvas");
                canvas = canvasGO.GetComponent<Canvas>();
                canvasRect = canvasGO.GetComponent<RectTransform>();

                _avatarAudioSource = cc.gameObject.GetComponent<AudioSource>();
                
                frontSightImg = canvas.transform.Find("FrontSight").GetComponent<Image>();
                frontSightRT = frontSightImg.GetComponent<RectTransform>();
            }
        }

        public override void OnEnterMovement()
        {
            if (HasStateAuthority)
            {
                cc.enabled = false;
                rb.isKinematic = false;
                rb.useGravity = false; // Control gravity by myself.

                UIController.Instance.ShowDragonControlUI();
                UIController.Instance.ShowPrompt(); //"Hold mouse right button to control");

                // Apply an init velocity.
                BoostSpeed();
            }

            animator.SetBool(isFlying, true);

            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                collider.gameObject.tag = "Player";
                if (collider is not CharacterController)
                {
                    collider.isTrigger = false;
                }

                if (collider.gameObject.GetComponent<DragonAvatarCollideReactor>() == null)
                {
                    var collideReactor = collider.gameObject.AddComponent<DragonAvatarCollideReactor>();

                    collideReactor.collideAction.AddListener(OnCollisionEnter);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (HasStateAuthority)
            {
                if (collision.gameObject.CompareTag("Terrain"))
                {
                    Debug.Log("Collided with terrain, begin to land!");
                    controller.SwitchTo<DragonAvatarLandMovement>();
                }
            }
        }

        public override void OnLeaveMovement()
        {
            if (HasStateAuthority)
            {
                rb.isKinematic = false;
                cc.enabled = true; // Still use cc to control the character. 
                animator.SetBool(isFlying, false);

                Cursor.visible = false;
                fpsCamera.enabled = true;
                
                frontSightImg.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                _inputHandler.Update();

                if (!_inputHandler.IsRightMouseHold)
                {
                    Vector3 targetPosition = fpsCamera.target.position;
                    
                    Cam.transform.position = Vector3.Lerp(Cam.transform.position,
                        targetPosition - ccTransform.forward * fpsCamera.distance, 10.0f * Time.deltaTime);
                    Cam.transform.LookAt(targetPosition);
                }
            }
        }

        private void BoostSpeed()
        {
            float curMag = rb.velocity.magnitude;
            if (curMag < Mathf.Epsilon)
            {
                rb.velocity = ccTransform.forward * BoostSpeedAcc;
            }

            rb.velocity += ccTransform.forward * BoostSpeedAcc;
            if (curMag > MaxSpeed) // Clamp
            {
                Vector3 maxSpeed = MaxSpeed * ccTransform.forward;
                maxSpeed.y = rb.velocity.y; // Keep y velocity.

                rb.velocity = maxSpeed;
            }
        }

        private void EasyControl()
        {
            // if (Runner.IsForward)
            {
                float delta = Runner.DeltaTime;

                if (_inputHandler.Jump)
                {
                    if (animator.GetFloat(dashProgress) > 0.0f)
                    {
                        UIController.Instance.ShowGameMsg("Can't dash while dashing");
                    }
                    else
                    {
                        BoostSpeed();

                        animator.SetBool(isDashing, true);

                        _avatarAudioSource.loop = false;
                        _avatarAudioSource.clip = flapAC;
                        _avatarAudioSource.Play();
                    }
                }
                else
                {
                    if (animator.GetFloat(dashProgress) > 0.0f)
                    {
                        animator.SetBool(isDashing, false);
                    }
                }

                if (!_inputHandler.IsRightMouseHold)
                {
                    fpsCamera.enabled = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    // Cursor.visible = true;
                    frontSightImg.gameObject.SetActive(true);

                    Vector2 canvasDim = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
                    Vector2 canvasCenter = 0.5f * canvasDim;

                    Vector2 inputMousePosition = canvasCenter + (_inputHandler.MousePosition - canvasCenter) * 0.7f;
                    Vector2 mousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                        inputMousePosition, canvas.worldCamera, out mousePos);
                    Ray mousePointRay = CamComp.ScreenPointToRay(inputMousePosition);
                    frontSightRT.anchoredPosition = inputMousePosition;

                    Vector3 dragonPosition = fpsCamera.target.position;
                    float cameraToProjectPlaneDistance =
                        (dragonPosition - Cam.transform.position).magnitude + projectDistance;

                    Vector3 projectedPoint =
                        mousePointRay.origin + mousePointRay.direction * cameraToProjectPlaneDistance;
                    Vector3 dragonTargetPoint = projectedPoint;

                    Quaternion targetRotation = Quaternion.LookRotation(dragonTargetPoint - dragonPosition);
                    ccTransform.rotation = Quaternion.Slerp(ccTransform.rotation, targetRotation,
                        camSwitchRotationSpeed * delta);

                    Vector3 curVelocity = rb.velocity;
                    float curVelocityMag = curVelocity.magnitude;
                    
                    rb.velocity = ccTransform.forward * curVelocityMag;
                    // rb.MoveRotation(targetRotation);
                }
                else
                {
                    Cursor.visible = false;
                    fpsCamera.enabled = true;
                    frontSightImg.gameObject.SetActive(false);
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                try
                {
                    if (controller.currentMovement != this) // I'm not the current movement.
                    {
                        return;
                    }
                    
                    if (_inputHandler.Land)
                    {
                        controller.SwitchTo<DragonAvatarLandMovement>();
                        return;
                    }
                    
                    EasyControl();
                    
                    Vector3 xzVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
                    //
                    // float speedPortion = xzVelocity.magnitude;
                    // float levitationForce = speedPortion * speedPortion;
                    // float maxLevitationForce = MaxSpeed * MaxSpeed;
                    //
                    // float resultLevitationForcePortion = levitationForce / maxLevitationForce;
                    //
                    // float gravityPortion = 0.1f * Math.Max(flyingGravityPortion, 1.0f - resultLevitationForcePortion);
                    //
                    // Vector3 remainGravityForce = Physics.gravity * gravityPortion;
                    //
                    // rb.AddForce(remainGravityForce, ForceMode.Acceleration);
                    //
                    // Debug.Log("Gravity is:" + remainGravityForce);

                    animator.SetFloat(flyingSpeed, xzVelocity.magnitude / MaxSpeed);

                    UIController.Instance.ShowSpeed(rb.velocity, MaxSpeed);
                }
                finally
                {
                    _inputHandler.Reset();
                }
            }
        }
    }
}