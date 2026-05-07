// MapData.cs
// 위치: Assets/Scripts/MapData.cs
// 맵 배치 정보를 ScriptableObject로 저장/불러오기

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map Editor/Map Data")]
public class MapData : ScriptableObject
{
    [System.Serializable]
    public class PlacedObject
    {
        public string prefabPath;   // Resources 폴더 기준 경로
        public Vector3 position;
        public Quaternion rotation;
        public bool isTile;         // true = 타일, false = 프리팹
    }

    public List<PlacedObject> objects = new List<PlacedObject>();

    // ── 에디터에서 현재 씬 상태를 직렬화 ──────────────────
#if UNITY_EDITOR
    public void CaptureFromScene(string tileRootName = "_TileRoot",
                                 string prefabRootName = "_PrefabRoot")
    {
        objects.Clear();

        CaptureRoot(tileRootName,   isTile: true);
        CaptureRoot(prefabRootName, isTile: false);

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[MapData] {objects.Count}개 오브젝트 저장 완료");
    }

    private void CaptureRoot(string rootName, bool isTile)
    {
        var root = GameObject.Find(rootName);
        if (root == null) return;

        foreach (Transform child in root.transform)
        {
            var prefabSource = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
            if (prefabSource == null) continue;

            string path = UnityEditor.AssetDatabase.GetAssetPath(prefabSource);
            // Resources/ 이후 경로만 추출
            const string res = "Resources/";
            int resIdx = path.IndexOf(res);
            string resourcePath = resIdx >= 0
                ? path.Substring(resIdx + res.Length).Replace(".prefab", "")
                : path;

            objects.Add(new PlacedObject
            {
                prefabPath = resourcePath,
                position   = child.position,
                rotation   = child.rotation,
                isTile     = isTile
            });
        }
    }
#endif

    // ── 런타임에서 씬에 복원 ──────────────────────────────
    public void RestoreToScene()
    {
        var tileRoot   = GetOrCreate("_TileRoot");
        var prefabRoot = GetOrCreate("_PrefabRoot");

        foreach (var obj in objects)
        {
            var prefab = Resources.Load<GameObject>(obj.prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[MapData] 프리팹을 찾을 수 없음: {obj.prefabPath}");
                continue;
            }
            var go = Instantiate(prefab, obj.position, obj.rotation);
            go.transform.SetParent(obj.isTile ? tileRoot : prefabRoot);
        }
    }

    private Transform GetOrCreate(string name)
    {
        var existing = GameObject.Find(name);
        if (existing != null) return existing.transform;
        return new GameObject(name).transform;
    }
}
