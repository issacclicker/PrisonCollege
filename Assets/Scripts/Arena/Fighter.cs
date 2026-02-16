using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Fighter : MonoBehaviour
{
    private BT_Node _root;
    private Blackboard _blackboard;
    private DamageReceiver _damageReceiver;
    private CharacterRagdoll _characterRagdoll;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Collider _characterCollider;

    public UnityEvent<Fighter> DamageEvent = new();
    public UnityEvent<Fighter> DieEvent = new();

    private GameObject _enemyObject;

    private AttributeModifier _moveSpeedModifier;



    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _characterCollider = GetComponent<Collider>();
        _characterRagdoll = GetComponent<CharacterRagdoll>();
        _damageReceiver = GetComponent<DamageReceiver>();
        _damageReceiver.StatDownEvent?.AddListener(OnDamaged);
        _damageReceiver.DepletedEvent?.AddListener(OnDie);
    }



    private void Start()
    {
        _moveSpeedModifier = AttributeSystem.Instance.StudMoveSpeedMod;
        _anim.SetFloat("MoveSpeedScale", _moveSpeedModifier.GetFinalValue());
        _agent.acceleration = 100f;
        _anim.SetLayerWeight(Global.COMBAT_LAYER_INDEX, 1);
    }



    private void Update()
    {
        if (_root != null)
        {
            _root.Evaluate();
        }
    }



    public void StartFight(GameObject enemyObject)
    {
        Debug.Log($"Kill {enemyObject.name}!!");
        _blackboard = new Blackboard(gameObject, null, null, null);
        _enemyObject = enemyObject;
        _root = ConstructBehavior();
        _root.SetBlackboard(_blackboard);
    }



    private BT_Node ConstructBehavior()
    {
        return new TakeHitReactivePattern(new AttackReactivePattern
        (
            new Selector(new List<BT_Node>
            {
                new OverrideAttackTarget(() => _enemyObject),
                new Sequence(new List<BT_Node>
                {
                    new LerpLayerWeight(Global.COMBAT_LAYER_INDEX, 0, 5),
                    new SetAnimBool("Victorying", true)
                })
            })));
    }



    private void OnDamaged(HitInfo hitInfo, float hitAmount)
    {
        DamageEvent?.Invoke(this);
        _blackboard.isDamaged = true;
        _blackboard.isStunned = true;
    }


    private void OnDie(HitInfo hitInfo)
    {
        DieEvent?.Invoke(this);
        _root = null;
        _agent.speed = 0;
        _agent.enabled = false;
        _anim.enabled = false;
        _characterCollider.enabled = false;
        _blackboard.targetDamageable = null;
        _blackboard.targetObject = null;
        StopAllCoroutines();
        //_ragdollStandup.SetRagdoll(true);
        _characterRagdoll.TriggerRagdoll();
        _characterRagdoll.ApplyBoneImpact(hitInfo.hitPoint, hitInfo.hitRotation, hitInfo.impulse);

        //Invoke(nameof(Revive), 2f);
    }
}
