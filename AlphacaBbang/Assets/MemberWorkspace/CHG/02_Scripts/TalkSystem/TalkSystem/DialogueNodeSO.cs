using System;
using System.Collections.Generic;
using MemberWorkspace.CHG._02_Scripts.TalkSystem.TalkSystem;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.TextBoxSystem
{
    [Serializable]
    public class DialogueChoice
    {
        public string ChoiceText;
        public DialogueNodeSO NextNode;
    }
    
    [CreateAssetMenu(fileName = "DialogueNodeSO", menuName = "CHG/DialogueNodeSO", order = 0)]
    public class DialogueNodeSO : ScriptableObject
    {
        public DialogueNodeType DialogueNodeType = DialogueNodeType.Normal;
        [TextArea(3,6)]
        public string Text;
        
        public DialogueNodeSO NextNode;
        public List<DialogueChoice> Choices = new();
        public string QuestId;
    }
}