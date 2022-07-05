using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for handling properties that determine the instantiation of this GO by the PerlinPlacer.
/// </summary>
public class PerlinPlacedProp : MonoBehaviour
{
    #region fields

    [Header("Instantiation Properties")]
    public List<GameObject> objectTypes = new List<GameObject>();
    public bool alignWithNormal = false;
    [SerializeField] [Range(0f, 359f)] private float minYRotation = 0f;
    [SerializeField] [Range(0f, 359f)] private float maxYRotation = 359f;
    [SerializeField] [Min(0.01f)] private float minScale = 1f;
    [SerializeField] [Min(0.01f)] private float maxScale = 1f;
    
    [Header("Placement Properties")]
    [SerializeField] [Range(0f, 1f)] private float minPerlinThreshold = 0f;
    [SerializeField] [Range(0f, 1f)] private float maxPerlinThreshold = 1f;
    [SerializeField] [Min(1f)] private float density = 1f;

    #endregion

    #region properties

    public float MinPerlinThreshold { get => minPerlinThreshold; }
    public float MaxPerlinThershold { get => maxPerlinThreshold; }
    public float MinRotation { get => minYRotation; }
    public float MaxRotation { get => maxYRotation; }
    public float MinScale { get => minScale; }
    public float MaxScale { get => maxScale; }
    public float Density { get => density;}

    #endregion

    #region monobehaviour


    #endregion

    #region instantiation

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitPosition"></param>
    /// <param name="hitRotation"></param>
    /// <returns></returns>
    public GameObject InstantiateObject(Vector3 hitPosition, Quaternion hitRotation)
    {
        if (objectTypes.Count == 0)
        {
            Debug.LogWarning("Could not instantiate props from this gameobject because it has none attached.", gameObject);
            return null;
        }

        GameObject clone;

        if (alignWithNormal)
        {
            clone = Instantiate(RandomizeObject(), hitPosition, hitRotation);
        }
        else
        {
            clone = Instantiate(RandomizeObject(), hitPosition, Quaternion.identity);
        }

        RandomizeYRotation(clone);
        RandomizeScale(clone);

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
    public bool CanSpawn(float perlinValue)
    {
        return perlinValue >= minPerlinThreshold && perlinValue <= maxPerlinThreshold;
    }

    /// <summary>
    /// Randomly chooses which prop out of the collection to instantiate.
    /// </summary>
    /// <returns>A randomly chosen GO from the props collecton.</returns>
    private GameObject RandomizeObject()
    {
        return objectTypes[UnityEngine.Random.Range(0, objectTypes.Count - 1)];
    }

    /// <summary>
    /// Randomizes the rotation of the specified gameobject.
    /// </summary>
    /// <param name="prop">The gameobject to have it's rotation changed.</param>
    private void RandomizeYRotation(GameObject prop)
    {
        Quaternion propRotation = prop.transform.localRotation;
        Quaternion randYRotation = Quaternion.Euler(propRotation.x, UnityEngine.Random.Range(minYRotation, maxYRotation), propRotation.y);
        prop.transform.localRotation = randYRotation;
    }

    /// <summary>
    /// Randomizes the scale of the specified gameobject.
    /// </summary>
    /// <param name="prop">The gameobject to have it's scale changed.</param>
    private void RandomizeScale(GameObject prop)
    {
        float scale = UnityEngine.Random.Range(minScale, maxScale);
        prop.transform.localScale = new Vector3(scale, scale, scale);
    }

    #endregion
}
