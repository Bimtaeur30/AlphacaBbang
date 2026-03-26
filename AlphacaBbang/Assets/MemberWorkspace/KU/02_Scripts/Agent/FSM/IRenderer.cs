using UnityEngine;

public interface IRenderer
{
    Animator Animator { get; }

    void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
}
