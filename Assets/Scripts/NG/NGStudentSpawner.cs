using UnityEngine;
using UnityEngine.AI;

public class NGStudentSpawner : MonoBehaviour
{
    [SerializeField] private StageSpots _stageSpots;
    [SerializeField] private GameObject _studentPrefab;
    [SerializeField] private Transform _spawnPoint;



    private void Start()
    {
        StudentNG student = SpawnAndModifyToStudentNG(_studentPrefab, _spawnPoint.position, _spawnPoint.rotation);
        //student.Stage = _stageSpots;
        student.StartBehavior();
    }



    private StudentNG SpawnAndModifyToStudentNG(GameObject originalPrefab, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        bool originalState = originalPrefab.activeSelf;
        originalPrefab.SetActive(false);
        GameObject studentObj = Instantiate(originalPrefab, spawnPosition, spawnRotation);
        originalPrefab.SetActive(originalState);
        studentObj.RemoveComponent<PostStudent>(true);
        //studentObj.RemoveComponent<NavMeshAgent>(true);
        //studentObj.RemoveComponent<DamageReceiver>(true);
        //RemoveHealthCompsNotHundred(studentObj);
        //studentObj.RemoveComponent<BaldOutlines>(true);
        //studentObj.RemoveComponentsInChildren<Outline>(true, true);
        //studentObj.RemoveComponentsInChildren<Fire>(true, true);
        //studentObj.RemoveGameObjectsWithComponent<OverlapAttacker>(true, true);

        AnimAttacher[] animAttachers = studentObj.GetComponents<AnimAttacher>();
        foreach (AnimAttacher attacher in animAttachers)
        {
            attacher.HideAll();
        }

        //studentObj.GetComponent<CharacterRagdoll>()._isAutoStandUp = false;
        //studentObj.GetComponent<AnimAttack>()._damageData = _fightData;
        //studentObj.AddComponent<DamageReceiver>();
        studentObj.AddComponent<StudentNG>();

        studentObj.SetActive(true);
        return studentObj.GetComponent<StudentNG>();
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
