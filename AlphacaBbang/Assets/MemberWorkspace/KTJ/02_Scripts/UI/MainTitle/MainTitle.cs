using Reflex.Attributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainTitle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startGameTxt;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitBtn;
    [Inject] private DataManager dataManager;

    private SceneType targetScene = SceneType.BASE;
    [Inject] private SceneChangeManager sceneChangeManager;

    private void Awake()
    {
        startBtn.onClick.AddListener(HandleStartBtn);
        settingBtn.onClick.AddListener(HandleSettingBtn);
        exitBtn.onClick.AddListener(HandleExitBtn);
    }

    private void HandleExitBtn()
    {
        Application.Quit();
    }

    private void HandleSettingBtn()
    {
    }

    private void HandleStartBtn()
    {
        sceneChangeManager.SceneLoad(targetScene);
    }

    private void Start()
    {
        bool hasPlayed = dataManager.HasSaveData();
        startGameTxt.text = hasPlayed ? "이어하기" : "시작하기";
        if (hasPlayed)
            targetScene = SceneType.BASE;
        else
            targetScene = SceneType.START_SCENE;

    }
}
