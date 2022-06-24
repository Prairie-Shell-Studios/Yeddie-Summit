using System;
using UnityEngine;

namespace PrairieShellStudios.MountainGeneration
{
    [RequireComponent(typeof(MeshFilter))]
    public class MountainGenerator : MonoBehaviour
    {

        #region fields

        [Header("Mountain Dimensions")]
        [SerializeField] [Tooltip("Max X-Value")] private float width = 10f;
        [SerializeField] [Tooltip("Max Z-Value")] private float length = 10f;
        [SerializeField] [Tooltip("Range for the max Y-Value")] private Vector2 heightRange = new Vector2(10f, 10f);

        [Header("Mesh Resolution")]
        [SerializeField] [Tooltip("Number of Bezier Surfaces")] 
        private int mountainResolution = 1;
        [SerializeField] [Tooltip("Max number of Segments per Bezier Surface")] 
        private int surfaceResolution = 10;

        [Header("Mesh Effects")]
        public bool hasNoise = false;

        [Header("Boundary Gizmos")]
        public bool showBounds = false;
        public float pointSize = 0.2f;

        private Mesh mesh;
        private Vector3[] controlPoints;
        private Tuple<Vector3[], int[]> meshInfo;

        #endregion

        #region monobehaviour

        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateMountain();

            if (meshInfo != null)
            {
                if (hasNoise)
                {
                    AddNoise();
                }
                UpdateMesh();
            }
            else
            {
                Debug.LogWarning("No meshInfo could be found when updating mesh.", gameObject);
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

        private void CreateMountain()
        {
            BezierSurfaceGenerator bezierGen = new BezierSurfaceGenerator();
            BezierControlPointGenerator cpGen = new BezierControlPointGenerator();

            controlPoints = cpGen.GenerateControlPoints(transform.position, width, UnityEngine.Random.Range(heightRange.x, heightRange.y), length, mountainResolution);

            int xRes = width >= length ? surfaceResolution : Mathf.CeilToInt(width / length * surfaceResolution);
            int zRes = width <= length ? surfaceResolution : Mathf.CeilToInt(length / width * surfaceResolution);

            meshInfo = bezierGen.GenerateSurface(controlPoints, xRes, zRes);
        }


        private void UpdateMesh()
        {
            mesh.Clear();

            mesh.vertices = meshInfo.Item1;
            mesh.triangles = meshInfo.Item2;

            mesh.RecalculateNormals();
        }

        #endregion

        #region noise

        private void AddNoise()
        {
            for (int vert = 0; vert < meshInfo.Item1.Length; vert++)
            {
                Vector3 vertex = meshInfo.Item1[vert];
                vertex.y += Mathf.PerlinNoise(vertex.x, vertex.z);
                meshInfo.Item1[vert] = vertex;
            }
        }

        #endregion
    }
}