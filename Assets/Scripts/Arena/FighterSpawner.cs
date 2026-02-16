using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utils;
using DG.Tweening;

public class FighterSpawner : MonoBehaviour
{
    [SerializeField] private DamageData _fightData;
    [SerializeField] private Transform _startPoint1;
    [SerializeField] private Transform _startPoint2;
    [SerializeField] private Transform[] _spectatorSpots;
    private Fighter _fighter1;
    private Fighter _fighter2;
    private Transform _focusPoint;

    [Header("UI Bindings")]
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private Image _leftProfileImg;
    [SerializeField] private TextMeshProUGUI _leftNameTmp;
    [SerializeField] private StatBar _leftHealthBar;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] private Image _rightProfileImg;
    [SerializeField] private TextMeshProUGUI _rightNameTmp;
    [SerializeField] private StatBar _rightHealthBar;
    [Header("Helmet & Gloves")]
    [SerializeField] private GameObject _leftGlovePrefab;
    [SerializeField] private GameObject _rightGlovePrefab;
    [SerializeField] private GameObject _helmetPrefab;
    [SerializeField] private Material _redMat;
    [SerializeField] private Material _blueMat;



    private void Start()
    {
        _focusPoint = new GameObject().transform;
        _focusPoint.position = (_startPoint1.transform.position + _startPoint2.transform.position) * 0.5f + Vector3.up;
        SpawnFightersAndSpectators();
        AttachHelmetAndGloves();
        _fighter1.DamageEvent.AddListener(OnFighterDamaged);
        _fighter2.DamageEvent.AddListener(OnFighterDamaged);
        _fighter1.DieEvent.AddListener(OnFighterDead);
        _fighter2.DieEvent.AddListener(OnFighterDead);
    }



    private void OnFighterDamaged(Fighter fighter)
    {
        RectTransform targetPanel = fighter == _fighter1 ? _leftPanel : _rightPanel;
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

        _fighter1.AttachLeftGlove(leftGlove1);
        _fighter1.AttachRightGlove(rightGlove1);
        _fighter1.AttachHelmet(helmet1);

        _fighter2.AttachLeftGlove(leftGlove2);
        _fighter2.AttachRightGlove(rightGlove2);
        _fighter2.AttachHelmet(helmet2);
    }



    private void OnFighterDead(Fighter fighter)
    {
        RectTransform targetPanel = fighter == _fighter1 ? _leftPanel : _rightPanel;
        targetPanel.DOShakeAnchorPos(0.4f, 30f, 20);
        targetPanel.DOShakeRotation(0.4f, 10f);
    }



    private void SpawnFightersAndSpectators()
    {
        StudentEntry[] fighterEntries = StudentDB.Instance.GetRandomStudentEntries(2, out StudentEntry[] spectatorEntries);
        SpawnTwoFighters(fighterEntries[0], fighterEntries[1]);
        SpawnSpectators(spectatorEntries);
        BindFightersInfo(fighterEntries[0], fighterEntries[1]);
    }


    private void BindFightersInfo(StudentEntry leftFighterEntry, StudentEntry rightFighterEntry)
    {
        _leftProfileImg.sprite = leftFighterEntry.profile;
        _leftNameTmp.text = $"{leftFighterEntry.koreanName}  <size=60%>{leftFighterEntry.course}</size>";
        _leftHealthBar.SetTarget(_fighter1.GetComponent<Health>());

        _rightProfileImg.sprite = rightFighterEntry.profile;
        _rightNameTmp.text = $"<size=60%>{rightFighterEntry.course}</size>  {rightFighterEntry.koreanName}";
        _rightHealthBar.SetTarget(_fighter2.GetComponent<Health>());
    }



    public void SpawnTwoFighters(StudentEntry entry1, StudentEntry entry2)
    {
        _fighter1 = SpawnAndModifyToFighter(entry1.prefab, _startPoint1.position, _startPoint2.position);
        _fighter2 = SpawnAndModifyToFighter(entry2.prefab, _startPoint2.position, _startPoint1.position);
    }



    private void SpawnSpectators(StudentEntry[] spectatorEntries)
    {
        Transform[] spots = _spectatorSpots.GetRandomElements(spectatorEntries.Length);
        for (int i = 0; i < spots.Length; i++)
        {
            Spectator spectator = SpawnAndModifyToSpectator(spectatorEntries[i].prefab, spots[i].transform.position, spots[i].transform.forward);
            spectator.StartCheer(_focusPoint);
        }
    }



    public void SpawnTwoFighters(GameObject prefab1, GameObject prefab2)
    {
        _fighter1 = SpawnAndModifyToFighter(prefab1, _startPoint1.position, _startPoint2.position);
        _fighter2 = SpawnAndModifyToFighter(prefab2, _startPoint2.position, _startPoint1.position);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _fighter1.StartFight(_fighter2.gameObject);
            _fighter2.StartFight(_fighter1.gameObject);
        }
        _focusPoint.position = (_fighter2.transform.position + _fighter2.transform.position) * 0.5f + Vector3.up;
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
}
