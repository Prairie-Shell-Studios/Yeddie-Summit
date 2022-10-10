using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of pools to "spawn" gameobjects from.
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    /// <summary>
    /// The details of a single object pool.
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region fields

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #endregion

    #region singleton
    
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region monobehaviour

    void Start()
    {
        Init();
    }

    #endregion

    #region api

    /// <summary>
    /// Retrieves a gameobject from the specified pool and enables it at the specified transform.
    /// </summary>
    /// <param name="tag">A string representing the pool in which to instantiate objects from.</param>
    /// <param name="position">The position to place the newly enabled game object.</param>
    /// <param name="rotation">The rotation to orientate the newly enabled game object.</param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // TODO: move to the GO being spawned
        // if the game object has a rigidbody then reset it's velocity
        Rigidbody gObjectRB = objectToSpawn.GetComponent<Rigidbody>();
        if (gObjectRB != null)
        {
            gObjectRB.velocity = Vector3.zero;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        objectToSpawn.GetComponent<IPooledObject>()?.OnObjectSpawn();

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    #endregion

    #region utility

    /// <summary>
    /// Creates the objectPool queues inside the pool dictionary and instantiates required
    /// game objects.
    /// </summary>
    private void Init()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            // create a parent empty gameobject to keep hierarchy organized
            GameObject parentGO = Instantiate(new GameObject(pool.tag + "pool"), transform);

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int index = 0; index < pool.size; index++)
            {
                GameObject gObject = Instantiate(pool.prefab, parentGO.transform);
                gObject.SetActive(false);
                objectPool.Enqueue(gObject);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    #endregion
}
