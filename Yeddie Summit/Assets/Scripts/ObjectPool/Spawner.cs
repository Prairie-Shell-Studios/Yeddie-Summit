using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Spawner : MonoBehaviour
{
    #region fields

    public ObjectPool pool;
    public float spawnRate;

    #endregion

    #region monobehaviour

    void OnEnable() {}
    void OnDisable() {}
    void Awake() {}
    void Start() {}
    void Update() {}

    #endregion

    #region 

    protected abstract void StartSpawning();

    protected abstract void StopSpawning();

    #endregion

}
