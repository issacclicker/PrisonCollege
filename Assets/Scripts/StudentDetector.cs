using UnityEngine;

public class StudentDetector : MonoBehaviour
{
    [SerializeField] private StudentInfo _studentInfo;
    [SerializeField] private float _detectionRange = 50f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _blockLayer;

    private PostStudent _currentDetectedStudent;
    private Camera _mainCam;

    private void Awake()
    {
        // 성능을 위해 메인 카메라를 미리 찾아둡니다.
        _mainCam = Camera.main;
    }

    private void Update()
    {
        DetectStudent();
    }

    private void DetectStudent()
    {
        if (_mainCam == null) return;

        RaycastHit hit;
        LayerMask combinedLayer = _targetLayer | _blockLayer;

        // [수정] 1인칭 시점: 카메라의 위치에서 카메라가 바라보는 정면(화면 중앙)으로 레이 발사
        Ray ray = new Ray(_mainCam.transform.position, _mainCam.transform.forward);

        // Physics.Raycast의 매개변수를 생성한 ray로 교체
        if (Physics.Raycast(ray, out hit, _detectionRange, combinedLayer))
        {
            // 2. 장애물에 가려졌는지 확인 (확장 메서드 IsInLayerMask 유지)
            if (hit.collider.gameObject.IsInLayerMask(_blockLayer))
            {
                ClearDetection();
                return;
            }

            // 3. 학생인지 확인
            PostStudent student = hit.collider.GetComponentInParent<PostStudent>();
            if (student != null)
            {
                if (_currentDetectedStudent != student)
                {
                    _currentDetectedStudent = student;
                    _studentInfo.Show(student);
                }
                return;
            }
        }

        // 4. 아무것도 맞지 않았거나 학생이 아닌 경우
        ClearDetection();
    }

    private void ClearDetection()
    {
        if (_currentDetectedStudent != null)
        {
            _studentInfo.Hide();
            _currentDetectedStudent = null;
        }
    }
}
