using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[RequireComponent(typeof(WeaponAnimator))]
public class WeaponBase : MonoBehaviour
{
    [Header("--- Base ---")]
    [SerializeField] protected WeaponData _weaponData;
    protected GameObject _owner;
    private WeaponAnimator _animator;
    public bool IsPlayingAttackAnim => _animator.IsPlayAttackAnim;


    private void Awake()
    {
        _owner = GetComponentInParent<WeaponController>().FirstPersonController.gameObject;
        _animator = GetComponent<WeaponAnimator>();
    }


    public void PlayAttackAnim()
    {
        _animator.StartAttack(ExecuteAttack, _weaponData.animLength);
    }



    public void PlayHolsterAnim(float duration, System.Action onComplete)
    {
        _animator.Holster(duration, onComplete);
    }

    public void PlayDrawAnim(float duration)
    {
        _animator.Draw(duration);
    }


    protected virtual void ExecuteAttack() { }
}
