using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using static CartoonFX.CFXR_Effect;

public class Professor : MonoBehaviour, IAttackable
{

    public bool IsAttacking => throw new System.NotImplementedException();

    public int CurrentAttackID => throw new System.NotImplementedException();
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private bool _isSwapWheelnvert = false; // true면 방향이 반대가 됨
    [SerializeField] private float _sprintStaminaDrain = 20f;
    [SerializeField] private float _staminaRegenRate = 5f;
    [SerializeField] private PlayerCamera _playerCamera;

    private Rigidbody _rigidbody;
    private FirstPersonController _controller;
    private PlayerInteraction _playerInteraction;
    private Stamina _stamina;
    private HealthVolume _healthVolume;
    private Health _health;
    private DamageReceiver _damageReceiver;
    private Collider _collider;
    private StatRecovery _statRecovery;

    public UnityEvent DieEvent = new();

    private void Awake()
    {
        _healthVolume = GetComponent<HealthVolume>();
        _health = GetComponent<Health>();
        _health.DecreaseEvent.AddListener(_ => _healthVolume.AdjustVolume(_health.Ratio));
        _health.IncreaseEvent.AddListener(_ => _healthVolume.AdjustVolume(_health.Ratio));
        _damageReceiver = GetComponent<DamageReceiver>();
        _damageReceiver.StatDownEvent.AddListener(OnDamaged);
        _damageReceiver.DepletedEvent.AddListener(Die);
        _rigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<FirstPersonController>();
        _playerInteraction = GetComponent<PlayerInteraction>();
        _stamina = GetComponent<Stamina>();
        _stamina.Initialize();
        _collider = GetComponent<Collider>();
        _statRecovery = GetComponent<StatRecovery>();
    }


    private void Start()
    {
        _health.ResetEvent.AddListener(_ => _healthVolume.AdjustVolume(_health.Ratio));
        _playerCamera.DisablePhysics();
        _weaponController.EquipWeapon(0, gameObject);
        _healthVolume.AdjustVolume(_health.Ratio);
    }



    private void Update()
    {
        // if (Input.GetMouseButtonDown(0) && CanAttack())
        // {
        //     attackAnimator.PlayMeleeSwing(Attack);
        // }
        if (_health.IsDepleted) return;
        HandleSprintStamina();
        HandleWeaponAttack();
        HandleWeaponSwap();
    }



    private void Die(HitInfo hitInfo)
    {
        _statRecovery.enabled = false;
        _collider.enabled = false;
        _controller.enabled = false;
        _weaponController.Hide();
        _playerCamera.ApplyDeathPhysics(hitInfo);
        hitInfo.attacker.GetComponent<PostStudent>().UnFocusProfessorAttack();
        DieEvent?.Invoke();
    }



    private void OnDamaged(HitInfo hitInfo, float amount)
    {
        CameraShaker.Instance.DoDamagedShake(amount);
    }



    public void UnsetTaskPose()
    {
        _controller.enabled = true;
        _rigidbody.isKinematic = false;
        _weaponController.Show();
        _playerCamera.DisableTaskMode();
    }



    public void SetTaskPose()
    {
        _controller.StopSprinting();
        _controller.enabled = false;
        _rigidbody.isKinematic = true;
        _weaponController.Hide();
        _playerCamera.EnableTaskMode(transform.forward);
    }



    private void HandleSprintStamina()
    {
        if (_controller && _controller.IsSprinting)
        {
            _stamina.Decrease(_sprintStaminaDrain *  Time.deltaTime);
        }
        else
        {
            _stamina.Increase(_staminaRegenRate * Time.deltaTime);
        }
        //else if (_weaponController.CurrentWeapon.IsPlayingAttackAnim == false)
        //{
        //    _stamina.Increase(_staminaRegenRate * Time.deltaTime);
        //}
    }



    private void HandleWeaponAttack()
    {
        if (_weaponController.IsHiding) return;
        if (Input.GetMouseButtonDown(0))
        {
            float currentWeaponStaminaCost = _weaponController.CurrentWeapon.StaminaCost;
            if (_stamina.Current < currentWeaponStaminaCost)
            {
                Debug.Log("스테미나가 부족합니다!");
                return;
            }
            if (_weaponController.TryAttack())
            {
                _stamina.Decrease(currentWeaponStaminaCost);
                _playerInteraction.CancelActiveInteraction();
            }
        }
    }



    private void HandleWeaponSwap()
    {
        if (_weaponController.IsHiding) return;
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



    public void TakeDamage(float amount, Vector3 hitPoint, GameObject attacker)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        
    }
}