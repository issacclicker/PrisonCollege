using UnityEngine;

[CreateAssetMenu(fileName = "NewRechargeReducer", menuName = "Item/RechargeReducer")]
public class WeaponSupplyFaster : PassiveItem
{
    public float supplyTimePercent;


    public override void Activate()
    {
        AttributeSystem.Instance.WeaponSupplyTimeMod.AddPercent(supplyTimePercent);
    }
}
