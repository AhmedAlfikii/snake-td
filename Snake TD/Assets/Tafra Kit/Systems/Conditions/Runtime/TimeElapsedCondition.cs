using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Conditions;
public class TimeElapsedCondition : TickingCondition
{
    [SerializeField] private float time;
    [SerializeField] private bool useUnscaledTime;

    private float timeElapsed;
    protected override void OnActivate()
    {
        timeElapsed = 0;

        base.OnActivate();
    }

    protected override bool PerformCheck()
    {
        if(timeElapsed < time)
        {
            if(useUnscaledTime)
                timeElapsed += Time.unscaledDeltaTime;
            else
                timeElapsed += Time.deltaTime;

            return false;
        }
        else
            return true;
    }
}
