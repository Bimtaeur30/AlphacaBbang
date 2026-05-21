using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour, IInstaller
{
    #region 데이터 구조체
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

    #endregion

    #region 인스펙터 필드

    [SerializeField] private string prefKey = "saveData";
    [SerializeField] private string savePointIdKey = "SavePointId";
    [SerializeField] private string saveFileName = "BbangSaveData.dat";
    [SerializeField] private string newGameSceneName = "02_KTJ_Main";

    [SerializeField] private bool isEncrypt = false;
    [SerializeField] private string encryptCode = "ggm_high";

    [field: SerializeField] public EventChannelSO SystemChannel { get; private set; }
    #endregion

    private List<SaveData> _unUsedData = new List<SaveData>();

    #region 라이프사이클

    private void OnApplicationQuit()
    {
        SystemChannel.RaiseEvent(SystemEvents.SaveFileEvent); // 게임 종료 시 파일저장
    }

    private void Awake()
    {
        SystemChannel.AddListener<SavePrefEvent>(HandleSavePrefEvent);
        SystemChannel.AddListener<LoadPrefEvent>(HandleLoadPrefEvent);
        SystemChannel.AddListener<SaveFileEvent>(HandleSaveFileEvent);
        SystemChannel.AddListener<StartNewGameEvent>(HandleStartNewGame);
        SystemChannel.AddListener<LoadFileEvent>(HandleLoadFileEvent);
        SceneManager.sceneLoaded += OnSceneLoaded;

        SystemChannel.RaiseEvent(SystemEvents.LoadPrefEvent);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleLoadPrefEvent(default);
    }

    private void OnDestroy()
    {
        SystemChannel.RemoveListener<SavePrefEvent>(HandleSavePrefEvent);
        SystemChannel.RemoveListener<LoadPrefEvent>(HandleLoadPrefEvent);
        SystemChannel.RemoveListener<SaveFileEvent>(HandleSaveFileEvent);
        SystemChannel.RemoveListener<StartNewGameEvent>(HandleStartNewGame);
        SystemChannel.RemoveListener<LoadFileEvent>(HandleLoadFileEvent);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region 이벤트 핸들러

    private void HandleStartNewGame(StartNewGameEvent _)
    {
        DeleteSaveFile();
        PlayerPrefs.DeleteKey(prefKey);
        PlayerPrefs.DeleteKey(savePointIdKey);

        // TODO: 새 게임 씬으로 전환 (newGameSceneName)
    }

    private void HandleSavePrefEvent(SavePrefEvent _)
    {
        string saveData = GetSaveData();
        PlayerPrefs.SetString(prefKey, saveData);
        Debug.Log(saveData);
    }

    private void HandleSaveFileEvent(SaveFileEvent evt)
    {
        FileSaveData fileSaveData = new FileSaveData
        {
            PrefData = GetSaveData()
        };

        string saveContent = JsonUtility.ToJson(fileSaveData);

        if (isEncrypt)
            saveContent = Base64Process(EncryptAndDeCryptData(saveContent), encoding: true);

        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(filePath, saveContent);
        Debug.Log($"{filePath}에 세이브 파일을 저장합니다.");
    }

    private void HandleLoadPrefEvent(LoadPrefEvent _)
    {
        string dataJson = PlayerPrefs.GetString(prefKey, string.Empty);
        RestoreData(dataJson);
    }

    private void HandleLoadFileEvent(LoadFileEvent _)
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("저장된 파일이 없습니다.");
            return;
        }

        string rawData = File.ReadAllText(filePath);

        if (isEncrypt)
            rawData = EncryptAndDeCryptData(Base64Process(rawData, encoding: false));

        FileSaveData fileSaveData = JsonUtility.FromJson<FileSaveData>(rawData);

        PlayerPrefs.SetString(prefKey, fileSaveData.PrefData);

        // 실제 오브젝트에 데이터 복원
        RestoreData(fileSaveData.PrefData);

        // TODO: 저장된 씬으로 전환 (fileSaveData.SceneIndex)
    }

    #endregion

    #region 저장 로직

    private string GetSaveData()
    {
        var saveableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISaveable>();

        List<SaveData> toSaveData = saveableObjects
            .Where(s => s.SaveId != null)                  // SaveId null 방어
            .Select(s => new SaveData
            {
                Id = s.SaveId.Id,
                Data = s.GetSaveData() ?? string.Empty     // GetSaveData() null 방어
            })
            .ToList();

        toSaveData.AddRange(_unUsedData);

        return JsonUtility.ToJson(new DataCollection { dataCollection = toSaveData });
    }

    #endregion

    #region 로드 로직

    private void RestoreData(string dataJson)
    {
        var saveableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .Where(s => s.SaveId != null); //  null 방어 추가

        var collection = string.IsNullOrEmpty(dataJson)
            ? new DataCollection()
            : JsonUtility.FromJson<DataCollection>(dataJson);

        _unUsedData.Clear();

        if (collection.dataCollection == null) return;

        foreach (var saveData in collection.dataCollection)
        {
            var target = saveableObjects.FirstOrDefault(s => s.SaveId.Id == saveData.Id);

            if (target != null)
                target.RestoreData(saveData.Data);
            else
                _unUsedData.Add(saveData);
        }
    }

    #endregion

    #region 암호화 로직

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
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
        else
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }

    #endregion

    #region 유틸리티

    [ContextMenu("Clear Save File")]
    private void DeleteSaveFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    [ContextMenu("Clear Pref Data")]
    public void ClearPrefData() => PlayerPrefs.DeleteKey(prefKey);

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
    #endregion
}