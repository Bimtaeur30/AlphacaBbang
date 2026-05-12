using System.Collections;
using UnityEngine;

public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    //[SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private InventoryUI lootBoxUI;
    [SerializeField] private LootBoxOpeningUI openingUI;

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

        //playerController.SetMoveable(false);
        //playerCombat.SetAttackable(false);

        float timer = 0f;
        float totalTime = lootBox.RequiredOpenTime;

        while (timer < totalTime)
        {
            timer += Time.deltaTime;
            openingUI?.SetProgress(timer, totalTime);
            yield return null;
        }

        openingUI?.Hide();

        //lootBoxUI.Bind(lootBox);
        //lootBoxUI.Open();

        //playerController.SetMoveable(true);
        //playerCombat.SetAttackable(true);

        _isOpening = false;
    }
}