using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RagdollStandup : MonoBehaviour
{
    private class BoneTransform
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [Header("Settings")]
    [SerializeField] private float _timeToWakeup = 3f; // 사용 안함? (Blend 시간으로 대체된듯)
    [SerializeField] private string _standupStateName = "StandUp";
    [SerializeField] private string _standupClipName = "StandUpClip";
    [SerializeField] private float _timeToResetBones = 0.5f;

    [Header("Components")]
    private Transform _hipsBone;
    private Animator _anim;
    private NavMeshAgent _agent;
    private Rigidbody[] _boneRigidBodies;
    private Rigidbody _rootRigidbody;
    private Collider _rootCollider;

    private BoneTransform[] _standupBones;
    private BoneTransform[] _ragdollBones;
    private Transform[] _bones;

    public UnityEvent StandUpCompleteEvent = new();

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _rootRigidbody = GetComponent<Rigidbody>();
        _rootCollider = GetComponent<Collider>();

        _hipsBone = _anim.GetBoneTransform(HumanBodyBones.Hips);
        _boneRigidBodies = _hipsBone.GetComponentsInChildren<Rigidbody>();

        // 뼈대 배열 초기화
        _bones = new Transform[_boneRigidBodies.Length];
        _standupBones = new BoneTransform[_bones.Length];
        _ragdollBones = new BoneTransform[_bones.Length];

        for (int i = 0; i < _boneRigidBodies.Length; i++)
        {
            _bones[i] = _boneRigidBodies[i].transform;
            _standupBones[i] = new BoneTransform();
            _ragdollBones[i] = new BoneTransform();
        }

        // Awake에서 미리 일어나기 애니메이션의 '첫 프레임' 포즈를 저장해둡니다.
        //PopulateAnimationStartBoneTransform(_standupClipName, _standupBones);
    }


    public void WakeUp()
    {
        DOTween.Kill(this);
        AlignRotationToHips();
        AlignPositonToHips();
        PopulateBoneTransform(_ragdollBones);
        CaptureStandUpPose();
        BlendToAnimation(_timeToResetBones);
    }

    // 실시간 포즈 캡처를 위한 함수
    private void CaptureStandUpPose()
    {
        _anim.Play(_standupStateName, 0, 0f);
        _anim.Update(0f);
        PopulateBoneTransform(_standupBones);
    }



    public void BlendToAnimation(float duration)
    {
        DOVirtual.Float(0f, 1f, duration, (float value) =>
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                // value 0: 래그돌 포즈, value 1: 일어나기 시작 포즈
                _bones[i].localPosition = Vector3.Lerp(_ragdollBones[i].position, _standupBones[i].position, value);
                _bones[i].localRotation = Quaternion.Lerp(_ragdollBones[i].rotation, _standupBones[i].rotation, value);
            }
        })
        .SetEase(Ease.InQuad)
        .SetTarget(this) // 스크립트가 파괴되면 트윈도 정지
        .OnComplete(() =>
        {
            // 6. 보간이 끝나면 비로소 애니메이터를 켜고 애니메이션 재생
            //_anim.enabled = true;
            SetRagdoll(false);
            _anim.Rebind();
            _anim.Update(0f); // 초기화
            _anim.Play(_standupStateName);
            //_anim.CrossFadeInFixedTime(_standupStateName, 0.15f, 0, 0f);

            // 7. 애니메이션 길이만큼 대기 후 완료 이벤트 실행
            float animLength = _anim.GetCurrentAnimatorStateInfo(0).length;
            DOVirtual.DelayedCall(animLength, OnStandUpComplete).SetTarget(this);
        });
    }

    private void OnStandUpComplete()
    {
        Debug.Log("DoTween: 캐릭터가 완전히 일어났습니다.");

        // 이동 가능하도록 NavMeshAgent 활성화
        if (_agent != null) _agent.enabled = true;

        StandUpCompleteEvent?.Invoke();
    }

    public void SetRagdoll(bool isActive)
    {
        _anim.enabled = !isActive;
        if (_agent != null) _agent.enabled = !isActive;

        _rootCollider.enabled = !isActive;
        _rootRigidbody.useGravity = !isActive;

        foreach (Rigidbody rb in _boneRigidBodies)
        {
            rb.isKinematic = !isActive;

            if (isActive) rb.velocity = Vector3.zero;

            if (rb.TryGetComponent(out Collider col))
            {
                col.isTrigger = !isActive;
            }
        }
    }

    private void AlignPositonToHips()
    {
        Vector3 originalHipsPos = _hipsBone.position;

        transform.position = _hipsBone.position;

        Vector3 positonOffset = _standupBones[0].position;
        positonOffset.y = 0;
        positonOffset = transform.rotation * positonOffset;
        transform.position -= positonOffset;

        if (Physics.Raycast(_hipsBone.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hitInfo, 5f))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }
        _hipsBone.position = originalHipsPos;
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredForward = -_hipsBone.up;
        desiredForward.y = 0;

        if (desiredForward.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(desiredForward);
        }

        _hipsBone.position = originalHipsPosition;
        _hipsBone.rotation = originalHipsRotation;
    }


    private void PopulateBoneTransform(BoneTransform[] boneTransforms)
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            boneTransforms[i].position = _bones[i].localPosition;
            boneTransforms[i].rotation = _bones[i].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransform(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in _anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransform(boneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }
}