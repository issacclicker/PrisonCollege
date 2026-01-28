using UnityEngine;

public class LerpStatBar : StatBar
{
    [SerializeField] private float _interpSpeed = 5f;
    private float _targetRatio;

    protected override void Start()
    {
        base.Start(); // 부모의 리스너 등록 로직 실행
        _targetRatio = _targetStat.Ratio;
    }



    protected override void OnStatChanged(float amount)
    {
        // 부모처럼 바로 UpdateUI를 하지 않고, 목표값만 갱신
        _targetRatio = _targetStat.Ratio;
    }



    private void Update()
    {
        float current = _fillImage.fillAmount;
        if (!Mathf.Approximately(current, _targetRatio))
        {
            float next = Mathf.Lerp(current, _targetRatio, Time.deltaTime * _interpSpeed);
            UpdateUI(next); // 부모의 UI 업데이트 함수 호출
        }
    }
}
