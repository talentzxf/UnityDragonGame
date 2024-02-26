using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public static class vMeshHelper
    {
        private static void UnweldVertices(this Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;

            List<Vector3> unweldedVerticesList = new List<Vector3>();
            int[][] unweldedSubTriangles = new int[mesh.subMeshCount][];
            List<Vector2> unweldedUvsList = new List<Vector2>();
            int currVertex = 0;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                Vector3[] unweldedVertices = new Vector3[triangles.Length];
                int[] unweldedTriangles = new int[triangles.Length];
                Vector2[] unweldedUVs = new Vector2[unweldedVertices.Length];

                for (int j = 0; j < triangles.Length; j++)
                {
                    unweldedVertices[j] = vertices[triangles[j]]; //unwelded vertices are just all the vertices as they appear in the triangles array
                    if (uvs.Length > triangles[j])
                    {
                        unweldedUVs[j] = uvs[triangles[j]];
                    }
                    unweldedTriangles[j] = currVertex; //the unwelded triangle array will contain global progressive vertex indexes (1, 2, 3, ...)
                    currVertex++;
                }

                unweldedVerticesList.AddRange(unweldedVertices);
                unweldedSubTriangles[i] = unweldedTriangles;
                unweldedUvsList.AddRange(unweldedUVs);
            }

            mesh.vertices = unweldedVerticesList.ToArray();
            mesh.uv = unweldedUvsList.ToArray();

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                mesh.SetTriangles(unweldedSubTriangles[i], i, false);
            }

            RecalculateTangents(mesh);
        }

        /// <summary>
        ///     Recalculate the normals of a mesh based on an angle threshold. This takes
        ///     into account distinct vertices that have the same position.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="angle">      
        /// </param>
        public static void RecalculateNormals(this Mesh mesh, float angle)
        {
            UnweldVertices(mesh);

            float cosineThreshold = Mathf.Cos(angle * Mathf.Deg2Rad);

            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = new Vector3[vertices.Length];

            // Holds the normal of each triangle in each sub mesh.
            Vector3[][] triNormals = new Vector3[mesh.subMeshCount][];

            Dictionary<VertexKey, List<VertexEntry>> dictionary = new Dictionary<VertexKey, List<VertexEntry>>(vertices.Length);

            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; ++subMeshIndex)
            {

                int[] triangles = mesh.GetTriangles(subMeshIndex);

                triNormals[subMeshIndex] = new Vector3[triangles.Length / 3];

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int i1 = triangles[i];
                    int i2 = triangles[i + 1];
                    int i3 = triangles[i + 2];

                    // Calculate the normal of the triangle
                    Vector3 p1 = vertices[i2] - vertices[i1];
                    Vector3 p2 = vertices[i3] - vertices[i1];
                    Vector3 normal = Vector3.Cross(p1, p2);
                    float magnitude = normal.magnitude;
                    if (magnitude > 0)
                    {
                        normal /= magnitude;
                    }

                    int triIndex = i / 3;
                    triNormals[subMeshIndex][triIndex] = normal;

                    List<VertexEntry> entry;
                    VertexKey key;

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i1]), out entry))
                    {
                        entry = new List<VertexEntry>(4);
                        dictionary.Add(key, entry);
                    }

                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i1));

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i2]), out entry))
                    {
                        entry = new List<VertexEntry>();
                        dictionary.Add(key, entry);
                    }

                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i2));

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i3]), out entry))
                    {
                        entry = new List<VertexEntry>();
                        dictionary.Add(key, entry);
                    }

                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i3));
                }
            }

            // Each entry in the dictionary represents a unique vertex position.

            foreach (List<VertexEntry> vertList in dictionary.Values)
            {
                for (int i = 0; i < vertList.Count; ++i)
                {

                    Vector3 sum = new Vector3();
                    VertexEntry lhsEntry = vertList[i];

                    for (int j = 0; j < vertList.Count; ++j)
                    {
                        VertexEntry rhsEntry = vertList[j];

                        if (lhsEntry.VertexIndex == rhsEntry.VertexIndex)
                        {
                            sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                        }
                        else
                        {
                            // The dot product is the cosine of the angle between the two triangles.
                            // A larger cosine means a smaller angle.
                            float dot = Vector3.Dot(
                                triNormals[lhsEntry.MeshIndex][lhsEntry.TriangleIndex],
                                triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex]);
                            if (dot >= cosineThreshold)
                            {
                                sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                            }
                        }
                    }

                    normals[lhsEntry.VertexIndex] = sum.normalized;
                }
            }

            mesh.normals = normals;
        }

        private struct VertexKey
        {
            private readonly long _x;
            private readonly long _y;
            private readonly long _z;

            // Change this if you require a different precision.
            private const int Tolerance = 100000;

            // Magic FNV values. Do not change these.
            private const long FNV32Init = 0x811c9dc5;
            private const long FNV32Prime = 0x01000193;

            public VertexKey(Vector3 position)
            {
                _x = (long)(Mathf.Round(position.x * Tolerance));
                _y = (long)(Mathf.Round(position.y * Tolerance));
                _z = (long)(Mathf.Round(position.z * Tolerance));
            }

            public override bool Equals(object obj)
            {
                VertexKey key = (VertexKey)obj;
                return _x == key._x && _y == key._y && _z == key._z;
            }

            public override int GetHashCode()
            {
                long rv = FNV32Init;
                rv ^= _x;
                rv *= FNV32Prime;
                rv ^= _y;
                rv *= FNV32Prime;
                rv ^= _z;
                rv *= FNV32Prime;

                return rv.GetHashCode();
            }
        }

        private struct VertexEntry
        {
            public int MeshIndex;
            public int TriangleIndex;
            public int VertexIndex;

            public VertexEntry(int meshIndex, int triIndex, int vertIndex)
            {
                MeshIndex = meshIndex;
                TriangleIndex = triIndex;
                VertexIndex = vertIndex;
            }
        }


        /// <summary>
        /// Recalculates mesh tangents       
        /// </summary>
        /// <param name="mesh"></param>
        public static void RecalculateTangents(this Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;

            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            Vector4[] tangents = new Vector4[vertexCount];

            for (int a = 0; a < triangleCount; a += 3)
            {
                int i1 = triangles[a + 0];
                int i2 = triangles[a + 1];
                int i3 = triangles[a + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float div = s1 * t2 - s2 * t1;
                float r = div == 0.0f ? 0.0f : 1.0f / div;

                Vector3 sDir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tDir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sDir;
                tan1[i2] += sDir;
                tan1[i3] += sDir;

                tan2[i1] += tDir;
                tan2[i2] += tDir;
                tan2[i3] += tDir;
            }

            for (int a = 0; a < vertexCount; ++a)
            {
                try
                {
                    Vector3 n = normals[a];
                    Vector3 t = tan1[a];

                    Vector3.OrthoNormalize(ref n, ref t);
                    tangents[a].x = t.x;
                    tangents[a].y = t.y;
                    tangents[a].z = t.z;

                    tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
                }
                catch
                {
                    break;
                }
            }

            mesh.tangents = tangents;
        }

        /// <summary>
        /// Make a cylinder
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="radius"></param>
        /// <param name="length"></param>
        /// <param name="radialSegments"></param>
        /// <param name="heightSegments"></param>
        public static void CreateCylinder(this Mesh mesh, float radius, float length, int radialSegments, int heightSegments)
        {
            //calculate how many vertices we need
            var numVertexColumns = radialSegments + 1;  //+1 for welding
            var numVertexRows = heightSegments + 1;

            //calculate sizes
            int numVertices = numVertexColumns * numVertexRows;
            int numUVs = numVertices;                                   //always
            int numSideTris = radialSegments * heightSegments * 2;      //for one cap
            int numCapTris = radialSegments - 2;                        //fact
            int trisArrayLength = (numSideTris + numCapTris * 2) * 3;   //3 places in the array for each tri

            //optional: log the number of tris
            //Debug.Log ("CustomCylinder has " + trisArrayLength/3 + " tris");

            //initialize arrays
            Vector3[] Vertices = new Vector3[numVertices];
            Vector2[] UVs = new Vector2[numUVs];
            int[] Tris = new int[trisArrayLength];

            //precalculate increments to improve performance
            float heightStep = length / heightSegments;
            float angleStep = 2 * Mathf.PI / radialSegments;
            float uvStepH = (1.0f / radialSegments);
            float uvStepV = (1.0f / heightSegments);

            for (int j = 0; j < numVertexRows; j++)
            {
                for (int i = 0; i < numVertexColumns; i++)
                {
                    //calculate angle for that vertex on the unit circle
                    float angle = i * angleStep;

                    //"fold" the sheet around as a cylinder by placing the first and last vertex of each row at the same spot
                    if (i == numVertexColumns - 1)
                    {
                        angle = 0;
                    }

                    //position current vertex
                    Vertices[j * numVertexColumns + i] = new Vector3(radius * Mathf.Cos(angle), j * heightStep, radius * Mathf.Sin(angle));

                    //calculate UVs
                    UVs[j * numVertexColumns + i] = new Vector2(i * uvStepH, j * uvStepV * length);

                    //create the tris				
                    if (j == 0 || i >= numVertexColumns - 1)
                    {
                        //nothing to do on the first and last "floor" on the tris, capping is done below
                        //also nothing to do on the last column of vertices
                        continue;
                    }
                    else
                    {
                        //create 2 tris below each vertex
                        //6 seems like a magic number. For every vertex we draw 2 tris in this for-loop, therefore we need 2*3=6 indices in the Tris array
                        //offset the base by the number of slots we need for the bottom cap tris. Those will be populated once we draw the cap
                        int baseIndex = numCapTris * 3 + (j - 1) * radialSegments * 6 + i * 6;

                        //1st tri - below and in front
                        Tris[baseIndex + 0] = j * numVertexColumns + i;
                        Tris[baseIndex + 1] = j * numVertexColumns + i + 1;
                        Tris[baseIndex + 2] = (j - 1) * numVertexColumns + i;

                        //2nd tri - the one it doesn't touch
                        Tris[baseIndex + 3] = (j - 1) * numVertexColumns + i;
                        Tris[baseIndex + 4] = j * numVertexColumns + i + 1;
                        Tris[baseIndex + 5] = (j - 1) * numVertexColumns + i + 1;
                    }
                }
            }

            //draw caps
            bool leftSided = true;
            int leftIndex = 0;
            int rightIndex = 0;
            int middleIndex = 0;
            int topCapVertexOffset = numVertices - numVertexColumns;
            for (int i = 0; i < numCapTris; i++)
            {
                int bottomCapBaseIndex = i * 3;
                int topCapBaseIndex = (numCapTris + numSideTris) * 3 + i * 3;

                if (i == 0)
                {
                    middleIndex = 0;
                    leftIndex = 1;
                    rightIndex = numVertexColumns - 2;
                    leftSided = true;
                }
                else if (leftSided)
                {
                    middleIndex = rightIndex;
                    rightIndex--;
                }
                else
                {
                    middleIndex = leftIndex;
                    leftIndex++;
                }
                leftSided = !leftSided;

                //assign bottom tris
                Tris[bottomCapBaseIndex + 0] = rightIndex;
                Tris[bottomCapBaseIndex + 1] = middleIndex;
                Tris[bottomCapBaseIndex + 2] = leftIndex;

                //assign top tris
                Tris[topCapBaseIndex + 0] = topCapVertexOffset + leftIndex;
                Tris[topCapBaseIndex + 1] = topCapVertexOffset + middleIndex;
                Tris[topCapBaseIndex + 2] = topCapVertexOffset + rightIndex;
            }

            //assign vertices, uvs and tris
            mesh.vertices = Vertices;
            mesh.uv = UVs;
            mesh.triangles = Tris;

            mesh.RecalculateNormals(80);
            mesh.RecalculateBounds();

            CalculateMesh(mesh);
        }

        /// <summary>
        /// Recalculate mesh tangents
        /// </summary>
        /// <param name="mesh"></param>        
        public static void CalculateMesh(this Mesh mesh)
        {

            //speed up math by copying the mesh arrays
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;

            //variable definitions
            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            Vector4[] tangents = new Vector4[vertexCount];

            for (long a = 0; a < triangleCount; a += 3)
            {
                long i1 = triangles[a + 0];
                long i2 = triangles[a + 1];
                long i3 = triangles[a + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (long a = 0; a < vertexCount; ++a)
            {
                Vector3 n = normals[a];
                Vector3 t = tan1[a];

                //Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
                //tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
                Vector3.OrthoNormalize(ref n, ref t);
                tangents[a].x = t.x;
                tangents[a].y = t.y;
                tangents[a].z = t.z;

                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }

            mesh.tangents = tangents;
        }


    }
}