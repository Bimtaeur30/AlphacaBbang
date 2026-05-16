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
        int idx = 0;
        switch (sceneType)
        {
            case SceneType.TITLE:
                currentMessageTxt = "메인 타이틀로 이동중";
                idx = 1;
                break;
            case SceneType.BASE:
                currentMessageTxt = "기지로 이동중";
                idx = 1;
                break;
            case SceneType.STAGE_1:
                currentMessageTxt = "스테이지 1로 이동중";
                idx = 1;
                break;
            case SceneType.STAGE_2:
                currentMessageTxt = "스테이지 2로 이동중";
                idx = 1;
                break;
            case SceneType.STAGE_3:
                currentMessageTxt = "스테이지 3로 이동중";
                idx = 1;
                break;
        }
        string randomTip = tipMessages[UnityEngine.Random.Range(0, tipMessages.Length - 1)];
        currentTipTxt = randomTip;

        messageTxt.text = currentMessageTxt;
        tipTxt.text = currentTipTxt;

        systemChannel.RaiseEvent(SystemEvents.SavePrefEvent);
        Action onEnd = () => SceneChange(idx);
        SceneExitEffect(onEnd);
    }

    private void SceneChange(int idx)
    {
        
        SceneManager.LoadScene(idx);
    }

    private void SceneEnterEffect()
    {
        messageTxt.text = currentMessageTxt;
        tipTxt.text = currentTipTxt;
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, transitionDuration);
    }

    private void SceneExitEffect(Action act)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, transitionDuration).OnComplete(() =>
        {
            act.Invoke();
        });
    }

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterValue(this);
    }
}
