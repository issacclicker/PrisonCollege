using UnityEngine;

public class AttributeSystem : PersistentSingleton<AttributeSystem>
{
    public AttributeModifier StudMoveSpeedMod { private set; get; } = new AttributeModifier();
    public AttributeModifier ProfMoveSpeedMod { private set; get; } = new AttributeModifier();
    public AttributeModifier TaskEfficiencyMod { private set; get; } = new AttributeModifier();
    public AttributeModifier BoostTaskChanceMod { private set; get; } = new AttributeModifier();
    public AttributeModifier BarricadeInstallSpeedMod { private set; get; } = new AttributeModifier();
    public AttributeModifier HackRepairSpeedMod { private set; get; } = new AttributeModifier();
    public AttributeModifier WeaponSupplyTimeMod { private set; get; } = new AttributeModifier();
}
