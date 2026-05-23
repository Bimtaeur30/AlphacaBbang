using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemyFindSkill : MonoBehaviour, IEnemySkill
    {
        private AbstractEnemy _owner;

        public void Initialize(AbstractEnemy owner)
        {
            _owner = owner;
        }

        public void UseSkill()
        {
            _owner.EnemyInterface.EnemyTalk.ShowText();
            _owner.EnemySoundPlayer.PlaySound(_owner.EnemyData.FindSound);
        }
    }
}