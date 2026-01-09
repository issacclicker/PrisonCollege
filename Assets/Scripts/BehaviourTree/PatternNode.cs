using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Global;
using static UnityEditor.Experimental.GraphView.GraphView;



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



public class CombatPattern : PatternNode
{
    public CombatPattern()
    {
        _patternRoot = new ParallelNode(new List<BT_Node>
        {
            new CombatApproachPattern(),
            new RotateToTarget(),
        });
    }
}



public class CombatApproachPattern : PatternNode
{
    private const float SPRINT_THRESHOLD = 3.0f;
    private const float APPROACH_RANGE = 1.4f;
    private const float ATTACK_RANGE = 1.6f;
    private bool _isAttacking = false;

    public CombatApproachPattern()
    {
        _patternRoot = new ReactiveSelector(new List<BT_Node>
        {
            new ConditionDecorator(() => _bb.isStunned,
                new Sequence(new List<BT_Node>
                {
                    new SetAnimRootMotion(true),
                    new WaitUntilCondition(() => !_bb.isDamaged),
                    new Delay(() => UnityEngine.Random.Range(1f, 2f)),
                    new ActionNode(() => _bb.isStunned = false, NodeState.Success),
                })
            ),
            // 1. 전력질주 구간 (5m 이상)
            new ConditionDecorator(() => GetDistance() >= ATTACK_RANGE && !_isAttacking,
                new Sequence(new List<BT_Node>
                {
                    new SetAnimRootMotion(false),
                    new SetSpeed(() => 6.75f),
                    new ParallelNode(new List<BT_Node>
                    {
                        new LerpLayerWeight(COMBAT_LAYER_INDEX, 0f, 10f),
                        new MoveToTarget(),
                        //new RotateToTarget()
                    }),
                    new SetAnimRootMotion(false),
                })
            ),

            // 3. 최종 정지 구간 (1.5m 미만)
            new Sequence(new List<BT_Node>
            {
                // --- [공격 단계] ---
                //new ActionNode(() => _bb.Anim.SetLayerWeight(COMBAT_LAYER_INDEX, 1), NodeState.Success),
                new SetAnimRootMotion(true),
                new LerpLayerWeight(STRIKE_LAYER_INDEX, 0f, 10f),
                new LerpLayerWeight(COMBAT_LAYER_INDEX, 1f, 10f),
                new StopNode(),
                new Delay(() => UnityEngine.Random.Range(1f, 2f)),
                new ActionNode(() => _isAttacking = true, NodeState.Success), // 플래그 ON

                new MeleeAttackPattern(), // 실제 주먹 휘두르는 동안

                new ActionNode(() => _isAttacking = false, NodeState.Success), // 공격 끝나자마자 플래그 OFF

                // --- [후딜레이 단계] ---
                // 이제 _isAttacking이 false이므로, 
                // 딜레이 도중 플레이어가 멀어지면 상위 Selector가 1번(추격)으로 즉시 갈아탑니다.
                new Delay(() => UnityEngine.Random.Range(0f, 1f)),
                new SetAnimRootMotion(false),
            })
        });
    }

    private bool IsAttacking()
    {
        return _bb.Anim.GetCurrentAnimatorStateInfo(Global.COMBAT_LAYER_INDEX).IsTag("Attack");
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



public class MeleeAttackPattern : PatternNode
{
    private static readonly string[] _animNames = {};
    private static readonly int[] _animProbs = {};

    public MeleeAttackPattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new PrintDebug("MeleeAttackPattern Start"),
            //new SetAnimRootMotion(true),
            new RandomSelector(
                new List<BT_Node> {
                    new PlayOnceAnim("Punch1", "Punch1", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch2", "Punch2", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch3", "Punch3", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch4", "Punch4", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch5", "Punch5", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),

                    new PlayOnceAnim("Elbow1", "Elbow1", COMBAT_LAYER_INDEX),

                    new PlayOnceAnim("Kick1", "Kick1", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Kick2", "Kick2", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Kick3", "Kick3", COMBAT_LAYER_INDEX),
                }
            ),
            //new SetAnimRootMotion(false),
            new PrintDebug("MeleeAttackPattern End"),
        });
    }

    public override NodeState Evaluate()
    {
        // base.Evaluate()가 RandomSelector를 실행하고, 
        // 그 안의 PlayOnceAnim이 Running/Success를 알아서 판단합니다.
        NodeState state = base.Evaluate();

        // 만약 한 사이클의 공격이 끝났다면(Success), 
        // 다음 접근을 위해 내부 상태를 Reset 해줍니다.
        if (state == NodeState.Success)
        {
            Reset();
        }

        return state;
    }
}



public class RandomSpotSelectPattern : PatternNode
{
    public RandomSpotSelectPattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new RandomSelector(
                new List<BT_Node> {
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Punch6", "Punch6", COMBAT_LAYER_INDEX),

                    new PlayOnceAnim("Elbow1", "Elbow1", COMBAT_LAYER_INDEX),

                    new PlayOnceAnim("Kick3", "Kick3", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Kick3", "Kick3", COMBAT_LAYER_INDEX),
                    new PlayOnceAnim("Kick3", "Kick3", COMBAT_LAYER_INDEX)
                },
                new List<System.Func<int>> {
                    () => 50, // 잽은 자주
                    () => 10, // 훅은 보통
                    () => 10  // 어퍼컷은 가끔
                }
            ),
            //new SetAnimRootMotion(false),
            new PrintDebug("MeleeAttackPattern End"),
        });
    }
}




public class FindSpotPattern : PatternNode
{
    private const int MAX_RETRY = 3;
    private int _currentRetryCount = 0;

    public FindSpotPattern()
    {
        _patternRoot = new Selector(new List<BT_Node>
        {
            new FindDestSpot(), 
            new Sequence(new List<BT_Node>
            {
                new Delay(() => 1.0f),       // 1초 대기
                new ActionNode(() => 
                {
                    _currentRetryCount++;
                    Debug.Log($"[AI] 자리가 없어 재시도 중... ({_currentRetryCount}/{MAX_RETRY})");
                }, NodeState.Failure)
            })
        });
    }

    public override NodeState Evaluate()
    {
        if (_currentRetryCount >= MAX_RETRY)
        {
            Debug.Log("[AI] 모든 재시도 실패. 행동을 포기합니다.");
            Reset(); // 카운트 초기화
            return NodeState.Failure; // 전체 패턴 실패 -> 상위에서 다른 BehaviorType 결정 유도
        }

        // 2. 내부 트리 실행 (FindDestSpot 시도 -> 실패 시 Wait)
        NodeState state = _patternRoot.Evaluate();

        // 3. 만약 내부에서 스팟 찾기에 성공(Success)했다면 카운트 초기화
        if (state == NodeState.Success)
        {
            _currentRetryCount = 0;
        }

        return state; // Running(대기 중) 또는 Success(찾음) 반환
    }

    public override void Reset()
    {
        base.Reset();
        _currentRetryCount = 0;
    }
}



public class DoorEscapePatter : PatternNode
{
    public DoorEscapePatter() 
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new SetAnimRootMotion(true),
            new SetAnimBool("EscapeRunning", true),
            new PlayOnceAnim("EscapeJump", "EscapeJump"),
        });
    }
}



public class WindowEscapePattern : PatternNode
{
    public WindowEscapePattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new SetAnimRootMotion(true),
            //new SetAnimBool("EscapeRunning", true),
            new PlayOnceAnim("EscapeJump", "EscapeJump"),
            new ActionNode(() =>
            {
                Transform hipTransform = _bb.Avatar.transform.Find("Root/Hips");
                _bb.Anim.enabled = false;
                _bb.Agent.enabled = false;
                foreach (var rb in _bb.Avatar.GetComponentsInChildren<Rigidbody>())
                {
                    rb.isKinematic = false;
                    rb.velocity = Vector3.zero;
                    rb.AddForce(Vector3.down * 12f, ForceMode.VelocityChange);
                    rb.AddForce((Vector3.down + _bb.Avatar.forward).normalized * 2f, ForceMode.VelocityChange);
            
                    if (rb.TryGetComponent(out Collider col))
                    {
                        col.isTrigger = false;
                    }
                }
            }),
        });
    }
}



public class VentEscapePattern : PatternNode
{
    public VentEscapePattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
        });
    }
}



public class EscapeTypeSelectPattern : PatternNode
{
    public EscapeTypeSelectPattern()
    {
        _patternRoot = new Selector(new List<BT_Node>
        {
            new ConditionDecorator(() => (_bb.destSpot as ExitSpot).GateType == ExitGateType.Door, new DoorEscapePatter()),
            new ConditionDecorator(() => (_bb.destSpot as ExitSpot).GateType == ExitGateType.Window, new WindowEscapePattern()),
            new ConditionDecorator(() => (_bb.destSpot as ExitSpot).GateType == ExitGateType.Vent, new VentEscapePattern()),
        });
    }
}



public class RushThroughPattern : PatternNode
{
    public RushThroughPattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new PrintDebug("RushThroughPattern"),
            new SetRandomSpeedPattern(),
            //new SetSpeed(() => PostStudent._walkSpeed),
            new MoveToSpot(),
            new RotateToSpot(),
            new StopAndDisableAgentUpdate(),
            new SetAnimRootMotion(true),
            new SetAnimBool("Rush", true),
            new Delay(() => 1.1f),
            new ActionNode(() => {
                var attacker = _bb.Avatar.GetComponentInChildren<OverlapAttacker>();
                attacker.StartAttack();
            }, NodeState.Success),
            new ActionNode(null, NodeState.Running),
        });
    }
}



public class CoopPattern : PatternNode
{
    public CoopPattern()
    {
        // 협동 패턴 루트 구성 예시
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new OverrideBehaveSpot(() => _bb.coopData.spot, () => _bb.coopData.type),
            new SetAnimRootMotion(false),
            new ResetAnimParameters(),
            new MoveToSpot(),
            new RotateToSpot(),
            new ActionNode(() => _bb.destSpot.Arrived(_bb.Avatar.GetComponent<PostStudent>())),
            
            //new PlayWaitAnimation(),

            // 3. 실행 신호가 올 때까지 대기 (Phase가 Ready가 될 때까지)
            new WaitUntilCondition(() => _bb.coopData.isExecuting),

            // 4. 실제 협동 애니메이션 실행
            //new SetAnimRootMotion(true),
            //new SetAnimBool("Talking", true),
            new OverrideAttackTarget(() => _bb.coopData.targetObject),
            new ActionNode(null, NodeState.Running),
            //new SetAnimRootMotion(false),
            //new SetAttackTarget()
            //new PlayCoopAnimationNode()
        });
    }
}



public class CoopReactivePatttern : PatternNode
{
    public CoopReactivePatttern(BT_Node normalRoutine)
    {
        _patternRoot = new ReactiveSelector(new List<BT_Node>
        {
            new ConditionDecorator(() => _bb.coopData.spot != null, new CoopPattern()),
            normalRoutine
        });
    }
}



public class AttackReactivePattern : PatternNode
{
    public AttackReactivePattern(BT_Node normalRoutine)
    {
        _patternRoot = new ReactiveSelector(new List<BT_Node>
        {
            new ConditionDecorator(() => _bb.targetDamageable != null, new CombatPattern()),
            normalRoutine
        });
    }
}



public class TakeHitPattern : PatternNode
{
    public TakeHitPattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new RandomSelector(new List<BT_Node>
            {
                new PlayOnceAnim("OnHit", "OnHit", 5),
                //new PlayOnceAnim("OnHit2", "OnHit2", 5),
                //new PlayOnceAnim("OnHit3", "OnHit3", 5),
            }),
            new ActionNode(() => _bb.isDamaged = false, NodeState.Success),
        });
    }
}



public class TakeHitReactivePattern : PatternNode
{
    public TakeHitReactivePattern(BT_Node normalRoutine)
    {
        _patternRoot = new ParallelOR(new List<BT_Node>
        {
            new ConditionDecorator(() => _bb.isDamaged, new TakeHitPattern()),
            normalRoutine
        });
    }
}



//버그 : 차단막 파괴되었는데도 한번 더 때리는 경우있음, 그리고 탈출대기중인거 강화할때 버그 (둘다 랜덤)
public class TryEscapePattern : PatternNode
{
    public TryEscapePattern()
    {
        // 내부 로직 설계: Selector를 통해 조건별 분기
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new PrintDebug("TryEscapePattern"),
            new SetRandomSpeedPattern(),
            //new SetSpeed(() => PostStudent._walkSpeed),
            new MoveToSpot(),
            new RotateToSpot(),
            new ReactiveSelector(new List<BT_Node>
            {
                new ConditionDecorator(() =>
                {
                   ExitSpot exitSpot = _bb.destSpot as ExitSpot;
                   return exitSpot != null && exitSpot.CanExit;
                },
                   new Sequence(new List<BT_Node>
                   {
                       new ActionNode(() => _bb.isEscaping = true),
                       new PrintDebug("Escape success!"),
                       new StopAndDisableAgentUpdate(),
                       new FadeLayerByIndex(0, 0.2f),
                       new ActionNode(() =>
                       {
                           ExitSpot exitGate = _bb.destSpot as ExitSpot;
                           exitGate.OpenGate();
                       }, NodeState.Success),
                       new EscapeTypeSelectPattern(),
                       // exitGate의 세부타입별로 다른 PatternNode 실행하기
                       new ActionNode(null, NodeState.Running),
                       new ActionNode(() => _bb.isEscaping = false),
                   })
                ),
                new ConditionDecorator(() => _bb.isStunned,
                   new Sequence(new List<BT_Node>
                   {
                       new SetAnimRootMotion(true),
                       new WaitUntilCondition(() => !_bb.isDamaged),
                       new Delay(() => UnityEngine.Random.Range(1f, 2f)),
                       new ActionNode(() => _bb.isStunned = false, NodeState.Success),
                   })
                ),
                new Sequence(new List<BT_Node>
                {
                    // --- [공격 단계] ---
                    new LerpLayerWeight(COMBAT_LAYER_INDEX, 0, 10),
                    new LerpLayerWeight(STRIKE_LAYER_INDEX, 1, 10),
                    //new ActionNode(() => _bb.Anim.SetLayerWeight(STRIKE_LAYER_INDEX, 1), NodeState.Success),
                    new StopAndDisableAgentUpdate(),
                    new SetAnimRootMotion(true),

                    new ExitAttackPattern(), // 실제 주먹 휘두르는 동안
                
                    //new Delay(() => Time.deltaTime),
                    new SetAnimRootMotion(false),

                    new EnableAgentUpdate(),
                })
            }),
        });
    }

    public override NodeState Evaluate()
    {
        // 1. 방어 코드: destSpot이 없거나 ExitSpot이 아니면 패턴 실패
        if (_bb.destSpot == null || !(_bb.destSpot is ExitSpot))
        {
            Debug.LogError("[TryEscapePattern] 목적지가 ExitSpot이 아닙니다.");
            return NodeState.Failure;
        }

        // 2. 내부 트리(Selector) 실행
        return _patternRoot.Evaluate();
    }
}



public class ExitAttackPattern : PatternNode
{
    private static readonly string[] _animNames = { };
    private static readonly int[] _animProbs = { };

    public ExitAttackPattern()
    {
        _patternRoot = new Sequence(new List<BT_Node>
        {
            new PrintDebug("ExitAttackPattern Start"),
            //new SetAnimRootMotion(true),
            new RandomSelector(
                new List<BT_Node> {
                    new PlayOnceAnim("Punch1_z", "Punch1_z", STRIKE_LAYER_INDEX),
                    //new PlayOnceAnim("Punch2_z", "Punch2_z", STRIKE_LAYER_INDEX),
                    new PlayOnceAnim("Punch3_z", "Punch3_z", STRIKE_LAYER_INDEX),
                    new PlayOnceAnim("Kick1_z", "Kick1_z", STRIKE_LAYER_INDEX),
                },
                new List<System.Func<int>> {
                    () => 10,
                    //() => 0,
                    () => 0,
                    () => 0,
                }
            ),
            //new SetAnimRootMotion(false),
            new PrintDebug("ExitAttackPattern End"),
        });
    }

    public override NodeState Evaluate()
    {
        NodeState state = base.Evaluate();
        if (state == NodeState.Success)
        {
            Reset();
        }

        return state;
    }
}



public class SetRandomSpeedPattern : PatternNode
{
    public SetRandomSpeedPattern()
    {
        _patternRoot = new RandomSelector(
            new List<BT_Node> {
                new SetSpeed(() => PostStudent._walkSpeed),
                new SetSpeed(() => PostStudent._jogSpeed),
                new SetSpeed(() => PostStudent._slowRunSpeed),
                new SetSpeed(() => PostStudent._mediumRunSpeed),
                new SetSpeed(() => PostStudent._fastRunSpeed),
                new SetSpeed(() => PostStudent._sprintSpeed),
            },
            new List<System.Func<int>> {
                () => 40, // Walk 확률 40%
                () => 25, // Jog 확률 25%
                () => 15, // SlowRun 15%
                () => 10, // MedRun 10%
                () => 7,  // FastRun 7%
                () => 3   // Sprint 3%
            }
        );
    }
}