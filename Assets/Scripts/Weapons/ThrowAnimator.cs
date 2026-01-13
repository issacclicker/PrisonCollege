using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOTweenSeq = DG.Tweening.Sequence;

public class ThrowAnimator : WeaponAnimator
{
    [Header("--- Throw Settings ---")]
    [SerializeField] private GameObject _throwablePrefab; // ���� ��ü ������
    [SerializeField] private Transform _throwableModel; // ���� ��ü ������
    [SerializeField] private Transform _throwPoint;       // ��ü�� ������ ��ġ (���� ��ó)
    [SerializeField] private float _throwForce = 15f;     // ������ ��
    [SerializeField] private Vector3 _throwOffset = new Vector3(0.5f, -0.2f, 1.0f);
    [SerializeField] private float _flipSpeed = 20f; // ���Ʒ� ȸ�� �ӵ� (�������� ����)
    [Range(0f, 1f)] public float _spreadAmount = 0.02f; // ź���� ����
    [Range(0f, 1f)] public float _torqueRandomness = 0.5f; // ȸ�� �ұ�Ģ��

    private Vector3 _initialScale;
    private Quaternion _initialRotation;
    private Vector3 _initialPosition;


    protected override void Awake()
    {
        base.Awake();
        _initialScale = _throwableModel.localScale;
        _initialRotation = _throwableModel.localRotation;
        _initialPosition = _throwableModel.localPosition;
    }


    protected override void AddAttackFrames(DOTweenSeq attackAnimSeq, System.Action attackExecution, float attackDuration)
    {
        // [�ٽ� 1] ������ ���� ��� ���� �մϴ�. 
        // ���� ������ ���߿� ���ܼ� ���� ä�� �������� ��츦 ����մϴ�.
        attackAnimSeq.AppendCallback(() => _throwableModel.gameObject.SetActive(true));

        float recoilTime = attackDuration * 0.2f;
        float returnTime = attackDuration * 0.7f; // �ð� �հ踦 ���߱� ���� �ణ ����
        float throwActionTime = 0.1f;              // ������ ���� ���� �ð�

        // 1. �غ� ���� (�ڷ� ����)
        attackAnimSeq.Append(transform.DOLocalMove(new Vector3(0.1f, -0.1f, -0.2f), recoilTime).SetEase(Ease.OutQuad));
        attackAnimSeq.Join(transform.DOLocalRotate(new Vector3(-20f, 10f, 0f), recoilTime).SetEase(Ease.OutQuad));

        // 2. ������ ���� (�߻� �� �� �����)
        attackAnimSeq.AppendCallback(() => {
            _throwableModel.gameObject.SetActive(false);
            //ThrowProjectile();
            attackExecution.Invoke();
        });

        // 3. ������ ���� (�� ����)
        attackAnimSeq.Append(transform.DOLocalMove(new Vector3(0f, 0f, 0.2f), throwActionTime).SetEase(Ease.OutCubic));

        // [�ٽ� 2] ���� ���� ������ ���� �ٽ� Ȱ��ȭ!
        // OnComplete�� �������� ������ ����Ǿ�߸� ���������, 
        // AppendCallback�� ������ �帧�� �ش� Ÿ�ֿ̹� ������ ����˴ϴ�.
        attackAnimSeq.AppendCallback(() => {
            _throwableModel.gameObject.SetActive(true);

            // [���� ����] ũ��� 0, ��ġ�� �ణ �Ʒ����� ����
            _throwableModel.localScale = Vector3.zero;
            _throwableModel.localPosition = _initialPosition + new Vector3(0, -0.1f, 0);

            // [�ִϸ��̼�] ����ص� '�ʱⰪ'���� ����
            _throwableModel.DOScale(_initialScale, returnTime).SetEase(Ease.OutBack);
            _throwableModel.DOLocalMove(_initialPosition, returnTime).SetEase(Ease.OutCubic);
            _throwableModel.DOLocalRotateQuaternion(_initialRotation, returnTime).SetEase(Ease.OutCubic);
        });

        // 4. ���� ���� (���� ��ġ��)
        attackAnimSeq.Append(transform.DOLocalMove(Vector3.zero, returnTime).SetEase(Ease.OutCubic));
        attackAnimSeq.Join(transform.DOLocalRotate(Vector3.zero, returnTime).SetEase(Ease.OutCubic));
    }



    private void ThrowProjectile()
    {
        Camera playerCamera = Camera.main;
        if (_throwablePrefab == null || playerCamera == null) return;

        // 1. ī�޶� ���� ������
        Vector3 camPos = playerCamera.transform.position;
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        Vector3 camUp = playerCamera.transform.up;
        Quaternion camRot = playerCamera.transform.rotation;

        // 2. ���� ��ġ ��� (ī�޶� �߽� ������)
        Vector3 spawnPos = camPos
                           + (camRight * _throwOffset.x)
                           + (camUp * _throwOffset.y)
                           + (camForward * _throwOffset.z);

        // 3. ���� ȸ�� (�� ���� ����)
        Quaternion finalRot = camRot * _throwableModel.localRotation;

        GameObject projectileObj = Instantiate(_throwablePrefab, spawnPos, finalRot);
        projectileObj.transform.localScale = _throwableModel.localScale;
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Owner = _weaponController.Owner;

        Rigidbody rb = projectileObj.GetComponent<Rigidbody>();
        if (rb == null) rb = projectileObj.AddComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.maxAngularVelocity = 1000f;

            // 4. [�������� �߰�] �߻� ���⿡ ��¦ ���� �ֱ�
            // camForward ���⿡ ���� �̼��ϰ� ��/�Ʒ�/�� ���� ���͸� �����ϴ�.
            Vector3 randomSpread = (camUp * Random.Range(-_spreadAmount, _spreadAmount))
                                 + (camRight * Random.Range(-_spreadAmount, _spreadAmount));
            Vector3 finalThrowDir = (camForward + randomSpread).normalized;

            rb.AddForce(finalThrowDir * _throwForce, ForceMode.Impulse);

            // 5. [�������� �߰�] ȸ�� ������ �ߵ��ϰ� �����
            // ������ ������ ȸ��(camRight)�� �ణ�� �����¿� ��Ʋ�⸦ �����ϴ�.
            Vector3 randomTorque = new Vector3(
                Random.Range(-_torqueRandomness, _torqueRandomness),
                Random.Range(-_torqueRandomness, _torqueRandomness),
                Random.Range(-_torqueRandomness, _torqueRandomness)
            );

            // �⺻ ȸ����(Right)�� ���� ��Ʋ�� �߰�
            Vector3 mixedAngularVelocity = (camRight + randomTorque) * _flipSpeed;
            rb.angularVelocity = mixedAngularVelocity;
        }

        // �浹 ����
        Collider playerCol = GetComponentInParent<Collider>();
        Collider projCol = projectileObj.GetComponent<Collider>();
        if (playerCol != null && projCol != null) Physics.IgnoreCollision(playerCol, projCol);
    }
}
