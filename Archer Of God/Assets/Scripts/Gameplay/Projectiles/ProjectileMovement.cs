using UnityEngine;

public abstract class ProjectileMovement : ScriptableObject
{
    public float targetHeightAdjustment = 0f;
    public abstract void Init(Projectile projectile);
    public abstract void Move(Projectile projectile);
}
