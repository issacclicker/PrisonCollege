using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : EffectReceiver
{
    private Stat _health;
    public override Stat EffectedStat => _health;
    public override bool CanEffect => _health != null && !_health.IsDepleted;



    private void Awake()
    {
        _health = GetComponent<Stat>();
    }



    protected override void ApplyEffect(EffectData data, HitInfo hitInfo)
    {
        DamageData damageData = data as DamageData;
        if (!damageData) return;
        Debug.Log("ApplyEffect");
        DecreaseStat(hitInfo, data.value);
    }
}
