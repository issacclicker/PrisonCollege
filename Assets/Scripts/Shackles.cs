using UnityEngine;

public class Shackles : MonoBehaviour
{
    [SerializeField] private Transform _chain;
    [SerializeField] private Transform _weight;


    private void Start()
    {
        _chain.SetParent(null);
        _weight.SetParent(null);
    }
}
