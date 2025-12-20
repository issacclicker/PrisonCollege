using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
// using System.Numerics;

public class HUDController : MonoBehaviour
{
    
    public Image img;

    bool canErase;

    void Start()
    {
        img.color = new Vector4(255,255,255,0);
    }

    
    public void UpdateInterctingUI(float t)
    {
        img.color = new Vector4(255,255,255,255);
        img.fillAmount = t;
        
    }

}
