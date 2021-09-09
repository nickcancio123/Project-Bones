using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordController : WeaponController
{
    new protected void Start()
    {
        base.Start();   //Handles base class reference caching
    }
}
