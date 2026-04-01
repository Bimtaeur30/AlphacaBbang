using System.Collections;
using UnityEngine;

public class SmokeZone : MonoBehaviour
{
    // 적이 플레이어 못보는 그런 코드
    public float SpawnSecond;
    public float duration;
    public float range;

    private void Start()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        float time = 0f;

        while (time < duration)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, range);

            yield return new WaitForSeconds(1f);
            
            time += 1f;
        }

        Destroy(gameObject);
    }
}
