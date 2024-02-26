using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UnityStandardAssets.CrossPlatformInput
{
    public class MoveCamera : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        protected string cameraAxisX = "Mouse X";
        [SerializeField]
        protected string cameraAxisY = "Mouse Y";
        [SerializeField]
        protected float _sensitivityX = 1f;
        [SerializeField]
        protected float _sensitivityY = 1f;
        [SerializeField]
        protected RectTransform touchPointer;
        CrossPlatformInputManager.VirtualAxis m_CameraX_VirtualAxis; // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_CameraY_VirtualAxis; // Reference to the joystick in the cross platform input

        public Vector2 touchDirection { get; protected set; }
        public Vector2 previoustouchPosition { get; protected set; }
        public int currentPointerID { get; protected set; }
        public bool isPressed { get; protected set; }

        public bool touchPosToMousePos;
        public float Sensitivity_Y { get; set; }
        public float Sensitivity_X { get; set; }

        void Start()
        {
            CreateVirtualAxes();
            if (touchPointer)
            {
                touchPointer.gameObject.SetActive(false);
            }

            Sensitivity_X = 1;
            Sensitivity_Y = 1;
        }

        void CreateVirtualAxes()
        {
            // create new axes based on axes to use
            if (CrossPlatformInputManager.AxisExists(cameraAxisX))
            {
                m_CameraX_VirtualAxis = CrossPlatformInputManager.VirtualAxisReference(cameraAxisX);
            }
            else
            {
                m_CameraX_VirtualAxis = new CrossPlatformInputManager.VirtualAxis(cameraAxisX);
                CrossPlatformInputManager.RegisterVirtualAxis(m_CameraX_VirtualAxis);
            }
            if (CrossPlatformInputManager.AxisExists(cameraAxisY))
            {
                m_CameraY_VirtualAxis = CrossPlatformInputManager.VirtualAxisReference(cameraAxisY);
            }
            else
            {
                m_CameraY_VirtualAxis = new CrossPlatformInputManager.VirtualAxis(cameraAxisY);
                CrossPlatformInputManager.RegisterVirtualAxis(m_CameraY_VirtualAxis);
            }
        }

        void Update()
        {
            HandleTouchDirection();
        }

        void HandleTouchDirection()
        {
            if (isPressed)
            {
                vMousePositionHandler.Instance.clampScreen = !touchPosToMousePos;
                if (currentPointerID >= 0 && currentPointerID < Input.touches.Length)
                {                    
                    Vector2 touchPos = Input.touches[currentPointerID].position;
                    
                    if (touchPosToMousePos)
                    {
                        vMousePositionHandler.Instance.joystickMousePos = touchPos;
                    }                        

                    touchDirection = touchPos - previoustouchPosition;
                    if (touchPointer)
                    {
                        touchPointer.position = touchPos;
                    }

                    previoustouchPosition = Input.touches[currentPointerID].position;
                }
                else
                {

                    Vector2 touchPos = Input.mousePosition;// new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                    if (touchPosToMousePos)
                        vMousePositionHandler.Instance.joystickMousePos = touchPos;

                    touchDirection = touchPos - previoustouchPosition;
                    if (touchPointer)
                    {
                        touchPointer.position = touchPos;
                    }

                    previoustouchPosition = Input.mousePosition;
                }
                UpdateVirtualAxes(new Vector3(touchDirection.x * _sensitivityX * Sensitivity_X, touchDirection.y * _sensitivityY * Sensitivity_Y, 0));
            }
        }

        void UpdateVirtualAxes(Vector3 value)
        {

            m_CameraX_VirtualAxis.Update(value.x);
            m_CameraY_VirtualAxis.Update(value.y);
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (touchPointer)
            {
                touchPointer.position = data.position;
                touchPointer.gameObject.SetActive(true);
            }
            isPressed = true;
            currentPointerID = data.pointerId;
            previoustouchPosition = data.position;
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (touchPointer)
            {
                touchPointer.position = data.position;
                touchPointer.gameObject.SetActive(false);
            }
            isPressed = false;
            touchDirection = Vector2.zero;
            UpdateVirtualAxes(Vector3.zero);
        }
    }
}