using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Professor : MonoBehaviour, IDamageable, IAttackable
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Sprint")]
    [SerializeField] private float _sprintMultiplier = 1.7f;

    [Header("Mouse Look")]
    [SerializeField] private float _mouseSensitivity = 100f;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _maxLookAngle = 80f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _xRotation = 0f;
    private float _speedRate = 1;

    public bool IsDead => false;

    public bool IsInvincible => false;

    public Vector3 Position => transform.position;

    public bool IsAttacking => throw new System.NotImplementedException();

    public int CurrentAttackID => throw new System.NotImplementedException();
    private AttackAnimator attackAnimator;

    private void Start()
    {
        attackAnimator = GetComponent<AttackAnimator>();
        return;
        _controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            attackAnimator.PlayMeleeSwing(Attack);
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