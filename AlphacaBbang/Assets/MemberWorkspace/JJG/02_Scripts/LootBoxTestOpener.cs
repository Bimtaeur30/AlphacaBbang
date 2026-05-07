using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LootBoxTestOpener : MonoBehaviour
{
    [SerializeField] private LootBoxContainer lootBox;

    private bool isOpening;

    private void Awake()
    {
        if (lootBox == null)
            lootBox = GetComponent<LootBoxContainer>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartOpen();
        }
    }

    private void StartOpen()
    {
        if (isOpening)
            return;

        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isOpening = true;

        Debug.Log($"상자 열기 시작 / 필요 시간: {lootBox.RequiredOpenTime:F2}초");

        float timer = 0f;

        while (timer < lootBox.RequiredOpenTime)
        {
            timer += Time.deltaTime;
            Debug.Log($"상자 여는 중... {timer:F1} / {lootBox.RequiredOpenTime:F1}");
            yield return null;
        }

        Debug.Log("상자 열기 완료!");

        PrintLootItems();

        isOpening = false;
    }

    private void PrintLootItems()
    {
        Debug.Log("=== 상자 아이템 목록 ===");

        for (int i = 0; i < lootBox.SlotCount; i++)
        {
            ItemSlot slot = lootBox.GetSlot(i);

            if (slot == null || slot.IsEmpty)
                continue;

            Debug.Log($"[{i}] {slot.ItemData.ItemName} x{slot.Amount} / 등급: {slot.ItemData.GradeType}");
        }
    }
}