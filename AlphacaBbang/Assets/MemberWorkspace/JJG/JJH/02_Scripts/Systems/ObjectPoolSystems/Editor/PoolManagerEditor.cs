using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameLib.ObjectPool.Editor
{
    public class PoolManagerEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualAsset = default;
        [SerializeField] private PoolManagerSO poolManager = default;
        [SerializeField] private VisualTreeAsset itemAsset; //아이템 UI
        private string _rootFolder = "Assets/ObjectPool";

        private Button _createBtn;
        private ScrollView _itemView;

        private List<PoolItemView> _itemList;
        private PoolItemView _currentItem;

        private UnityEditor.Editor _cachedEditor;
        private VisualElement _inspector;


        [MenuItem("Tools/PoolManager")]
        public static void ShowWindow()
        {
            PoolManagerEditor wnd = GetWindow<PoolManagerEditor>();
            wnd.titleContent = new GUIContent("PoolManager");
        }

        private string GetCurrentDirectory()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            return Path.GetDirectoryName(scriptPath);
        }

        private void InitializeRootFolder()
        {
            string dirName = GetCurrentDirectory();
            DirectoryInfo parentDirectory = Directory.GetParent(dirName);
            Debug.Assert(parentDirectory != null, $"Parent directory is null check path : {dirName}");


            string dataPath = Application.dataPath;
            _rootFolder = parentDirectory.FullName.Replace('\\', '/');  //Asset으로 시작하는 폴더명으로 변경.
            if (_rootFolder.StartsWith(dataPath))
            {
                _rootFolder = "Assets/MemberWorkspace/JJH/04_Data/02_SO/ObjectPool";
            }
        }

        public void CreateGUI()
        {
            InitializeRootFolder();
            VisualElement root = rootVisualElement;
            if (visualAsset == null)
            {
                string dirName = GetCurrentDirectory();
                visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{dirName}/PoolManagerEditor.uxml");
            }
            visualAsset.CloneTree(root);

            InitializeItems(root);
            GeneratePoolingItemUI();
        }

        private void InitializeItems(VisualElement root)
        {
            _createBtn = root.Q<Button>("CreateBtn");
            _createBtn.clicked += HandleCreateItem;
            _itemView = root.Q<ScrollView>("ItemView");

            _itemView.Clear();
            _itemList = new List<PoolItemView>();

            _inspector = root.Q<VisualElement>("InspectorView");
        }

        private void GeneratePoolingItemUI()
        {
            _itemView.Clear();
            _itemList.Clear();
            _inspector.Clear();

            if (poolManager == null)
            {
                string filePath = $"{_rootFolder}/PoolManager.asset";
                poolManager = AssetDatabase.LoadAssetAtPath<PoolManagerSO>(filePath);
                if (poolManager == null)
                {
                    Debug.LogWarning("pool manager so is not exist, create new one");
                    poolManager = ScriptableObject.CreateInstance<PoolManagerSO>();
                    AssetDatabase.CreateAsset(poolManager, filePath);
                }
            }

            if (itemAsset == null)
            {
                string dirName = GetCurrentDirectory();
                itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{dirName}/PoolItemView.uxml");
            }

            foreach (PoolItemSO item in poolManager.itemList)
            {
                TemplateContainer itemUI = itemAsset.Instantiate();
                PoolItemView itemView = new PoolItemView(itemUI, item);
                _itemView.Add(itemUI); //스크롤뷰에 넣고 리스트 관리
                _itemList.Add(itemView);

                itemView.Name = item.name;
                itemView.IsEmpty = item.prefab == null;
                itemView.IsActive = false;

                itemView.OnSelectEvent += HandleSelectionEvent;
                itemView.OnDeleteEvent += HandleDeleteEvent;
            }
        }

        private void HandleDeleteEvent(PoolItemView item)
        {
            poolManager.itemList.Remove(item.poolingItem);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.poolingItem));
            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();

            if (item == _currentItem)
            {
                _currentItem = null;
                //인스펙터 클리어도 해야함. 아직 인스펙터가 없어서 여기서 끝.
            }

            GeneratePoolingItemUI();

        }

        private void HandleCreateItem()
        {
            //타입 생성
            Guid itemGUID = Guid.NewGuid();
            PoolItemSO newItem = ScriptableObject.CreateInstance<PoolItemSO>();
            newItem.poolingName = itemGUID.ToString();

            if (Directory.Exists($"{_rootFolder}/Items") == false)
            {
                Directory.CreateDirectory($"{_rootFolder}/Items");
            }

            AssetDatabase.CreateAsset(newItem, $"{_rootFolder}/Items/{newItem.poolingName}.asset");

            poolManager.itemList.Add(newItem);

            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();

            GeneratePoolingItemUI();
        }


        private void HandleSelectionEvent(PoolItemView selectItem)
        {
            _itemList.ForEach(item => item.IsActive = false);
            selectItem.IsActive = true;
            _currentItem = selectItem;

            _inspector.Clear();
            UnityEditor.Editor.CreateCachedEditor(_currentItem.poolingItem, null, ref _cachedEditor);
            VisualElement inspectorElement = _cachedEditor.CreateInspectorGUI();

            SerializedObject serializedObject = new SerializedObject(_currentItem.poolingItem);
            inspectorElement.Bind(serializedObject);
            inspectorElement.TrackSerializedObjectValue(serializedObject, so =>
            {
                selectItem.Name = so.FindProperty("poolingName").stringValue;
                selectItem.IsEmpty = so.FindProperty("prefab").objectReferenceValue == null;
            });
            _inspector.Add(inspectorElement);
        }
    }
}
