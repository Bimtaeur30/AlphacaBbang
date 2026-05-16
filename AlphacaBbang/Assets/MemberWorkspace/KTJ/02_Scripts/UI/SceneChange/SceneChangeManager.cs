using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using Reflex.Core;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    TITLE, BASE, STAGE_1, STAGE_2, STAGE_3
}

public class SceneChangeManager : MonoBehaviour, IInstaller
{
    [SerializeField] private EventChannelSO systemChannel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float transitionDuration = 1f;

    private void Awake()
    {
        SceneEnterEffect();
    }

    public void SceneLoad(SceneType sceneType)
    {
        systemChannel.RaiseEvent(SystemEvents.SavePrefEvent);
        Action onEnd = () => SceneChange(sceneType);
        SceneExitEffect(onEnd);
    }

    private void SceneChange(SceneType sceneType)
    {
        int idx = 0;
        switch (sceneType)
        {
            case SceneType.TITLE:
            idx = 1;
            break;
            case SceneType.BASE:
            idx = 1;
            break;
            case SceneType.STAGE_1:
            idx = 1;
            break;
            case SceneType.STAGE_2:
            idx = 1;
            break;
            case SceneType.STAGE_3:
            idx = 1;
            break;
        }
        SceneManager.LoadScene(idx);
    }

    private void SceneEnterEffect()
    {
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
