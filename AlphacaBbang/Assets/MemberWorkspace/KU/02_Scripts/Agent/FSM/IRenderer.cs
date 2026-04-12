using UnityEngine;

public interface IRenderer
{
    Animator Animator { get; }

    void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
    public void SetFloat(int hash, float value, float dampTime = 0, float deltaTime = 0);
    public void SetBool(int hash, bool value);
}
