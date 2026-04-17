using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public enum ProjectileType { Single, Spread, Burst }

public class HorseProjectile : MonoBehaviour
{
    [Header("General Settings")]
    public ProjectileType ProjectileType = ProjectileType.Single;
    public float Speed = 40f;
    public float LifeSpan = 3f;

    [Header("Behavior Data")]
    public int burstCount = 3;
    public float burstInterval = 0.1f;
    public float spreadAngle = 15f;

    [HideInInspector] public bool isSubProjectile = false;

    private Vector3 spawnLocation;
    private Quaternion spawnRotation;
    
    float randomShake = 0f;

    void Start()
    {
        spawnLocation = transform.position;
        spawnRotation = transform.rotation;
        Destroy(gameObject, LifeSpan);
        if (!isSubProjectile)
        {
            ExecuteBehavior();
        }
        if (ProjectileType == ProjectileType.Single)
        {
            GameManager.GetManager().PlaySound("HORSE_PROJECTILE");
        }
        else
        {
            GameManager.GetManager().PlaySound("PROJECTILE");
        }

        randomShake = Random.Range(0.02f, 0.06f);
    }

    void Update()
    {
       Vector3 newPosition = transform.position + transform.forward * Speed * Time.deltaTime;
       if (ProjectileType != ProjectileType.Single)
       {
            newPosition += transform.right * Mathf.Sin(Time.time * 20f) * randomShake; // Add slight horizontal wave
       }
       transform.position = newPosition;
    }

    void ExecuteBehavior()
    {
        switch (ProjectileType)
        {
            case ProjectileType.Spread:
                //change to how many pellets entry
                SpawnCopy(-spreadAngle);
                SpawnCopy(spreadAngle);
                break;
            case ProjectileType.Burst:
                StartCoroutine(BurstRoutine());
                break;
        }
    }

    void SpawnCopy(float angleOffset)
    {
        Quaternion rotation = spawnRotation * Quaternion.Euler(0, angleOffset, 0);
        GameObject copy = Instantiate(gameObject, spawnLocation, rotation);

        copy.GetComponent<HorseProjectile>().isSubProjectile = true;
    }

    void SpawnCopyTowardsPlayer()
{
        Vector3 toPlayer = GameManager.GetManager().GetPlayerPosition() - spawnLocation;
        toPlayer.y = 0.0f;
        GameObject copy = Instantiate(gameObject, spawnLocation, Quaternion.LookRotation(toPlayer));

        copy.GetComponent<HorseProjectile>().isSubProjectile = true;
    }

    IEnumerator BurstRoutine()
    {
        for (int i = 0; i < burstCount - 1; i++) // -1 because the first one already exists
        {
            yield return new WaitForSeconds(burstInterval);
            SpawnCopyTowardsPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.GetManager().DamagePlayer();
        }
        else if (other.GetComponent<Furniture>() != null)
        {
            other.GetComponent<Furniture>().DoDamage();
        }
    }
}


