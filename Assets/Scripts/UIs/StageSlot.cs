using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StageSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Base")]
    [SerializeField] private Image _borderImg;
    [Header("Focus Settings")]
    [SerializeField] private CanvasGroup _focusGroup;
    [SerializeField] private Color _borderFocusColor;
    [Header("Lock Settings")]
    [SerializeField] private CanvasGroup _lockGroup;
    [SerializeField] private Color _borderLockColor;
    [SerializeField] private bool _isLocked = false;
    private Color _originBorderColor;

    [HideInInspector] public UnityEvent<StageSlot> MouseClickEvent = new();




    private void Awake()
    {
        _originBorderColor = _borderImg.color;
        _focusGroup.gameObject.SetActive(true);
        _lockGroup.gameObject.SetActive(true);
        Unfocus();
    }



    private void Start()
    {
        if (_isLocked)
        {
            _lockGroup.alpha = 1;
            _lockGroup.interactable = true;
            _lockGroup.blocksRaycasts = true;
            _borderImg.color = _borderLockColor;
        }
        else
        {
            _lockGroup.alpha = 0;
            _lockGroup.interactable = false;
            _lockGroup.blocksRaycasts = false;
            _borderImg.color = _originBorderColor;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isLocked) return;
        MouseClickEvent?.Invoke(this);
    }



    public void Focus()
    {
        _focusGroup.alpha = 1;
        _focusGroup.interactable = true;
        _focusGroup.blocksRaycasts = true;
        _borderImg.color = _borderFocusColor;
    }



    public void Unfocus()
    {
        _focusGroup.alpha = 0;
        _focusGroup.interactable = false;
        _focusGroup.blocksRaycasts = false;
        _borderImg.color = _originBorderColor;
    }
}
