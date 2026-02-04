using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : SceneSingleton<StageController>
{
    [Header("UI Bindings")]
    [SerializeField] private TextMeshProUGUI _timerTmp;
    [SerializeField] private TextMeshProUGUI _chaosTmp;
    [SerializeField] private TextMeshProUGUI _escapeTmp;
    [SerializeField] private TextMeshProUGUI _moneyTmp;
    [SerializeField] private TextMeshProUGUI _workingTmp;
    [SerializeField] private Image _projectProgressBar;
    [Header("Stats")]
    [SerializeField] private Stat _timerStat;
    [SerializeField] private Stat _chaosStat;
    [SerializeField] private Stat _escapeStat;
    [SerializeField] private Stat _projectStat;
    [Header("Stage Play Values")]
    [SerializeField] private float _studProjectProgress = 5;
    [SerializeField] private float _profProjectProgress = 20;
    [SerializeField] private float _chaosIncrease = 3;
    [SerializeField] private float _chaosDecrease = 5;
    [SerializeField] private int _progectReward = 50;
    [SerializeField] private float _minDelayFactor = 0.25f;
    [SerializeField] private float _delayFuncFactor = 0.5f;
    [Header("Professor Task Place")]
    [SerializeField] private ProfessorTask[] _professorTasks;
    [Header("ETC")]
    [SerializeField] private Professor _player;
    [SerializeField] private StageSpots _stageSpots;
    [SerializeField] private StudentSpawner _studentSpawner;
    [SerializeField] private StageOver _stageOver;
    [SerializeField] private bool _isTestMode = true;

    private int _money = 0;
    private int _workingStudCount = 0;
    private bool _isProfWorking = false;
    private List<PostStudent> _studentList = new();

    public float ProjectProgress => _projectStat.Ratio;
    public Professor Player => _player;
    public StageSpots StageSpots => _stageSpots;



    protected override void Awake()
    {
        base.Awake();
        _timerStat.Initialize();
        _chaosStat.Initialize(true);
        _escapeStat.Initialize(true);
        _projectStat.Initialize(true);

        _timerStat.DepletedEvent.AddListener(() => GameOver(true));
        _escapeStat.MaxReachEvent.AddListener(() => GameOver(false));
        _projectStat.MaxReachEvent.AddListener(OnProjectSuccessed);

        //SetStudentList();

        //foreach (var student in _studentList)
        //{
        //    student.DieEvent.AddListener(OnStudentDied);
        //    student.EscapeEvent.AddListener(OnStudentEscaped);
        //}
    }



    private void Start()
    {
        _studentList = _studentSpawner.SpawnStudents();

        foreach (var student in _studentList)
        {
            student.DieEvent.AddListener(OnStudentDied);
            student.EscapeEvent.AddListener(OnStudentEscaped);
        }
    }



    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    private void SetStudentList()
    {
        GameObject[] studentTagObjects = GameObject.FindGameObjectsWithTag("Student");

        foreach (GameObject obj in studentTagObjects)
        {
            PostStudent student = obj.GetComponent<PostStudent>();
            if (student == null) continue;
            _studentList.Add(student);
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //_chaosStat.Increase(20);
        }
        CountWorkingStudents();
        CheckProfessorProgressing();
        ProgressProject();
        IncreaseChaos();
        DecreaseStats();
        UpdateUIs();
    }



    private void CountWorkingStudents()
    {
        _workingStudCount = 0;
        foreach (var student in _studentList)
        {
            if (student.IsWorking)
            {
                _workingStudCount++;
            }
        }
    }



    private void CheckProfessorProgressing()
    {
        _isProfWorking = false;
        foreach (var profTask in _professorTasks)
        {
            if (profTask.IsTasking)
            {
                _isProfWorking = true;
                break;
            }
        }
    }



    private void ProgressProject()
    {
        float studTotalProgress = _workingStudCount * _studProjectProgress * Time.deltaTime;
        float profTotalProgress = _isProfWorking ? _profProjectProgress * Time.deltaTime : 0;
        _projectStat.Increase(studTotalProgress + profTotalProgress);
    }



    private void ProgressProject_T()
    {
        _isProfWorking = Input.GetKey(KeyCode.Escape);
    }



    private void GameOver(bool isSuccess)
    {
        Time.timeScale = 0;
        Player.DisableController();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _stageOver.ShowOverPanel(isSuccess);
    }


    private void OnStudentEscaped(PostStudent student)
    {
        _chaosStat.Increase(30);
        _escapeStat.Increase(1);
    }



    private void OnStudentDied(PostStudent student, HitInfo hitInfo)
    {
        if (student.IsDoingHazardBehavior == false && hitInfo.attacker == Player.gameObject)
        {
            _chaosStat.Increase(10);
        }
    }



    private void OnProjectSuccessed()
    {
        _projectStat.Initialize(true);
        _money += _progectReward;
    }



    private void UpdateUIs()
    {
        int minutes = Mathf.FloorToInt(_timerStat.Current / 60f);
        int seconds = Mathf.FloorToInt(_timerStat.Current % 60f);
        _timerTmp.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        _chaosTmp.text = _chaosStat.Current.ToString("F0");

        _escapeTmp.text = $"{_escapeStat.Current.ToString("F0")} / {_escapeStat.Max.ToString("F0")}";

        _moneyTmp.text = _money.ToString();

        _workingTmp.text = $"{_workingStudCount.ToString()}명 작업중";

        _projectProgressBar.fillAmount = _projectStat.Ratio;
    }



    private void IncreaseChaos()
    {
        int chaosCauseCount = 0;
        foreach (PostStudent student in _studentList)
        {
            if (student.IsCausingChaos)
            {
                chaosCauseCount++;
            }
        }
        _chaosStat.Increase(chaosCauseCount * _chaosIncrease * Time.deltaTime);
    }



    private void DecreaseStats()
    {
        _timerStat.Decrease(Time.deltaTime);
        _chaosStat.Decrease(_chaosDecrease * Time.deltaTime);
    }



    public float GetChaosEffectedDelay(float delay)
    {
        float chaosRatio = _chaosStat.Ratio;
        float delayFactor = _delayFuncFactor * chaosRatio * chaosRatio + (_minDelayFactor - 1 - _delayFuncFactor) * chaosRatio + 1;
        return delayFactor * delay;
    }
}
