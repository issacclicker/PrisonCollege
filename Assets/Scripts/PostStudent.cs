using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class PostStudent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _anim;
    private BT_Node _root;
    private Blackboard _blackboard = new Blackboard();

    [Header("이동 속도 설정 (Threshold 기반)")]
    [SerializeField] private float _idleSpeed = 0f;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _runSpeed = 7.0f;
    [SerializeField] private float _sprintSpeed = 10.0f;

    [Header("설정")]
    [SerializeField] private float _changeInterval = 2.0f; // 2초 간격
    [SerializeField] private Transform _targetDestination; // 이동 목표 지점

    [SerializeField] private BehaveSpot chairSpot; // 가중치 스팟 데이터 SO
    [SerializeField] private SpotGroup restSpots; // 가중치 스팟 데이터 SO


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _agent.acceleration = 30f;

        _blackboard.Setup(_agent, _anim, transform);
        _root = ConstructBehaviorTree();
        _root.SetBlackboard(_blackboard);
    }

    private BT_Node ConstructBehaviorTree()
    {
        // 동작 설계: 
        // 1. 랜덤 지점으로 이동
        // 2. 도착하면 3초간 주변 구경(Loop)
        // 3. 50% 확률로 기지개 켜기(Once), 50% 확률로 그냥 대기

        Sequence restSequence = new Sequence(new List<BT_Node>
        {
            new SetRandomBehaveSpot(restSpots),
            new SetRandomSpeed(GetRandomSpeed),
            new MoveToTarget(),
            new PlayOnceAnim("LookAround", "LookAround")
            //new PlayLoopAnim("LookAround", 5)
        });
        Sequence workSequence = new Sequence(new List<BT_Node>
        {
            new SetBehaveSpot(chairSpot),
            new SetRandomSpeed(GetRandomSpeed),
            new MoveToTarget(),
            new RotateToTarget(),
            new PlayLoopAnim("Typing", 5)
        });

        RandomSelector randomJobSelector = new RandomSelector(
            new List<BT_Node> { restSequence, workSequence },
            new List<System.Func<int>> { () => 50, () => 50 }
        );

        // 4. 전체 루트를 반복(Selector 또는 Sequence) 하도록 설정
        return randomJobSelector;
        return new Selector(new List<BT_Node> { randomJobSelector });
    }



    private float GetRandomSpeed()
    {
        return UnityEngine.Random.Range(_walkSpeed, _sprintSpeed);
    }

    //private void Awake()
    //{
    //    _agent = GetComponent<NavMeshAgent>();
    //    _anim = GetComponent<Animator>();

    //    // 가속도를 높여야 속도 변화가 즉각적으로 보입니다.
    //    _agent.acceleration = 30f;
    //}

    //private void Start()
    //{
    //    if (_targetDestination != null)
    //    {
    //        _agent.SetDestination(_targetDestination.position);
    //        StartCoroutine(MovementRoutine());
    //    }
    //}

    private void Update()
    {
        // 현재 에이전트의 실제 속도를 애니메이터에 전달 (보폭 맞추기)
        // Magnitude를 사용하면 방향과 상관없이 실제 이동 속도가 전달됩니다.
        //_anim.SetFloat("MoveSpeed", _agent.velocity.magnitude, 0.1f, Time.deltaTime);
        if (_root != null)
        {
            _root.Evaluate();
        }
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            // 1단계: 정지
            UpdateState("정지", _idleSpeed);
            yield return new WaitForSeconds(_changeInterval);

            // 2단계: 걷기
            UpdateState("걷기", _walkSpeed);
            yield return new WaitForSeconds(_changeInterval);

            // 3단계: 조깅
            //UpdateState("조깅", _jogSpeed);
            //yield return new WaitForSeconds(_changeInterval);

            // 4단계: 뛰기
            UpdateState("뛰기", _runSpeed);
            yield return new WaitForSeconds(_changeInterval);

            // 5단계: 전력질주
            UpdateState("전력질주", _sprintSpeed);
            yield return new WaitForSeconds(_changeInterval);
        }
    }

    private void UpdateState(string stateName, float speed)
    {
        _agent.speed = speed;
        Debug.Log($"현재 상태: {stateName} (속도: {speed})");
    }
}