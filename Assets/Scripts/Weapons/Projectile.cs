using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _minImpactThreshold = 5.0f;
    [SerializeField] private float _lifeTime = 5.0f;

    public WeaponData WeaponData { get; set; }
    public GameObject Owner { get; set; }



    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.IsInLayerMask(Global.STUDENT_LAYER_NAME) == false) return;
        if (collision.gameObject == Owner) return;

        float impactForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        if (impactForce < _minImpactThreshold)
        {
            Debug.Log($"충격이 너무 약함 ({impactForce:F2}), 무시합니다.");
            return;
        }

        if (collision.gameObject.TryGetComponent(out DamageReceiver receiver))
        {
            ContactPoint contact = collision.contacts[0];

            Vector3 hitPoint = contact.point;     // 충돌 지점
            Vector3 hitNormal = contact.normal;   // 충돌 지점의 각도 (법선 벡터)
            HitInfo hitInfo = new HitInfo(hitPoint, Quaternion.LookRotation(hitNormal), Owner, WeaponData.hitImpulse);

            receiver.TakeEffect(WeaponData.effect, hitInfo);
        }
    }
}
