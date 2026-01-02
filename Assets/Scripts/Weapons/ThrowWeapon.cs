using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowWeapon : WeaponBase
{
    [Header("--- Throw ---")]
    [SerializeField] private GameObject _throwablePrefab; // 던질 물체 프리팹
    [SerializeField] private Transform _throwableModel; // 던질 물체 프리팹
    [SerializeField] private float _throwForce = 15f;     // 던지는 힘
    [SerializeField] private Vector3 _throwOffset = new Vector3(0.5f, -0.2f, 1.0f);
    [SerializeField] private float _flipSpeed = 20f; // 위아래 회전 속도 (높을수록 빠름)
    [Range(0f, 1f)] public float _spreadAmount = 0.02f; // 탄퍼짐 정도
    [Range(0f, 1f)] public float _torqueRandomness = 0.5f; // 회전 불규칙도



    protected override void ExecuteAttack()
    {
        ThrowProjectile();
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

        GameObject projectileObj = Instantiate(_throwablePrefab, spawnPos, finalRot);
        projectileObj.transform.localScale = _throwableModel.localScale;
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.EffectData = _weaponData.effect;
        projectile.Owner = _owner;

        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
        if (rb == null) rb = projectileObj.AddComponent<Rigidbody>();

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
        Collider projCol = projectileObj.GetComponent<Collider>();
        if (playerCol != null && projCol != null) Physics.IgnoreCollision(playerCol, projCol);
    }
}
