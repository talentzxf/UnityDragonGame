using UnityEditor;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DragonGameNetworkProject.Gems
{
    public class CylinderGemGenerator: AbstractGemGenerator
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(CylinderGemGenerator))]
        class CylinderGemGeneratorEditor: GemGeneratorEditor{}

        [SerializeField] private float rMin = 10.0f;
        [SerializeField] private float rMax = 20.0f;
        [SerializeField] private float hMax = 20.0f;
        [SerializeField] private int count = 36;
        
        public override void GenerateGems()
        {
            int segments = count;
            float angleIncrement = 360f / segments;

            Vector3 center = transform.position;

            for (int i = 0; i < count; i++)
            {
                float angle = i * angleIncrement;
                float radius = Random.Range(rMin, rMax);
                float x = center.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = center.z + radius * Mathf.Sin(Mathf.Deg2Rad * angle);

                float height = center.y + Random.Range(-hMax * 0.5f, hMax * 0.5f);
                GenerateGemAtPosition(new Vector3(x, height, z));
            }
        }

        private const float GIZMO_DISK_THICKNESS = 0.01f;

        private void DrawDisk(Vector3 position, float radius)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        private void DrawCylinderSide(Vector3 center, float radius, float height)
        {
            Gizmos.color = Color.yellow;
            int segments = count;
            float angleIncrement = 360f / segments;
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleIncrement;
                float x = center.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = center.z + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
                Vector3 startPoint = new Vector3(x, center.y, z) - Vector3.up * height * 0.5f;
                Vector3 endPoint = new Vector3(x, center.y, z) + Vector3.up * height * 0.5f;
                
                Gizmos.DrawLine(startPoint, endPoint);
            }
        }
        
        private void OnDrawGizmos()
        {
            Vector3 center = transform.position;
            
            Vector3 topCenter = center + Vector3.up * hMax * 0.5f;
            DrawDisk(topCenter, rMax);
            DrawDisk(topCenter, rMin);

            Vector3 bottomCenter = center - Vector3.up * hMax * 0.5f;
            DrawDisk(bottomCenter, rMax);
            DrawDisk(bottomCenter, rMin);
            
            DrawCylinderSide(center, rMin, hMax);
            DrawCylinderSide(center, rMax, hMax);
        }
#endif
    }
}