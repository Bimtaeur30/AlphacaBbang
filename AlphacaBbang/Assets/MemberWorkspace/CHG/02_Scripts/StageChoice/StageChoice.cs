using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.StageChoice
{
    [Serializable]
    public struct StageChoiceContent
    {
        public string Name;
        public int SceneIndex;
        public Sprite Image;
        public SceneType SceneType;
    }
    
    public class StageChoice : MonoBehaviour
    {
        [SerializeField] private GameObject contentLayout;
        [SerializeField] private GameObject stageContentPrefab;
        [SerializeField] private List<StageChoiceContent> contents;
        
        private SceneChangeManager _sceneChangeManager;
        private CanvasGroup _canvasGroup;
        private Tween _tween;
        
        private void Awake()
        {
            _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
            Debug.Assert(_sceneChangeManager != null, "SceneChangeManager not found");
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            _tween = _canvasGroup.DOFade(0, 0);
        }

        private void Start()
        {
            foreach (StageChoiceContent content in contents)
            {
                GameObject obj = Instantiate(stageContentPrefab, contentLayout.transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = content.Name;
                obj.GetComponentsInChildren<Image>()[1].sprite = content.Image;
                obj.GetComponent<Button>().onClick.AddListener(() => _sceneChangeManager.SceneLoad(content.SceneType));
            }

            StartCoroutine(LayoutRebuilde());
        }

        private IEnumerator LayoutRebuilde()
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentLayout.GetComponent<RectTransform>());
        }

        public void ShowUI()
        {
            _tween?.Kill();
            _tween = _canvasGroup.DOFade(1, 0.5f);
        }

        public void HideUI()
        {
            _tween?.Kill();
            _canvasGroup.DOFade(0, 0.5f);
        }
        
    }
}
