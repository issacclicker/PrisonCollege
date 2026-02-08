using UnityEngine;

[CreateAssetMenu(fileName = "NewHeadBigger", menuName = "Item/HeadBigger")]
public class HeadBigger : PassiveItem
{
    public float studHeadScale = 1.75f;
    public float taskPercent;



    public override void Activate()
    {
        AttributeSystem.Instance.TaskEfficiencyMod.AddPercent(studHeadScale);
        AttributeSystem.Instance.StudHeadScaleMod.AddPercent(taskPercent);
    }
}
