using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class DataManager : MonoBehaviour, IInstaller
{
    [Serializable]
    public struct DataCollection
    {
        public List<SaveData> dataCollection;
    }

    [Serializable]
    public struct SaveData
    {
        public int Id;
        public string Data;
    }

    [Serializable]
    public struct FileSaveData
    {
        public string PrefData;
    }

    [SerializeField] private string prefKey = "saveData";
    [SerializeField] private string savePointIdKey = "SavePointId";
    [SerializeField] private string saveFileName = "BbangSaveData.dat";
    [SerializeField] private string newGameSceneName = "02_KTJ_Main";

    [SerializeField] private bool isEncrypt = false;
    [SerializeField] private string encryptCode = "ggm_high";

    [field: SerializeField] public EventChannelSO SystemChannel { get; private set; }

    private readonly List<SaveData> _unUsedData = new();

    private void Awake()
    {
        SystemChannel.AddListener<SavePrefEvent>(HandleSavePrefEvent);
        SystemChannel.AddListener<LoadPrefEvent>(HandleLoadPrefEvent);
        SystemChannel.AddListener<SaveFileEvent>(HandleSaveFileEvent);
        SystemChannel.AddListener<StartNewGameEvent>(HandleStartNewGame);
        SystemChannel.AddListener<LoadFileEvent>(HandleLoadFileEvent);

        Debug.Log($"[DataManager] Awake / Scene DataManager 등록됨: {gameObject.name}");
    }

    private void OnDestroy()
    {
        if (SystemChannel == null) return;

        SystemChannel.RemoveListener<SavePrefEvent>(HandleSavePrefEvent);
        SystemChannel.RemoveListener<LoadPrefEvent>(HandleLoadPrefEvent);
        SystemChannel.RemoveListener<SaveFileEvent>(HandleSaveFileEvent);
        SystemChannel.RemoveListener<StartNewGameEvent>(HandleStartNewGame);
        SystemChannel.RemoveListener<LoadFileEvent>(HandleLoadFileEvent);

        Debug.Log($"[DataManager] OnDestroy / Scene DataManager 제거됨: {gameObject.name}");
    }

    private void HandleStartNewGame(StartNewGameEvent _)
    {
        DeleteSaveFile();
        PlayerPrefs.DeleteKey(prefKey);
        PlayerPrefs.DeleteKey(savePointIdKey);

        Debug.Log("[SAVE] 새 게임 시작 - 저장 데이터 삭제");
    }

    private void HandleSavePrefEvent(SavePrefEvent _)
    {
        string saveData = GetSaveData();
        PlayerPrefs.SetString(prefKey, saveData);

        Debug.Log($"[SAVE_PREF] 저장 데이터: {saveData}");
    }

    private void HandleSaveFileEvent(SaveFileEvent evt)
    {
        Debug.Log("[SAVE] SaveFileEvent 받음");

        FileSaveData fileSaveData = new FileSaveData
        {
            PrefData = GetSaveData()
        };

        string saveContent = JsonUtility.ToJson(fileSaveData);

        if (isEncrypt)
            saveContent = Base64Process(EncryptAndDeCryptData(saveContent), true);

        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        File.WriteAllText(filePath, saveContent);

        Debug.Log($"[SAVE] 파일 저장 완료: {filePath}");
        Debug.Log($"[SAVE] 저장된 원본 내용: {saveContent}");
    }

    private void HandleLoadPrefEvent(LoadPrefEvent _)
    {
        Debug.Log("[LOAD_PREF] LoadPrefEvent 받음");

        string dataJson = PlayerPrefs.GetString(prefKey, string.Empty);

        Debug.Log($"[LOAD_PREF] PlayerPrefs 데이터: {dataJson}");

        RestoreData(dataJson);
    }

    private void HandleLoadFileEvent(LoadFileEvent _)
    {
        Debug.Log("[LOAD] LoadFileEvent 받음");

        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        Debug.Log($"[LOAD] 파일 경로: {filePath}");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("[LOAD] 저장된 파일이 없습니다.");
            return;
        }

        string rawData = File.ReadAllText(filePath);

        Debug.Log($"[LOAD] 파일 원본 데이터: {rawData}");

        if (isEncrypt)
            rawData = EncryptAndDeCryptData(Base64Process(rawData, false));

        FileSaveData fileSaveData = JsonUtility.FromJson<FileSaveData>(rawData);

        Debug.Log($"[LOAD] PrefData: {fileSaveData.PrefData}");

        PlayerPrefs.SetString(prefKey, fileSaveData.PrefData);

        RestoreData(fileSaveData.PrefData);
    }

    private string GetSaveData()
    {
        var saveableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .Where(s => s.SaveId != null)
            .ToList();

        Debug.Log($"[SAVE] 현재 씬 ISaveable 개수: {saveableObjects.Count}");

        foreach (var saveable in saveableObjects)
        {
            Debug.Log($"[SAVE] 저장 대상 SaveId: {saveable.SaveId.Id}, Object: {((MonoBehaviour)saveable).name}");
        }

        List<SaveData> toSaveData = saveableObjects
            .Select(s => new SaveData
            {
                Id = s.SaveId.Id,
                Data = s.GetSaveData() ?? string.Empty
            })
            .ToList();

        toSaveData.AddRange(_unUsedData);

        string json = JsonUtility.ToJson(new DataCollection
        {
            dataCollection = toSaveData
        });

        Debug.Log($"[SAVE] 최종 저장 JSON: {json}");

        return json;
    }

    private void RestoreData(string dataJson)
    {
        Debug.Log($"[RESTORE] RestoreData 호출됨: {dataJson}");

        var saveableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .Where(s => s.SaveId != null)
            .ToList();

        Debug.Log($"[RESTORE] 현재 씬 ISaveable 개수: {saveableObjects.Count}");

        foreach (var saveable in saveableObjects)
        {
            Debug.Log($"[RESTORE] 씬에 존재하는 SaveId: {saveable.SaveId.Id}, Object: {((MonoBehaviour)saveable).name}");
        }

        var collection = string.IsNullOrEmpty(dataJson)
            ? new DataCollection()
            : JsonUtility.FromJson<DataCollection>(dataJson);

        _unUsedData.Clear();

        if (collection.dataCollection == null)
        {
            Debug.LogWarning("[RESTORE] dataCollection이 null입니다.");
            return;
        }

        Debug.Log($"[RESTORE] 저장 파일 데이터 개수: {collection.dataCollection.Count}");

        foreach (var saveData in collection.dataCollection)
        {
            Debug.Log($"[RESTORE] 로드 시도 Id: {saveData.Id}, Data: {saveData.Data}");

            var target = saveableObjects.FirstOrDefault(s => s.SaveId.Id == saveData.Id);

            if (target != null)
            {
                Debug.Log($"[RESTORE] 로드 성공 Id: {saveData.Id}, Target: {((MonoBehaviour)target).name}");
                target.RestoreData(saveData.Data);
            }
            else
            {
                Debug.LogWarning($"[RESTORE] 로드 대상 없음 Id: {saveData.Id}");
                _unUsedData.Add(saveData);
            }
        }
    }

    private string EncryptAndDeCryptData(string data)
    {
        StringBuilder sb = new StringBuilder(data.Length);

        for (int i = 0; i < data.Length; i++)
            sb.Append((char)(data[i] ^ encryptCode[i % encryptCode.Length]));

        return sb.ToString();
    }

    private string Base64Process(string data, bool encoding)
    {
        if (encoding)
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));

        return Encoding.UTF8.GetString(Convert.FromBase64String(data));
    }

    [ContextMenu("Clear Save File")]
    private void DeleteSaveFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"[SAVE] 세이브 파일 삭제: {filePath}");
        }
    }

    [ContextMenu("Clear Pref Data")]
    public void ClearPrefData()
    {
        PlayerPrefs.DeleteKey(prefKey);
        Debug.Log("[SAVE] PlayerPrefs 저장 데이터 삭제");
    }

    public bool HasSaveData()
    {
        if (PlayerPrefs.HasKey(prefKey) &&
            !string.IsNullOrEmpty(PlayerPrefs.GetString(prefKey)))
            return true;

        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        return File.Exists(filePath);
    }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
    }
}