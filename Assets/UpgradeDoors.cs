using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
public class UpgradeDoors : MonoBehaviour, IInteractable
{
    public enum specifics{doors, window, electricShield};
    public specifics Specifics;
    public GameObject doorObject;
    public UnityEvent LightOffEvent = new UnityEvent();

    private int health = 100;
    
    public bool isUpgraded = true;

    public string AnimName { get {
            if (Specifics == specifics.doors)
            {
                return isUpgraded ? "Punch" : "OpenDoor";
            }
            else if (Specifics == specifics.window)
            {
                return isUpgraded ? "Punch" : "JumpWindow"; 
            }
            return "Idle";
        }
        set => throw new System.NotImplementedException(); 
    }

    private bool isLight = true;

    UnityEvent UseChangeEvent { get; set; } = new UnityEvent();

    void Start()
    {
        if(Specifics == specifics.doors || Specifics == specifics.window)
        {
            if (Extension.Check(0.5f))
            {
                gameObject.GetComponent<MeshRenderer>().enabled=false; 
                isUpgraded = false;
                gameObject.layer = 6;
                UseChangeEvent?.Invoke();
            }
            else
            {
                DoUpgrade();
            }
            //DoCrash();
        // gameObject.GetComponent<MeshRenderer>().enabled=false;       
        // isUpgraded = false;
        }
        else if(Specifics == specifics.electricShield)
        {
            // DoFixLight();
        }
    }

    public int GetSpecific()
    {
        return (int)Specifics;
    }
    public void OnInteract()
    {
        Debug.Log("AAA");
        if(Specifics == specifics.doors || Specifics == specifics.window) DoUpgrade();
        else if(Specifics == specifics.electricShield) DoFixLight();
    }

    public void DoUpgrade()
    {
        gameObject.GetComponent<MeshRenderer>().enabled=true;
        isUpgraded = true;  
        gameObject.layer = 0;
        UseChangeEvent?.Invoke();
    }

    public void DoCrash()
    {
        if (isUpgraded == false)
            return;
        gameObject.GetComponent<MeshRenderer>().enabled=false; 
        isUpgraded = false;
        gameObject.layer = 6;
        UseChangeEvent?.Invoke();
        AudioSource.PlayClipAtPoint(Player.Instance.audioPlayer.woodBreakSound, transform.position, 0.6f);
    }

    void DoFixLight()
    {
        isLight = true;
        GameObject EnvCtrl = GameObject.FindWithTag("EnvCtrl");
        EnvCtrl.GetComponent<EnvironmentEventController>().EndRedLight();
        gameObject.layer = 0;
    }


    public void OffLight()
    {
        if (isLight == false)
            return;
        isLight = false;
        LightOffEvent?.Invoke();
        GameObject EnvCtrl = GameObject.FindWithTag("EnvCtrl");
        EnvCtrl.GetComponent<EnvironmentEventController>().startRedLight();
    }

    public void Use()
    {
        health -= 20;
        if (health <= 0)
        {
            DoCrash();
        }
    }


    IEnumerator DelayClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseDoor();
    }


    bool isDoorOpen = false;


    public void OpenDoor()
    {
        if (isDoorOpen)
            return;
        isDoorOpen = true;
        CancelInvoke();
        doorObject.transform.rotation = Quaternion.Euler(0, -90f, 0);
        Invoke("CloseDoor", 2);
        Debug.Log("OpenDoor");
        AudioSource.PlayClipAtPoint(Player.Instance.audioPlayer.doorOpenSound, transform.position, 0.6f);
        //DelayClose(5f);
    }


    public void CloseDoor()
    {
        isDoorOpen = false;
        doorObject.transform.rotation = Quaternion.Euler(0, 0f, 0);
    }


    void Update()
    {
        if (Specifics == specifics.electricShield && Input.GetKeyDown(KeyCode.Space))
        {
            //OffLight();
        }
    }
}
