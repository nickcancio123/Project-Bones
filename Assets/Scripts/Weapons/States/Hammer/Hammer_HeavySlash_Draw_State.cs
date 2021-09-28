using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer_HeavySlash_Draw_State : Slash_Draw_State
{
    protected override void CreateNextState() => nextState = gameObject.AddComponent<Hammer_HeavySlash_Swing_State>();
}
