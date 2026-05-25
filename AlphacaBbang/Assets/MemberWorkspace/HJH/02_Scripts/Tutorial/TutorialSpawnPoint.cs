using JJH._02_Scripts.Systems.ObjectPoolSystems;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.MemberWorkspace.JJH._02_Scripts.Agents.Enemies
{

    public class TutorialSpawnPoint : MonoBehaviour
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO dummyPoolItem;
        [SerializeField] private LayerMask TargetLayer;
        [SerializeField] private float SpawnCheckDistance = 50f;
        [SerializeField] private float RespawnDelay = 3f;

        private Vector3 _position => transform.position;
        private bool _isSpawned = false;
        private Coroutine _respawnCoroutine;

        private void Update()
        {
            if (Physics.CheckSphere(transform.position, SpawnCheckDistance, TargetLayer) && !_isSpawned)
            {
                SpawnEnemy();
                _isSpawned = true;
            }
        }

        private void SpawnEnemy()
        {
            var dummy = poolManager.Pop<TutorialDummySetup>(dummyPoolItem);
            if (dummy == null)
            {
                Debug.LogWarning("Pool에서 더미를 가져오지 못했습니다.");
                return;
            }

            dummy.GameObject.transform.position = _position;
            dummy.OnDummyDead += OnDummyDead;
        }

        private void OnDummyDead()
        {
            _isSpawned = false;

            if (_respawnCoroutine != null)
                StopCoroutine(_respawnCoroutine);

            _respawnCoroutine = StartCoroutine(RespawnAfterDelay());
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(RespawnDelay);
            SpawnEnemy();
            _isSpawned = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, SpawnCheckDistance);
        }
    }
}