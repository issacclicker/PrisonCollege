using UnityEngine;

public class StageLayout : MonoBehaviour
{
    [SerializeField] private SimplePanel _stageSelectPanel;
    private StageSlot[] _stageSlots;
    private StageSlot _selectedStage;



    private void Awake()
    {
        _stageSlots = GetComponentsInChildren<StageSlot>(true);
        foreach (var slot in _stageSlots)
        {
            slot.MouseClickEvent.AddListener(OnSlotMouseClicked);
        }
        _stageSelectPanel.DeactivateEvent.AddListener(UnselectStage);
    }



    private void UnselectStage()
    {
        _selectedStage?.Unfocus();
        _selectedStage = null;
    }



    private void OnSlotMouseClicked(StageSlot targetSlot)
    {
        if (targetSlot == _selectedStage)
        {
            _selectedStage.Unfocus();
            _selectedStage = null;
        }
        else
        {
            _selectedStage?.Unfocus();
            _selectedStage = targetSlot;
            _selectedStage.Focus();
        }
    }
}
