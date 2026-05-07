// MapDataEditorExtension.cs
// 위치: Assets/Editor/MapDataEditorExtension.cs
// MapData 인스펙터에 "씬 저장" / "씬 복원" 버튼 추가

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapData))]
public class MapDataEditorExtension : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(8);
        var mapData = (MapData)target;

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("💾  현재 씬 저장", GUILayout.Height(32)))
            {
                mapData.CaptureFromScene();
                AssetDatabase.SaveAssets();
            }
            GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);
            if (GUILayout.Button("📂  씬에 복원", GUILayout.Height(32)))
            {
                if (EditorUtility.DisplayDialog("복원", "현재 씬에 맵을 복원합니다.\n기존 오브젝트와 중복될 수 있습니다.", "복원", "취소"))
                    mapData.RestoreToScene();
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.HelpBox(
            $"저장된 오브젝트 수: {mapData.objects.Count}개\n" +
            "※ 프리팹은 반드시 Resources/ 폴더 내에 있어야 저장됩니다.",
            MessageType.Info);
    }
}
