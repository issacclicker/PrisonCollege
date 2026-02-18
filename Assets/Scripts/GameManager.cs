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
    [SerializeField] private string _arena = "Arena";
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
        else
        {
            LoadStageProgress();
        }
        //ShowMainScreen();
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGlobalControlStats();
    }



    public void StartStage()
    {
        SceneManager.LoadScene($"{_stagePrefix}{_currentStage.number}");
    }



    private void LoadStageProgress()
    {
        int lastClearedStageNum = PlayerPrefs.GetInt("MaxClearStage", 0);
        int[] stageDifficulties = new int[_stageEntries.Length];
        for (int i = 0; i < stageDifficulties.Length; i++)
        {
            stageDifficulties[i] = PlayerPrefs.GetInt("StageDifficulty_" + (i + 1), 0);
        }

        for(int i = 0; i < _stageEntries.Length; ++i)
        {
            _stageEntries[i].maxClearDifficulty = (DifficultyLevel)stageDifficulties[i];
            _stageEntries[i].isLocked = _stageEntries[i].number > lastClearedStageNum + 1;
        }
    }



    private void SaveStageProgress(int stageNum, DifficultyLevel difficultyLevel)
    {
        int maxClearStage = Mathf.Max(stageNum, PlayerPrefs.GetInt("MaxClearStage", 0));
        PlayerPrefs.SetInt("MaxClearStage", maxClearStage);
        PlayerPrefs.SetInt("StageDifficulty_" + stageNum, (int)difficultyLevel);
        PlayerPrefs.Save();
    }




    public void StageCleared()
    {
        DifficultyLevel maxDifficulty = (DifficultyLevel)Mathf.Max((int)_currentDifficulty, (int)_currentStage.maxClearDifficulty);
        _currentStage.maxClearDifficulty = maxDifficulty;
        _stageEntries[_currentStage.number].isLocked = false;
        SaveStageProgress(_currentStage.number, _currentStage.maxClearDifficulty);
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



    public void GoArena()
    {
        SceneManager.LoadScene(_arena);
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