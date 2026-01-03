using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : Stat
{
    public override void Reset()
    {
        _currentStat = 0;
    }
}
