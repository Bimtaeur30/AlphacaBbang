using System;
using MemberWorkspace.CHG._02_Scripts.TextBoxSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCTalkSystem : MonoBehaviour
{
    [SerializeField] private Collider talkingZoneCollider;
    [SerializeField] private TextSettingSO textSetting;
    [SerializeField] private TextMeshPro text;
    
    private int count = 0;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextText();
        }
    }

    public void NextText()
    {
        text.text = textSetting.Text[count];

        if (count == textSetting.Text.Count)
        {
            Debug.Log("End");
        }
        else
        {
           count++;
        }
    }
    
}
