using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    [Header("Dev Only")]
    [SerializeField] private int _stageNumber;
    [SerializeField] private StageInfo[] _stageEntries;
    [Header("Scene Names")]
    [SerializeField] private string _mainScreen;
    [SerializeField] private string _stagePrepare;
    [SerializeField] private string _stagePrefix;
    [SerializeField] private string _store;
    private StageInfo _currentStage;
    [SerializeField] private DifficultyLevel _currentDifficulty;
    public bool hasToStageSelect = false;


    public UnityEvent ControlSettingChangeEvent = new();
    public StageInfo[] StageEntries => _stageEntries;
    public string StageTitle => $"{_currentStage.number}. {_currentStage.name}";
    public DifficultyLevel Difficulty => _currentDifficulty;



    protected override void Awake()
    {
        base.Awake();
        if (_stageEntries == null)
        {
            _currentStage = new StageInfo();
            _currentStage.number = StageController.Instance.StageNumber;
        }
        if (_stageNumber > 0)
        {
            _currentStage = _stageEntries[_stageNumber - 1];
            InventorySystem.Instance.ResetInventory(false);
            WaveSystem.Instance.ResetWave();
        }
        //ShowMainScreen();
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGlobalControlStats();
    }



    public void StartStage()
    {
        SceneManager.LoadScene($"{_stagePrefix}{_currentStage.number}");
    }




    public void StageCleared()
    {
        DifficultyLevel maxDifficulty = (DifficultyLevel)Mathf.Max((int)_currentDifficulty, (int)_currentStage.maxClearDifficulty);
        _currentStage.maxClearDifficulty = maxDifficulty;
    }



    public void PrepareStage(int stageNum, DifficultyLevel difficultyLevel)
    {
        WaveSystem.Instance.ResetWave();
        InventorySystem.Instance.ResetInventory();
        _currentStage = _stageEntries[stageNum - 1];
        _currentDifficulty = difficultyLevel;
        SceneManager.LoadScene(_stagePrepare);
    }



    public void Restart()
    {
        PrepareStage(_currentStage.number, _currentDifficulty);
    }



    public void GoStore()
    {
        SceneManager.LoadScene(_store);
    }



    public void ShowMainScreen()
    {
        SceneManager.LoadScene(_mainScreen);
        _currentStage = null;
        _currentDifficulty = DifficultyLevel.None;
    }



    public void ShowStageSelect()
    {
        hasToStageSelect = true;
        ShowMainScreen();
    }



    private void ResetGlobalControlStats()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    public void ControlSettingChanged()
    {
        ControlSettingChangeEvent?.Invoke();
    }



    public void ExitGame()
    {
        Application.Quit();
    }
}



[System.Serializable]
public class StageInfo
{
    public int number;
    public string name;
    public DifficultyLevel maxClearDifficulty;
    public bool isLocked;
    public Sprite sprite;
}