using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios.MountainGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class MountainGenerator : MonoBehaviour
    {

        #region fields

        [Header("Generation Options")]
        [SerializeField]
        [Tooltip("Whether not the mesh should be recomputed in Start method")]
        private bool generateOnStart = false;

        [Header("Mountain Dimensions")]
        [SerializeField] [Tooltip("Max X-Value")] private float width = 10f;
        [SerializeField] [Tooltip("Max Z-Value")] private float length = 10f;
        [SerializeField] [Tooltip("Range for the max Y-Value")] private Vector2 heightRange = new Vector2(10f, 10f);

        [Header("Mesh Resolution")]
        [SerializeField] [Min(1)] [Tooltip("Number of Bezier Surfaces")] 
        private int mountainResolution = 1;
        [SerializeField] [Min(1)] [Tooltip("Max number of Segments per Bezier Surface")] 
        private int surfaceResolution = 10;

        [Header("Mesh Effects")]
        public bool hasNoise = false;
        [SerializeField] [Min(0.01f)] private float scale = 1f;
        [SerializeField] private bool clampedEdges = false;

        [Header("Boundary Gizmos")]
        public bool showBounds = false;
        public float pointSize = 0.2f;

        private Mesh mesh;
        private Vector3[][] controlPoints;
        private List<Vector3> vertices;
        private List<int> triangles;

        #endregion

        #region monobehaviour

        void Start()
        {
            if (generateOnStart)
            {
                GenerateMountain();
            }
        }

        void OnDrawGizmos()
        {
            if (showBounds)
            {
                Gizmos.DrawSphere(transform.position + new Vector3(-width, 0f, -length), pointSize);
                Gizmos.DrawSphere(transform.position + new Vector3(-width, 0f, length), pointSize);
                Gizmos.DrawSphere(transform.position + new Vector3(width, 0f, -length), pointSize);
                Gizmos.DrawSphere(transform.position + new Vector3(width, 0f, length), pointSize);
            }
        }

        #endregion

        #region mesh

        public void GenerateMountain()
        {
            Init();

            CreateMountain();

            if (hasNoise)
            {
                AddNoise();
            }

            UpdateMesh();
        }

        private void CreateMountain()
        {
            BezierSurfaceGenerator bezierGen = new BezierSurfaceGenerator();
            BezierControlPointGenerator cpGen = new BezierControlPointGenerator();

            controlPoints = cpGen.GenerateControlPoints(transform.position, width, UnityEngine.Random.Range(heightRange.x, heightRange.y), length, mountainResolution);

            int xRes = width >= length ? surfaceResolution : Mathf.CeilToInt(width / length * surfaceResolution);
            int zRes = width <= length ? surfaceResolution : Mathf.CeilToInt(length / width * surfaceResolution);

            int numSurfaces = (int) Mathf.Pow(mountainResolution, 2);
            int jump = (zRes + 1) * (xRes + 1);

            for (int surface = 0, offset = 0; surface < numSurfaces; surface++)
            {
                Tuple<Vector3[], int[]> meshInfo = bezierGen.GenerateSurface(controlPoints[surface], xRes, zRes, offset);
                vertices.AddRange(meshInfo.Item1);
                triangles.AddRange(meshInfo.Item2);

                offset += jump;
            }
        }


        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
        }

        #endregion

        #region noise

        private void AddNoise()
        {
            // randomize sample position
            float[] offset = {UnityEngine.Random.Range(0f, 999999f), UnityEngine.Random.Range(0f, 999999f)};

            for (int vert = 0; vert < vertices.Count; vert++)
            {
                Vector3 vertex = vertices[vert];

                if (!IsEdge(vertex))
                {
                    vertex.y += Mathf.PerlinNoise((vertex.x + offset[0]) * scale, (vertex.z + offset[1]) * scale);
                    vertices[vert] = vertex;
                }
            }
        }

        private bool IsEdge(Vector3 vertex)
        {
            if (clampedEdges && vertex != null)
            {
                return vertex.x == transform.position.x - width || vertex.x == transform.position.x + width ||
                    vertex.z == transform.position.z - length || vertex.z == transform.position.z + length;
            }
            else
            {
                return false;
            }
        }
        
        #endregion

        #region initialization

        private void Init()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                GetComponent<MeshFilter>().mesh = mesh;
            }

            vertices = new List<Vector3>();
            triangles = new List<int>();
        }

        #endregion
    }
}