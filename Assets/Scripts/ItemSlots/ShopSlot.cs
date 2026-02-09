using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : ItemSlot
{
    [SerializeField] private Image _IconImg;
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _typeTmp;
    [SerializeField] private TextMeshProUGUI _priceTmp;



    protected override void UpdateSlotUI()
    {
        _IconImg.sprite = _item.icon;
        _nameTmp.text = _item.name;
        _typeTmp.text = _item.Type;
        _priceTmp.text = $"$ {_item.price}";
    }
}
