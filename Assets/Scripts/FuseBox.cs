using UnityEngine;

public class FuseBox : MonoBehaviour
{
    private ClickAndWait _interaction;



    private void Awake()
    {
        _interaction = GetComponent<ClickAndWait>();
        _interaction.ProgressCompleteEvent.AddListener(TurnOnLights);
        LabLightSystem.Instance.LightsOffEvent.AddListener(() => _interaction.SetInteractable(true));
        LabLightSystem.Instance.LightsOnEvent.AddListener(() => _interaction.SetInteractable(false));
    }


    private void Start()
    {
        _interaction.SetInteractable(false);
    }


    private void TurnOnLights()
    {
        LabLightSystem.Instance.TurnOn();
    }
}
