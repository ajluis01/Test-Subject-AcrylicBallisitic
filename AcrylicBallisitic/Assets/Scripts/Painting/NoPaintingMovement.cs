using UnityEngine;
using PrimeTween;

public class NoPaintingMovement : PaintingMovement
{
    [SerializeField] float moveDuration = 11.0f;

    float moveTimer = 0.0f;

    override protected void Move()
    {
        base.Move();
        moveTimer = moveDuration;
    }

    override protected void Update()
    {
        base.Update();
        if (state == State.Moving)
        {
            moveTimer -= Time.deltaTime;
        }
    }

    protected override bool ShouldDisappear()
    {
        return state == State.Moving && moveTimer <= 0.0f;
    }
}
