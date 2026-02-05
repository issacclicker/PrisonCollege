using UnityEngine;

[CreateAssetMenu(fileName = "NewRepairFaster", menuName = "Item/RepairFaster")]
public class BarricadeFaster : PassiveItem
{
    public float barricadeTimePercent;



    public override void Activate()
    {
        AttributeSystem.Instance.BarricadeInstallSpeedMod.AddPercent(barricadeTimePercent);
    }
}
