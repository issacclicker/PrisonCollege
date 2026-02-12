using UnityEngine;

public class MenuPanel : MonoBehaviour, IEscapeControllable
{
    [SerializeField] private SlotEntry _passiveSlotsEntry;
    [SerializeField] private SettingPanel _settingPanel;
    [SerializeField] private SimplePanel _restartCheckPanel;
    [SerializeField] private SimplePanel _exitCheckPanel;
    private CanvasGroup _canvasGroup;
    private bool _isActive = false;



    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }



    private void Start()
    {
        InventorySystem.Instance.ConstructShopSlots(_passiveSlotsEntry);
        Hide();
    }



    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if (_isActive)
    //        {
    //            Hide();
    //        }
    //        else
    //        {
    //            Show();
    //        }
    //    }
    //}



    public void Show()
    {
        _isActive = true;
        Time.timeScale = 0;
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }



    public void Hide()
    {
        _isActive = false;
        Time.timeScale = 1;
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }



    public void Resume_Btn()
    {
        Hide();
        EscapeInputSystem.Instance.DisablePanel(this);
    }



    public void Restart_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_restartCheckPanel);
    }



    public void Settings_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_settingPanel);
    }



    public void Exit_Btn()
    {
        EscapeInputSystem.Instance.EnablePanel(_exitCheckPanel);
    }

    public void Activate()
    {
        Show();
    }

    public void Deactivate()
    {
        Hide();
    }
}
