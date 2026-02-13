using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    [Header("Dev Only")]
    [SerializeField] private StageInfo[] _stageEntries;
    [Header("Scene Names")]
    [SerializeField] private string _mainScreen;
    [SerializeField] private string _stagePrepare;
    [SerializeField] private string _stagePrefix;
    private StageInfo _currentStage;
    private DifficultyLevel _currentDifficulty;


    public StageInfo[] StageEntries => _stageEntries;
    public string StageTitle => $"{_currentStage.number}. {_currentStage.name}";



    protected override void Awake()
    {
        base.Awake();
        if (_stageEntries == null)
        {
            _currentStage = new StageInfo();
            _currentStage.number = StageController.Instance.StageNumber;
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




    public void StageCleared(int stageNum, DifficultyLevel difficultyLevel)
    {
        StageInfo targetStage = _stageEntries[stageNum - 1];
        DifficultyLevel maxDifficulty = (DifficultyLevel)Mathf.Max((int)difficultyLevel, (int)targetStage.maxClearDifficulty);
        _stageEntries[stageNum - 1].maxClearDifficulty = maxDifficulty;
    }



    public void PrepareStage(int stageNum, DifficultyLevel difficultyLevel)
    {
        WaveSystem.Instance.ResetWave();
        _currentStage = _stageEntries[stageNum - 1];
        _currentDifficulty = difficultyLevel;
        SceneManager.LoadScene(_stagePrepare);
    }



    public void ShowMainScreen()
    {
        SceneManager.LoadScene(_mainScreen);
        _currentStage = null;
        _currentDifficulty = DifficultyLevel.None;
    }



    private void ResetGlobalControlStats()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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