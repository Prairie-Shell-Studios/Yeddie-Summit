using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrairieShellStudios.Timer;
using System;


namespace PraireShellStudios.Status
{
    /// <summary>
    /// A ScriptableObject that has a min, max, and current value.
    /// The current value may change over time dependending on the specified behaviour.
    /// e.g. Health that regens or Hunger that degrades.
    /// </summary>
    [CreateAssetMenu(fileName = "Status", menuName = "ScriptableObjects/Status", order = 1)]
    public class StatusScriptableObject : ScriptableObject
    {
        #region fields

        [SerializeField]
        private string statusName = "default";
        [SerializeField]
        private int max = 100;
        [SerializeField]
        private int current = 100;
        [SerializeField]
        private int min = 0;

        private SimpleTimer timer = new SimpleTimer();
        [SerializeField]
        private StatusBehaviour behaviour = StatusBehaviour.None;
        [SerializeField]
        private float behaviourRate = 5f;

        #endregion

        #region properties

        public int Max
        {
            get => max;
            set
            {
                current = (current == max) ? value : current;
                max = value;
            }
        }

        public int Min { get => min; }

        public int Current
        {
            get => current;
            set 
            {
                if (value <= max && value >= min)
                {
                    current = value;
                }
                else
                {
                    current = (value > max) ? max : min;
                }
            }
        }

        /// <summary>
        /// The direction in which current value should be changed.
        /// </summary>
        public StatusBehaviour Behaviour
        {
            get => behaviour;
            set
            {
                timer.Stop();
                timer.Reset();
                
                if (value != StatusBehaviour.None)
                {
                    timer.Start();
                }

                behaviour = value;
            }
        }

        /// <summary>
        /// The duration of the timer used to change the current value.
        /// </summary>
        public float BehaviourRate
        {
            get => behaviourRate;
            set
            {
                behaviourRate = (value < 0) ? 0 : value;
                timer.ChangeDuration(behaviourRate);
            }
        }

        public string StatusName { get => statusName; }

        #endregion

        #region api

        /// <summary>
        /// Start the timer to change the current value over time.
        /// Should be called only once in an Awake or Start method.
        /// </summary>
        public void Init()
        {
            if (behaviour != StatusBehaviour.None)
            {
                if (timer.Duration() != behaviourRate)
                {
                    timer.ChangeDuration(behaviourRate);
                }
                timer.Start();
            }
        }

        /// <summary>
        /// Handle the timer to change the current value over time.
        /// Should be called in an Update method.
        /// </summary>
        public void HandleBehaviour()
        {
            if (timer.HasExpired())
            {
                if (behaviour == StatusBehaviour.Regen)
                {
                    ChangeCurrent(1);
                }
                else if (behaviour == StatusBehaviour.Degrade)
                {
                    ChangeCurrent(-1);
                }
                timer.Reset();
            }
        }

        /// <summary>
        /// Change the current value by the specified amount.
        /// </summary>
        /// <param name="amount">A value amount to change the current value.</param>
        /// <returns>The new current value.</returns>
        public int ChangeCurrent(int amount)
        {
            Current += amount;
            return current;
        }

        #endregion
        
    }

    #region enums

    /// <summary>
    /// Classify behaviour of the direction the current value changes over time.
    /// </summary>
    public enum StatusBehaviour { None, Regen, Degrade };

    #endregion
}
