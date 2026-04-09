using JJH._02_Scripts.Agents;

public class Agent : ModuleOwner
{
    public ISensor Sensor { get; private set; }
    public IRenderer Renderer { get; private set; }
    public IControllerMovement Movement { get; private set; }
    public IWeapon Weapon { get; private set; }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        Sensor = GetModule<ISensor>();
        Renderer = GetModule<IRenderer>();
        Movement = GetModule<IControllerMovement>();
        Weapon = GetModule<IWeapon>();
    }
}
