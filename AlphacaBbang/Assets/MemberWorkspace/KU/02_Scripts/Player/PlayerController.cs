using UnityEngine;

public class PlayerController : Agent
{
    [field: SerializeField] public PlayerInputSO playerInput { get; private set; }


    protected override void Awake()
    {
        base.Awake();
    }


}
