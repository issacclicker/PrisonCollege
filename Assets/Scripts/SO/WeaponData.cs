using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Tooltip("공격 모션 길이")]
    public float animLength = 1.5f;
    [Tooltip("스테미나 소모")]
    public float staminaCost = 1f;

    [Tooltip("타격 효과")]
    public EffectData effect;
}