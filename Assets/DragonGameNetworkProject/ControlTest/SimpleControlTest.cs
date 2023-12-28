using UnityEngine;
using UnityEngine.UI;

public class SimpleControlTest : MonoBehaviour
{
    private Camera Cam;
    private Image frontSightImg;
    private RectTransform frontSightRT;
    private FirstPersonCamera fpsCamera;
    private Canvas canvas;

    private float distance = 5f;

    private GameObject dragonTargetGO;

    void Start()
    {
        Cam = Camera.main.GetComponent<Camera>();
        fpsCamera = Cam.GetComponent<FirstPersonCamera>();
        fpsCamera.SetCameraTarget(gameObject, false);

        GameObject canvasGO = GameObject.Find("Canvas");
        canvas = canvasGO.GetComponent<Canvas>();

        frontSightImg = canvas.transform.Find("FrontSight").GetComponent<Image>();
        frontSightRT = frontSightImg.GetComponent<RectTransform>();
        
        dragonTargetGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
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

        if (isRightMouseDown)
        {
            fpsCamera.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            frontSightImg.gameObject.SetActive(true);

            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                Input.mousePosition, canvas.worldCamera, out mousePos);

            frontSightRT.anchoredPosition = mousePos;


            Vector3 planeNormal = (transform.position - Cam.transform.position).normalized;
            Vector3 planePosition = transform.position + 5.0f * planeNormal;

            Plane plane = new Plane(planeNormal, planePosition);
            Ray mouseRay = Cam.ScreenPointToRay(mousePos);
            
            if(plane.Raycast(mouseRay, out float distance))
            {
                Vector3 intersectionPoint = mouseRay.GetPoint(distance);

                dragonTargetGO.transform.position = intersectionPoint;
            }
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