using UnityEngine;

public class ChaosUI : MonoBehaviour
{
    [Header("Warning Popup")]
    [SerializeField] private Transform _waringParent;
    [SerializeField] private GameObject _waringPrefab;
    [SerializeField] private Vector3 _spawnPosition;
    [SerializeField] private float _velocity;
    [SerializeField] private float _duration;



    public void SpawnWarningPanel(ChaosInfo chaosInfo)
    {
        GameObject warningPanelObj = Instantiate(_waringPrefab, _waringParent);
        warningPanelObj.GetComponent<RectTransform>().anchoredPosition = _spawnPosition;
        ChaosWarning warningPanel = warningPanelObj.GetComponent<ChaosWarning>();
        warningPanel.Play(chaosInfo, _velocity, _duration);
    }
}
