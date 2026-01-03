using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] private float _maxStat = 100f;
    private float _currentStat;

    public float Current => _currentStat;
    public float Max => _maxStat;
    public float Ratio => _currentStat / _maxStat;
    public bool IsDepleted => _currentStat <= 0;

    protected virtual void Awake() => _currentStat = _maxStat;



    public void Decrease(float amount)
    {
        if (IsDepleted) return;
        _currentStat = Mathf.Max(0, _currentStat - amount);
    }

    public void Increase(float amount)
    {
        _currentStat = Mathf.Min(_maxStat, _currentStat + amount);
    }
}
