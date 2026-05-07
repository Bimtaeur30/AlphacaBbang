using JJH._02_Scripts_Systems.EventSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour, ISaveable
{
    [field: SerializeField] public int TestSaveValue { get; private set; }
    [field: SerializeField] public EventChannelSO saveChannel;
    [field: SerializeField] public EventChannelSO systemChannel;

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