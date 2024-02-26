using UnityEngine;

public class vCoverLine : MonoBehaviour
{
    public bool close;
    [Tooltip("Approximate Width of Cover Points"), Range(.1f, 1f)]
    public float width = 1f;
    [Range(.5f, 2f)]
    public float height = 1f;
    [Tooltip("The Z size of Cover Points"), Range(.2f, 1f)]
    public float depht = 0.5f;
    [Tooltip("The Z position of Cover Points"), Range(0f, 1f)]
    public float offsetZ = 0;
    [vTagSelector]
    public string coverPointTag = "CoverPoint";
    [vLayerSelector]
    public string coverPointLayer = "CoverPoint";
    public Transform CtrlPTransform;
    public Transform CPTransform;
    int pointCount;
    public enum CurveType
    {
        Linear,
        LinearRight,
        LinearLeft,
        Align,
        Mirror,
        Free
    }

    private void Reset()
    {


        close = false;
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        CtrlPTransform = new GameObject("ControlPoints").transform;
        CPTransform = new GameObject("CoverPoints").transform;
        CtrlPTransform.parent = transform;
        CPTransform.parent = transform;
        CtrlPTransform.localPosition = Vector3.zero;
        CtrlPTransform.localEulerAngles = Vector3.zero;
        CPTransform.localPosition = Vector3.zero;
        CPTransform.localEulerAngles = Vector3.zero;
        var p1 = new GameObject("P1").AddComponent<vCoverLinePoint>();
        var p2 = new GameObject("P2").AddComponent<vCoverLinePoint>();
        p1.transform.parent = CtrlPTransform;
        p2.transform.parent = CtrlPTransform;
        p1.transform.localPosition = Vector3.right * -1;
        p2.transform.localPosition = Vector3.right;
        p1.curveType = CurveType.Linear;
        p2.curveType = CurveType.Linear;
        p1.conectedRight = p2;
        p2.conectedLeft = p1;

    }

    public vCoverLinePoint NewPoint(Vector3 localPosition)
    {
        var p1 = new GameObject("P" + (CtrlPTransform.childCount + 1)).AddComponent<vCoverLinePoint>();
        p1.transform.parent = CtrlPTransform;
        p1.transform.localPosition = localPosition;
        p1.curveType = CurveType.Linear;


        return p1;
    }
    public vCoverLinePoint NewPoint(Vector3 localPosition, int siblingIndex)
    {
        var p1 = NewPoint(localPosition);
        p1.transform.SetSiblingIndex(siblingIndex);

        return p1;
    }
}
