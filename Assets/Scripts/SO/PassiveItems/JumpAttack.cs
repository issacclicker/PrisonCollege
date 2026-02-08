using UnityEngine;

[CreateAssetMenu(fileName = "NewJumpAttack", menuName = "Item/JumpAttack")]
public class JumpAttack : PassiveItem
{
    public float jumpDamageScale = 2f;


    public override void Activate()
    {
        AttributeSystem.Instance.JumpDamageMod.AddPercent(jumpDamageScale);
    }
}
