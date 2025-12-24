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
        Attack2();
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

    private void Attack2()
    {
        // 1. 플레이어 위치를 기준으로 주변의 모든 적 후보군을 가져옵니다.
        Collider[] hits = Physics.OverlapSphere(mainCamera.transform.position, attackRange, studentLayer);

        // 카메라의 전방 방향에서 Y축을 제거하고 평면 벡터화 합니다.
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        foreach (Collider hit in hits)
        {
            // 2. 대상의 위치(또는 중심점)에서 Y축 차이를 제거한 방향 계산
            // hit.bounds.center를 사용하면 피벗이 발바닥에 있어도 몸통 중심을 기준으로 계산해 더 정확합니다.
            Vector3 targetPos = hit.bounds.center;
            Vector3 dirToTarget = (targetPos - mainCamera.transform.position);
            dirToTarget.y = 0; // 높이 차이를 0으로 만들어 수평 방향만 남깁니다.
            
            float distance = dirToTarget.magnitude;
            dirToTarget.Normalize();

            // 3. 예외 처리: 대상이 카메라와 거의 같은 위치에 있을 경우 (계산 오류 방지)
            if (distance < 0.1f) 
            {
                ApplyDamage(hit);
                continue;
            }

            // 4. 수평 각도 판정
            // Vector3.Angle은 두 벡터 사이의 각도를 0~180도로 반환합니다.
            float angle = Vector3.Angle(cameraForward, dirToTarget);

            // 설정한 공격 각도의 절반(왼쪽/오른쪽 범위) 안에 들어오는지 확인
            if (angle <= attackAngle * 0.5f)
            {
                ApplyDamage(hit);
                // 5. 장애물 체크 (선택 사항: 벽 뒤에 있는 적까지 때리는 것을 방지)
                // if (HasLineOfSight(hit, targetPos))
                // {
                //     ApplyDamage(hit);
                // }
            }
        }
    }

    // 데미지 적용 로직 분리
    private void ApplyDamage(Collider hit)
    {
        Student student = hit.GetComponent<Student>();
        if (student != null)
        {
            student.TakeDamage(damage);
            // 타격 성공 시 효과음을 해당 위치에 발생 (이전에 배운 내용 활용)
            // AudioSource.PlayClipAtPoint(punchSound, hit.bounds.center);
        }
    }

    // 벽 너머의 적을 방지하기 위한 함수
    private bool HasLineOfSight(Collider targetCollider, Vector3 targetPos)
    {
        RaycastHit rayHit;
        if (Physics.Linecast(mainCamera.transform.position, targetPos, out rayHit))
        {
            // 레이가 가다가 다른 물체(벽 등)에 먼저 맞았다면 false
            if (rayHit.collider != targetCollider) return false;
        }
        return true;
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

