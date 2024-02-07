using UnityEngine;

public class FlyingController : MonoBehaviour
{
    private Rigidbody rb;

    public float initSpeed = 0.0f;

    private Canvas canvas;
    private Transform Cam;
    private Camera CamComp;
    private RectTransform canvasRect;
    
    private float projectDistance = 10f;
    private float camSwitchRotationSpeed = 10.0f;
    private Transform lookAt;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * initSpeed;
        
        var canvasGO = GameObject.Find("Canvas");
        canvas = canvasGO.GetComponent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        Cam = Camera.main.transform;
        CamComp = Camera.main;

        lookAt = GameObject.Find("lookAt").transform;
    }
    
    private bool IsRightMouseHold = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity += transform.forward;
        }
        
        if (Input.GetMouseButtonDown(1)) // 1 -- Right Mouse Button
        {
            IsRightMouseHold = true;
        }


        if (Input.GetMouseButtonUp(1))
        {
            IsRightMouseHold = false;
        }

        if (IsRightMouseHold)
        {
            float delta = Time.deltaTime;
            Cursor.lockState = CursorLockMode.Confined;


            Vector2 canvasDim = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
            Vector2 canvasCenter = 0.5f * canvasDim;
                
            Vector2 inputMousePosition = canvasCenter + (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - canvasCenter) * 0.1f;
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                inputMousePosition, canvas.worldCamera, out mousePos);
            Ray mousePointRay = CamComp.ScreenPointToRay(inputMousePosition);
        
            float cameraToProjectPlaneDistance =
                (transform.position - Cam.transform.position).magnitude + projectDistance;
        
            Vector3 projectedPoint =
                mousePointRay.origin + mousePointRay.direction * cameraToProjectPlaneDistance;

            lookAt.position = projectedPoint;
        
            Quaternion targetRotation = Quaternion.LookRotation(lookAt.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                camSwitchRotationSpeed * delta);
            
            lookAt.rotation = Quaternion.LookRotation(lookAt.position - transform.position);
        }
    }
}
