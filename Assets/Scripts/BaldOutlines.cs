using UnityEngine;
using UnityEngine.UI;

public class BaldOutlines : MonoBehaviour
{
    private Outline[] outlines;
    private bool _activationState;


    private void Awake()
    {
        outlines = GetComponentsInChildren<Outline>();
        LabLightSystem.Instance.LightsOffEvent.AddListener(() => OnLightChanged(false));
        LabLightSystem.Instance.LightsOnEvent.AddListener(() => OnLightChanged(true));
    }




    private void Start()
    {
        _activationState = AttributeSystem.Instance.IsStudOutline;
        SetOutlines(_activationState);
    }


    private void SetOutlines(bool hasToEnable)
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = hasToEnable;
        }
    }


    private void OnLightChanged(bool isLightOn)
    {
        if (!_activationState) return;
        SetOutlines(isLightOn);
    }
}
