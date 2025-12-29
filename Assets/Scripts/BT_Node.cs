using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static Global;



public enum NodeState
{
    Running, // 실행 중 (예: 목적지로 이동 중)
    Success, // 성공 (예: 목적지 도착, 조건 만족)
    Failure  // 실패 (예: 경로 없음, 조건 불만족)
}



[System.Serializable]
public abstract class BT_Node
{
    protected Blackboard _bb; // 모든 자식 노드에서 접근 가능
    public virtual void SetBlackboard(Blackboard blackboard) => _bb = blackboard;
    public virtual void Reset() { }
    public abstract NodeState Evaluate();
}



public abstract class CompositeNode : BT_Node
{
    protected List<BT_Node> children = new List<BT_Node>();

    public CompositeNode(List<BT_Node> children)
    {
        this.children = children;
    }

    // 부모 노드에 블랙보드가 주입될 때 자식들에게도 전파 (재귀)
    public override void SetBlackboard(Blackboard blackboard)
    {
        base.SetBlackboard(blackboard);
        foreach (var child in children)
        {
            child.SetBlackboard(blackboard);
        }
    }

    public override void Reset()
    {
        foreach (var child in children) child.Reset();
    }
}



public class Sequence : CompositeNode
{
    private int _currentIndex = 0;
    public Sequence(List<BT_Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        if (_currentIndex >= children.Count) return NodeState.Success;

        var result = children[_currentIndex].Evaluate();

        switch (result)
        {
            case NodeState.Success:
                _currentIndex++;
                if (_currentIndex >= children.Count)
                {
                    Reset(); // 전체 완료 시 리셋
                    return NodeState.Success;
                }
                return NodeState.Running; // 다음 자식을 위해 계속 진행

            case NodeState.Failure:
                Reset(); // 중간 실패 시 리셋
                return NodeState.Failure;

            case NodeState.Running:
                return NodeState.Running;
        }

        return NodeState.Failure;
    }

    public override void Reset()
    {
        base.Reset(); // 모든 자식 리셋
        _currentIndex = 0; // 내 인덱스 초기화
    }
}



public class Selector : CompositeNode
{
    private BT_Node _lastRunningNode; // 지난 틱에 실행 중이던 자식 저장
    public Selector(List<BT_Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        BT_Node currentRunningNode = null;
        NodeState finalResult = NodeState.Failure;

        foreach (var child in children)
        {
            var result = child.Evaluate();

            if (result != NodeState.Failure)
            {
                currentRunningNode = (result == NodeState.Running) ? child : null;
                finalResult = result;
                break; // 하나라도 성공/진행 중이면 중단
            }
        }

        // [핵심] 실행 중인 노드가 바뀌었다면(Interrupt) 이전 노드 리셋
        if (_lastRunningNode != null && _lastRunningNode != currentRunningNode)
        {
            _lastRunningNode.Reset();
        }

        _lastRunningNode = currentRunningNode;
        return finalResult;
    }

    public override void Reset()
    {
        base.Reset();
        _lastRunningNode = null;
    }
}



public class RandomSelector : CompositeNode
{
    private List<System.Func<int>> _weights;
    private BT_Node _selectedChild; // 현재 선택되어 실행 중인 자식

    public RandomSelector(List<BT_Node> children, List<System.Func<int>> weights) : base(children)
    {
        _weights = weights;
    }

    public override NodeState Evaluate()
    {
        if (children.Count == 0) return NodeState.Failure;

        // 1. 선택된 자식이 없다면 새로 뽑기
        if (_selectedChild == null)
        {
            int totalWeight = 0;
            foreach (var w in _weights) totalWeight += Mathf.Max(0, w());
            if (totalWeight <= 0) return NodeState.Failure;

            int roll = UnityEngine.Random.Range(0, totalWeight);
            int cursor = 0;

            for (int i = 0; i < children.Count; i++)
            {
                cursor += _weights[i]();
                if (roll < cursor)
                {
                    _selectedChild = children[i];
                    break;
                }
            }
        }

        // 2. 선택된 자식 실행
        var result = _selectedChild.Evaluate();

        // 3. 실행이 끝났다면 참조 제거 (다음번에 새로 뽑도록)
        if (result != NodeState.Running)
        {
            Reset();
        }

        return result;
    }

    public override void Reset()
    {
        _selectedChild?.Reset();
        _selectedChild = null;
    }
}



public class PrioritySelector : CompositeNode
{
    private int _lastRunningIndex = -1;

    public PrioritySelector(List<BT_Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        for (int i = 0; i < children.Count; i++)
        {
            NodeState state = children[i].Evaluate();

            // 이번 프레임에 성공(Success)하거나 실행(Running) 중인 노드를 찾음
            if (state != NodeState.Failure)
            {
                // [중단 로직] 이전에 실행하던 노드가 있고, 그 노드보다 현재 노드의 우선순위가 높다면(인덱스가 작다면)
                if (_lastRunningIndex != -1 && _lastRunningIndex > i)
                {
                    children[_lastRunningIndex].Reset();
                }

                // 현재 실행 중인 인덱스 기록 (Running일 때만 유지, Success/Failure면 초기화)
                _lastRunningIndex = (state == NodeState.Running) ? i : -1;
                return state;
            }
        }

        // 모든 자식이 Failure를 반환한 경우
        _lastRunningIndex = -1;
        return NodeState.Failure;
    }

    public override void Reset()
    {
        base.Reset(); // 모든 자식 노드를 재귀적으로 Reset
        _lastRunningIndex = -1;
    }
}



public class SetRandomBehaveSpot : BT_Node
{
    private SpotGroup _behaveSpots;

    public SetRandomBehaveSpot(SpotGroup behaveSpots)
    {
        _behaveSpots = behaveSpots;
    }

    public override NodeState Evaluate()
    {
        // 현재 위치 주변 랜덤 좌표 계산
        BehaveSpot randomPoint = _behaveSpots.GetRandomSpotByWeight();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint.transform.position, out hit, NAVMESH_SAMPLE_RANGE, 1))
        {
            _bb.destSpot = randomPoint;
            _bb.destPosition = hit.position; // 블랙보드에 목적지 저장
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}



public class SetBehaveSpot : BT_Node
{
    private BehaveSpot _behaveSpot;

    public SetBehaveSpot(BehaveSpot behaveSpot)
    {
        _behaveSpot = behaveSpot;
    }

    public override NodeState Evaluate()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(_behaveSpot.transform.position, out hit, NAVMESH_SAMPLE_RANGE, 1))
        {
            _bb.destSpot = _behaveSpot;
            _bb.destPosition = hit.position; // 블랙보드에 목적지 저장
            return NodeState.Success;
        }
        return NodeState.Failure;
    }
}



public class MoveToTarget : BT_Node
{
    public override NodeState Evaluate()
    {
        _bb.Agent.SetDestination(_bb.destSpot.transform.position);
        Debug.Log($"목적지: {_bb.destSpot.name}, 남은 거리: {_bb.Agent.remainingDistance}");

        // 목적지에 거의 도착했는지 확인
        if (!_bb.Agent.pathPending && _bb.Agent.remainingDistance <= _bb.Agent.stoppingDistance)
        {
            _bb.Anim.SetFloat("MoveSpeed", 0);
            return NodeState.Success;
        }

        float currentSpeed = _bb.Agent.velocity.magnitude;
        _bb.Anim.SetFloat("MoveSpeed", currentSpeed);
        return NodeState.Running; // 아직 가는 중
    }
}



//나중에 일정 주기 가동시 Time.deltaTime 보정 필요
public class RotateToTarget : BT_Node
{
    private float _rotationSpeed = STUDENT_ROTQTE_SPEED;
    private float _threshold = 0.999f; // 약 1도 이내로 정렬되면 완료



    public override NodeState Evaluate()
    {
        if (_bb.destSpot == null) return NodeState.Failure;

        // 1. 목표 회전값 계산
        Quaternion targetRot = _bb.destSpot.transform.rotation;

        // 2. 현재 각도와 목표 각도의 차이(내적) 확인
        float dot = Vector3.Dot(_bb.Avatar.forward, _bb.destSpot.transform.forward);

        // 3. 이미 정렬되어 있다면 성공 반환
        if (dot >= _threshold)
        {
            _bb.Avatar.rotation = targetRot; // 오차 보정
            return NodeState.Success;
        }

        // 4. 부모(Owner)를 부드럽게 회전
        _bb.Avatar.rotation = Quaternion.Slerp(
            _bb.Avatar.rotation,
            targetRot,
            Time.deltaTime * _rotationSpeed
        );

        return NodeState.Running;
    }
}



// 중간에 Interrupt 발생시, Timer 초기화 로직 필요
public class Delay : BT_Node
{
    private Func<float> _getWaitFunc; // 대기 시간을 가져올 함수
    private float _timer = 0f;
    private float _currentWaitTime = -1f; // 이번 차례에 기다려야 할 시간

    // 생성자에서 함수를 주입받음
    public Delay(Func<float> getWaitFunc)
    {
        _getWaitFunc = getWaitFunc;
    }

    public override void Reset()
    {
        _timer = 0f;
        _currentWaitTime = -1f; // 초기화하여 다음 진입 시 새로 시간을 계산하게 함

        // 대기 중단 시 애니메이션 초기화 (선택 사항)
        // if (_bb.Anim != null) _bb.Anim.SetFloat("Speed", 0f);
    }

    public override NodeState Evaluate()
    {
        // 1. 처음 진입했을 때만 대기 시간을 함수로부터 받아옴
        if (_currentWaitTime < 0f)
        {
            _currentWaitTime = _getWaitFunc != null ? _getWaitFunc() : 0f;

            // 대기 시작 시 이동 애니메이션 멈춤
            // if (_bb.Anim != null) _bb.Anim.SetFloat("Speed", 0f);
        }

        // 2. 타이머 진행
        _timer += Time.deltaTime;

        // 3. 목표 시간에 도달했는지 확인
        if (_timer >= _currentWaitTime)
        {
            Reset(); // 성공했으므로 다음을 위해 리셋
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}



public class SetRandomSpeed : BT_Node
{
    private Func<float> _getSpeedFunc;

    public SetRandomSpeed(Func<float> getSpeedFunc)
    {
        _getSpeedFunc = getSpeedFunc;
    }

    public override NodeState Evaluate()
    {
        if (_getSpeedFunc == null) return NodeState.Failure;

        float speed = _getSpeedFunc();
        _bb.Agent.speed = speed;
        return NodeState.Success;
    }
}



public class SetSpeed : BT_Node
{
    private Func<float> _getSpeedFunc;

    public SetSpeed(Func<float> getSpeedFunc)
    {
        _getSpeedFunc = getSpeedFunc;
    }

    public override NodeState Evaluate()
    {
        if (_getSpeedFunc == null) return NodeState.Failure;

        float speed = _getSpeedFunc();
        _bb.Agent.speed = speed;
        return NodeState.Success;
    }
}



public class PlayLoopAnim : BT_Node
{
    private string _boolName;
    private float _duration;
    private float _timer = 0f;
    private int _layer; // 레이어 정보 추가

    public PlayLoopAnim(string boolName, float duration, int layer = 0)
    {
        _boolName = boolName;
        _duration = duration;
        _layer = layer;
    }

    public override void Reset()
    {
        _timer = 0f;
        if (_bb.Anim != null) _bb.Anim.SetBool(_boolName, false);
    }

    public override NodeState Evaluate()
    {
        if (_timer == 0f)
        {
            if (_bb.Anim != null) _bb.Anim.SetBool(_boolName, true);
        }

        _timer += Time.deltaTime;

        // 나중에 필요하다면 여기서 _layer를 사용해 특정 상태인지 확인할 수 있습니다.
        // var stateInfo = bb.Anim.GetCurrentAnimatorStateInfo(_layer);

        if (_timer >= _duration)
        {
            Reset();
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}



public class PlayOnceAnim : BT_Node
{
    private string _triggerName;
    private string _stateName;   // 애니메이터에 설정된 스테이트 이름
    private int _layer;
    private bool _triggered = false;

    public PlayOnceAnim(string triggerName, string stateName, int layer = 0)
    {
        _triggerName = triggerName;
        _stateName = stateName;
        _layer = layer;
    }

    public override void Reset()
    {
        _triggered = false;
    }

    public override NodeState Evaluate()
    {
        var stateInfo = _bb.Anim.GetCurrentAnimatorStateInfo(_layer);

        // 1. 트리거 실행
        if (!_triggered)
        {
            _bb.Anim.SetTrigger(_triggerName);
            _triggered = true;
            return NodeState.Running;
        }

        // 2. 애니메이션이 목표 스테이트에 있고, 한 바퀴 다 돌았는지 확인
        // IsName은 스테이트 이름 혹은 "Base Layer.StateName" 형태여야 할 수 있습니다.
        if (stateInfo.IsName(_stateName))
        {
            if (stateInfo.normalizedTime >= 0.99f)
            {
                Reset();
                return NodeState.Success;
            }
        }
        else if (_triggered && !_bb.Anim.IsInTransition(_layer))
        {
            // 트리거는 당겼는데 아직 스테이트 진입도 안 했고 트랜지션 중도 아니라면 대기
            return NodeState.Running;
        }

        return NodeState.Running;
    }
}



public class SetAnimBool : BT_Node
{
    private string _paramName;
    private bool _value;

    public SetAnimBool(string paramName, bool value)
    {
        _paramName = paramName;
        _value = value;
    }

    public override NodeState Evaluate()
    {
        _bb.Anim.SetBool(_paramName, _value);
        return NodeState.Success;
    }
}



public class SetAttackTarget : BT_Node
{
    private GameObject _targetObject;
    private IDamageable _targetDamageable;

    public SetAttackTarget(GameObject target)
    {
        _targetObject = target;
    }

    public override NodeState Evaluate()
    {
        if (_targetObject == null)
        {
            Debug.LogWarning("SetAttackTarget: Target GameObject is null.");
            return NodeState.Failure;
        }

        // 1. 타겟으로부터 IDamageable 인터페이스 추출
        _targetDamageable = _targetObject.GetComponent<IDamageable>();

        // 2. 공격 가능한 대상인지 검사 (인터페이스 존재 여부 및 생존 여부)
        if (_targetDamageable != null && !_targetDamageable.IsDead)
        {
            // 3. 블랙보드에 타겟 정보 저장 (이후 Chase, Attack 노드에서 사용)
            _bb.targetObject = _targetObject;
            _bb.targetDamageable = _targetDamageable;
            
            return NodeState.Success;
        }

        // 공격 불가능한 대상인 경우
        return NodeState.Failure;
    }
}



public class ProbabilisticDodge : BT_Node
{
    private float _chance;
    private int _lastProcessedAttackID = -1;

    public ProbabilisticDodge(float chance) => _chance = chance;

    public override NodeState Evaluate()
    {
        if (_bb.targetObject == null) return NodeState.Failure;

        var targetAttackable = _bb.targetObject.GetComponent<IAttackable>();
        if (targetAttackable == null || !targetAttackable.IsAttacking) 
        {
            _lastProcessedAttackID = -1;
            return NodeState.Failure;
        }

        // 새로운 공격 세션인 경우에만 확률 계산
        if (targetAttackable.CurrentAttackID != _lastProcessedAttackID)
        {
            _lastProcessedAttackID = targetAttackable.CurrentAttackID;
            if (UnityEngine.Random.value < _chance)
            {
                _bb.Anim.SetTrigger("tDodge");
                return NodeState.Success;
            }
        }
        return NodeState.Failure;
    }
}



public class DefenseAttack : BT_Node
{
    private int _lastProcessedAttackID = -1;
    private RandomSelector _randomSelector;

    public DefenseAttack()
    {
        // 30% 회피, 40% 가드, 30% 멍때리기(피격)
        _randomSelector = new RandomSelector(new List<BT_Node> {
            new PlayOnceAnim("Dodge", "Dodge"), 
            new PlayOnceAnim("Guard", "Guard"),
            new SuccessNode() 
        }, new List<System.Func<int>> { () => 30, () => 40, () => 30 });
    }

    public override void SetBlackboard(Blackboard blackboard)
    {
        base.SetBlackboard(blackboard);
        _randomSelector.SetBlackboard(blackboard);
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
            return _randomSelector.Evaluate();
        }

        _lastProcessedAttackID = -1;
        return NodeState.Failure;
    }


    public override void Reset()
    {
        _randomSelector?.Reset();
        _lastProcessedAttackID = -1;
    }
}



public class CombatApproach : BT_Node
{
    public override NodeState Evaluate()
    {
        float dist = Vector3.Distance(_bb.Avatar.position, _bb.targetDamageable.Position);
        float attackRange = 1.5f;

        if (dist <= attackRange) return NodeState.Success; // 사거리 진입

        // 5m 기준으로 상태 변화
        if (dist > 5.0f)
        {
            _bb.Agent.speed = 6.75f; // Sprint Speed
            _bb.Anim.SetBool("Boxing", false);
        }
        else
        {
            _bb.Agent.speed = 2.43f; // Jog/Boxing Speed
            _bb.Anim.SetBool("Boxing", true);
        }

    
        _bb.Agent.SetSampleDestination(_bb.targetDamageable.Position, 2);
        return NodeState.Running;
    }

    public override void Reset()
    {
        _bb.Anim.SetBool("Boxing", false);
        _bb.Agent.ResetPath();
    }
}



public class MeleeAttack : BT_Node
{
    private string[] _attackTriggers = { "tJab", "tHook", "tUppercut" };

    public override NodeState Evaluate()
    {
        // 이미 공격 애니메이션 재생 중이면 대기
        if (_bb.Anim.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
            return NodeState.Running;

        // 랜덤 공격 선택 및 실행
        string selected = _attackTriggers[UnityEngine.Random.Range(0, _attackTriggers.Length)];
        _bb.Anim.SetTrigger(selected);
        
        return NodeState.Success;
    }
}



public class ConditionNode : BT_Node
{
    private System.Func<bool> _condition;

    // 생성 시 판단 로직을 함수로 전달받음
    public ConditionNode(System.Func<bool> condition)
    {
        _condition = condition;
    }

    public override NodeState Evaluate()
    {
        // 조건이 참이면 Success, 거짓이면 Failure 반환
        return _condition() ? NodeState.Success : NodeState.Failure;
    }
}



public class SuccessNode : BT_Node {
    public override NodeState Evaluate() => NodeState.Success;
}



[System.Serializable]
public class Blackboard
{
    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }
    public Transform Avatar { get; private set; }

    public void Setup(NavMeshAgent agent, Animator animator, Transform transform)
    {
        Agent = agent;
        Anim = animator;
        Avatar = transform;
    }

    public Vector3 destPosition;
    public BehaveSpot destSpot;
    public BehaveSpot mySeatSpot;
    public bool isBehaving;
    public AIState currentState;
    public IDamageable targetDamageable;
    public GameObject targetObject;

    public bool IsSeating()
    {
        return isBehaving && (destSpot == mySeatSpot);
    }
}



public enum AIState { Idle, Working, Fighting }