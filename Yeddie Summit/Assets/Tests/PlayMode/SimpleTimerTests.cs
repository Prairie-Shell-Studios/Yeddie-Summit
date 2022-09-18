using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrairieShellStudios.Timer;

public class SimpleTimerTests
{
    #region fields

    static private float defaultStartTime = 0f;
    static private float testDuration = 1f;
    static private float testStartTime = 1f;
    static private float unlimitedStopTime = -1f;

    static TestTimerWrapper[] timerWrappers =
    {
        new TestTimerWrapper(new SimpleTimer(), false, TimerDirection.CountUp,
            defaultStartTime, unlimitedStopTime), // unlimited default
        new TestTimerWrapper(
            new SimpleTimer(testStartTime),
            false, TimerDirection.CountUp, testStartTime, unlimitedStopTime), // unlimited with start time
        new TestTimerWrapper(
            new SimpleTimer(defaultStartTime, defaultStartTime + testDuration),
            true, TimerDirection.CountUp, defaultStartTime, defaultStartTime + testDuration), // count up with limit
        new TestTimerWrapper(
            new SimpleTimer(TimerDirection.CountDown, testDuration),
            true, TimerDirection.CountDown, testDuration, defaultStartTime), //countdown with limit
        new TestTimerWrapper(
                new SimpleTimer(TimerDirection.CountUp, testStartTime, testStartTime + testDuration),
                true, TimerDirection.CountUp, testStartTime, testStartTime + testDuration), // count up with limit and range
        new TestTimerWrapper(
                new SimpleTimer(TimerDirection.CountDown, testStartTime, testStartTime + testDuration),
                true, TimerDirection.CountDown, testStartTime + testDuration, testStartTime) // count down with limit and range
    };

    #endregion

    #region tests

    [Test]
    public void CountDownwithDurationChangeTest()
    {
        // create a countdown timer
        SimpleTimer timer = new SimpleTimer(TimerDirection.CountDown, testDuration);
        // increase the duration
        float newDuration = 2f * testDuration;
        timer.ChangeDuration(newDuration);
        timer.Reset();
        Assert.AreEqual(newDuration, timer.MaxTime);
        Assert.AreEqual(newDuration, timer.CurrentTime);
        // decrease the duration
        newDuration = testDuration / 2f;
        timer.ChangeDuration(newDuration);
        timer.Reset();
        Assert.AreEqual(newDuration, timer.MaxTime);
        Assert.AreEqual(newDuration, timer.CurrentTime);
    }

    [Test]
    public void CountUpWithDurationChangeTest()
    {
        // create an unlimited timer
        SimpleTimer timer = new SimpleTimer();
        // change the duration
        timer.ChangeDuration(testDuration);
        Assert.AreEqual(testDuration, timer.StopTime());
        Assert.AreEqual(defaultStartTime, timer.CurrentTime);
    }

    [Test]
    public void NegativeDurationChangeTest()
    {
        // create a default timer
        SimpleTimer testTimer = new SimpleTimer();
        // change the duration to be a negative value
        testTimer.ChangeDuration(-2 * testDuration);
        Assert.AreEqual(2 * testDuration, testTimer.MaxTime);
        // create a countdown timer
        testTimer = new SimpleTimer(TimerDirection.CountDown, defaultStartTime, testDuration);
        // change the duration to be a negative value
        testTimer.ChangeDuration(-2 * testDuration);
        Assert.AreEqual(2 * testDuration, testTimer.MaxTime);
    }

    [Test]
    public void NegativeTimersTest()
    {
        // negative start time
        SimpleTimer testTimer = new SimpleTimer(-testStartTime);
        Assert.AreEqual(testStartTime, testTimer.MinTime);
        // negative start and negative stop time
        testTimer = new SimpleTimer(-testStartTime, -testStartTime - testDuration);
        Assert.AreEqual(testStartTime, testTimer.MinTime);
        Assert.AreEqual(testStartTime + testDuration, testTimer.MaxTime);
        // count-down: negative start and negative stop time
        testTimer = new SimpleTimer(TimerDirection.CountDown, -testStartTime, -testStartTime - testDuration);
        Assert.AreEqual(testStartTime, testTimer.MinTime);
        Assert.AreEqual(testStartTime + testDuration, testTimer.MaxTime);
    }

    [UnityTest]
    public IEnumerator TimerTests([ValueSourceAttribute(nameof(timerWrappers))] TestTimerWrapper timerWrapper)
    {
        SimpleTimer timer = timerWrapper.Timer;
        float startTime = timerWrapper.StartTime;
        float stopTime = timerWrapper.StopTime;
        bool hasLimit = timerWrapper.HasLimit;
        TimerDirection direction = timerWrapper.Direction;

        Assert.NotNull(timer);
        // start timer
        Assert.AreEqual(startTime, timer.CurrentTime);
        // verify durations
        if (!hasLimit)
        {
            Assert.True(timer.Duration() < 0);
        }
        else
        {
            Assert.AreEqual(testDuration, timer.Duration());
        }
        // verify start and stop times with min and max times
        if (direction == TimerDirection.CountUp)
        {
            if (hasLimit)
            {
                Assert.AreEqual(stopTime, timer.MaxTime);
            }
            Assert.AreEqual(startTime, timer.MinTime);
        }
        else
        {
            Assert.AreEqual(stopTime, timer.MinTime);
            Assert.AreEqual(startTime, timer.MaxTime);
        }

        Assert.AreEqual(startTime, timer.Start());
        // wait for a frame
        yield return null;
        // stop timer
        float recordedStopTime = timer.Stop();
        Assert.IsFalse(timer.HasExpired());

        yield return null;
        Assert.AreEqual(recordedStopTime, timer.CurrentTime);
        // start timer again and go until expired
        if (hasLimit)
        {
            timer.Start();
            float elapsedTime = 0f;
            while ((elapsedTime += Time.deltaTime) < testDuration)
            {
                yield return null;
                timer.HasExpired();
            }
            // wait for remainder of duration
            Assert.IsTrue(timer.HasExpired());
            Assert.AreEqual(stopTime, timer.CurrentTime);
            // stop timer one last time
            timer.Stop();
        }
        // reset timer
        timer.Reset();
        Assert.AreEqual(startTime, timer.CurrentTime);
    }

    #endregion
}

#region wrapper class
/// <summary>
/// Allows the parameterization of tests for the SimpleTimer.
/// </summary>
public class TestTimerWrapper
{
    #region fields

    private SimpleTimer timer;
    private bool hasLimit;
    private TimerDirection direction;
    private float startTime;
    private float stopTime;

    #endregion

    #region constructor(s)

    public TestTimerWrapper(SimpleTimer testTimer)
    {
        timer = testTimer;
        direction = testTimer.Direction;
        hasLimit = (Direction == TimerDirection.CountUp) ? testTimer.MaxTime != -1 : true;
        startTime = testTimer.StartTime();
        stopTime = testTimer.StopTime();
    }

    public TestTimerWrapper(SimpleTimer testTimer, bool hasLimit, TimerDirection direction, float startTime, float stopTime)
    {
        timer = testTimer;
        this.direction = direction;
        this.HasLimit = hasLimit;
        this.startTime = startTime;
        this.stopTime = stopTime;
    }

    #endregion
    
    #region properties

    public SimpleTimer Timer { get => timer; set => timer = value; }

    public bool HasLimit { get => hasLimit; set => hasLimit = value; }
        
    public TimerDirection Direction { get => direction; set => direction = value; }

    public float StartTime { get => startTime; set => startTime = value; }

    public float StopTime { get => stopTime; set => stopTime = value; }

    #endregion
}

#endregion