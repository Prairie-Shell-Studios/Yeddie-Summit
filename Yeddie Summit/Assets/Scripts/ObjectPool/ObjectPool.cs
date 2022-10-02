using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region fields

    [SerializeField] private GameObject prefab;
    [SerializeField] private int size = 25;
    private List<GameObject> pool;
    
    #endregion

    #region monobehaviour

    void Awake()
    {
        Init();
    }

    #endregion

    #region api

    /// <summary>
    /// Goes through the
    /// </summary>
    /// <returns></returns>
    public bool HasSpawnable()
    {
        bool hasSpawnable = false;
        foreach(GameObject gObject in pool)
        {
            if (!gObject.activeSelf)
            {
                hasSpawnable = true;
                break;
            }
        }

        return hasSpawnable;
    }

    /// <summary>
    /// Actives an inactive GO in the pool and aligns it with the transform.
    /// </summary>
    /// <param name="spawnTransform">The transform of where the prefab will be spawned.</param>
    public void SpawnObject(Transform spawnTransform)
    {
        GameObject inactivePrefab = GetInactivePrefab();

        if (inactivePrefab != null)
        {
            inactivePrefab.transform.position = spawnTransform.position;
            inactivePrefab.transform.rotation = spawnTransform.rotation;
            inactivePrefab.transform.localScale = spawnTransform.localScale;

            inactivePrefab.SetActive(true);
        }
    }

    #endregion

    #region utilities

    /// <summary>
    /// Initiates the pool with instances of the class prefab.
    /// </summary>
    private void Init()
    {
        pool = new List<GameObject>();

        if (prefab != null)
        {
            for (int count = 0; count < size; count++)
            {
                pool.Add(Instantiate(prefab));
                pool[pool.Count - 1].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Retreives an inactive gameobject within the pool.
    /// </summary>
    /// <returns>An IPoolable gameobject in the pool that is inactive.</returns>
    private GameObject GetInactivePrefab()
    {
        GameObject foundPrefab = null;

        foreach(GameObject gObject in pool)
        {
            if (!gObject.activeSelf)
            {
                foundPrefab = gObject;
                break;
            }
        }

        return foundPrefab;
    }

    #endregion
}