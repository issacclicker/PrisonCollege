using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoopSpot : SingleStudentSpot
{
    public int Index { get; set; } = -1;
    public UnityEvent<GameObject> JoinEvent = new();
    public UnityEvent<GameObject> DisjoinEvent = new();
    public UnityEvent<GameObject> ArriveEvent = new();


    public override void Use(PostStudent userStudent)
    {
        base.Use(userStudent);
        JoinEvent?.Invoke(userStudent.gameObject);
    }



    public override void Release(PostStudent userStudent)
    {
        base.Release(userStudent);
        DisjoinEvent?.Invoke(userStudent.gameObject);
    }



    public override void Arrived(PostStudent userStudent)
    {
        base.Arrived(userStudent);
        ArriveEvent?.Invoke(userStudent.gameObject);
    }
}
