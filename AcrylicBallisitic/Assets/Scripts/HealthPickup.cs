using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.GetManager().PlaySound("PLAYER_PICKUP_HEALTH", 20f);
            GameManager.GetManager().HealPlayer();
            Destroy(gameObject);
        }
    }
}
