using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlSetting : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private Slider _mouseSensitivitySlider;
    [SerializeField] private TextMeshProUGUI _sensitivityValueTmp;

    [Header("Sprint Mode (Toggle Group)")]
    [SerializeField] private Toggle _toggleModeBtn; // "토글 방식" 버튼
    [SerializeField] private Toggle _holdModeBtn;   // "홀드 방식" 버튼
    [SerializeField] private Image _toggleModeBg;   // 홀드 방식 배경 이미지
    [SerializeField] private Image _holdModeBg;   // 홀드 방식 배경 이미지
    [SerializeField] private Color _activeModeColor; // 선택되었을 때 색상
    private Color _originModeColor;



    private void Awake()
    {
        _originModeColor = _toggleModeBg.color;
    }

    private void Start()
    {
        // 1. 기존 설정값 로드 (0: 홀드, 1: 토글)
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        int savedSprintMode = PlayerPrefs.GetInt("SprintMode", 0);

        // 2. UI 초기 설정
        _mouseSensitivitySlider.value = savedSens;
        UpdateSensitivityText(savedSens);

        // 저장된 값에 따라 버튼 체크 상태 결정
        if (savedSprintMode == 1) _toggleModeBtn.isOn = true;
        else _holdModeBtn.isOn = true;

        // 3. 이벤트 연결
        _mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        // 토글 버튼들이 켜질 때(isOn == true)만 저장 로직 실행
        _toggleModeBtn.onValueChanged.AddListener(isOn => { if (isOn) OnSprintModeChanged(1); });
        _holdModeBtn.onValueChanged.AddListener(isOn => { if (isOn) OnSprintModeChanged(0); });

        UpdateToggleVisual(_toggleModeBtn.isOn, _toggleModeBg);
        UpdateToggleVisual(_holdModeBtn.isOn, _holdModeBg);

        // 이벤트 연결: 값이 바뀔 때마다 시각적 효과 업데이트
        _toggleModeBtn.onValueChanged.AddListener(isOn => {
            UpdateToggleVisual(isOn, _toggleModeBg);
            if (isOn) OnSprintModeChanged(1);
        });

        _holdModeBtn.onValueChanged.AddListener(isOn => {
            UpdateToggleVisual(isOn, _holdModeBg);
            if (isOn) OnSprintModeChanged(0);
        });
    }

    private void UpdateToggleVisual(bool isOn, Image bgImage)
    {
        if (bgImage != null)
        {
            bgImage.color = isOn ? _activeModeColor : _originModeColor;
        }
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        UpdateSensitivityText(value);
    }

    private void OnSprintModeChanged(int modeIndex)
    {
        // 0은 홀드, 1은 토글로 정의하여 저장
        PlayerPrefs.SetInt("SprintMode", modeIndex);
        Debug.Log(modeIndex == 1 ? "달리기 방식: 토글" : "달리기 방식: 홀드");
    }

    private void UpdateSensitivityText(float value)
    {
        if (_sensitivityValueTmp != null)
            _sensitivityValueTmp.text = value.ToString("F1");
    }
}