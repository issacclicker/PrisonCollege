using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StatBar : MonoBehaviour
{
    [SerializeField] protected Stat _targetStat;
    [SerializeField] protected Image _fillImage;
    [SerializeField] protected Gradient _colorGradient;



    protected virtual void Start()
    {
        OnStatChanged(0);
        _targetStat.IncreaseEvent.AddListener(OnStatChanged);
        _targetStat.DecreaseEvent.AddListener(OnStatChanged);
    }



    protected virtual void OnStatChanged(float amount)
    {
        UpdateUI(_targetStat.Ratio);
    }



    protected void UpdateUI(float ratio)
    {
        float clampedRatio = Mathf.Clamp01(ratio);
        _fillImage.fillAmount = clampedRatio;
        _fillImage.color = _colorGradient.Evaluate(clampedRatio);
    }
}
