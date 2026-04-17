using UnityEngine;
using PrimeTween;

public class PaintingMovement : MonoBehaviour
{
    public enum State
    {
        None,
        Emerging,
        Idle,
        Moving,
        Disappearing
    }

    Vector3 targetPosition;
    Vector3 startPosition;
    Tween idleTween;

    protected State state = State.None;
    protected Vector3 normal;

    protected GameManager game;

    public State GetState() { return state; }
    public Vector3 GetNormal() { return normal; }

    public void Emerge()
    {
        state = State.Emerging;

        Vector3 position = game.GetMovementArea().GetRandomPosition(out normal);

        targetPosition = position;
        startPosition = targetPosition + Vector3.up * 20.0f;
        transform.SetPositionAndRotation(startPosition, Quaternion.LookRotation(-normal));
        Tween.Position(transform, targetPosition, 1.0f, Ease.InOutCubic).OnComplete(() =>
        {
            Idle();
        });

        game.PlaySound("PAINTING_ENTER");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game = GameManager.GetManager();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (GameManager.GetManager().IsGracePeriod()) return;
        switch (state)
        {
            case State.None:
                break;
            case State.Emerging:
                break;
            case State.Idle:
                if (ShouldMove())
                {
                    Move();
                }
                break;
            case State.Moving:
                if (ShouldIdle())
                {
                    Idle();
                }
                break;
            case State.Disappearing:
                break;
        }

        if (ShouldDisappear())
        {
            Disappear();
        }
    }   

    virtual protected bool ShouldMove() { return true; }
    virtual protected void Move() { state = State.Moving; }

    virtual protected bool ShouldIdle() { return false; }
    virtual protected void Idle()
    {
        state = State.Idle;
        idleTween.Stop();
        idleTween = Tween.LocalPositionY(transform, transform.position.y + Random.Range(-1f, 2f), 1.0f, Ease.InOutSine, -1, CycleMode.Yoyo);
    }

    virtual protected bool ShouldDisappear() { return Input.GetKeyDown(KeyCode.Z); }
    virtual protected void Disappear()
    {
        state = State.Disappearing;

        idleTween.Stop();

        Vector3 position = transform.position;
        position.y += 20.0f;
        Tween.Position(transform, position, 1.0f, Ease.InOutCubic).OnComplete(() =>
        {
            state = State.None;
        });

        game.PlaySound("PAINTING_EXIT");
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(targetPosition, 1.0f);
    }
}
