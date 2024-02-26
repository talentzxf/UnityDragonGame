using UnityEngine;

namespace Invector
{
    [vClassHeader("Rope-ZipLine", openClose = false)]
    public class vRope : vMonoBehaviour
    {
        public Color gizmosColor = Color.green;
        [Range(3, 32)]
        public int radialSegments = 12;
        [Range(1, 100)]
        public int heightSegments = 1;
        public float radius = 0.01f;
        public float colliderSizeY = 0.5f;
        public float colliderSizeX = 0.5f;
        public float colliderOffsetStart;
        public float colliderOffsetEnd;
        public float colliderOffsetY;
        [vSeparator("Components")]
        public Transform ancorReference;
        public BoxCollider boxCollider;
        public Transform endPoint;
        public MeshFilter meshFilter;
        [vReadOnly(false)] protected float length;


        protected Vector3 lastPosition;
        protected Vector3 lastEndPosition;

        public float Length => length;

        [SerializeField] protected Mesh mesh;
        bool hasAndPoint;
        [RuntimeInitializeOnLoadMethod]
        public void Rebuild()
        {
            if (endPoint && meshFilter && ancorReference && boxCollider) BuildMesh();
        }
        private void OnDrawGizmos()
        {

            if (ancorReference == null)
            {
                ancorReference = transform.Find("AncorReference");
                if (ancorReference == null)
                {
                    ancorReference = new GameObject("AncorReference", typeof(BoxCollider)).transform;
                    ancorReference.SetParent(transform);
                    ancorReference.SetPositionAndRotation(transform.position, transform.rotation);
                }
                return;
            }
            if (boxCollider == null)
            {
                ancorReference.TryGetComponent(out boxCollider);
                return;
            }

            if (meshFilter == null)
            {
                var filterObject = ancorReference.Find("RopeMesh");

                if (filterObject == null)
                {
                    meshFilter = new GameObject("RopeMesh", typeof(MeshRenderer)).AddComponent<MeshFilter>();
                    meshFilter.transform.SetParent(ancorReference);
                    meshFilter.transform.SetPositionAndRotation(ancorReference.position, ancorReference.rotation);
                    if (meshFilter.TryGetComponent(out MeshRenderer renderer))
                    {
                        var material = (Material)Resources.Load("DefaultRopeMaterial");
                        if (material) renderer.material = material;
                    }
                }
                else
                {
                    filterObject.TryGetComponent(out meshFilter);
                }
                return;
            }

            if (endPoint)
            {
                if (lastEndPosition != endPoint.position || lastPosition != transform.position || !hasAndPoint)
                {
                    BuildMesh();
                }
                boxCollider.size = new Vector3(colliderSizeX, colliderSizeY, length + (colliderOffsetEnd + colliderOffsetStart));

                var center = (Vector3.forward * length * 0.5f) - (Vector3.forward * colliderOffsetStart * 0.5f) + (Vector3.forward * colliderOffsetEnd * 0.5f) + Vector3.up * colliderOffsetY;

                boxCollider.center = center;
                Matrix4x4 matrix = Gizmos.matrix;
                Gizmos.matrix = ancorReference.localToWorldMatrix;
                Gizmos.color = gizmosColor * 0.2f;
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                Gizmos.color = gizmosColor;
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
            else
            {
                hasAndPoint = false;
            }
        }
        private void BuildMesh()
        {
            hasAndPoint = true;
            lastEndPosition = endPoint.position;
            lastPosition = transform.position;
            ancorReference.LookAt(endPoint, Vector3.up);
            length = Vector3.Distance(ancorReference.position, endPoint.position);
            meshFilter.transform.localEulerAngles = new Vector3(90, 0, 0);
            meshFilter.transform.localPosition = Vector3.zero;
            ancorReference.localPosition = Vector3.zero;
            endPoint.LookAt(ancorReference);

            mesh = new Mesh();
            mesh.name = "vRope";
            meshFilter.mesh = mesh;
            mesh.CreateCylinder(radius, length, radialSegments, heightSegments);
        }
    }
}