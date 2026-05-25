using System.Collections.Generic;
using UnityEngine;

public class AgentRenderer : MonoBehaviour, IRenderer, IModule
{
    public Animator Animator { get; private set; }
    [field: SerializeField] public Material[] Materials { get; private set; }

    private Agent _owner;

    public void Initialize(ModuleOwner owner)
    {
        _owner = (Agent)owner;
        Animator = GetComponent<Animator>();
        CollectAllMaterials();
    }

    private void CollectAllMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        List<Material> materials = new List<Material>();

        for (int i = 0; i < renderers.Length; i++)
        {
            materials.AddRange(renderers[i].materials);
        }

        Materials = materials.ToArray();
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