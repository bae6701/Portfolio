using UnityEngine;

[CreateAssetMenu(fileName = "StraightMovement", menuName = "Projectile/Straight")]
public class StraightMovement : ProjectileMovement
{
    public float speed = 5f;

    public override void Init(Projectile projectile)
    {
        Vector3 direction = projectile.Direction;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float adjustedAngle = angle - 90f;

            projectile.transform.rotation = Quaternion.Euler(0, 0, adjustedAngle);
        }
    }

    public override void Move(Projectile projectile)
    {
        projectile.transform.position += projectile.Direction * speed * Time.deltaTime;
    }
}
