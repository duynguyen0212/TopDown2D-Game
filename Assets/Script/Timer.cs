using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float nextAttackTime;
    public bool isCoolingDown => Time.time < nextAttackTime;
    
    public void StartCoolDown(float cooldownTime) => nextAttackTime = Time.time + cooldownTime;
}
