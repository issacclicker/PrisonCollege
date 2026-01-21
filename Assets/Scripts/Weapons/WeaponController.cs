using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private FirstPersonController _firstPersonController;
    [SerializeField] private WeaponPanel _weaponPanel;
    public FirstPersonController FirstPersonController => _firstPersonController;

    [Header("무기 목록 (번호순)")]
    [SerializeField] private WeaponBase[] _weapons; 
    private int _currentIdx = 0;

    [Header("스왑 속도")]
    [SerializeField] private float _swapDuration = 0.3f;

    private bool _isSwapping = false;

    public WeaponBase CurrentWeapon => _weapons[_currentIdx];
    public int WeaponCount => _weapons.Length;
    public GameObject Owner { private set; get; } 

    // void Start()
    // {
    //     // 시작 시 모든 무기 비활성화 후 1번 무기만 활성화
    //     for (int i = 0; i < _weapons.Length; i++)
    //     {
    //         _weapons[i].gameObject.SetActive(false);
    //     }
    //     Equip(0);
    // }

    // void Update()
    // {
    //     HandleInput();
    // }
    public void EquipWeapon(int startingIndex, GameObject owner)
    {
        // 시작 시 모든 무기 비활성화 후 1번 무기만 활성화
        Owner = owner;
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weapons[i].gameObject.SetActive(false);
            _weapons[i].InfoUpdateEvent.AddListener(OnWeaponInfoUpdated);
        }
        Equip(startingIndex);
    }


    public void OnWeaponInfoUpdated(WeaponBase weapon)
    {
        if (weapon != CurrentWeapon) return;
        _weaponPanel.ShowInfo(CurrentWeapon);
    }

    public bool TryAttack()
    {
        if (_isSwapping || CurrentWeapon.IsPlayingAttackAnim || CurrentWeapon.CanAttack == false) return false;
        
        CurrentWeapon.PlayAttackAnim(); // 공격 명령
        return true;
    }


    public void ChangeWeaponByWheel(bool isNext)
    {
        // 공격 중이거나 스왑 중일 때는 입력을 무시
        if (_isSwapping || (CurrentWeapon != null && CurrentWeapon.IsPlayingAttackAnim)) return;

        int nextIdx = _currentIdx;

        if (isNext)
        {
            // 다음 무기 (마지막 무기에서 올리면 첫 번째로)
            nextIdx = (_currentIdx + 1) % _weapons.Length;
        }
        else
        {
            // 이전 무기 (첫 번째 무기에서 내리면 마지막으로)
            nextIdx = (_currentIdx - 1 + _weapons.Length) % _weapons.Length;
        }

        // 계산된 인덱스로 무기 교체 실행
        ChangeWeapon(nextIdx);
    }

    

    public void ChangeWeapon(int nextIdx)
    {
        if (nextIdx == _currentIdx || _isSwapping || CurrentWeapon.IsPlayingAttackAnim) return;

        StartCoroutine(SwapRoutine(nextIdx));
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
        CurrentWeapon.PlayHolsterAnim(_swapDuration, () => holsterComplete = true);
        
        yield return new WaitUntil(() => holsterComplete);

        // 2. 인덱스 교체 및 새 무기 꺼내기
        _currentIdx = newIdx;
        _weaponPanel.ShowInfo(CurrentWeapon);
        CurrentWeapon.PlayDrawAnim(_swapDuration);

        yield return new WaitForSeconds(_swapDuration);
        _isSwapping = false;
    }

    private void Equip(int idx)
    {
        _currentIdx = idx;
        CurrentWeapon.gameObject.SetActive(true);
        // 즉시 장착은 애니메이션 없이 위치만 고정
    }
}
