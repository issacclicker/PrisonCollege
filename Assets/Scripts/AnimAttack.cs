using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class AnimAttack : MonoBehaviour
{
    [SerializeField] private DamageData _damageData;
    [SerializeField] private float _hitImpulse = 100;
    [SerializeField] private float _attackRadius = 1.5f;   // 구체의 반지름
    [SerializeField] private float _attackDistance = 2.0f; // 캐릭터 정면으로 뻗어나갈 거리
    [SerializeField] private LayerMask targetLayer;



    public void OnAttackHit()
    {
        // 1. 캐릭터 정면 방향으로 SphereCastAll 실행
        // 시작 지점: 현재 위치 + 약간 위(허리 높이), 방향: 정면
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, _attackRadius, direction, _attackDistance, targetLayer);

        // 2. 검색된 모든 대상에게 데미지 적용
        foreach (var hit in hits)
        {
            // 자기 자신 제외 (혹시 레이어 설정이 겹칠 경우)
            if (hit.collider.gameObject == gameObject) continue;

            if (hit.collider.TryGetComponent(out DamageReceiver receiver))
            {
                // 공통 정보를 담은 HitInfo 생성
                Vector3 contactPoint = hit.GetContactPoint(origin);
                Vector3 normal = hit.GetNormal(direction);
                HitInfo hitInfo = new HitInfo(contactPoint, Quaternion.LookRotation(normal), gameObject, _hitImpulse);

                // 효과 적용 (다형성 실행)
                receiver.TakeEffect(_damageData, hitInfo);
            }
        }
    }
}
