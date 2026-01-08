using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaveSpot : MonoBehaviour
{
    [SerializeField] protected BehaviorType _behaviorTypes;
    public virtual BehaviorType BehaviorTypes => _behaviorTypes;
    public virtual bool IsUsable => true;


    public bool HasBehavior(BehaviorType type)
    {
        // 비트 연산으로 포함 여부 확인
        return (BehaviorTypes & type) != 0;
    }



    public virtual void Use(PostStudent userStudent) { }
    public virtual void Release(PostStudent userStudent) { }
    public virtual void Arrived(PostStudent userStudent) { }
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
}
