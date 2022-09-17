using UnityEngine;

namespace PrairieShellStudios.Timer
{
    /// <summary>
    /// A simple timer that can count either up or down.
    /// It can be set to have a time limit or to go indefinitely.
    /// Only a CountUp timer may go on indefinitely.
    /// </summary>
    public class SimpleTimer
    {
        #region fields

        private float maxTime = 60f;
        private float minTime = 0f;
        private float timeLimit = -1f;
        private float currentTime = 0f;
        private bool isActive = false;
        private TimerDirection direction = TimerDirection.CountUp;

        #endregion

        #region properties

        public float CurrentTime 
        { 
            get 
            { 
                ElapseTime(); 
                return currentTime; 
            } 
        }

        public float MaxTime { get => maxTime; }
        
        public float MinTime { get => minTime; }


        #endregion

        #region constructor(s)

        /// <summary>
        /// A default timer is set to count up from 0 and continue indefinitely.
        /// </summary>
        public SimpleTimer()
        {
        }

        /// <summary>
        /// A timer set to count up from startTime and continue indefinitely.
        /// </summary>
        /// <param name="startTime">The time that the timer will start at.</param>
        public SimpleTimer(float startTime)
        {
            minTime = startTime;
            currentTime = minTime;
        }

        /// <summary>
        /// A timer set to count up from startTime and expire at stopTime.
        /// </summary>
        /// <param name="startTime">The time to start at.</param>
        /// <param name="stopTime">The time to expire at.</param>
        public SimpleTimer(float startTime, float stopTime)
        {
            minTime = startTime;
            maxTime = stopTime;
            timeLimit = maxTime;
            currentTime = minTime;
        }

        /// <summary>
        /// A timer that will either countup to or countdown from the specified maxTime.
        /// The default minTime is 0.
        /// </summary>
        /// <param name="timerDirection">Whether the timer should count up or down.</param>
        /// <param name="maxTime">The maximum time for the timer.</param>
        public SimpleTimer(TimerDirection timerDirection, float maxTime)
        {
            direction = timerDirection;
            this.maxTime = maxTime;
            timeLimit = (direction == TimerDirection.CountUp) ? maxTime : minTime;
            currentTime = (direction == TimerDirection.CountUp) ? minTime : maxTime;
        }

        /// <summary>
        /// A timer that will either count up to or down from the maxTime.
        /// The specified timer will either count from or to the minTime.
        /// </summary>
        /// <param name="timerDirection">Whether the timer should count up or down.</param>
        /// <param name="minTime">The minimum time for the timer.</param>
        /// <param name="maxTime">The maximum time for the timer.</param>
        public SimpleTimer(TimerDirection timerDirection, float minTime, float maxTime)
        {
            direction = timerDirection;
            this.minTime = minTime;
            this.maxTime = maxTime;
            timeLimit = (direction == TimerDirection.CountUp) ? maxTime : minTime;
            currentTime = (direction == TimerDirection.CountUp) ? minTime : maxTime;
        }

        #endregion

        #region api

        /// <summary>
        /// Calculates the entire duration of the timer without considering elapsed time.
        /// </summary>
        /// <returns>The total length of the timer.</returns>
        public float Duration()
        {
            return (timeLimit >= 0) ? maxTime - minTime : -1;
        }

        /// <summary>
        /// Determines whether or not the time on the timer has reached its time limit.
        /// </summary>
        /// <returns>True if the time limit has been reached. 
        /// False if the time limit has not been reached.</returns>
        public bool HasExpired()
        {
            bool hasElapsed = false;

            if (timeLimit >= 0 && CurrentTime >= timeLimit)
            {
                hasElapsed = true;
            }

            return hasElapsed;
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        /// <returns>The start time for the timer.</returns>
        public float Reset()
        {
            currentTime = (direction == TimerDirection.CountUp) ? minTime : maxTime;
            return currentTime;
        }

        /// <summary>
        /// Begins the timer.
        /// </summary>
        /// <returns>The start time for the timer.</returns>
        public float Start()
        {
            isActive = true;
            return currentTime;
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <returns>The current time for the timer.</returns>
        public float Stop()
        {
            ElapseTime();
            isActive = false;
            return currentTime;
        }

        #endregion

        #region logic

        /// <summary>
        /// Elapses the timer in the appropriate direction unless the time limit has been reached or the timer is not active.
        /// </summary>
        private void ElapseTime()
        { 
            if (isActive)
            {
                float tempTime = currentTime;

                if (direction == TimerDirection.CountUp)
                {
                    tempTime += Time.deltaTime;
                    currentTime = (tempTime < timeLimit) ? tempTime : timeLimit;
                }
                else
                {
                    tempTime -= Time.deltaTime;
                    currentTime = (tempTime > timeLimit) ? tempTime : timeLimit;
                }
            }
        }

        #endregion
    }

    #region enums

    /// <summary>
    /// Classifies a timer as either counting up or counting down.
    /// </summary>
    public enum TimerDirection { CountUp, CountDown }

    #endregion
}
