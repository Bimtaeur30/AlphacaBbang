using System.Collections;
using UnityEngine;

public abstract class GrenadeBehavior : MonoBehaviour
{
    [Header("Blink Settings")]
    [SerializeField] private Material blinkMaterial;
    [SerializeField] private float blinkStartTime = 1f;
    [SerializeField] private float maxBlinkSpeed = 10f;

    private Material originalMaterial;
    private Renderer grenadeRenderer;

    protected virtual void Awake()
    {
        grenadeRenderer = GetComponent<Renderer>();
        if (grenadeRenderer != null)
            originalMaterial = grenadeRenderer.material;
    }

    public IEnumerator Boom(GameObject projectile, float boomTime)
    {
        float waitTime = boomTime - blinkStartTime;
        if (waitTime > 0)
            yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(BlinkRoutine(blinkStartTime));

        OnExplode();
        Destroy(projectile);
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        float timer = 0f;
        bool isBlink = false;

        while (timer < duration)
        {
            float t = timer / duration;

            float blinkSpeed = Mathf.Lerp(1f, maxBlinkSpeed, t);
            float interval = 1f / blinkSpeed;

            isBlink = !isBlink;
            grenadeRenderer.material = isBlink ? blinkMaterial : originalMaterial;

            timer += interval;
            yield return new WaitForSeconds(interval);
        }

        grenadeRenderer.material = originalMaterial;
    }

    protected abstract void OnExplode();
}