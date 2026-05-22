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

    [MenuItem("Tools/Prefab Sprite Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<PrefabSpriteGeneratorWindow>("Prefab Sprite Generator");
        window.minSize = new Vector2(400, 520);
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

        // ... outputPath, 버튼들은 그대로 ...

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        cameraPosition = EditorGUILayout.Vector3Field("Position", cameraPosition);
        cameraEuler = EditorGUILayout.Vector3Field("Rotation", cameraEuler);
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
        if (previewCamera == null)
            CreatePreviewCamera();

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
                previewInstance.transform.SetParent(previewRoot.transform, false);
                SetLayerRecursively(previewInstance, previewLayer);
            }
        }

        previewCamera.backgroundColor = backgroundColor;
        previewCamera.clearFlags = CameraClearFlags.Color;
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
        if (previewCamera == null)
            CreatePreviewCamera();

        if (prefabSource == null)
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));
            EditorGUI.LabelField(rect, "프리팹을 선택하면 미리보기가 표시됩니다.", EditorStyles.whiteLabel);
            return;
        }

        if (previewInstance != null)
        {
            previewInstance.transform.position = Vector3.zero;
            previewInstance.transform.rotation = Quaternion.identity;
        }

        previewCamera.transform.position = cameraPosition;
        previewCamera.transform.rotation = Quaternion.Euler(cameraEuler);
        previewCamera.orthographic = useOrthographic;
        previewCamera.orthographicSize = orthographicSize;
        previewCamera.fieldOfView = fieldOfView;
        previewCamera.backgroundColor = backgroundColor;
        previewCamera.clearFlags = CameraClearFlags.Color;

        RenderTexture rt = GetPreviewRenderTexture((int)rect.width, (int)rect.height);
        previewCamera.targetTexture = rt;
        previewCamera.Render();
        previewCamera.targetTexture = null;

        Matrix4x4 previousMatrix = GUI.matrix;
        if (flipX || flipY)
        {
            Vector2 pivot = rect.center;
            GUIUtility.ScaleAroundPivot(new Vector2(flipX ? -1f : 1f, flipY ? -1f : 1f), pivot);
        }
        GUI.DrawTexture(rect, rt, ScaleMode.ScaleToFit, false);
        GUI.matrix = previousMatrix;
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
            previewCamera.targetTexture = renderTex;
            previewCamera.transform.position = cameraPosition;
            previewCamera.transform.rotation = Quaternion.Euler(cameraEuler);
            previewCamera.orthographic = useOrthographic;
            previewCamera.orthographicSize = orthographicSize;
            previewCamera.fieldOfView = fieldOfView;
            previewCamera.backgroundColor = backgroundColor;
            previewCamera.clearFlags = CameraClearFlags.Color;
            previewCamera.Render();
            previewCamera.targetTexture = null;

            RenderTexture.active = renderTex;
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            texture.Apply();

            if (flipX || flipY)
                ApplyTextureFlip(texture);

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

    private void ApplyTextureFlip(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Color[] pixels = texture.GetPixels();
        Color[] flipped = new Color[pixels.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int srcIndex = y * width + x;
                int dstX = flipX ? width - 1 - x : x;
                int dstY = flipY ? height - 1 - y : y;
                flipped[dstY * width + dstX] = pixels[srcIndex];
            }
        }

        texture.SetPixels(flipped);
        texture.Apply();
    }

    private int previewLayer => LayerMask.NameToLayer("EditorOnly");

    private void CreatePreviewCamera()
    {
        if (previewCamera != null)
            return;

        previewRoot = new GameObject("PrefabSpriteGeneratorPreviewRoot");
        previewRoot.hideFlags = HideFlags.HideAndDontSave;

        GameObject cameraObject = new GameObject("PrefabSpriteGeneratorPreviewCamera");
        cameraObject.hideFlags = HideFlags.HideAndDontSave;
        cameraObject.transform.SetParent(previewRoot.transform, false);

        previewCamera = cameraObject.AddComponent<Camera>();
        previewCamera.enabled = false;
        previewCamera.clearFlags = CameraClearFlags.Color;
        previewCamera.backgroundColor = backgroundColor;
        previewCamera.nearClipPlane = 0.01f;
        previewCamera.farClipPlane = 1000f;
        previewCamera.cullingMask = 1 << previewLayer;
    }

    private void DestroyPreviewCamera()
    {
        if (previewCamera != null)
        {
            DestroyImmediate(previewCamera.gameObject);
            previewCamera = null;
        }

        if (previewRoot != null)
        {
            DestroyImmediate(previewRoot);
            previewRoot = null;
        }
    }

    private RenderTexture GetPreviewRenderTexture(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return null;

        if (previewRenderTexture == null || previewRenderTexture.width != width || previewRenderTexture.height != height)
        {
            if (previewRenderTexture != null)
            {
                previewRenderTexture.Release();
                previewRenderTexture = null;
            }

            previewRenderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 4
            };
            previewRenderTexture.Create();
        }

        return previewRenderTexture;
    }

    private void SetLayerRecursively(GameObject root, int layer)
    {
        if (root == null || layer < 0)
            return;

        root.layer = layer;
        foreach (Transform child in root.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
