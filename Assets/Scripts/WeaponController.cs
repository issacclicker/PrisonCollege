using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private FirstPersonController _firstPersonController;
    public FirstPersonController FirstPersonController => _firstPersonController;

    [Header("무기 목록 (번호순)")]
    [SerializeField] private WeaponAnimator[] _weapons; 
    private int _currentIdx = 0;

    [Header("스왑 속도")]
    [SerializeField] private float _swapDuration = 0.3f;

    private bool _isSwapping = false;

    void Start()
    {
        // 시작 시 모든 무기 비활성화 후 1번 무기만 활성화
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weapons[i].gameObject.SetActive(false);
        }
        Equip(0);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (_isSwapping) return;

        // 숫자 1, 2, 3... 키 입력 감지
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (_currentIdx != i) StartCoroutine(SwapRoutine(i));
            }
        }
    }

    private System.Collections.IEnumerator SwapRoutine(int newIdx)
    {
        _isSwapping = true;

        // 1. 현재 무기 넣기
        bool holsterComplete = false;
        _weapons[_currentIdx].Holster(_swapDuration, () => holsterComplete = true);
        
        yield return new WaitUntil(() => holsterComplete);

        // 2. 인덱스 교체 및 새 무기 꺼내기
        _currentIdx = newIdx;
        _weapons[_currentIdx].Draw(_swapDuration);

        yield return new WaitForSeconds(_swapDuration);
        _isSwapping = false;
    }

    private void Equip(int idx)
    {
        _currentIdx = idx;
        _weapons[_currentIdx].gameObject.SetActive(true);
        // 즉시 장착은 애니메이션 없이 위치만 고정
    }
}
