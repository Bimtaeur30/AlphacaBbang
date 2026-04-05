using TMPro;
using UnityEngine;

public class Target_TJ : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float Health { get; private set; } = 100f;
    [SerializeField] private TextMeshPro healthTxt;

    private void Awake()
    {
        healthTxt.text = Health.ToString();
    }
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Debug.Log(gameObject.name + "ÀÌ(°¡) µÚÁü.");
        }

        healthTxt.text = Health.ToString();
    }

    public void ApplyBurn(float dps, float duration)
    {
    }

}
