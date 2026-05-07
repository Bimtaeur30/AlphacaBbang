using System.Collections;
using UnityEngine;

public class LootBoxInteractor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    //[SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private InventoryUI lootBoxUI;

    private bool _isOpening;

    public void StartOpen(LootBoxContainer lootBox)
    {
        if (_isOpening) return;

        StartCoroutine(OpenRoutine(lootBox));
    }

    private IEnumerator OpenRoutine(LootBoxContainer lootBox)
    {
        _isOpening = true;

        //playerController.SetMoveable(false); //이동 막기
        //playerCombat.SetAttackable(false); //공격 막기

        float timer = 0f;

        while (timer < lootBox.RequiredOpenTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        //lootBoxUI.Bind(lootBox);
        //lootBoxUI.Open();

        //playerController.SetMoveable(true);
        //playerCombat.SetAttackable(true);

        _isOpening = false;
    }
}