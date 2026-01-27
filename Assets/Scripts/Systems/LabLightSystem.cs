using UnityEngine;

public class LabLightSystem : SceneSingleton<LabLightSystem>
{
    public void TurnOff()
    {
        Debug.Log("LightOff");
    }



    public void TurnOn()
    {
        Debug.Log("LightOn");
    }
}
