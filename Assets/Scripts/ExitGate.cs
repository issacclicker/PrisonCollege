using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class ExitGate : MonoBehaviour
{
    [SerializeField] private Transform _barricadeParent;
    [SerializeField] private GameObject _barricadePrefab;

    protected DamageReceiver _damageReceiver;
    protected ClickAndWait _interaction;
    protected GameObject _barricadePlaced;



    protected virtual void Awake()
    {
        _damageReceiver = GetComponent<DamageReceiver>();
        _interaction = GetComponent<ClickAndWait>();

        _interaction.ProgressCompleteEvent.AddListener(PlaceBarricade);
    }



    protected virtual void PlaceBarricade()
    {
        _interaction.SetInteractable(false);
        _barricadePlaced = Instantiate(_barricadePrefab, _barricadeParent.position, _barricadeParent.rotation, _barricadeParent);
    }



    protected virtual void BreakBarricade()
    {
        _interaction.SetInteractable(true);
        Destroy(_barricadePlaced);
        _barricadePlaced = null;
    }
}
