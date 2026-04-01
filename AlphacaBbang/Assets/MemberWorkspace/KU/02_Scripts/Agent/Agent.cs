using JJH._02_Scripts.Agents;

public class Agent : ModuleOwner
{
    public AgentSensor Sensor { get; private set; }
    public IRenderer Renderer { get; private set; }
    public IControllerMovement Movement { get; private set; }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        Sensor = GetModule<AgentSensor>();
        Renderer = GetModule<IRenderer>();
        Movement = GetModule<IControllerMovement>();
    }
}
