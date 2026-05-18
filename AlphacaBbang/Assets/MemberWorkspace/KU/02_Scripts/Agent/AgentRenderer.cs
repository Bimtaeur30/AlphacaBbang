using UnityEngine;

public class AgentRenderer : MonoBehaviour, IRenderer, IModule
{
    public Animator Animator { get; private set; }
    public Renderer Renderer { get; private set; }

    private Agent _owner;

    public void Initialize(ModuleOwner owner)
    {
        _owner = (Agent)owner;
        Animator = GetComponent<Animator>();
        Renderer = GetComponentInChildren<Renderer>();
    }

    public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0)
    {
        Animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
    }

    public void SetFloat(int hash, float value, float dampTime = 0, float deltaTime = 0)
    {
        Animator.SetFloat(hash, value, dampTime, deltaTime);
    }

    public void SetBool(int hash, bool value)
    {
        Animator.SetBool(hash, value);
    }
}