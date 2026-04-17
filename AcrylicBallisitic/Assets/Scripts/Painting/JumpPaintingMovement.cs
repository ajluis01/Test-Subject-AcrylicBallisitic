using UnityEngine;
using PrimeTween;

public class JumpPaintingMovement : PaintingMovement
{
    [SerializeField] int jumpCount = 3;
    [SerializeField] float jumpSpeed = 1.5f;
    [SerializeField] float idleDuration = 2.5f;

    int jumpsDone = 0;
    int extraSpins = 0;
    float idleTimer = 0.0f;

    protected override void Idle()
    {
        base.Idle();
        idleTimer = idleDuration;
    }

    override protected bool ShouldMove()
    {
        return state == State.Idle && idleTimer <= 0.0f && jumpsDone < jumpCount;
    }

    override protected void Move()
    {
        base.Move();

        // Random jump position
        // Vector3 randomPosition = Vector3.zero;
        // Vector3 randomNormal = normal;
        // while (Vector3.Dot(randomNormal, normal) > 0.2f)
        // {
        //     randomPosition = game.GetMovementArea().GetRandomPosition(out randomNormal);
        // }

        game.PlaySound("BRONZE_ATTACK");

        Vector3 playerPosition = game.GetPlayerPosition();
        Vector3 toPlayer = playerPosition - transform.position;
        toPlayer.y = 0.0f;
        toPlayer.Normalize();
        Vector3 targetPosition = game.GetMovementArea().GetCrossingPositionFromRay(transform.position + toPlayer * 1.5f, toPlayer, out Vector3 targetNormal);

        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= 10.0f) extraSpins = 0;
        else if (distance <= 25.0f) extraSpins = 1;
        else if (distance <= 45.0f) extraSpins = 2;
        else extraSpins = 3;

        Tween.PositionAtSpeed(transform, targetPosition, jumpSpeed, Ease.InOutExpo)
        .OnUpdate(transform, (target, tween) =>
        {
            Quaternion currentRot = Quaternion.LookRotation(-normal);
            Quaternion targetRot = Quaternion.LookRotation(-targetNormal);

            float t = tween.interpolationFactor;
            Quaternion baseRot = Quaternion.Slerp(currentRot, targetRot, t);

            float rollDegrees = 360.0f * extraSpins * t;
            Quaternion roll = Quaternion.AngleAxis(rollDegrees, Vector3.up);

            // roll around the facing axis (local forward)
            transform.rotation = baseRot * roll;
        })
        .OnComplete(() =>
        {
            normal = targetNormal;
            jumpsDone++;
            Idle();
        });
        transform.rotation = Quaternion.LookRotation(-targetNormal);
        // Tween.Rotation(transform, Quaternion.LookRotation(-targetNormal), distance / jumpSpeed, Ease.InOutCubic);
    }

    override protected void Update()
    {
        base.Update();
        if (state == State.Idle)
        {
            idleTimer -= Time.deltaTime;
        }
    }

    protected override bool ShouldDisappear()
    {
        return state == State.Idle && idleTimer <= 0.0f && jumpsDone >= jumpCount;
    }

    protected override void Disappear()
    {
        base.Disappear();
        jumpsDone = 0;
    }
}
