using DigitalRuby.RainMaker;
using UnityEngine;

public class ThrowWeapon2 : RangedWeapon
{
    [SerializeField] private float _throwVelocity;
    [SerializeField] private float _flipVelocity;
    [SerializeField] private float _torqueRandomness;
    private ThrowAnimator _throwAnimator;

    public override string TypeName => "ÅõÃ´";



    protected override void Awake()
    {
        base.Awake();
        _throwAnimator = GetComponent<ThrowAnimator>();
    }


    protected override void Shot(Vector3 viewportPoint)
    {
        Vector3 shotDestination = GetShotDestination(viewportPoint);
        Vector3 shotDirection = (shotDestination - _spawnPoint.position).normalized;
        Quaternion projectileRot = Camera.main.transform.rotation * _spawnPoint.localRotation;
        GameObject projectileSpawned = Instantiate(_projectilePrefab, _spawnPoint.position, projectileRot);
        projectileSpawned.transform.localScale = _spawnPoint.localScale;
        Projectile projectile = projectileSpawned.GetComponent<Projectile>();
        projectile.WeaponData = _weaponData;
        projectile.Owner = _owner;
        projectile.ResetForce();
        projectile.AddVelocityForce(shotDirection, _throwVelocity);
        projectile.AddTorqueForce(GetRandomTorgue(), _flipVelocity);

        //Debug.DrawRay(shotDestination, Vector3.up * 0.5f, Color.green, 1.0f);
        //Debug.DrawRay(shotDestination, Vector3.right * 0.5f, Color.green, 1.0f);
    }



    private Vector3 GetRandomTorgue()
    {
        Vector3 randomTorque = new Vector3(
                Random.Range(-_torqueRandomness, _torqueRandomness),
                Random.Range(-_torqueRandomness, _torqueRandomness),
                Random.Range(-_torqueRandomness, _torqueRandomness)
            );
        return randomTorque + Camera.main.transform.right;
    }



    private Vector3 GetShotDestination(Vector3 viewportPoint)
    {
        Ray ray = Camera.main.ViewportPointToRay(viewportPoint);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Çã°øÀ» ½úÀ» ¶§
            targetPoint = ray.GetPoint(_maxDistance);
        }
        return targetPoint;
    }



    protected override bool Acquire(int count)
    {
        if (base.Acquire(count) == false) return false;
        _throwAnimator.PlayRefillAnimation();
        return true;
    }


    protected override void CheckBullet()
    {
        base.CheckBullet();
        if (!_magazine.IsDepleted)
        {
            _throwAnimator.PlayRefillAnimation();
        }
        else
        {
            _spawnPoint.gameObject.SetActive(false);
        }
    }
}
