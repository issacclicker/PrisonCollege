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



public class ConditionDecorator : BT_Node
{
    private readonly Func<bool> _condition; // 체크할 조건식
    private readonly BT_Node _child;         // 실행할 자식 노드

    public ConditionDecorator(Func<bool> condition, BT_Node child)
    {
        _condition = condition;
        _child = child;
    }

    public override void SetBlackboard(Blackboard blackboard)
    {
        base.SetBlackboard(blackboard);
        _child?.SetBlackboard(blackboard);
    }

    public override NodeState Evaluate()
    {
        if (_condition == null || _child == null) return NodeState.Failure;

        // 1. 조건을 체크한다.
        if (_condition.Invoke())
        {
            // 2. 조건이 맞으면 자식을 실행하고 그 결과를 그대로 부모에게 보고한다.
            return _child.Evaluate();
        }

        // 3. 조건이 틀리면 자식을 리셋하고 실패를 보고한다.
        _child.Reset();
        return NodeState.Failure;
    }

    public override void Reset()
    {
        _child?.Reset();
    }
}



public class ActionNode : BT_Node
{
    private readonly Action _action;
    private readonly NodeState _resultState;

    // 실행할 함수와, 종료 후 보고할 상태를 인자로 받음
    public ActionNode(Action action, NodeState resultState = NodeState.Success)
    {
        _action = action;
        _resultState = resultState;
    }

    public override NodeState Evaluate()
    {
        // 1. 주입된 함수 실행 (null 체크 포함)
        _action?.Invoke();

        // 2. 지정된 노드 상태 반환
        return _resultState;
    }
}



public class StopNode : BT_Node
{
    private readonly int _speedHash = Animator.StringToHash("MoveSpeed");

    public override NodeState Evaluate()
    {
        if (_bb.Agent != null && _bb.Agent.isOnNavMesh)
        {
            // 1. 물리적 속도 즉시 제거
            _bb.Agent.velocity = Vector3.zero;
            _bb.Agent.speed = 0;
            // 2. NavMeshAgent의 경로 계산 중지 및 정지
            _bb.Agent.isStopped = true; 
            _bb.Agent.ResetPath();

            // 3. 애니메이션 파라미터 즉시 0으로 설정 (DampTime 제거)
            _bb.Anim.SetFloat(_speedHash, 0f);
        }

        // 즉시 중지이므로 바로 Success 반환
        return NodeState.Success;
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



public class MoveToSpot : BT_Node
{
    public override NodeState Evaluate()
    {
        //Debug.Log(_bb.destSpot);
        _bb.Agent.SetSampleDestination(_bb.destSpot.transform.position, 1);
        //Debug.Log($"목적지: {_bb.destSpot.name}, 남은 거리: {_bb.Agent.remainingDistance}");

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



public class MoveToTarget : BT_Node
{
    public override NodeState Evaluate()
    {
        _bb.Agent.SetSampleDestination(_bb.targetDamageable.Position, 2);
        //Debug.Log($"목적지: {_bb.targetObject.name}, 남은 거리: {_bb.Agent.remainingDistance}");

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
public class RotateToSpot : BT_Node
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



public class RotateToTarget : BT_Node
{
    private const float ROTATION_SPEED = 10f; // 회전 속도
    private const float FINISH_ANGLE = 5.0f;  // 이 각도 이내로 들어오면 완료

    public override NodeState Evaluate()
    {
        if (_bb.targetDamageable == null) return NodeState.Failure;

        Vector3 targetDir = _bb.targetDamageable.Position - _bb.Avatar.transform.position;
        targetDir.y = 0;

        if (targetDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            _bb.Avatar.transform.rotation = Quaternion.Slerp(
                _bb.Avatar.transform.rotation, 
                targetRotation, 
                Time.deltaTime * 10f // 회전 속도
            );
        }

        // ParallelNode 안에서 계속 돌아야 하므로 항상 Running 반환
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



//public class SetRandomSpeed : BT_Node
//{
//    private Func<float> _getSpeedFunc;

//    public SetRandomSpeed(Func<float> getSpeedFunc)
//    {
//        _getSpeedFunc = getSpeedFunc;
//    }

//    public override NodeState Evaluate()
//    {
//        if (_getSpeedFunc == null) return NodeState.Failure;

//        float speed = _getSpeedFunc();
//        _bb.Agent.speed = speed;
//        return NodeState.Success;
//    }
//}



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



public class Accelerate : BT_Node
{
    private Func<float> _getSpeedFunc;
    private float _acceleration = 5f; // 초당 속도 변화량 (가속도)
    private readonly int _speedHash = Animator.StringToHash("MoveSpeed");

    public Accelerate(Func<float> getSpeedFunc, float acceleration = 5f)
    {
        _getSpeedFunc = getSpeedFunc;
        _acceleration = acceleration;
    }

    public override NodeState Evaluate()
    {
        if (_getSpeedFunc == null) return NodeState.Failure;

        float targetSpeed = _getSpeedFunc();
        
        // 1. 현재 에이전트의 속도값 가져오기
        float currentSpeed = _bb.Agent.speed;

        // 2. 목표 속도를 향해 부드럽게 보간 (MoveTowards는 목표치에 정확히 안착함)
        float nextSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, _acceleration * Time.deltaTime);

        // 3. 에이전트와 애니메이터에 동시에 적용
        _bb.Agent.speed = nextSpeed;
        _bb.Anim.SetFloat(_speedHash, nextSpeed);

        // 4. 목표 속도에 충분히 도달했으면 Success, 아니면 계속 가감속 중이므로 Running
        if (Mathf.Approximately(nextSpeed, targetSpeed))
        {
            return NodeState.Success;
        }

        return NodeState.Running;
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



public class SetAnimRootMotion : BT_Node
{
    private bool _useRootMotion;

    public SetAnimRootMotion(bool useRootMotion)
    {
        _useRootMotion = useRootMotion;
    }

    public override NodeState Evaluate()
    {
        _bb.Anim.applyRootMotion = _useRootMotion;
        return NodeState.Success;
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
    private DamageReceiver _targetDamageable;

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
        _targetDamageable = _targetObject.GetComponent<DamageReceiver>();

        // 2. 공격 가능한 대상인지 검사 (인터페이스 존재 여부 및 생존 여부)
        if (_targetDamageable != null && _targetDamageable.CanEffect)
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



public class LerpLayerWeight : BT_Node
{
    private int _layerIndex;
    private float _targetWeight;
    private float _lerpSpeed;

    public LerpLayerWeight(int layerIndex, float targetWeight, float lerpSpeed = 5f)
    {
        _layerIndex = layerIndex;
        _targetWeight = targetWeight;
        _lerpSpeed = lerpSpeed;
    }

    public override NodeState Evaluate()
    {
        float currentWeight = _bb.Anim.GetLayerWeight(_layerIndex);
        
        // 목표값과 현재값의 차이가 아주 작으면 완료(Success)
        if (Mathf.Abs(currentWeight - _targetWeight) < 0.01f)
        {
            _bb.Anim.SetLayerWeight(_layerIndex, _targetWeight);
            return NodeState.Success;
        }

        // 점진적 보간
        float nextWeight = Mathf.Lerp(currentWeight, _targetWeight, Time.deltaTime * _lerpSpeed);
        _bb.Anim.SetLayerWeight(_layerIndex, nextWeight);
        
        return NodeState.Running;
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



public class DoSuccess : BT_Node {
    public override NodeState Evaluate() => NodeState.Success;
}



public class PrintDebug : BT_Node
{
    private string _message;
    private Color _logColor;

    // 메시지와 로그 색상을 지정할 수 있는 생성자
    public PrintDebug(string message, string color = "white")
    {
        _message = message;
        _logColor = GetColor(color);
    }

    public override NodeState Evaluate()
    {
        // 리치 텍스트를 이용해 콘솔에서 눈에 띄게 출력
        string colorHex = ColorUtility.ToHtmlStringRGB(_logColor);
        Debug.Log($"<color=#{colorHex}>[BT_Debug]: {_message}</color>");
        
        return NodeState.Success;
    }

    private Color GetColor(string color)
    {
        return color.ToLower() switch
        {
            "red" => Color.red,
            "green" => Color.green,
            "blue" => Color.blue,
            "yellow" => Color.yellow,
            _ => Color.white
        };
    }
}



public class SetRandomBehavior : BT_Node
{
    public override NodeState Evaluate()
    {
        // 1. 블랙보드에서 필요한 데이터 참조 (캐싱되어 있다고 가정)
        var weightSet = _bb.BehaviorWeightSet;

        if (weightSet == null)
        {
            Debug.LogError("블랙보드에 BehaviorWeightSet이 설정되지 않았습니다.");
            return NodeState.Failure;
        }

        BehaviorType pickedType = weightSet.GetRandomValue();

        if (pickedType == BehaviorType.None)
        {
            return NodeState.Failure;
        }
        _bb.destBehavior = pickedType;
        Debug.Log($"[BT] 행동 결정됨: {pickedType}");

        return NodeState.Success;
    }
}



public class FindDestSpot : BT_Node
{
    private float _sampleRange = 2.0f; // 스팟 주변에서 NavMesh를 검색할 반경

    public override NodeState Evaluate()
    {
        BehaviorType targetType = _bb.destBehavior;
        BehaveSpot spot = _bb.StageSpots.GetRandomSpotByType(targetType);

        if (spot != null && spot.IsUsable)
        {
            Vector3 rawPosition = spot.transform.position;
            if (NavMesh.SamplePosition(rawPosition, out NavMeshHit hit, _sampleRange, NavMesh.AllAreas))
            {
                _bb.destSpot = spot;
                _bb.destPosition = hit.position;
                Debug.Log($"FindDestSpot : {spot}");
                return NodeState.Success;
            }
            else
            {
                Debug.LogWarning($"[FindDestSpot] {spot.name} 주변에서 유효한 NavMesh를 찾을 수 없습니다.");
                return NodeState.Failure;
            }
        }

        return NodeState.Failure;
    }
}



public class EnumSwitchSelector<TEnum> : BT_Node where TEnum : Enum
{
    private readonly Dictionary<TEnum, BT_Node> _subTrees;
    private readonly BT_Node _defaultNode;

    // 블랙보드에서 어떤 열거형 값을 가져올지 결정하는 델리게이트
    private readonly Func<Blackboard, TEnum> _valueSelector;

    public EnumSwitchSelector(
        Func<Blackboard, TEnum> valueSelector,
        Dictionary<TEnum, BT_Node> subTrees,
        BT_Node defaultNode = null)
    {
        _valueSelector = valueSelector;
        _subTrees = subTrees;
        _defaultNode = defaultNode;
    }

    public override void SetBlackboard(Blackboard blackboard)
    {
        base.SetBlackboard(blackboard);
        foreach (var node in _subTrees.Values)
        {
            node.SetBlackboard(blackboard);
        }
        _defaultNode?.SetBlackboard(blackboard);
    }

    public override NodeState Evaluate()
    {
        TEnum currentValue = _valueSelector(_bb);

        if (_subTrees.TryGetValue(currentValue, out BT_Node node))
        {
            return node.Evaluate();
        }

        if (_defaultNode != null)
        {
            return _defaultNode.Evaluate();
        }

        return NodeState.Failure;
    }

    public override void Reset()
    {
        base.Reset();
        foreach (var node in _subTrees.Values) node.Reset();
        _defaultNode?.Reset();
    }
}



public class StopAndDisableAgentUpdate : BT_Node
{
    public override NodeState Evaluate()
    {
        if (_bb.Agent != null)
        {
            _bb.Agent.isStopped = true;       // 물리적 정지 명령
            _bb.Agent.velocity = Vector3.zero; // 남은 관성 제거
            _bb.Agent.updatePosition = false; // ★ 에이전트가 트랜스폼을 건드리지 못하게 함
            _bb.Agent.updateRotation = false; // 필요 시 회전도 고정
        }
        return NodeState.Success;
    }
}



public class EnableAgentUpdate : BT_Node
{
    public override NodeState Evaluate()
    {
        _bb.Agent.updatePosition = true;
        _bb.Agent.updateRotation = true;
        _bb.Agent.isStopped = false;
        return NodeState.Success;
    }
}