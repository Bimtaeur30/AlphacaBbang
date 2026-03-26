using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AgentRenderer_TJ : MonoBehaviour, IModule
{
    private ModuleOwner _moduleOwner;
    public void Initialize(ModuleOwner owner)
    {
        _moduleOwner = owner as ModuleOwner;
    }
}
