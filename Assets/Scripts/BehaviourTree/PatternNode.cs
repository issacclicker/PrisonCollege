using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class PatternNode : BT_Node
{
    protected BT_Node _patternRoot;

    protected PatternNode() { }

    public override void SetBlackboard(Blackboard blackboard)
    {
        base.SetBlackboard(blackboard);
        _patternRoot?.SetBlackboard(blackboard);
    }

    public override NodeState Evaluate()
    {
        return _patternRoot != null ? _patternRoot.Evaluate() : NodeState.Failure;
    }

    public override void Reset()
    {
        _patternRoot?.Reset();
    }
}



public class DefenseAttackPattern : PatternNode
{
    private int _lastProcessedAttackID = -1;

    public DefenseAttackPattern()
    {
        // 30% 회피, 40% 가드, 30% 멍때리기(피격)
        _patternRoot = new RandomSelector(new List<BT_Node> {
            new PlayOnceAnim("Dodge", "Dodge"), 
            new PlayOnceAnim("Guard", "Guard"),
            new DoSuccess() 
        }, new List<System.Func<int>> { () => 30, () => 40, () => 30 });
    }

    public override NodeState Evaluate()
    {
        var target = _bb.targetObject.GetComponent<IAttackable>();
        
        if (target != null && target.IsAttacking)
        {
            if (target.CurrentAttackID == _lastProcessedAttackID)
            {
                return NodeState.Failure; 
            }
            _lastProcessedAttackID = target.CurrentAttackID;
            return _patternRoot.Evaluate();
        }

        _lastProcessedAttackID = -1;
        return NodeState.Failure;
    }


    public override void Reset()
    {
        base.Reset();
        _lastProcessedAttackID = -1;
    }
}



public class CombatApproachPattern : PatternNode
{
    private const float SPRINT_THRESHOLD = 3.0f;
    private const float ATTACK_RANGE = 1.7f;
    private const int COMBAT_LAYER_INDEX = 3;

    public CombatApproachPattern()
    {
        _patternRoot = new InterruptSelector(new List<BT_Node>
        {
            // 1. 전력질주 구간 (5m 이상)
            new ConditionDecorator(() => GetDistance() >= ATTACK_RANGE,
                new Sequence(new List<BT_Node>
                {
                    new ParallelNode(new List<BT_Node>
                    {
                        //new Accelerate(() => 6.75f, 5f),
                        new SetSpeed(() => 6.75f),
                        //new SetAnimBool("Fighting", false),
                        new LerpLayerWeight(COMBAT_LAYER_INDEX, 0f, 5f),
                        new MoveToTarget(),
                        new RotateToTarget()
                    })
                })
            ),

            // 2. 복싱접근 구간 (1.5m ~ 5m)
            // new ConditionDecorator(() => GetDistance() >= ATTACK_RANGE,
            //     new Sequence(new List<BT_Node>
            //     {
            //         new ParallelNode(new List<BT_Node>
            //         {
            //             new Accelerate(() => 1.04f, 12f),
            //             new SetAnimBool("Fighting", true),
            //             //new LerpLayerWeight(COMBAT_LAYER_INDEX, 1f, 12f),
            //             new MoveToTarget(),
            //             new RotateToTarget()
            //         })
            //     })
            // ),

            // 3. 최종 정지 구간 (1.5m 미만)
            new Sequence(new List<BT_Node>
            {
                new LerpLayerWeight(COMBAT_LAYER_INDEX, 1f, 8f),
                //new SetAnimBool("Fighting", true),
                new StopNode()
            })
        });
    }

    public override NodeState Evaluate()
    {
        return base.Evaluate();
        // 공격 사거리 안에 들어오면 패턴 성공(Success)으로 종료
        if (GetDistance() <= ATTACK_RANGE)
        {
            _bb.Agent.ResetPath();
            return NodeState.Success;
        }

        // 아직 멀다면 내부 트리(Selector) 실행
        return base.Evaluate();
    }

    private float GetDistance()
    {
        if (_bb.targetObject == null) return float.MaxValue;
        
        return Vector3.Distance(_bb.Avatar.transform.position, _bb.targetDamageable.Position);
    }
}