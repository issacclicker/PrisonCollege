using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


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



public enum BehaviorSafety { Safe, Hazard }

[AttributeUsage(AttributeTargets.Field)]
public class BehaviorInfoAttribute : Attribute
{
    public BehaviorSafety Safety { get; }
    public BehaviorInfoAttribute(BehaviorSafety safety) => Safety = safety;
}



[System.Flags]
public enum BehaviorType
{
    [BehaviorInfo(BehaviorSafety.Safe)] None = 0,
    [BehaviorInfo(BehaviorSafety.Safe)] Work = 1 << 0,
    [BehaviorInfo(BehaviorSafety.Safe)] LookAround = 1 << 1,
    [BehaviorInfo(BehaviorSafety.Safe)] UseMicrowave = 1 << 2,
    [BehaviorInfo(BehaviorSafety.Hazard)] Escape = 1 << 3,
    [BehaviorInfo(BehaviorSafety.Hazard)] RushThrough = 1 << 4,
    [BehaviorInfo(BehaviorSafety.Hazard)] Fight = 1 << 5,
    [BehaviorInfo(BehaviorSafety.Hazard)] Smoke = 1 << 6,
    [BehaviorInfo(BehaviorSafety.Hazard)] Tackle = 1 << 7,
    [BehaviorInfo(BehaviorSafety.Hazard)] Hack = 1 << 8,
    [BehaviorInfo(BehaviorSafety.Safe)] Game = 1 << 9,
}