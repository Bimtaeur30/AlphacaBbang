using System.Collections;
using UnityEngine;

public class CanvasFade : MonoBehaviour, IFadeConversion
{
    [SerializeField] CanvasGroup canvasGroup;

    public IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            //canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            //t += Time.deltaTime;
            yield return null;
        }
        //canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            //canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            //t += Time.deltaTime;
            yield return null;
        }
        //canvasGroup.alpha = 0f;
    }
}