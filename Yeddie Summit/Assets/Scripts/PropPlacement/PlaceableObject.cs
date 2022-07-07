using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PrairieShellStudios.ObjectPlacement
{
    /// <summary>
    /// Responsible for handling properties that determine the instantiation of this GO by the PerlinPlacer.
    /// </summary>
    public class PlaceableObject : MonoBehaviour
    {
        #region fields

        [Header("Instantiation Properties")]
        [SerializeField] [Range(0f, 359f)] private float minYRotation = 0f;
        [SerializeField] [Range(0f, 359f)] private float maxYRotation = 359f;
        [SerializeField] [Min(0.01f)] private float minScale = 1f;
        [SerializeField] [Min(0.01f)] private float maxScale = 1f;
    
        #endregion

        #region properties

        public float MinRotation { get => minYRotation; }
        public float MaxRotation { get => maxYRotation; }
        public float MinScale { get => minScale; }
        public float MaxScale { get => maxScale; }

        #endregion

        #region monobehaviour


        #endregion

        #region instantiation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject Instantiate(Vector3 position, Quaternion rotation)
        {           
            GameObject clone = Instantiate(gameObject, position, rotation);

            RandomizeYRotation(clone);
            RandomizeScale(clone);

            return clone;
        }

        /// <summary>
        /// Randomizes the rotation of the specified gameobject.
        /// </summary>
        /// <param name="clone">The gameobject to have it's rotation changed.</param>
        private void RandomizeYRotation(GameObject clone)
        {
            clone.transform.Rotate(0f, UnityEngine.Random.Range(minYRotation, maxYRotation), 0f, Space.Self);
        }

        /// <summary>
        /// Randomizes the scale of the specified gameobject.
        /// </summary>
        /// <param name="clone">The gameobject to have it's scale changed.</param>
        private void RandomizeScale(GameObject clone)
        {
            float scale = UnityEngine.Random.Range(minScale, maxScale);
            clone.transform.localScale = new Vector3(scale, scale, scale);
        }

        #endregion
    }
}
