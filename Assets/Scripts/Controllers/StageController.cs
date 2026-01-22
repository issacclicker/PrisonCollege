using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageController : SceneSingleton<StageController>
{
    [SerializeField] private TextMeshProUGUI _timerTmp;
    [SerializeField] private TextMeshProUGUI _chaosTmp;
    [SerializeField] private TextMeshProUGUI _escapeTmp;
    [SerializeField] private TextMeshProUGUI _moneyTmp;
    [SerializeField] private TextMeshProUGUI _workingTmp;
    [SerializeField] private Image _projectProgressBar;
}
