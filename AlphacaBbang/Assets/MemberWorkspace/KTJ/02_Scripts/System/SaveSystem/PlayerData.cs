using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour, IModule, ISaveable
{
    public PlayerController Player { get; private set; }
    [field: SerializeField] public int TestSaveValue { get; private set; }
    [field: SerializeField] public EventChannelSO saveChannel;
    [field: SerializeField] public EventChannelSO systemChannel;

    public void Initialize(ModuleOwner owner)
    {
        Player = owner as PlayerController;
        saveChannel.AddListener<AddTestValue>(HandleAddTestValue);
    }

    private void OnDestroy()
    {
        saveChannel.RemoveListener<AddTestValue>(HandleAddTestValue);
    }

    private void HandleAddTestValue(AddTestValue value)
    {
        TestSaveValue += value.val;
    }

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
            saveChannel.RaiseEvent(SaveEvents.AddTestValue.Init(1));
        if (Keyboard.current.yKey.wasPressedThisFrame)
            systemChannel.RaiseEvent(SystemEvents.SaveFileEvent.Init(0, 0));

    }

    #region 세이브 로직
    [Header("Save Section")]
    [field: SerializeField] public SaveIdData SaveId { get; private set; }

    [Serializable]
    public struct PlayerSaveData
    {
        public int testSaveValue;
    }

    public string GetSaveData()
    {
        PlayerSaveData saveData = new PlayerSaveData()
        {
            testSaveValue = TestSaveValue
        };
        return JsonUtility.ToJson(saveData);
    }

    public void RestoreData(string data)
    {
        var parsedData = JsonUtility.FromJson<PlayerSaveData>(data);
        TestSaveValue = parsedData.testSaveValue;
    }
    #endregion
}