using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.ParticleSystems;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyDeadSkill : MonoBehaviour, IEnemySkill
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO deadBoomPref;
        [SerializeField] private Transform lootBoxSpawnTrans;

        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            EnemyDeadBoomParticle effect = poolManager.Pop<EnemyDeadBoomParticle>(deadBoomPref);
            effect.transform.position = _owner.transform.position;
            effect.PlayBoomParticle();

            GameObject lootBoxObject = Instantiate(_owner.EnemyData.EnemyInventoryPrefab, _owner.transform.position, Quaternion.identity);
            LootBox lootBox = lootBoxObject.GetComponent<LootBox>();
            lootBox.Init(_owner.LootTables[_owner.WeaponNum], "전리품");
            lootBoxObject.transform.position = lootBoxSpawnTrans.position;

            _owner.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            Destroy(_owner.gameObject);
        }
    }
}