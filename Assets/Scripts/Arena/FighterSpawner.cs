using UnityEngine;

public class FighterSpawner : MonoBehaviour
{
    [SerializeField] private DamageData _fightData;
    [SerializeField] private GameObject _studentPrefab1;
    [SerializeField] private GameObject _studentPrefab2;
    [SerializeField] private Transform _startPoint1;
    [SerializeField] private Transform _startPoint2;
    private Fighter _fighter1;
    private Fighter _fighter2;



    private void Start()
    {
        SpawnTwoFighters(_studentPrefab1, _studentPrefab2);
    }


    //public void SpawnTwoFighters(StudentEntry entry1, StudentEntry entry2) 
    //{
    //    SpawnAndModifyStudent(entry1.prefab);
    //    SpawnAndModifyStudent(entry2.prefab);
    //}



    public void SpawnTwoFighters(GameObject prefab1, GameObject prefab2)
    {
        _fighter1 = SpawnAndModifyStudent(prefab1, _startPoint1.position, _startPoint2.position);
        _fighter2 = SpawnAndModifyStudent(prefab2, _startPoint2.position, _startPoint1.position);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _fighter1.StartFight(_fighter2.gameObject);
            _fighter2.StartFight(_fighter1.gameObject);
        }
    }



    public Fighter SpawnAndModifyStudent(GameObject originalPrefab, Vector3 spawnPosition, Vector3 otherPosition)
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
