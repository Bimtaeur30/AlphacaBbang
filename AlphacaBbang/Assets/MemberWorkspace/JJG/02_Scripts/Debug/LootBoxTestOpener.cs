using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LootBoxTestOpener : MonoBehaviour
{
    private LootBoxInteractor _lootBoxInteractor;
    private LootBoxContainer _lootBoxContainer;
    
    private void Awake()
    {
        _lootBoxInteractor = GetComponent<LootBoxInteractor>();
        _lootBoxContainer = GetComponent<LootBoxContainer>();
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            _lootBoxInteractor.StartOpen(_lootBoxContainer);
        }
    }
}