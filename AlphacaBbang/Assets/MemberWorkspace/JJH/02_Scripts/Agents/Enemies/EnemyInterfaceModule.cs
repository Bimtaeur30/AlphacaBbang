using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJH._02_Scripts.Agents.Enemies
{
    public class EnemyInterfaceModule : MonoBehaviour, IModule, IEnemyInterface
    {
        [SerializeField] private Image _enemyIcon;
        [SerializeField] private TextMeshPro _nameText;

        private AbstractEnemy _owner;

        public void Initialize(ModuleOwner owner)
        {
            _owner = (AbstractEnemy)owner;

            SetInterfaceShow(true);
            _nameText.text = _owner.EnemyData.EnemyName;
        }

        public void SetInterfaceShow(bool value)
        {
            _nameText.gameObject.SetActive(value);
            _enemyIcon.gameObject.SetActive(_owner.EnemyData.IsBoss && value);
        }
    }
}