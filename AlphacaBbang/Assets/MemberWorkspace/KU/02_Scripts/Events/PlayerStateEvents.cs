using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class PlayerStateEvents
{
    public static readonly AddMaxHealth AddMaxHealth = new AddMaxHealth();
    public static readonly AddMaxStamina AddMaxStamina = new AddMaxStamina();
    public static readonly AddMaxStamina AddMaxAimStamina = new AddMaxStamina();

}

public class AddMaxHealth : GameEvent
{
    public int val { get; private set; }
    public AddMaxHealth Init(int val)
    {
        this.val = val;
        return this;
    }
}

public class AddMaxStamina : GameEvent
{
    public int val { get; private set; }
    public AddMaxStamina Init(int val)
    {
        this.val = val;
        return this;
    }
}

public class AddMaxAimStamina : GameEvent
{
    public int val { get; private set; }
    public AddMaxAimStamina Init(int val)
    {
        this.val = val;
        return this;
    }
}