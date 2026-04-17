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

    public bool IsReloading
    {
        get { return _loading; }
        set { _loading = value; }
    }

    public void Initialize(Gun gun)
    {
        _gun = gun;
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

    public void TryReload(Action OnReloadEnd)
    {
        if (_loading)
            return;

        int inventoryBulletCount = 1000; // 나중에 실제 인벤토리 값으로 교체
        int emptySpace = MaxBulletCount - CurrentBulletCount;

        if (emptySpace <= 0)
        {
            Debug.Log("이미 탄창이 가득 찼습니다.");
            return;
        }

        if (inventoryBulletCount <= 0)
        {
            Debug.Log("인벤토리에 총알이 없어요!");
            return;
        }

        int reloadBulletCount = Mathf.Min(emptySpace, inventoryBulletCount);
        StartCoroutine(Reload(reloadBulletCount, OnReloadEnd));
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