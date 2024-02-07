using UnityEngine;
using UnityEngine.UI;

public class SimpleControlTest : MonoBehaviour
{
    private Camera Cam;
    private Image frontSightImg;
    private RectTransform frontSightRT;
    private FirstPersonCamera fpsCamera;
    private Canvas canvas;

    private float cameraDistance = 0.0f;

    private float rotationSpeed = 10.0f;

    private float distance = 10f;

    private GameObject dragonTargetGO;

    private Rigidbody rb;

    void Start()
    {
        Cam = Camera.main.GetComponent<Camera>();
        fpsCamera = Cam.GetComponent<FirstPersonCamera>();
        fpsCamera.SetCameraTarget(gameObject);

        GameObject canvasGO = GameObject.Find("Canvas");
        canvas = canvasGO.GetComponent<Canvas>();

        frontSightImg = canvas.transform.Find("FrontSight").GetComponent<Image>();
        frontSightRT = frontSightImg.GetComponent<RectTransform>();
        
        dragonTargetGO = GameObject.CreatePrimitive(PrimitiveType.Cube);

        rb = GetComponent<Rigidbody>();
        
        rb.velocity = transform.forward * 2.0f;

        cameraDistance = (Cam.transform.position - transform.position).magnitude;
    }

    private bool isRightMouseDown = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseDown = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float curMag = rb.velocity.magnitude;
            
            rb.velocity = curMag * 1.5f * rb.velocity.normalized;
        }

        if (isRightMouseDown)
        {
            fpsCamera.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            frontSightImg.gameObject.SetActive(true);

            Vector2 inputMousePosition = Input.mousePosition;
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                inputMousePosition, canvas.worldCamera, out mousePos);

            frontSightRT.anchoredPosition = mousePos;
           
            Ray mousePointRay = Cam.ScreenPointToRay(inputMousePosition);

            float cameraToProjectPlaneDistance = (transform.position - Cam.transform.position).magnitude + this.distance;

            Vector3 projectedPoint = mousePointRay.origin + mousePointRay.direction * cameraToProjectPlaneDistance;
            dragonTargetGO.transform.position = projectedPoint;

            Quaternion targetRotation = Quaternion.LookRotation(dragonTargetGO.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                rotationSpeed * Time.deltaTime);


            float curVelocityMag = rb.velocity.magnitude;

            rb.velocity =
                (rb.velocity.normalized + (dragonTargetGO.transform.position - transform.position).normalized)
                .normalized * curVelocityMag;

            Cam.transform.position = transform.position +
                                     (Cam.transform.position - transform.position).normalized * cameraDistance;
            
            Cam.transform.LookAt(transform.position);
        }
        else
        {
            fpsCamera.enabled = true;
            frontSightImg.gameObject.SetActive(false);
        }
    }

    void ClampToScreen(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, 0, Screen.width);
        pos.y = Mathf.Clamp(pos.y, 0, Screen.height);
        rectTransform.anchoredPosition = pos;
    }
}