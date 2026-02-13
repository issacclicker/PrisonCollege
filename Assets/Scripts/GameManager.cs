using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    [Header("Dev Only")]
    [SerializeField] private StageInfo[] _stageEntries;
    [Header("Scene Names")]
    [SerializeField] private string _mainScreen;
    [SerializeField] private string _stageStart;


    public StageInfo[] StageEntries => _stageEntries;



    protected override void Awake()
    {
        base.Awake();
    }




    public void StageCleared(int stageNum, DifficultyLevel difficultyLevel)
    {
        StageInfo targetStage = _stageEntries[stageNum - 1];
        DifficultyLevel maxDifficulty = (DifficultyLevel)Mathf.Max((int)difficultyLevel, (int)targetStage.maxClearDifficulty);
        _stageEntries[stageNum - 1].maxClearDifficulty = maxDifficulty;
    }



    public void StartStage(int stageNum, DifficultyLevel difficultyLevel)
    {
        ResetGlobalControlStats();
        SceneManager.LoadScene(_stageStart);
    }



    public void ShowMainScreen()
    {
        ResetGlobalControlStats();
        SceneManager.LoadScene(_mainScreen);
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