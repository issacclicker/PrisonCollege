using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stat : MonoBehaviour
{
    [SerializeField] private float _maxStat = 100f;
    private float _currentStat;

    public float Current => _currentStat;
    public float Max => _maxStat;
    public float Ratio => _currentStat / _maxStat;
    public bool IsDepleted => _currentStat <= 0;
    public bool IsMax => _currentStat >= _maxStat;

    public UnityEvent<float> IncreaseEvent = new UnityEvent<float>();
    public UnityEvent<float> DecreaseEvent = new UnityEvent<float>();
    public UnityEvent DepleteEvent = new UnityEvent();

    protected virtual void Awake() => _currentStat = _maxStat;



    public void Decrease(float amount)
    {
        if (IsDepleted) return;
        _currentStat = Mathf.Max(0, _currentStat - amount);
        DecreaseEvent?.Invoke(amount);
    }

    public void Increase(float amount)
    {
        _currentStat = Mathf.Min(_maxStat, _currentStat + amount);
        IncreaseEvent?.Invoke(amount);
    }

    private void Depleted()
    {
        DepleteEvent?.Invoke();
    }
}
