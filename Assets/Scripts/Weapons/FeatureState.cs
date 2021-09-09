using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureState : MonoBehaviour
{
    protected bool isActiveState = false;

    public virtual void BeginState()
    {

    }

    //Will contain the behavior of each state
    public abstract void Behave();


    protected virtual void EndState()
    {

    }
}
