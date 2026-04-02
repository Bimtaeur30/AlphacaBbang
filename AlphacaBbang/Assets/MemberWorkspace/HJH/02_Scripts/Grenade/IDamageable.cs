using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
    void ApplyBurn(float dps, float duration);
}