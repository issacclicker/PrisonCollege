using UnityEngine;

public class Fire : MonoBehaviour
{
    private Stat _burnDuration;
    private bool _isBurning = false;



    private void Awake()
    {
        _burnDuration = GetComponent<Stat>();
        _burnDuration.Initialize(true);
        _burnDuration.MaxReachEvent.AddListener(ActivateExtinguisher);
    }



    private void Update()
    {
        if (_isBurning && !_burnDuration.IsMax)
        {
            _burnDuration.Increase(Time.deltaTime);
        }
    }



    private void ActivateExtinguisher()
    {

    }



    public void Ignite()
    {
        _isBurning = true;
        _burnDuration.Initialize(true);
    }



    public void Extinguish()
    {
        _isBurning = false;
        _burnDuration.Initialize(true);
    }
}
