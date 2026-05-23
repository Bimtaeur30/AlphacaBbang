using UnityEngine;

namespace Assets.MemberWorkspace.JJH._02_Scripts.Agents.Enemies
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        public Vector3 Position => transform.position;
        public GameObject[] EnemyPrefabs;
        [Range(0, 100)] public int SpawnPercent = 100;
    }
}