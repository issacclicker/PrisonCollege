using UnityEngine;

[CreateAssetMenu(fileName = "NewStudFood", menuName = "Item/StudFood")]
public class StudFood : PassiveItem
{
    public float studMoveSpeedPercent;
    public float studStomachScale;
    public GameObject deskFoodPrefab;




    public override void Activate()
    {
        AttributeSystem.Instance.StudMoveSpeedMod.AddFlat(studMoveSpeedPercent);
    }
}
