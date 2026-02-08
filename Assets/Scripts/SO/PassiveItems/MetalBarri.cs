using UnityEngine;

[CreateAssetMenu(fileName = "NewMetalBarri", menuName = "Item/MetalBarri")]
public class MetalBarri : PassiveItem
{
    public bool isMetalBarricade;
    public override void Activate()
    {
        AttributeSystem.Instance.IsMetalBarricade = isMetalBarricade;
    }
}
