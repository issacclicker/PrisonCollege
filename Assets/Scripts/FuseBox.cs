using DG.Tweening;
using UnityEngine;

public class FuseBox : MonoBehaviour
{
    [SerializeField] private Light _redLight;
    private ClickAndWait _interaction;
    private Tween _blinkTween;



    private void Awake()
    {
        _interaction = GetComponent<ClickAndWait>();
        _interaction.ProgressCompleteEvent.AddListener(SwitchOnLights);
        LabLightSystem.Instance.LightsOffEvent.AddListener(LightsTurnedOff);
        LabLightSystem.Instance.LightsOnEvent.AddListener(LightsTurnedOn);
    }



    private void Start()
    {
        LightsTurnedOn();
    }



    private void SwitchOnLights()
    {
        LabLightSystem.Instance.TurnOn();
    }



    private void LightsTurnedOn()
    {
        _interaction.SetInteractable(false);
        _blinkTween?.Kill();
        _redLight.intensity = 0f;
        _redLight.enabled = false;
    }



    private void LightsTurnedOff()
    {
        _interaction.SetInteractable(true);
        _blinkTween?.Kill();
        _redLight.enabled = true;
        _redLight.intensity = 0f;
        _blinkTween = _redLight.DOIntensity(1, 0.5f)
            .SetLoops(-1, LoopType.Yoyo) // 무한 반복(-1), 왔다 갔다(Yoyo)
            .SetEase(Ease.InOutSine);    // 부드럽게 깜빡임 (취향껏 변경 가능)
    }
}
