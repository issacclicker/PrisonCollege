using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : PersistentSingleton<InventorySystem>
{
    [SerializeField] private List<Item> _totalItemList;
    [SerializeField] private int _money;
    private HashSet<Item> _nonPurchasedItemSet = new();
    private HashSet<Item> _purchasedItemSet = new();
    private List<WeaponItem> _equipedItemList = new();

    private List<PassiveItem> _passiveItemList;


    public int Money => _money;
    public List<PassiveItem> PassiveItemList => _passiveItemList;



    protected override void Awake()
    {
        base.Awake();
        foreach (var item in _totalItemList)
        {
            _nonPurchasedItemSet.Add(item);
        }
    }



    public void Purchase(Item item)
    {
        _nonPurchasedItemSet.Remove(item);
        _purchasedItemSet.Add(item);
    }



    public void ConstructShopSlots(SlotEntry slotEntry)
    {
        foreach (var item in _nonPurchasedItemSet)
        {
            GameObject slotObject = Instantiate(slotEntry.prefab, slotEntry.parent);
            ItemSlot itemSlot = slotObject.GetComponent<ItemSlot>();
            itemSlot.SetItem(item);
        }
    }



    public void ConstructPassiveSlots(SlotEntry slotEntry)
    {
        foreach (var item in _purchasedItemSet)
        {
            if (item is PassiveItem == false) continue;
            GameObject slotObject = Instantiate(slotEntry.prefab, slotEntry.parent);
            ItemSlot itemSlot = slotObject.GetComponent<ItemSlot>();
            itemSlot.SetItem(item);
        }
    }



    private void ClearItemSlots(List<ItemSlot> itemSlots)
    {
        foreach (var slot in itemSlots)
        {
            slot.ClearItem();
        }
    }



    public void FillWeaponSlots(List<ItemSlot> itemSlots)
    {
        ClearItemSlots(itemSlots);
        List<WeaponItem> weaponItemList = _purchasedItemSet.OfType<WeaponItem>().ToList();
        for (int i = 0; i < weaponItemList.Count; ++i)
        {
            Item weaponItem = weaponItemList[i];
            itemSlots[i].SetItem(weaponItem);
        }
    }



    public void FillEquipSlots(List<ItemSlot> itemSlots)
    {
        ClearItemSlots(itemSlots);
        for (int i = 0; i < _equipedItemList.Count; ++i)
        {
            Item equipedItem = _equipedItemList[i];
            if (equipedItem is WeaponItem == false) continue;
            itemSlots[i].SetItem(equipedItem);
        }
    }
}
