using System;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.SettingUI
{
    public class SettingUITabChanger : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;
        [SerializeField] private CanvasGroup[] groups;
        [SerializeField] private float hideAlpha;

        private void Awake()
        {
            ChangeTab(0);
        }

        public void ChangeTab(int index)
        {
            for (int i = 0; i <  groups.Length; i++)
            {
                if (i == index)
                {
                    ShowTab(buttons[i], groups[i]);
                    continue;
                }
                HideTab(buttons[i], groups[i]);
            }
        }

        private void ShowTab(Button button, CanvasGroup group)
        {
            button.interactable = false;
            button.transform.SetAsLastSibling();
            Color color = button.image.color;
            /*color.a = hideAlpha;
            button.image.color = color;*/
            color.a = 1;
            button.image.color = color;
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        
        private void HideTab(Button button, CanvasGroup group)
        {
            button.interactable = true;
            button.transform.SetAsFirstSibling();
            Color color = button.image.color;
            /*color.a = 255;
            button.image.color = color;*/
            color.a = hideAlpha;
            button.image.color = color;
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}
