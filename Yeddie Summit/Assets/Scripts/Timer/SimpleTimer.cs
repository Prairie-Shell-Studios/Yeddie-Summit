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

        /// <summary>
        /// Elapse the time if timer is active and return the value.
        /// </summary>
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

        public TimerDirection Direction { get => direction; }

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
        /// If the startTime is specified as negative, they will be inverted.
        /// </summary>
        /// <param name="startTime">A positive time value to start at.</param>
        public SimpleTimer(float startTime)
        {
            minTime = (startTime < 0) ? -startTime : startTime;
            currentTime = this.minTime;
        }

        /// <summary>
        /// A timer set to count up from startTime and expire at stopTime.
        /// If either start or stop time are negative, they will be inverted.
        /// </summary>
        /// <param name="startTime">A positive time value to start at.</param>
        /// <param name="stopTime">A positive time value to expire at.</param>
        public SimpleTimer(float startTime, float stopTime)
        {
            minTime = (startTime < 0) ? -startTime : startTime;
            maxTime = (stopTime < 0) ? -stopTime : stopTime;
            timeLimit = this.maxTime;
            currentTime = this.minTime;
        }

        /// <summary>
        /// A timer that will either count-up to, or count-down from the specified maxTime.
        /// The default minTime is 0.
        /// If the maxTime is specified as negative, they will be inverted.
        /// </summary>
        /// <param name="timerDirection">Whether the timer should count-up or count-down.</param>
        /// <param name="maxTime">A positive value for the maximum time of the timer.
        /// This is stop time for a count-up timer or start time for a count-down timer.</param>
        public SimpleTimer(TimerDirection timerDirection, float maxTime)
        {
            direction = timerDirection;
            this.maxTime = (maxTime < 0) ? -maxTime : maxTime;
            timeLimit = (direction == TimerDirection.CountUp) ? this.maxTime : this.minTime;
            currentTime = (direction == TimerDirection.CountUp) ? this.minTime : this.maxTime;
        }

        /// <summary>
        /// A timer that will either count-up to or count-down from the maxTime.
        /// The specified timer will either count-up from or count-down to the minTime.
        /// If either the minTime or maxTime values are negative, they will be inverted.
        /// </summary>
        /// <param name="timerDirection">Whether the timer should count up or down.</param>
        /// <param name="minTime">A positive value for the minimum time for the timer.
        /// This is start time for a count-up timer or stop time for a count-down timer.</param>
        /// <param name="maxTime">A positive value for the maximum time for the timer.
        /// This is stop time for a count-up timer or stop time for a count-down timer.</param>
        public SimpleTimer(TimerDirection timerDirection, float minTime, float maxTime)
        {
            direction = timerDirection;
            this.minTime = (minTime < 0) ? -minTime : minTime;
            this.maxTime = (maxTime < 0) ? -maxTime : maxTime;
            timeLimit = (direction == TimerDirection.CountUp) ? this.maxTime : this.minTime;
            currentTime = (direction == TimerDirection.CountUp) ? this.minTime : this.maxTime;
        }

        #endregion

        #region api

        /// <summary>
        /// Change the duration of the timer and handles the elapsed time change.
        /// If a negative value is specified, the value will be inverted.
        /// NOTE: Make sure to Reset timer if you want it to start at the correct time.
        /// </summary>
        /// <param name="duration">A positive value for the new duration that the timer will have</param>
        public void ChangeDuration(float duration)
        {
            float newDuration = (duration < 0) ? -duration : duration;
            float durationDifference = Duration() - newDuration;
            maxTime = newDuration;

            // increase the value of the current time for an active count-down timer
            if (isActive && direction == TimerDirection.CountDown)
            {
                currentTime += durationDifference;
            }
            else if (direction == TimerDirection.CountUp)
            {
                timeLimit = maxTime;
            }
        }

        /// <summary>
        /// Calculate the entire duration of the timer without considering elapsed time.
        /// </summary>
        /// <returns>A positive value representing total length of the timer.
        /// A -1 value representing an unlimited timer.</returns>
        public float Duration()
        {
            return (timeLimit >= 0) ? maxTime - minTime : -1;
        }

        /// <summary>
        /// Determine whether or not the time on the timer has reached its time limit.
        /// </summary>
        /// <returns>True if the time limit has been reached. 
        /// False if the time limit has not been reached.</returns>
        public bool HasExpired()
        {
            bool hasElapsed = false;

            ElapseTime();

            if (timeLimit >= 0 && currentTime >= timeLimit)
            {
                hasElapsed = true;
            }

            return hasElapsed;
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        /// <returns>A positive value representing the start time for the timer.</returns>
        public float Reset()
        {
            currentTime = (direction == TimerDirection.CountUp) ? minTime : maxTime;
            return currentTime;
        }

        /// <summary>
        /// Start the timer.
        /// </summary>
        /// <returns>A positive value representing the start time for the timer.</returns>
        public float Start()
        {
            isActive = true;
            return currentTime;
        }

        /// <summary>
        /// Get the start time of the timer.
        /// </summary>
        /// <returns>A positive value the represents what time the timer starts at.</returns>
        public float StartTime()
        {
            return (direction == TimerDirection.CountUp) ? minTime : maxTime;
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        /// <returns>A positive value representing the current time for the timer.</returns>
        public float Stop()
        {
            ElapseTime();
            isActive = false;
            return currentTime;
        }

        /// <summary>
        /// Get the stop time of the timer.
        /// </summary>
        /// <returns>A positive value representing the stop time for the timer.</returns>
        public float StopTime()
        {
            return (direction == TimerDirection.CountUp) ? maxTime : minTime;
        }

        override public string ToString()
        {
            return "Start: " + StartTime() + "\tCurrent: " + currentTime + "\tStop: " + StopTime() + "\n";
        }

        #endregion

        #region logic

        /// <summary>
        /// Elapse the timer in the appropriate direction, unless the time limit has been reached or the timer is not active.
        /// </summary>
        private void ElapseTime()
        { 
            if (isActive)
            {
                float tempTime = currentTime;
                Debug.Log("Is active");
                if (direction == TimerDirection.CountUp)
                {
                    Debug.Log("Adding time");
                    tempTime += Time.deltaTime;
                    currentTime = (tempTime < timeLimit) ? tempTime : timeLimit;
                }
                else
                {
                    Debug.Log("Decreasing time");
                    tempTime -= Time.deltaTime;
                    currentTime = (tempTime > timeLimit) ? tempTime : timeLimit;
                }
            }
        }

        #endregion
    }

    #region enums

    /// <summary>
    /// Classify a timer as either counting up or counting down.
    /// </summary>
    public enum TimerDirection { CountUp, CountDown }

    #endregion
}
