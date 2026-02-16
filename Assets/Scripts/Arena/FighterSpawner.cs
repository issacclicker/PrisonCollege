using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils;
using DG.Tweening;

public class FighterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FighterInfo
    {
        public Fighter mainComp;
        public bool isDead;
        public bool isBetted;
    }

    [SerializeField] private DamageData _fightData;
    [SerializeField] private Transform _startPoint1;
    [SerializeField] private Transform _startPoint2;
    [SerializeField] private Transform[] _spectatorSpots;
    private FighterInfo _fighter1;
    private FighterInfo _fighter2;
    private Transform _focusPoint;

    [Header("StudentInfo UIs")]
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private Image _leftProfileImg;
    [SerializeField] private TextMeshProUGUI _leftNameTmp;
    [SerializeField] private StatBar _leftHealthBar;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] private Image _rightProfileImg;
    [SerializeField] private TextMeshProUGUI _rightNameTmp;
    [SerializeField] private StatBar _rightHealthBar;
    [Header("MainPanel UIs")]
    //[SerializeField] private TextMeshProUGUI _timerTmp;
    [SerializeField] private BettingHelper _bettingHelper;
    [SerializeField] private FightFocusCamera _focusCamera;
    [Header("Helmet & Gloves")]
    [SerializeField] private GameObject _leftGlovePrefab;
    [SerializeField] private GameObject _rightGlovePrefab;
    [SerializeField] private GameObject _helmetPrefab;
    [SerializeField] private Material _redMat;
    [SerializeField] private Material _blueMat;
    private bool _isFighting = false;
    private bool _isWinnerDetermined = false;
    private int _bettedMoney = 0;



    private void Awake()
    {
        _fighter1 = new FighterInfo();
        _fighter2 = new FighterInfo();
    }



    private void Start()
    {
        _focusPoint = new GameObject().transform;
        _focusPoint.position = (_startPoint1.transform.position + _startPoint2.transform.position) * 0.5f + Vector3.up;
        _focusCamera.target = _focusPoint;
        SpawnFightersAndSpectators();
        AttachHelmetAndGloves();
        _fighter1.mainComp.DamageEvent.AddListener(OnFighterDamaged);
        _fighter2.mainComp.DamageEvent.AddListener(OnFighterDamaged);
        _fighter1.mainComp.DieEvent.AddListener(OnFighterDead);
        _fighter2.mainComp.DieEvent.AddListener(OnFighterDead);
    }


    private void Update()
    {
        //if (!_isFighting && Input.GetKeyDown(KeyCode.Space))
        //{
        //    _isFighting = true;
        //    _fighter1.mainComp.StartFight(_fighter2.mainComp.gameObject);
        //    _fighter2.mainComp.StartFight(_fighter1.mainComp.gameObject);
        //}
        //if (_isFighting)
        //{

        //}
        if (_fighter2.isDead && _fighter2.isDead) return;
        Vector3 _focusPosition = Vector3.zero + Vector3.up;
        _focusPosition += _fighter1.isDead ? Vector3.zero : _fighter1.mainComp.transform.position * 0.5f;
        _focusPosition += _fighter2.isDead ? Vector3.zero : _fighter2.mainComp.transform.position * 0.5f;
        _focusPoint.position = _focusPosition;
    }



    public void OnFighterSelected(SelectedSide selectedSide)
    {
        _fighter1.mainComp.SetOutlines(selectedSide == SelectedSide.Left);
        _fighter2.mainComp.SetOutlines(selectedSide == SelectedSide.Right);
    }


    public void ChooseAndStartFight(SelectedSide selectedSide, int bettedMoney)
    {
        _isFighting = true;
        _bettedMoney = bettedMoney;
        _fighter1.mainComp.StartFight(_fighter2.mainComp.gameObject);
        _fighter2.mainComp.StartFight(_fighter1.mainComp.gameObject);
        FighterInfo choosedFighter = selectedSide == SelectedSide.Left ? _fighter1 : _fighter2;
        choosedFighter.isBetted = true;
    }



    private void OnFighterDamaged(Fighter fighter)
    {
        RectTransform targetPanel = fighter == _fighter1.mainComp ? _leftPanel : _rightPanel;
        targetPanel.DOShakeAnchorPos(0.5f, 20f, 10, 90f);
    }



    private void AttachHelmetAndGloves()
    {
        GameObject leftGlove1 = Instantiate(_leftGlovePrefab);
        GameObject rightGlove1 = Instantiate(_rightGlovePrefab);
        GameObject helmet1 = Instantiate(_helmetPrefab);

        GameObject leftGlove2 = Instantiate(_leftGlovePrefab);
        GameObject rightGlove2 = Instantiate(_rightGlovePrefab);
        GameObject helmet2 = Instantiate(_helmetPrefab);

        leftGlove1.GetComponentInChildren<Renderer>().material = _redMat;
        rightGlove1.GetComponentInChildren<Renderer>().material = _redMat;
        helmet1.GetComponent<Renderer>().material = _redMat;

        leftGlove2.GetComponentInChildren<Renderer>().material = _blueMat;
        rightGlove2.GetComponentInChildren<Renderer>().material = _blueMat;
        helmet2.GetComponent<Renderer>().material = _blueMat;

        _fighter1.mainComp.AttachLeftGlove(leftGlove1);
        _fighter1.mainComp.AttachRightGlove(rightGlove1);
        _fighter1.mainComp.AttachHelmet(helmet1);

        _fighter2.mainComp.AttachLeftGlove(leftGlove2);
        _fighter2.mainComp.AttachRightGlove(rightGlove2);
        _fighter2.mainComp.AttachHelmet(helmet2);
    }



    private void OnFighterDead(Fighter fighter)
    {
        RectTransform targetPanel = fighter == _fighter1.mainComp ? _leftPanel : _rightPanel;
        targetPanel.DOShakeAnchorPos(0.4f, 30f, 20);
        targetPanel.DOShakeRotation(0.4f, 10f);

        FighterInfo deadFighter = fighter == _fighter1.mainComp ? _fighter1 : _fighter2;
        deadFighter.isDead = true;
        Invoke(nameof(DetermineWinner), 0.5f);
    }



    private void StopFighting()
    {
        if (_isFighting == false) return;
        _isFighting = false;
    }



    private void DetermineWinner()
    {
        if (_isWinnerDetermined) return;
        _isWinnerDetermined = true;
        CancelInvoke(nameof(DetermineWinner));

        if (_fighter1.isDead == _fighter2.isDead)
        {
            //公铰何
            ShowResult(WinSide.None);
        }
        else if (_fighter2.isDead)
        {
            //哭率 铰府
            if (_fighter1.isBetted)
            {
                GainMoney();
            }
            else
            {
                LoseMoney();
            }
            ShowResult(WinSide.Left);
        }
        else
        {
            //坷弗率 铰府
            if (_fighter2.isBetted)
            {
                GainMoney();
            }
            else
            {
                LoseMoney();
            }
            ShowResult(WinSide.Right);
        }
    }



    private void GainMoney()
    {
        int currentMoney = InventorySystem.Instance.Money;
        InventorySystem.Instance.SetMoney(currentMoney + _bettedMoney * 2);
        _bettingHelper.UpdateTotalMoneyUI();
    }



    private void LoseMoney()
    {
        int currentMoney = InventorySystem.Instance.Money;
        InventorySystem.Instance.SetMoney(currentMoney - _bettedMoney);
        _bettingHelper.UpdateTotalMoneyUI();
    }



    private void ShowResult(WinSide winSide)
    {

    }



    private void SpawnFightersAndSpectators()
    {
        StudentEntry[] fighterEntries = StudentDB.Instance.GetRandomStudentEntries(2, out StudentEntry[] spectatorEntries);
        SpawnTwoFighters(fighterEntries[0], fighterEntries[1]);
        SpawnSpectators(spectatorEntries);
        BindFightersInfo(fighterEntries[0], fighterEntries[1]);
        _bettingHelper.WriteButtonNameTmp(fighterEntries[0].koreanName, fighterEntries[1].koreanName);
    }


    private void BindFightersInfo(StudentEntry leftFighterEntry, StudentEntry rightFighterEntry)
    {
        _leftProfileImg.sprite = leftFighterEntry.profile;
        _leftNameTmp.text = $"{leftFighterEntry.koreanName}  <size=60%>{leftFighterEntry.course}</size>";
        _leftHealthBar.SetTarget(_fighter1.mainComp.GetComponent<Health>());

        _rightProfileImg.sprite = rightFighterEntry.profile;
        _rightNameTmp.text = $"<size=60%>{rightFighterEntry.course}</size>  {rightFighterEntry.koreanName}";
        _rightHealthBar.SetTarget(_fighter2.mainComp.GetComponent<Health>());
    }



    public void SpawnTwoFighters(StudentEntry entry1, StudentEntry entry2)
    {
        _fighter1.mainComp = SpawnAndModifyToFighter(entry1.prefab, _startPoint1.position, _startPoint2.position);
        _fighter2.mainComp = SpawnAndModifyToFighter(entry2.prefab, _startPoint2.position, _startPoint1.position);
    }



    private void SpawnSpectators(StudentEntry[] spectatorEntries)
    {
        Transform[] spots = _spectatorSpots.GetRandomElements(spectatorEntries.Length);
        for (int i = 0; i < spots.Length; i++)
        {
            Vector3 lookDir = _focusPoint.position - spots[i].position;
            Spectator spectator = SpawnAndModifyToSpectator(spectatorEntries[i].prefab, spots[i].transform.position, lookDir);
            spectator.StartCheer(_focusPoint);
        }
    }



    public void SpawnTwoFighters(GameObject prefab1, GameObject prefab2)
    {
        _fighter1.mainComp = SpawnAndModifyToFighter(prefab1, _startPoint1.position, _startPoint2.position);
        _fighter2.mainComp = SpawnAndModifyToFighter(prefab2, _startPoint2.position, _startPoint1.position);
    }



    private Spectator SpawnAndModifyToSpectator(GameObject originalPrefab, Vector3 spawnPosition, Vector3 otherPosition)
    {
        bool originalState = originalPrefab.activeSelf;
        originalPrefab.SetActive(false);
        GameObject studentObj = Instantiate(originalPrefab, spawnPosition, Quaternion.LookRotation(otherPosition));
        originalPrefab.SetActive(originalState);
        studentObj.RemoveComponent<PostStudent>(true);
        //studentObj.RemoveComponent<CharacterRagdoll>(true);
        studentObj.RemoveComponent<DamageReceiver>(true);
        RemoveHealthCompsNotHundred(studentObj);
        studentObj.RemoveComponent<BaldOutlines>(true);
        studentObj.RemoveComponentsInChildren<Outline>(true, true);
        studentObj.RemoveComponentsInChildren<Fire>(true, true);
        studentObj.RemoveGameObjectsWithComponent<OverlapAttacker>(true, true);

        AnimAttacher[] animAttachers = studentObj.GetComponents<AnimAttacher>();
        foreach (AnimAttacher attacher in animAttachers)
        {
            attacher.HideAll();
        }

        studentObj.GetComponent<CharacterRagdoll>()._isAutoStandUp = false;
        studentObj.GetComponent<AnimAttack>()._damageData = _fightData;
        studentObj.AddComponent<DamageReceiver>();
        studentObj.AddComponent<Spectator>();

        studentObj.SetActive(true);
        return studentObj.GetComponent<Spectator>();
    }



    private Fighter SpawnAndModifyToFighter(GameObject originalPrefab, Vector3 spawnPosition, Vector3 otherPosition)
    {
        bool originalState = originalPrefab.activeSelf;
        originalPrefab.SetActive(false);
        GameObject studentObj = Instantiate(originalPrefab, spawnPosition, Quaternion.LookRotation(otherPosition));
        originalPrefab.SetActive(originalState);
        studentObj.RemoveComponent<PostStudent>(true);
        //studentObj.RemoveComponent<CharacterRagdoll>(true);
        studentObj.RemoveComponent<DamageReceiver>(true);
        RemoveHealthCompsNotHundred(studentObj);
        studentObj.RemoveComponent<BaldOutlines>(true);
        //studentObj.RemoveComponentsInChildren<Outline>(true, true);
        studentObj.RemoveComponentsInChildren<Fire>(true, true);
        studentObj.RemoveGameObjectsWithComponent<OverlapAttacker>(true, true);

        AnimAttacher[] animAttachers = studentObj.GetComponents<AnimAttacher>();
        foreach (AnimAttacher attacher in animAttachers)
        {
            attacher.HideAll();
        }

        studentObj.GetComponent<CharacterRagdoll>()._isAutoStandUp = false;
        studentObj.GetComponent<AnimAttack>()._damageData = _fightData;
        studentObj.AddComponent<DamageReceiver>();
        studentObj.AddComponent<Fighter>();

        studentObj.SetActive(true);
        return studentObj.GetComponent<Fighter>();
    }



    private void RemoveHealthCompsNotHundred(GameObject targetObject)
    {
        Health[] healths = targetObject.GetComponents<Health>();
        foreach (Health health in healths)
        {
            if (!Mathf.Approximately(health.Max, 100))
            {
                DestroyImmediate(health);
            }
        }
    }


    public enum WinSide
    {
        None, Left, Right
    }
}
