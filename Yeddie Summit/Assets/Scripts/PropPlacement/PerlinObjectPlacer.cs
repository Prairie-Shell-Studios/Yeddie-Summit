using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios.ObjectPlacement
{
    /// <summary>
    /// Uses Perlin noise and raycasting to procedurally place GOs.
    /// </summary>
    public class PerlinObjectPlacer : MonoBehaviour
    {
        #region fields

        [Header("Placer Options")]
        [SerializeField] private bool placeOnStart = false;
        [SerializeField] private LayerMask rayMask;
        [SerializeField] [Min(0)] private float delay = 0f;

        [Header("Placement Dimensions")]
        [SerializeField] [Min(0)] private float width = 10f; // x
        [SerializeField] [Min(0)] private float length = 10f; // z
        [SerializeField] private float height = 250f; // y

        [Header("Props To Place")]
        public List<PlaceableObjectCollection> placeableObjects = new List<PlaceableObjectCollection>();

        [Header("Noise Properties")]
        [SerializeField] [Min(0.01f)] float noiseScale = 1f;
        private float xOffset = 0f;
        private float zOffset = 0f;

        [Header("Boundary Gizmos")]
        [SerializeField] private bool showBounds = false;


        #endregion

        #region properties

        #endregion

        #region monobehaviour

        void Start()
        {
            if (placeOnStart)
            {
                StartCoroutine(LateStart());
            }
        }

        /// <summary>
        /// Delays the prop placement until the specified delay time has elapsed.
        /// Required so that the terrain can first be generated.
        /// </summary>
        /// <returns>A WaitForSeconds with the delay time.</returns>
        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(delay);
            SpawnObjects();
        }

        private void OnDrawGizmos()
        {
            if (showBounds)
            {
                Gizmos.DrawWireCube(transform.position - new Vector3(0f, height/2, 0f), new Vector3(2*width, height, 2*length));
            }
        }

        #endregion

        #region placement

        /// <summary>
        /// Places each prop within the collection of PerlinPlacedProp instances.
        /// </summary>
        public void SpawnObjects()
        {
            xOffset = Random.Range(0f, 99999f);
            zOffset = Random.Range(0f, 99999f);

            foreach (PlaceableObjectCollection collection in placeableObjects)
            {
                SpawnFromCollection(collection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        private void SpawnFromCollection(PlaceableObjectCollection collection)
        {
            // generate grid
            float startX = transform.position.x - width;
            float startZ = transform.position.z - length;
            float endX = transform.position.x + width;
            float endZ = transform.position.z + length;

            Vector2 cellSize = new Vector2(2 * width / collection.Density, 2 * length / collection.Density);

            for (float x = startX; x <= endX; x += cellSize.x)
            {
                float xCoord = (x + xOffset) * noiseScale;
                for (float z = startZ; z < endZ; z += cellSize.y)
                {
                    float zCoord = (z + zOffset) * noiseScale;
                    float perlinValue = Mathf.PerlinNoise(xCoord, zCoord);
                    // check if Perlin value is within thresholds
                    if (collection.ValidPerlinValue(perlinValue))
                    {
                        // cast a ray and check if it collides with terrain
                        SpawnRaycast(perlinValue, collection, x, z, cellSize);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public void SpawnRaycast(float perlinValue, PlaceableObjectCollection collection, float x, float z, Vector2 cellSize)
        {
            RaycastHit hit;

            Vector2 halfCellSize = cellSize / 2;
            Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(x - halfCellSize.x, x + halfCellSize.x),
                    UnityEngine.Random.Range(z - halfCellSize.y, z + halfCellSize.y));

            if (Physics.Raycast(new Vector3(spawnPos.x, transform.position.y, spawnPos.y), Vector3.down, out hit, height, rayMask))
            {
                if (collection.CanSpawn(perlinValue, hit.point.y))
                {
                    collection.InstantiatePlaceableObject(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
            }
        }

        #endregion
    }
}