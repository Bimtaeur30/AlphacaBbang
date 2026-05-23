using Assets.MemberWorkspace.JJH._02_Scripts.Agents.Enemies;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies
{
    public class EnemySpawnPoints : MonoBehaviour
    {
        [SerializeField] private EnemySpawnPoint[] spawnPoints;

        private void Start()
        {
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            foreach (EnemySpawnPoint spawnPoint in spawnPoints)
            {
                if (spawnPoint.EnemyPrefabs == null || spawnPoint.EnemyPrefabs.Length == 0)
                {
                    Debug.LogWarning($"{spawnPoint.name}에 EnemyPrefabs가 없습니다.");
                    continue;
                }

                int random = Random.Range(0, spawnPoint.EnemyPrefabs.Length);
                GameObject enemyPrefab = spawnPoint.EnemyPrefabs[random];
                Instantiate(enemyPrefab, spawnPoint.Position, Quaternion.identity);
            }
        }
    }
}