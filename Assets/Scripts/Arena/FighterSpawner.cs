using UnityEngine;
using static Utils;

public class FighterSpawner : MonoBehaviour
{
    [SerializeField] private DamageData _fightData;
    [SerializeField] private GameObject _studentPrefab1;
    [SerializeField] private GameObject _studentPrefab2;
    [SerializeField] private Transform _startPoint1;
    [SerializeField] private Transform _startPoint2;

    [SerializeField] private GameObject[] _spectatorPrefabs;
    [SerializeField] private Transform[] _spectatorSpots;
    private Fighter _fighter1;
    private Fighter _fighter2;
    private Transform _focusPoint;



    private void Start()
    {
        _focusPoint = new GameObject().transform;
        _focusPoint.position = (_startPoint1.transform.position + _startPoint2.transform.position) * 0.5f + Vector3.up;
        SpawnTwoFighters(_studentPrefab1, _studentPrefab2);
        SpawnSpectators();
    }


    //public void SpawnTwoFighters(StudentEntry entry1, StudentEntry entry2) 
    //{
    //    SpawnAndModifyStudent(entry1.prefab);
    //    SpawnAndModifyStudent(entry2.prefab);
    //}



    private void SpawnSpectators()
    {
        Transform[] spots = _spectatorSpots.GetRandomElements(_spectatorPrefabs.Length);
        for (int i = 0; i < spots.Length; i++)
        {
            Spectator spectator = SpawnAndModifyToSpectator(_spectatorPrefabs[i], spots[i].transform.position, spots[i].transform.forward);
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
        //studentObj.RemoveComponents<Health>(true);
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
        //studentObj.RemoveComponents<Health>(true);
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
}
