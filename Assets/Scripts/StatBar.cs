using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StatBar : MonoBehaviour
{
    [SerializeField] private Stat _targetStat;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Gradient _colorGradient;



    private void Start()
    {
        OnStatChanged(0);
        _targetStat.IncreaseEvent.AddListener(OnStatChanged);
        _targetStat.DecreaseEvent.AddListener(OnStatChanged);
    }



    private void OnStatChanged(float amount)
    {
        float ratio = Mathf.Clamp01(_targetStat.Ratio);
        _fillImage.fillAmount = ratio;
        _fillImage.color = _colorGradient.Evaluate(ratio);
    }
}
