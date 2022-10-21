using PrairieShellStudios.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrairieShellStudios
{
    public class Snowball : MonoBehaviour, IPooledObject
    {
        #region fields

        private Rigidbody rb;
        private ObjectPooler objectPooler;
        private SimpleTimer growthTimer;
        [SerializeField] [Min(0f)] private float growthRate = 5f;
        [SerializeField] [Min(0.01f)] private float growthInc = 0.1f;
        [SerializeField] [Min(0.01f)] private float initialScale = 0.5f;
        private float currentScale;
        [SerializeField] [Min(0.01f)] private float minSplitScale = 1.5f;
        // TODO: create tag filter https://www.brechtos.com/tagselectorattribute/
        [SerializeField] private string growthTagFilter;
        [SerializeField] private string destroyTagFilter;

        #endregion

        #region IPooledObject interface

        public void OnObjectSpawn(int spawnCase)
        {
            if (spawnCase == 0)
            {
                transform.localScale = initialScale * Vector3.one;
                currentScale = initialScale;
                if (objectPooler == null)
                {
                    Init();
                }
            }
            else
            {
                currentScale = transform.localScale.x;
            }
            growthTimer.Reset();
        }

        public void OnObjectDespawn()
        {
            // TODO: add FX
            gameObject.SetActive(false);
        }

        #endregion

        #region monobehaviour

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(growthTagFilter))
            {
                Debug.Log("Growth collision enter");
                // start growth timer if tag is "terrain"
                growthTimer.Start();
            }
            else if (collision.gameObject.CompareTag(destroyTagFilter))
            {
                Debug.Log("Destroy collision enter");
                // handle "prop" and "player" collisions
                if (currentScale >= minSplitScale)
                {
                    // snowball splits
                    HandleSplit();
                }
                else
                {
                    // snowball is destroyed
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag(growthTagFilter))
            {
                Debug.Log("Growth collision stay");
                // keep growth timer going if tag is the filterTag
                if (growthTimer.HasExpired())
                {
                    Debug.Log("Growth incremented");
                    growthTimer.Reset();
                    currentScale += growthInc;
                    transform.localScale = currentScale * Vector3.one;
                }
            }

        }

        private void OnCollisionExit(Collision collision)
        {
            // stop the timer whenever exiting a collision with "terrain"
            if (collision.gameObject.CompareTag(growthTagFilter))
            {
                Debug.Log("Growth collision exit");
                growthTimer.Stop();
            }
        }

        private void FixedUpdate()
        {
            // TODO: add velocity (maybe not)   
            Debug.Log(gameObject.activeSelf);
        }

        #endregion

        #region utility


        private void HandleSplit()
        {
            Debug.Log("Split was called");
            float newScale = currentScale / 2f;
            for (int _ = 0; _ < 2; _++)
            {
                objectPooler.SpawnFromPool(
                    "snowball",
                    transform.position,
                    transform.rotation,
                    newScale * Vector3.one
                    );
                // TODO: add force to new spawns
            }
            OnObjectDespawn();
        }

        private void Init()
        {
            rb = GetComponent<Rigidbody>();
            objectPooler = ObjectPooler.Instance;
            growthTimer = new SimpleTimer(TimerDirection.CountDown, growthRate);
            Debug.Log(growthTimer.Duration());
        }

        #endregion
    }
}
