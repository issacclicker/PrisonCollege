using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSpot : SingleStudentSpot
{
    [SerializeField] private ExitGate _exitGate;

    public bool CanExit => !_exitGate.IsBarricadePlaced;



    public void OpenGate()
    {
        _exitGate.Open();
    }



    public void CloseGate()
    {
        _exitGate.Close();
    }
}
