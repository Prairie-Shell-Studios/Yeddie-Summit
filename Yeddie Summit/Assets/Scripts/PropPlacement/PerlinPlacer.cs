using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudio.PropPlacement
{
    /// <summary>
    /// Uses Perlin noise and raycasting to procedurally place GOs.
    /// </summary>
    public class PerlinPlacer : MonoBehaviour
    {
        #region fields
        [SerializeField] [Min(0)] private float delay = 0f;

        [Header("Placement Dimensions")]
        [SerializeField] [Min(0)] private float width = 10f; // x
        [SerializeField] [Min(0)] private float length = 10f; // z
        [SerializeField] private float raycastDistance = 250f;
        [SerializeField] private LayerMask rayMask;

        [Header("Props To Place")]
        public List<PerlinPlacedProp> props = new List<PerlinPlacedProp>();

        [Header("Noise Properties")]
        [SerializeField] [Min(0.01f)] float noiseScale = 1f;
        private float xOffset = 0f;
        private float zOffset = 0f;

        #endregion

        #region properties

        #endregion

        #region monobehaviour

        void Start()
        {
            StartCoroutine(LateStart());
        }

        /// <summary>
        /// Delays the prop placement until the specified delay time has elapsed.
        /// Required so that the terrain can first be generated.
        /// </summary>
        /// <returns>A WaitForSeconds with the delay time.</returns>
        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(delay);
            PlaceProps();
        }

        #endregion

        #region placement

        /// <summary>
        /// Places each prop within the collection of PerlinPlacedProp instances.
        /// </summary>
        public void PlaceProps()
        {
            xOffset = Random.Range(0f, 99999f);
            zOffset = Random.Range(0f, 99999f);

            foreach (PerlinPlacedProp prop in props)
            {
                Place(prop);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        private void Place(PerlinPlacedProp prop)
        {
            // generate grid
            float startX = transform.position.x - width;
            float startZ = transform.position.z - length;
            float endX = transform.position.x + width;
            float endZ = transform.position.z + length;

            Vector2 cellSize = new Vector2(2 * width / prop.Density, 2 * length / prop.Density);

            for (float x = startX; x <= endX; x += cellSize.x)
            {
                float xCoord = (x + xOffset) * noiseScale;
                for (float z = startZ; z < endZ; z += cellSize.y)
                {
                    float zCoord = (z + zOffset) * noiseScale;
                    // check if Perlin value is within thresholds
                    if (prop.CanSpawn(Mathf.PerlinNoise(xCoord, zCoord)))
                    {
                        // cast a ray and check if it collides with terrain
                        SpawnRaycast(prop, x, z, cellSize);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        public void SpawnRaycast(PerlinPlacedProp prop, float x, float z, Vector2 cellSize)
        {
            RaycastHit hit;

            Vector2 halfCellSize = cellSize / 2;
            Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(x - halfCellSize.x, x + halfCellSize.x),
                    UnityEngine.Random.Range(z - halfCellSize.y, z + halfCellSize.y));

            if (Physics.Raycast(new Vector3(spawnPos.x, transform.position.y, spawnPos.y), Vector3.down, out hit, raycastDistance, rayMask))
            {
                prop.InstantiateObject(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
        }

        

        #endregion

        #region noise

        /// <summary>
        /// 
        /// </summary>
        private void GenerateNoise()
        {

        }

        #endregion
    }
}