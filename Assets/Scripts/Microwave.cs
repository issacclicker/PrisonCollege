using System;
using UnityEngine;

public class Microwave : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explosionParticle;
    [SerializeField] [Range(0f, 1f)] private float _fireInvokeThereshold;
    [SerializeField] private Transform _foodSocket;
    private Duration _operateDuration;
    private Fire _fire;

    private bool _isOperating = false;
    private FoodInfo _currentFoodInside = null;

    public bool IsOperating => _isOperating;



    private void Awake()
    {
        _explosionParticle.gameObject.SetActive(false);
        _operateDuration = GetComponent<Duration>();
        _fire = GetComponent<Fire>();
        _operateDuration.Initialize(true);
        _operateDuration.MaxReachEvent.AddListener(Quit);
    }



    private void Update()
    {
        if (_isOperating == false) return;
        _operateDuration.Increase(Time.deltaTime);
        if (_operateDuration.Ratio >= _fireInvokeThereshold && _currentFoodInside.isCauseFire)
        {
            Explode();
        }
    }



    public void PutFood(FoodInfo foodInfo)
    {
        _currentFoodInside = null;
        _currentFoodInside = new();
        _currentFoodInside.isCauseFire = foodInfo.isCauseFire;
        Quaternion initialRotation = Quaternion.Euler(-90f, 0f, 0f);
        _currentFoodInside.gameObj = Instantiate(foodInfo.gameObj, _foodSocket.position, initialRotation, _foodSocket);
        //AttachProp(_currentFoodInside.gameObj, _foodSocket);
        _currentFoodInside.gameObj.SetActive(true);
    }



    public void Operate()
    {
        if (_currentFoodInside == null) return;
        _isOperating = true;
        _operateDuration.Initialize(true);
    }



    public void Quit()
    {
        _isOperating = false;
        _currentFoodInside?.gameObj.SetActive(false);
        _currentFoodInside = null;
    }



    private void Explode()
    {
        _explosionParticle.gameObject.SetActive(true);
        _explosionParticle.Play();
        _fire.Ignite();
        Quit();
    }



    protected virtual void AttachProp(GameObject prop, Transform targetSocket)
    {
        prop.transform.SetParent(targetSocket);
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localRotation = Quaternion.identity;
    }
}



[Serializable]
public class FoodInfo
{
    public bool isCauseFire;
    public GameObject gameObj;
}