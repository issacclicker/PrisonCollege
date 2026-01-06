using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Blackboard
{
    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }
    public Transform Avatar { get; private set; }
    public BehaviorWeightSet BehaviorWeightSet { get; private set; }
    public StageSpots StageSpots { get; private set; }

    //public void Setup(NavMeshAgent agent, Animator animator, Transform transform)
    //{
    //    Agent = agent;
    //    Anim = animator;
    //    Avatar = transform;
    //}


    public Blackboard(GameObject owner, BehaviorWeightSet weightSet, StageSpots spots)
    {
        this.Agent = owner.GetComponent<NavMeshAgent>();
        this.Anim = owner.GetComponentInChildren<Animator>();
        this.Avatar = owner.transform;

        this.BehaviorWeightSet = weightSet;
        this.StageSpots = spots;
    }



    public Vector3 destPosition;
    public BehaveSpot destSpot;
    public BehaveSpot mySeatSpot;
    public bool isBehaving;
    public AIState currentState;
    public DamageReceiver targetDamageable;
    public GameObject targetObject;
    public BehaviorType destBehavior;

    public bool IsSeating()
    {
        return isBehaving && (destSpot == mySeatSpot);
    }
}



public enum AIState { Idle, Working, Fighting }
