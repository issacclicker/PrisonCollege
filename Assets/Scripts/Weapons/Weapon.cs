using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponAnimator))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage;
    private WeaponAnimator _animator;
    public bool IsPlayingAttackAnim => _animator.IsPlayAttackAnim;


    private void Awake()
    {
        _animator = GetComponent<WeaponAnimator>();
    }


    public void PlayAttackAnim()
    {
        _animator.StartAttack();
    }



    public void PlayHolsterAnim(float duration, System.Action onComplete)
    {
        _animator.Holster(duration, onComplete);
    }

    public void PlayDrawAnim(float duration)
    {
        _animator.Draw(duration);
    }
}
