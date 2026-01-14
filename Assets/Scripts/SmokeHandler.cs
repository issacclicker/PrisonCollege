using UnityEngine;

public class SmokingHandler : MonoBehaviour
{
    [Header("Sockets")]
    public Transform packHandSocket;
    public Transform lighterHandSocket;
    public Transform cigaretteHandSocket;
    public Transform cigaretteMouthSocket;

    [Header("Props")]
    public GameObject cigarettePack;  // 담배갑
    public GameObject lighter;        // 라이터
    public GameObject cigarette;      // 담배 개비

    private void Awake()
    {
        cigarettePack.SetActive(false);
        lighter.SetActive(false);
        cigarette.SetActive(false);
    }

    // 1. 담배갑 꺼내기 (주머니 위치에서 손으로)
    public void ShowPack()
    {
        cigarettePack.SetActive(true);
        AttachProp(cigarettePack, packHandSocket);
    }

    public void HidePack()
    {
        cigarettePack.SetActive(false);
    }

    // 2. 담배 한 개비 입에 물기
    public void PutCigaretteInMouth()
    {
        cigarette.SetActive(true);
        AttachProp(cigarette, cigaretteMouthSocket);
    }

    public void GrabCigarette()
    {
        cigarette.SetActive(true);
        AttachProp(cigarette, cigaretteHandSocket);
    }

    public void ReleaseCigarette()
    {
        cigarette.SetActive(false);
    }

    public void ShowLighter()
    {
        lighter.SetActive(true);
        AttachProp(lighter, lighterHandSocket);
    }

    public void HideLighter()
    {
        lighter.SetActive(false);
    }

    // 공통 부착 로직
    private void AttachProp(GameObject prop, Transform targetSocket)
    {
        prop.transform.SetParent(targetSocket);
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;
    }
}