using TMPro;
using UnityEngine;

public class StageOver : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panelCanvas;
    [SerializeField] private TextMeshProUGUI _titleTmp;
    [SerializeField] private TextMeshProUGUI _detailTmp;



    private void Awake()
    {
        _panelCanvas.alpha = 0f;
        _panelCanvas.interactable = false;
        _panelCanvas.blocksRaycasts = false;
    }



    public void ShowOverPanel(bool isSuccess)
    {
        _titleTmp.text = isSuccess ? "감금 성공!" : "감금 실패!";
        _detailTmp.text = isSuccess ? "대학원생들의 자유 박탈에 성공하였습니다." : "대학원생들에게 자유를 허락하고 말았습니다.";
        _panelCanvas.alpha = 1f;
        _panelCanvas.interactable = true;
        _panelCanvas.blocksRaycasts = true;
    }
}
