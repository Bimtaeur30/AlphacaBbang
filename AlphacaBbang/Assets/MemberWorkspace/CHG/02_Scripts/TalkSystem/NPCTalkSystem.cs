using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using MemberWorkspace.CHG._02_Scripts.TalkSystem.TalkSystem;
using MemberWorkspace.CHG._02_Scripts.TextBoxSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCTalkSystem : MonoBehaviour
{
    
    [SerializeField] private DialogueNodeSO firstDialogueNodeSO;
    [SerializeField] private bool wantTalk;
    [SerializeField] private string defaultText;
    
    [Header("Objects")]
    [SerializeField] private GameObject talkBox;
    [SerializeField] private List<GameObject> choiceTalkBoxes;
    [SerializeField] private List<TextMeshPro> _choiceTextMeshes = new();
    
    [Header("TextAnimator")]
    [SerializeField] private TypewriterByCharacter typewriter;         
    [SerializeField] private List<TypewriterByCharacter> choiceTypewriters = new(); 
    [SerializeField] private float typewriterSpeed = 0.1f;                
    
    private TextMeshPro _DialogueMesh;
    private DialogueNodeSO _currentNode;
    private DialogueNodeSO _lastNode;
    private bool _isTalking = false;
    private bool _waitingForInput;
    private bool _isTextTyping;
    private int _choiceResult = -1;
    
    private void Awake()
    {
        _DialogueMesh = GetComponentInChildren<TextMeshPro>();
        typewriter.SetTypewriterSpeed(typewriterSpeed);
        typewriter.onTextShowed.AddListener(OnMainTextShowed);
        
        for (int i = 0; i < choiceTalkBoxes.Count; i++)
        {
            //_choiceTextMeshes[i] = choiceTalkBoxes[i].GetComponentInChildren<TextMeshPro>();
            choiceTalkBoxes[i].SetActive(false);
            
            if (i < choiceTypewriters.Count)
                choiceTypewriters[i].SetTypewriterSpeed(typewriterSpeed);
        }

        if (!wantTalk)
            talkBox.SetActive(false);
        else
            typewriter.ShowText(defaultText);
    }
    
    private void OnDestroy()
    {
        typewriter.onTextShowed.RemoveListener(OnMainTextShowed);
    }

    

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //player move stop
            if (!_isTalking)
            {
                StartCoroutine(Talk());
            }
            else if (_isTextTyping)
            {
                typewriter.SkipTypewriter();
            }
            else if (_waitingForInput)
            {
                _waitingForInput = false;
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            Debug.Log($"IsTalking: {_isTalking}, WaitingForInput: {_waitingForInput}");
        
        if (_isTalking && _waitingForInput && _choiceResult == -1 && !_isTextTyping)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SetChoiceResult(0);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) SetChoiceResult(1);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame) SetChoiceResult(2);
            
        }
    }
    
    private void OnMainTextShowed()
    {
        _isTextTyping = false;
    }
    
    private IEnumerator Talk()
    { 
        _isTalking = true;
        talkBox.SetActive(true);
        _currentNode = _lastNode ?? firstDialogueNodeSO;
        while (_currentNode != null)
        {
            _lastNode = _currentNode;
            
            _isTextTyping = true;
            typewriter.ShowText(_currentNode.Text);
            yield return new WaitUntil(() => !_isTextTyping);
            
            if (_currentNode.DialogueNodeType == DialogueNodeType.Normal)
            {
                yield return StartCoroutine(ShowNormalCoroutine(_currentNode));
            }
            else if (_currentNode.DialogueNodeType == DialogueNodeType.Choice)
            {
                yield return StartCoroutine(ShowChoiceCoroutine(_currentNode));
            }
            else if (_currentNode.DialogueNodeType == DialogueNodeType.End)
            {
                yield return StartCoroutine(ShowEndCoroutine(_currentNode));
            }
        }
        
        _isTalking = false;
        talkBox.SetActive(false);
        Debug.Log("Talk End");
    }
    private IEnumerator ShowNormalCoroutine(DialogueNodeSO node)
    {
        
        
        _waitingForInput = true;
        yield return new WaitUntil(() => !_waitingForInput);
        
        _currentNode = node.NextNode;
    }

    private IEnumerator ShowChoiceCoroutine(DialogueNodeSO node)
    {
        
        
        for (int i = 0; i < choiceTalkBoxes.Count; i++)
        {
            if (i < node.Choices.Count)
            {
                choiceTalkBoxes[i].SetActive(true);
                _choiceTextMeshes[i].text = $"{i + 1}. {node.Choices[i].ChoiceText}";
            }
            else
            {
                choiceTalkBoxes[i].SetActive(false);
            }
        }
        
        _choiceResult = -1;
        _waitingForInput = true;
        yield return new WaitUntil(() => _choiceResult != -1 && !_waitingForInput);

        foreach (GameObject obj in choiceTalkBoxes)
            obj.SetActive(false);
        _currentNode = node.Choices[_choiceResult].NextNode;
    }

    private IEnumerator ShowEndCoroutine(DialogueNodeSO node)
    {
        Debug.Log($"End: {node.DialogueNodeType}");
        _waitingForInput = true;
        yield return new WaitUntil(() => !_waitingForInput);

        _lastNode = node.NextNode ?? node;
        _currentNode = null;
    }
    
    private void SetChoiceResult(int index)
    {
        if (_choiceResult == -1)
        {
            _choiceResult = index;
            _waitingForInput = false;
        }
    }
}
