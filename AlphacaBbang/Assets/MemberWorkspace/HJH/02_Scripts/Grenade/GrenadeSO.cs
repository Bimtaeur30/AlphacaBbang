using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeSO", menuName = "H_SO/GrenadeSO")]
public class GrenadeSO : ScriptableObject
{
    public string grenadeName;
    public GameObject prefab;
    public int count;
    public float Duration;
    public float damage;

    public float range;
}