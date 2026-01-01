using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Professor : MonoBehaviour, IDamageable, IAttackable
{
    public bool IsDead => false;

    public bool IsInvincible => false;

    public Vector3 Position => transform.position;

    public bool IsAttacking => throw new System.NotImplementedException();

    public int CurrentAttackID => throw new System.NotImplementedException();
    private AttackAnimator attackAnimator;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private bool _isSwapWheelnvert = false; // true면 방향이 반대가 됨

    private void Start()
    {
        attackAnimator = GetComponent<AttackAnimator>();
        _weaponController.EquipWeapon(0);
    }



    private void Update()
    {
        // if (Input.GetMouseButtonDown(0) && CanAttack())
        // {
        //     attackAnimator.PlayMeleeSwing(Attack);
        // }
        HandleWeaponAttack();
        HandleWeaponSwap();
    }



    private void HandleWeaponAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _weaponController.TryAttack();
        }
    }



    private void HandleWeaponSwap()
    {
        // 숫자키 입력 예시
        for (int i = 0; i < _weaponController.WeaponCount; i++)
        {
            // KeyCode.Alpha1에 i를 더하면 Alpha2, Alpha3... 순서로 체크 가능합니다.
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _weaponController.ChangeWeapon(i);
                break; // 해당 프레임에서 무기를 바꿨다면 루프 탈출
            }
        }
        
        // 휠 입력 예시
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel != 0)
        {
            bool isScrollDown = wheel < 0; 
            bool finalNext = isScrollDown ^ _isSwapWheelnvert;
            _weaponController.ChangeWeaponByWheel(finalNext);
        }
    }



    private bool CanAttack()
    {
        return !attackAnimator.IsSwinging;
    }



    public void TakeDamage(float amount, Vector3 hitPoint, GameObject attacker)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        
    }
}