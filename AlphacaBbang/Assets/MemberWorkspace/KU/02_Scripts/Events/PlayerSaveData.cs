using JJH._02_Scripts_Systems.EventSystems;
using NUnit.Framework.Internal;
using Reflex.Core;
using System;
using UnityEngine;

public class PlayerSaveData : MonoBehaviour, ISaveable, IInstaller
{
    [field: SerializeField] public EventChannelSO playerStatChannel;
    [field: SerializeField] public EventChannelSO systemChannel;

    public float MaxHealth { get; private set; }
    public float MaxStamina { get; private set; }
    public float GaugeMaxTime { get; private set; }

    public SaveIdData SaveId {get; private set;}

    private void OnEnable()
    {
        playerStatChannel.AddListener<AddMaxHealth>(HandleAddMaxHealth);
        systemChannel.AddListener<AddMaxStamina>(HandleAddMaxStamina);
        playerStatChannel.AddListener<AddMaxAimStamina>(HandleAddMaxAimStamina);
    }
    public string GetSaveData()
    {
        Debug.Log("aaaaaa");
        return JsonUtility.ToJson(new PlayerStateSaveData()
        {
            playerMaxHpSave = MaxHealth,
            playerMaxRunStaminaSave = MaxStamina,
            playerMaxAimStaminaSave = GaugeMaxTime
        });
    }

    public void RestoreData(string data)
    {
        var parsedData = JsonUtility.FromJson<PlayerStateSaveData>(data);
        MaxHealth = parsedData.playerMaxHpSave;
        MaxStamina = parsedData.playerMaxRunStaminaSave;
        GaugeMaxTime = parsedData.playerMaxAimStaminaSave;

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
        GaugeMaxTime += evt.val;

        Debug.Log($"최대 에임 스태미나 증가 : {evt.val}");
    }


    private void OnDisable()
    {
        playerStatChannel.RemoveListener<AddMaxHealth>(HandleAddMaxHealth);
        systemChannel.RemoveListener<AddMaxStamina>(HandleAddMaxStamina);
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