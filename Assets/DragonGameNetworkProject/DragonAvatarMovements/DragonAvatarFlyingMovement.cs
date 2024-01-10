using System;
using DragonGameNetworkProject.DragonMovements;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarFlyingMovement : AbstractCharacterMovement
    {
        [SerializeField] private float MaxSpeed = 15f; //max speed for basic movement
        [SerializeField] private float BoostSpeedAcc = 5.0f;
        
        private int isFlying = Animator.StringToHash("IsFlying");
        private float projectDistance = 10f;
        private Rigidbody rb;

        public float flyingGravityPortion = 0.1f;

        private InputHandler _inputHandler = new();
        
        private FirstPersonCamera fpsCamera;
        private Canvas canvas;
        private RectTransform canvasRect;
        private Transform Cam;
        private Camera CamComp;
        
        private float camSwitchRotationSpeed = 5.0f;

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
            }
        }

        public override void OnEnterMovement()
        {
            cc.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = false; // Control gravity by myself.

            UIController.Instance.ShowDragonControlUI();
            
            // Apply a init velocity.
            BoostSpeed();

            animator.SetBool(isFlying, true);
        }


        public override void OnLeaveMovement()
        {
            rb.isKinematic = false;
            cc.enabled = true; // Still use cc to control the character. 
            animator.SetBool(isFlying, false);
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                _inputHandler.Update();

                if (_inputHandler.IsRightMouseHold)
                {
                    Cam.transform.position = Vector3.Lerp(Cam.transform.position, 
                        ccTransform.position - ccTransform.forward * fpsCamera.distance, 10.0f * Time.deltaTime);
                    Cam.transform.LookAt(ccTransform);
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
                rb.velocity = MaxSpeed * ccTransform.forward;
            }
        }

        private void EasyControl()
        {
            if (Runner.IsForward)
            {
                float delta = Runner.DeltaTime;

                if (_inputHandler.Jump)
                {
                    BoostSpeed();
                }

                if (_inputHandler.IsRightMouseHold)
                {
                    fpsCamera.enabled = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    
                    Vector2 canvasDim = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
                    Vector2 canvasCenter = 0.5f * canvasDim;
                    
                    Vector2 inputMousePosition = canvasCenter + (_inputHandler.MousePosition - canvasCenter) * 0.15f;
                    Vector2 mousePos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                        inputMousePosition, canvas.worldCamera, out mousePos);

                    Vector3 dragonPosition = ccTransform.position;

                    Ray mousePointRay = CamComp.ScreenPointToRay(inputMousePosition);

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
                    
                    // Cam.transform.position = Vector3.Lerp(Cam.transform.position, 
                    //     dragonPosition + (dragonPosition - dragonTargetPoint).normalized * fpsCamera.distance, 10.0f * delta);
                    // Cam.transform.LookAt(ccTransform);
                    
                    UIController.Instance.ShowSpeed(rb.velocity, MaxSpeed);
                }
                else
                {
                    Cursor.visible = false;
                    fpsCamera.enabled = true;
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                try
                {
                    // Add a small amount of gravity.
                    Vector3 levitationForce = Physics.gravity * flyingGravityPortion;
                    rb.AddForce(levitationForce, ForceMode.Acceleration);

                    EasyControl();
                }
                finally
                {
                    _inputHandler.Reset();
                }
            }
        }
    }
}