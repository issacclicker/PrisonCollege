using UnityEngine;

public abstract class RangedWeapon : WeaponBase
{
    [Header("Ranged")]
    [SerializeField] protected float _maxDistance;
    [SerializeField] private float _spreadIntensity;
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected Transform _spawnPoint;
    protected Stat _magazine;

    public override string TypeName => "¿ø°Å¸®";
    public override bool CanAttack => base.CanAttack && !_magazine.IsDepleted;
    public float SpreadIntensity => _spreadIntensity;



    protected override void Awake()
    {
        base.Awake();
        _magazine = GetComponent<Stat>();
        _magazine.Initialize();
    }


    protected override void ExecuteAttack()
    {
        Vector3 viewportPoint = GetRandomViewportPoint();
        Shot(viewportPoint);
        _magazine.Decrease(1);
        CheckBullet();
        InfoUpdateEvent?.Invoke(this);
    }



    private Vector3 GetRandomViewportPoint()
    {
        Vector2 spreadOffset = Random.insideUnitCircle * _spreadIntensity;
        Vector3 viewportPoint = new Vector3(0.5f + spreadOffset.x, 0.5f + spreadOffset.y, 0);
        return viewportPoint;
    }



    protected virtual bool Acquire(int count)
    {
        if (_magazine.IsMax) return false;
        _magazine.Increase(count);
        InfoUpdateEvent?.Invoke(this);
        return true;
    }


    public bool Fill()
    {
        int fillAmount = (int)(_magazine.Max - _magazine.Current);
        return Acquire(fillAmount);
    }



    protected abstract void Shot(Vector3 shotDesination);

    protected virtual void CheckBullet() { }
}
