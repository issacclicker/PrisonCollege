using TMPro;
using UnityEngine;

public class WeaponPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _typeTmp;
    [SerializeField] private TextMeshProUGUI _countTmp;



    public void ShowInfo(WeaponBase weapon)
    {
        _nameTmp.text = weapon.Name;
        _typeTmp.text = weapon.TypeName;
        ICountableWeapon countable = weapon as ICountableWeapon;
        _countTmp.text = countable != null ? countable.Amount.ToString() : "-";
    }
}
