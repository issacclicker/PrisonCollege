using TMPro;
using UnityEngine;

public class WeaponPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _typeTmp;
    [SerializeField] private TextMeshProUGUI _curBulletTmp;
    [SerializeField] private TextMeshProUGUI _maxBulletTmp;



    public void ShowInfo(WeaponBase weapon)
    {
        _nameTmp.text = weapon.Name;
        _typeTmp.text = weapon.TypeName;

        Stat weaponBullet = weapon.GetComponent<Stat>();
        if (weaponBullet == null)
        {
            _curBulletTmp.text = "-";
            _maxBulletTmp.text = string.Empty;
        }
        else
        {
            _curBulletTmp.text = weaponBullet.Current.ToString();
            _maxBulletTmp.text = $"/ {weaponBullet.Max.ToString()}";
        }
    }
}
