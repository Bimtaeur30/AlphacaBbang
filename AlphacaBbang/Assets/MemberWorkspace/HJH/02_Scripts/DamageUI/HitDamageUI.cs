using DG.Tweening;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HitDamageUI : MonoBehaviour
{
    [SerializeField] private DamageTextItem damageTextPrefab;
    [SerializeField] private Transform poolParent;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private Vector3 spawnLocalPos = Vector3.zero;
    [SerializeField] private EventChannelSO agentEventChannel;

    private Queue<DamageTextItem> pool = new Queue<DamageTextItem>();

    private void Awake()
    {
        InitPool();
    }

    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ShowDamage(Random.Range(10f, 999f));
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var item = Instantiate(damageTextPrefab, poolParent);
            item.SetStartPosition(spawnLocalPos);
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
        }
    }

    private void OnEnable()
    {
        agentEventChannel.AddListener<AgentHealthChangeEvent>(OnDamageReceived);
    }

    private void OnDisable()
    {
        agentEventChannel.RemoveListener<AgentHealthChangeEvent>(OnDamageReceived);
    }

    private void OnDamageReceived(AgentHealthChangeEvent e)
    {
        ShowDamage(e.Damage);
    }

    private void ShowDamage(float damage)
    {
        if (pool.Count == 0) return;

        var item = pool.Dequeue();
        item.Play(damage, () => pool.Enqueue(item));
    }
}