using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios.ObjectPlacement
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class PlaceableObjectCollection
    {
        #region fields

        [Header("Instantiation Properties")]
        [SerializeField] private List<PlaceableObject> placeableObjects;
        [SerializeField] private bool alignWithNormal = false;

        [Header("Placement Properties")]
        [SerializeField] [Range(0f, 1f)] private float minPerlinThreshold = 0f;
        private float currMinPerlinThreshold = 0f;
        [SerializeField] [Range(0f, 1f)] private float maxPerlinThreshold = 1f;
        [SerializeField] private float minHeight = 0f;
        [SerializeField] private float maxHeight = 100f;
        [SerializeField] [Min(1f)] private float density = 1f;

        #endregion

        #region properties
        public List<PlaceableObject> PlaceableObjects { get => placeableObjects; }
        public bool AlignWithNormal { get => alignWithNormal; set => alignWithNormal = value; }
        public float MinPerlinThreshold { get => minPerlinThreshold; }
        public float MaxPerlinThreshold { get => maxPerlinThreshold; }
        public float Density { get => density; }

        #endregion

        #region constructors

        public PlaceableObjectCollection()
        {
            placeableObjects = new List<PlaceableObject>();
            currMinPerlinThreshold = minPerlinThreshold;
        }

        public PlaceableObjectCollection(bool alignWithNormal) : base()
        {
            this.alignWithNormal = alignWithNormal;
        }

        #endregion

        #region instantiation

        public GameObject InstantiatePlaceableObject(Vector3 position, Quaternion rotation, Transform parent)
        {
            if (placeableObjects.Count == 0)
            {
                Debug.LogWarning("Could not instantiate from empty collection of PlaceableObjects.");
                return null;
            }

            GameObject clone = null;

            if (alignWithNormal)
            {
                clone = RandomizeObject().Instantiate(position, rotation);
            }
            else
            {
                clone = RandomizeObject().Instantiate(position, Quaternion.identity);
            }

            clone.gameObject.transform.parent = parent;

            return clone;
        }

        /// <summary>
        /// Determines whether or not the provided value is within the min and max thresholds.
        /// </summary>
        /// <param name="perlinValue">The value to check if it's between the thresholds.</param>
        /// <returns>
        /// True if the param is within the min and max thresholds.
        /// False if the param is not.
        /// </returns>
        public bool ValidPerlinValue(float perlinValue)
        {
            return perlinValue >= minPerlinThreshold && perlinValue <= maxPerlinThreshold;
        }

        /// <summary>
        /// Linearly interpolates the current minimum Perlin noise value that the placeable object needs in order to be spawned.
        /// Provides a nice cutoff for the instantiating of objects.
        /// </summary>
        /// <param name="perlinValue"></param>
        /// <param name="yValue"></param>
        /// <returns></returns>
        public bool CanSpawn(float perlinValue, float yValue)
        {
            if (yValue >= minHeight && yValue <= maxHeight)
            {
                currMinPerlinThreshold = Mathf.Lerp(minPerlinThreshold, maxPerlinThreshold, ((yValue - minHeight) / (maxHeight - minHeight)));
                return perlinValue >= currMinPerlinThreshold && perlinValue <= maxPerlinThreshold;
            }

            return false;
        }

        /// <summary>
        /// Randomly chooses which prop out of the collection to instantiate.
        /// </summary>
        /// <returns>A randomly chosen GO from the props collecton.</returns>
        private PlaceableObject RandomizeObject()
        {
            return placeableObjects[Random.Range(0, placeableObjects.Count - 1)];
        }

        #endregion


    }
}