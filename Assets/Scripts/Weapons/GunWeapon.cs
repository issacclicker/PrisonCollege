using System.IO.Pipes;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GunWeapon : WeaponBase
{
    [SerializeField] private float _range = 100f; // 사거리
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _penetrableLayer;
    [SerializeField] private int _initialBullets;
    [SerializeField] private GameObject _bulletHolePrefab;

    public override string TypeName => "BB탄총";
    private Stat _magazine;
    public override bool CanAttack => base.CanAttack && !_magazine.IsDepleted;



    protected override void Awake()
    {
        base.Awake();
        _magazine = GetComponent<Stat>();
        _magazine.Initialize(true);
        _magazine.Increase(_initialBullets);
    }



    protected override void ExecuteAttack()
    {
        ShotBullet();
    }



    private void ShotBullet()
    {
        if (_magazine.IsDepleted) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, _range, _targetLayer | _penetrableLayer);

        // 2. 거리순으로 정렬 (가까운 곳부터 순차적으로 처리)
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.IsInLayerMask(_targetLayer))
            {
                EffectReceiver receiver = _weaponData.effect.GetActorReceiver(hit.collider.gameObject);
                if (receiver && receiver.CanEffect)
                {
                    Vector3 contactPoint = hit.GetContactPoint(ray.origin);
                    Vector3 safeNormal = hit.GetNormal(ray.direction);

                    // HitInfo 구성
                    HitInfo hitInfo = new HitInfo(
                        contactPoint,
                        Quaternion.LookRotation(safeNormal),
                        this.gameObject,
                        _weaponData.hitImpulse
                    );
                    receiver.TakeEffect(_weaponData.effect, hitInfo);
                }
            }

            // 대상의 레이어가 블록 레이어에 포함되어 있는지 확인
            if (hit.collider.gameObject.IsInLayerMask(_penetrableLayer))
            {
                GenerateBulletHole(hit);
                Debug.Log($"{hit.collider.name}에 탄흔을 생성했습니다.");
            }
        }

        _magazine.Decrease(1);
        InfoUpdateEvent?.Invoke(this); // 필요시 주석 해제
    }

    private void GenerateBulletHole(RaycastHit hit)
    {
        if (_bulletHolePrefab == null) return;
        GameObject hole = Instantiate(_bulletHolePrefab, hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
        hole.transform.SetParent(hit.transform);
        Destroy(hole, 5f);
    }



    public bool Acquire(int count)
    {
        if (_magazine.IsMax) return false;
        _magazine.Increase(count);
        InfoUpdateEvent?.Invoke(this);
        return true;
    }
}
