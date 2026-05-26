using DG.Tweening;
using Reflex.Attributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Esc : MonoBehaviour
{
    [SerializeField] private CanvasGroup CanvasGroup;
    [SerializeField] private Button RestartBtn;
    [SerializeField] private Button SettingBtn;
    [SerializeField] private Button QuitBtn;
    [SerializeField] private GameObject SettingUI;

    [SerializeField] SceneChangeManager sceneChangeManager;

    bool isActive = false;
    private void Start()
    {
        QuitBtn.onClick.AddListener(HandleQuitBtn);
        SettingBtn.onClick.AddListener(HandleSettingBtn);
        RestartBtn.onClick.AddListener(HandleRestartBtn);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isActive = true;
            CanvasGroup.gameObject.SetActive(true);
            CanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
            Time.timeScale = 0f;
        }
    }

    private void HandleQuitBtn()
    {
        Application.Quit();
    }

    private void HandleSettingBtn()
    {
        // ¼¼ÆÃ UI ¶ç¿ì±â
    }

    private void HandleRestartBtn()
    {
        isActive = false;
        CanvasGroup.gameObject.SetActive(false);
        CanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
        Time.timeScale = 1f;
    }
}
