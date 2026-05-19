using UnityEngine;
using UnityEngine.InputSystem;

public class LootBoxTestOpener : MonoBehaviour
{
    private LootBoxContainer lootBox;
    private LootBoxInteractor interactor;

    private void Awake()
    {
        if (lootBox == null)
            lootBox = GetComponent<LootBoxContainer>();

        if (interactor == null)
            interactor = GetComponent<LootBoxInteractor>();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            interactor.StartOpen(lootBox);
    }
}