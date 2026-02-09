using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickAndWait : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private string _actionName = "상호작용";

    private Progress _progress;
    private bool _isInteractable = true;
    private bool _isInteracting = false;
    public string InteractionPrompt => $"[토글] {_actionName}";
    public bool CanInteract => _isInteractable;
    public float UIFillRatio => _progress.Ratio;

    public UnityEvent ProgressStartEvent = new UnityEvent();
    public UnityEvent ProgressCancelEvent = new UnityEvent();
    public UnityEvent ProgressCompleteEvent = new UnityEvent();

    private AttributeModifier _attributeModifier;



    private void Awake()
    {
        _progress = GetComponent<Progress>();
        _progress.Initialize(true);
        _progress.MaxReachEvent.AddListener(() => ProgressCompleteEvent?.Invoke());

        if (gameObject.GetComponent<ExitGate>() != null)
        {
            _attributeModifier = AttributeSystem.Instance.BarricadeInstallSpeedMod;
        }
        else if (gameObject.GetComponent<FuseBox>() != null)
        {
            _attributeModifier = AttributeSystem.Instance.HackRepairSpeedMod;
        }
    }



    private void Update()
    {
        if (_isInteracting)
        {
            if (_attributeModifier != null)
            {
                _progress.Increase(Time.deltaTime * _attributeModifier.GetFinalValue(1));
            }
            else
            {
                _progress.Increase(Time.deltaTime);
            }
        }
    }



    public void SetInteractable(bool state)
    {
        _isInteractable = state;

        if (!state)
        {
            _progress.Initialize(true);
        }
    }



    public void OnInteractStart()
    {
        _isInteracting = true;
        ProgressStartEvent?.Invoke();
    }

    public void OnInteractCancel()
    {
        _isInteracting = false;
        _progress.Initialize(true);
        ProgressCancelEvent?.Invoke();
    }
}
