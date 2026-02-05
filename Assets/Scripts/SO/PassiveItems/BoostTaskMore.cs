using UnityEngine;

[CreateAssetMenu(fileName = "NewBoostTaskMore", menuName = "Item/BoostTaskMore")]
public class BoostTaskMore : PassiveItem
{
    public float boostTaskChanceFlat;
    public GameObject deskCoffeePrefab;



    public override void Activate()
    {
        AttributeSystem.Instance.BoostTaskChanceMod.AddFlat(boostTaskChanceFlat);
    }
}
