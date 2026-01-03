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
    public float StaminaCost => _weaponData.staminaCost;


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




[System.Serializable] // 인스펙터나 이벤트에서 확인하기 위해 추가
public struct HitInfo
{
    public Vector3 hitPoint;
    public Quaternion hitRotation;
    public GameObject attacker;
    public float impulse; // 아까 사용하던 충격량도 포함하면 좋습니다.

    // 생성자를 만들어두면 사용할 때 편합니다.
    public HitInfo(Vector3 point, Quaternion rotation, GameObject attacker, float impulse = 0f)
    {
        this.hitPoint = point;
        this.hitRotation = rotation;
        this.attacker = attacker;
        this.impulse = impulse;
    }
}