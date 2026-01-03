using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDamageData", menuName = "Combat/Damage Data")]
public class DamageData : EffectData
{
    public override void ApplyEffect(GameObject target, GameObject causer)
    {
        base.ApplyEffect(target, causer);
        if (target.TryGetComponent(out Health health))
        {
            // 데미지 적용 로직
            health.Decrease(value);
            Debug.Log($"{target.name}에게 {value} 데미지를 입힘.");
        }
    }
}