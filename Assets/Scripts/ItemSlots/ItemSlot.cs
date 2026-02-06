using System.Data;
using UnityEngine;
using static UnityEditor.Progress;

public abstract class ItemSlot : MonoBehaviour
{
    protected Item _item;



    public void SetItem(Item item)
    {
        _item = item;
        UpdateSlotUI();
    }



    public void ClearItem()
    {
        _item = null;
        UpdateSlotUI();
    }



    protected abstract void UpdateSlotUI();
}
