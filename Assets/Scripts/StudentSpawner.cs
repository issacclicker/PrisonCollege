using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StudentSpawner : MonoBehaviour
{
    [SerializeField] private SpawnEntry[] spawnEntries;



    public List<PostStudent> SpawnStudents()
    {
        List<PostStudent> studentList = new();
        foreach (var entry in spawnEntries)
        {
            PostStudent student = Instantiate(entry.studentPrefab, entry.spawnTransform.position, entry.spawnTransform.rotation).GetComponent<PostStudent>();
            student.SeatSpot = entry.seatSpot;
            studentList.Add(student);
        }
        return studentList;
    }
}


[System.Serializable]
public struct SpawnEntry
{
    public GameObject studentPrefab;
    public MonitorSpot seatSpot;
    public Transform spawnTransform;
}