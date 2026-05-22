using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefabSpriteGeneratorWindow : EditorWindow
{
    private GameObject prefabSource;
    private Vector3 cameraPosition = new Vector3(0f, 0f, -5f);
    private Vector3 cameraEuler = new Vector3(0f, 0f, 0f);
    private bool useOrthographic = true;
    private float orthographicSize = 1f;
    private float fieldOfView = 30f;
    private int spriteSize = 512;
    private Color backgroundColor = Color.clear;
    private string outputPath = "Assets/GeneratedSprites";
    private PreviewRenderUtility previewUtility;
    private GameObject previewInstance;
    private Texture2D previewTexture;

    [MenuItem("Tools/Prefab Sprite Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PrefabSpriteGeneratorWindow>("Prefab Sprite Generator");
        window.minSize = new Vector2(400, 520);
    }

    private void OnEnable()
    {
        previewUtility = new PreviewRenderUtility(true);
        previewUtility.cameraFieldOfView = fieldOfView;
        previewUtility.camera.clearFlags = CameraClearFlags.Color;
        previewUtility.camera.backgroundColor = backgroundColor;
        previewUtility.camera.nearClipPlane = 0.01f;
        previewUtility.camera.farClipPlane = 1000f;
        previewUtility.lights[0].intensity = 1.4f;
        previewUtility.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
        previewUtility.lights[1].intensity = 1f;
        previewUtility.lights[1].transform.rotation = Quaternion.Euler(340f, 218f, 0f);
        RefreshPreview();
    }

    private void OnDisable()
    {
        ClearPreviewInstance();
        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }

        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Sprite Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        prefabSource = (GameObject)EditorGUILayout.ObjectField("Prefab Source", prefabSource, typeof(GameObject), false);
        outputPath = EditorGUILayout.TextField("Output Folder", outputPath);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Browse Folder"))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Output Folder", Application.dataPath, string.Empty);
            if (!string.IsNullOrEmpty(selected) && selected.StartsWith(Application.dataPath))
            {
                outputPath = "Assets" + selected.Substring(Application.dataPath.Length);
            }
            else if (!string.IsNullOrEmpty(selected))
            {
                EditorUtility.DisplayDialog("경고", "출력 폴더는 반드시 프로젝트의 Assets 폴더 아래여야 합니다.", "확인");
            }
        }
        if (GUILayout.Button("Create Folder", GUILayout.Width(110)))
        {
            if (!AssetDatabase.IsValidFolder(outputPath))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, outputPath.Replace("Assets/", string.Empty)));
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        cameraPosition = EditorGUILayout.Vector3Field("Position", cameraPosition);
        cameraEuler = EditorGUILayout.Vector3Field("Rotation", cameraEuler);
        useOrthographic = EditorGUILayout.Toggle("Orthographic", useOrthographic);
        if (useOrthographic)
        {
            orthographicSize = EditorGUILayout.FloatField("Orthographic Size", Mathf.Max(0.01f, orthographicSize));
        }
        else
        {
            fieldOfView = EditorGUILayout.Slider("Field of View", Mathf.Clamp(fieldOfView, 1f, 179f), 1f, 179f);
        }
        spriteSize = EditorGUILayout.IntField("Sprite Size", Mathf.Clamp(spriteSize, 16, 4096));
        backgroundColor = EditorGUILayout.ColorField("Background", backgroundColor);

        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Preview"))
        {
            RefreshPreview();
        }

        GUILayout.Label("Preview", EditorStyles.boldLabel);
        Rect previewRect = GUILayoutUtility.GetRect(10, 320, GUILayout.ExpandWidth(true));
        DrawPreview(previewRect);

        EditorGUILayout.Space();

        if (prefabSource == null)
        {
            EditorGUILayout.HelpBox("Prefab Source를 먼저 지정하세요.", MessageType.Info);
        }
        else
        {
            if (GUILayout.Button("Generate Sprite"))
            {
                GenerateSprite();
            }
        }
    }

    private void RefreshPreview()
    {
        if (previewUtility == null)
            return;

        if (prefabSource == null)
        {
            ClearPreviewInstance();
            Repaint();
            return;
        }

        if (previewInstance == null || previewInstance.name != prefabSource.name)
        {
            ClearPreviewInstance();
            previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabSource);
            if (previewInstance != null)
            {
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
                foreach (Transform child in previewInstance.GetComponentsInChildren<Transform>(true))
                    child.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        if (previewInstance != null && previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = new PreviewRenderUtility(true);
            previewUtility.cameraFieldOfView = fieldOfView;
            previewUtility.camera.clearFlags = CameraClearFlags.Color;
            previewUtility.camera.backgroundColor = backgroundColor;
            previewUtility.camera.nearClipPlane = 0.01f;
            previewUtility.camera.farClipPlane = 1000f;
            previewUtility.lights[0].intensity = 1.4f;
            previewUtility.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
            previewUtility.lights[1].intensity = 1f;
            previewUtility.lights[1].transform.rotation = Quaternion.Euler(340f, 218f, 0f);
            previewUtility.AddSingleGO(previewInstance);
            Repaint();
        }
    }

    private void ClearPreviewInstance()
    {
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
    }

    private void DrawPreview(Rect rect)
    {
        if (previewUtility == null)
            return;

        if (prefabSource == null)
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            EditorGUI.LabelField(rect, "프리팹을 선택하면 미리보기가 표시됩니다.", EditorStyles.whiteLabel);
            return;
        }

        previewUtility.BeginPreview(rect, GUIStyle.none);
        previewUtility.camera.transform.position = cameraPosition;
        previewUtility.camera.transform.rotation = Quaternion.Euler(cameraEuler);
        previewUtility.camera.orthographic = useOrthographic;
        previewUtility.camera.orthographicSize = orthographicSize;
        previewUtility.camera.fieldOfView = fieldOfView;
        previewUtility.camera.backgroundColor = backgroundColor;
        previewUtility.camera.clearFlags = CameraClearFlags.Color;

        if (previewInstance != null)
        {
            previewInstance.transform.position = Vector3.zero;
            previewInstance.transform.rotation = Quaternion.identity;
        }

        previewUtility.Render();
        Texture result = previewUtility.EndPreview();
        if (result != null)
            GUI.DrawTexture(rect, result, ScaleMode.ScaleToFit, false);
    }

    private void GenerateSprite()
    {
        if (prefabSource == null)
            return;

        string defaultName = prefabSource.name + "_Sprite.png";
        string path = EditorUtility.SaveFilePanelInProject("Save Sprite", defaultName, "png", "저장할 스프라이트 파일을 선택하세요.", outputPath);
        if (string.IsNullOrEmpty(path))
            return;

        int size = Mathf.Max(16, spriteSize);
        RenderTexture renderTex = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        try
        {
            renderTex.antiAliasing = 4;
            previewUtility.BeginPreview(new Rect(0, 0, size, size), GUIStyle.none);
            previewUtility.camera.targetTexture = renderTex;
            previewUtility.camera.transform.position = cameraPosition;
            previewUtility.camera.transform.rotation = Quaternion.Euler(cameraEuler);
            previewUtility.camera.orthographic = useOrthographic;
            previewUtility.camera.orthographicSize = orthographicSize;
            previewUtility.camera.fieldOfView = fieldOfView;
            previewUtility.camera.backgroundColor = backgroundColor;
            previewUtility.camera.clearFlags = CameraClearFlags.Color;
            previewUtility.Render();
            previewUtility.camera.targetTexture = null;

            RenderTexture.active = renderTex;
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            texture.Apply();

            byte[] pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            RenderTexture.active = null;

            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
                AssetDatabase.ImportAsset(path);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaSource = TextureImporterAlphaSource.FromInput;
                    importer.mipmapEnabled = false;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.SaveAndReimport();
                }
                AssetDatabase.Refresh();
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                Selection.activeObject = sprite != null ? (Object)sprite : AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }
        finally
        {
            renderTex.Release();
        }
    }
}
