using UnityEngine;

public enum EffectType { Damage, Work }

[CreateAssetMenu(fileName = "NewEffectData", menuName = "Combat/Effect Data")]
public class EffectData : ScriptableObject
{
    public float value;          // 수치
    public GameObject effectVisualPrefab; // 피격 시 생성될 이펙트 (선택 사항)
}