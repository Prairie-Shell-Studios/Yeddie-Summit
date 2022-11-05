using PrairieShellStudios.ScriptableObjects;
using PrairieShellStudios.Timer;
using UnityEngine;

namespace PrairieShellStudios
{
    public class Snowball : MonoBehaviour, IPooledObject
    {
        #region fields

        private Rigidbody rb;
        private SphereCollider sphereCollider;
        private ObjectPooler objectPooler;
        private SimpleTimer growthTimer;
        [SerializeField] [Min(0f)] private float growthRate = 5f;
        [SerializeField] [Min(0.01f)] private float growthInc = 0.1f;
        [SerializeField] [Min(0.01f)] private float initialScale = 0.5f;
        [SerializeField] [Min(0.01f)] private float maxScale = 2f;
        private float currentScale;
        [SerializeField] [Min(0.01f)] private float minSplitScale = 1.0f;
        // TODO: create tag filter https://www.brechtos.com/tagselectorattribute/
        [SerializeField] private LayerMask growthMask;
        [SerializeField] private LayerMask destroyMask;

        [Header("Split Snowball Spawning")]
        [SerializeField] private FloatScriptableObject raycastHeight;
        [SerializeField] private LayerMask spawnMask;

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
            if (IsInLayerMask(collision.gameObject, destroyMask))
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
                    OnObjectDespawn();
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (IsInLayerMask(collision.gameObject, growthMask))
            {
                HandleGrowth();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // stop the timer whenever exiting a collision with "terrain"
            if (IsInLayerMask(collision.gameObject, growthMask))
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

        private Vector3[] GetSplitPositions()
        {
            Vector3[] newPos = new Vector3[2];
            float distanceFromOther = 0.5f * sphereCollider.radius;
            float distanceFromOriginal = sphereCollider.radius + distanceFromOther;

            // cast ray to determine y coords
            RaycastHit[] raycastHits = new RaycastHit[2];
            for (int snowball = 0; snowball < 2; snowball++)
            {

            }

            return newPos;
        }

        private bool IsInLayerMask(GameObject checkGO, LayerMask mask)
        {
            // TODO: Fix to check a LayerMask with multiple layers
            return (mask & (1 << checkGO.layer)) != 0;
        }

        private void HandleGrowth()
        {
            if (currentScale >= maxScale)
            {
                if (growthTimer.IsActive)
                {
                    growthTimer.Stop();
                }
            }
            else
            {
                if (!growthTimer.IsActive)
                {
                    growthTimer.Start();
                }

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

        private void HandleSplit()
        {
            Debug.Log("Split was called");
            float newScale = currentScale / 2f;
            Vector3[] splitPos = new Vector3[2];
            splitPos = GetSplitPositions();
            for (int _ = 0; _ < 2; _++)
            {
                objectPooler.SpawnFromPool(
                    "snowball",
                    splitPos[_],
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
            sphereCollider = GetComponent<SphereCollider>();
            objectPooler = ObjectPooler.Instance;
            growthTimer = new SimpleTimer(TimerDirection.CountDown, growthRate);
            Debug.Log(growthTimer.Duration());
        }

        #endregion
    }
}
