using System.Collections.Generic;
using UnityEngine;

public class OverlapAttacker : MonoBehaviour
{
    private bool _isAttacking = false;
    private HashSet<GameObject> _hitTargets = new HashSet<GameObject>();

    [Header("Settings")]
    [SerializeField] private DamageData _damageDate;
    [SerializeField] private float _hitImpulse;

    [Header("Layer Filters")]
    [SerializeField] private LayerMask _victimOnlyLayer; // 얘네는 맞기만 함 (예: Enemy)
    [SerializeField] private LayerMask _bothDamageLayer; // 닿으면 양쪽 다 데미지 (예: Trap, Destructible)

    public void StartAttack()
    {
        _hitTargets.Clear();
        _isAttacking = true;
    }

    public void StopAttack() => _isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttacking) return;

        // 1. 최상위 부모 기준으로 중복 체크
        GameObject rootTarget = other.transform.root.gameObject;
        if (_hitTargets.Contains(rootTarget)) return;

        int targetLayer = other.gameObject.layer;
        bool isVictimOnly = ((1 << targetLayer) & _victimOnlyLayer) != 0;
        bool isBothDamage = ((1 << targetLayer) & _bothDamageLayer) != 0;

        if (!isVictimOnly && !isBothDamage) return; // 설정 안 된 레이어는 무시

        // 2. 공통 데이터 계산 (Contact Point 등)
        Vector3 origin = transform.GetComponent<Collider>().bounds.center;
        Vector3 contactPoint = other.ClosestPoint(origin);
        Vector3 normal = (origin - contactPoint).normalized;
        if (normal == Vector3.zero) normal = -transform.forward;

        HitInfo hitInfoToOther = new HitInfo(contactPoint, Quaternion.LookRotation(normal), gameObject, _hitImpulse);

        // 3. 상대방 공격 (VictimOnly 또는 BothDamage일 때)
        if (other.TryGetComponent(out DamageReceiver otherReceiver))
        {
            otherReceiver.TakeEffect(_damageDate, hitInfoToOther);
            _hitTargets.Add(rootTarget);
        }

        // 4. 자신도 피격 (BothDamage 레이어일 때만)
        //if (isBothDamage)
        //{
        //    if (transform.root.TryGetComponent(out DamageReceiver myReceiver))
        //    {
        //        // 자신에게 오는 HitInfo는 방향을 반대로 설정
        //        HitInfo hitInfoToMe = new HitInfo(contactPoint, Quaternion.LookRotation(-normal), other.gameObject, _hitImpulse);
        //        myReceiver.TakeEffect(_damageDate, hitInfoToMe);
        //    }
        //}
        if (isBothDamage)
        {
            if (transform.root.TryGetComponent(out DamageReceiver myReceiver))
            {
                // 1. 낑김 방지를 위한 미세 위치 보정 (Push-out)
                // normal은 충돌지점에서 나를 향하는 방향이므로, 이 방향으로 살짝 밀어줍니다.
                float pushDist = 1f; // 약 5cm 정도 미세하게 밀어내기
                transform.root.position += normal * pushDist;

                // 2. 자신에게 오는 HitInfo 설정 (방향은 유지)
                // Quaternion.LookRotation(-normal)은 이펙트가 나를 향하게 만듭니다.
                HitInfo hitInfoToMe = new HitInfo(
                    contactPoint,
                    Quaternion.LookRotation(-normal),
                    other.gameObject,
                    _hitImpulse
                );

                // 3. 효과 적용
                myReceiver.TakeEffect(_damageDate, hitInfoToMe);
            }
        }
    }
}