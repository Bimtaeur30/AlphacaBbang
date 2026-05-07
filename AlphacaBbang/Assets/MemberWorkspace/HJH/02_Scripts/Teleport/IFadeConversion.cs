using System.Collections;
using UnityEngine;
public interface IFadeConversion
{
    IEnumerator FadeOut(float duration);
    IEnumerator FadeIn(float duration);
}