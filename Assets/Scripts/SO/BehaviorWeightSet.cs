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