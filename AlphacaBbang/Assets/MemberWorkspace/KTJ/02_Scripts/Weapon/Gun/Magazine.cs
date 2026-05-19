using Reflex.Attributes;
using Reflex.Core;
using System;
using System.Collections;
using System.Security.Claims;
using TMPro;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    [field: SerializeField] public int CurrentBulletCount { get; private set; } = 0;
    [field: SerializeField] public int MaxBulletCount { get; private set; } = 20;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bulletCountTxt;
    [SerializeField] private RectTransform reloadUI;
    [SerializeField] private RectTransform bulletUI;

    private Gun _gun;
    private bool _loading = false;
    private float _reloadDuration = 2f;
    private float _rotationSpeed = 360f;

    [Inject] private InventoryContainer inventoryContainer;

    public bool IsReloading
    {
        get { return _loading; }
        set { _loading = value; }
    }

    public void Initialize(Gun gun)
    {
        _gun = gun;
        _reloadDuration = gun.GunDataSO.ReloadDuration;
        MaxBulletCount = gun.GunDataSO.MagCapacity;
        Debug.Assert(_gun != null, "Gun을 가져오지 못했습니다.");

        if (reloadUI != null)
            reloadUI.gameObject.SetActive(false);

        RefreshUI();
    }

    public bool TryUseBullet()
    {
        if (_loading)
            return false;

        if (CurrentBulletCount <= 0)
            return false;

        CurrentBulletCount--;
        RefreshUI();
        return true;
    }

    public bool TryReload(Action OnReloadEnd)
    {
        if (_loading)
            return false;

        int inventoryBulletCount = inventoryContainer.GetItemCount(_gun.GunDataSO.BulletType);

        int emptySpace = MaxBulletCount - CurrentBulletCount;

        if (emptySpace <= 0)
        {
            Debug.Log("이미 탄창이 가득 찼습니다.");
            return false;
        }

        if (inventoryBulletCount <= 0)
        {
            Debug.Log("인벤토리에 총알이 없어요!");
            return false;
        }

        int reloadBulletCount = Mathf.Min(emptySpace, inventoryBulletCount);
        for (int i = 0; i < reloadBulletCount; i++)
            inventoryContainer.UseItem(emptySpace, null);

        StartCoroutine(Reload(reloadBulletCount, OnReloadEnd));
        return true;
    }

    private IEnumerator Reload(int reloadBulletCount, Action onReloadEnd)
    {
        float currentTime = 0f;

        _loading = true;

        if (bulletUI != null)
            bulletUI.gameObject.SetActive(false);

        if (reloadUI != null)
        {
            reloadUI.gameObject.SetActive(true);
            reloadUI.localRotation = Quaternion.identity;
        }

        while (currentTime < _reloadDuration)
        {
            currentTime += Time.deltaTime;

            if (reloadUI != null)
                reloadUI.Rotate(0f, 0f, -_rotationSpeed * Time.deltaTime);

            yield return null;
        }

        CurrentBulletCount += reloadBulletCount;
        CurrentBulletCount = Mathf.Clamp(CurrentBulletCount, 0, MaxBulletCount);

        if (bulletUI != null)
            bulletUI.gameObject.SetActive(true);

        if (reloadUI != null)
            reloadUI.gameObject.SetActive(false);

        _loading = false;
        RefreshUI();

        Debug.Log($"재장전 완료: {reloadBulletCount}발 장전");
        onReloadEnd?.Invoke();
    }

    private void RefreshUI()
    {
        if (bulletCountTxt != null)
            bulletCountTxt.text = $"{CurrentBulletCount}/{MaxBulletCount}";
    }
}