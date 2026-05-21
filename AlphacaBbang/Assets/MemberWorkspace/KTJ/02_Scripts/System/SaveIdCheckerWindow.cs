using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveIdCheckerWindow : EditorWindow
{
    private Vector2 _scroll;
    private List<SaveIdInfo> _infos = new();

    private class SaveIdInfo
    {
        public SaveIdData SaveIdData;
        public int Id;
        public List<MonoBehaviour> References = new();
    }

    [MenuItem("Tools/SaveId Checker")]
    public static void Open()
    {
        GetWindow<SaveIdCheckerWindow>("SaveId Checker");
    }

    private void OnGUI()
    {
        GUILayout.Space(8);

        if (GUILayout.Button("SaveId 검사"))
        {
            CheckSaveIds();
        }

        GUILayout.Space(8);

        if (_infos == null || _infos.Count == 0)
        {
            EditorGUILayout.HelpBox("검사 결과가 없습니다.", MessageType.Info);
            return;
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        foreach (var info in _infos.OrderBy(x => x.Id))
        {
            bool duplicateId = _infos.Count(x => x.Id == info.Id) > 1;
            bool sharedReference = info.References.Count > 1;

            EditorGUILayout.BeginVertical("box");

            GUI.color = duplicateId ? Color.red : Color.white;
            EditorGUILayout.LabelField($"ID : {info.Id}", EditorStyles.boldLabel);
            GUI.color = Color.white;

            EditorGUILayout.ObjectField("SaveIdData", info.SaveIdData, typeof(SaveIdData), false);

            if (duplicateId)
            {
                EditorGUILayout.HelpBox("같은 Id를 가진 SaveIdData 에셋이 있습니다.", MessageType.Error);
            }

            if (sharedReference)
            {
                EditorGUILayout.HelpBox("이 SaveIdData를 여러 ISaveable 오브젝트가 같이 참조 중입니다.", MessageType.Warning);
            }

            EditorGUILayout.LabelField($"참조 중인 오브젝트 수 : {info.References.Count}");

            if (info.References.Count == 0)
            {
                EditorGUILayout.HelpBox("현재 열린 씬에서 참조 중인 오브젝트가 없습니다.", MessageType.Info);
            }
            else
            {
                foreach (var reference in info.References)
                {
                    EditorGUILayout.ObjectField(reference.name, reference, typeof(MonoBehaviour), true);
                }
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();
    }

    private void CheckSaveIds()
    {
        _infos.Clear();

        List<SaveIdData> allSaveIds = FindAllSaveIdAssets();
        List<ISaveable> sceneSaveables = FindSceneSaveables();

        foreach (var saveIdData in allSaveIds)
        {
            SaveIdInfo info = new SaveIdInfo
            {
                SaveIdData = saveIdData,
                Id = saveIdData.Id,
                References = sceneSaveables
                    .Where(x => x.SaveId == saveIdData)
                    .Select(x => x as MonoBehaviour)
                    .Where(x => x != null)
                    .ToList()
            };

            _infos.Add(info);
        }

        Debug.Log($"[SaveIdChecker] SaveIdData 에셋 {allSaveIds.Count}개 검사 완료");
        Debug.Log($"[SaveIdChecker] 현재 씬 ISaveable {sceneSaveables.Count}개 검사 완료");
    }

    private List<SaveIdData> FindAllSaveIdAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:SaveIdData");

        return guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<SaveIdData>)
            .Where(x => x != null)
            .ToList();
    }

    private List<ISaveable> FindSceneSaveables()
    {
        return Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .Where(x => x.SaveId != null)
            .ToList();
    }
}