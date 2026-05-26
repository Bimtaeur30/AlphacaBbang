using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.CHG._02_Scripts;
using Reflex.Attributes;
using Reflex.Core;
using System;
using System.Collections;
using System.Security.Claims;
using TMPro;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    private int currentBulletCount = 0;
    [field: SerializeField] public int CurrentBulletCount
    {
        get
        {
            return currentBulletCount;
        }
        set
        {
            currentBulletCount = value;
            _gun.WeaponHandleModule.OnCurrentBulletChanged(currentBulletCount.ToString(), MaxBulletCount.ToString());
        }
    }
    [field: SerializeField] public int MaxBulletCount { get; private set; } = 20;

    //[Header("UI")]
    //[SerializeField] private TextMeshProUGUI bulletCountTxt;
    //[SerializeField] private RectTransform reloadUI;
    //[SerializeField] private RectTransform bulletUI;
    [SerializeField] private EventChannelSO systemChannel;

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
        _reloadDuration = gun.WeaponData.ReloadDuration;
        MaxBulletCount = gun.WeaponData.MagCapacity;
        Debug.Assert(_gun != null, "Gun�� �������� ���߽��ϴ�.");

        //if (reloadUI != null)
        //    reloadUI.gameObject.SetActive(false);

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

        int inventoryBulletCount = 0;
        if (_gun.WeaponHandleModule is EnemyWeaponHandleModule)
            inventoryBulletCount = int.MaxValue;
        else
            inventoryBulletCount = (_gun.WeaponHandleModule as PlayerGunHandleModule)
                                ?.InventoryContainer
                                ?.GetItemCount(_gun.WeaponData.BulletType) ?? 0;

        int emptySpace = MaxBulletCount - CurrentBulletCount;

        if (emptySpace <= 0)
        {
            Debug.Log("�̹� źâ�� ���� á���ϴ�.");
            return false;
        }

        if (inventoryBulletCount <= 0)
        {
            Debug.Log("�κ��丮�� �Ѿ��� �����!");
            systemChannel.RaiseEvent(SystemEvents.SystemNotificationEvent.Init("ź�˺���", "�κ��丮�� ź���� �����ؿ�"));
            return false;
        }

        int reloadBulletCount = Mathf.Min(emptySpace, inventoryBulletCount);
        if (_gun.WeaponHandleModule is not EnemyWeaponHandleModule)
        {
            for (int i = 0; i < reloadBulletCount; i++)
                (_gun.WeaponHandleModule as PlayerGunHandleModule)
                    ?.InventoryContainer
                    .ConsumeBulletByName(_gun.WeaponData.BulletType.ItemName);
        }

        StartCoroutine(Reload(reloadBulletCount, OnReloadEnd));
        return true;
    }

    private IEnumerator Reload(int reloadBulletCount, Action onReloadEnd)
    {
        float currentTime = 0f;

        _loading = true;

        //if (bulletUI != null)
        //    bulletUI.gameObject.SetActive(false);

        //if (reloadUI != null)
        //{
        //    reloadUI.gameObject.SetActive(true);
        //    reloadUI.localRotation = Quaternion.identity;
        //}

        while (currentTime < _reloadDuration)
        {
            currentTime += Time.deltaTime;

            //if (reloadUI != null)
            //    reloadUI.Rotate(0f, 0f, -_rotationSpeed * Time.deltaTime);

            yield return null;
        }

        CurrentBulletCount += reloadBulletCount;
        CurrentBulletCount = Mathf.Clamp(CurrentBulletCount, 0, MaxBulletCount);

        //if (bulletUI != null)
        //    bulletUI.gameObject.SetActive(true);

        //if (reloadUI != null)
        //    reloadUI.gameObject.SetActive(false);

        _loading = false;
        RefreshUI();

        Debug.Log($"������ �Ϸ�: {reloadBulletCount}�� ����");
        onReloadEnd?.Invoke();
    }

    private void RefreshUI()
    {
        //if (bulletCountTxt != null)
        //    bulletCountTxt.text = $"{CurrentBulletCount}/{MaxBulletCount}";
    }
}