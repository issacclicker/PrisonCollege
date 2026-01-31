using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _minImpactThreshold = 5.0f;
    [SerializeField] private float _lifeTime = 5.0f;
    [SerializeField] private bool _destroyOnHit = false;

    public WeaponData WeaponData { get; set; }
    public GameObject Owner { get; set; }

    // 중복 충돌을 방지하기 위한 셋 (오브젝트 참조 저장)
    private HashSet<GameObject> _hitObjects = new HashSet<GameObject>();

    protected virtual void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. 레이어 체크 및 주인 제외
        if (collision.gameObject.IsInLayerMask(Global.STUDENT_LAYER_NAME) == false) return;
        if (collision.gameObject == Owner) return;

        // 2. ★ 이미 맞은 대상인지 체크 ★
        if (_hitObjects.Contains(collision.gameObject)) return;

        float impactVelocity = collision.relativeVelocity.magnitude;
        // 3. 충격량 체크
        float impactForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        if (impactVelocity < _minImpactThreshold)
        {
            Debug.Log($"충격이 너무 약함 ({impactForce:F2}), 무시합니다.");
            return;
        }

        // 4. 데미지 전달
        if (WeaponData == null || WeaponData.effect == null) return;
        EffectReceiver receiver = WeaponData.effect.GetActorReceiver(collision.gameObject);
        if (receiver && receiver.CanEffect)
        {
                // 목록에 추가하여 다시 맞지 않게 함
            _hitObjects.Add(collision.gameObject);

            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            HitInfo hitInfo = new HitInfo(hitPoint, Quaternion.LookRotation(hitNormal), Owner, WeaponData.hitImpulse);

            receiver.TakeEffect(WeaponData.effect, hitInfo);

            // 만약 관통형이 아니라 첫 충돌에 바로 사라져야 한다면 아래 주석 해제
            if (_destroyOnHit)
                Destroy(gameObject); 
        }
    }
}