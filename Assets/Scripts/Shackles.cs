using UnityEngine;

public class Shackles : MonoBehaviour
{
    [SerializeField] private Transform _chain;
    [SerializeField] private Transform _weight;


    private void Start()
    {
        bool hasToActivate = AttributeSystem.Instance.IsStudShackle;
        if (hasToActivate == false)
        {
            Destroy(gameObject);
            return;
        }
        _chain.SetParent(null);
        _weight.SetParent(null);
    }
}
