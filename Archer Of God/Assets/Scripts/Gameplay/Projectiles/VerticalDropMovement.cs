using UnityEngine;

[CreateAssetMenu(fileName = "VerticalDropMovement", menuName = "Projectile/VerticalDrop")]
public class VerticalDropMovement : ProjectileMovement
{
    public float dropSpeed = 20f;

    public override void Init(Projectile projectile)
    {
        projectile.transform.rotation = Quaternion.Euler(0, 0, -180f);
    }

    public override void Move(Projectile projectile)
    {
        projectile.transform.position += Vector3.down * dropSpeed * Time.deltaTime;
    }
}
