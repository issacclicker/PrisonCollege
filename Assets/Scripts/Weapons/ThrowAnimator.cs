using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOTweenSeq = DG.Tweening.Sequence;

public class ThrowAnimator : WeaponAnimator
{
    [Header("--- Throw Settings ---")]
    [SerializeField] private GameObject _throwablePrefab; // 던질 물체 프리팹
    [SerializeField] private Transform _throwableModel; // 던질 물체 프리팹
    [SerializeField] private Transform _throwPoint;       // 물체가 생성될 위치 (무기 근처)
    [SerializeField] private float _throwForce = 15f;     // 던지는 힘
    [SerializeField] private Vector3 _throwOffset = new Vector3(0.5f, -0.2f, 1.0f);
    [SerializeField] private float _flipSpeed = 20f; // 위아래 회전 속도 (높을수록 빠름)
    [Range(0f, 1f)] public float _spreadAmount = 0.02f; // 탄퍼짐 정도
    [Range(0f, 1f)] public float _torqueRandomness = 0.5f; // 회전 불규칙도

    private Vector3 _initialScale;
    private Quaternion _initialRotation;
    private Vector3 _initialPosition;


    protected override void Awake()
    {
        base.Awake();
        _initialScale = _throwableModel.localScale;
        _initialRotation = _throwableModel.localRotation;
        _initialPosition = _throwableModel.localPosition;
    }


    protected override void AddAttackFrames(DOTweenSeq attackAnimSeq)
    {
        // [핵심 1] 시퀀스 시작 즉시 모델을 켭니다. 
        // 이전 공격이 도중에 끊겨서 꺼진 채로 남아있을 경우를 대비합니다.
        attackAnimSeq.AppendCallback(() => _throwableModel.gameObject.SetActive(true));

        float recoilTime = _attackDuration * 0.2f;
        float returnTime = _attackDuration * 0.7f; // 시간 합계를 맞추기 위해 약간 줄임
        float throwActionTime = 0.1f;              // 던지는 뻗기 동작 시간

        // 1. 준비 동작 (뒤로 당기기)
        attackAnimSeq.Append(transform.DOLocalMove(new Vector3(0.1f, -0.1f, -0.2f), recoilTime).SetEase(Ease.OutQuad));
        attackAnimSeq.Join(transform.DOLocalRotate(new Vector3(-20f, 10f, 0f), recoilTime).SetEase(Ease.OutQuad));

        // 2. 던지는 시점 (발사 및 모델 숨기기)
        attackAnimSeq.AppendCallback(() => {
            _throwableModel.gameObject.SetActive(false);
            ThrowProjectile();
        });

        // 3. 던지는 동작 (팔 뻗기)
        attackAnimSeq.Append(transform.DOLocalMove(new Vector3(0f, 0f, 0.2f), throwActionTime).SetEase(Ease.OutCubic));

        // [핵심 2] 복귀 시작 직전에 모델을 다시 활성화!
        // OnComplete는 시퀀스가 끝까지 재생되어야만 실행되지만, 
        // AppendCallback은 시퀀스 흐름상 해당 타이밍에 무조건 실행됩니다.
        attackAnimSeq.AppendCallback(() => {
            _throwableModel.gameObject.SetActive(true);

            // [시작 상태] 크기는 0, 위치는 약간 아래에서 시작
            _throwableModel.localScale = Vector3.zero;
            _throwableModel.localPosition = _initialPosition + new Vector3(0, -0.1f, 0);

            // [애니메이션] 기억해둔 '초기값'으로 복구
            _throwableModel.DOScale(_initialScale, returnTime).SetEase(Ease.OutBack);
            _throwableModel.DOLocalMove(_initialPosition, returnTime).SetEase(Ease.OutCubic);
            _throwableModel.DOLocalRotateQuaternion(_initialRotation, returnTime).SetEase(Ease.OutCubic);
        });

        // 4. 복귀 동작 (원래 위치로)
        attackAnimSeq.Append(transform.DOLocalMove(Vector3.zero, returnTime).SetEase(Ease.OutCubic));
        attackAnimSeq.Join(transform.DOLocalRotate(Vector3.zero, returnTime).SetEase(Ease.OutCubic));

        //attackAnimSeq.AppendCallback(() => _throwableModel.gameObject.SetActive(true));
        //attackAnimSeq.AppendCallback(() => {
        //    _throwableModel.gameObject.SetActive(true);

        //    // [시작 상태] 크기는 0, 위치는 약간 아래에서 시작
        //    _throwableModel.localScale = Vector3.zero;
        //    _throwableModel.localPosition = _initialPosition + new Vector3(0, -0.1f, 0);

        //    // [애니메이션] 기억해둔 '초기값'으로 복구
        //    _throwableModel.DOScale(_initialScale, returnTime).SetEase(Ease.OutBack);
        //    _throwableModel.DOLocalMove(_initialPosition, returnTime).SetEase(Ease.OutCubic);
        //    _throwableModel.DOLocalRotateQuaternion(_initialRotation, returnTime).SetEase(Ease.OutCubic);
        //});
    }



    private void ThrowProjectile()
    {
        Camera playerCamera = Camera.main;
        if (_throwablePrefab == null || playerCamera == null) return;

        // 1. 카메라 방향 데이터
        Vector3 camPos = playerCamera.transform.position;
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        Vector3 camUp = playerCamera.transform.up;
        Quaternion camRot = playerCamera.transform.rotation;

        // 2. 생성 위치 계산 (카메라 중심 오프셋)
        Vector3 spawnPos = camPos
                           + (camRight * _throwOffset.x)
                           + (camUp * _throwOffset.y)
                           + (camForward * _throwOffset.z);

        // 3. 생성 회전 (손 각도 유지)
        Quaternion finalRot = camRot * _throwableModel.localRotation;

        GameObject projectile = Instantiate(_throwablePrefab, spawnPos, finalRot);
        projectile.transform.localScale = _throwableModel.localScale;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null) rb = projectile.AddComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.maxAngularVelocity = 1000f;

            // 4. [무작위성 추가] 발사 방향에 살짝 오차 주기
            // camForward 방향에 아주 미세하게 위/아래/옆 랜덤 벡터를 섞습니다.
            Vector3 randomSpread = (camUp * Random.Range(-_spreadAmount, _spreadAmount))
                                 + (camRight * Random.Range(-_spreadAmount, _spreadAmount));
            Vector3 finalThrowDir = (camForward + randomSpread).normalized;

            rb.AddForce(finalThrowDir * _throwForce, ForceMode.Impulse);

            // 5. [무작위성 추가] 회전 방향을 삐딱하게 만들기
            // 정직한 덤블링 회전(camRight)에 약간의 상하좌우 비틀기를 섞습니다.
            Vector3 randomTorque = new Vector3(
                Random.Range(-_torqueRandomness, _torqueRandomness),
                Random.Range(-_torqueRandomness, _torqueRandomness),
                Random.Range(-_torqueRandomness, _torqueRandomness)
            );

            // 기본 회전축(Right)에 랜덤 비틀기 추가
            Vector3 mixedAngularVelocity = (camRight + randomTorque) * _flipSpeed;
            rb.angularVelocity = mixedAngularVelocity;
        }

        // 충돌 무시
        Collider playerCol = GetComponentInParent<Collider>();
        Collider projCol = projectile.GetComponent<Collider>();
        if (playerCol != null && projCol != null) Physics.IgnoreCollision(playerCol, projCol);
    }
}
