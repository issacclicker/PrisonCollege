using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SlotSelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _targetImage;
    [SerializeField] private Color _selectedColor;
    private Color _originColor;

    [HideInInspector] public UnityEvent<SlotSelector> PointerClickEvent = new();


    private void Awake()
    {
        _originColor = _targetImage.color;
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClickEvent?.Invoke(this);
    }



    public void HighLight()
    {
        _targetImage.color = _selectedColor;
    }



    public void Darken()
    {
        _targetImage.color = _originColor;
    }
}
