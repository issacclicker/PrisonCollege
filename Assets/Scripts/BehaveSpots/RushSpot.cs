using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushSpot : SingleStudentSpot
{
    [SerializeField] private ExitSpot _targetExitSpot;
    public override bool IsUsable => base.IsUsable && _targetExitSpot.IsUsable && !_targetExitSpot.CanExit;
    public override BehaviorType BehaviorTypes => BehaviorType.RushThrough;
}
