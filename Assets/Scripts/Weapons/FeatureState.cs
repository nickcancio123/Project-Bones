using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeatureState : MonoBehaviour
{
    public abstract void BeginState();
    public abstract void Behave();
    protected abstract void EndState();
}
