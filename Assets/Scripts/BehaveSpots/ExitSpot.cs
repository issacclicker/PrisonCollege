using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpot : SingleStudentSpot
{
    [SerializeField] private ExitGate _exitGate;

    public bool CanExit => !_exitGate.IsBarricadePlaced;
    public ExitGateType GateType => _exitGate.GateType;



    public void OpenGate()
    {
        _exitGate.Open();
    }



    public void CloseGate()
    {
        _exitGate.Close();
    }
}



public enum ExitGateType
{
    None,
    Door,
    Window,
    Vent,
}
