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
    private bool flipX = false;
    private bool flipY = false;
    private Color backgroundColor = Color.clear;
    private string outputPath = "Assets/GeneratedSprites";

    private Camera previewCamera;
    private GameObject previewRoot;
    private GameObject previewInstance;
    private RenderTexture previewRenderTexture;
    
    private const int PREVIEW_LAYER = 23;

    [MenuItem("Tools/Prefab Sprite Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PrefabSpriteGeneratorWindow>("Prefab Sprite Generator");
        window.minSize = new Vector2(400, 600);
    }

    private void OnEnable()
    {
        CreatePreviewCamera();
        RefreshPreview();
    }

    private void OnDisable()
    {
        ClearPreviewInstance();
        DestroyPreviewCamera();
        if (previewRenderTexture != null)
        {
            previewRenderTexture.Release();
            previewRenderTexture = null;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Sprite Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        prefabSource = (GameObject)EditorGUILayout.ObjectField("Prefab Source", prefabSource, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck()) RefreshPreview();

        outputPath = EditorGUILayout.TextField("Output Folder", outputPath);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Browse Folder"))
        {
            string selected = EditorUtility.OpenFolderPanel("Select Output Folder", Application.dataPath, string.Empty);
            if (!string.IsNullOrEmpty(selected) && selected.StartsWith(Application.dataPath))
                outputPath = "Assets" + selected.Substring(Application.dataPath.Length);
        }
        if (GUILayout.Button("Create Folder", GUILayout.Width(110)))
        {
            if (!AssetDatabase.IsValidFolder(outputPath))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, outputPath.Replace("Assets/", "")));
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        cameraPosition = EditorGUILayout.Vector3Field("Position", cameraPosition);
        cameraEuler    = EditorGUILayout.Vector3Field("Rotation", cameraEuler);
        useOrthographic = EditorGUILayout.Toggle("Orthographic", useOrthographic);
        if (useOrthographic)
            orthographicSize = EditorGUILayout.FloatField("Orthographic Size", Mathf.Max(0.01f, orthographicSize));
        else
            fieldOfView = EditorGUILayout.Slider("Field of View", Mathf.Clamp(fieldOfView, 1f, 179f), 1f, 179f);
        spriteSize = EditorGUILayout.IntField("Sprite Size", Mathf.Clamp(spriteSize, 16, 4096));
        flipX = EditorGUILayout.Toggle("Flip X", flipX);
        flipY = EditorGUILayout.Toggle("Flip Y", flipY);
        backgroundColor = EditorGUILayout.ColorField("Background", backgroundColor);
        if (EditorGUI.EndChangeCheck()) RefreshPreview();

        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh Preview")) RefreshPreview();

        GUILayout.Label("Preview", EditorStyles.boldLabel);
        Rect previewRect = GUILayoutUtility.GetRect(10, 320, GUILayout.ExpandWidth(true));
        DrawPreview(previewRect);

        EditorGUILayout.Space();
        if (prefabSource == null)
            EditorGUILayout.HelpBox("Prefab Source를 먼저 지정하세요.", MessageType.Info);
        else if (GUILayout.Button("Generate Sprite"))
            GenerateSprite();
    }

    private void RefreshPreview()
    {
        if (previewCamera == null) CreatePreviewCamera();
        if (prefabSource == null) { ClearPreviewInstance(); Repaint(); return; }

        // 프리팹이 바뀐 경우만 인스턴스 재생성
        if (previewInstance == null || previewInstance.name != prefabSource.name)
        {
            ClearPreviewInstance();
            previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabSource);
            if (previewInstance != null)
            {
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
                previewInstance.transform.SetParent(previewRoot.transform, false);
                //SetLayerRecursively(previewInstance, PREVIEW_LAYER);
            }
        }

        ApplyCameraSettings();
        Repaint();
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
        if (prefabSource == null)
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            EditorGUI.LabelField(rect, "프리팹을 선택하면 미리보기가 표시됩니다.", EditorStyles.whiteLabel);
            return;
        }

        if (previewCamera == null) CreatePreviewCamera();
        if (previewInstance == null) RefreshPreview();

        ApplyCameraSettings();

        RenderTexture rt = GetPreviewRenderTexture((int)rect.width, (int)rect.height);
        if (rt == null) return;

        previewCamera.targetTexture = rt;
        previewCamera.Render();
        previewCamera.targetTexture = null;

        // Flip 적용
        Matrix4x4 prev = GUI.matrix;
        if (flipX || flipY)
            GUIUtility.ScaleAroundPivot(new Vector2(flipX ? -1f : 1f, flipY ? -1f : 1f), rect.center);
        GUI.DrawTexture(rect, rt, ScaleMode.ScaleToFit, true);
        GUI.matrix = prev;
    }

    private void ApplyCameraSettings()
    {
        if (previewCamera == null) return;
        previewCamera.transform.position   = cameraPosition;
        previewCamera.transform.rotation   = Quaternion.Euler(cameraEuler);
        previewCamera.orthographic         = useOrthographic;
        previewCamera.orthographicSize     = orthographicSize;
        previewCamera.fieldOfView          = fieldOfView;
        previewCamera.backgroundColor      = backgroundColor;
        previewCamera.clearFlags           = CameraClearFlags.SolidColor;
    }

    private void CreatePreviewCamera()
    {
        if (previewCamera != null) return;

        previewRoot = new GameObject("__PreviewRoot__") { hideFlags = HideFlags.HideAndDontSave };

        var camObj = new GameObject("__PreviewCamera__") { hideFlags = HideFlags.HideAndDontSave };
        camObj.transform.SetParent(previewRoot.transform, false);

        previewCamera = camObj.AddComponent<Camera>();
        previewCamera.enabled       = false; // 자동 렌더 방지, 수동으로만 Render()
        previewCamera.nearClipPlane = 0.01f;
        previewCamera.farClipPlane  = 1000f;
        previewCamera.cullingMask   = -1;
    }

    private void DestroyPreviewCamera()
    {
        if (previewCamera != null) { DestroyImmediate(previewCamera.gameObject); previewCamera = null; }
        if (previewRoot != null)   { DestroyImmediate(previewRoot); previewRoot = null; }
    }

    private RenderTexture GetPreviewRenderTexture(int width, int height)
    {
        if (width <= 0 || height <= 0) return null;
        if (previewRenderTexture != null &&
            previewRenderTexture.width == width &&
            previewRenderTexture.height == height)
            return previewRenderTexture;

        if (previewRenderTexture != null) { previewRenderTexture.Release(); previewRenderTexture = null; }

        previewRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
        previewRenderTexture.Create();
        return previewRenderTexture;
    }

    private void GenerateSprite()
    {
        if (prefabSource == null) return;

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Sprite", prefabSource.name + "_Sprite.png", "png", "저장할 파일 선택", outputPath);
        if (string.IsNullOrEmpty(path)) return;

        int size = Mathf.Max(16, spriteSize);
        RenderTexture rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };

        try
        {
            ApplyCameraSettings();
            previewCamera.targetTexture = rt;
            previewCamera.Render();
            previewCamera.targetTexture = null;

            RenderTexture.active = rt;
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            if (flipX || flipY) ApplyTextureFlip(tex);

            byte[] png = tex.EncodeToPNG();
            DestroyImmediate(tex);

            File.WriteAllBytes(path, png);
            AssetDatabase.ImportAsset(path);

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType      = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaSource      = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled    = false;
                importer.filterMode       = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        finally
        {
            rt.Release();
        }
    }

    private void ApplyTextureFlip(Texture2D texture)
    {
        int w = texture.width, h = texture.height;
        Color[] src = texture.GetPixels();
        Color[] dst = new Color[src.Length];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                dst[(flipY ? h-1-y : y) * w + (flipX ? w-1-x : x)] = src[y * w + x];
        texture.SetPixels(dst);
        texture.Apply();
    }

    private void SetLayerRecursively(GameObject go, int layer)
    {
        if (go == null || layer < 0) return;
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}