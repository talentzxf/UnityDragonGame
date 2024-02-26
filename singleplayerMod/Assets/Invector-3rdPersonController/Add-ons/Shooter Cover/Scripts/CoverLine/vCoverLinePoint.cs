
using UnityEngine;

public class vCoverLinePoint : MonoBehaviour
{
    public Vector3 tangentLeft = Vector3.one;
    public Vector3 tangentRight = Vector3.one * -1;
    public vCoverLine.CurveType curveType = vCoverLine.CurveType.Align;

    public vCoverLinePoint conectedLeft, conectedRight;
    private void Reset()
    {

    }
    public float tangentsLenght => (tangentLeft.magnitude + tangentRight.magnitude);

}