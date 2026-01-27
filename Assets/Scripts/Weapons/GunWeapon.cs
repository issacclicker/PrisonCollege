using UnityEngine;

public class GunWeapon : WeaponBase
{
    [SerializeField] private int _initialBullets;
    public override string TypeName => "¿¡¾î°Ç";
    private Stat _magazine;
    public override bool CanAttack => base.CanAttack && !_magazine.IsDepleted;



    protected override void Awake()
    {
        base.Awake();
        _magazine = GetComponent<Stat>();
        _magazine.Initialize(true);
        _magazine.Increase(_initialBullets);
    }



    public bool Acquire(int count)
    {
        if (_magazine.IsMax) return false;
        _magazine.Increase(count);
        InfoUpdateEvent?.Invoke(this);
        return true;
    }
}
