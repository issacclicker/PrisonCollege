using UnityEngine;

[CreateAssetMenu(fileName = "NewHackRepairFaster", menuName = "Item/HackRepairFaster")]
public class HackRepairFaster : PassiveItem
{
    public float hackRepairTimePercent;



    public override void Activate()
    {
        AttributeSystem.Instance.HackRepairSpeedMod.AddPercent(hackRepairTimePercent);
    }
}
