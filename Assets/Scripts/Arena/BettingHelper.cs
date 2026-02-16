using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BettingHelper : MonoBehaviour
{
    [SerializeField] private Image _leftBtnBackImg;
    [SerializeField] private TextMeshProUGUI _leftBtnTmp;
    [SerializeField] private Color _leftHightlightColor;
    [SerializeField] private GameObject _leftChooseBorder;

    [SerializeField] private Image _rightBtnBackImg;
    [SerializeField] private TextMeshProUGUI _rightBtnTmp;
    [SerializeField] private Color _rightHightlightColor;
    [SerializeField] private GameObject _rightChooseBorder;

    [SerializeField] private GameObject _continueTmpObj;
    private Color _originalLeftColor;
    private Color _originalRightColor;
    private SelectedSide _selectedSide = SelectedSide.None;
    private bool _isStarted = false;

    public UnityEvent<SelectedSide> FightStartEvent = new();
    public UnityEvent<SelectedSide> SelectEvent = new();



    private void Awake()
    {
        _originalLeftColor = _leftBtnBackImg.color;
        _originalRightColor = _rightBtnBackImg.color;
    }



    private void Start()
    {
        UpdateUIs();
    }



    public void WriteButtonNameTmp(string leftName, string rightName)
    {
        _leftBtnTmp.text = leftName;
        _rightBtnTmp.text = rightName;
    }



    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && _selectedSide != SelectedSide.None)
        {
            _isStarted = true;
            FightStartEvent?.Invoke(_selectedSide);
            gameObject.SetActive(false);
        }
    }



    public void LeftSelected_Btn()
    {
        if (_selectedSide == SelectedSide.Left) return;
        if (_isStarted) return;
        _selectedSide = SelectedSide.Left;
        SelectEvent?.Invoke(_selectedSide);
        UpdateUIs();
    }



    public void RightSelected_Btn()
    {
        if (_selectedSide == SelectedSide.Right) return;
        if (_isStarted) return;
        _selectedSide = SelectedSide.Right;
        SelectEvent?.Invoke(_selectedSide);
        UpdateUIs();
    }



    private void UpdateUIs()
    {
        if (_selectedSide == SelectedSide.Left)
        {
            HighlightLeftButton();
            UnhighlightRightButton();
        }
        else if (_selectedSide == SelectedSide.Right)
        {
            UnhighlightLeftButton();
            HighlightRightButton();
        }
        else
        {
            UnhighlightLeftButton();
            UnhighlightRightButton();
        }
        _continueTmpObj.SetActive(_selectedSide != SelectedSide.None);
    }



    private void HighlightLeftButton()
    {
        _leftBtnBackImg.color = _leftHightlightColor;
        _leftBtnTmp.color = Color.white;
        _leftChooseBorder.SetActive(true);
    }



    private void UnhighlightLeftButton()
    {
        _leftBtnBackImg.color = _originalLeftColor;
        _leftBtnTmp.color = Color.black;
        _leftChooseBorder.SetActive(false);
    }



    private void HighlightRightButton()
    {
        _rightBtnBackImg.color = _rightHightlightColor;
        _rightBtnTmp.color = Color.white;
        _rightChooseBorder.SetActive(true);
    }



    private void UnhighlightRightButton()
    {
        _rightBtnBackImg.color = _originalRightColor;
        _rightBtnTmp.color = Color.black;
        _rightChooseBorder.SetActive(false);
    }
}


[System.Serializable]
public enum SelectedSide
{
    None, Left, Right
}