using TMPro;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _typeTmp;
    [SerializeField] private TextMeshProUGUI _priceTmp;
    [SerializeField] private TextMeshProUGUI _effectTmp;
    [SerializeField] private TextMeshProUGUI _descriptionTmp;
    private CanvasGroup _canvasGroup;



    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }




    public void ShowPanel(Item item)
    {
        _nameTmp.text = item.name;
        _priceTmp.text = $"$ {item.price}";
        _effectTmp.text = item.effect;
        _descriptionTmp.text = item.description;
        _canvasGroup.alpha = 1;
    }



    public void HidePanel()
    {
        _canvasGroup.alpha = 0;
    }
}
