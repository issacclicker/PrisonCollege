using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] private float _chaosDecrease = 5;
    [SerializeField] private int _progectReward = 50;
    [Header("ETC")]
    [SerializeField] private bool _isTestMode = true;

    private int _money = 0;
    private int _workingStudCount = 0;
    private bool _isProfWorking = false;
    private List<PostStudent> _students = new();



    protected override void Awake()
    {
        base.Awake();
        _timerStat.Initialize();
        _chaosStat.Initialize(true);
        _escapeStat.Initialize(true);
        _projectStat.Initialize(true);

        _timerStat.DepletedEvent.AddListener(GameOver);
        _escapeStat.MaxReachEvent.AddListener(GameOver);
        _projectStat.MaxReachEvent.AddListener(OnProjectSuccessed);

        SetStudentList();

        foreach (var student in _students)
        {
            student.DieEvent.AddListener(OnStudentDied);
            student.EscapeEvent.AddListener(OnStudentEscaped);
        }
    }



    private void SetStudentList()
    {
        GameObject[] studentTagObjects = GameObject.FindGameObjectsWithTag("Student");

        foreach (GameObject obj in studentTagObjects)
        {
            PostStudent student = obj.GetComponent<PostStudent>();
            if (student == null) continue;
            _students.Add(student);
        }
    }



    private void Update()
    {
        CountWorkingStudents();
        ProgressProject_T();
        ProgressProject();
        DecreaseStats();
        UpdateUIs();
    }



    private void CountWorkingStudents()
    {
        _workingStudCount = 0;
        foreach (var student in _students)
        {
            if (student.IsWorking)
            {
                _workingStudCount++;
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



    private void GameOver()
    {

    }


    private void OnStudentEscaped(PostStudent student)
    {

    }



    private void OnStudentDied(PostStudent student)
    {
        if (student.IsDoingHazardBehavior == false)
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



    private void DecreaseStats()
    {
        _timerStat.Decrease(Time.deltaTime);
        _chaosStat.Decrease(_chaosDecrease * Time.deltaTime);
    }
}
