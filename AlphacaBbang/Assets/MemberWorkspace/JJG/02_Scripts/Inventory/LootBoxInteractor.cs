using System.Collections;
using UnityEngine;

public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LootBoxOpeningUI openingUI;

    [Header("Post-Open UI")]
    [SerializeField] private GameObject inventoryUIRoot;
    [SerializeField] private LootBoxRevealUI lootBoxRevealUI;

    private bool _isOpening;

    public void StartOpen(LootBoxContainer lootBox)
    {
        if (_isOpening) return;

        StartCoroutine(OpenRoutine(lootBox));
    }

    private IEnumerator OpenRoutine(LootBoxContainer lootBox)
    {
        _isOpening = true;
        openingUI?.Show();

        float timer = 0f;
        float totalTime = lootBox.RequiredOpenTime;

        while (timer < totalTime)
        {
            timer += Time.deltaTime;
            openingUI?.SetProgress(timer, totalTime);
            yield return null;
        }

        openingUI?.Hide();

        inventoryUIRoot?.SetActive(true);
        lootBoxRevealUI?.Show(lootBox);

        _isOpening = false;
    }
}