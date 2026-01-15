using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fireParticle;
    private Stat _burnDuration;
    private bool _isBurning = false;



    private void Awake()
    {
        _burnDuration = GetComponent<Stat>();
        _burnDuration.Initialize(true);
        _burnDuration.MaxReachEvent.AddListener(ActivateExtinguisher);
        FireSuppressionSystem.Instance.FireExtinguishEvent.AddListener(Extinguish);
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
        Debug.Log("ActivateExtinguisher");
        FireSuppressionSystem.Instance.StartSuppression();
    }



    public void Ignite()
    {
        _isBurning = true;
        _burnDuration.Initialize(true);
        _fireParticle.gameObject.SetActive(true);
    }



    public void Extinguish()
    {
        _isBurning = false;
        _burnDuration.Initialize(true);
        _fireParticle.gameObject.SetActive(false);
    }
}
