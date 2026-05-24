using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.MemberWorkspace.JJH._02_Scripts.Agents.Enemies
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private EnemySpawnData[] EnemySpawnDatas;
        [SerializeField] private LayerMask TargetLayer;
        [SerializeField] private float SpawnCheckDistance = 50f;

        private Vector3 _position => transform.position;
        private bool _isSpawned = false;

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
            if (EnemySpawnDatas == null || EnemySpawnDatas.Length == 0)
            {
                Debug.LogWarning($"{name}에 EnemySpawnDatas가 없습니다.");
                return;
            }

            float totalPercent = 0f;
            for (int i = 0; i < EnemySpawnDatas.Length; i++)
            {
                totalPercent += EnemySpawnDatas[i].EnemySpawnPercent;
            }

            float random = Random.Range(0f, totalPercent);
            float currentPercent = 0f;

            for (int i = 0; i < EnemySpawnDatas.Length; i++)
            {
                currentPercent += EnemySpawnDatas[i].EnemySpawnPercent;

                if (random <= currentPercent)
                {
                    Instantiate(EnemySpawnDatas[i].EnemyPrefab, _position, Quaternion.identity);
                    return;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, SpawnCheckDistance);
        }
    }

    [Serializable]
    public struct EnemySpawnData
    {
        public GameObject EnemyPrefab;
        [Range(0f, 100f)] public float EnemySpawnPercent;
    }
}