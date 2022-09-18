using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PraireShellStudios.Status;

public class StatusScriptableObjectTests
{
    #region fields

    static private int defaultMinValue = 0;
    static private int defaultMaxValue = 100;
    static private StatusBehaviour defaultBehaviour = StatusBehaviour.None;
    static private float defaultBehaviourRate = 5f;
    static private float testBehaviourRate = 0.25f;
    static private int testLoopCount = 50;

    static TestStatusSOWrapper[] statusSOWrappers =
    {
        new TestStatusSOWrapper(ScriptableObject.CreateInstance<StatusScriptableObject>(),
            defaultMaxValue, defaultMaxValue, defaultBehaviour, testBehaviourRate), // default status
        new TestStatusSOWrapper(ScriptableObject.CreateInstance<StatusScriptableObject>(),
            defaultMaxValue, defaultMaxValue, StatusBehaviour.Degrade, testBehaviourRate), // degrade status
        new TestStatusSOWrapper(ScriptableObject.CreateInstance<StatusScriptableObject>(),
            defaultMaxValue, defaultMinValue, StatusBehaviour.Regen, testBehaviourRate) // regen status
    };

    #endregion

    [Test]
    public void DefaultStatusTest()
    {
        var status = ScriptableObject.CreateInstance<StatusScriptableObject>();
        Assert.AreEqual(defaultMinValue, status.Min);
        Assert.AreEqual(defaultMaxValue, status.Max);
        Assert.AreEqual(defaultMaxValue, status.Current);
        Assert.AreEqual(defaultBehaviour, status.Behaviour);
        Assert.AreEqual(defaultBehaviourRate, status.BehaviourRate);
    }

    [Test]
    public void SettersTest()
    {
        var status = ScriptableObject.CreateInstance<StatusScriptableObject>();
        // max setter
        int newMax = 50;
        status.Max = newMax;
        Assert.AreEqual(newMax, status.Max);
        Assert.AreEqual(newMax, status.Current);
        // current setter
        status.Current = defaultMaxValue;
        Assert.AreEqual(newMax, status.Current);
        int newCurrent = -10;
        status.Current = newCurrent;
        Assert.AreEqual(defaultMinValue, status.Current);
        newCurrent = 15;
        status.Current = newCurrent;
        Assert.AreEqual(newCurrent, status.Current);
        // behaviour setter
        StatusBehaviour newBehaviour = StatusBehaviour.Regen;
        status.Behaviour = newBehaviour;
        Assert.AreEqual(newBehaviour, status.Behaviour);
        newBehaviour = StatusBehaviour.Degrade;
        status.Behaviour = newBehaviour;
        Assert.AreEqual(newBehaviour, status.Behaviour);
        status.Behaviour = StatusBehaviour.None;
        // behaviour rate setter
        float newRate = -1f;
        status.BehaviourRate = newRate;
        Assert.AreEqual(defaultMinValue, status.BehaviourRate);
        newRate = 10f;
        status.BehaviourRate = newRate;
        Assert.AreEqual(newRate, status.BehaviourRate);
    }

    [Test]
    public void ChangeCurrentTest()
    {
        var status = ScriptableObject.CreateInstance<StatusScriptableObject>();
        int changeAmount = 10;
        // change beyond max
        status.ChangeCurrent(changeAmount);
        Assert.AreEqual(defaultMaxValue, status.Current);
        // change valid amount
        status.ChangeCurrent(-changeAmount);
        Assert.AreEqual(defaultMaxValue - changeAmount, status.Current);
        // change beyond min
        status.ChangeCurrent(-status.Max);
        Assert.AreEqual(defaultMinValue, status.Current);
    }

    [UnityTest]
    public IEnumerator StatusTests([ValueSourceAttribute(nameof(statusSOWrappers))] TestStatusSOWrapper statusWrapper)
    {
        statusWrapper.Status.Init();
        for (int count = 0; count < testLoopCount; count++)
        {
            yield return null;
            statusWrapper.Status.HandleBehaviour();
            if (statusWrapper.Behaviour == StatusBehaviour.None)
            {
                Assert.AreEqual(statusWrapper.Current, statusWrapper.Status.Current);
            }
        }
        if (statusWrapper.Behaviour == StatusBehaviour.Regen)
        {
            Assert.Greater(statusWrapper.Status.Current, statusWrapper.Current);
        }
        else if (statusWrapper.Behaviour == StatusBehaviour.Degrade)
        {
            Assert.Less(statusWrapper.Status.Current, statusWrapper.Current);
        }
        statusWrapper.Status.Behaviour = StatusBehaviour.None;
    }
}

#region wrapper class

/// <summary>
/// Allows the parameterization of tests for StatusScriptableObject.
/// </summary>
public class TestStatusSOWrapper
{
    #region fields

    private StatusScriptableObject status;
    private int max;
    private int current;
    private StatusBehaviour behaviour;
    private float behaviourRate;

    #endregion

    #region constructor(s)

    public TestStatusSOWrapper(StatusScriptableObject status, int max, int current,
        StatusBehaviour behaviour, float rate)
    {
        this.status = status;
        this.max = max;
        this.status.Max = this.max;
        this.current = current;
        this.status.Current = this.current;
        this.behaviour = behaviour;
        this.status.Behaviour = this.behaviour;
        this.behaviourRate = rate;
        this.status.BehaviourRate = this.behaviourRate;
    }

    #endregion

    #region properties

    public StatusScriptableObject Status { get => status; }
    
    public int Max { get => max; }
    
    public StatusBehaviour Behaviour { get => behaviour; }
    
    public float BehaviourRate { get => behaviourRate; }
    public int Current { get => current; }

    #endregion
}

#endregion
