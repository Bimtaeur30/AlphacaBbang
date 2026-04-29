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
    [SerializeField] private GameObject mainTalkBox;
    [SerializeField] private List<GameObject> choiceTalkBoxes;
    
    private TypewriterByCharacter _typewriter;

    private TypewriterByCharacter[] _choiceTypewriters; 
    [SerializeField] private float typewriterSpeed = 0.1f;                
    
    private DialogueNodeSO _currentNode;
    private DialogueNodeSO _lastNode;
    private bool _isTalking;
    private bool _waitingForInput;
    private bool _isTextTyping;
    private int _choiceResult = -1;
    
    private void Awake()
    {
        _typewriter = mainTalkBox.GetComponentInChildren<TypewriterByCharacter>();
        Debug.Assert(_typewriter != null, $"{gameObject.name}: TypewriterByCharacter not found");
        
        _typewriter.SetTypewriterSpeed(typewriterSpeed);
        _typewriter.onTextShowed.AddListener(OnMainTextShowed);
        
        _choiceTypewriters = new TypewriterByCharacter[choiceTalkBoxes.Count];
        
        for (int i = 0; i < choiceTalkBoxes.Count; i++)
        {
            _choiceTypewriters[i] = choiceTalkBoxes[i].GetComponentInChildren<TypewriterByCharacter>();
            choiceTalkBoxes[i].SetActive(false);
            
            if (i < _choiceTypewriters.Length)
                _choiceTypewriters[i].SetTypewriterSpeed(0);
        }

        if (!wantTalk)
            mainTalkBox.SetActive(false);
        else
            _typewriter.ShowText(defaultText);
    }
    
    private void OnDestroy()
    {
        _typewriter.onTextShowed.RemoveListener(OnMainTextShowed);
    }

    

    //Change -> player Script
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
                _typewriter.SkipTypewriter();
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
        mainTalkBox.SetActive(true);
        _currentNode = _lastNode ?? firstDialogueNodeSO;
        while (_currentNode != null)
        {
            _lastNode = _currentNode;
            
            _isTextTyping = true;
            _typewriter.ShowText(_currentNode.Text);
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
        mainTalkBox.SetActive(false);
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
                _choiceTypewriters[i].ShowText(node.Choices[i].ChoiceText);
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
