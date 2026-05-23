using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    TITLE, BASE, STAGE_1, STAGE_2, STAGE_3
}

public class SceneChangeManager : MonoBehaviour, IInstaller
{
    public static string currentMessageTxt;
    public static string currentTipTxt;

    [SerializeField] private EventChannelSO systemChannel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageTxt;
    [SerializeField] private TextMeshProUGUI tipTxt;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private string[] tipMessages;

    private void Awake()
    {
        SceneEnterEffect();
    }

    public void SceneLoad(SceneType sceneType)
    {
        int idx = GetSceneIndex(sceneType);

        currentMessageTxt = GetSceneMessage(sceneType);
        currentTipTxt = GetRandomTip();

        messageTxt.text = currentMessageTxt;
        tipTxt.text = currentTipTxt;

        // 현재 씬 데이터 저장
        systemChannel.RaiseEvent(SystemEvents.SaveFileEvent);

        SceneExitEffect(() =>
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
            SceneManager.LoadScene(idx);
        });
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        // 새 씬의 ISaveable 오브젝트들이 생성된 뒤 로드
        systemChannel.RaiseEvent(SystemEvents.LoadFileEvent);
        Debug.Log("씬 메니저에서 로드 파일을 요청함");
    }

    private int GetSceneIndex(SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.TITLE => 0,
            SceneType.BASE => 1,
            SceneType.STAGE_1 => 2,
            SceneType.STAGE_2 => 3,
            SceneType.STAGE_3 => 4,
            _ => 0
        };
    }

    private string GetSceneMessage(SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.TITLE => "메인 타이틀로 이동중",
            SceneType.BASE => "기지로 이동중",
            SceneType.STAGE_1 => "스테이지 1로 이동중",
            SceneType.STAGE_2 => "스테이지 2로 이동중",
            SceneType.STAGE_3 => "스테이지 3로 이동중",
            _ => "씬 이동중"
        };
    }

    private string GetRandomTip()
    {
        if (tipMessages == null || tipMessages.Length == 0)
            return string.Empty;

        return tipMessages[UnityEngine.Random.Range(0, tipMessages.Length)];
    }

    private void SceneEnterEffect()
    {
        messageTxt.text = currentMessageTxt;
        tipTxt.text = currentTipTxt;

        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, transitionDuration);
    }

    private void SceneExitEffect(Action onComplete)
    {
        canvasGroup.alpha = 0f;

        canvasGroup
            .DOFade(1f, transitionDuration)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
    }
}