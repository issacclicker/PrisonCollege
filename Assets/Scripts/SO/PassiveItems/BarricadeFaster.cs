using UnityEngine;

[CreateAssetMenu(fileName = "NewRepairFaster", menuName = "Item/RepairFaster")]
public class BarricadeFaster : PassiveItem
{
    public float repairSpeedPercent;



    public override void Activate()
    {
        AttributeSystem.Instance.BarricadeInstallTimeMod.AddPercent(repairSpeedPercent);
        AttributeSystem.Instance.HackRepairTimeMod.AddPercent(repairSpeedPercent);
    }
}
