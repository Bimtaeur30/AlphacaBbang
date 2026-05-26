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
    [SerializeField] private DataManager dataManager;

    private SceneType targetScene = SceneType.BASE;
    [SerializeField] private SceneChangeManager sceneChangeManager;

    //private void Start()
    //{
    //    startBtn.onClick.AddListener(HandleStartBtn);
    //    settingBtn.onClick.AddListener(HandleSettingBtn);
    //    exitBtn.onClick.AddListener(HandleExitBtn);
    //}

    public void HandleExitBtn()
    {
        Application.Quit();
    }

    public void HandleSettingBtn()
    {
    }

    public void HandleStartBtn()
    {
        sceneChangeManager.SceneLoad(targetScene);
    }

    public void HandleRemoveAllDatas()
    {
        dataManager.DeleteAllData();
    }

    private void Start()
    {

        //startBtn.onClick.AddListener(HandleStartBtn);
        //settingBtn.onClick.AddListener(HandleSettingBtn);
        //exitBtn.onClick.AddListener(HandleExitBtn);

        bool hasPlayed = dataManager.HasSaveData();
        startGameTxt.text = hasPlayed ? "이어하기" : "시작하기";
        if (hasPlayed)
            targetScene = SceneType.BASE;
        else
            targetScene = SceneType.START_SCENE;

    }
}
