using UnityEngine;

public enum EffectType { Damage, Work }

[CreateAssetMenu(fileName = "NewEffectData", menuName = "Combat/Effect Data")]
public class EffectData : ScriptableObject
{
    public EffectType type;      // 효과 종류
    public float value;          // 수치
    public float hitImpulse;
    [Range(0f, 1f)] public float dodgeProb;
    public GameObject effectVisualPrefab; // 피격 시 생성될 이펙트 (선택 사항)
}