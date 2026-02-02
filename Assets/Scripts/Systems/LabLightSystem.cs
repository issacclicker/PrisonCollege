using UnityEngine;
using UnityEngine.Events;

public class LabLightSystem : SceneSingleton<LabLightSystem>
{
    [SerializeField] private GameObject _toggleableLightGroup;
    public bool IsLightsOn => _toggleableLightGroup.activeSelf;

    [HideInInspector] public UnityEvent LightsOffEvent = new();
    [HideInInspector] public UnityEvent LightsOnEvent = new();



    protected override void Awake()
    {
        base.Awake();
    }



    private void Start()
    {
        TurnOn();
    }



    public void TurnOff()
    {
        if (!IsLightsOn) return;
        Debug.Log("LightOff");
        _toggleableLightGroup.SetActive(false);
        LightsOffEvent?.Invoke();
    }



    public void TurnOn()
    {
        if (IsLightsOn) return;
        Debug.Log("LightOn");
        _toggleableLightGroup.SetActive(true);
        LightsOnEvent?.Invoke();
    }
}
