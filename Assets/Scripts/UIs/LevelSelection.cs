using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    [Tooltip("인트로 화면")]
    [SerializeField] private GameObject introGroup;
    [Tooltip("레벨 선택 화면")]
    [SerializeField] private GameObject levelSelectionGroup;

    public void IntroOn()
    {
        introGroup.SetActive(true);
    }

    public void IntroOff()
    {
        introGroup.SetActive(false);
    }

    public void LevelSelectionOn()
    {
        levelSelectionGroup.SetActive(true);
    }

    public void LevelSelectionOff()
    {
        levelSelectionGroup.SetActive(false);
    }
}
