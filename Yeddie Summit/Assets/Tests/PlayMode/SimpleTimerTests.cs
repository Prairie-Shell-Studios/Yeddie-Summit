using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PrairieShellStudios.Timer;

public class SimpleTimerTests
{

    private float stopWaitTime = 0.25f;

    [Test]
    public void UnlimitedDefaultTimerTest()
    {
        SimpleTimer timer = new SimpleTimer();
        Assert.NotNull(timer);
        TimerTest(timer, false, 0f, -1f, stopWaitTime);
    }

    [Test]
    public void UnlimitedTimerWithStartTimeTest()
    {
        SimpleTimer timer = new SimpleTimer();
        Assert.NotNull(timer);
        TimerTest(timer, false, 1f, -1f, stopWaitTime);
    }

    [Test]
    public void TimerWithLimitTest()
    {
        SimpleTimer timer = new SimpleTimer(1f, 1.25f);
        Assert.NotNull(timer);
        TimerTest(timer, true, 1f, 1.25f, 0.125f);
    }


    [Test]
    public void CountUpTimerWithLimitTest()
    {
        SimpleTimer timer = new SimpleTimer(TimerDirection.CountUp, 0.25f);
        Assert.NotNull(timer);
        TimerTest(timer, true, 0f, 0.25f, 0.125f);
    }

    [Test]
    public void CountDownTimerWithLimitTest()
    {
        SimpleTimer timer = new SimpleTimer(TimerDirection.CountDown, 0.25f);
        Assert.NotNull(timer);
        TimerTest(timer, true, 0.25f, 0f, 0.125f);
    }

    [Test]
    public void CountUpTimerWithLimitAndRangeTest()
    {
        SimpleTimer timer = new SimpleTimer(TimerDirection.CountUp, 1f, 1.25f);
        Assert.NotNull(timer);
        TimerTest(timer, true, 1f, 1.25f, 0.125f);
    }

    [Test]
    public void CountDownTimerWithLimitAndRangeTest()
    {
        SimpleTimer timer = new SimpleTimer(TimerDirection.CountDown, 1.25f, 1f);
        Assert.NotNull(timer);
        TimerTest(timer, true, 1.25f, 1f, 0.125f);
    }

    public IEnumerator TimerTest(SimpleTimer timer, bool hasLimit, float startTime, float stopTime, float waitTime)
    {
        // start timer
        Assert.AreEqual(timer.CurrentTime, startTime);
        Assert.AreEqual(timer.Start(), startTime);
        // wait for half timer duration
        yield return new WaitForSeconds(waitTime);
        Assert.IsFalse(timer.HasExpired());
        // stop timer
        float recordedStopTime = timer.Stop();
        yield return new WaitForSeconds(stopWaitTime);
        Assert.AreEqual(recordedStopTime, timer.CurrentTime);
        // start timer again and go until expired
        if (hasLimit)
        {
            // wait for remainder of duration
            yield return new WaitForSeconds(waitTime);
            Assert.IsTrue(timer.HasExpired());
            Assert.AreEqual(timer.CurrentTime, stopTime);
            // stop timer one last time
            timer.Stop();
        }
        // reset timer
        timer.Reset();
        Assert.AreEqual(timer.CurrentTime, 0f);
    }

}
