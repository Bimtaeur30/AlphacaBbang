using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.SettingUI
{
    [Serializable]
    public struct TabButton
    {
        public Button button;
        public TextMeshProUGUI text;
    }
    
    public class SettingUITabChanger : MonoBehaviour
    {
        [SerializeField] private TabButton[] tabButtons;
        [SerializeField] private CanvasGroup[] groups;
        [SerializeField] private RectTransform curTapImgRect;
        [SerializeField] private float hideAlpha = 0.3f;

        private void Awake()
        {
            //ChangeTab(0);
        }

        public void ChangeTab(int index)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                if (i == index)
                {
                    ShowTab(tabButtons[i], groups[i]);
                    continue;
                }
                HideTab(tabButtons[i], groups[i]);
            }
        }

        private void ShowTab(TabButton tabButton, CanvasGroup group)
        {
            tabButton.button.interactable = false;
            Color color = tabButton.text.color;
            color.a = 1;
            tabButton.text.color = color;
            curTapImgRect.DOMoveX(tabButton.text.rectTransform.position.x, 0.3f);
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        
        private void HideTab(TabButton tabButton, CanvasGroup group)
        {
            tabButton.button.interactable = true;
            Color color = tabButton.text.color;
            color.a = hideAlpha;
            tabButton.text.color = color;
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}
