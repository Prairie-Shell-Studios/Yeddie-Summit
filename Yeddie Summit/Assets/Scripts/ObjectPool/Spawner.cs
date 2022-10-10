using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrairieShellStudios.Timer;
using System;

/// <summary>
/// A simple spawner that is intended to just work for the prototype.
/// This could be made more complex with more features and that is a goal for later.
/// </summary>
public class Spawner : MonoBehaviour
{

    #region fields

    private ObjectPooler objectPooler;
    [SerializeField] private bool randomizeSpawnObjects = false;
    [SerializeField] private string spawnTag;
    [SerializeField] private float rate = 5f;
    [SerializeField] private bool spawnOnStart = true;
    private bool isActive = false;
    private SimpleTimer timer;

    [Header("Randomized Position")]
    [SerializeField] private bool randomizePosition = false;
    [SerializeField] private SpawnArea spawnArea = SpawnArea.Rectangle;
    [SerializeField] [Min(0)] private float width = 10f; // x
    [SerializeField] [Min(0)] private float length = 10f; // z
    [SerializeField] [Min(0)] private float radius = 20f;
    [SerializeField] private bool showBounds = false;
    private Tuple<float, float> minMaxX;
    private Tuple<float, float> minMaxZ;

    #endregion

    #region monobehaviour

    private void Awake()
    {
        minMaxX = new Tuple<float, float>(transform.position.x - width, transform.position.x + width);
        minMaxZ = new Tuple<float, float>(transform.position.z - length, transform.position.z + length);
    }

    /// <summary>
    /// Initiate spawner here because ObjectPool is initiated in Awake.
    /// </summary>
    void Start()
    {
        objectPooler = ObjectPooler.Instance;

        timer = new SimpleTimer(TimerDirection.CountDown, rate);

        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        if (isActive)
        {
            if (timer.HasExpired())
            {
                //string poolTag = GetRandomSpawnObject();
                objectPooler?.SpawnFromPool(spawnTag, GetRandomPosition(), Quaternion.identity);
                timer.Reset();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showBounds)
        {
            if (spawnArea == SpawnArea.Rectangle)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(2 * width, 0f, 2 * length));
            }
            else if (spawnArea == SpawnArea.Circle)
            {
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }

    #endregion

    #region api

    public void StartSpawning()
    {
        isActive = true;
        timer.Start();
    }

    public void StopSpawning()
    {
        isActive = false;
        timer.Stop();
        timer.Reset();
    }

    #endregion

    #region utility

    protected Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = this.transform.position;

        if (randomizePosition)
        {
            if (spawnArea == SpawnArea.Rectangle)
            {
                randomPosition = new Vector3(UnityEngine.Random.Range(minMaxX.Item1, minMaxX.Item2), 
                    randomPosition.y, UnityEngine.Random.Range(minMaxZ.Item1, minMaxZ.Item2));
            }
            else if (spawnArea == SpawnArea.Circle)
            {
                Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;
                randomPosition = new Vector3(randomPoint.x, randomPosition.y, randomPoint.y);
            }
        }

        return randomPosition;
    }

    //private string GetRandomSpawnObject()
    //{
    //    string randomSpawnTag = spawnObjects[UnityEngine.Random.Range(0, spawnObjects.Count)].Item1;
    //    if (randomizeSpawnObjects)
    //    {
    //        // TODO: randomize spawn object
    //    }

    //    return randomSpawnTag;
    //}

    #endregion
}

#region enums

public enum SpawnArea { Rectangle, Circle }

#endregion