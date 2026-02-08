using UnityEngine;

public class BaldModifier : ScaleModifer
{
    protected override AttributeModifier GetItemAttribute()
    {
        return AttributeSystem.Instance.StudHairScaleMod;
    }
}
