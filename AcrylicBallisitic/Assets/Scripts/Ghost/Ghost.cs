using UnityEngine;
using PrimeTween;
using System.Collections;

public class Ghost : MonoBehaviour
{
    enum State
    {
        Idle,
        Emerging,
        Fusing,
        Disappearing,
        Disabled
    }

    [Header("Timing")]
    public float minAttackInterval = 8f;
    public float maxAttackInterval = 15f;
    public float fuseTime = 2.5f;

    // [Header("Area Settings")]
    // public float minSpawnDist = 0f;
    // public float maxSpawnDist = 3f;

    [SerializeField] GameObject blastIndicatorPrefab;
    [SerializeField] GameObject blastEffectPrefab;

    GameManager game;
    float attackTimer = 0f;
    float fuseTimer = 0f;
    State currentState = State.Disabled;
    Vector3 startPosition;
    Vector3 endPosition;
    GameObject indicator;
    float flashInterval = 0.35f;

    void OnDifficultyChanged(DifficultyChangedEvent evt)
    {
        if (evt.newDifficulty == DifficultyProgression.DifficultyLevel.Easy)
        {
            currentState = State.Disabled;
        }
        else if (currentState == State.Disabled)
        {
            currentState = State.Idle;
            attackTimer = Random.Range(minAttackInterval, maxAttackInterval);
        }
    }

    void Start()
    {
        game = GameManager.GetManager();
        EventManager.AddListener<DifficultyChangedEvent>(OnDifficultyChanged);
    }

    void Update()
    {
        if (GameManager.GetManager().IsGracePeriod())
        {
            if (currentState == State.Emerging || currentState == State.Fusing)
            {
                endPosition = transform.position;
                endPosition.y = startPosition.y;
                Tween.PositionAtSpeed(transform, endPosition, 2.5f, Ease.InOutCubic).OnComplete(() =>
                {
                    currentState = State.Idle;
                    attackTimer = Random.Range(minAttackInterval, maxAttackInterval);
                });
            }
            return;
        }

        flashInterval -= Time.deltaTime;
        if (indicator != null && flashInterval <= 0f)
        {
            indicator.SetActive(!indicator.activeSelf);
            flashInterval = 0.35f;
        }

        Vector3 toPlayer = game.GetPlayerPosition() - transform.position;
        toPlayer.y = 0.0f;
        if (toPlayer != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(toPlayer);
            transform.rotation = rotation;
        }
        
        switch (currentState)
        {
            case State.Idle:
                if (attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                }
                else
                {
                    // Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDist, maxSpawnDist);
                    // Vector3 targetPos = game.GetPlayerPosition() + new Vector3(randomCircle.x, 0, randomCircle.y);
                    Vector3 targetPos = game.GetPlayerPosition();
                    transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                    startPosition = transform.position;
                    endPosition = transform.position + Vector3.up * 15.0f;

                    currentState = State.Emerging;
                    Tween.Position(transform, endPosition, 2.5f, Ease.InOutCubic).OnComplete(() =>
                    {
                        currentState = State.Fusing;
                        IndicateAttack();
                        fuseTimer = fuseTime;
                    });
                    StartCoroutine(DelayEnterSound());
                }
                break;
            case State.Emerging:
                break;
            case State.Fusing:
                if (fuseTimer > 0)
                {
                    fuseTimer -= Time.deltaTime;
                }
                else
                {
                    StartCoroutine(Attack());
                    endPosition = transform.position;
                    endPosition.y = startPosition.y;
                    Tween.PositionAtSpeed(transform, endPosition, 2.5f, Ease.InOutCubic).OnComplete(() =>
                    {
                        currentState = State.Idle;
                        attackTimer = Random.Range(minAttackInterval, maxAttackInterval);
                    });
                }
                break;
            case State.Disappearing:
                break;
            case State.Disabled:
                break;
        }
    }

    void IndicateAttack()
    {
        if (blastIndicatorPrefab != null)
        {
            indicator = Instantiate(blastIndicatorPrefab, transform.position, Quaternion.identity, transform);
            flashInterval = 0.35f;
        }
    }

    IEnumerator DelayEnterSound()
    {
        yield return new WaitForSeconds(1.35f);
        game.PlaySound("GHOST_ENTER");
    }

    IEnumerator Attack()
    {
        if (indicator != null)
        {
            Destroy(indicator);
        }
        if (blastEffectPrefab != null)
        {
            Instantiate(blastEffectPrefab, transform.position, Quaternion.identity);
        }
        game.PlaySound("GHOST_ATTACK");
        currentState = State.Disappearing;

        yield return new WaitForSeconds(2.05f);
        game.PlaySound("GHOST_LEAVE");
        
    }
}

