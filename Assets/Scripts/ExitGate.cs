using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

public class ExitGate : MonoBehaviour
{
    [SerializeField] private Transform _barricadeParent;
    [SerializeField] private GameObject _barricadePrefab;
    [SerializeField] private bool _isbarricadeEnabled;

    protected DamageReceiver _damageReceiver;
    protected ClickAndWait _interaction;
    protected GameObject _barricadePlaced;
    protected StatRecovery _statRecovery;
    protected ExplosionShacker _explosionShacker;
    private Health _health;

    public bool IsBarricadePlaced => _barricadePlaced != null;
    public virtual ExitGateType GateType => ExitGateType.None;
    public float HealthRatio => _health ? _health.Ratio : 0.0f;



    protected virtual void Awake()
    {
        _health = GetComponent<Health>();
        _explosionShacker = GetComponent<ExplosionShacker>();
        _damageReceiver = GetComponent<DamageReceiver>();
        _interaction = GetComponent<ClickAndWait>();
        _statRecovery = GetComponent<StatRecovery>();

        _interaction.ProgressCompleteEvent.AddListener(PlaceBarricade);
        _damageReceiver.StatDownEvent.AddListener((_, decreasion) => OnDamaged(decreasion));
        _damageReceiver.DepletedEvent.AddListener(_ => BreakBarricade());
        Close();
    }



    private void Start()
    {
        if (_isbarricadeEnabled)
            PlaceBarricade();
        else
            BreakBarricade();
    }



    private void OnDamaged(float decreasion)
    {
        if (decreasion / _health.Max > 0.99)
        {
            _explosionShacker.PlayShake();
        }
    }



    protected virtual void PlaceBarricade()
    {
        _interaction.SetInteractable(false);
        _barricadePlaced = Instantiate(_barricadePrefab, _barricadeParent);
        _damageReceiver.SetStatFull();
        _statRecovery.CanRecover = true;
    }



    protected virtual void BreakBarricade()
    {
        _interaction.SetInteractable(true);
        Destroy(_barricadePlaced);
        _barricadePlaced = null;
        _damageReceiver.SetStatEmpty();
        _statRecovery.CanRecover = false;
    }

    public virtual void Open() { }

    public virtual void Close() { }
}
