using UnityEngine;

public class AttributeSystem : SceneSingleton<AttributeSystem>
{
    public AttributeModifier StudMoveSpeedMod { private set; get; } = new AttributeModifier();
    public AttributeModifier ProfMoveSpeedMod { private set; get; } = new AttributeModifier();
    public AttributeModifier TaskEfficiencyMod { private set; get; } = new AttributeModifier();
    public AttributeModifier BoostTaskChanceMod { private set; get; } = new AttributeModifier();
    public AttributeModifier BarricadeInstallTimeMod { private set; get; } = new AttributeModifier();
    public AttributeModifier HackRepairTimeMod { private set; get; } = new AttributeModifier();
    public AttributeModifier WeaponSupplyTimeMod { private set; get; } = new AttributeModifier();
    public AttributeModifier HackBlockChanceMod { private set; get; } = new AttributeModifier();
    public AttributeModifier StudStomachScaleMod { private set; get; } = new AttributeModifier();
    public AttributeModifier StudHeadScaleMod { private set; get; } = new AttributeModifier();
    public AttributeModifier JumpDamageMod { private set; get; } = new AttributeModifier();
    public AttributeModifier TurtleNeckDistanceMod { private set; get; } = new AttributeModifier();
    public AttributeModifier StudEscapeChanceMod { private set; get; } = new AttributeModifier();
    public AttributeModifier StudDamageMod { private set; get; } = new AttributeModifier();
    public AttributeModifier ChaosDecreaseMod { private set; get; } = new AttributeModifier();
    public AttributeModifier HealDelayTimeMod { private set; get; } = new AttributeModifier();
    public AttributeModifier StaminaCostMod { private set; get; } = new AttributeModifier();
    public AttributeModifier ShotSpreadMod { private set; get; } = new AttributeModifier();
    public AttributeModifier MutinyMoneyMod { private set; get; } = new AttributeModifier();


    public bool IsDeskCoffee { set; get; }
    public bool IsStudBald { set; get; }
    public bool IsStudOutline { set; get; }
    public bool IsDeskFood { set; get; }
    public bool IsExitAlarm { set; get; }
    public bool IsStudShackle { set; get; }
    public bool IsOtakuPoster { set; get; }
    public bool IsMetalBarricade { set; get; }
}
