using UnityEngine;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private SimplePanel _stageSelectPanel;
    [SerializeField] private SettingPanel _settingPanel;
    [SerializeField] private SimplePanel _membersPanel;
    [SerializeField] private SimplePanel _exitCheckPanel;



    private void Awake()
    {
        _stageSelectPanel.gameObject.SetActive(true);
        _settingPanel.gameObject.SetActive(true);
        _membersPanel.gameObject.SetActive(true);
        _exitCheckPanel.gameObject.SetActive(true);
    }



    public void Start_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_stageSelectPanel);
    }



    public void Setting_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_settingPanel);
    }



    public void Members_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_membersPanel);
    }



    public void Exit_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_exitCheckPanel);
    }
}
