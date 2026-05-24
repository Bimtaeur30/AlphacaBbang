using UnityEngine;

namespace Assets.MemberWorkspace.JJH._02_Scripts.Agents.Enemies
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField][Range(0, 100)] private int SpawnPercent = 100;
        [SerializeField] private GameObject[] EnemyPrefabs;
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
            if (Random.value > SpawnPercent / 100f)
                return;

            if (EnemyPrefabs == null || EnemyPrefabs.Length == 0)
            {
                Debug.LogWarning($"{name}에 EnemyPrefabs가 없습니다.");
                return;
            }

            int random = Random.Range(0, EnemyPrefabs.Length);
            GameObject enemyPrefab = EnemyPrefabs[random];
            Instantiate(enemyPrefab, _position, Quaternion.identity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, SpawnCheckDistance);
        }
    }
}