using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public EffectData EffectData { get; set; }
    public GameObject Owner { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IHittable target))
        {
            ContactPoint contact = collision.contacts[0];

            Vector3 hitPoint = contact.point;     // 충돌 지점
            Vector3 hitNormal = contact.normal;   // 충돌 지점의 각도 (법선 벡터)
            Quaternion hitRotation = Quaternion.LookRotation(hitNormal);

            target.TakeHit(EffectData, hitPoint, hitRotation, Owner);
        }
    }
}
