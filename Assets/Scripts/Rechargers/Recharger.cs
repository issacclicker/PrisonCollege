using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public abstract class Recharger : MonoBehaviour
{
    [SerializeField] protected WeaponController _weaponCtrl;
    private List<WeaponBase> _targetWeapons;
    private Click _interaction;
    private Stat _supplyProgress;
    private bool _canRecharge = false;



    private void Awake()
    {
        _interaction = GetComponent<Click>();
        _supplyProgress = GetComponent<Stat>();
        _interaction.ActionName = GetActionName();
        _targetWeapons = GetTargetWeapons();
        _supplyProgress.Initialize(true);
        _supplyProgress.MaxReachEvent.AddListener(() => _canRecharge = true);
        _interaction.ClickEvent.AddListener(RechargeWeapons);
    }



    private void Update()
    {
        if (!_canRecharge)
        {
            _supplyProgress.Increase(Time.deltaTime);
        }
        _interaction.FillAmount = _supplyProgress.Ratio;
    }


    protected abstract string GetActionName();
    protected abstract List<WeaponBase> GetTargetWeapons();



    private void RechargeWeapons()
    {
        if (!_canRecharge) return;
        bool recharged = false;
        foreach (var weapon in _targetWeapons)
        {
            GunWeapon gunWeapon = weapon as GunWeapon;
            ThrowWeapon throwWeapon = weapon as ThrowWeapon;
            recharged |= (throwWeapon?.Fill() ?? false) || (gunWeapon?.Fill() ?? false);
        }

        if (recharged)
        {
            _canRecharge = false;
            _supplyProgress.Initialize(true);
        }
    }
}
