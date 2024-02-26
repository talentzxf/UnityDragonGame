using UnityEngine;

public class vZiplineAnchorPoints : MonoBehaviour
{
    public bool invertDirection;
    public virtual Vector3 movementDirection => invertDirection ? -transform.forward : transform.forward;
}
