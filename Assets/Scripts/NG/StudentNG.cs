using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StudentNG : PostStudent
{
    private Blackboard _blackboard;



    protected override void Awake()
    {
        base.Awake();
        OverlapAttacker[] overlapAttackers = GetComponentsInChildren<OverlapAttacker>();
        _bodyOverlapAttacker = overlapAttackers[0];
        _tackleOverlapAttacker = overlapAttackers[1];
    }



    protected override BT_Node ConstructBehaviorTree()
    {
        BT_Node bt = new Sequence(new List<BT_Node>
        {
            new OverrideBehaveSpot(() => SeatSpot, () => BehaviorType.Work),
            new FindSpotPattern(),
            new WorkPattern(),
        });
        return bt;
    }
}
