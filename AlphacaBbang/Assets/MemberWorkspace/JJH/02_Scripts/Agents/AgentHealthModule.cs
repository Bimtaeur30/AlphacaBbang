using UnityEngine;

namespace MemberWorkspace.JJH._02_Scripts.Agents
{
    public class AgentHealthModule : MonoBehaviour, IModule, IHealth
    {
        private ModuleOwner _owner;



        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }

    }
}