using UnityEngine;

public class GunWeapon : WeaponBase, ICountableWeapon
{
    public override string TypeName => "¿¡¾î°Ç";
    private int _count = 0;
    public int Amount => _count;
    public override bool CanAttack => base.CanAttack && Amount > 0;

    public void Acquire(int count)
    {
        _count += count;
        InfoUpdateEvent?.Invoke(this);
    }
}
