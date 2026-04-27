using System;
using System.Collections;
using System.Collections.Generic;
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
    

    
    private TextMeshPro _DialogueMesh;
    private bool _isTalking = false;
    private DialogueNodeSO _currentNode;
    private DialogueNodeSO _lastNode;
    private bool _waitingForInput;
    private int _choiceResult = -1;
    
    private void Awake()
    {
        _DialogueMesh = GetComponentInChildren<TextMeshPro>();
        for (int i = 0; i < choiceTalkBoxes.Count; i++)
        {
            //_choiceTextMeshes[i] = choiceTalkBoxes[i].GetComponentInChildren<TextMeshPro>();
            choiceTalkBoxes[i].SetActive(false);
        }

        if (!wantTalk)
            talkBox.SetActive(false);
        else
        {
            _DialogueMesh.text = defaultText;
        }

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
            else if (_waitingForInput)
            {
                _waitingForInput = false;
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            Debug.Log($"IsTalking: {_isTalking}, WaitingForInput: {_waitingForInput}");
        
        if (_isTalking && _waitingForInput)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SetChoiceResult(0); 
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SetChoiceResult(1); 
            
        }
    }

    private IEnumerator Talk()
    { 
        _isTalking = true;
        talkBox.SetActive(true);
        _currentNode = _lastNode ?? firstDialogueNodeSO;
        Debug.Log("Talk Start");
        while (_currentNode != null)
        {
            _lastNode = _currentNode;
            
            if (_currentNode.DialogueNodeType == DialogueNodeType.Normal)
            {
                yield return StartCoroutine(ShowNormalCoroutine(_currentNode));
            }
            else if (_currentNode.DialogueNodeType == DialogueNodeType.Choice)
            {
                yield return StartCoroutine(ShowChoiceCoroutine(_currentNode));
            }
        }
        
        _isTalking = false;
        talkBox.SetActive(false);
        Debug.Log("Talk End");
    }

    private IEnumerator ShowChoiceCoroutine(DialogueNodeSO currentNode)
    {
        _DialogueMesh.text = currentNode.Text;
        
        for (int i = 0; i < choiceTalkBoxes.Count; i++)
        {
            if (i < currentNode.Choices.Count)
            {
                choiceTalkBoxes[i].SetActive(true);
                _choiceTextMeshes[i].text = $"{i + 1}. {currentNode.Choices[i].ChoiceText}";
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
        _currentNode = _currentNode.Choices[_choiceResult].NextNode;
    }

    private IEnumerator ShowNormalCoroutine(DialogueNodeSO node)
    {
        _DialogueMesh.text = node.Text;
        
        _waitingForInput = true;
        yield return new WaitUntil(() => !_waitingForInput);

        _currentNode = node.NextNode;
        
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
