using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Swing Positions (Local)")]
    public Vector3 startPos;
    public Vector3 hitPos;

    [Header("Swing Settings")]
    public float swingDuration = 0.1f;
    public float returnDuration = 0.08f;

    private bool isSwinging = false;
    public float swingAngle = 10;
    private Quaternion originRotation;
    private Camera mainCamera;

    [Header("SwingAudio")]
    public AudioSource audioSwing;

    void Start()
    {
        mainCamera = Camera.main;
        startPos = transform.localPosition;
        originRotation = transform.localRotation;
        //transform.localPosition = startPos;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSwinging)
        {
            audioSwing.Play();
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        isSwinging = true;

        // 앞으로 휘두르기
        yield return MoveWeapon(startPos, hitPos, swingDuration);
        Attack();
        // 원래 위치로 복귀
        yield return MoveWeapon(hitPos, startPos, returnDuration);

        isSwinging = false;
    }

    [Header("Attack Settings")]
    public float attackRange = 2f;       // 공격 사거리
    public float attackAngle = 60f;        // 공격 전방 각도
    public int damage = 20;
    public LayerMask studentLayer;         // Student 레이어


    private void Attack()
    {
        // 플레이어 위치를 기준으로 OverlapSphere
        Collider[] hits = Physics.OverlapSphere(mainCamera.transform.position, attackRange, studentLayer);

        foreach (Collider hit in hits)
        {
            // 1️⃣ 카메라에서 Student 콜라이더까지 가장 가까운 지점
            Vector3 targetPos = hit.ClosestPoint(mainCamera.transform.position);

            // 2️⃣ 방향 & 거리 계산
            Vector3 dirToTarget = (targetPos - mainCamera.transform.position).normalized;
            float distance = Vector3.Distance(mainCamera.transform.position, targetPos);

            // 3️⃣ 가까이 붙으면 각도 무시, 아니면 전방 각도 판정
            if (distance > 0.5f)
            {
                Vector3 forward = mainCamera.transform.forward;
                float angle = Vector3.Angle(forward, dirToTarget);
                if (angle > attackAngle * 0.5f)
                    continue;
            }

            // 4️⃣ Student 스크립트 가져오기
            Student student = hit.GetComponent<Student>();
            if (student != null)
            {
                student.TakeDamage(damage);
            }
        }
    }

    IEnumerator MoveWeapon(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // 🔹 이징 (빠르게 치고 천천히 멈춤)
            float eased = Mathf.Sin(t * Mathf.PI * 0.5f);

            // 🔹 위치 이동 (이징 적용)
            transform.localPosition = Vector3.Lerp(from, to, eased);

            // 🔹 회전 이동 (이징 적용)
            Vector3 originEuler = originRotation.eulerAngles;
            Quaternion swingRotation = Quaternion.Euler(
                originEuler.x + swingAngle * eased,
                originEuler.y,
                originEuler.z
            );

            transform.localRotation = swingRotation;

            yield return null;
        }


        // 마지막 위치 보정
        transform.localPosition = to;
        transform.localRotation = originRotation;
    }
}

