using UnityEngine;
using DG.Tweening;

public class SprinklerController : MonoBehaviour
{
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _targetEmission;
    private ParticleSystem[] _rainParticles;
    private float _currentEmissionRate = 0f; // 현재 보간 값을 저장할 변수
    private Tweener _emissionTweener;

    private void Awake()
    {
        _rainParticles = GetComponentsInChildren<ParticleSystem>();
    }

    public void TurnOn()
    {
        PlayEmissionTween(_targetEmission, _fadeDuration);
    }

    public void TurnOff()
    {
        PlayEmissionTween(0f, _fadeDuration);
    }

    public void TurnOffImmediate()
    {
        _emissionTweener?.Kill();
        SetEmissionRate(0f);

        foreach (var ps in _rainParticles)
        {
            if (ps == null) continue;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void PlayEmissionTween(float targetValue, float duration)
    {
        _emissionTweener?.Kill();
        _emissionTweener = DOTween.To(() => _currentEmissionRate,
                                     x => SetEmissionRate(x),
                                     targetValue,
                                     duration)
                                  .SetEase(Ease.OutQuad); // 자연스러운 가속/감속
    }

    private void SetEmissionRate(float rate)
    {
        _currentEmissionRate = rate;

        foreach (var ps in _rainParticles)
        {
            if (ps == null) continue;

            var emission = ps.emission;
            emission.rateOverTime = rate;
            if (rate > 0.1f && !ps.isPlaying) ps.Play();
            else if (rate <= 0.1f && ps.isPlaying) ps.Stop();
        }
    }
}