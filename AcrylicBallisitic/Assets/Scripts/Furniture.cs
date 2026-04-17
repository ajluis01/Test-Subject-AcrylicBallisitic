using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Furniture : MonoBehaviour
{
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    public void DoDamage()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized;
        body.AddForce(randomDirection * 5f, ForceMode.Impulse);
    }
}