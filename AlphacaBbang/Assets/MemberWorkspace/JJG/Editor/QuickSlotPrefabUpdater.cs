using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class QuickSlotPrefabUpdater
{
    private const string QuickSlotPrefabPath = "Assets/MemberWorkspace/JJG/03_GameModule/Prefabs/QuickSlot.prefab";

    [MenuItem("JJG/Add SelectedOverlay to QuickSlot Prefab")]
    public static void AddSelectedOverlay()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(QuickSlotPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"QuickSlot 프리팹을 찾을 수 없습니다: {QuickSlotPrefabPath}");
            return;
        }

        using (var scope = new PrefabUtility.EditPrefabContentsScope(QuickSlotPrefabPath))
        {
            GameObject root = scope.prefabContentsRoot;

            // 이미 있으면 중복 추가 방지
            Transform existing = root.transform.Find("SelectedOverlay");
            if (existing != null)
            {
                Debug.Log("[JJG] SelectedOverlay가 이미 존재합니다.");
                return;
            }

            // SelectedOverlay Image 생성 (부모 꽉 채우는 반투명 컬러 오버레이)
            GameObject overlayGO = new GameObject("SelectedOverlay");
            overlayGO.layer = 5;
            overlayGO.transform.SetParent(root.transform, false);

            RectTransform rt = overlayGO.AddComponent<RectTransform>();
            rt.anchorMin        = Vector2.zero;
            rt.anchorMax        = Vector2.one;
            rt.sizeDelta        = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;

            overlayGO.AddComponent<CanvasRenderer>();
            Image overlayImage = overlayGO.AddComponent<Image>();
            overlayImage.color        = new Color(1f, 0.85f, 0f, 0.45f); // 노란 반투명
            overlayImage.raycastTarget = false;
            overlayImage.enabled      = false; // 기본 비활성

            // ItemSlotUI에 _selectedOverlay 연결
            ItemSlotUI slotUI = root.GetComponent<ItemSlotUI>();
            if (slotUI != null)
            {
                SerializedObject so = new SerializedObject(slotUI);
                so.FindProperty("_selectedOverlay").objectReferenceValue = overlayImage;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[JJG] QuickSlot 프리팹에 SelectedOverlay 추가 완료");
    }
}
