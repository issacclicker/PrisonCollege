using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public WeaponData WeaponData { get; set; }
    public GameObject Owner { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
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
