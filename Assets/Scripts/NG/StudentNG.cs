using Mono.Cecil;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StudentNG : PostStudent
{
    private GameObject _target;



    protected override void Awake()
    {
        base.Awake();
        OverlapAttacker[] overlapAttackers = GetComponentsInChildren<OverlapAttacker>();
        _bodyOverlapAttacker = overlapAttackers[0];
        _tackleOverlapAttacker = overlapAttackers[1];
    }



    public void SetTarget(GameObject target)
    {
        _target = target;
    }



    public void TakeDamage(DamageData damageData)
    {
        Debug.Log("TakeDamage");
        HitInfo hitInfo = new();
        hitInfo.attacker = null;
        hitInfo.impulse = 500f;
        hitInfo.hitPoint = transform.position + Vector3.up;
        hitInfo.hitRotation = Quaternion.LookRotation(transform.forward);
        GetComponent<DamageReceiver>().TakeEffect(damageData, hitInfo);
    }



    protected override BT_Node ConstructBehaviorTree()
    {
        //Sequence coopSequence = new Sequence(new List<BT_Node>
        //{
        //    new ActionNode(() => Debug.Log(_blackboard.destSpot as CoopSpot2)),
        //    new ConditionNode(() => (_blackboard.destSpot as CoopSpot2).InviteParticipant(this, BehaviorType.Fight, 20)),
        //    new ClearDestSpot(),
        //});
        BT_Node bt = new Sequence(new List<BT_Node>
        {
        });
        return new SwimOverridePattern(bt);
    }
}
