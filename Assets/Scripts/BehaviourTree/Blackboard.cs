using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class Blackboard
{
    public NavMeshAgent Agent { get; private set; }
    public Animator Anim { get; private set; }
    public Transform Avatar { get; private set; }
    public BehaviorWeightSet BehaviorWeightSet { get; private set; }
    public StageSpots StageSpots { get; private set; }
    public GameObject Player { get; private set; }

    public UnityEvent EscapeSuccessEvent = new();

    //public void Setup(NavMeshAgent agent, Animator animator, Transform transform)
    //{
    //    Agent = agent;
    //    Anim = animator;
    //    Avatar = transform;
    //}


    public Blackboard(GameObject owner, BehaviorWeightSet weightSet, StageSpots spots, GameObject player)
    {
        this.Agent = owner.GetComponent<NavMeshAgent>();
        this.Anim = owner.GetComponentInChildren<Animator>();
        this.Avatar = owner.transform;
        this.Player = player;
        this.coopData = new();

        this.BehaviorWeightSet = weightSet;
        this.StageSpots = spots;
        this.currentState = AIState.Idle;
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
    public bool isDamaged;
    public bool isStunned;
    public bool isEscaping;

    public bool hasToWork;
    public bool hasToFrenzy;

    public bool isForceBehavior;

    public CoopData coopData;

    public bool IsSeating()
    {
        return isBehaving && (destSpot == mySeatSpot);
    }


    //나쁜 행동중에는 코옵 불가능
    //public bool CanCoop => coopData.spot == null && !isEscaping
    //    && destBehavior != BehaviorType.Tackle
    //    && destBehavior != BehaviorType.RushThrough
    //    && destBehavior != BehaviorType.Escape;
    //    //&& destBehavior != BehaviorType.Fight;


    public bool CanCoop => coopData.spot == null && destBehavior.GetSafety() == BehaviorSafety.Safe && targetObject == null && !isForceBehavior && isEscaping == false;



    public void LeadCoop()
    {
        coopData.spot = destSpot as CoopSpot;
        coopData.type = destBehavior;
        coopData.isLeader = true;
    }



    public void InviteCoop(CoopSpot spot, BehaviorType type)
    {
        coopData.spot = spot;
        coopData.type = type;
        coopData.isLeader = false;
    }



    public void ExecuteCoop(GameObject targetObject = null)
    {
        coopData.slotIndex = coopData.spot.Index;
        coopData.isExecuting = true;
        coopData.targetObject = targetObject;
    }



    public void ExecuteTalk()
    {
        coopData.isExecuting = true;
        coopData.targetAnimName = "Talking";
    }



    public void SecadeCoop()
    {
        coopData.spot = null;
        coopData.type = BehaviorType.None;
        coopData.isLeader = false;
        coopData.slotIndex = -1;
        coopData.isExecuting = false;
        coopData.targetObject = null;
        coopData.targetAnimName = null;
    }


    public void DisableSpot()
    {
        coopData.spot = null;
    }
}



public struct CoopData
{
    public CoopSpot spot; // 협동 지점
    public BehaviorType type;
    public bool isLeader;              // "Leader" 또는 "Follower"
    public int slotIndex;            // 배정된 자리 번호 (0, 1, 2...)
    public bool isExecuting;         // 실행 중인지 여부
    public GameObject targetObject;
    public string targetAnimName;
}



public enum AIState { Idle, Working, Attacking }
