using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LootBoxContainer))]
public class LootBoxContainerEditor : Editor
{
    private SerializedProperty lootTableProp;
    private SerializedProperty lootTablesProp;
    private SerializedProperty selectionModeProp;
    private SerializedProperty specificIndexProp;

    private void OnEnable()
    {
        lootTableProp = serializedObject.FindProperty("lootTable");
        lootTablesProp = serializedObject.FindProperty("lootTables");
        selectionModeProp = serializedObject.FindProperty("selectionMode");
        specificIndexProp = serializedObject.FindProperty("specificTableIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(selectionModeProp);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(lootTableProp, new GUIContent("Legacy Loot Table"));
        EditorGUILayout.PropertyField(lootTablesProp, true);

        LootBoxContainer.LootSelectionMode mode = (LootBoxContainer.LootSelectionMode)selectionModeProp.enumValueIndex;
        if (mode == LootBoxContainer.LootSelectionMode.SpecificIndex)
        {
            if (lootTablesProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("LootTables 리스트가 비어 있습니다. Specific Index 모드를 사용하려면 테이블을 추가하세요.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(specificIndexProp, new GUIContent("Specific Table Index"));

            int idx = specificIndexProp.intValue;
            if (lootTablesProp.arraySize > 0)
            {
                idx = Mathf.Clamp(idx, 0, lootTablesProp.arraySize - 1);
                specificIndexProp.intValue = idx;

                var element = lootTablesProp.GetArrayElementAtIndex(idx);
                EditorGUILayout.LabelField("Selected Table:", element.objectReferenceValue != null ? element.objectReferenceValue.name : "None");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
