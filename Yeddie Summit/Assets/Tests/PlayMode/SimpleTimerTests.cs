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
    static private float stopWaitTime;
    static private float testDuration = 0.25f;
    static private float testStartTime = 1f;
    static private float unlimitedStopTime = -1f;

    static private List<TestTimerWrapper> listTimerWrappers = new List<TestTimerWrapper>();

    static TestTimerWrapper[] GetTimerWrappers() 
    {
        stopWaitTime = testDuration / 2f;

        UnlimitedDefault();
        UnlimitedWithStartTime();
        CountUpWithLimit();
        CountDownWithLimit();
        CountUpWithLimitAndRange();
        CountDownWithLimitAndRange();

        return listTimerWrappers.ToArray(); 
    }

    #endregion

    #region setup and teardowns

    [OneTimeSetUp]
    public void SetUp()
    {
        stopWaitTime = testDuration / 2f;
    }

    #endregion

    #region static methods

    static void UnlimitedDefault()
    {
        Debug.Log("running unlimited default");
        listTimerWrappers.Add(new TestTimerWrapper(new SimpleTimer(), false, TimerDirection.CountUp, 
            defaultStartTime, unlimitedStopTime));
    }

     static void UnlimitedWithStartTime()
     {
        Debug.Log("running unlimited with start time");
        listTimerWrappers.Add(
            new TestTimerWrapper(
            new SimpleTimer(testStartTime), 
            false, TimerDirection.CountUp, testStartTime, unlimitedStopTime));
     }

     static void CountUpWithLimit()
     {
        Debug.Log("running countup with limit");
        listTimerWrappers.Add(
            new TestTimerWrapper(
            new SimpleTimer(defaultStartTime, defaultStartTime + testDuration), 
            true, TimerDirection.CountUp, defaultStartTime, defaultStartTime + testDuration));
     }

     static void CountDownWithLimit()
     {
        Debug.Log("running countdown with limit");
        listTimerWrappers.Add(
            new TestTimerWrapper(
            new SimpleTimer(TimerDirection.CountDown, testDuration),
            true, TimerDirection.CountDown, testDuration, defaultStartTime));
     }

     static void CountUpWithLimitAndRange()
     {
        Debug.Log("running countup with limit and range");
        listTimerWrappers.Add(
            new TestTimerWrapper(
                new SimpleTimer(TimerDirection.CountUp, testStartTime, testStartTime + testDuration),
                true, TimerDirection.CountUp, testStartTime, testStartTime + testDuration));
     }

     static void CountDownWithLimitAndRange()
     {
        Debug.Log("running countdown with limit and range");
        listTimerWrappers.Add(
            new TestTimerWrapper(
                new SimpleTimer(TimerDirection.CountDown, testStartTime + testDuration, testStartTime),
                true, TimerDirection.CountDown, testStartTime + testDuration, testStartTime));
     }

    /*[Test]
    public void CountUpWithDurationChangeTest()
    {
        // create an unlimited timer
        SimpleTimer timer = new SimpleTimer();
        TimerTests(timer, false, TimerDirection.CountUp, defaultStartTime, unlimitedStopTime);
        // change the duration
        timer.ChangeDuration(testDuration);
        TimerTests(timer, true, TimerDirection.CountUp, defaultStartTime, testDuration);
    }

    [Test]
    public void CountDownwithDurationChangeTest()
    {
        // create a countdown timer
        SimpleTimer timer = new SimpleTimer(TimerDirection.CountDown, testDuration);
        // increase the duration
        testDuration *= 2f;
        timer.ChangeDuration(testDuration);
        TimerTests(timer, true, TimerDirection.CountDown, testDuration, defaultStartTime);
        // decrease the duration
        testDuration /= 2f;
        timer.ChangeDuration(testDuration);
        TimerTests(timer, true, TimerDirection.CountDown, testDuration, defaultStartTime);
    }*/

    #endregion

    #region tests

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

    [UnityTest, Sequential]
    public IEnumerator TimerTests([ValueSourceAttribute(nameof(GetTimerWrappers))] TestTimerWrapper timerWrapper)
    {
        Debug.Log("New Timer");
        SimpleTimer timer = timerWrapper.Timer;
        float startTime = timerWrapper.StartTime;
        float stopTime = timerWrapper.StopTime;
        bool hasLimit = timerWrapper.HasLimit;
        TimerDirection direction = timerWrapper.Direction;

        Assert.NotNull(timer);
        Debug.Log("Start Time = " + timer.ToString());
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
        // wait for half timer duration
        yield return new WaitForSeconds(stopWaitTime);
        Assert.IsFalse(timer.HasExpired());
        // stop timer
        Debug.Log("First stop = " + timer.ToString());
        float recordedStopTime = timer.Stop();
        /*Debug.Log(timer.ToString());*/

        yield return new WaitForSeconds(stopWaitTime);
        Assert.AreEqual(recordedStopTime, timer.CurrentTime);
        // start timer again and go until expired
        if (hasLimit)
        {
            timer.Start();
            // wait for remainder of duration
            yield return new WaitForSeconds(stopWaitTime);
            Assert.IsTrue(timer.HasExpired());
            Assert.AreEqual(stopTime, timer.CurrentTime);
            // stop timer one last time
            timer.Stop();
            Debug.Log("Last Stop = " + timer.ToString());
        }
        // reset timer
        timer.Reset();
        Assert.AreEqual(startTime, timer.CurrentTime);
    }

    #endregion

}

#region class(es)

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