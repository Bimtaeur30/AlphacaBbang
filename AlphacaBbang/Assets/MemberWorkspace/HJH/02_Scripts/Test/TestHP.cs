using UnityEngine;

public class TestHP : MonoBehaviour, IDamageable
{
    public float maxHP = 100f;
    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        Debug.Log($"{gameObject.name} 데미지 받음: {damage} / 남은 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject);
    }

    public void ApplyBurn(float dps, float duration)
    {
        // 코드 버그 떴음 하...그냥 이건 나중에 고쳐 아ㅏㅏ몰라
        currentHP -= dps * duration;

        Debug.Log($"{gameObject.name} 데미지 받음: {dps * duration} / 남은 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }
}