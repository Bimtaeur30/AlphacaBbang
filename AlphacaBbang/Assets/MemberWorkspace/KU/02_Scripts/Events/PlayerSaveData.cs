using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSaveData : MonoBehaviour, ISaveable, IInstaller
{
    [field: SerializeField] public EventChannelSO playerStatChannel;
    [field: SerializeField] public EventChannelSO systemChannel;
    [field: SerializeField] public SaveIdData SaveId { get; private set; }

    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    [field: SerializeField] public float MaxStamina { get; private set; } = 10f;
    [field: SerializeField] public float MaxAimStamina { get; private set; } = 5f;

    private void OnEnable()
    {
        playerStatChannel.AddListener<AddMaxHealth>(HandleAddMaxHealth);
        playerStatChannel.AddListener<AddMaxStamina>(HandleAddMaxStamina);
        playerStatChannel.AddListener<AddMaxAimStamina>(HandleAddMaxAimStamina);

        Debug.Log("MaxHealth" + MaxHealth);
        Debug.Log("MaxStamina" + MaxStamina);
        Debug.Log("MaxAimStamina" + MaxAimStamina);

    }
    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
            playerStatChannel.RaiseEvent(PlayerStateEvents.AddMaxHealth.Init(10));
        if (Keyboard.current.yKey.wasPressedThisFrame)
            playerStatChannel.RaiseEvent(PlayerStateEvents.AddMaxAimStamina.Init(1));
        if (Keyboard.current.uKey.wasPressedThisFrame)
            playerStatChannel.RaiseEvent(PlayerStateEvents.AddMaxStamina.Init(1));
        if (Keyboard.current.iKey.wasPressedThisFrame)
            systemChannel.RaiseEvent(SystemEvents.SaveFileEvent);
    }
    public string GetSaveData()
    {
        Debug.Log("aaaaaa");
        //return JsonUtility.ToJson(new PlayerStateSaveData()
        //{
        //    playerMaxHpSave = MaxHealth,
        //    playerMaxRunStaminaSave = MaxStamina,
        //    playerMaxAimStaminaSave = GaugeMaxTime
        //});
        PlayerStateSaveData saveData = new PlayerStateSaveData()
        {
            playerMaxHpSave = MaxHealth,
            playerMaxRunStaminaSave = MaxStamina,
            playerMaxAimStaminaSave = MaxAimStamina,
        };
        return JsonUtility.ToJson(saveData);
    }

    public void RestoreData(string data)
    {
        Debug.Log("aaa");
        
        var parsedData = JsonUtility.FromJson<PlayerStateSaveData>(data);
        MaxHealth = parsedData.playerMaxHpSave;
        MaxStamina = parsedData.playerMaxRunStaminaSave;
        MaxAimStamina = parsedData.playerMaxAimStaminaSave;


        Debug.Log($"Health: {parsedData.playerMaxHpSave}");
        Debug.Log($"Stamina: {parsedData.playerMaxRunStaminaSave}");
    }

    private void HandleAddMaxHealth(AddMaxHealth evt)
    {
        MaxHealth += evt.val;

        Debug.Log($"최대 체력 증가 : {evt.val}");
    }

    private void HandleAddMaxStamina(AddMaxStamina evt)
    {
        MaxStamina += evt.val;

        Debug.Log($"최대 스태미나 증가 : {evt.val}");
    }
    private void HandleAddMaxAimStamina(AddMaxAimStamina evt)
    {
        MaxAimStamina += evt.val;

        Debug.Log($"최대 에임 스태미나 증가 : {evt.val}");
    }



    private void OnDisable()
    {
        playerStatChannel.RemoveListener<AddMaxHealth>(HandleAddMaxHealth);
        playerStatChannel.RemoveListener<AddMaxStamina>(HandleAddMaxStamina);
        playerStatChannel.RemoveListener<AddMaxAimStamina>(HandleAddMaxAimStamina);

    }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
    }
}

[Serializable]
public struct PlayerStateSaveData
{
    public float playerMaxHpSave;
    public float playerMaxRunStaminaSave;
    public float playerMaxAimStaminaSave;
}