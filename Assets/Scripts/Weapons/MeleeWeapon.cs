using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class MeleeWeapon : WeaponBase
{
    [Header("--- Melee ---")]
    [SerializeField] private float _attackRange = 3.5f;
    [SerializeField] private float _attackRadius = 0.7f;  // 판정 두께 (구체 반지름)
    [SerializeField] private LayerMask _hitLayer;      // 대상 레이어 (Enemy, Obstacle 등)
    [SerializeField] private LayerMask _blockLayer;

    protected override void ExecuteAttack() => PerformMeleeAttack(_attackRange, _attackRadius, _hitLayer, _blockLayer);


    protected void PerformMeleeAttack(float range, float radius, LayerMask hitLayer, LayerMask blockLayer)
    {
        Transform cam = Camera.main.transform;
        Vector3 origin = cam.position;
        Vector3 direction = cam.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, range, hitLayer | blockLayer);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj == _owner) continue;

            // 1. 유틸리티 함수: 벽 레이어 체크
            if (hitObj.IsInLayerMask(blockLayer))
            {
                Debug.Log("벽에 막힘!");
                break;
            }

            if (hit.collider.TryGetComponent(out DamageReceiver receiver))
            {
                // 2. 유틸리티 함수: 안전한 위치 및 회전값 계산
                Vector3 contactPoint = hit.GetContactPoint(origin);
                Vector3 normal = hit.GetNormal(direction);
                HitInfo hitInfo = new HitInfo(contactPoint, Quaternion.LookRotation(normal), _owner, _weaponData.hitImpulse);
                receiver.TakeEffect(_weaponData.effect, hitInfo);
            }
        }
    }
}
