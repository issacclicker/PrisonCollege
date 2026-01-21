using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBehaviorWeightSet", menuName = "Combat/Behavior Weight Set")]
public class BehaviorWeightSet : WeightedSetSO<BehaviorType, BehaviorChance>
{
}



[System.Serializable]
public class BehaviorChance : IWeightedEntry<BehaviorType>
{
    [SerializeField] private BehaviorType _behaviorType;
    public BehaviorType Value => _behaviorType;
    [SerializeField] private float _chance;
    public float Chance => _chance;
}



[System.Flags]
public enum BehaviorType
{
    None = 0,
    Sit = 1 << 0,
    LookAround = 1 << 1,
    UseMicrowave = 1 << 2,
    Escape = 1 << 3,
    RushThrough = 1 << 4,
    Fight = 1 << 5,
    Smoke = 1 << 6,
    Tackle = 1 << 7,
}