using PrimeTween;
using UnityEngine;

public class GhostBlast : MonoBehaviour
{
    [SerializeField] float blastRadiusMultiplier = 1.25f;
    // [SerializeField] float damage = 20f;
    // [SerializeField] float flashInterval = 0.1f;

    void Start()
    {
        transform.localScale = transform.localScale / blastRadiusMultiplier;
        Tween.Scale(transform, transform.localScale * blastRadiusMultiplier, 1.0f, Ease.OutCubic).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.GetManager().DamagePlayer();
        }
        else if (other.GetComponent<Furniture>() != null)
        {
            other.GetComponent<Furniture>().DoDamage();
        }
    }
}