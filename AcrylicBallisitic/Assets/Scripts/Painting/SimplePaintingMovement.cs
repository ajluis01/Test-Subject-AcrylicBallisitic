using UnityEngine;
using PrimeTween;

public class SimplePaintingMovement : PaintingMovement
{
    [SerializeField] float speed = 5.0f;
    [SerializeField] float moveDuration = 8.0f;

    Vector3 moveDirection;
    float moveTimer = 0.0f;

    override protected void Move()
    {
        base.Move();
        moveDirection = Vector3.Cross(normal, Vector3.up);
        moveTimer = moveDuration;
    }

    override protected void Update()
    {
        base.Update();
        if (state == State.Moving)
        {
            moveTimer -= Time.deltaTime;
            Vector3 predictedPosition = transform.position + moveDirection * Time.deltaTime * speed;
            if (game.GetMovementArea().IsOutOfBounds(predictedPosition))
            {
                moveDirection = -moveDirection;
            }
            Vector3 newPosition = transform.position + moveDirection * Time.deltaTime * speed;
            transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
        }
    }

    protected override bool ShouldDisappear()
    {
        return state == State.Moving && moveTimer <= 0.0f;
    }
}
