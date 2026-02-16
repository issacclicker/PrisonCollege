using TMPro;
using UnityEngine;
using DG.Tweening;

public class BetResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTmp;
    [SerializeField] private TextMeshProUGUI _mainMoneyTmp;
    [SerializeField] private TextMeshProUGUI _bonusMoneyTmp;
    private int _currentDisplayMoney = 0;
    
    
    
    public void Show(BetResult betResult, int money, int increase)
    {
        _currentDisplayMoney = money;
        _mainMoneyTmp.text = _currentDisplayMoney.ToString("N0");
        if (betResult == BetResult.Success)
        {
            _titleTmp.text = "베팅 성공!!";
            _titleTmp.color = new Color(0, 220 / 255f, 0);
            _bonusMoneyTmp.text = $"+{increase.ToString("N0")}";
            StartIncreaseAnimation(money + increase);
        }
        else if (betResult == BetResult.Failed)
        {
            _titleTmp.text = "베팅 실패!!";
            _titleTmp.color = new Color(240/255f, 40/255f, 40/255f);
            _bonusMoneyTmp.text = string.Empty;
        }
        else
        {
            _titleTmp.text = "무승부!!";
            _titleTmp.color = Color.white;
            _bonusMoneyTmp.text = string.Empty;
        }
        gameObject.SetActive(true);
    }



    private void StartIncreaseAnimation(int targetMoney, float duration = 3.0f, float delay = 3.0f)
    {
        DOTween.Kill(this);
        DOTween.To(() => _currentDisplayMoney, x => _currentDisplayMoney = x, targetMoney, duration)
            .SetDelay(delay)
            .OnStart(() =>
            {
                _bonusMoneyTmp.text = string.Empty;
            })
            .OnUpdate(() =>
            {
                // 숫자가 변할 때마다 텍스트 갱신 (세 자리 쉼표 포함)
                _mainMoneyTmp.text = _currentDisplayMoney.ToString("N0");
            })
            .SetEase(Ease.OutExpo); // 뒤로 갈수록 천천히 멈추는 효과
    }
}
