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

    [SerializeField] private ObjectPool pool;
    [SerializeField] private float rate = 5f;
    [SerializeField] private bool spawnOnStart = true;
    private bool isActive = false;
    SimpleTimer timer;

    [Header("Randomized Position")]
    [SerializeField] private bool randomizePosition = false;
    [SerializeField] [Min(0)] private float width = 10f; // x
    [SerializeField] [Min(0)] private float length = 10f; // z
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
                pool?.SpawnObject(GetRandomTransform());
                timer.Reset();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showBounds)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(2 * width, 0f, 2 * length));
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

    protected Transform GetRandomTransform()
    {
        Transform randomTransform = this.transform;

        if (randomizePosition)
        {
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(minMaxX.Item1, minMaxX.Item2), 
                randomTransform.position.y, UnityEngine.Random.Range(minMaxZ.Item1, minMaxZ.Item2));
            randomTransform.position = randomPosition;
        }

        return randomTransform;
    }

    #endregion
}
