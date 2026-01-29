using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class ProfessorTask : MonoBehaviour
{
    [SerializeField] private Monitor _monitor;
    [SerializeField] private Professor _professor;
    [SerializeField] private Transform _cameraSocket;
    private Click _interaction;
    private float _taskElapsed = 0;
    private bool _isTasking = false;
    public bool IsTasking => _isTasking;



    private void Awake()
    {
        _interaction = GetComponent<Click>();
        _interaction.ActionName = "프로젝트 진행";
        _interaction.ClickEvent.AddListener(OnTaskStateChanged);
        _interaction.FillAmount = 0;
        _professor.DieEvent?.AddListener(OnProfessorDied);
    }



    private void Update()
    {
        ElapseTaskTime();
        CheckMovementInputToStopTask();
        ApplyProjectProgressFill();
    }



    private void ApplyProjectProgressFill()
    {
        if (IsTasking)
        {
            _interaction.FillAmount = StageController.Instance.ProjectProgress;
        }
    }



    private void ElapseTaskTime()
    {
        if (_isTasking)
        {
            _taskElapsed += Time.deltaTime;
        }
    }



    private void CheckMovementInputToStopTask()
    {
        if (!_isTasking) return;
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        //float hRaw = Input.GetAxisRaw("Horizontal");
        //float vRaw = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
        {
            StopTask();
        }
    }



    private void OnTaskStateChanged()
    {
        if (_isTasking)
        {
            StopTask();
        }
        else
        {
            DoTask();
        }
    }



    private void OnProfessorDied()
    {
        if (!IsTasking) return;
        _isTasking = false;
        _monitor.ChangeDisplay(DisplayState.Off);
    }



    private void DoTask()
    {
        _isTasking = true;
        AttachProp(_professor.gameObject, _cameraSocket);
        _professor.SetTaskPose();
        _taskElapsed = 0;
        _monitor.ChangeDisplay(DisplayState.Working);
    }



    private void StopTask()
    {
        _interaction.FillAmount = 0;
        _isTasking = false;
        _professor.UnsetTaskPose();
        _taskElapsed = 0;
        _monitor.ChangeDisplay(DisplayState.Off);
    }



    protected virtual void AttachProp(GameObject prop, Transform targetSocket)
    {
        prop.transform.SetParent(targetSocket);
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;
    }
}
