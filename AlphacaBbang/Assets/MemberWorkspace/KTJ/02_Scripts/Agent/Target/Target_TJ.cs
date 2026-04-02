using UnityEngine;

public class Target_TJ : MonoBehaviour, IDamageable
{
    public float Health { get; private set; }
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Debug.Log(gameObject.name + "ÀÌ(°¡) µÚÁü.");
        }
    }

    public void ApplyBurn(float dps, float duration)
    {
    }

}
