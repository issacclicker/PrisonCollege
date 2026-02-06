using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotPackage : MonoBehaviour
{
    [SerializeField] private SlotEntry _shopSlotEntry;
    [SerializeField] private SlotEntry _passiveSlotEntry;
    [SerializeField] private List<ItemSlot> _weaponSlotList;
    [SerializeField] private List<ItemSlot> _equipSlotList;
    private List<SlotSelector> _slotList = new();
    private SlotSelector _selectedSlot;
    private bool _isSelectedSlotFixed;



    private void Awake()
    {
    }



    private void Start()
    {
        //foreach (SlotEntry slotEntry in _slotEntries)
        //{
        //    for (int i = 0; i < slotEntry.count; i++)
        //    {
        //        GameObject slotObject = Instantiate(slotEntry.prefab, slotEntry.parent);
        //        SlotSelector slotSelector = slotObject.GetComponent<SlotSelector>();
        //        slotSelector.PointerClickEvent.AddListener(SlotPointerClicked);
        //        _slotList.Add(slotSelector);
        //    }
        //}
        InventorySystem.Instance.ConstructShopSlots(_shopSlotEntry);
        InventorySystem.Instance.ConstructPassiveSlots(_passiveSlotEntry);
        InventorySystem.Instance.FillWeaponSlots(_weaponSlotList);
        InventorySystem.Instance.FillEquipSlots(_equipSlotList);
        _slotList = Object.FindObjectsByType<SlotSelector>(FindObjectsSortMode.None).ToList();
        foreach (var slot in _slotList)
        {
            slot.PointerClickEvent.AddListener(SlotPointerClicked);
        }
    }



    private void SlotPointerClicked(SlotSelector targetSlot)
    {
        _selectedSlot?.Darken();
        if (_selectedSlot == targetSlot)
        {
            _selectedSlot = null;
        }
        else
        {
            _selectedSlot = targetSlot;
        }
        _selectedSlot?.HighLight();
    }
}


[System.Serializable]
public class SlotEntry
{
    public Transform parent;
    public GameObject prefab;
}