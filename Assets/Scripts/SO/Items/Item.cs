using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class Item : ScriptableObject
{
    public int id;
    public new string name;
    public int price;
    [TextArea] public string description;
    public ItemRarity rarity;
}



public enum ItemRarity
{
    Normal,
    Rare,
    Epic,
    Legendary,
}