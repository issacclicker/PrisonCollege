using UnityEngine;

public class BaldOutlines : MonoBehaviour
{
    private void Start()
    {
        bool hasToActivate = AttributeSystem.Instance.IsStudOutline;
        Outline[] outlines = GetComponentsInChildren<Outline>();
        foreach (Outline outline in outlines)
        {
            outline.enabled = hasToActivate;
        }
    }
}
