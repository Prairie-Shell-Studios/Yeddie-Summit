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
        [SerializeField] private LayerMask growthMask;
        [SerializeField] private LayerMask splitMask;
        [SerializeField] private LayerMask destroyMask;

        [Header("Split Snowball Spawning")]
        [SerializeField] private FloatScriptableObject raycastHeight;
        [SerializeField] private LayerMask spawnMask;
        [SerializeField] [Min(0.01f)] private float hitForce = 5f;

        [Header("Despawning")]
        private SimpleTimer despawnTimer;
        [SerializeField] private float despawnTimerDuration = 3f;

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

            // stop movement
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            growthTimer.Reset();
        }

        public void OnObjectDespawn()
        {
            // TODO: add FX
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            // snowball is stationary (ie stuck)
            if (rb.velocity == Vector3.zero)
            {
                // start timer if not active
                if (!despawnTimer.IsActive)
                {
                    despawnTimer.Reset();
                    despawnTimer.Start();
                }
                // timer has expired so stop it and despawn object
                if (despawnTimer.HasExpired())
                {
                    despawnTimer.Stop();
                    OnObjectDespawn();
                }
            }
            // turn timer off when active
            else if (despawnTimer.IsActive)
            {
                despawnTimer.Stop();
            }
        }

        #endregion

        #region monobehaviour

        private void OnCollisionEnter(Collision collision)
        {
            if (IsInLayerMask(collision.gameObject, splitMask))
            {
                // handle "prop" and "player" collisions
                if (currentScale >= minSplitScale)
                {
                    // snowball splits
                    Vector3 hitVector = transform.position - collision.transform.position;
                    HandleSplit(hitVector);
                }
                else
                {
                    // snowball is destroyed
                    OnObjectDespawn();
                }
            }
            else if (IsInLayerMask(collision.gameObject, destroyMask))
            {
                // snowball is destroyed
                OnObjectDespawn();
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
                //Debug.Log("Growth collision exit");
                growthTimer.Stop();
            }
        }

        #endregion

        #region utility

        private Vector3[] GetSplitPositions(Vector3 hitDirection)
        {
            //Debug.Log("Getting Split Positions");
            Vector3[] newPos = new Vector3[2];
            RaycastHit[] raycastHits = new RaycastHit[2];

            float distanceFromOriginal = 0.75f * sphereCollider.radius;
            Vector3 placeDir = sphereCollider.radius * Vector3.Cross(Vector3.up, hitDirection).normalized;

            for (int snowball = 0; snowball < 2; snowball++)
            {
                newPos[snowball] = transform.position;
                newPos[snowball].x += (distanceFromOriginal * hitDirection.x) + placeDir.x;
                newPos[snowball].z += (distanceFromOriginal * hitDirection.z) + placeDir.z;

                Vector3 rayCastPos = new Vector3(newPos[snowball].x, raycastHeight.Value, newPos[snowball].z);
                // cast ray to determine y coords
                if (Physics.Raycast(rayCastPos, Vector3.down, out raycastHits[snowball], raycastHeight.Value, spawnMask))
                {
                    newPos[snowball].y = raycastHits[snowball].point.y;
                }
                else
                {
                    Debug.Log("Could not determine spawn height for split snowball due to no terrain underneath.");
                }

                placeDir = -placeDir; // reverse direction to place other snowball in opposite position
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
                    //Debug.Log("Growth incremented");
                    growthTimer.Reset();
                    currentScale += growthInc;
                    transform.localScale = currentScale * Vector3.one;
                }
            }
        }

        private void HandleSplit(Vector3 hitVector)
        {
            //Debug.Log("Split was called");
            float newScale = currentScale / 2f;
            Vector3[] splitPos = new Vector3[2];
            splitPos = GetSplitPositions(hitVector.normalized);
            for (int _ = 0; _ < 2; _++)
            {
                GameObject snowball = objectPooler.SpawnFromPool(
                    "snowball",
                    splitPos[_],
                    transform.rotation,
                    newScale * Vector3.one
                    );
                Vector3 hit = hitForce * (snowball.transform.position - transform.position);
                hit += Vector3.Scale(rb.velocity, gameObject.transform.localScale);
                Rigidbody snowball_rb = snowball.GetComponent<Rigidbody>();
                snowball_rb?.AddForce(hit, ForceMode.Impulse);
                // maintain some of the original velocity after collision
                snowball_rb.velocity += (rb.velocity / 3f);
                
            }
            OnObjectDespawn();
        }

        private void Init()
        {
            rb = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
            objectPooler = ObjectPooler.Instance;
            growthTimer = new SimpleTimer(TimerDirection.CountDown, growthRate);
            despawnTimer = new SimpleTimer(TimerDirection.CountDown, despawnTimerDuration);
        }

        #endregion
    }
}
