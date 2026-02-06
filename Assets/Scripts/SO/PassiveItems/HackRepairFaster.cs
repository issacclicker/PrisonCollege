using UnityEngine;

[CreateAssetMenu(fileName = "NewHackRepairFaster", menuName = "Item/HackRepairFaster")]
public class HackRepairFaster : PassiveItem
{
    public float hackBlockChanceFlat;



    public override void Activate()
    {
        AttributeSystem.Instance.HackBlockChanceMod.AddPercent(hackBlockChanceFlat);
    }
}
