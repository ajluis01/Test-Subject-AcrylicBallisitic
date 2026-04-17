using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Movement Move = other.gameObject.GetComponentInParent<Movement>();
            if (Move != null)
            {
                GameManager.GetManager().PlaySound("PLAYER_PICKUP_POWER", 5f);
                Move.TriggerPowerUp();
                Destroy(gameObject);
                Debug.Log("Triggered");
            }
            else
            {
                Debug.Log("null movement");
            }
        }
    }
}