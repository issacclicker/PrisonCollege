using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
// using System.Numerics;

public class HUDController : MonoBehaviour
{
    
    public Image img;

    public TextMeshProUGUI complexityScore;
    private Color fillColor;

    bool canErase;

    void Start()
    {
        fillColor = img.color;
        img.color = fillColor;
    }

    
    public void UpdateInterctingUI(float t)
    {
        img.color = fillColor;
        img.fillAmount = t;
        
    }

    void Update()
    {
        complexityScore.text = "혼란 : " + ChaosSystem.chaos;
    }

}
