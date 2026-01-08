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
        this.coopData = new();

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
    public BehaviorType prevBehavior;
    public BehaviorType destBehavior;

    public CoopData coopData;

    public bool IsSeating()
    {
        return isBehaving && (destSpot == mySeatSpot);
    }



    public bool CanCoop => coopData.spot == null && currentState == AIState.Idle;



    public void LeadCoop()
    {
        coopData.spot = destSpot as CoopSpot;
        coopData.isLeader = true;
    }



    public void InviteCoop(CoopSpot spot)
    {
        coopData.spot = spot;
        coopData.isLeader = false;
    }



    public void ExecuteCoop()
    {
        coopData.slotIndex = coopData.spot.Index;
        coopData.isExecuting = true;
    }



    public void SecadeCoop()
    {
        coopData.spot = null;
        coopData.isLeader = false;
        coopData.slotIndex = -1;
        coopData.isExecuting = false;
    }
}



public struct CoopData
{
    public CoopSpot spot; // 협동 지점
    public bool isLeader;              // "Leader" 또는 "Follower"
    public int slotIndex;            // 배정된 자리 번호 (0, 1, 2...)
    public bool isExecuting;         // 실행 중인지 여부
}



public enum AIState { Idle, Working, Attacking }
