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
        private SimpleTimer growTimer;
        [SerializeField] [Min(0f)] private float growthRate = 5f;
        [SerializeField] [Min(0.01f)] private float initialScale = 0.5f;
        [SerializeField] [Min(0.01f)] private float minSplitScale = 1.5f;
        // TODO: create tag filter https://www.brechtos.com/tagselectorattribute/
        [SerializeField] private string[] tagFilter = new string[] { }; 
        [SerializeField] [Min(1)] private int maxCollisions = 1;


        #endregion

        #region IPooledObject interface

        public void OnObjectSpawn()
        {
            if (transform.position.y != 0)
            {
                transform.localScale = initialScale * Vector3.one;
            }
            growTimer.Reset();
            growTimer.Start();
        }

        #endregion

        #region monobehaviour

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            objectPooler = ObjectPooler.Instance;
            growTimer = new SimpleTimer(TimerDirection.CountDown, growthRate);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // TODO: start growth timer if tag is "terrain"
            // TODO: handle "prop" and "player" collisions
        }


        private void OnCollisionStay(Collision collision)
        {
            // TODO: keep growth timer going if tag is "terrain"
        }

        private void OnCollisionExit(Collision collision)
        {
            // TODO: stop the timer whenever exiting a collision with "terrain"
        }

        private void FixedUpdate()
        {
            // TODO: add velocity   
        }

        #endregion
    }
}
