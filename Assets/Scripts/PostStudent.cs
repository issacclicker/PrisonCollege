using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PostStudent : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _anim;

    [Header("이동 속도 설정 (Threshold 기반)")]
    [SerializeField] private float _idleSpeed = 0f;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _runSpeed = 7.0f;
    [SerializeField] private float _sprintSpeed = 10.0f;

    [Header("설정")]
    [SerializeField] private float _changeInterval = 2.0f; // 2초 간격
    [SerializeField] private Transform _targetDestination; // 이동 목표 지점

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        // 가속도를 높여야 속도 변화가 즉각적으로 보입니다.
        _agent.acceleration = 30f;
    }

    private void Start()
    {
        if (_targetDestination != null)
        {
            _agent.SetDestination(_targetDestination.position);
            StartCoroutine(MovementRoutine());
        }
    }

    private void Update()
    {
        // 현재 에이전트의 실제 속도를 애니메이터에 전달 (보폭 맞추기)
        // Magnitude를 사용하면 방향과 상관없이 실제 이동 속도가 전달됩니다.
        _anim.SetFloat("MoveSpeed", _agent.velocity.magnitude, 0.1f, Time.deltaTime);
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
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
