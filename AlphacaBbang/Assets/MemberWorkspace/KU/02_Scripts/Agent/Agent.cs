using JJH._02_Scripts.Agents;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class Agent : ModuleOwner
{
    [field: SerializeField] public EventChannelSO AgentEventChannel;

    public ISensor Sensor { get; private set; }
    public IRenderer Renderer { get; private set; }
    public IControllerMovement Movement { get; private set; }
    public IWeapon Weapon { get; private set; }
    public IHealth HealthModule { get; private set; }
    public IAgentArmor Armor { get; private set; }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        Sensor = GetModule<ISensor>();
        Renderer = GetModule<IRenderer>();
        Movement = GetModule<IControllerMovement>();
        Weapon = GetModule<IWeapon>();
        Armor = GetModule<IAgentArmor>();
        HealthModule = GetModule<IHealth>();
    }
}
