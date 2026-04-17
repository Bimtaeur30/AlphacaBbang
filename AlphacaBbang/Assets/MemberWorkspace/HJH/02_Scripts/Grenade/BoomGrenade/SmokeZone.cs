using System.Collections;
using UnityEngine;

public class SmokeZone : MonoBehaviour
{
    // 적이 플레이어 못보는 그런 코드
    public GrenadeSO Grenade;
    private float duration;
    private float range;
    //[SerializeField]
    private ParticleSystem smokeParticle;

    private void Start()
    {
        range = Grenade.range;
        duration = Grenade.Duration;
        
        smokeParticle = GetComponentInChildren<ParticleSystem>();

        var main = smokeParticle.main;
        main.startLifetime = duration;

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
