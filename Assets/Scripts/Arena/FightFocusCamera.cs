using UnityEngine;

public class FightFocusCamera : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float rotationSpeed = 5f;



    private void Update()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
