using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoostData", menuName = "Combat/Boost Data")]
public class BoostData : EffectData
{
    public override void ApplyEffect(GameObject target, GameObject causer) 
    {
        base.ApplyEffect(target, causer);

    }
}
